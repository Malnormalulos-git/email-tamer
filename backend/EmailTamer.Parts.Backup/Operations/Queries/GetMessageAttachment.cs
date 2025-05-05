using System.Web;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Database.Tenant.Entities;
using EmailTamer.Parts.Sync.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.Sync.Operations.Queries;

public sealed record GetMessageAttachment(string MessageId, string AttachmentId) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<GetMessageAttachment>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotNull().NotEmpty();
            RuleFor(x => x.AttachmentId).NotNull().NotEmpty();
        }
    }
}

[UsedImplicitly]
internal class GetMessageAttachmentQueryHandler(
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    ITenantRepository filesRepository
    ) : IRequestHandler<GetMessageAttachment, IActionResult>
{
    public async Task<IActionResult> Handle(GetMessageAttachment request, CancellationToken cancellationToken)
    {
        var messageId = request.MessageId;
        var attachmentId = request.AttachmentId;

        var message = await repository.ReadAsync((r, ct) =>
                r.Set<Message>()
                    .AsNoTracking()
                    .Include(m => m.Attachments)
                    .FirstOrDefaultAsync(m => m.Id == messageId, ct)
            , cancellationToken);

        if (message == null)
            return new NotFoundResult();

        var attachmentEntity = message.Attachments.FirstOrDefault(a => a.Id == Guid.Parse(attachmentId));

        if (attachmentEntity == null)
            return new NotFoundResult();

        var messageAttachmentKey = MessageAttachmentKey.Create(attachmentEntity, message);

        var attachment = await filesRepository.GetAttachmentAsync(messageAttachmentKey, cancellationToken);

        return new FileStreamResult(attachment.Content, attachment.ContentType)
        {
            FileDownloadName = attachmentEntity.FileName,
            EnableRangeProcessing = true
        };
    }
}