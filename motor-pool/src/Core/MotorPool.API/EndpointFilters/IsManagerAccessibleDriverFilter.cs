using MotorPool.Services.Drivers.Models;
using MotorPool.Services.Manager;

namespace MotorPool.API.EndpointFilters;

public class IsManagerAccessibleDriverFilter : IEndpointFilter
{

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        DriverViewModel currentDriver =
            context.HttpContext.Items["Driver"] as DriverViewModel ?? throw new InvalidOperationException("Driver not found in the request context.");

        if (currentDriver.ManagerIds.Contains(context.HttpContext.User.GetManagerId())) return await next(context);

        context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

        return null;
    }

}