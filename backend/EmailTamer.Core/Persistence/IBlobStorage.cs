namespace EmailTamer.Core.Persistence;

public interface IBlobStorage
{
    public Task SaveAsync(Blob blob, string storageName, CancellationToken cancellationToken = default);

    public Task<Blob> GetAsync(string key, string storageName, CancellationToken cancellationToken = default);

    public Task DeleteAsync(string key, string storageName, CancellationToken cancellationToken = default);
}