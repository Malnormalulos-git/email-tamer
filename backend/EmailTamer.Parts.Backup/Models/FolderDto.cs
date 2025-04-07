using AutoMapper;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using EmailTamer.Database.Tenant.Entities;

namespace EmailTamer.Parts.Sync.Models;

public class FolderDto : IMappable, IOutbound
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<Folder, FolderDto>(MemberList.Destination);
    }
}