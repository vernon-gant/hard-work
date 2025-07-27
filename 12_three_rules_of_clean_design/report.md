# 1. Eliminate error prone behavior on the interface level

## Deadlock and unpredicted behavior

Taken from my university project from the 1st semester.

```c#
using MTCG.Services.BattleServices.Battle;

public class BattleLobby
{

    private BattleRequest? _waitingBattleRequest;

    private ManualResetEventSlim _battleReadyEvent = new (false);

    public BattleRequest? GetEnemy(BattleRequest battleRequest)
    {
        lock (this)
        {
            if (_waitingBattleRequest == null)
            {
                _waitingBattleRequest = battleRequest;

                return null;
            }

            BattleRequest waitingRequest = _waitingBattleRequest;

            _waitingBattleRequest = null;

            _battleReadyEvent.Set();

            return waitingRequest;
        }
    }

    public void WaitForOpponent()
    {
        _battleReadyEvent.Wait();
    }

}
```

The idea of the project was that there is a card trading game and that 2 players connect to the server(using HTTP) and make a battle request. The first one connects and waits and then the second one connects and the battle is performed. Here the design of the class makes it error prone to situations where we might call the wait for opponent twice and this will lead to a deadlock and unpredicted behavior afterwards. The logic is clear - first one connects and waits for results where the second player connects, takes the request from the first one, exeutes battle and saves the result so that other player can obtain it. After thinking on the third level this leads us to a very straightforward design even without rethinking and rewriting the whole app completely

```c#
public class BattleLobby
{
    private WaitingRequest? _waitingBattleRequest;
    private readonly BattleFactory _battleFactory;

    public BattleLobby(BattleFactory battleFactory)
    {
        _battleFactory = battleFactory;
    }

    public Battle GetBattle(BattleRequest battleRequest)
    {
        lock (this)
        {
            if (_waitingBattleRequest == null)
            {
                _waitingBattleRequest = new WaitingRequest(battleRequest);

                return _battleFactory.WaitForResults(_waitingBattleRequest);
            }

            var waitingRequest = _waitingBattleRequest;

            _waitingBattleRequest = null;

            return _battleFactory.PerformBattle(battleRequest, waitingRequest);
        }
    }
}

public delegate Task<BattleResult> Battle();

public class BattleFactory
{
    private readonly BattleArena _battleArena;
    private readonly BattleResultsStorage _battleResultsStorage;

    public BattleFactory(BattleArena battleArena, BattleResultsStorage battleResultsStorage)
    {
        _battleArena = battleArena;
        _battleResultsStorage = battleResultsStorage;
    }

    public Battle WaitForResults(WaitingRequest battleRequest)
    {
        return async () => await _battleResultsStorage.GetBattleResultAsync(battleRequest);
    }

    public Battle PerformBattle(BattleRequest request, WaitingRequest opponentRequest)
    {
        return () =>
        {
            var result = _battleArena.Battle(request, opponentRequest.Request);
            _battleResultsStorage.StoreBattleResult(opponentRequest, result);
            return Task.FromResult(result);
        };
    }
}

public class BattleRequest
{
    public required User User { get; set; }

    public required Deck Deck { get; set; }
}

public record WaitingRequest(BattleRequest Request);
```

With small type adjustments we prevented from incorrect usage of the BattleLobby and now it will always complete succsefully just by reworking the logic and making it only responsible for calling appropriate factory method to produce either a battle waiting scenario and then retrieving the results or battle performing and writing the results for the waiting opponent.

## Init methods are evil

The easiest example of error prone interface are interfaces with init method or when we need to do something before some operation only MAY be performed. This example is from the same project

