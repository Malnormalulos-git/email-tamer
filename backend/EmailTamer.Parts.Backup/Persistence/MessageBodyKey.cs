using EmailTamer.Database.Tenant.Entities;

namespace EmailTamer.Parts.Sync.Persistence;

public sealed class MessageBodyKey : TenantRepositoryKey
{

    public static implicit operator string(MessageBodyKey key) => key.ToString();

    public override string ToString()
        => base.ToString()
           + "body.html";

    public static MessageBodyKey Create(Message message) =>
        new()
        {
            MessageId = message.Id
        };
}