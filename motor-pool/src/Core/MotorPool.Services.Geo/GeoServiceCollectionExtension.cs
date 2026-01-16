using Microsoft.Extensions.DependencyInjection;

using MotorPool.Services.Geo.GraphHopper;
using MotorPool.Services.Geo.Services;
using MotorPool.Services.Geo.Services.Concrete;

namespace MotorPool.Services.Geo;

public static class GeoServiceCollectionExtension
{

    public static void AddGeoServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(GeoServiceCollectionExtension));
        services.AddScoped<TripQueryService, DefaultTripQueryService>();

        services.AddHttpClient<GraphHopperClient>()
                .ConfigureHttpClient((provider, client) =>
                {
                    GraphHopperConfiguration graphHopperConfiguration = provider.GetRequiredService<GraphHopperConfiguration>();
                    client.BaseAddress = new Uri(graphHopperConfiguration.BaseUrl);
                });
    }

}