```c#
public abstract class ReportGenerator
{
    protected string _filePath = string.Empty;

    public void SetFilePath(string filePath)
    {
        _filePath = filePath;
    }

    public abstract void Init();

    public abstract void Generate(SingleTourReport report);

    public abstract void Generate(TourSummaryReport report);
}

public class PdfReportGenerator : ReportGenerator
{
    private PdfWriter _writer = null!;

    private PdfDocument _entryDocument = null!;

    public override void Init()
    {
        _writer = new PdfWriter(_filePath);
        _entryDocument = new PdfDocument(_writer);
    }

    public override void Generate(SingleTourReport report)
    {
        using Document document = new(_entryDocument);
        document.Add(new Paragraph("Tour Report").SetFontSize(20)
                                                 .SetBold()
                                                 .SetTextAlignment(TextAlignment.CENTER)
                                                 .SetMarginTop(20)
                                                 .SetMarginBottom(20));

        ...

        _entryDocument.Close();
        _writer.Close();
    }

    public override void Generate(TourSummaryReport report)
    {
        using Document document = new(_entryDocument);
        document.Add(new Paragraph("Tour Summary Report").SetFontSize(20)
                                                         .SetBold()
                                                         .SetTextAlignment(TextAlignment.CENTER)
                                                         .SetMarginTop(20)
                                                         .SetMarginBottom(20));

        ...

        _entryDocument.Close();
        _writer.Close();
    }
}
```

If we do not call Init before any of these methods then we will get an exception. WPF uses as far as I remember some components which need initializaton, however this is a completely different thing - if something wants to be loaded in order to work then it implements this interface and get loaded. But this comes as an explicit interface. Here, however, I did not know about the IDisposable and crafted this masterpiece. So just remove the Init method and also add a constructor which accepts report path and not a `SetFilePath` method in the abstract parent class. So finally

```c#
public interface ReportGenerator
{
    void Generate(SingleTourReport report);

    void Generate(TourSummaryReport report);
}

public class PdfReportGenerator : ReportGenerator
{
    private ReportPath _reportPath;

    public PdfReportGenerator(IOptions<ReportPath> reportPathOptions)
    {
        _reportPath = reportPathOptions.Value;
    }

    public void Generate(SingleTourReport report)
    {
        using var pdfWriter = new PdfWriter(_reportPath.Value);
        using var pdfDocument = new PdfDocument(pdfWriter);
        using Document document = new(pdfDocument);

        ...
    }

    ...
}
```

now all pdf related things are disposed after usage, we do not need to have some state and interface looks clear and we can not get to an invalid state.

# 2. No default constructors

## Can produce unexpected behavior

In the example below we have an option to create a filestore using the current directoy in the default constructor. This is very error prone, because depending on the context of the task we might want only be able to write to some directories. Say if this is a ecommerce app, writing to the directory where the dll is located might be not the best idea.

```c#
public class FileSystemStore : IFileStore
{
    private readonly string _fileSystemPath;

    public FileSystemStore()
    {
        _fileSystemPath = Path.GetFullPath(Directory.GetCurrentDirectory());
    }

    public FileSystemStore(string fileSystemPath)
    {
        _fileSystemPath = Path.GetFullPath(fileSystemPath);
    }

    public Task<IFileStoreEntry> GetFileInfo(string path)
    {
        var physicalPath = GetPhysicalPath(path);

        var fileInfo = new PhysicalFileInfo(new FileInfo(physicalPath));

        return Task.FromResult<IFileStoreEntry>(fileInfo.Exists ? new FileSystemStoreEntry(path, fileInfo) : null);
    }

    public IFileStoreEntry GetDirectoryInfo(string path)
    {
        var physicalPath = GetPhysicalPath(path);

        var directoryInfo = new PhysicalDirectoryInfo(new DirectoryInfo(physicalPath));

        return directoryInfo.Exists ? new FileSystemStoreEntry(path, directoryInfo) : null;
    }

    ...
}
```

so to prevent this unexpected behavior we delete this possibility completely and we could even create a type which would represent an allowed path for storing something in the app.

## Add more control

