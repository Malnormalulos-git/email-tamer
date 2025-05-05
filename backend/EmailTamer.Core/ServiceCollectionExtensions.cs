using EmailTamer.Core.MediatR;
using EmailTamer.Core.Startup;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;

namespace EmailTamer.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddSingleton(typeof(ISystemClock), typeof(SystemClock));
        services.TryAddTransient<IStartupActionCoordinator, StartupActionCoordinator>();
        return services;

    }
}