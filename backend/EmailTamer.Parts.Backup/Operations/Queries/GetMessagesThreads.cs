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

public sealed record GetMessagesThreads(
    Guid? FolderId,
    Guid[]? EmailBoxesIds,
    string? SearchTerm,
    int Page,
    int Size,
    bool IsByDescending = true)
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
        var filerByFolder = query.FolderId != null;
        var filerByEmailBoxes = query.EmailBoxesIds != null && query.EmailBoxesIds.Length > 0;
        var searchTerm = query.SearchTerm?.ToUpper();

        if (filerByFolder)
        {
            var folder = await repository.ReadAsync((r, ct) =>
                    r.Set<Folder>()
                        .FirstOrDefaultAsync(f => f.Id == query.FolderId, ct),
                cancellationToken);
            if (folder == null)
                return new NotFoundResult();
        }

        if (filerByEmailBoxes)
        {
            var emailBoxes = await repository.ReadAsync((r, ct) =>
                    r.Set<EmailBox>()
                        .Where(eb => query.EmailBoxesIds!.Contains(eb.Id))
                        .ToListAsync(ct),
                cancellationToken);
            if (emailBoxes.Count == 0) return new NotFoundResult();
        }

        var threadsPagedResult = await repository.ReadAsync(async (r, ct) =>
        {
            var targetMessagesQuery = r.Set<Message>()
                .AsNoTracking()
                .WhereIf(filerByFolder, msg => msg.Folders.Any(f => f.Id == query.FolderId))
                .WhereIf(filerByEmailBoxes, msg => msg.EmailBoxes.Any(eb => query.EmailBoxesIds!.Contains(eb.Id)))
                .WhereIf(searchTerm != null, msg =>
                        msg.Subject != null && msg.Subject.ToUpper().Contains(searchTerm) ||
                        msg.TextBody != null && msg.TextBody.ToUpper().Contains(searchTerm) ||
                        msg.Attachments.Any(a =>
                            a.FileName.ToUpper().Contains(searchTerm))
                    /*msg.To.Any(a =>
                        a.Name != null && a.Name.ToUpper().Contains(searchTerm) ||
                        a.Address.ToUpper().Contains(searchTerm)) ||
                    msg.From.Any(a =>
                        a.Name != null && a.Name.ToUpper().Contains(searchTerm) ||
                        a.Address.ToUpper().Contains(searchTerm)) ||
                    */)
                .Where(m => m.ThreadId != null);

            var threadIdsQuery = targetMessagesQuery
                .Select(m => m.ThreadId)
                .Distinct();

            var baseQuery = r.Set<Message>()
                .AsNoTracking()
                .Where(m => threadIdsQuery.Contains(m.ThreadId));

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
                        .FirstOrDefault(m => m.ThreadId == threadId && lastMessagesQuery.Contains(m.Id)),
                    FirstMessage = baseQuery
                        .FirstOrDefault(m => m.ThreadId == threadId && firstMessagesQuery.Contains(m.Id)),
                    Length = baseQuery.Count(m => m.ThreadId == threadId)
                });

            var orderedThreadQuery = query.IsByDescending
                ? threadQuery.OrderByDescending(t => t.LastMessage.Date)
                : threadQuery.OrderBy(t => t.LastMessage.Date);

            var pagedThreads = await orderedThreadQuery
                .ToPagedResultAsync(query, ct);

            var result = pagedThreads.Items.Select(t => new MessagesThreadShortDto
            {
                ThreadId = t.ThreadId,
                LastMessage = mapper.Map<MessageDto>(t.LastMessage),
                Subject = t.FirstMessage.Subject,
                StartDate = t.FirstMessage.Date,
                EndDate = t.LastMessage.Date,
                Length = t.Length
            }).ToList();

            return new PagedResult<MessagesThreadShortDto>(result, pagedThreads.Page, pagedThreads.Size, pagedThreads.Total);
        }, cancellationToken);

        return new OkObjectResult(threadsPagedResult);
    }
}