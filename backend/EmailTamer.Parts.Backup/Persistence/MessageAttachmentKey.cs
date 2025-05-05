using EmailTamer.Database.Tenant.Entities;
using MimeKit;

namespace EmailTamer.Parts.Sync.Persistence;

public sealed class MessageAttachmentKey : TenantRepositoryKey
{
    public string AttachmentId { get; init; } = null!;

    public static implicit operator string(MessageAttachmentKey key) => key.ToString();

    public override string ToString()
        => base.ToString()
           + $"attachments/{Uri.EscapeDataString(AttachmentId)}";

    public static MessageAttachmentKey Create(Attachment attachment, Message message) =>
        new()
        {
            MessageId = message.Id,
            AttachmentId = attachment.Id.ToString()
        };

    public static MessageAttachmentKey Create(Attachment attachment, MimeMessage message) =>
        new()
        {
            MessageId = message.MessageId,
            AttachmentId = attachment.Id.ToString()
        };
}