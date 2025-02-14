using AutoMapper;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Database.Tenant.Entities;
using EmailTamer.Parts.Sync.Persistence;
using FluentValidation;
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

public sealed record BackUpEmailBoxMessages(Guid EmailBoxId) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<BackUpEmailBoxMessages>
    {
        public Validator()
        {
            RuleFor(x => x.EmailBoxId).NotNull();
        }
    }
}

[UsedImplicitly]
internal class BackUpEmailBoxMessagesCommandHandler(
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    IMapper mapper,
    ISystemClock clock,
    ITenantRepository filesRepository)
    : IRequestHandler<BackUpEmailBoxMessages, IActionResult>
{
    public async Task<IActionResult> Handle(BackUpEmailBoxMessages command, CancellationToken cancellationToken)
    {
        var emailBox = await repository.ReadAsync((r, ct) =>
                r.Set<EmailBox>()
                    .FirstOrDefaultAsync(x => x.Id == command.EmailBoxId, ct),
            cancellationToken);
        
        if (emailBox is null)
        {
            return new NotFoundResult();
        }
        
        var backedUpMessages = await repository.ReadAsync((r, ct) =>
                r.Set<Message>()
                    .Include(x => x.EmailBoxes)
                    // not .Where(x => x.EmailBoxId == emailBox.Id) for cases when emailBoxes with shared messages are backed up 
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
        
        // var saveToRepoTasks = new List<Task>();
        
        foreach (var folder in folders)
        {
            await folder.OpenAsync(FolderAccess.ReadOnly, cancellationToken);
            
            var messages = await folder.SearchAsync(SearchQuery.All, cancellationToken);
        
            foreach (var message in messages)
            {
                var mimeMessage = await folder.GetMessageAsync(message, cancellationToken);

                var backedUpMessage = backedUpMessages
                    .FirstOrDefault(x => x.Id == mimeMessage.MessageId);
                
                if (backedUpMessage is not null)
                {
                    if (!backedUpMessage.Folders.Contains(folder.Name))
                    {
                        backedUpMessage.Folders.Add(folder.Name);
                        repository.Update(backedUpMessage);
                    }

                    if (!backedUpMessage.EmailBoxes.Contains(emailBox))
                    {
                        backedUpMessage.EmailBoxes.Add(emailBox);
                        repository.Update(backedUpMessage);
                    }
                    continue;
                }
                
                if (!newMessagesDictionary.TryGetValue(mimeMessage.MessageId, out var newMessage))
                {
                    newMessage = mapper.Map<Message>(mimeMessage);
                    
                    newMessage.EmailBoxes.Add(emailBox);

                    var saveToRepoTasks = new List<Task>();
                    
                    if (mimeMessage.HtmlBody is not null)
                    {
                        saveToRepoTasks.Add(
                             filesRepository.SaveBodyAsync(
                                new MessageBodyKey
                                {
                                    MessageId = mimeMessage.MessageId
                                },
                                new MessageBody(
                                    new MemoryStream(System.Text.Encoding.UTF8.GetBytes(mimeMessage.HtmlBody))
                                ),
                                cancellationToken
                            ));
                    }
                    
                    foreach (var attachment in mimeMessage.Attachments)
                    {
                        if (attachment.IsAttachment)
                        {
                            var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                            var contentType = attachment.ContentType.MimeType;
                            var content = ((MimePart)attachment).Content.Open();
                            var contentStream = new MemoryStream();
                            await content.CopyToAsync(contentStream, cancellationToken);
                                
                            saveToRepoTasks.Add(
                                filesRepository.SaveAttachmentAsync(
                                    new MessageAttachmentKey
                                    {
                                        MessageId = mimeMessage.MessageId,
                                        FileName = fileName
                                    },
                                    new MessageAttachment(
                                        contentStream,
                                        fileName,
                                        contentType),
                                    cancellationToken
                                ));
                            
                            newMessage.AttachmentFilesNames.Add(fileName);
                        }
                    }
                    
                    newMessagesDictionary[mimeMessage.MessageId] = newMessage;
                    
                    await Task.WhenAll(saveToRepoTasks);
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
            var messagesToAdd = newMessagesDictionary.Values.ToList();

            // Look at BackUpEmailBoxMessages command
            // var alreadyTrackedMessages = repository.ChangeTrackerEntries<Message>()
            //     .Where(e => messagesToAdd.Contains(e.Entity))
            //     .Select(x => x.Entity)
            //     .ToList();
            //
            // if (alreadyTrackedMessages.Count > 0)
            // {
            //     messagesToAdd.RemoveAll(x => alreadyTrackedMessages.Contains(x));
            //     foreach (var alreadyTrackedMessage in alreadyTrackedMessages)
            //     {
            //         alreadyTrackedMessage.Folders.AddRange(
            //             newMessagesDictionary[alreadyTrackedMessage.Id].Folders
            //                 .Where(folder => !alreadyTrackedMessage.Folders.Contains(folder))
            //         );
            //         repository.Update(alreadyTrackedMessage);
            //     }
            // }
            
            repository.AddRange(messagesToAdd);
        }
        
        repository.Update(emailBox);
        
        await repository.PersistAsync(cancellationToken);
        
        // await Task.WhenAll(saveToRepoTasks);

        return new OkResult();
    }
}