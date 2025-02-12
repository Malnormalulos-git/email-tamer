using System.Web;
using AutoMapper;
using EmailTamer.Core.Mappers;
using EmailTamer.Database.Tenant.Entities;
using HtmlAgilityPack;
using MimeKit;

namespace EmailTamer.Parts.Sync.Mappers;

public class MessageMappingProfile : MappableProfile, IMappable
{
    public static void AddProfileMapping(Profile profile)
    {
        profile.CreateMap<MimeMessage, Message>(MemberList.Destination)
            .EasyMember(x => x.Id, y => y.MessageId)
            .EasyMember(x => x.Date, y => y.Date.UtcDateTime)
            .EasyMember(x => x.ResentDate, y => y.ResentDate.UtcDateTime)
            .EasyMember(x => x.References, y => y.References.ToList())
            .EasyMember(x => x.TextBody, y => GetTextBody(y))
            .ForMember(x => x.To,
                opt => opt.MapFrom(src =>
                    src.To.OfType<MailboxAddress>()
                        .Select(m => new EmailAddress { Name = m.Name, Address = m.Address, Domain = m.Domain })))
            .ForMember(x => x.From,
                opt => opt.MapFrom(src =>
                    src.From.OfType<MailboxAddress>()
                        .Select(m => new EmailAddress { Name = m.Name, Address = m.Address, Domain = m.Domain })))
            .IgnoreMember(x => x.EmailBoxes)
            .IgnoreMember(x => x.Folders)
            // .IgnoreMember(x => x.S3FolderName)
            ;
    }
    
    private static string GetTextBody(MimeMessage y)
    {
        var textBody = y.TextBody;
        if (string.IsNullOrEmpty(textBody))
        {
            var htmlBody = y.HtmlBody?.Substring(0, Math.Min(y.HtmlBody.Length, 10000));
            if (!string.IsNullOrEmpty(htmlBody))
            {
                try
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(htmlBody);
                    textBody = HttpUtility.HtmlDecode(doc.DocumentNode.InnerText);
                }
                catch
                {
                    textBody = string.Empty;
                }
            }
        }
        return textBody?.Trim() ?? string.Empty;
    }

}