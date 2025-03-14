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
            .EasyMember(x => x.ResentDate, y => 
                y.ResentDate.UtcDateTime != DateTime.MinValue 
                    ? y.ResentDate.UtcDateTime 
                    : (DateTime?)null)
            .EasyMember(x => x.References, y => y.References.ToList())
            .EasyMember(x => x.TextBody, y => GetTextBody(y))
            .EasyMember(x => x.To, src => 
                src.To.OfType<MailboxAddress>()
                    .Select(ToEmailAddress))
            .EasyMember(x => x.From,src => 
                src.From.OfType<MailboxAddress>()
                    .Select(ToEmailAddress))
            .IgnoreMember(x => x.EmailBoxes)
            .IgnoreMember(x => x.Folders)
            .IgnoreMember(x => x.AttachmentFilesNames);
    }
    
    private static string GetTextBody(MimeMessage message)
    {
        var textBody = message.TextBody;
        if (string.IsNullOrEmpty(textBody))
        {
            var htmlBody = message.HtmlBody;
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

    private static EmailAddress ToEmailAddress(MailboxAddress mailboxAddress)
    {
        return new EmailAddress { Name = mailboxAddress.Name, Address = mailboxAddress.Address.ToLower(), Domain = mailboxAddress.Domain.ToLower() };
    }
}