using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using MotorPool.Services.VehicleBrand.Models;
using MotorPool.Services.VehicleBrand.Services;

namespace MotorPool.UI.Pages.Brands;

public class CreateModel(VehicleBrandService vehicleBrandService) : PageModel
{

    [BindProperty]
    public VehicleBrandDTO VehicleBrandDto { get; set; } = default!;

    public IActionResult OnGet()
    {
        return Page();
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        await vehicleBrandService.CreateAsync(VehicleBrandDto);

        return RedirectToPage("./Index");
    }

}