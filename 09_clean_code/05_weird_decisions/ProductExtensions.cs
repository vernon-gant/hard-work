// BEFORE

public static class ProductExtensions
{
    /// <summary>
    ///     SKU
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributes">Attributes</param>
    /// <returns>SKU</returns>
    public static string FormatSku(this Product product, IList<CustomAttribute> attributes = null)
    {
        ArgumentNullException.ThrowIfNull(product);

        product.GetSkuMpnGtin(attributes, out var sku, out _, out _);

        return sku;
    }

    /// <summary>
    ///     MPN
    /// </summary>
    /// <param name="product">Product</param>
    /// <param name="attributes">Attributes</param>
    /// <returns>Collection part number</returns>
    public static string FormatMpn(this Product product, IList<CustomAttribute> attributes = null)
    {
        ArgumentNullException.ThrowIfNull(product);

        product.GetSkuMpnGtin(attributes, out _, out var Mpn, out _);

        return Mpn;
    }
}

// AFTER

Nothing