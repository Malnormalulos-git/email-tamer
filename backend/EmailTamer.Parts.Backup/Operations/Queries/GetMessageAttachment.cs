using System.Web;
using EmailTamer.Parts.Sync.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmailTamer.Parts.Sync.Operations.Queries;

public sealed record GetMessageAttachment(string MessageId, string FileName) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<GetMessageAttachment>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotNull().NotEmpty();
            RuleFor(x => x.FileName).NotNull().NotEmpty();
        }
    }
}

[UsedImplicitly]
internal class GetMessageAttachmentQueryHandler(ITenantRepository filesRepository)
    : IRequestHandler<GetMessageAttachment, IActionResult>
{
    public async Task<IActionResult> Handle(GetMessageAttachment request, CancellationToken cancellationToken)
    {
        var messageId = HttpUtility.UrlDecode(request.MessageId);
        var fileName = HttpUtility.UrlDecode(request.FileName);
        
        var messageAttachmentKey = new MessageAttachmentKey
        {
            MessageId = messageId,
            FileName = fileName
        };
        
        var attachment = await filesRepository.GetAttachmentAsync(messageAttachmentKey, cancellationToken);
         
        return new FileStreamResult(attachment.Content, attachment.ContentType)
        {
            FileDownloadName = fileName,
            EnableRangeProcessing = true
        };
    }
}