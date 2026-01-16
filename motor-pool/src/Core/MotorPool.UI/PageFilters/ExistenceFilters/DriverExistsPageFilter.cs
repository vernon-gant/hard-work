using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MotorPool.Domain;
using MotorPool.Repository.Driver;

namespace MotorPool.UI.PageFilters.ExistenceFilters;

public class DriverExistsPageFilter(DriverQueryRepository driverQueryRepository) : IAsyncPageFilter
{

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        string? driverIdString = context.RouteData.Values["driverId"]?.ToString();

        if (driverIdString == null)
        {
            await next();
            return;
        }

        int driverId = int.Parse(driverIdString);

        Driver? driver = await driverQueryRepository.GetByIdAsync(driverId);

        if (driver == null)
        {
            context.Result = new RedirectToPageResult("/Error/NotFound");
            return;
        }

        context.HttpContext.Items["Driver"] = driver;
        await next();
    }

}