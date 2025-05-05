using AutoMapper;
using EmailTamer.Auth;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using EmailTamer.Database.Tenant.Entities;
using FluentValidation;

namespace EmailTamer.Parts.EmailBox.Models;

public sealed class CreateEmailBoxDto : IMappable, IInbound
{
    public string? BoxName { get; set; }

    public string? UserName { get; set; }

    public string Email { get; set; } = null!;

    public bool AuthenticateByEmail { get; set; } = true;

    public string Password { get; set; } = null!;

    public string EmailDomainConnectionHost { get; set; } = null!;

    public int EmailDomainConnectionPort { get; set; } = 993;

    public bool UseSSl { get; set; } = true;

    public class EmailBoxDtoValidator : AbstractValidator<CreateEmailBoxDto>
    {
        public EmailBoxDtoValidator()
        {
            RuleFor(x => x.BoxName)
                .MaximumLength(30);

            RuleFor(x => x.UserName)
                .NotNull()
                .NotEmpty()
                .When(x => x.AuthenticateByEmail == false);

            RuleFor(x => x.Email)
                .NotEmpty()
                .Email();

            RuleFor(x => x.Password)
                .NotEmpty();

            RuleFor(x => x.EmailDomainConnectionHost)
                .NotEmpty();
        }
    }

    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<CreateEmailBoxDto, Database.Tenant.Entities.EmailBox>(MemberList.Source)
            .EasyMember(eb => eb.BackupStatus, _ => BackupStatus.Idle);
    }
}