using AutoMapper;
using EmailTamer.Core.Mappers;
using EmailTamer.Database.Entities;
using EmailTamer.Infrastructure.Auth;

namespace EmailTamer.Auth.Mappers;

public class UsersMappingProfile : MappableProfile
{
    public UsersMappingProfile()
    {
        CreateMap<EmailTamerUser, AuthUser>(MemberList.Destination)
            .WithUserRoleContext()
            .EasyContextMember(x => x.Role, ctx => ctx.Role);
    }
}