namespace EmailTamer.Parts.Sync.Persistence;

public sealed record MessageAttachment(Stream Content, string FileName, string ContentType);