using System.Collections.Generic;

namespace Import.Improved;

public record HeaderValidationContext
{
    public required List<string> AssetHeader { get; init; }

    public required List<string> RequiredAssetHeader { get; init; }

    public required List<string> SpecialAttributeTitles { get; init; }
}