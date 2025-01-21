using System.Data;
using EmailTamer.Database.Entities.Base;

namespace EmailTamer.Database.Persistence;

public interface ITenantRepository
{
	Task<T> ReadAsync<T>(Func<ITenantRepository, CancellationToken, Task<T>> func,
		CancellationToken cancellationToken = default);

	Task PersistAsync(CancellationToken cancellationToken = default);

	Task WriteInTransactionAsync(IsolationLevel isolationLevel,
		Func<ITenantRepository, CancellationToken, Task> operation,
		CancellationToken cancellationToken = default);

	Task<T> WriteInTransactionAsync<T>(IsolationLevel isolationLevel,
		Func<ITenantRepository, CancellationToken, Task<T>> operation,
		CancellationToken cancellationToken = default);

	IQueryable<T> Set<T>() where T : class, IEntity;

	public void Add<T>(T item) where T : class, IEntity;

	public void Update<T>(T item) where T : class, IEntity;

	public void Remove<T>(T item) where T : class, IEntity;

	public void AddRange<T>(IEnumerable<T> items) where T : class, IEntity;

	public void RemoveRange<T>(IEnumerable<T> items) where T : class, IEntity;
}