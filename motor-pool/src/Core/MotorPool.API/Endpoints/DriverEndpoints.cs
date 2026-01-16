using System.Security.Claims;
using AutoMapper;
using MotorPool.Domain;
using MotorPool.Persistence;
using MotorPool.Persistence.QueryObjects;
using MotorPool.Repository.Driver;
using MotorPool.Services.Drivers;
using MotorPool.Services.Drivers.Models;
using MotorPool.Services.Manager;

namespace MotorPool.API.Endpoints;

public static class DriverEndpoints
{
    public static void MapDriverEndpoints(this IEndpointRouteBuilder managerResourcesGroupBuilder)
    {
        RouteGroupBuilder driversGroupBuilder = managerResourcesGroupBuilder.MapGroup("drivers")
                                                                            .WithParameterValidation();

        driversGroupBuilder.MapGet("", GetAll)
                           .WithName("GetAllDrivers")
                           .Produces<List<DriverViewModel>>();

        driversGroupBuilder.MapPost("{driverId}/{vehicleId}", Assign)
                           .WithName("AssignDriver")
                           .Produces<DriverViewModel>();
    }

    private static async Task<IResult> GetAll(DriverQueryRepository driverQueryRepository, ClaimsPrincipal principal, [AsParameters] Options options, IMapper mapper)
    {
        int managerId = principal.GetManagerId();

        PagedResult<Driver> drivers = await driverQueryRepository.GetAllAsync(managerId, options.ToPageOptions());

        return Results.Ok(mapper.Map<List<DriverViewModel>>(drivers.Elements));
    }

    private static async Task<IResult> Assign(AssignmentTransactionHandler transactionHandler, ClaimsPrincipal principal, int driverId, int vehicleId)
    {
        DriverVehicle assignment = new DriverVehicle
                                   {
                                       DriverId = driverId,
                                       VehicleId = vehicleId,
                                   };

        return await transactionHandler.AssignVehicleAsync(assignment) switch
        {
            TransactionSuccess => Results.Ok(),
            TransactionError error => Results.BadRequest(error.ErrorMessage),
            _ => Results.NotFound()
        };
    }
}