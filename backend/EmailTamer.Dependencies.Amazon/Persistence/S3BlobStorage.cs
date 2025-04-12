using Amazon.S3;
using Amazon.S3.Model;
using EmailTamer.Core.Extensions;
using EmailTamer.Core.Persistence;
using EmailTamer.Core.Persistence.Exceptions;

namespace EmailTamer.Dependencies.Amazon.Persistence;

internal sealed class S3BlobStorage(IAmazonS3 s3Client) : IBlobStorage
{
	public async Task SaveAsync(Blob blob, string storageName, CancellationToken cancellationToken = default)
	{

		var request = new PutObjectRequest
		{
			BucketName = storageName,
			Key = blob.Name,
			InputStream = blob.Content,
			ContentType = blob.ContentType
		};
		foreach (var (key, value) in blob.Metadata)
		{
			request.Metadata.Add(key, value);
		}

		await s3Client.PutObjectAsync(request, cancellationToken);
	}

	public Task<Blob> GetAsync(string key, string storageName, CancellationToken cancellationToken = default)
		=> ExecuteInErrorBoundaryAsync(async (s, k, token) =>
		{
			var response = await s3Client.GetObjectAsync(s, k, token);

			return new Blob
			{
				Content = response.ResponseStream,
				Metadata = response.Metadata.Map(m => m.Keys.ToDictionary(x => x, x => m[x])),
				Name = k,
				ContentType = response.Headers.ContentType
			};
		}, key, storageName, cancellationToken);

	public Task DeleteAsync(string key, string storageName, CancellationToken cancellationToken = default)
		=> ExecuteInErrorBoundaryAsync(s3Client.DeleteObjectAsync,
			key,
			storageName,
			cancellationToken);

	private static async Task<T> ExecuteInErrorBoundaryAsync<T>(Func<string, string, CancellationToken, Task<T>> func,
														 string key,
														 string storageName,
														 CancellationToken cancellationToken)
	{
		try
		{
			return await func(storageName, key, cancellationToken);
		}
		catch (AmazonS3Exception e)
		{
			throw e.ErrorCode switch
			{
				"NoSuchKey" => new BlobNotFoundException(storageName, key, e),
				_ => new BlobStorageException("Blob storage action ended with an error", e)
			};
		}
	}
}