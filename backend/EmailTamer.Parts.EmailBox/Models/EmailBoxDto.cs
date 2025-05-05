using AutoMapper;
using EmailTamer.Auth;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using EmailTamer.Database.Tenant.Entities;
using FluentValidation;

namespace EmailTamer.Parts.EmailBox.Models;

public sealed class EmailBoxDto : IMappable, IOutbound
{
    public Guid Id { get; set; }

    public string BoxName { get; set; }

    public DateTime? LastSyncAt { get; set; }

    public ConnectionFault? ConnectionFault { get; set; }

    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<Database.Tenant.Entities.EmailBox, EmailBoxDto>(MemberList.Destination)
            .EasyMember(x => x.BoxName, y => y.BoxName ?? y.Email)
            .EasyMember(x => x.LastSyncAt, y =>
                y.LastSyncAt == default ? (DateTime?)null : y.LastSyncAt);
    }
}