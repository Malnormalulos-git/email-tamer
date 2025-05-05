using EmailTamer.Auth.Auth;
using EmailTamer.Auth.Config;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EmailTamer.Auth.Middleware;

internal sealed class AuthMiddleware(RequestDelegate next)
{
    [UsedImplicitly]
    public async Task InvokeAsync(HttpContext httpContext,
                            IOptionsMonitor<AuthConfig> configMonitor)
    {
        var config = configMonitor.CurrentValue;
        var services = httpContext.RequestServices;

        if (config.AllowAnonymous.Contains(httpContext.Request.Path.ToString()))
        {
            await next(httpContext);
            return;
        }

        var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

        if (userId is not null)
        {
            var accessor = services.GetRequiredService<IConfigurableUserContextAccessor>();
            accessor.Configure(userId);
        }

        await next(httpContext);
    }
}