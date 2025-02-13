using EmailTamer.Database.Entities;
using EmailTamer.Database.Persistence;
using EmailTamer.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Database.Tenant.Accessor;

public sealed class TenantContextAccessor(
	IUserContextAccessor userContextAccessor,
	UserManager<EmailTamerUser> userManager,
	[FromKeyedServices(nameof(EmailTamerDbContext))] IEmailTamerRepository emailTamerRepository
	) : ITenantContextAccessor
{
	private string? _id;

	public async Task<string> GetTenantId()
	{
		if (string.IsNullOrEmpty(_id))
		{
			var userId = userContextAccessor.Id;
			var user = await userManager.FindByIdAsync(userId);
			
			_id = user!.TenantId.ToString();

			if (string.IsNullOrEmpty(_id))
			{
				var newTenant = new Database.Entities.Tenant
				{
					Id = Guid.NewGuid(),
					OwnerId = new Guid(userId)
				};

				emailTamerRepository.Add(newTenant);
				await emailTamerRepository.PersistAsync();

				user.TenantId = newTenant.Id;
				await userManager.UpdateAsync(user);

				_id = newTenant.Id.ToString();
			}
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