using EmailTamer.Core.Exceptions;

namespace EmailTamer.Core.Persistence.Exceptions;

public class BlobStorageException : EmailTamerApiException
{
    public BlobStorageException(string message) : base(message)
    {
    }

    public BlobStorageException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}