namespace Import.First;

public class ParsedBook : ParsedAsset
{
    public required string? Author { get; init; }

    public required string? ISBN { get; init; }

    public required string? Publisher { get; init; }

    public required string? YearPublished { get; init; }

    public required double? Price { get; init; }

    public required double? Rating { get; init; }
}