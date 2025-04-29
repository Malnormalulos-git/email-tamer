using AutoMapper;
using EmailTamer.Auth;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using FluentValidation;

namespace EmailTamer.Parts.EmailBox.Models;

public sealed class TestConnectionDto : IMappable, IInbound
{
    public Guid? Id { get; set; }
    
    public string? UserName { get; set; }

    public string Email { get; set; } = null!;

    public bool AuthenticateByEmail { get; set; } = true;

    public string? Password { get; set; } = null!;

    public string EmailDomainConnectionHost { get; set; } = null!;

    public int EmailDomainConnectionPort { get; set; } = 993;

    public bool UseSSl { get; set; } = true;

    public class TestEmailBoxConnectionDtoValidator : AbstractValidator<TestConnectionDto>
    {
        public TestEmailBoxConnectionDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEmpty()
                .When(x => x.Password == null);
            
            RuleFor(x => x.UserName)
                .NotEmpty()
                .When(x => x.AuthenticateByEmail == false);

            RuleFor(x => x.Email)
                .NotEmpty()
                .Email();

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .When(x => x.Id == null);

            RuleFor(x => x.EmailDomainConnectionHost)
                .NotEmpty();
        }
    }

    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<TestConnectionDto, Database.Tenant.Entities.EmailBox>(MemberList.Source);
    }
}