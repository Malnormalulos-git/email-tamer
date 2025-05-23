using AutoMapper;
using EmailTamer.Auth;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using FluentValidation;

namespace EmailTamer.Parts.EmailBox.Models;

public sealed class EditEmailBoxDto : IMappable, IInbound
{
    public Guid Id { get; set; }

    public string? BoxName { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public bool? AuthenticateByEmail { get; set; }

    public string? Password { get; set; }

    public string? EmailDomainConnectionHost { get; set; }

    public int? EmailDomainConnectionPort { get; set; }

    public bool? UseSSl { get; set; }

    public class EditEmailBoxDtoValidator : AbstractValidator<EditEmailBoxDto>
    {
        public EditEmailBoxDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.BoxName)
                .MaximumLength(30);

            RuleFor(x => x.UserName)
                .NotNull()
                .NotEmpty()
                .When(x => x.AuthenticateByEmail == false);

            RuleFor(x => x.Email)
                .NotNull()
                .NotEmpty()
                .When(x => x.Email != null)!
                .Email();

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .When(x => x.Password != null);

            RuleFor(x => x.EmailDomainConnectionHost)
                .NotNull()
                .NotEmpty()
                .When(x => x.EmailDomainConnectionHost != null);
        }
    }

    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<EditEmailBoxDto, Database.Tenant.Entities.EmailBox>(MemberList.Source);
    }
}