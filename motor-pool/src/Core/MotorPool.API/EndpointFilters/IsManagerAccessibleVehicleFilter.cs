using MotorPool.Domain;
using MotorPool.Services.Manager;

namespace MotorPool.API.EndpointFilters;

public class IsManagerAccessibleVehicleFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        Vehicle currentVehicle = context.HttpContext.Items["Vehicle"] as Vehicle ?? throw new InvalidOperationException("Vehicle not found in the request context.");

        if (currentVehicle.IsManagerAccessible(context.HttpContext.User.GetManagerId())) return await next(context);

        context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

        return null;
    }
}