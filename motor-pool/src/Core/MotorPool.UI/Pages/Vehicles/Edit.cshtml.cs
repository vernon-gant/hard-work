using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MotorPool.Domain;
using MotorPool.Repository.Vehicle;
using MotorPool.Services.VehicleBrand.Services;
using MotorPool.Services.Vehicles.Models;

namespace MotorPool.UI.Pages.Vehicles;

public class EditModel(VehicleChangeRepository vehicleChangeRepository,
    VehicleQueryRepository vehicleQueryRepository,
    VehicleBrandService vehicleBrandService, IMapper mapper) : VehicleBrandsSelectListPageModel
{

    [BindProperty]
    public VehicleViewModel VehicleViewModel { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int vehicleId)
    {
        await PopulateVehicleBrandsDropDownList(vehicleBrandService);

        Vehicle requestVehicle = HttpContext.Items["Vehicle"] as Vehicle ?? throw new InvalidOperationException();

        VehicleViewModel = mapper.Map<VehicleViewModel>(requestVehicle);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        Vehicle? foundVehicle = await vehicleQueryRepository.GetByIdAsync(VehicleViewModel.VehicleId);

        if (foundVehicle is null) return NotFound();

        foundVehicle = mapper.Map(VehicleViewModel, foundVehicle);

        await vehicleChangeRepository.UpdateAsync(foundVehicle);

        return RedirectToPage("./Index");
    }

}