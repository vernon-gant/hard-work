using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MotorPool.Persistence;

public static class PersistenceServiceCollectionExtension
{
    public static void AddPersistenceServices(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddDbContext<AppDbContext>(options => { options.UseSqlServer(connectionString); });
    }
}