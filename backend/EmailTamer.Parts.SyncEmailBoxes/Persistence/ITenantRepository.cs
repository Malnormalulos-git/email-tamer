namespace EmailTamer.Parts.Sync.Persistence;

internal interface ITenantRepository
{
	public Task SaveBodyAsync(MessageBodyKey key,
		MessageBody body,
		CancellationToken cancellationToken = default);

	public Task<MessageBody> GetBodyAsync(MessageBodyKey key,
		CancellationToken cancellationToken = default);
	
	public Task SaveAttachmentAsync(MessageAttachmentKey key,
									MessageAttachment attachment,
									CancellationToken cancellationToken = default);

	public Task<MessageAttachment> GetAttachmentAsync(MessageAttachmentKey key,
														   CancellationToken cancellationToken = default);
}