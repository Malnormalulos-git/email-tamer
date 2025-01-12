using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmailTamer.Core.Startup;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddStartupAction<T>(this IServiceCollection services) where T : IAsyncStartupAction
	{
		var startupActionDescriptor =
			new ServiceDescriptor(typeof(IAsyncStartupAction), typeof(T), ServiceLifetime.Scoped);
		services.TryAddEnumerable(startupActionDescriptor);
		return services;
	}
}