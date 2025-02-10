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
        throw new NotImplementedException();
        
        /*
         * Problem "System.InvalidOperationException: The instance of entity type 'Message' cannot be tracked
         * because another instance with the key value '{Id: messageId}' is already being tracked.
         * When attaching existing entities, ensure that only one entity instance with a given key value is attached."
         *
         * where emailBoxes with shared messages are backed up 
        */
        
        var emailBoxesIds = await repository.ReadAsync((r, ct) =>
                r.Set<EmailBox>()
                    .AsNoTracking()
                    .Select(x => x.Id)
                    .ToListAsync(ct),
            cancellationToken);

        var syncTasks = emailBoxesIds.Select(emailBoxId =>
            mediator.Send(new SyncEmailBox(emailBoxId), cancellationToken)
        );
        
        await Task.WhenAll(syncTasks);
        
        // foreach (var emailBoxId in emailBoxesIds)
        // {
        //     await mediator.Send(new SyncEmailBox(emailBoxId), cancellationToken);
        // }

        
        return new OkResult();
    }
}