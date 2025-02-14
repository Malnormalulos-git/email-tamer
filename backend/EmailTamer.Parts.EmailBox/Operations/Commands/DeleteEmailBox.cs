using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.EmailBox.Operations.Commands;

public sealed record DeleteEmailBox(Guid Id) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<DeleteEmailBox>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotNull();
        }
    }
}

[UsedImplicitly]
public class DeleteEmailBoxCommandHandler([FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository)
    : IRequestHandler<DeleteEmailBox, IActionResult>
{
    public async Task<IActionResult> Handle(DeleteEmailBox command, CancellationToken cancellationToken)
    {
        var emailBox = await repository.ReadAsync((r, ct) =>
                r.Set<Database.Tenant.Entities.EmailBox>()
                    .FirstOrDefaultAsync(x => x.Id == command.Id, ct),
            cancellationToken);

        if (emailBox is null)
        {
            return new NotFoundResult();
        }

        // TODO: delete all backups
        repository.Remove(emailBox);
        await repository.PersistAsync(cancellationToken);

        return new OkResult();
    }
}