```c#
public class MongoDBStartupBase : IStartupBase
{
    public int Priority => 0;

    /// <summary>
    ///     Register MongoDB mappings
    /// </summary>
    public void Execute()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));
        BsonSerializer.RegisterSerializer(typeof(Dictionary<int, int>),
            new DictionaryInterfaceImplementerSerializer<Dictionary<int, int>>(DictionaryRepresentation.ArrayOfArrays));

        //global set an equivalent of [BsonIgnoreExtraElements] for every Domain Model
        var cp = new ConventionPack {
            new IgnoreExtraElementsConvention(true),
        };
        ConventionRegistry.Register("ApplicationConventions", cp, t => true);

        BsonClassMap.RegisterClassMap<Download>(cm =>
        {
            cm.AutoMap();
            cm.UnmapMember(m => m.DownloadBinary);
        });
    }
}
```

In the example above there is an implicit default constructor which initilizes the priority to 0. In some cases, however, we might want to change the priority. In that setup it is impossible. Moreover it makes sense to pass the control of setting the priority to the client and not to the object itself.

# 3. No primitive obession

## Make intent more explicit

https://github.com/vernon-gant/grandnode2/blob/main/src/Business/Grand.Business.Common/Services/Pdf/HtmlToPdfService.cs#L71

I deleted the Argument null checks because in modern versions of C# we can enable the null passings warning to indicate an error and catch these errors at compile time. The major improvements on the interface level are focused on the underlying logic of the method. We do not want to print the packaging slip for no orders so a NonEmptyList which I use myself in my projects and at work guarantees that there is at least one shipment. We express the expectation on a type level because we know that the NonEmptyList exists in runtime only as a non empty collectio of elements. The we also require the client to provide the language and not the language id which can reference an unexistin language or whatever. Again on the interface level we know that language represents a valid language in the system.

Then we also remove the Stream from the argument list because this is a very dangerous part and one can pass everything he wants. Ideally it must be decided how big the files can be and whether we should return a stream or just an array of bytes.

 As a result using non primitive types and reworking the expectations we cut off at least 3 invalid uses cases of the method. Now (okay with this type system not but on the logical level of these types) we are sure that we have a non empty list of valid shipments and a valid language which exists in the system. Way stronger!

 ```c#
 public record struct NonEmptyList<T>
{
    public IReadOnlyList<T> Value { get; }

    public NonEmptyList(IList<T> value)
    {
        if (value == null || value.Count == 0)
            throw new ArgumentException("Value cannot be null or empty", nameof(value));

        Value = value.AsReadOnly();
    }
}

public async Task<OneOf<byte[], Error> ProducePdfPackageSlip(NonEmptyList<Shipment> shipments, Language language)
{
    try
    {
        var html = await _viewRenderService.RenderToStringAsync(ShipmentsTemplate, shipments);
        TextReader reader = new StringReader(html);
        using var doc = Document.ParseDocument(reader, ParseSourceType.DynamicContent);
        var memoryStream = new MemoryStream();
        doc.SaveAsPDF(memoryStream);
        return memoryStream.ToArray();
    } catch (Exception e)
    {
        _logger.LogError("Failed to create a pdf package slip", e);
        return new Error();
    }
}
 ```

## Closer to domain

I found a very interesting case which showed to me that yeah the C# type system unfortunately not always allows us to guarantee a correct usage on the interface level if we really want.

```
public interface IFillableGrid<TElement>
{
    // Commands

    /// <remarks>Precondition: The column is not full.</remarks>
    /// <remarks>Postcondition: The elements in the column are shifted down by one row, and the topmost cell is empty.</remarks>
    OneOf<Success, ColumnIndexOutOfBounds, CanNotShiftDown> ShiftDown(int columnIndex);

    /// <remarks>Precondition: The column is not full.</remarks>
    /// <remarks>Postcondition: The element is added to the top of the column.</remarks>
    OneOf<Success, ColumnIndexOutOfBounds, CanNotAddTop> AddTop(int columnIndex, TElement element);


    // Queries

    OneOf<bool, ColumnIndexOutOfBounds> IsColumnFull(int columnIndex);

    OneOf<bool, ColumnIndexOutOfBounds> CanShiftDown(int columnIndex);

    List<int> FillableColumns { get; }
}

public struct CanNotShiftDown;

public struct CanNotAddTop;

public struct ColumnIndexOutOfBounds;
```

