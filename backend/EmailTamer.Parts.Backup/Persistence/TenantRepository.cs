using EmailTamer.Core.Persistence;
using EmailTamer.Core.Persistence.Exceptions;
using EmailTamer.Parts.Sync.Exceptions;

namespace EmailTamer.Parts.Sync.Persistence;

internal sealed class TenantRepository(
    IBlobStorage blobStorage,
    string bucketName)
    : ITenantRepository
{
    public Task SaveBodyAsync(MessageBodyKey key,
                              MessageBody body,
                              CancellationToken cancellationToken = default)
    {
        body.Content.Position = 0;

        var blob = new Blob
        {
            Content = body.Content,
            Name = key,
            ContentType = "text/html"
        };

        return blobStorage.SaveAsync(blob, CurrentBucketName(), cancellationToken);
    }

    public async Task<MessageBody> GetBodyAsync(MessageBodyKey key,
                                                    CancellationToken cancellationToken = default)
    {
        try
        {
            var blob = await blobStorage.GetAsync(key, CurrentBucketName(), cancellationToken);
            return new MessageBody(blob.Content);
        }
        catch (BlobNotFoundException e)
        {
            throw new MessageBodyNotFoundException(key, e);
        }
    }

    public Task DeleteBodyAsync(MessageBodyKey key, CancellationToken cancellationToken = default)
    {
        return blobStorage.DeleteAsync(key, CurrentBucketName(), cancellationToken);
    }

    public Task SaveAttachmentAsync(MessageAttachmentKey key,
                                    MessageAttachment attachment,
                                    CancellationToken cancellationToken = default)
    {
        attachment.Content.Position = 0;

        var blob = new Blob
        {
            Content = attachment.Content,
            Name = key,
            ContentType = attachment.ContentType
        };

        return blobStorage.SaveAsync(blob, CurrentBucketName(), cancellationToken);
    }

    public async Task<MessageAttachment> GetAttachmentAsync(MessageAttachmentKey key,
                                                                 CancellationToken cancellationToken = default)
    {
        try
        {
            var blob = await blobStorage.GetAsync(key, CurrentBucketName(), cancellationToken);
            var fileName = Uri.UnescapeDataString(GetFileName(blob.Name));
            return new MessageAttachment(blob.Content, fileName, blob.ContentType);
        }
        catch (BlobNotFoundException e)
        {
            throw new MessageAttachmentNotFoundException(key, e);
        }
    }

    public Task DeleteAttachmentAsync(MessageAttachmentKey key, CancellationToken cancellationToken = default)
    {
        return blobStorage.DeleteAsync(key, CurrentBucketName(), cancellationToken);
    }

    private static string GetFileName(string key)
    {
        var i = key.LastIndexOf('/');
        if (i != -1 && i < key.Length - 1)
        {
            return key[(i + 1)..];
        }
        return string.Empty;
    }

    private string CurrentBucketName() => bucketName;
}