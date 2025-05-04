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

public sealed record GetMessagesThread(string MessageId)  : IRequest<IActionResult>
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
internal class GetMessagesThreadQueryHandler(
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    IMapper mapper,
    ITenantRepository filesRepository)
    : IRequestHandler<GetMessagesThread, IActionResult>
{
    public async Task<IActionResult> Handle(GetMessagesThread query, CancellationToken cancellationToken)
    {
        var messageId = HttpUtility.UrlDecode(query.MessageId);
        
        var thread = await repository.ReadAsync(async (r, ct) =>
        {
            var message = await r.Set<Message>()
                .AsNoTracking()
                .Where(m => m.Id == messageId)
                .Select(m => new { m.ThreadId })
                .FirstOrDefaultAsync(ct);

            if (message == null || string.IsNullOrEmpty(message.ThreadId))
                return null;

            var threadMessages = await r.Set<Message>()
                .AsNoTracking()
                .Include(m => m.Attachments)
                .Where(m => m.ThreadId == message.ThreadId)
                .OrderBy(m => m.Date)
                .AsNoTracking()
                .ToListAsync(ct);

            return threadMessages;
        }, cancellationToken);

        if (thread == null)
            return new NotFoundResult();

        var lastMessage = thread.Last();
        var lastMessageDto = mapper.Map<MessageDetailsDto>(lastMessage);

        if (lastMessage.HasHtmlBody)
        {
            var messageBodyKey = MessageBodyKey.Create(lastMessage);
            var htmlBody = await filesRepository.GetBodyAsync(messageBodyKey, cancellationToken);

            if (htmlBody.Content.Length > 0)
            {
                using (var reader = new StreamReader(htmlBody.Content))
                {
                    lastMessageDto.HtmlBody = await reader.ReadToEndAsync(cancellationToken);
                }
            }
        }

        var messagesDto = thread
            .Take(thread.Count - 1) 
            .Select(m => mapper.Map<MessageDto>(m))
            .ToList();

        var threadDto = new MessagesThreadDto
        {
            ThreadId = thread.First().ThreadId,
            Messages = messagesDto,
            Subject = thread.First().Subject,
            LastMessage = lastMessageDto
        };

        return new OkObjectResult(threadDto);
    }
}