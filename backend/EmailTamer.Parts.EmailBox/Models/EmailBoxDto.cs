using AutoMapper;
using EmailTamer.Auth;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using FluentValidation;

namespace EmailTamer.Parts.EmailBox.Models;

public sealed class EmailBoxDto : IMappable, IOutbound
{
    public Guid Id { get; set; }

    public string BoxName { get; set; }

    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<Database.Tenant.Entities.EmailBox, EmailBoxDto>(MemberList.Destination)
            .EasyMember(x => x.BoxName, y => y.BoxName ?? y.Email);
    }
}