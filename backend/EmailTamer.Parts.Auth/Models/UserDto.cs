using AutoMapper;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using EmailTamer.Infrastructure.Auth;

namespace EmailTamer.Auth.Models;

public sealed class UserDto : IMappable, IOutbound
{
    public string Id { get; set; } = null!;

    public string Email { get; set; } = null!;

    public UserRole Role { get; set; }


    public static void AddProfileMapping(Profile profile)
        => profile.CreateMap<AuthUser, UserDto>(MemberList.Destination);
}