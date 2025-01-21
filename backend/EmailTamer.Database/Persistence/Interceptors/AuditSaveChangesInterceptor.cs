using EmailTamer.Core.Extensions;
using EmailTamer.Database.Entities.Base;
using EmailTamer.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace EmailTamer.Database.Persistence.Interceptors;

public sealed class AuditSaveChangesInterceptor(
	ILogger<AuditSaveChangesInterceptor> logger,
	ISystemClock clock,
	IUserContextAccessor accessor)
	: SaveChangesInterceptor, IOrderedInterceptor
{
	public uint Order => 0;

	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
																				InterceptionResult<int> result,
																				CancellationToken cancellationToken = default)
	{
		await base.SavingChangesAsync(eventData, result, cancellationToken);

		var ctx = eventData.Context;
		if (ctx is null)
		{
			logger.LogWarning("Database context is null");
			return result;
		}

		logger.Time(() => AuditEntities(ctx.ChangeTracker), "Audit tracked database context '{DatabaseContextName}' entities",
			ctx.GetType().Name);
		return result;
	}

	private void AuditEntities(ChangeTracker changeTracker)
	{
		var entries = changeTracker.Entries();
		var now = clock.UtcNow.DateTime;
		var userId = accessor.Id;

		foreach (var entry in entries)
		{
			if (entry.Entity is IDateAuditableEntity dateAudit)
			{
				switch (entry.State)
				{
					case EntityState.Modified:
						dateAudit.ModifiedAt = now;
						break;
					case EntityState.Added:
						dateAudit.CreatedAt = now;
						dateAudit.ModifiedAt = now;
						break;
					case EntityState.Detached:
					case EntityState.Unchanged:
					case EntityState.Deleted:
					default:
						break;
				}
			}

			if (entry.Entity is IEmailTamerUserAuditableEntity userAudit)
			{
				switch (entry.State)
				{
					case EntityState.Modified:
						userAudit.ModifiedByUserId = userId;
						break;
					case EntityState.Added:
						userAudit.CreatedByUserId = userId;
						userAudit.ModifiedByUserId = userId;
						break;
					case EntityState.Detached:
					case EntityState.Unchanged:
					case EntityState.Deleted:
					default:
						break;
				}
			}

			var isSoftDeleted = false;

			if (entry.Entity is IDateSoftDeletableEntity dateSoftDeletable)
			{
				switch (entry.State)
				{
					case EntityState.Deleted:
						isSoftDeleted = true;
						dateSoftDeletable.DeletedAt = now;
						break;
					case EntityState.Modified:
					case EntityState.Added:
					case EntityState.Detached:
					case EntityState.Unchanged:
					default:
						break;
				}
			}
			
			if (entry.Entity is IEmailTamerUserSoftDeletableEntity userSoftDeletable)
			{
				switch (entry.State)
				{
					case EntityState.Deleted:
						isSoftDeleted = true;
						userSoftDeletable.DeletedByUserId = userId;
						break;
					case EntityState.Modified:
					case EntityState.Added:
					case EntityState.Detached:
					case EntityState.Unchanged:
					default:
						break;
				}
			}

			if (isSoftDeleted)
			{
				entry.State = EntityState.Modified;
			}
		}
	}
}