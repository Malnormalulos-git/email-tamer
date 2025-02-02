using AutoMapper;
using EmailTamer.Database.Persistence;
using EmailTamer.Parts.EmailBox.Models;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
public class DeleteEmailBoxCommandHandler(
    ITenantRepository repository)
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

        // TODO: remove all backups
        repository.Remove(emailBox);
        await repository.PersistAsync(cancellationToken);

        return new OkResult();
    }
}