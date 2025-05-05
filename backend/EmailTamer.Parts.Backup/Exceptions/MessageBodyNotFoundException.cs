using EmailTamer.Parts.Sync.Persistence;

namespace EmailTamer.Parts.Sync.Exceptions;

internal sealed class MessageBodyNotFoundException(MessageBodyKey key, System.Exception innerException)
    : System.Exception($"Message body with key {key} was not found in storage", innerException);