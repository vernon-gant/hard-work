using MotorPool.Domain;
using MotorPool.Repository.Vehicle;

namespace MotorPool.API.EndpointFilters;

public class VehicleExistsFilter(VehicleQueryRepository vehicleQueryRepository) : IEndpointFilter
{

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        string vehicleIdString = context.HttpContext.Request.RouteValues["vehicleId"]?.ToString() ?? throw new InvalidOperationException("VehicleId is not found in route data");
        int vehicleId = int.Parse(vehicleIdString);

        Vehicle? vehicle = await vehicleQueryRepository.GetByIdAsync(vehicleId);

        if (vehicle is not null)
        {
            context.HttpContext.Items["Vehicle"] = vehicle;
            return await next(context);
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        return null;
    }

}