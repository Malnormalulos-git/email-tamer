using EmailTamer.Database.Tenant.Entities;

namespace EmailTamer.Parts.Sync.Exceptions;

public sealed class MailKitImapConnectorException(ConnectionFault fault, Exception innerException)
    : Exception($"An exception has been occured while connecting to ImapClient: {fault.ToString()}", innerException)
{
    public ConnectionFault Fault => fault;
}