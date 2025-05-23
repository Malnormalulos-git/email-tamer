using AutoMapper;
using EmailTamer.Auth;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using EmailTamer.Database.Tenant.Entities;
using FluentValidation;

namespace EmailTamer.Parts.EmailBox.Models;

public sealed class EmailBoxDetailsDto : IMappable, IOutbound
{
    public Guid Id { get; set; }

    public string? BoxName { get; set; }

    public string? UserName { get; set; }

    public string Email { get; set; }

    public bool AuthenticateByEmail { get; set; }

    public string EmailDomainConnectionHost { get; set; }

    public int EmailDomainConnectionPort { get; set; }

    public bool UseSSl { get; set; }

    public ConnectionFault? ConnectionFault { get; set; }

    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<Database.Tenant.Entities.EmailBox, EmailBoxDetailsDto>(MemberList.Destination);
    }
}