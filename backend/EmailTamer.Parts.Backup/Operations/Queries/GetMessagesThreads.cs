using AutoMapper;
using EmailTamer.Core.Extensions;
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

public sealed record GetMessagesThreads(Guid[]? FoldersIds, Guid[]? EmailBoxesIds, int Page, int Size)
    : IRequest<IActionResult>, IPagedRequest
{
    public class Validator : AbstractValidator<GetMessagesThreads>
    {
        public Validator(IValidator<IPagedRequest> prValidator)
        {
            Include(prValidator);
        }
    }
}

[UsedImplicitly]
public class GetMessagesThreadsQueryHandler(
    [FromKeyedServices(nameof(TenantDbContext))]
    IEmailTamerRepository repository,
    IMapper mapper)
    : IRequestHandler<GetMessagesThreads, IActionResult>
{
    public async Task<IActionResult> Handle(GetMessagesThreads query, CancellationToken cancellationToken)
    {
        var filerByFolders = query.FoldersIds != null && query.FoldersIds.Length > 0;
        var filerByEmailBoxes = query.EmailBoxesIds != null && query.EmailBoxesIds.Length > 0;

        if (filerByFolders)
        {
            var folders = await repository.ReadAsync((r, ct) =>
                    r.Set<Folder>().Where(f => query.FoldersIds!.Contains(f.Id)).ToListAsync(ct),
                cancellationToken);
            if (folders.Count == 0) return new NotFoundResult();
        }

        if (filerByEmailBoxes)
        {
            var emailBoxes = await repository.ReadAsync((r, ct) =>
                    r.Set<EmailBox>().Where(eb => query.EmailBoxesIds!.Contains(eb.Id)).ToListAsync(ct),
                cancellationToken);
            if (emailBoxes.Count == 0) return new NotFoundResult();
        }

        var threadsPagedResult = await repository.ReadAsync(async (r, ct) =>
        {
            var baseQuery = r.Set<Message>()
                .WhereIf(filerByFolders, msg => msg.Folders.Any(f => query.FoldersIds!.Contains(f.Id)))
                .WhereIf(filerByEmailBoxes, msg => msg.EmailBoxes.Any(eb => query.EmailBoxesIds!.Contains(eb.Id)))
                .Where(m => m.ThreadId != null)
                .AsNoTracking();

            var threadIdsQuery = baseQuery
                .Select(m => m.ThreadId)
                .Distinct();

            var lastMessagesQuery = baseQuery
                .GroupBy(m => m.ThreadId)
                .Select(g => g.OrderByDescending(m => m.Date).First().Id);

            var firstMessagesQuery = baseQuery
                .GroupBy(m => m.ThreadId)
                .Select(g => g.OrderBy(m => m.Date).First().Id);

            var threadQuery = threadIdsQuery
                .Select(threadId => new
                {
                    ThreadId = threadId,
                    LastMessage = baseQuery
                        .Include(x => x.Folders)
                        .Include(x => x.EmailBoxes)
                        .FirstOrDefault(m => m.ThreadId == threadId && lastMessagesQuery.Contains(m.Id)),
                    FirstMessage = baseQuery
                        .Include(x => x.Folders)
                        .Include(x => x.EmailBoxes)
                        .FirstOrDefault(m => m.ThreadId == threadId && firstMessagesQuery.Contains(m.Id)),
                    Length = baseQuery.Count(m => m.ThreadId == threadId)
                })
                .OrderByDescending(t => t.LastMessage.Date);

            var totalCount = await threadIdsQuery.CountAsync(ct);
            var pagedThreads = await threadQuery
                .Skip((query.Page - 1) * query.Size)
                .Take(query.Size)
                .ToListAsync(ct);

            var result = pagedThreads.Select(t => new MessagesThreadShortDto
            {
                ThreadId = t.ThreadId,
                LastMessage = mapper.Map<MessageDto>(t.LastMessage),
                Subject = t.FirstMessage.Subject,
                StartDate = t.FirstMessage.Date,
                EndDate = t.LastMessage.Date,
                Length = t.Length
            }).ToList();

            return new PagedResult<MessagesThreadShortDto>(result, query.Page, query.Size, totalCount);
        }, cancellationToken);

        return new OkObjectResult(threadsPagedResult);
    }
}