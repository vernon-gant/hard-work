# Another import

I found another project where I again implemented import functionality.

```c#
public class XlsxImporter(IMapper mapper, ILogger<XlsxImporter> logger) : TourImporter(mapper)
{
    private IExcelDataReader _excelDataReader = null!;

    protected override OperationResult<List<TourExportModel>> ReadTours()
    {
        try
        {
            _excelDataReader = ExcelReaderFactory.CreateReader(_fileStream);

            _excelDataReader.Read();

            if (!ValidTourHeaders()) return OperationResult<List<TourExportModel>>.Error();

            List<TourExportModel> tourExportModels = new();
            while (_excelDataReader.Read())
            {
                tourExportModels.Add(new TourExportModel
                                     {
                                         TourNumber = (int)_excelDataReader.GetDouble(0),
                                         Description = _excelDataReader.GetString(1),
                                         Name = _excelDataReader.GetString(2),
                                         TransportType = GetTransportType(_excelDataReader.GetString(3)),
                                         Start = _excelDataReader.GetString(4),
                                         StartLatitude = (decimal)_excelDataReader.GetDouble(5),
                                         StartLongitude = (decimal)_excelDataReader.GetDouble(6),
                                         End = _excelDataReader.GetString(7),
                                         EndLatitude = (decimal)_excelDataReader.GetDouble(8),
                                         EndLongitude = (decimal)_excelDataReader.GetDouble(9),
                                         RouteGeometry = _excelDataReader.GetString(10),
                                         DistanceMeters = (decimal)_excelDataReader.GetDouble(11),
                                         EstimatedTime = (long)_excelDataReader.GetDouble(12),
                                         Popularity = GetPopularity(_excelDataReader.GetString(13)),
                                         ChildFriendliness = _excelDataReader.GetString(14)
                                     });
            }

            return OperationResult<List<TourExportModel>>.Ok(tourExportModels);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during tour import");
            return OperationResult<List<TourExportModel>>.Error();
        }
    }

    protected override OperationResult<List<TourLogExportModel>> ReadTourLogs()
    {
        try
        {
            _excelDataReader.NextResult();

            _excelDataReader.Read();

            List<TourLogExportModel> tourLogExportModels = new();
            while (_excelDataReader.Read())
            {
                tourLogExportModels.Add(new TourLogExportModel
                                        {
                                            TourNumber = (int)_excelDataReader.GetDouble(0),
                                            Comment = _excelDataReader.GetString(1),
                                            Difficulty = GetDifficulty(_excelDataReader.GetString(2)),
                                            TotalDistanceMeters = (decimal)_excelDataReader.GetDouble(3),
                                            TotalTime = (long)_excelDataReader.GetDouble(4),
                                            Rating = (short)_excelDataReader.GetDouble(5)
                                        });
            }

            return OperationResult<List<TourLogExportModel>>.Ok(tourLogExportModels);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during tour log import");
            return OperationResult<List<TourLogExportModel>>.Error();
        }
    }

    protected override bool ValidTourHeaders() => Enumerable.Range(0, ExportHeaders.TourHeaders.Count)
                                                            .Select(_excelDataReader.GetString)
                                                            .SequenceEqual(ExportHeaders.TourHeaders);

    protected override bool ValidTourLogHeaders() => Enumerable.Range(0, ExportHeaders.TourHeaders.Count)
                                                               .Select(_excelDataReader.GetString)
                                                               .SequenceEqual(ExportHeaders.TourHeaders);

    public override bool CanHandle(string format) => format == ExportImportFileFormats.XlsxFormat;
}
```

Although this is a bit less than 100 lines of code when I looked at this code before improving it the first and the biggest pain in my eyes were these type casts. Logically we firstly read data from the first sheet where tours are located(which is already flawed because we stick to a specific call sequence) and then we read the tour logs from next sheet. For some reason we also validate headers on the xlsx level.

