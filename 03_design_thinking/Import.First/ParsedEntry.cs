using System.Collections.Generic;

namespace Import.First;

public record ParsedEntry(int RowNumber, ParsedAsset Asset, List<ParsedSpecialAttribute> SpecialAttributes);

public record ParsedSpecialAttribute(string Title, string Value);