using EmailTamer.Parts.Sync.Models;
using EmailTamer.Parts.Sync.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmailTamer.Parts.Sync.Operations.Queries;

public sealed record GetMessageAttachment(GetMessageAttachmentDto GetMessageAttachmentDto) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<GetMessageAttachment>
    {
        public Validator(IValidator<GetMessageAttachmentDto> validator)
        { 
            RuleFor(x => x.GetMessageAttachmentDto).SetValidator(validator);
        }
    }
}

[UsedImplicitly]
internal class GetMessageAttachmentQueryHandler(ITenantRepository filesRepository)
    : IRequestHandler<GetMessageAttachment, IActionResult>
{
    public async Task<IActionResult> Handle(GetMessageAttachment request, CancellationToken cancellationToken)
    {
        var messageAttachmentKey = new MessageAttachmentKey
        {
            MessageId = request.GetMessageAttachmentDto.MessageId,
            FileName = request.GetMessageAttachmentDto.FileName
        };
        
        var attachment = await filesRepository.GetAttachmentAsync(messageAttachmentKey, cancellationToken);
         
        return new FileStreamResult(attachment.Content, attachment.ContentType)
        {
            FileDownloadName = request.GetMessageAttachmentDto.FileName,
            EnableRangeProcessing = true
        };
    }
}