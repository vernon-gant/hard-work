using MotorPool.Domain;
using MotorPool.Repository.Enterprise;

namespace MotorPool.API.EndpointFilters;

public class EnterpriseExistsFilter(EnterpriseQueryRepository enterpriseQueryRepository) : IEndpointFilter
{

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        string enterpriseIdString = context.HttpContext.Request.RouteValues["enterpriseId"]?.ToString() ?? throw new InvalidOperationException("EnterpriseId is not found in route data");
        int enterpriseId = int.Parse(enterpriseIdString);

        Enterprise? enterprise = await enterpriseQueryRepository.GetByIdAsync(enterpriseId);

        if (enterprise is not null)
        {
            context.HttpContext.Items["Enterprise"] = enterprise;
            return await next(context);
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        return null;
    }

}