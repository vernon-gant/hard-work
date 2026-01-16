using MotorPool.API.Cache;
using MotorPool.Services.VehicleBrand.Models;
using MotorPool.Services.VehicleBrand.Services;

namespace MotorPool.API.Endpoints;

public static class VehicleBrandEndpoints
{
    public static void MapVehicleBrandEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder vehicleBrandsGroupBuilder = app.MapGroup("vehicle-brands");

        vehicleBrandsGroupBuilder.MapGet("", GetAll)
                                 .WithName("GetAllVehicleBrands")
                                 .Produces<List<VehicleBrandViewModel>>()
                                 .CacheOutput(policyBuilder =>
                                  {
                                      policyBuilder.AddPolicy<AllowAuthorizationCachePolicy>()
                                                   .Expire(TimeSpan.FromHours(12));
                                  }, excludeDefaultPolicy: true);
    }

    private static async Task<IResult> GetAll(VehicleBrandService vehicleBrandService) => Results.Ok(await vehicleBrandService.GetAllAsync());
}