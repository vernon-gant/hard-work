using MotorPool.Domain;
using MotorPool.Services.Manager;

namespace MotorPool.API.EndpointFilters;

public class IsManagerAccessibleEnterpriseFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        Enterprise requestedEnterprise = context.HttpContext.Items["Enterprise"] as Enterprise ?? throw new InvalidOperationException("Enterprise not found in the request context.");

        if (requestedEnterprise.IsManagerAccessible(context.HttpContext.User.GetManagerId())) return await next(context);

        context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

        return null;
    }
}