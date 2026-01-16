using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MotorPool.Domain;
using MotorPool.Repository.Vehicle;
using MotorPool.Services.VehicleBrand.Services;
using MotorPool.Services.Vehicles.Models;

namespace MotorPool.UI.Pages.Vehicles;

public class CreateModel(VehicleChangeRepository vehicleChangeRepository, VehicleBrandService vehicleBrandService, IMapper mapper) : VehicleBrandsSelectListPageModel
{

    [BindProperty]
    public VehicleDTO VehicleDto { get; set; } = default!;

    public async Task<IActionResult> OnGet()
    {
        await PopulateVehicleBrandsDropDownList(vehicleBrandService);
        return Page();
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        await vehicleChangeRepository.CreateAsync(mapper.Map<Vehicle>(VehicleDto));

        return RedirectToPage("./Index");
    }

}