using EmailTamer.Core.Startup;
using EmailTamer.Database.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace EmailTamer.Database.Startup;

[UsedImplicitly]
public class RunDbMigrationsAction(IDatabasePolicySet policySet, EmailTamerDbContext dbContext)
	: IAsyncStartupAction
{
	public uint Order => 0;

	public Task PerformActionAsync(CancellationToken cancellationToken = default)
		=> policySet.DatabaseWritePolicy.ExecuteAsync(() => dbContext.Database.MigrateAsync(cancellationToken));
}