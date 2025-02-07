using AutoMapper;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Database.Tenant.Entities;
using FluentValidation;
using HtmlAgilityPack;
using JetBrains.Annotations;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Internal;
using MimeKit;

namespace EmailTamer.Parts.Sync.Operations.Commands;

public sealed record SyncEmailBox(Guid EmailBoxId) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<SyncEmailBox>
    {
        public Validator()
        {
            RuleFor(x => x.EmailBoxId).NotNull();
        }
    }
}

[UsedImplicitly]
public class SyncEmailBoxCommandHandler(
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    IMapper mapper,
    ISystemClock clock)
    : IRequestHandler<SyncEmailBox, IActionResult>
{
    public async Task<IActionResult> Handle(SyncEmailBox command, CancellationToken cancellationToken)
    {
        var emailBox = await repository.ReadAsync((r, ct) =>
                r.Set<EmailBox>()
                    .FirstOrDefaultAsync(x => x.Id == command.EmailBoxId, ct),
            cancellationToken);
        
        if (emailBox is null)
        {
            return new NotFoundResult();
        }
        
        var backupedMessages = await repository.ReadAsync((r, ct) =>
                r.Set<Message>()
                    .Where(x => x.EmailBoxId == emailBox.Id)
                    .ToListAsync(ct),
            cancellationToken);
        
        using var client = new ImapClient();
        await client.ConnectAsync(
            emailBox.EmailDomainConnectionHost, 
            emailBox.EmailDomainConnectionPort, 
            emailBox.UseSSl, cancellationToken);  
        await client.AuthenticateAsync(
            emailBox.AuthenticateByEmail ? emailBox.Email : emailBox.UserName, 
            emailBox.Password, cancellationToken);
        
        var newMessagesDictionary = new Dictionary<string, Message>();
        
        var namespaces = client.PersonalNamespaces;
        
        var rootNamespace = namespaces.Count > 0 ? namespaces[0] : new FolderNamespace('.', "");
        
        // we need all folders to figure out what folders each message is in
        var folders = (await client.GetFoldersAsync(rootNamespace, cancellationToken: cancellationToken))
            .Where(f => !f.Attributes.HasFlag(FolderAttributes.NonExistent))
            .Where(f => !f.Attributes.HasFlag(FolderAttributes.Drafts))
            .Where(f => !f.Attributes.HasFlag(FolderAttributes.Trash))
            .Where(f => !f.Attributes.HasFlag(FolderAttributes.Junk))
            .Where(f => !f.Attributes.HasFlag(FolderAttributes.All))
            .ToList();
        
        // for cases when inbox is not in root namespace, or it filtered out
        if (!folders.Contains(client.Inbox)) 
        {
            folders.Add(client.Inbox);
        }
        
        emailBox.LastSyncAt = clock.UtcNow.DateTime;
        
        foreach (var folder in folders)
        {
            await folder.OpenAsync(FolderAccess.ReadOnly, cancellationToken);
            
            var messages = await folder.SearchAsync(SearchQuery.All, cancellationToken);
        
            foreach (var message in messages)
            {
                var mimeMessage = await folder.GetMessageAsync(message, cancellationToken);

                var backupedMessage = backupedMessages
                    .FirstOrDefault(x => x.Id == mimeMessage.MessageId);
                
                if (backupedMessage is not null)
                {
                    if (!backupedMessage.Folders.Contains(folder.Name))
                    {
                        backupedMessage.Folders.Add(folder.Name);
                        repository.Update(backupedMessage);
                        await repository.PersistAsync(cancellationToken);
                    }
                    continue;
                }
                
                if (!newMessagesDictionary.TryGetValue(mimeMessage.MessageId, out var newMessage))
                {
                    newMessage = mapper.Map<Message>(mimeMessage);

                    newMessage.S3FolderName = "Test"; //TODO

                    newMessage.EmailBox = emailBox;
                    newMessage.EmailBoxId = emailBox.Id;
                    
                    newMessagesDictionary[mimeMessage.MessageId] = newMessage;
                }
        
                if(!folder.Attributes.HasFlag(FolderAttributes.All))
                {
                    newMessage.Folders.Add(folder.Name);
                }
            }
            
            await folder.CloseAsync(cancellationToken: cancellationToken);
        }
        
        await client.DisconnectAsync(true, cancellationToken);

        if (newMessagesDictionary.Count > 0)
        {
            repository.AddRange(newMessagesDictionary.Values.ToList());
        }
        
        repository.Update(emailBox);
        
        await repository.PersistAsync(cancellationToken);

        return new OkResult();
    }
}