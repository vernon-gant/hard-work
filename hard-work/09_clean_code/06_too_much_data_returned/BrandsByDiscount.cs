// BEFORE

public virtual async Task<IList<Brand>> GetAllBrandsByDiscount(string discountId)
{
    var query = from c in _brandRepository.Table where c.AppliedDiscounts.Any(x => x == discountId)select c;
    
    return await Task.FromResult(query.ToList());
}

[PermissionAuthorizeAction(PermissionActionName.Preview)]
[HttpPost]
public async Task<IActionResult> BrandList(DataSourceRequest command, string discountId, [FromServices] IBrandService brandService)
{
    var discount = await _discountService.GetDiscountById(discountId);
    if (discount == null)
        throw new Exception("No discount found with the specified id");

    var brands = await brandService.GetAllBrandsByDiscount(discount.Id);
    var gridModel = new DataSourceResult {
        Data = brands.Select(x => new DiscountModel.AppliedToBrandModel {
            BrandId = x.Id,
            BrandName = x.Name
        }),
        Total = brands.Count
    };

    return Json(gridModel);
}

// AFTER

public virtual async Task<IList<(string Id, string Name)>> GetAllBrandsByDiscount(string discountId)
{
    var query = from c in _brandRepository.Table where c.AppliedDiscounts.Any(x => x == discountId) select c;

    return await Task.FromResult(query.Select(x => new ValueTuple<string, string>(x.Id, x.Name)).ToList());
}