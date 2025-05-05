namespace EmailTamer.Core.Exceptions;

public abstract class EmailTamerApiException : ApplicationException
{
    protected EmailTamerApiException(string message) : base(message)
    {
    }

    protected EmailTamerApiException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public virtual int StatusCode { get; set; } = 500;
}