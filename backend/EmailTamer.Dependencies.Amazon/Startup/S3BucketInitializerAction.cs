using Amazon.S3;
using Amazon.S3.Util;
using EmailTamer.Core.Startup;
using EmailTamer.Dependencies.Amazon.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmailTamer.Dependencies.Amazon.Startup;

public abstract class S3BucketInitializerAction<T>(
	ILogger<S3BucketInitializerAction<T>> logger,
	IOptionsMonitor<T> configMonitor,
	IAmazonS3 client)
	: IAsyncStartupAction
	where T : IHasBucketName
{
	public uint Order => 0;

	public Task PerformActionAsync(CancellationToken cancellationToken = default)
		=> EnsureBucketExistsAsync(configMonitor.CurrentValue.BucketName, cancellationToken);

	private async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken)
	{
		if (!await AmazonS3Util.DoesS3BucketExistV2Async(client, bucketName))
		{
			await client.PutBucketAsync(bucketName, cancellationToken);
			logger.LogInformation("Initialized {BucketName} bucket", bucketName);
			return;
		}

		logger.LogInformation("Bucket {BucketName} already exists", bucketName);
	}
}