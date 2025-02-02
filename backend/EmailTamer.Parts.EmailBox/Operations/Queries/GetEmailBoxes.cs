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

public sealed record GetEmailBoxes(int Page, int Size)
    : IRequest<IActionResult>, IPagedRequest
{
    public class Validator : AbstractValidator<GetEmailBoxes>
    {
        public Validator(IValidator<IPagedRequest> prValidator)
        {
            Include(prValidator);
        }
    }
}

[UsedImplicitly]
public class GetEmailBoxesQueryHandler(
	[FromKeyedServices(nameof(TenantDbContext))] IEmailTamerRepository repository,
	IMapper mapper)
	: IRequestHandler<GetEmailBoxes, IActionResult>
{
	public async Task<IActionResult> Handle(GetEmailBoxes query, CancellationToken cancellationToken)
    {
	    var emailBoxesPagedResult = await repository.ReadAsync((r, ct) =>
			    r.Set<Database.Tenant.Entities.EmailBox>()
				    .Select(t => mapper.Map<EmailBoxDto>(t))
				    .AsNoTracking()
				    .ToPagedResultAsync(query, ct),
		    cancellationToken);

        return new ObjectResult(emailBoxesPagedResult);
    }
}