This design is mad, I can not even describe it further. We mix things from different levels together. Taking any prorgamming task too naive leads to such code. After taking time and trying to come up with a logical solution of what we really want from import I came to this - one of the most important things during import is to make sure that the imorted data can be properly deserialize raw data into correct types(even easy DTOs) where these types can have fields/properties represented by other complex types. For example DateTime which we want to accept only in specific format, currency, float, enums or any other complex data type which can be parsed from string using some special parsing method which will differ from type to type. Then we must also validate the input, however in this case I want to concentrate on the declarative way of building such parsing.

The first things we do - we abstract away the way we think about import. Concrete import formats - stream, xlsx, csv or even xml are just our providers of data. We do not have to worry about what data sits there - if it is valid or not, if types are ok and so on. We simply return list of list of strings - how easy right? For any type of import on the lowest level we only care about retrieving the data.

```c#
public interface ImportDataProvider
{
    OneOf<List<List<string>>, ReadingFailed> GetData(Stream stream);

    bool CanHandle(string format);
}

public struct ReadingFailed;
```

After we read everything, we want to deserialize data with proper types. However we want to have control of what parsers parse which fields. We want in other words to build the "parsing process" so that we compose and change later the whole entity parsing if needed. Even the parsing itself we can divide as just value mapping and prouding the actual entity - the final goal of deserialization. On the lowest level - say raw level we just work with conversion of string to any other type. On the leve above we set what was converted or ignore in case of error and finally return the entity. We also want to have some sort of an EntityParser and its builder which could also validate us if we messed up the building process - for example we did not register all the properties. By design we also make a desicion that each such builder must take expected header. By deisng we also make a decision that registration of these handlers must be in order fields in which headers appear - any discrepancies are returned as error. At the end the build parser takes a list of strings which corresponds to a row from the import and returns us a strongly typed object or an error. These are logical design decisions - we do not think for now about the code. But when we start thinking - code just flows(okay witha bit of research on compiled expressions - i had no idea about them).

```c#
public interface RawParser<T>
{
    OneOf<T, ParsingFailed> Parse(string value);

    string ErrorMessage { get; }
}

public struct ParsingFailed;

// Example

public class TransportTypeParser : RawParser<TransportType>
{
    private const string ErrorMessageConst = "Invalid transport type. Allowed values: Foot, Car, Truck, Bicycle, Bike.";

    public OneOf<TransportType, ParsingFailed> Parse(string value)
    {
        if (Enum.TryParse<TransportType>(value, true, out var result))
        {
            return result;
        }

        return new ParsingFailed();
    }

    public string ErrorMessage => ErrorMessageConst;
}
```

This is a simple enum example - however we could imagine any type on its place. Then we have the actual parsing and setting - ADT above comes as dependency

```c#
public interface IPropertyParser<TEntity>
{
    OneOf<Success, Error<string>> Set(TEntity entity, string rawValue);
}

public class PropertyParser<TEntity, TProp> : IPropertyParser<TEntity>
{
    private readonly RawParser<TProp> _rawParser;
    private readonly Action<TEntity, TProp> _setter;

    public PropertyParser(Expression<Func<TEntity, TProp>> propertySelector, RawParser<TProp> rawParser)
    {
        if (propertySelector.Body is not MemberExpression { Member: PropertyInfo prop })
            throw new ArgumentException("Expression must select a property.");

        _rawParser = rawParser;
        _setter = BuildSetter(prop);
    }

    public OneOf<Success, Error<string>> Set(TEntity entity, string rawValue)
    {
        return _rawParser.Parse(rawValue)
                           .Match<OneOf<Success,Error<string>>>(parsedValue =>
                            {
                                _setter(entity, parsedValue);
                                return new Success();
                            }, _ => new Error<string>(_rawParser.ErrorMessage));
    }

    private static Action<TEntity, TProp> BuildSetter(PropertyInfo prop)
    {
        var entityParam = Expression.Parameter(typeof(TEntity));
        var valueParam = Expression.Parameter(typeof(TProp));
        var setExpr = Expression.Lambda<Action<TEntity, TProp>>(
            Expression.Assign(Expression.Property(entityParam, prop), valueParam), entityParam, valueParam);
        return setExpr.Compile();
    }
}
```

