using EmailTamer.Database;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Database.Tenant.Entities;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.Sync.Operations.Commands;

public sealed record SyncEmailBoxes: IRequest<IActionResult>
{
    public class Validator : AbstractValidator<SyncEmailBoxes>;
}

[UsedImplicitly]
public class SyncEmailBoxesCommandHandler(
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    IMediator mediator)
    : IRequestHandler<SyncEmailBoxes, IActionResult>
{
    public async Task<IActionResult> Handle(SyncEmailBoxes request, CancellationToken cancellationToken)
    {
        var emailBoxes = await repository.ReadAsync((r, ct) =>
                r.Set<EmailBox>()
                    .AsNoTracking()
                    .ToListAsync(ct),
            cancellationToken);

        var syncTasks = emailBoxes.Select(emailBox =>
            mediator.Send(new SyncEmailBox(emailBox.Id), cancellationToken)
        );

        await Task.WhenAll(syncTasks);

        
        return new OkResult();
    }
}