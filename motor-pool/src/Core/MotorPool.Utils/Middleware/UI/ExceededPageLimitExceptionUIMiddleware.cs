using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MotorPool.Utils.Exceptions;

namespace MotorPool.Utils.Middleware.UI;

public class ExceededPageLimitExceptionUIMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ExceededPageLimitException ex)
        {
            var factory = context.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = factory.GetTempData(context);

            tempData["ErrorMessage"] = ex.Message;
            tempData["ErrorType"] = "Page limit exceeded";
            tempData["ReturnLink"] = "/Vehicles/Index";

            tempData.Save();

            context.Response.Redirect("/Error");
        }
    }
}