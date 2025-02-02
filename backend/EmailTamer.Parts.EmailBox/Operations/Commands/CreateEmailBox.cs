using AutoMapper;
using EmailTamer.Database.Persistence;
using EmailTamer.Parts.EmailBox.Models;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailTamer.Parts.EmailBox.Operations.Commands;

public sealed record CreateEmailBox(CreateEmailBoxDto EmailBox) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<CreateEmailBox>
    {
        public Validator(IValidator<CreateEmailBoxDto> validator)
        {
            RuleFor(x => x.EmailBox).SetValidator(validator);
        }
    }
}

[UsedImplicitly]
public class CreateEmailBoxCommandHandler(
    ITenantRepository repository,
	IMapper mapper)
	: IRequestHandler<CreateEmailBox, IActionResult>
{
	public async Task<IActionResult> Handle(CreateEmailBox command, CancellationToken cancellationToken)
    {
        var emailBox = await repository.ReadAsync((r, ct) =>
                r.Set<Database.Tenant.Entities.EmailBox>()
                    .FirstOrDefaultAsync(x => x.Email == command.EmailBox.Email, ct),
            cancellationToken);

        if (emailBox is not null)
        {
            return new ConflictResult();
        }
        
        emailBox = mapper.Map<Database.Tenant.Entities.EmailBox>(command.EmailBox); 
        
        emailBox.Id = Guid.NewGuid();

        
        repository.Add(emailBox);
        await repository.PersistAsync(cancellationToken);

        return new OkResult();
    }
}