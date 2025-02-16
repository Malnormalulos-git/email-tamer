using AutoMapper;
using EmailTamer.Core.Extensions;
using EmailTamer.Database.Extensions;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Database.Tenant.Entities;
using EmailTamer.Database.Utilities.Paging;
using EmailTamer.Parts.Sync.Models;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.Sync.Operations.Queries;

public sealed record GetMessages(string? FolderName, int Page, int Size)
    : IRequest<IActionResult>, IPagedRequest
{
    public class Validator : AbstractValidator<GetMessages>
    {
        public Validator(IValidator<IPagedRequest> prValidator)
        {
            Include(prValidator);
        }
    }
}

[UsedImplicitly]
public class GetMessagesQueryHandler(
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    IMapper mapper)
    : IRequestHandler<GetMessages, IActionResult>
{
    public async Task<IActionResult> Handle(GetMessages query, CancellationToken cancellationToken)
    {
        if (query.FolderName != null)
        {
            var folderExists  = await repository.ReadAsync((r, ct) =>
                    r.Set<Folder>()
                        .AnyAsync(f => f.Name == query.FolderName, ct)
                , cancellationToken);
            
            if (!folderExists)
                return new NotFoundResult();
        }
        
        // TODO: refactor this
        var messagesPagedResult = await repository.ReadAsync(async (r, ct) =>
        {
            
            // Get all message IDs that are replies to other messages 
            var repliedToIds = await r.Set<Message>()
                .Where(m => m.InReplyTo != null)
                .Select(m => m.InReplyTo)
                .Distinct()
                .ToListAsync(ct);
            
            // Main query for paged messages
            var pagedResult = await r.Set<Message>()
                .Include(x => x.Folders)
                .WhereIf(query.FolderName != null, msg => msg.Folders.Any(f => f.Name == query.FolderName))
                .Where(m => m.InReplyTo == null || !repliedToIds.Contains(m.Id))
                .AsNoTracking()
                .ToPagedResultAsync(query, ct);
            
            var allReferences = pagedResult.Items
                .SelectMany(x => x.References)
                .Where(refId => !string.IsNullOrEmpty(refId))
                .Distinct()
                .ToList();
            
            var messagesReferredToIds = r.Set<Message>()
                .Where(msg => allReferences.Contains(msg.Id))
                .Select(x => x.Id)
                .AsNoTracking()
                .ToList();
            
            // Map results efficiently
            var mappedData = pagedResult.Items
                .Select(m =>
                {
                    var result = mapper.Map<MessageDto>(m);
            
                    result.Participants = m.From
                        .Select(x => !string.IsNullOrEmpty(x.Name) ? x.Name : x.Address)
                        .Concat(m.To.Select(x => !string.IsNullOrEmpty(x.Name) ? x.Name : x.Address))
                        .ToList();

                    result.ConversationChainLength = m.InReplyTo == null || 
                                                     m.References.Count == 0 || 
                                                     string.IsNullOrEmpty(m.References[0])
                        ? 1
                        : messagesReferredToIds.Count(id => m.References.Contains(id)) + 1;
            
                    return result;
                })
                .ToList();
            
            return new PagedResult<MessageDto>(
                mappedData,
                pagedResult.Page,
                pagedResult.Size,
                pagedResult.Total
            );
        }, cancellationToken);
            
        return new OkObjectResult(messagesPagedResult);
    }
}