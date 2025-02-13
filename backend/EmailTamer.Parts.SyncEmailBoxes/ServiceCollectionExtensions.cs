using Amazon.S3;
using Amazon.S3.Util;
using EmailTamer.Core.Persistence;
using EmailTamer.Database.Tenant.Accessor;
using EmailTamer.Parts.Sync.Controllers;
using EmailTamer.Parts.Sync.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace EmailTamer.Parts.Sync;

public static class ServiceCollectionExtensions
{
	public static IMvcCoreBuilder AddSyncPart(this IMvcCoreBuilder builder)
	{
		builder.AddEmailTamerPart<SyncEmailBoxesController>();
		
		var services = builder.Services;
		
		services.AddScoped<ITenantRepositoryFactory, TenantRepositoryFactory>();
		
		services.TryAddScoped<ITenantRepository>(sp =>
			sp.GetRequiredService<ITenantRepositoryFactory>()
				.Create(default)
				.GetAwaiter()
				.GetResult());
		
		return builder;
	}
}