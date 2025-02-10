namespace EmailTamer.Core.Persistence.Exceptions;

public sealed class BlobNotFoundException : BlobStorageException
{
	public BlobNotFoundException(string message) : base(message)
	{
	}

	public BlobNotFoundException(string storageName, string key, Exception innerException) : base($"Blob '{key}' has not been found in storage '{storageName}'", innerException)
	{
	}
}