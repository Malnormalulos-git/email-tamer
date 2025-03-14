using AutoMapper;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using EmailTamer.Database.Tenant.Entities;
using EmailTamer.Parts.Sync.Persistence;

namespace EmailTamer.Parts.Sync.Models;

public class MessageDetailsDto : IMappable, IOutbound
{
    public string Id { get; set; } = null!;
    
    public string? InReplyTo { get; set; }
    
    public string? Subject { get; set; } 
    
    public string? TextBody { get; set; } 
    
    public List<string> AttachmentFilesNames { get; set; } = [];
    
    public List<string> References { get; set; } = []; 
    
    public List<EmailAddress> From { get; set; } = []; 
    
    public List<EmailAddress> To { get; set; } = [];

    public DateTime Date { get; set; }
    
    public DateTime? ResentDate { get; set; }
    
    public string? HtmlBody { get; set; }
    
    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<Message, MessageDetailsDto>(MemberList.Destination)
            .IgnoreMember(x => x.HtmlBody);
    }
}