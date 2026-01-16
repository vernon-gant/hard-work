using Microsoft.Extensions.DependencyInjection;

using MotorPool.Services.VehicleBrand.Services;
using MotorPool.Services.VehicleBrand.Services.Concrete;

namespace MotorPool.Services.VehicleBrand;

public static class VehicleBrandServicesExtension
{

    public static void AddVehicleBrandServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(VehicleBrandServicesExtension));
        services.AddScoped<VehicleBrandService, DefaultVehicleBrandService>();
    }

}