using MotorPool.Domain;
using MotorPool.Repository.Driver;

namespace MotorPool.API.EndpointFilters;

public class DriverExistsFilter(DriverQueryRepository driverQueryRepository) : IEndpointFilter
{

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        int driverId = int.Parse(context.HttpContext.Request.RouteValues["driverId"]!.ToString()!);

        Driver? driver = await driverQueryRepository.GetByIdAsync(driverId);

        if (driver is not null)
        {
            context.HttpContext.Items["Driver"] = driver;
            return await next(context);
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        return null;
    }

}