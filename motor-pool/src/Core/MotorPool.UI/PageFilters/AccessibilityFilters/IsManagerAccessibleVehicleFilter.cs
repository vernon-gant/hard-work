using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using MotorPool.Services.Manager;

namespace MotorPool.UI.PageFilters.AccessibilityFilters;

public class IsManagerAccessibleVehicleFilter(ManagerPermissionService managerPermissionService) : IAsyncPageFilter
{

    public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        await Task.CompletedTask;
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

        if (!await managerPermissionService.IsManagerAccessibleVehicle(context.HttpContext.User.GetManagerId(), vehicleId))
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }

}