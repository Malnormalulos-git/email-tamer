using EmailTamer.Database.Entities;
using EmailTamer.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;

namespace EmailTamer.Database.Tenant.Accessor;

public sealed class TenantContextAccessor : ITenantContextAccessor
{
    private IUserContextAccessor? _userContextAccessor;
    private UserManager<EmailTamerUser>? _userManager;
    private string? _id;

    public TenantContextAccessor(
        IUserContextAccessor userContextAccessor,
        UserManager<EmailTamerUser> userManager)
    {
        _userContextAccessor = userContextAccessor;
        _userManager = userManager;
    }
    public TenantContextAccessor(Guid tenantId)
    {
        _id = tenantId.ToString();
    }

    public async Task<string> GetTenantId()
    {
        if (string.IsNullOrEmpty(_id))
        {
            var userId = _userContextAccessor.Id;
            var user = await _userManager.FindByIdAsync(userId);

            _id = user!.TenantId.ToString();
        }

        return _id;
    }

    public string GetDatabaseName()
    {
        var tenantId = GetTenantId().GetAwaiter().GetResult();

        var dbName = $"tenant_{tenantId.Replace("-", "_")}";

        return dbName;
    }

    public string GetS3BucketName()
    {
        var tenantId = GetTenantId().GetAwaiter().GetResult();

        var bucketName = $"tenant-{tenantId}";

        return bucketName;
    }
}