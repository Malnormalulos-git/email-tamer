using System.Web;
using AutoMapper;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Database.Tenant.Entities;
using EmailTamer.Parts.Sync.Models;
using EmailTamer.Parts.Sync.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.Sync.Operations.Queries;

public sealed record GetMessageDetails(string MessageId) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<GetMessageDetails>
    {
        public Validator()
        {
            RuleFor(x => x.MessageId).NotNull().NotEmpty();
        }
    }
}

[UsedImplicitly]
internal class GetMessageDetailsQueryHandler(
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    IMapper mapper,
    ITenantRepository filesRepository) 
    : IRequestHandler<GetMessageDetails, IActionResult>
{
    public async Task<IActionResult> Handle(GetMessageDetails request, CancellationToken cancellationToken)
    {
        var messageId = HttpUtility.UrlDecode(request.MessageId);
        
        var message = await repository.ReadAsync((r, ct) => 
                r.Set<Message>()
                    .AsNoTracking()
                    .Include(m => m.Attachments)
                    .FirstOrDefaultAsync(x => x.Id == messageId, ct),
            cancellationToken);
        
        if (message == null)
        {
            return new NotFoundResult();
        }
        
        var result = mapper.Map<MessageDetailsDto>(message);

        if (message.HasHtmlBody)
        {
            var messageBodyKey = MessageBodyKey.Create(message);
            var htmlBody = await filesRepository.GetBodyAsync(messageBodyKey, cancellationToken);

            if (htmlBody.Content.Length > 0)
            {
                using (var reader = new StreamReader(htmlBody.Content))
                {
                    result.HtmlBody = await reader.ReadToEndAsync(cancellationToken);
                }
            }
        }
        
        return new OkObjectResult(result);
    }
}