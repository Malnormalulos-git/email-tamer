using EmailTamer.Core.DependencyInjection;
using EmailTamer.Infrastructure.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Infrastructure;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		services.AddCoreServicesFromAssemblyContaining<IUserContextAccessor>();
		return services;
	}
}