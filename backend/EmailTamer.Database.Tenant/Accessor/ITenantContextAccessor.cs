namespace EmailTamer.Database.Tenant.Accessor;

public interface ITenantContextAccessor
{
    public Task<string> GetTenantId();

    public string GetDatabaseName();
    public string GetS3BucketName();
}