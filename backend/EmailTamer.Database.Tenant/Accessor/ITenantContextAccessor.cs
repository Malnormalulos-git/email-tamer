namespace EmailTamer.Database.Tenant.Accessor;

public interface ITenantContextAccessor
{
	public Task<string> GetTenantId();
}