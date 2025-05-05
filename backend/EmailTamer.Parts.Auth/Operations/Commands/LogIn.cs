using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmailTamer.Auth.Config;
using EmailTamer.Auth.Models;
using EmailTamer.Core.Config;
using EmailTamer.Database.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EmailTamer.Auth.Operations.Commands;

public sealed record LogInCommand(LogInDto Credentials) : IRequest<IActionResult>
{
    public class Validator : AbstractValidator<LogInCommand>
    {
        public Validator(IValidator<LogInDto> validator)
        {
            RuleFor(x => x.Credentials).SetValidator(validator);
        }
    }
}

public class LogInCommandHandler(UserManager<EmailTamerUser> userManager,
                                 IOptionsMonitor<JwtConfig> jwtConfig,
                                 ISystemClock systemClock)
    : IRequestHandler<LogInCommand, IActionResult>
{
    public async Task<IActionResult> Handle(LogInCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(request.Credentials.Email);

        if (user is null) return new UnauthorizedResult();

        var passwordIsConfirmed = await userManager.CheckPasswordAsync(user, request.Credentials.Password);

        return passwordIsConfirmed
            ? new OkObjectResult(new AuthResponseDto { Token = await IssueToken(user) })
            : new UnauthorizedResult();
    }

    private async Task<string> IssueToken(EmailTamerUser user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var config = jwtConfig.CurrentValue;

        var key = Encoding.UTF8.GetBytes(config.Key);
        var secretKey = new SymmetricSecurityKey(key);

        var claims = new List<Claim>
        {
            new ("id", user.Id ),
            new (ClaimTypes.Email, user.Email!)
        };
        var roles = await userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new(claims),
            Issuer = config.Issuer,
            Audience = config.Audience,
            Expires = systemClock.UtcNow.AddHours(24).DateTime,
            SigningCredentials = new(secretKey, SecurityAlgorithms.HmacSha256)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);
        return jwtToken;
    }
}

