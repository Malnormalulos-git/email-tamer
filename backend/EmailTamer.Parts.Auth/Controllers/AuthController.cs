using EmailTamer.Auth.Models;
using EmailTamer.Auth.Operations.Commands;
using EmailTamer.Auth.Operations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailTamer.Auth.Controllers;

[Route("api/auth")]
public class AuthController(IMediator mediator) : Controller
{
    [HttpPost("register", Name = nameof(Register))]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public Task<IActionResult> Register([FromBody] CreateUserDto dto, CancellationToken ct = default) =>
        mediator.Send(new CreateUser(dto), ct);
    
    [HttpPost("login", Name = nameof(Login))]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(401)]
    public Task<IActionResult> Login([FromBody] LogInDto dto, CancellationToken ct = default) =>
        mediator.Send(new LogInCommand(dto), ct);
    
    
    [Authorize]
    [HttpGet("user", Name = nameof(GetCurrentUser))]
    [ProducesResponseType(typeof(UserDto), 200)]
    public Task<IActionResult> GetCurrentUser(CancellationToken ct = default) =>
        mediator.Send(new GetCurrentUser(), ct);

}