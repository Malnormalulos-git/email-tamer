using AutoMapper;
using EmailTamer.Database.Extensions;
using EmailTamer.Database.Persistence;
using EmailTamer.Database.Tenant;
using EmailTamer.Database.Utilities.Paging;
using EmailTamer.Parts.EmailBox.Models;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Parts.EmailBox.Operations.Queries;

public sealed record GetEmailBoxes() : IRequest<IActionResult>;

[UsedImplicitly]
public class GetEmailBoxesQueryHandler(
	[FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
	IMapper mapper)
	: IRequestHandler<GetEmailBoxes, IActionResult>
{
	public async Task<IActionResult> Handle(GetEmailBoxes query, CancellationToken cancellationToken)
    {
	    var emailBoxes = await repository.ReadAsync((r, ct) =>
			    r.Set<Database.Tenant.Entities.EmailBox>()
				    .AsNoTracking()
				    .Select(t => mapper.Map<EmailBoxDto>(t))
				    .ToListAsync(ct),
		    cancellationToken);

        return new ObjectResult(emailBoxes);
    }
}