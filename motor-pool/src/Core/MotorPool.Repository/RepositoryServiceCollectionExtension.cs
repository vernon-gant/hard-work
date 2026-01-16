using Microsoft.Extensions.DependencyInjection;
using MotorPool.Repository.Driver;
using MotorPool.Repository.Enterprise;
using MotorPool.Repository.Geo;
using MotorPool.Repository.Vehicle;

namespace MotorPool.Repository;

public static class RepositoryServiceCollectionExtension
{
    public static void AddRepository(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<DriverQueryRepository, EfCoreDriverQueryRepository>();
        serviceCollection.AddScoped<EnterpriseQueryRepository, EfCoreEnterpriseQueryRepository>();
        serviceCollection.AddScoped<EnterpriseChangeRepository, EfCoreEnterpriseChangeRepository>();
        serviceCollection.AddScoped<VehicleQueryRepository, EfCoreVehicleQueryRepository>();
        serviceCollection.AddScoped<VehicleChangeRepository, EfCoreVehicleChangeRepository>();
        serviceCollection.AddScoped<GeoQueryRepository, EfCoreGeoRepository>();
    }
}