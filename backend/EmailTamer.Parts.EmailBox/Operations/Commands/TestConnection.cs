using AutoMapper;
using EmailTamer.Parts.EmailBox.Models;
using EmailTamer.Parts.Sync.Exceptions;
using EmailTamer.Parts.Sync.Services;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
public class TestConnectionCommandHandler(IMapper mapper) : IRequestHandler<TestConnection, IActionResult>
{
    public async Task<IActionResult> Handle(TestConnection command, CancellationToken cancellationToken)
    {
        var mappedEmailBox = mapper.Map<Database.Tenant.Entities.EmailBox>(command.EmailBox);

        try
        {
            await MailKitImapConnector.ConnectToImapClient(mappedEmailBox, cancellationToken);
        }
        catch (MailKitImapConnectorException e)
        {
            return new BadRequestObjectResult(e.Fault);
        }

        return new OkResult();
    }
}