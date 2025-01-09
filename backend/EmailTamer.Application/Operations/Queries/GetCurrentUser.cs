using AutoMapper;
using EmailTamer.Auth;
using EmailTamer.Database.Entities;
using EmailTamer.Models.Auth;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmailTamer.Operations.Queries;

public sealed record GetCurrentUser : IRequest<IActionResult>
{
	internal sealed class Validator : AbstractValidator<GetCurrentUser>
    {
        public Validator() { }
    }
}

[UsedImplicitly]
internal sealed class GetCurrentUserQueryHandler(
	IMapper mapper,
	UserManager<EmailTamerUser> userManager,
	IUserContextAccessor userContextAccessor)
	: IRequestHandler<GetCurrentUser, IActionResult>
{
    public async Task<IActionResult> Handle(GetCurrentUser request, CancellationToken cancellationToken)
    {
	    var authUser = await userManager.GetAuthUserAsync(userContextAccessor.Id, mapper);
        var userDto = mapper.Map<UserDto>(authUser);
        return new OkObjectResult(userDto);
    }
}