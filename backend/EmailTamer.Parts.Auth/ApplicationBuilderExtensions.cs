using EmailTamer.Auth.Middleware;
using Microsoft.AspNetCore.Builder;

namespace EmailTamer.Auth;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseEmailTamerAuth(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<AuthMiddleware>();
        return builder;
    }
}