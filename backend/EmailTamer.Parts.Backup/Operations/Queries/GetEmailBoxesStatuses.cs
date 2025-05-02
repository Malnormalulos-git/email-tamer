using AutoMapper;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Database.Tenant.Entities;
using EmailTamer.Parts.Sync.Models;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.Sync.Operations.Queries;

public sealed record GetEmailBoxesStatuses : IRequest<IActionResult>;

[UsedImplicitly]
internal class GetEmailBoxesStatusesQueryHandler(
    [FromKeyedServices(nameof(TenantDbContext))]
    IEmailTamerRepository repository,
    IMapper mapper)
    : IRequestHandler<GetEmailBoxesStatuses, IActionResult>
{
    public async Task<IActionResult> Handle(GetEmailBoxesStatuses request, CancellationToken cancellationToken)
    {
        var emailBoxesStatuses = await repository.ReadAsync((r, ct) =>
                r.Set<EmailBox>()
                    .AsNoTracking()
                    .Select(x => mapper.Map<EmailBoxStatusDto>(x))
                    .ToListAsync(ct)
            , cancellationToken);

        return new OkObjectResult(emailBoxesStatuses);
    }
}