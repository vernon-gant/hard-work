using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MotorPool.Domain;
using MotorPool.Repository.Vehicle;

namespace MotorPool.UI.PageFilters.ExistenceFilters;

public class VehicleExistsPageFilter(VehicleQueryRepository vehicleQueryRepository) : IAsyncPageFilter
{

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        string? vehicleIdString = context.RouteData.Values["vehicleId"]?.ToString();

        if (vehicleIdString == null)
        {
            await next();
            return;
        }

        int vehicleId = int.Parse(vehicleIdString);

        Vehicle? vehicle = await vehicleQueryRepository.GetByIdAsync(vehicleId);

        if (vehicle == null)
        {
            context.Result = new RedirectToPageResult("/Error/NotFound");
            return;
        }

        context.HttpContext.Items["Vehicle"] = vehicle;
        await next();
    }

}