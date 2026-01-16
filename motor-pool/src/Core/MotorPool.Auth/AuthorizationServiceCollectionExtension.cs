using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MotorPool.Auth.User;

namespace MotorPool.Auth;

public static class AuthorizationServiceCollectionExtension
{

    public static void AddAppIdentity(this IServiceCollection services, string connectionString)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;

                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();

        services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(connectionString));
    }

    public static void AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("IsAuthenticated", policy => policy.RequireAuthenticatedUser());
            options.AddPolicy("IsManager", policy => policy.RequireClaim("ManagerId"));

            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim("ManagerId")
                .Build();
        });
    }

    public static async Task SetupAuthDatabaseAsync(this IHost webHost)
    {
        using var freshScope = webHost.Services.CreateScope();
        var appDbContext = freshScope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var loggerFactory = freshScope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Migration");

        try
        {
            await appDbContext.Database.EnsureCreatedAsync();
            if (!(await appDbContext.Database.GetPendingMigrationsAsync()).Any()) return;

            logger.LogInformation("Migrating the user database");
            await appDbContext.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while migrating the database");
            throw;
        }
    }

}