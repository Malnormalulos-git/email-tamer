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

public sealed record BackUpEmailBoxesMessages : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<BackUpEmailBoxesMessages>;
}

[UsedImplicitly]
public class BackUpEmailBoxesMessagesCommandHandler(
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    IMediator mediator)
    : IRequestHandler<BackUpEmailBoxesMessages, IActionResult>
{
    public async Task<IActionResult> Handle(BackUpEmailBoxesMessages request, CancellationToken cancellationToken)
    {
        var emailBoxes = await repository.ReadAsync((r, ct) =>
                r.Set<EmailBox>()
                    .AsNoTracking()
                    .Where(x => x.BackupStatus != BackupStatus.Queued || x.BackupStatus != BackupStatus.InProgress)
                    .ToListAsync(ct),
            cancellationToken);

        foreach (var emailBox in emailBoxes)
        {
            emailBox.BackupStatus = BackupStatus.Queued;
        }
        repository.UpdateRange(emailBoxes);
        await repository.PersistAsync(cancellationToken);
        
        var emailBoxesIds = emailBoxes.Select(x => x.Id).ToList();
        
        foreach (var emailBoxId in emailBoxesIds)
        {
            await mediator.Send(new BackUpEmailBoxMessages(emailBoxId), cancellationToken);
        }
        
        return new OkResult();
    }
}