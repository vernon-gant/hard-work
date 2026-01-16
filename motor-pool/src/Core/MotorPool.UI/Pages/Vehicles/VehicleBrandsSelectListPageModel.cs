using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MotorPool.Services.VehicleBrand.Models;
using MotorPool.Services.VehicleBrand.Services;

namespace MotorPool.UI.Pages.Vehicles;

public class VehicleBrandsSelectListPageModel : PageModel
{

    public SelectList VehicleBrandSelectList { get; set; } = new (new List<VehicleBrandSignatureWithId>());

    public async Task PopulateVehicleBrandsDropDownList(VehicleBrandService vehicleBrandService, object? selectedVehicleBrand = null)
    {
        var vehicleBrandSignatureWithIds = await vehicleBrandService.GetVehicleBrandsWithIdAsync();

        VehicleBrandSelectList = new SelectList(vehicleBrandSignatureWithIds, nameof(VehicleBrandSignatureWithId.VehicleBrandId),
                                                nameof(VehicleBrandSignatureWithId.BrandSignature), selectedVehicleBrand);
    }

}