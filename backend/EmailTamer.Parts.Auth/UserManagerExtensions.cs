using AutoMapper;
using EmailTamer.Auth.Exceptions;
using EmailTamer.Auth.Mappers;
using EmailTamer.Core.Mappers;
using EmailTamer.Database.Entities;
using EmailTamer.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;

namespace EmailTamer.Auth;

public static class UserManagerExtensions
{
	public static async Task<(EmailTamerUser user, UserRole role)> GetUserAndRoleAsync(
		this UserManager<EmailTamerUser> manager,
		string userId)
	{
		var user = await manager.GetUserAsync(userId);
		var role = await manager.GetUserRoleAsync(user);

		return (user, role);
	}

	private static async Task<EmailTamerUser> GetUserAsync(
		this UserManager<EmailTamerUser> manager,
		string userId)
	{
		var user = await manager.FindByIdAsync(userId);

		if (user is null)
		{
			throw new UserDoesNotExistException(userId);
		}

		return user;
	}

	public static async Task<UserRole> GetUserRoleAsync(
		this UserManager<EmailTamerUser> manager,
		EmailTamerUser user)
	{
		var userRoles = await manager.GetRolesAsync(user);
		var role = userRoles[0];
		return Enum.Parse<UserRole>(role);
	}

	public static async Task<AuthUser> GetAuthUserAsync(
		this UserManager<EmailTamerUser> manager,
		string id,
		IMapper mapper)
	{
		try
		{
			var (user, role) = await manager.GetUserAndRoleAsync(id);
			var userRoleCtx = new UserRoleMappingContext(role);
			return mapper.MapWithContext<AuthUser>(user, userRoleCtx);
		}
		catch (UserDoesNotExistException)
		{
			return new();
		}
	}

	public static Task<IList<EmailTamerUser>> GetUsersInRoleAsync(
		this UserManager<EmailTamerUser> manager,
		UserRole role)
		=> manager.GetUsersInRoleAsync(role.ToString("G"));
}