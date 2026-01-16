using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MotorPool.Domain;
using MotorPool.Repository.Vehicle;
using MotorPool.Services.Vehicles.Models;

namespace MotorPool.UI.Pages.Vehicles;

public class DeleteModel(VehicleChangeRepository vehicleChangeRepository, IMapper mapper) : PageModel
{

    [BindProperty]
    public VehicleViewModel VehicleViewModel { get; set; } = default!;

    public Task<IActionResult> OnGetAsync(int vehicleId)
    {
        Vehicle requestVehicle = HttpContext.Items["Vehicle"] as Vehicle ?? throw new InvalidOperationException();

        VehicleViewModel = mapper.Map<VehicleViewModel>(requestVehicle);

        return Task.FromResult<IActionResult>(Page());
    }

    public async Task<IActionResult> OnPostAsync(int vehicleId)
    {
        await vehicleChangeRepository.DeleteAsync(vehicleId);

        string returnUrl = HttpContext.Session.GetString("VehicleIndexReturnUrl") ?? "/Vehicles/Index";

        return Redirect(returnUrl);
    }

}