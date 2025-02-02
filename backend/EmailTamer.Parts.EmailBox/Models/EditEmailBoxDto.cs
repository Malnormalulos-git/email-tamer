using AutoMapper;
using EmailTamer.Auth;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using FluentValidation;

namespace EmailTamer.Parts.EmailBox.Models;

public sealed class EditEmailBoxDto : IMappable, IInbound
{
    public Guid Id { get; set; }
    
    public string? Name { get; set; }

    public string Email { get; set; } = null!;
    
    public string Password { get; set; } = null!;
    
    public string EmailDomainConnectionHost { get; set; } = null!;

    public int EmailDomainConnectionPort { get; set; } = 993;

    public bool UseSSl { get; set; } = true;

    public class EditEmailBoxDtoValidator : AbstractValidator<EditEmailBoxDto>
    {
        public EditEmailBoxDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .NotNull();
            RuleFor(x => x.Name)
                .MaximumLength(30);
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .Email();
            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty();
            RuleFor(x => x.EmailDomainConnectionHost)
                .NotNull()
                .NotEmpty();
        }
    }

    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<EditEmailBoxDto, Database.Tenant.Entities.EmailBox>(MemberList.Source);
    }
}