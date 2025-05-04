using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Parts.Sync.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.EmailBox.Operations.Commands;

public sealed record DeleteEmailBox(Guid Id) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<DeleteEmailBox>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotNull();
        }
    }
}

[UsedImplicitly]
public class DeleteEmailBoxCommandHandler(
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    ITenantRepository filesRepository)
    : IRequestHandler<DeleteEmailBox, IActionResult>
{
    public async Task<IActionResult> Handle(DeleteEmailBox command, CancellationToken cancellationToken)
    {
        
        var emailBoxToDelete = await repository.ReadAsync((r, ct) =>
                r.Set<Database.Tenant.Entities.EmailBox>()
                    .FirstOrDefaultAsync(x => x.Id == command.Id, ct),
            cancellationToken);

        if (emailBoxToDelete is null)
        {
            return new NotFoundResult();
        }
        
        var messagesToDelete = await repository.ReadAsync((r, ct) =>
                r.Set<Database.Tenant.Entities.Message>()
                    .Include(m => m.EmailBoxes)
                    .Where(m => m.EmailBoxes.Count == 1 && m.EmailBoxes.First().Id == command.Id)
                    .ToListAsync(ct)
            , cancellationToken);
        
        var repoDeletingTasks = new List<Task>();
        
        var messagesWithAttachments = messagesToDelete
            .Where(m => m.Attachments.Count > 0)
            .ToList();
        foreach (var message in messagesWithAttachments)
        {
            foreach (var attachment in message.Attachments)
            {
                repoDeletingTasks.Add(filesRepository.DeleteAttachmentAsync(
                    MessageAttachmentKey.Create(attachment, message), cancellationToken));
            }
        }
        
        var messagesWithHtmlBody = messagesToDelete
            .Where(m => m.HasHtmlBody)
            .ToList();
        foreach (var message in messagesWithHtmlBody)
        {
            repoDeletingTasks.Add(filesRepository.DeleteBodyAsync(
                new MessageBodyKey { MessageId = message.Id }, cancellationToken));
        }
        
        await Task.WhenAll(repoDeletingTasks);

        repository.RemoveRange(messagesToDelete);
        repository.Remove(emailBoxToDelete);
        await repository.PersistAsync(cancellationToken);

        return new OkResult();
    }
}