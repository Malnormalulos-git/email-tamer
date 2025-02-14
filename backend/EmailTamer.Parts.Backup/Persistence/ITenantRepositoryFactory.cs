namespace EmailTamer.Parts.Sync.Persistence;

internal interface ITenantRepositoryFactory
{
    Task<ITenantRepository> Create(CancellationToken cancellationToken);
}