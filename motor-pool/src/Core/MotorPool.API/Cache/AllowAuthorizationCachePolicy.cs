using Microsoft.AspNetCore.OutputCaching;

namespace MotorPool.API.Cache;

public class AllowAuthorizationCachePolicy : IOutputCachePolicy
{
    public static readonly AllowAuthorizationCachePolicy Instance = new();

    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        var attemptOutputCaching = AttemptOutputCaching(context);
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = attemptOutputCaching;
        context.AllowCacheStorage = attemptOutputCaching;
        context.AllowLocking = true;

        context.CacheVaryByRules.QueryKeys = "*";

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        var response = context.HttpContext.Response;

        if (response.StatusCode == StatusCodes.Status200OK) return ValueTask.CompletedTask;

        context.AllowCacheStorage = false;
        return ValueTask.CompletedTask;
    }

    private static bool AttemptOutputCaching(OutputCacheContext context)
    {
        var request = context.HttpContext.Request;

        return HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method);
    }
}