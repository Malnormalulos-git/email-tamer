using EmailTamer.Database.Tenant.Entities;

namespace EmailTamer.Parts.Sync.Persistence;

public sealed class MessageAttachmentKey : TenantRepositoryKey
{
	public string FileName { get; init; } = null!;

	public static implicit operator string(MessageAttachmentKey key) => key.ToString();

	public override string ToString()
		=> base.ToString()
		   + $"attachments/{Uri.EscapeDataString(FileName)}";

	public static MessageAttachmentKey Create(string fileName,
												   Message message) =>
		new()
		{
			MessageId = message.Id,
			FileName = fileName
		};
}