Although all potential error cases are caught on the interface level we can eliminate them by adding some types instead of primitives : we add row and column types which are coming from the grid itself and are always valid because they are produced by the grid itself and nowhere else. We must not avoid but prohibit on the type level any manual instantiation of rows and columns on the type level. This can be done using interfaces and private classes.

```c#
public interface IRow<TGrid> : IEquatable<IRow<TGrid>>
{
    int Value { get; }
}

public interface IColumn<TGrid> : IEquatable<IColumn<TGrid>>
{
    int Value { get; }
}

public interface IReadableGrid<TElement, TGrid> : IEnumerable<Cell<TElement, TGrid>> where TElement : notnull where TGrid : notnull
{
    OneOf<Cell<TElement, TGrid>, OutOfBounds> CellAt(int row, int column);

    OneOf<IRow<TGrid>, OutOfBounds> RowAt(int row);

    OneOf<IColumn<TGrid>, OutOfBounds> ColumnAt(int column);
}

public struct OutOfBounds;

public class MatrixGrid<TElement, TGridId> : IReadableGrid<TElement, TGridId> where TElement : notnull where TGridId : notnull
{
    private readonly CellContent<TElement>[,] _grid;

    public IEnumerator<Cell<TElement, TGridId>> GetEnumerator()
    {
        for (var row = 0; row < _grid.GetLength(0); row++)
        {
            for (var column = 0; column < _grid.GetLength(1); column++)
            {
                yield return new Cell<TElement, TGridId>(_grid[row, column], new MatrixRow { Value = row }, new MatrixColumn { Value = column });
            }
        }
    }

    private class MatrixRow : IRow<TGridId>
    {
        public int Value { get; init; }

        public bool Equals(IRow<TGridId>? other)
        {
            throw new NotImplementedException();
        }
    }

    private class MatrixColumn : IColumn<TGridId>
    {
        public int Value { get; init; }

        public bool Equals(IColumn<TGridId>? other)
        {
            throw new NotImplementedException();
        }
    }
}
```

and finally the updated fillable grid

```
public interface IFillableGrid<TElement, TGrid> : IReadableGrid<TElement, TGrid> where TElement : notnull where TGrid : notnull
{
    // Commands
    OneOf<Success, CanNotShiftDown> ShiftDown(IColumn<TGrid> column);

    OneOf<Success, CanNotAddTop> AddTop(IColumn<TGrid> column, TElement element;


    // Queries

    bool IsColumnFull(IColumn<TGrid> column);

    bool CanShiftDown(IColumn<TGrid> column);

    List<IColumn<TGrid>> FillableColumns { get; }
}

public struct CanNotShiftDown;

public struct CanNotAddTop;
```

now should look good - our rows and columns have a coupling to a readable grid which in turn produces these rows. To use the C# type system to 100% we even add a grid ID type parameter so that

```
var grid1 = new MatrixGrid<string, Grid1>();
var grid2 = new MatrixGrid<string, Grid2>();

// IRow<Grid1> and IRow<Grid2> are now incompatible
```

and on the interface level we can not pass row from one grid to another grid. We have modelled the interface more closer to the domain by adding types related to grid. But here is the problem - two different instances of `MatrixGrid<string, Grid1>`with different dimensions like 5x5 and 10x10 can still accept rows and columns from each other. With C# type system, however, this can not be really fixed - only adding identities to each grid aka guid and storing them in the rows and columns and then checking in say a wrapper `IdentityGrid` using "proxy pattern" that the accepted row or column are coming from the grid which issued these rows and columns. These are limitations which we need to take into account. At least we can not pass -10000 :) Maybe an overkill for this simple example, however these "this type owns this" problem can be met in many other domains.