using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using OneOf;

namespace Import.First;

public abstract class AssetImport
{
    private const int CommonAssetPropertiesNumber = 3;
    private const int AssetCategoryIdx = 1;
    private const int AssetConditionIdx = 2;

    private readonly List<List<string>> _entries;
    private readonly List<string> _headers;
    private readonly ConcurrentDictionary<int, ImportRowError> _parsingErrors = new();
    private readonly List<string> _specialAttributeTitles;

    protected AssetImport(StringedImport stringedImport)
    {
        _headers = stringedImport.Headers;

        if (_headers.Count < MinimalRequiredHeadersNumber)
            throw new InvalidOperationException("Not all asset headers are set. Check the example file.");

        _entries = stringedImport.Entries;

        var entriesWithLessValuesThanHeaders = _entries.Select((entry, idx) => (entry, idx)).Where(entryIdx => entryIdx.entry.Count < _headers.Count).Select(entryIdx => entryIdx.idx + 2).ToList();

        if (entriesWithLessValuesThanHeaders.Count != 0)
            throw new InvalidOperationException($"Entries have missing values according to headers: {JsonSerializer.Serialize(entriesWithLessValuesThanHeaders)}");

        _specialAttributeTitles = _headers.Skip(MinimalRequiredHeadersNumber).Take(_headers.Count).ToList();
    }

    private int MinimalRequiredHeadersNumber => CommonAssetPropertiesNumber + SpecialAssetPropertiesNumber();
    private List<string> AssetHeaders => _headers.Take(MinimalRequiredHeadersNumber).ToList();
    private List<string> AssetCategoryTitles => _entries.Select(entry => entry[AssetCategoryIdx]).ToList();
    private List<string> AssetConditionTitles => _entries.Select(entry => entry[AssetConditionIdx]).ToList();

    private IEnumerable<int> SpecialAttributesIndices =>
        Enumerable.Range(CommonAssetPropertiesNumber + SpecialAssetPropertiesNumber(), _headers.Count - CommonAssetPropertiesNumber - SpecialAssetPropertiesNumber());

    public OneOf<ParsedImport, List<ImportRowError>> Parse()
    {
        var parsedEntries = _entries.AsParallel().Select((entry, idx) => new ParsedEntry(idx + 2, ParseAsset(idx + 2, entry), ParseSpecialAttributes(idx + 2, entry))).OrderBy(entry => entry.RowNumber).ToList();

        var parsingSuccess = _parsingErrors.Count == 0;
        var parsedImport = new ParsedImport(AssetHeaders, _specialAttributeTitles, AssetCategoryTitles, AssetConditionTitles, parsedEntries);

        return parsingSuccess ? parsedImport : _parsingErrors.Values.ToList();
    }

    protected abstract int SpecialAssetPropertiesNumber();
    protected abstract ParsedAsset ParseAsset(int rowNumber, List<string> assetContent);

    protected double? ParseDouble(int rowNumber, string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var formatInfo = new NumberFormatInfo { NegativeSign = "−" };

        if (double.TryParse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, formatInfo, out var result))
            return result;

        AddError(rowNumber, $"{fieldName} on row {rowNumber} is not a valid floating point number.");
        return null;
    }

    private List<ParsedSpecialAttribute> ParseSpecialAttributes(int rowNumber, List<string> entry)
    {
        if (SpecialAttributesIndices.Count() == entry.Count - CommonAssetPropertiesNumber - SpecialAssetPropertiesNumber())
            return SpecialAttributesIndices.Select(idx => (AttributeTitleIdx: idx - CommonAssetPropertiesNumber - SpecialAssetPropertiesNumber(), Value: entry[idx]))
                                           .Where(tuple => !string.IsNullOrWhiteSpace(tuple.Value))
                                           .Select(tuple => new ParsedSpecialAttribute(_specialAttributeTitles[tuple.AttributeTitleIdx], tuple.Value))
                                           .ToList();

        AddError(rowNumber, $"Entry on row {rowNumber} has an inconsistent number of special attributes.");
        return [];
    }

    private void AddError(int rowNumber, string errorMessage)
    {
        if (!_parsingErrors.TryGetValue(rowNumber, out var parsingError))
        {
            parsingError = new ImportRowError { Row = rowNumber };
            _parsingErrors.TryAdd(rowNumber, parsingError);
        }

        parsingError.ErrorMessages.Add(errorMessage);
    }
}