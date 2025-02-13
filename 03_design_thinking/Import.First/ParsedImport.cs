using System.Collections.Generic;

namespace Import.First;

public record ParsedImport(
    List<string> AssetHeaders,
    List<string> SpecialAttributeTitles,
    List<string> AssetCategoryTitles,
    List<string> AssetConditionTitles,
    List<ParsedEntry> ParsedEntries
);