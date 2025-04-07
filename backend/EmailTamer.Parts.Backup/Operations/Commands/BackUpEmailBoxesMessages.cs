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
        var emailBoxesIds = await repository.ReadAsync((r, ct) =>
                r.Set<EmailBox>()
                    .AsNoTracking()
                    .Select(x => x.Id)
                    .ToListAsync(ct),
            cancellationToken);
        
        foreach (var emailBoxId in emailBoxesIds)
        {
            await mediator.Send(new BackUpEmailBoxMessages(emailBoxId), cancellationToken);
        }
        
        return new OkResult();
    }
}