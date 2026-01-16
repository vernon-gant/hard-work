using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MotorPool.Services.Drivers;

public static class DriverServiceExtension
{

    public static void AddDriverServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DriverServiceExtension));
        services.AddScoped<AssignmentTransactionHandler, DefaultAssignmentTransactionHandler>();
        services.AddScoped<AssignmentChangeLogger, TransactionalAssignmentFileLogger>(provider =>
        {
            string filePath = provider.GetRequiredService<IConfiguration>().GetValue<string>("AssignmentLogPath") ?? "Logs/assignment.log";
            if (!File.Exists(filePath)) { File.Create(filePath).Close(); }
            return new TransactionalAssignmentFileLogger(filePath, provider.GetRequiredService<ILogger<TransactionalAssignmentFileLogger>>());
        });
    }

}