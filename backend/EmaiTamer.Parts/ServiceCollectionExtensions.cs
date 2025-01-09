using EmailTamer.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EmaiTamer.Parts;

public static class ServiceCollectionExtensions
{
	public static IMvcCoreBuilder AddEmailTamerPart<T>(this IMvcCoreBuilder builder)
	{
		var assembly = typeof(T).Assembly;
		builder.Services.AddCoreServicesFromAssembly(assembly);
		builder.AddApplicationPart(assembly);
		return builder;
	}
}