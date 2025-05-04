using AutoMapper;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using EmailTamer.Database.Tenant.Entities;

namespace EmailTamer.Parts.Sync.Models;

public class AttachmentDto : IOutbound, IMappable
{
    public string Id { get; set; } = null!;
    
    public string FileName { get; set; } = null!;
    
    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<Attachment, AttachmentDto>(MemberList.Destination);
    }
}