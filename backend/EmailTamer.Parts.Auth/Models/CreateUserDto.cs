using AutoMapper;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using EmailTamer.Database.Entities;
using EmailTamer.Infrastructure.Auth;
using FluentValidation;

namespace EmailTamer.Auth.Models;

public class CreateUserDto: IMappable, IInbound
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
    
    public UserRole Role { get; set; } = UserRole.User;
    
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .Email();
            RuleFor(x => x.Password)
                .NotEmpty()
                .NotNull()
                .Password();
            RuleFor(x => x.Role)
                .IsInEnum()
                .NotEqual(UserRole.Admin);
        }
    }

    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<CreateUserDto, EmailTamerUser>(MemberList.Source)
            .EasyMember(x => x.UserName,
                (s, _) => s.Email)
            .IgnoreSourceMember(x => x.Password)
            .IgnoreSourceMember(x => x.Role);
    }
}