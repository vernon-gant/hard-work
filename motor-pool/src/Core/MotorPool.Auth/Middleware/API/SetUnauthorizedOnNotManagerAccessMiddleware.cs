using System.Net;
using MotorPool.Services.Manager;

namespace MotorPool.Auth.Middleware.API;

public class SetUnauthorizedOnNotManagerAccessMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        await next(context);

        if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden && !context.User.IsManager())
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    }
}

public static class UnauthorizedOnNotManagerAccessMiddlewareExtensions
{
    public static IApplicationBuilder UseUnauthorizedOnNotManagerAccess(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SetUnauthorizedOnNotManagerAccessMiddleware>();
    }
}