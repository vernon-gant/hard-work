using MotorPool.Persistence;
using MotorPool.Services.Manager;

namespace MotorPool.API.EndpointFilters;

public class ManagerExistsFilter(AppDbContext dbContext, ILogger<ManagerExistsFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        int managerId = context.HttpContext.User.GetManagerId();
        Domain.Manager? manager = await dbContext.Managers.FindAsync(managerId);

        if (manager is not null)
        {
            logger.LogInformation("Manager with id {ManagerId} found", managerId);
            return await next(context);
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
        return null;
    }
}