using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EmailTamer.Core.Extensions;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Database.Tenant.Entities;
using EmailTamer.Parts.Sync.Exceptions;
using EmailTamer.Parts.Sync.Persistence;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace EmailTamer.Parts.Sync.Services;

internal class BackupService(
    IMapper mapper,
    ISystemClock clock,
    ILogger<BackupService> logger)
    : IBackupService
{
    private const int BatchSize = 100;

    public async Task<IActionResult> BackupEmailBoxAsync(
        Guid emailBoxId,
        IEmailTamerRepository repository,
        ITenantRepository tenantRepository,
        CancellationToken cancellationToken)
    {
        var (emailBox, backedUpMessages, existingFolders) =
            await LoadInitialData(emailBoxId, repository, cancellationToken);

        if (emailBox is null)
            return new NotFoundResult();

        emailBox.BackupStatus = BackupStatus.InProgress;
        repository.Update(emailBox);
        await repository.PersistAsync(cancellationToken);

        logger.LogInformation("Starting backup for EmailBox {EmailBoxId}", emailBoxId);

        try
        {
            using var client = await MailKitImapConnector.ConnectToImapClient(emailBox, cancellationToken);

            var folders = await GetRelevantFolders(client, cancellationToken);
            var synchronizationStartedAt = clock.UtcNow.DateTime;
            var newMessagesDictionary = new Dictionary<string, Message>();

            foreach (var folder in folders)
            {
                await ProcessFolder(
                    folder,
                    emailBox,
                    backedUpMessages,
                    existingFolders,
                    newMessagesDictionary,
                    repository,
                    tenantRepository,
                    cancellationToken);
            }

            await client.DisconnectAsync(true, cancellationToken);

            AssignThreadIds(backedUpMessages, newMessagesDictionary);

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

                emailBox.ConnectionFault = null;
                emailBox.LastSyncAt = synchronizationStartedAt;
                emailBox.BackupStatus = BackupStatus.Idle;
                repo.Update(emailBox);

                await repo.PersistAsync(ct);
            }, cancellationToken);

            logger.LogInformation("Backup completed for EmailBox {EmailBoxId}", emailBoxId);
            return new OkResult();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while backing up EmailBox {EmailBoxId}", emailBoxId);

            emailBox.BackupStatus = BackupStatus.Failed;
            repository.Update(emailBox);
            await repository.PersistAsync(cancellationToken);

            if (e is MailKitImapConnectorException mailKitImapConnectorException)
            {
                emailBox.ConnectionFault = mailKitImapConnectorException.Fault;
                repository.Update(emailBox);
                await repository.PersistAsync(cancellationToken);
                return new BadRequestObjectResult(mailKitImapConnectorException.Fault);
            }

            return new BadRequestResult();
        }
    }

    public async Task<IActionResult> BackupTenantEmailBoxesAsync(
        Guid[]? emailBoxesIds,
        IEmailTamerRepository repository,
        ITenantRepository tenantRepository,
        CancellationToken cancellationToken)
    {
        var filterByEmailBoxes = emailBoxesIds != null && emailBoxesIds.Length > 0;

        var emailBoxes = await repository.ReadAsync((r, ct) =>
                r.Set<EmailBox>()
                    .AsNoTracking()
                    .WhereIf(filterByEmailBoxes, x => emailBoxesIds!.Contains(x.Id))
                    .Where(x => x.BackupStatus != BackupStatus.Queued && x.BackupStatus != BackupStatus.InProgress)
                    .ToListAsync(ct),
            cancellationToken);

        if (!emailBoxes.Any())
        {
            logger.LogInformation("No email boxes to backup.");
            return new OkResult();
        }

        foreach (var emailBox in emailBoxes)
        {
            emailBox.BackupStatus = BackupStatus.Queued;
        }

        repository.UpdateRange(emailBoxes);
        await repository.PersistAsync(cancellationToken);

        foreach (var emailBox in emailBoxes)
        {
            try
            {
                var result = await BackupEmailBoxAsync(emailBox.Id, repository, tenantRepository, cancellationToken);
                if (result is not OkResult)
                {
                    logger.LogWarning("Backup failed for EmailBox {EmailBoxId}", emailBox.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during backup of EmailBox {EmailBoxId}", emailBox.Id);
            }
        }

        return new OkResult();
    }

    private async Task<(EmailBox? EmailBox, List<Message> BackedUpMessages, List<Folder> ExistingFolders)>
        LoadInitialData(Guid emailBoxId, IEmailTamerRepository repository, CancellationToken cancellationToken)
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
                .ToListAsync(ct);

            var folders = await r.Set<Folder>()
                .ToListAsync(ct);

            return (box, messages, folders);
        }, cancellationToken);
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
        IEmailTamerRepository repository,
        ITenantRepository tenantRepository,
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
                repository,
                tenantRepository,
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
        IEmailTamerRepository repository,
        ITenantRepository tenantRepository,
        CancellationToken cancellationToken)
    {
        var backedUpMessage = backedUpMessages
            .FirstOrDefault(x => x.Id == mimeMessage.MessageId);
        if (backedUpMessage != null)
        {
            await ProcessExistingMessage(backedUpMessage, folder, emailBox, existingFolders, repository);
            return;
        }

        if (!newMessagesDictionary.TryGetValue(mimeMessage.MessageId, out var newMessage))
        {
            newMessage = await CreateNewMessage(mimeMessage, emailBox, tenantRepository, cancellationToken);
            newMessagesDictionary[mimeMessage.MessageId] = newMessage;
        }

        await ProcessMessageFolder(newMessage, folder, existingFolders, repository);
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
        List<Folder> existingFolders,
        IEmailTamerRepository repository)
    {
        if (!string.IsNullOrEmpty(folder.Name) &&
            message.Folders.All(x => x.Name != folder.Name))
        {
            var existingFolder = existingFolders.FirstOrDefault(x => x.Name == folder.Name)
                                 ?? CreateNewFolder(folder.Name, existingFolders, repository);

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

    private Folder CreateNewFolder(string folderName, List<Folder> existingFolders, IEmailTamerRepository repository)
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
        ITenantRepository tenantRepository,
        CancellationToken cancellationToken)
    {
        var newMessage = mapper.Map<Message>(mimeMessage);
        newMessage.EmailBoxes.Add(emailBox);

        var saveToRepoTasks = new List<Task>();

        if (mimeMessage.HtmlBody != null)
        {
            saveToRepoTasks.Add(SaveMessageBody(mimeMessage, tenantRepository, cancellationToken));
        }

        await ProcessAttachments(mimeMessage, newMessage, saveToRepoTasks, tenantRepository, cancellationToken);
        await Task.WhenAll(saveToRepoTasks);

        return newMessage;
    }

    private Task SaveMessageBody(MimeMessage mimeMessage, ITenantRepository tenantRepository, CancellationToken cancellationToken)
    {
        return tenantRepository.SaveBodyAsync(
            new MessageBodyKey { MessageId = mimeMessage.MessageId },
            new MessageBody(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(mimeMessage.HtmlBody!))),
            cancellationToken);
    }

    private async Task ProcessAttachments(MimeMessage mimeMessage, Message newMessage, List<Task> saveToRepoTasks,
        ITenantRepository tenantRepository, CancellationToken cancellationToken)
    {
        foreach (var attachment in mimeMessage.Attachments)
        {
            if (!attachment.IsAttachment) continue;

            var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
            var content = ((MimePart)attachment).Content.Open();
            var contentStream = new MemoryStream();
            await content.CopyToAsync(contentStream, cancellationToken);

            var newAttachment = new Attachment
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                MessageId = newMessage.Id
            };

            saveToRepoTasks.Add(
                tenantRepository.SaveAttachmentAsync(
                    MessageAttachmentKey.Create(newAttachment, mimeMessage),
                    new MessageAttachment(
                        contentStream,
                        fileName,
                        attachment.ContentType.MimeType),
                    cancellationToken));

            newMessage.Attachments.Add(newAttachment);
        }
    }

    private Task ProcessMessageFolder(Message message, IMailFolder folder, List<Folder> existingFolders, IEmailTamerRepository repository)
    {
        if (string.IsNullOrEmpty(folder.Name))
            return Task.CompletedTask;

        var existingFolder = existingFolders.FirstOrDefault(x => x.Name == folder.Name)
                             ?? CreateNewFolder(folder.Name, existingFolders, repository);

        message.Folders.Add(existingFolder);
        return Task.CompletedTask;
    }
}