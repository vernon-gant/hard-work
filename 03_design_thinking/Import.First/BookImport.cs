using System.Collections.Generic;

namespace Import.First;

public class BookImport(StringedImport stringedImport) : AssetImport(stringedImport)
{
    protected override int SpecialAssetPropertiesNumber() => 6;

    protected override ParsedAsset ParseAsset(int rowNumber, List<string> assetContent) =>
        new ParsedBook
        {
            Title = assetContent[0],
            Category = assetContent[1],
            Condition = assetContent[2],
            Author = assetContent[3],
            ISBN = assetContent[4],
            Publisher = assetContent[5],
            YearPublished = assetContent[6],
            Price = ParseDouble(rowNumber, assetContent[7], "Price"),
            Rating = ParseDouble(rowNumber, assetContent[8], "Rating")
        };
}