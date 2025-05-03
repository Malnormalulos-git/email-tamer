using EmailTamer.Database.Entities;
using EmailTamer.Database.Persistence;
using EmailTamer.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Database.Tenant.Accessor;

public sealed class TenantContextAccessor : ITenantContextAccessor
{
	private IUserContextAccessor? _userContextAccessor;
	private UserManager<EmailTamerUser>? _userManager;
	private IEmailTamerRepository? _emailTamerRepository;
	private string? _id;

	public TenantContextAccessor(
		IUserContextAccessor userContextAccessor,
		UserManager<EmailTamerUser> userManager,
		[FromKeyedServices(nameof(EmailTamerDbContext))] IEmailTamerRepository emailTamerRepository)
	{
		_userContextAccessor = userContextAccessor;
		_userManager = userManager;
		_emailTamerRepository = emailTamerRepository;
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