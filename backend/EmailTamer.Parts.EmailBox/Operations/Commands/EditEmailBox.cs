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
    private record struct PropertyUpdate<T>(T CurrentValue, T? NewValue)
    {
        public bool HasChanged(string propertyName) =>
            propertyName is nameof(Database.Tenant.Entities.EmailBox.BoxName) or nameof(Database.Tenant.Entities.EmailBox.UserName)
                ? !Equals(CurrentValue, NewValue) 
                : NewValue != null && !Equals(CurrentValue, NewValue); 
    }
    
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

        var updates = new Dictionary<string, PropertyUpdate<object>>
        {
            { nameof(emailBox.BoxName), new(emailBox.BoxName, command.EditEmailBoxDto.BoxName) },
            { nameof(emailBox.UserName), new(emailBox.UserName, command.EditEmailBoxDto.UserName) },
            { nameof(emailBox.Email), new(emailBox.Email, command.EditEmailBoxDto.Email) },
            { nameof(emailBox.AuthenticateByEmail), new(emailBox.AuthenticateByEmail, command.EditEmailBoxDto.AuthenticateByEmail) },
            { nameof(emailBox.Password), new(emailBox.Password, command.EditEmailBoxDto.Password) },
            { nameof(emailBox.EmailDomainConnectionHost), new(emailBox.EmailDomainConnectionHost, command.EditEmailBoxDto.EmailDomainConnectionHost) },
            { nameof(emailBox.EmailDomainConnectionPort), new(emailBox.EmailDomainConnectionPort, command.EditEmailBoxDto.EmailDomainConnectionPort) },
            { nameof(emailBox.UseSSl), new(emailBox.UseSSl, command.EditEmailBoxDto.UseSSl) }
        };

        var changedProperties = updates
            .Where(u => u.Value.HasChanged(u.Key))
            .ToList();
        
        if (changedProperties.Count == 0)
        {
            return new StatusCodeResult(304);
        }

        if (changedProperties.Any(p => p.Key == nameof(emailBox.Email)))
        {
            var duplicateExists = await repository.ReadAsync((r, ct) =>
                r.Set<Database.Tenant.Entities.EmailBox>()
                    .AnyAsync(x => x.Email == command.EditEmailBoxDto.Email, ct),
                cancellationToken);

            if (duplicateExists)
            {
                return new ConflictResult();
            }
        }

        foreach (var change in changedProperties)
        {
            var property = typeof(Database.Tenant.Entities.EmailBox).GetProperty(change.Key);
            property?.SetValue(emailBox, change.Value.NewValue);
        }

        repository.Update(emailBox);
        await repository.PersistAsync(cancellationToken);
        
        return new OkResult();
    }
}