and we go up to the last level - on the entity level

```c#
using System.Diagnostics;
using OneOf;
using OneOf.Types;

namespace TP.Import;

public class EntityParser<TEntity> where TEntity : new()
{
    private readonly List<IPropertyParser<TEntity>> _propertyParsers;
    private readonly List<string> _header;

    internal EntityParser(List<IPropertyParser<TEntity>> propertyParsers, List<string> header)
    {
        Debug.Assert(header.Count == propertyParsers.Count);

        _propertyParsers = propertyParsers;
        _header = header;
    }

    public OneOf<TEntity, SizeMismatch, Error<List<string>>> Parse(List<string> row)
    {
        if (row.Count != _header.Count)
            return new SizeMismatch();

        var entity = new TEntity();
        var errors = new List<string>();

        for (int i = 0; i < _propertyParsers.Count; i++)
        {
            var parseResult = _propertyParsers[i].Set(entity, row[i]);

            if (parseResult.IsT1)
                errors.Add($"{_header[i]} could not be parsed - {parseResult.AsT1.Value}");
        }

        return errors.Count == 0 ? entity : new Error<List<string>>(errors);
    }
}

public struct SizeMismatch;
```

and finally what makes real difference and makes the whole approach configurable, descriptive and declarative - builder. Which requires a bit of C# knowledge.

```c#
public class EntityParserBuilder<TEntity>(List<string> header) where TEntity : new()
{
    private readonly Dictionary<string, object> _parsers = new();

    public EntityParserBuilder<TEntity> With<TProp, TParser>(Expression<Func<TEntity, TProp>> propertySelector) where TParser : RawParser<TProp>, new()
    {
        var propName = GetPropertyName(propertySelector);

        if (_parsers.ContainsKey(propName))
            throw new InvalidOperationException($"Parser already registered for property {propName}");

        _parsers[propName] = new PropertyParser<TEntity, TProp>(propertySelector, new TParser());

        return this;
    }

    public EntityParser<TEntity> Build()
    {
        var allPublicProperties = typeof(TEntity)
                                 .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Select(p => p.Name)
                                 .ToHashSet();

        var keysSet = _parsers.Keys.ToHashSet();

        if (keysSet.SetEquals(allPublicProperties))
            return new EntityParser<TEntity>(_parsers.Values.Cast<IPropertyParser<TEntity>>().ToList(), header);

        var missing = allPublicProperties.Except(_parsers.Keys).ToList();
        var extra = keysSet.Except(allPublicProperties).ToList();

        var message = "Parser setup mismatch:";
        if (missing.Count != 0)
            message += $" Missing parsers for properties: {string.Join(", ", missing)}.";

        if (extra.Count != 0)
            message += $" Parsers registered for non-existing properties: {string.Join(", ", extra)}.";

        throw new InvalidOperationException(message);
    }

    private static string GetPropertyName<TProp>(Expression<Func<TEntity, TProp>> propertySelector)
    {
        if (propertySelector.Body is MemberExpression memberExpr && memberExpr.Member is PropertyInfo prop)
            return prop.Name;

        throw new ArgumentException("Expression must select a property.");
    }
}
```

however the most important is what it gives us

```c#
var tourDTOBuildingResult = new EntityParserBuilder<TourDTO>(["Name", "Description", "Price", "Duration", "Location"])
    .With<string, StringParser>(t => t.Name)
    .With<string, StringParser>(t => t.Description)
    .With<TransportType, TransportTypeParser>(t => t.TransportType)
    .With<string, StringParser>(t => t.Start)
    .With<string, StringParser>(t => t.End)
    .Build();
```

I really learned a lot in terms of thinking on the logical level here - it was hard, because requires treating all small details as important and thinking about them in terms of abstractions. In this case our design - that we want to control building of our desired entity parser - is followed by the code 1 to 1. We simply put these concepts into the code. Nothing more.

This was the first iteration, so it took impressive 8 hours, because I tried to make myself think on the third level and had no idea about some C# internals.