using System.Net.Sockets;
using System.Security.Authentication;
using EmailTamer.Database.Tenant.Entities;
using EmailTamer.Parts.Sync.Exceptions;
using MailKit.Net.Imap;

namespace EmailTamer.Parts.Sync.Services;

public static class MailKitImapConnector
{
    public static async Task<IImapClient> ConnectToImapClient(EmailBox emailBox, CancellationToken cancellationToken)
    {
        var client = new ImapClient();

        try
        {
            await client.ConnectAsync(
                emailBox.EmailDomainConnectionHost,
                emailBox.EmailDomainConnectionPort,
                emailBox.UseSSl,
                cancellationToken);
        }
        catch (Exception e)
        {
            if (e is System.Net.Sockets.SocketException se)
                throw new MailKitImapConnectorException(ConnectionFault.ConnectionRefused, se);
            
            if (e is System.ArgumentOutOfRangeException auore)
                throw new MailKitImapConnectorException(ConnectionFault.PortOutOfRange, auore);
            
            throw new MailKitImapConnectorException(ConnectionFault.Other, e);
        }

        try
        {
            await client.AuthenticateAsync(
                emailBox.AuthenticateByEmail ? emailBox.Email : emailBox.UserName,
                emailBox.Password,
                cancellationToken);
            
        }
        catch (Exception e)
        {
            if (e is MailKit.Security.AuthenticationException ae)
                throw new MailKitImapConnectorException(ConnectionFault.WrongAuthenticationCredentials, ae);
            
            throw new MailKitImapConnectorException(ConnectionFault.Other, e);
        }
        

        return client;
    }
}