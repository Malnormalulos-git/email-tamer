using AutoMapper;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using EmailTamer.Database.Tenant.Entities;

namespace EmailTamer.Parts.Sync.Models;

public class EmailBoxStatusDto : IMappable, IOutbound
{
    public Guid Id { get; set; }

    public BackupStatus BackupStatus { get; set; }

    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<EmailBox, EmailBoxStatusDto>(MemberList.Destination);
    }
}