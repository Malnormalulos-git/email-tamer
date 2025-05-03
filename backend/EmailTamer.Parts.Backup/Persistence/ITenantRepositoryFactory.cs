using EmailTamer.Database.Tenant.Accessor;

namespace EmailTamer.Parts.Sync.Persistence;

internal interface ITenantRepositoryFactory
{
    Task<ITenantRepository> Create(CancellationToken cancellationToken);
    Task<ITenantRepository> Create(ITenantContextAccessor tenantAccessor, CancellationToken cancellationToken);
}