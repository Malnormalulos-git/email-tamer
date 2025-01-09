using AutoMapper;
using EmailTamer.Auth.Models;
using EmailTamer.Database.Entities;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmailTamer.Auth.Operations.Commands;

public sealed record CreateUser(CreateUserDto User) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<CreateUser>
    {
        public Validator(IValidator<CreateUserDto> validator)
        {
            RuleFor(x => x.User).SetValidator(validator);
        }
    }
}

[UsedImplicitly]
public class CreateUserCommandHandler(UserManager<EmailTamerUser> userManager, IMapper mapper)
    : IRequestHandler<CreateUser, IActionResult>
{
    public async Task<IActionResult> Handle(CreateUser request, CancellationToken cancellationToken)
    {
        var user = mapper.Map<EmailTamerUser>(request.User);

        var registrationResult = await userManager.CreateAsync(user, request.User.Password);

        if (registrationResult.Errors.Any(x => x.Code == "DuplicateUserName"))
        {
            return new ConflictResult();
        }

        if (!registrationResult.Succeeded)
        {
            return new BadRequestResult();
        }

        var assignRoleResult = await userManager.AddToRoleAsync(user, request.User.Role.ToString("G"));

        if (assignRoleResult.Succeeded)
        {
            return new OkResult();
        }

        await userManager.DeleteAsync(user);

        return new BadRequestResult();
    }
}