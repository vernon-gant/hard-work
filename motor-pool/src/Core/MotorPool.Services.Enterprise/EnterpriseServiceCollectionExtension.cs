using Microsoft.Extensions.DependencyInjection;

namespace MotorPool.Services.Enterprise;

public static class EnterpriseServiceExtension
{

    public static void AddEnterpriseServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(EnterpriseServiceExtension));
    }

}