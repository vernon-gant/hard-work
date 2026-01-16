using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MotorPool.Persistence;

public static class DatabaseSetupExtension
{
    public static async Task SetupDatabaseAsync(this IHost webHost)
    {
        using IServiceScope freshScope = webHost.Services.CreateScope();
        AppDbContext appDbContext = freshScope.ServiceProvider.GetRequiredService<AppDbContext>();
        ILoggerFactory loggerFactory = freshScope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger("Migration");

        try
        {
            if (!(await appDbContext.Database.GetPendingMigrationsAsync()).Any()) return;

            logger.LogInformation("Migrating the database");
            await appDbContext.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while migrating the database");
            throw;
        }
    }
}