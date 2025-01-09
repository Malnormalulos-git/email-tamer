using AutoMapper;
using EmailTamer.Database.Entities;
using EmailTamer.Models.Auth;

namespace EmailTamer.Mapper.Users;

public class UsersMappingProfile : MappableProfile
{
	public UsersMappingProfile()
	{
		CreateMap<EmailTamerUser, AuthUser>(MemberList.Destination)
			.WithUserRoleContext()
			.EasyContextMember(x => x.Role, ctx => ctx.Role);
	}
}