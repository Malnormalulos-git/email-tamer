using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EmailTamer.Database.Persistence.Interceptors;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDatabaseSaveChangesInterceptor<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
		where T : ISaveChangesInterceptor, IOrderedInterceptor
	{
		services.TryAddEnumerable(new ServiceDescriptor(typeof(IOrderedInterceptor), typeof(T), lifetime));
		return services;
	}
}