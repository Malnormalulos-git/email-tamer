using Amazon.S3;
using Amazon.S3.Util;
using EmailTamer.Core.Persistence;
using EmailTamer.Database.Tenant.Accessor;
using Microsoft.Extensions.Logging;

namespace EmailTamer.Parts.Sync.Persistence;

internal class TenantRepositoryFactory(
    IBlobStorage blobStorage,
    ILogger<ITenantRepository> logger,
    ITenantContextAccessor accessor,
    IAmazonS3 client
    ) : ITenantRepositoryFactory
{
    public async Task<ITenantRepository> Create(CancellationToken cancellationToken)
    {
        var bucketName = accessor.GetS3BucketName();
        
        if (!await AmazonS3Util.DoesS3BucketExistV2Async(client, bucketName))
        {
            await client.PutBucketAsync(bucketName, cancellationToken);
            logger.LogInformation("Initialized {BucketName} bucket", bucketName);
        }
        else
        {
            logger.LogInformation("Bucket {BucketName} already exists", bucketName);
        }
			
        return new TenantRepository(blobStorage, bucketName);
    }
}