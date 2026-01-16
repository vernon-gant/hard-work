using MotorPool.Utils.Middleware.API;
using MotorPool.Utils.Middleware.UI;

namespace MotorPool.Utils;

public static class MiddlewareExtensions
{

    public static void UseCustomExceptionAPIMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceededPageLimitExceptionAPIMiddleware>();
    }

    public static void UseCustomExceptionUIMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceededPageLimitExceptionUIMiddleware>();
    }

}