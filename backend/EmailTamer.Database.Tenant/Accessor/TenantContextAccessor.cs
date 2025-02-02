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
	private string? Id;

	public async Task<string> GetTenantId()
	{
		if (string.IsNullOrEmpty(Id))
		{
			var userId = userContextAccessor.Id;
			var user = await userManager.FindByIdAsync(userId);
			
			Id = user!.TenantId.ToString();

			if (string.IsNullOrEmpty(Id))
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

				Id = newTenant.Id.ToString();
			}
		}

		return Id;
	}
}