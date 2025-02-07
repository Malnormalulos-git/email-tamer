using EmailTamer.Parts.Sync.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.Sync;

public static class ServiceCollectionExtensions
{
	public static IMvcCoreBuilder AddSyncPart(this IMvcCoreBuilder builder)
	{
		builder.AddEmailTamerPart<SyncEmailBoxesController>();
		return builder;
	}
}