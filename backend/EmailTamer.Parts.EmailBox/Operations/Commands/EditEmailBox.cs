using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Parts.EmailBox.Models;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.EmailBox.Operations.Commands;

public sealed record EditEmailBox(EditEmailBoxDto EditEmailBoxDto) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<EditEmailBox>
    {
        public Validator(IValidator<EditEmailBoxDto> validator)
        {
            RuleFor(x => x.EditEmailBoxDto).SetValidator(validator);
        }
    }
}

[UsedImplicitly]
public class EditEmailBoxCommandHandler([FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository)
    : IRequestHandler<EditEmailBox, IActionResult>
{
    public async Task<IActionResult> Handle(EditEmailBox command, CancellationToken cancellationToken)
    {
        var emailBox = await repository.ReadAsync((r, ct) =>
                r.Set<Database.Tenant.Entities.EmailBox>()
                    .FirstOrDefaultAsync(x => x.Id == command.EditEmailBoxDto.Id, ct),
            cancellationToken);

        if (emailBox is null)
        {
            return new NotFoundResult();
        }

        bool areChangesMade = false;

        if (!string.Equals(emailBox.BoxName, command.EditEmailBoxDto.BoxName))
        {
            emailBox.BoxName = command.EditEmailBoxDto.BoxName;
            areChangesMade = true;
        }

        if (!string.Equals(emailBox.UserName, command.EditEmailBoxDto.UserName))
        {
            emailBox.UserName = command.EditEmailBoxDto.UserName;
            areChangesMade = true;
        }
        
        if (!string.Equals(emailBox.Email, command.EditEmailBoxDto.Email))
        {
            var duplicatedEmailBox = await repository.ReadAsync((r, ct) =>
                    r.Set<Database.Tenant.Entities.EmailBox>()
                        .FirstOrDefaultAsync(x => x.Email == command.EditEmailBoxDto.Email, ct),
                cancellationToken);

            if (duplicatedEmailBox is not null)
            {
                return new ConflictResult();
            }
            
            emailBox.Email = command.EditEmailBoxDto.Email;
            areChangesMade = true;
        }

        if (emailBox.AuthenticateByEmail != command.EditEmailBoxDto.AuthenticateByEmail)
        {
            emailBox.AuthenticateByEmail = command.EditEmailBoxDto.AuthenticateByEmail;
            areChangesMade = true;
        }
        
        if (!string.Equals(emailBox.Password, command.EditEmailBoxDto.Password))
        {
            emailBox.Password = command.EditEmailBoxDto.Password;
            areChangesMade = true;
        }

        if (!string.Equals(emailBox.EmailDomainConnectionHost, command.EditEmailBoxDto.EmailDomainConnectionHost))
        {
            emailBox.EmailDomainConnectionHost = command.EditEmailBoxDto.EmailDomainConnectionHost;
            areChangesMade = true;
        }
        
        if (emailBox.EmailDomainConnectionPort != command.EditEmailBoxDto.EmailDomainConnectionPort)
        {
            emailBox.EmailDomainConnectionPort = command.EditEmailBoxDto.EmailDomainConnectionPort;
            areChangesMade = true;
        }
        
        if (emailBox.UseSSl != command.EditEmailBoxDto.UseSSl)
        {
            emailBox.UseSSl = command.EditEmailBoxDto.UseSSl;
            areChangesMade = true;
        }
        
        if (areChangesMade)
        {
            repository.Update(emailBox);
            await repository.PersistAsync(cancellationToken);
            
            return new OkResult();
        }

        return new StatusCodeResult(304);
    }
}