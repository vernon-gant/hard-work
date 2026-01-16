using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MotorPool.Domain;
using MotorPool.Persistence;
using MotorPool.Persistence.QueryObjects;
using MotorPool.Repository.Vehicle;
using MotorPool.Services.Manager;
using MotorPool.Services.Vehicles.Models;
using MotorPool.UI.Pages.Shared;

namespace MotorPool.UI.Pages.Vehicles;

public class IndexModel(VehicleQueryRepository vehicleQueryRepository, IMapper mapper, ManagerPermissionService permissionService) : PagedModel
{

    public override int ELEMENTS_PER_PAGE => 10;

    public override int PAGES_DISPLAY_RANGE => 5;

    public List<VehicleViewModel> Vehicles { get; set; } = default!;

    public string? EnterpriseName { get; set; }

    public string? VehicleBrandSignature { get; set; }

    public async Task<IActionResult> OnGetAsync(VehicleQueryOptions queryOptions, Options options, string? enterpriseName)
    {
        PageOptions pageOptions = options.ToPageOptions(ELEMENTS_PER_PAGE);
        CurrentPage = pageOptions.CurrentPage;
        queryOptions.ManagerId ??= User.GetManagerId();
        EnterpriseName = enterpriseName;

        if (queryOptions.EnterpriseId.HasValue && !await permissionService.IsManagerAccessibleEnterprise(User.GetManagerId(), queryOptions.EnterpriseId.Value)) return Forbid();

        PagedResult<Vehicle> enterpriseVehiclesPagedResult = await vehicleQueryRepository.GetAllAsync(options.ToPageOptions(ELEMENTS_PER_PAGE), queryOptions);

        TotalPages = enterpriseVehiclesPagedResult.TotalPages;
        Vehicles = mapper.Map<List<VehicleViewModel>>(enterpriseVehiclesPagedResult.Elements);

        VehicleBrandSignature = queryOptions.VehicleBrandId.HasValue ? Vehicles.First().CompanyName + " " + Vehicles.First().ModelName : null;

        HttpContext.Session.SetString("VehicleIndexReturnUrl", Request.Path + Request.QueryString);

        return Page();
    }

}