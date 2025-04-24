using System.Data;
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
using Microsoft.Extensions.Logging;
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
    ITenantRepository filesRepository,
    ILogger<BackUpEmailBoxMessagesCommandHandler> logger)
    : IRequestHandler<BackUpEmailBoxMessages, IActionResult>
{
    private const int BatchSize = 100;

    public async Task<IActionResult> Handle(BackUpEmailBoxMessages command, CancellationToken cancellationToken)
    {
        var (emailBox, backedUpMessages, existingFolders) =
            await LoadInitialData(command.EmailBoxId, cancellationToken);

        if (emailBox is null)
            return new NotFoundResult();

        logger.LogInformation("Starting backup for EmailBox {EmailBoxId}", command.EmailBoxId);

        try
        {
            using var client = await ConnectToImapClient(emailBox, cancellationToken);

            // we need all relevant folders to figure out what folders each message is in
            var folders = await GetRelevantFolders(client, cancellationToken);

            emailBox.LastSyncAt = clock.UtcNow.DateTime;
            var newMessagesDictionary = new Dictionary<string, Message>();

            foreach (var folder in folders)
            {
                await ProcessFolder(
                    folder,
                    emailBox,
                    backedUpMessages,
                    existingFolders,
                    newMessagesDictionary,
                    cancellationToken);
            }

            AssignThreadIds(backedUpMessages, newMessagesDictionary);

            await client.DisconnectAsync(true, cancellationToken);

            await repository.WriteInTransactionAsync(IsolationLevel.ReadCommitted, async (repo, ct) =>
            {
                if (newMessagesDictionary.Count > 0)
                {
                    var messages = newMessagesDictionary.Values.ToList();

                    for (var i = 0; i < messages.Count; i += BatchSize)
                    {
                        var batch = messages.Skip(i).Take(BatchSize).ToList();
                        repo.AddRange(batch);
                    }
                }

                repo.Update(emailBox);
                await repo.PersistAsync(ct);
            }, cancellationToken);

            logger.LogInformation("Backup completed for EmailBox {EmailBoxId}", command.EmailBoxId);
            return new OkResult();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while backing up EmailBox {EmailBoxId}", command.EmailBoxId);
            return new BadRequestResult();
        }
    }

    private async Task<(EmailBox? EmailBox, List<Message> BackedUpMessages, List<Folder> ExistingFolders)>
        LoadInitialData(Guid emailBoxId, CancellationToken cancellationToken)
    {
        return await repository.ReadAsync(async (r, ct) =>
        {
            var box = await r.Set<EmailBox>()
                .FirstOrDefaultAsync(x => x.Id == emailBoxId, ct);

            if (box == null)
                return (null, [], []);

            var messages = await r.Set<Message>()
                .Include(x => x.EmailBoxes)
                .Include(x => x.Folders)
                // not .Where(x => x.EmailBoxId == emailBox.Id) for cases when emailBoxes with shared messages are backed up
                .ToListAsync(ct);

            var folders = await r.Set<Folder>()
                .ToListAsync(ct);

            return (box, messages, folders);
        }, cancellationToken);
    }

    private async Task<IImapClient> ConnectToImapClient(EmailBox emailBox, CancellationToken cancellationToken)
    {
        var client = new ImapClient();
        await client.ConnectAsync(
            emailBox.EmailDomainConnectionHost,
            emailBox.EmailDomainConnectionPort,
            emailBox.UseSSl,
            cancellationToken);

        await client.AuthenticateAsync(
            emailBox.AuthenticateByEmail ? emailBox.Email : emailBox.UserName,
            emailBox.Password,
            cancellationToken);

        return client;
    }

    private async Task<List<IMailFolder>> GetRelevantFolders(IImapClient client, CancellationToken cancellationToken)
    {
        var rootNamespace = client.PersonalNamespaces.Count > 0
            ? client.PersonalNamespaces[0]
            : new FolderNamespace('.', "");

        var folders = (await client.GetFoldersAsync(rootNamespace, cancellationToken: cancellationToken))
            .Where(f =>
                !f.Attributes.HasFlag(FolderAttributes.NonExistent) &&
                !f.Attributes.HasFlag(FolderAttributes.Drafts) &&
                !f.Attributes.HasFlag(FolderAttributes.Trash) &&
                !f.Attributes.HasFlag(FolderAttributes.Junk) &&
                !f.Attributes.HasFlag(FolderAttributes.All))
            .ToList();

        // for cases when inbox is not in root namespace, or it filtered out
        if (!folders.Contains(client.Inbox))
        {
            folders.Add(client.Inbox);
        }

        return folders;
    }

    private async Task ProcessFolder(
        IMailFolder folder,
        EmailBox emailBox,
        List<Message> backedUpMessages,
        List<Folder> existingFolders,
        Dictionary<string, Message> newMessagesDictionary,
        CancellationToken cancellationToken)
    {
        await folder.OpenAsync(FolderAccess.ReadOnly, cancellationToken);
        var messages = await folder.SearchAsync(SearchQuery.All, cancellationToken);

        foreach (var messageIndex in messages)
        {
            var mimeMessage = await folder.GetMessageAsync(messageIndex, cancellationToken);
            await ProcessMessage(
                mimeMessage,
                folder,
                emailBox,
                backedUpMessages,
                existingFolders,
                newMessagesDictionary,
                cancellationToken);
        }

        await folder.CloseAsync(cancellationToken: cancellationToken);
    }

    private async Task ProcessMessage(
        MimeMessage mimeMessage,
        IMailFolder folder,
        EmailBox emailBox,
        List<Message> backedUpMessages,
        List<Folder> existingFolders,
        Dictionary<string, Message> newMessagesDictionary,
        CancellationToken cancellationToken)
    {
        var backedUpMessage = backedUpMessages
            .FirstOrDefault(x => x.Id == mimeMessage.MessageId);
        if (backedUpMessage != null)
        {
            await ProcessExistingMessage(backedUpMessage, folder, emailBox, existingFolders);
            return;
        }

        if (!newMessagesDictionary.TryGetValue(mimeMessage.MessageId, out var newMessage))
        {
            newMessage = await CreateNewMessage(mimeMessage, emailBox, cancellationToken);
            newMessagesDictionary[mimeMessage.MessageId] = newMessage;
        }

        await ProcessMessageFolder(newMessage, folder, existingFolders);
    }

    private void AssignThreadIds(List<Message> backedUpMessages, Dictionary<string, Message> newMessagesDictionary)
    {
        var allMessages = backedUpMessages.Concat(newMessagesDictionary.Values).ToList();
        var messageLookup = allMessages.ToDictionary(m => m.Id, m => m);

        foreach (var message in newMessagesDictionary.Values)
        {
            if (string.IsNullOrEmpty(message.InReplyTo) &&
                (message.References.Count == 0))
            {
                message.ThreadId = message.Id;
                continue;
            }

            string threadId = null;

            if (!string.IsNullOrEmpty(message.InReplyTo) &&
                messageLookup.TryGetValue(message.InReplyTo, out var repliedTo))
            {
                threadId = repliedTo.ThreadId ?? repliedTo.Id;
            }

            if (threadId == null && message.References.Count > 0)
            {
                foreach (var refId in message.References)
                {
                    if (messageLookup.TryGetValue(refId, out var refMessage))
                    {
                        threadId = refMessage.ThreadId ?? refId;
                        break;
                    }
                }
            }

            message.ThreadId = threadId ?? message.Id;
        }
    }

    private Task ProcessExistingMessage(
        Message message,
        IMailFolder folder,
        EmailBox emailBox,
        List<Folder> existingFolders)
    {
        if (!string.IsNullOrEmpty(folder.Name) &&
            message.Folders.All(x => x.Name != folder.Name))
        {
            var existingFolder = existingFolders.FirstOrDefault(x => x.Name == folder.Name)
                                 ?? CreateNewFolder(folder.Name, existingFolders);

            message.Folders.Add(existingFolder);
            repository.Update(message);
        }

        if (!message.EmailBoxes.Contains(emailBox))
        {
            message.EmailBoxes.Add(emailBox);
            repository.Update(message);
        }

        return Task.CompletedTask;
    }

    private Folder CreateNewFolder(string folderName, List<Folder> existingFolders)
    {
        var newFolder = new Folder
        {
            Id = Guid.NewGuid(),
            Name = folderName
        };

        existingFolders.Add(newFolder);
        repository.Add(newFolder);
        return newFolder;
    }

    private async Task<Message> CreateNewMessage(
        MimeMessage mimeMessage,
        EmailBox emailBox,
        CancellationToken cancellationToken)
    {
        var newMessage = mapper.Map<Message>(mimeMessage);
        newMessage.EmailBoxes.Add(emailBox);

        var saveToRepoTasks = new List<Task>();

        if (mimeMessage.HtmlBody != null)
        {
            saveToRepoTasks.Add(SaveMessageBody(mimeMessage, cancellationToken));
        }

        await ProcessAttachments(mimeMessage, newMessage, saveToRepoTasks, cancellationToken);
        await Task.WhenAll(saveToRepoTasks);

        return newMessage;
    }

    private Task SaveMessageBody(MimeMessage mimeMessage, CancellationToken cancellationToken)
    {
        return filesRepository.SaveBodyAsync(
            new MessageBodyKey { MessageId = mimeMessage.MessageId },
            new MessageBody(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(mimeMessage.HtmlBody!))),
            cancellationToken);
    }

    private async Task ProcessAttachments(MimeMessage mimeMessage, Message newMessage, List<Task> saveToRepoTasks,
        CancellationToken cancellationToken)
    {
        foreach (var attachment in mimeMessage.Attachments)
        {
            if (!attachment.IsAttachment) continue;

            var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
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
                        attachment.ContentType.MimeType),
                    cancellationToken));

            newMessage.AttachmentFilesNames.Add(fileName);
        }
    }

    private Task ProcessMessageFolder(Message message, IMailFolder folder, List<Folder> existingFolders)
    {
        if (string.IsNullOrEmpty(folder.Name))
            return Task.CompletedTask;

        var existingFolder = existingFolders.FirstOrDefault(x => x.Name == folder.Name)
                             ?? CreateNewFolder(folder.Name, existingFolders);

        message.Folders.Add(existingFolder);
        return Task.CompletedTask;
    }
}