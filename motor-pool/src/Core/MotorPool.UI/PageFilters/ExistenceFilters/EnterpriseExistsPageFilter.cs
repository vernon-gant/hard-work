using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MotorPool.Domain;
using MotorPool.Repository.Enterprise;

namespace MotorPool.UI.PageFilters.ExistenceFilters;

public class EnterpriseExistsPageFilter(EnterpriseQueryRepository enterpriseQueryService) : IAsyncPageFilter
{

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        string? enterpriseIdString = context.RouteData.Values["enterpriseId"]?.ToString();

        if (enterpriseIdString == null)
        {
            await next();
            return;
        }

        int enterpriseId = int.Parse(enterpriseIdString);

        Enterprise? enterprise = await enterpriseQueryService.GetByIdAsync(enterpriseId);

        if (enterprise == null)
        {
            context.Result = new RedirectToPageResult("/Error/NotFound");
            return;
        }

        context.HttpContext.Items["Enterprise"] = enterprise;
        await next();
    }

}