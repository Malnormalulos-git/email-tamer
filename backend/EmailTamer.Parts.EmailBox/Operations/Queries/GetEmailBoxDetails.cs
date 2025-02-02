using AutoMapper;
using EmailTamer.Database.Persistence;
using EmailTamer.Parts.EmailBox.Models;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmailTamer.Parts.EmailBox.Operations.Queries;

public sealed record GetEmailBoxDetails(Guid Id) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<GetEmailBoxDetails>
    {
	    public Validator()
	    {
		    RuleFor(x => x.Id).NotNull();
	    }
    }
}

[UsedImplicitly]
public class GetEmailBoxDetailsQueryHandler(
	ITenantRepository repository,
	IMapper mapper)
	: IRequestHandler<GetEmailBoxDetails, IActionResult>
{
    public async Task<IActionResult> Handle(GetEmailBoxDetails request, CancellationToken cancellationToken)
    {
	    var emailBox = await repository.ReadAsync((r, ct) =>
			    r.Set<Database.Tenant.Entities.EmailBox>()
				    .AsNoTracking()
				    .FirstOrDefaultAsync(x => x.Id == request.Id, ct),
		    cancellationToken);

	    if (emailBox is null)
	    {
		    return new NotFoundResult();
	    }

	    var result = mapper.Map<EmailBoxDetailsDto>(emailBox);

	    return new OkObjectResult(result);
    }
}