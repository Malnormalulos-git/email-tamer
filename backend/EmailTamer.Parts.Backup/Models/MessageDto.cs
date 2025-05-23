using AutoMapper;
using EmailTamer.Core.Mappers;
using EmailTamer.Core.Models;
using EmailTamer.Database.Tenant.Entities;

namespace EmailTamer.Parts.Sync.Models;

public class MessageDto : IMappable, IOutbound
{
    public string Id { get; set; }

    public string? ThreadId { get; set; }

    public string? Subject { get; set; }

    public string? TextBody { get; set; }

    public DateTime Date { get; set; }

    public List<string> Participants { get; set; } = [];

    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<Message, MessageDto>(MemberList.Destination)
            .EasyMember(x => x.TextBody, y =>
                !string.IsNullOrEmpty(y.TextBody)
                    ? y.TextBody.Substring(0, Math.Min(y.TextBody.Length, 200))
                    : null)
            .EasyMember(x => x.Participants, m =>
                m.From
                    .Select(x => !string.IsNullOrEmpty(x.Name) ? x.Name : x.Address)
                    .Concat(m.To.Select(x => !string.IsNullOrEmpty(x.Name) ? x.Name : x.Address))
                    .ToList());
    }
}