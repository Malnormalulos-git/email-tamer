using EmailTamer.Models.Auth;
using EmailTamer.Operations.Commands;
using EmailTamer.Operations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailTamer.Controllers;

[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register", Name = "Register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public Task<IActionResult> СreateUser([FromBody] CreateUserDto dto, CancellationToken ct = default) =>
        _mediator.Send(new CreateUser(dto), ct);
    
    [HttpPost("login", Name = "LogIn")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<IActionResult> Login([FromBody] LogInDto dto, CancellationToken ct = default) =>
        _mediator.Send(new LogInCommand(dto), ct);
    
    
    [Authorize]
    [HttpGet("user", Name = "GetCurrentUser")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public Task<IActionResult> CurrentUser(CancellationToken ct = default) =>
        _mediator.Send(new GetCurrentUser(), ct);

}