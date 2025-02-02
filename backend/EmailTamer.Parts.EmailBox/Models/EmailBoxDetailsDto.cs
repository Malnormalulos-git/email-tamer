using AutoMapper;
using EmailTamer.Auth;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using FluentValidation;

namespace EmailTamer.Parts.EmailBox.Models;

public sealed class EmailBoxDetailsDto : IMappable, IOutbound
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string Email { get; set; }
    
    public string EmailDomainConnectionHost { get; set; }

    public int EmailDomainConnectionPort { get; set; }

    public bool UseSSl { get; set; }

    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<Database.Tenant.Entities.EmailBox, EmailBoxDetailsDto>(MemberList.Destination);
    }
}