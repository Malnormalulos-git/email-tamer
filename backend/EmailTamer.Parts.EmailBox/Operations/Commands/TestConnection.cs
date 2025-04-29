using AutoMapper;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Parts.EmailBox.Models;
using EmailTamer.Parts.Sync.Exceptions;
using EmailTamer.Parts.Sync.Services;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.EmailBox.Operations.Commands;

public sealed record TestConnection(TestConnectionDto EmailBox) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<TestConnection>
    {
        public Validator(IValidator<TestConnectionDto> validator)
        {
            RuleFor(x => x.EmailBox).SetValidator(validator);
        }
    }
}

[UsedImplicitly]
public class TestConnectionCommandHandler(
    [FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
    IMapper mapper) : IRequestHandler<TestConnection, IActionResult>
{
    public async Task<IActionResult> Handle(TestConnection command, CancellationToken cancellationToken)
    {
        var mappedEmailBox = mapper.Map<Database.Tenant.Entities.EmailBox>(command.EmailBox);

        if (command.EmailBox.Id != null && command.EmailBox.Password == null)
        {
            var existingEmailBox = await repository.ReadAsync((r, ct) =>
                r.Set<Database.Tenant.Entities.EmailBox>()
                    .FirstOrDefaultAsync(mb => mb.Id == command.EmailBox.Id,ct)
                , cancellationToken);
            
            if (existingEmailBox is null)
                return new NotFoundResult();
            
            mappedEmailBox.Password = existingEmailBox.Password;
        }

        try
        {
            var client = await MailKitImapConnector.ConnectToImapClient(mappedEmailBox, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (MailKitImapConnectorException e)
        {
            return new BadRequestObjectResult(e.Fault);
        }

        return new OkResult();
    }
}