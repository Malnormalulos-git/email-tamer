using System.Data;
using EmailTamer.Database.Entities.Base;
using EmailTamer.Database.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EmailTamer.Database.Persistence;

public class EmailTamerRepository<TContext>(
    TContext dbContext,
    IDatabasePolicySet databasePolicySet)
    : IEmailTamerRepository
    where TContext : DbContext
{
    public Task<T> ReadAsync<T>(Func<IEmailTamerRepository, CancellationToken, Task<T>> func,
								CancellationToken cancellationToken = default)
	{
		return databasePolicySet.DatabaseReadPolicy.ExecuteAsync(ct => func(this, ct), cancellationToken);
	}

	public Task PersistAsync(CancellationToken cancellationToken = default)
		=> databasePolicySet.DatabaseWritePolicy.ExecuteAsync(dbContext.SaveChangesAsync,
			cancellationToken);

	public Task WriteInTransactionAsync(IsolationLevel isolationLevel,
										Func<IEmailTamerRepository, CancellationToken, Task> operation,
										CancellationToken cancellationToken)
		=> ExecutionStrategyExtensions.ExecuteInTransactionAsync(
			dbContext.Database.CreateExecutionStrategy(),
			this,
			// returning 0 because this overload of ExecuteInTransactionAsync requires result
			(r, ct) => operation(r, ct).ContinueWith(t => t.Exception is { } e
				? throw e
				: 0, ct),
			// unconditional "true" because it will never get called for postgres
			(_, _) => Task.FromResult(true),
			// this is the reason we're using this exact overload - ability to specify isolation level
			(db, ct) => db.Database.BeginTransactionAsync(isolationLevel, ct),
			cancellationToken);

	public Task<T> WriteInTransactionAsync<T>(IsolationLevel isolationLevel,
											  Func<IEmailTamerRepository, CancellationToken, Task<T>> operation,
											  CancellationToken cancellationToken)
		=> ExecutionStrategyExtensions.ExecuteInTransactionAsync(
			dbContext.Database.CreateExecutionStrategy(),
			this,
			(r, ct) => operation(r, ct),
			(_, _) => Task.FromResult(true),
			// this is the reason we're using this exact overload - ability to specify isolation level
			(db, ct) => db.Database.BeginTransactionAsync(isolationLevel, ct),
			cancellationToken);

	public IQueryable<T> Set<T>() where T : class, IEntity => dbContext.Set<T>();

	public void Add<T>(T item) where T : class, IEntity => dbContext.Add(item);

	public void UpdateRange<T>(IEnumerable<T> items) where T : class, IEntity => dbContext.UpdateRange(items);

	public void AddRange<T>(IEnumerable<T> items) where T : class, IEntity => dbContext.AddRange(items);

	public void RemoveRange<T>(IEnumerable<T> items) where T : class, IEntity => dbContext.RemoveRange(items);

	public void Update<T>(T item) where T : class, IEntity => dbContext.Update(item);

	public void Remove<T>(T item) where T : class, IEntity => dbContext.Remove(item);

	public IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T>> ChangeTrackerEntries<T>()
		where T : class, IEntity => dbContext.ChangeTracker.Entries<T>();
}