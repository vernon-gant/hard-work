using Microsoft.Extensions.DependencyInjection;

namespace MotorPool.Services.Vehicles;

public static class VehicleServicesExtension
{

    public static void AddVehicleServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(VehicleServicesExtension));
    }

}