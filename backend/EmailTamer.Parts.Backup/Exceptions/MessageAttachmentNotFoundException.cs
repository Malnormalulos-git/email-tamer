using EmailTamer.Parts.Sync.Persistence;

namespace EmailTamer.Parts.Sync.Exceptions;

internal sealed class MessageAttachmentNotFoundException(MessageAttachmentKey key, System.Exception innerException)
    : System.Exception($"Message attachment with key {key} was not found in storage", innerException);