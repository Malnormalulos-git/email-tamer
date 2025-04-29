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

public sealed record GetFolders : IRequest<IActionResult>;

[UsedImplicitly]
public class GetFoldersQueryHandler(
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    IMapper mapper)
    : IRequestHandler<GetFolders, IActionResult>
{
    public async Task<IActionResult> Handle(GetFolders query, CancellationToken cancellationToken)
    {
        var folders = await repository.ReadAsync((r, ct) =>
                r.Set<Folder>()
                    .AsNoTracking()
                    .Select(f => mapper.Map<FolderDto>(f))
                    .ToListAsync(ct)
            , cancellationToken);
            
        return new OkObjectResult(folders.OrderBy(f => f.Name));
    }
}