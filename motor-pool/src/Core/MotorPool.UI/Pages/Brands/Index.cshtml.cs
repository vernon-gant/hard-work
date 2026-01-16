using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MotorPool.Services.VehicleBrand.Models;
using MotorPool.Services.VehicleBrand.Services;

namespace MotorPool.UI.Pages.Brands;

[AllowAnonymous]
public class IndexModel(VehicleBrandService vehicleBrandService) : PageModel
{

    public List<VehicleBrandViewModel> VehicleBrand { get; set; } = default!;

    public async Task OnGetAsync()
    {
        VehicleBrand = await vehicleBrandService.GetAllAsync();
    }

}