using EmailTamer.Infrastructure.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailTamer.Parts.Sync.Controllers;

[Route("api/syncEmailBoxes")]
public class SyncEmailBoxesController(IMediator mediator) : Controller
{
    [HttpPost("{id:guid}", Name = nameof(SyncEmailBox))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> SyncEmailBox([FromRoute(Name = "id")] Guid id, CancellationToken ct = default) =>
        mediator.Send(new Operations.Commands.SyncEmailBox(id), ct);
    
    // [HttpPost(Name = nameof(SyncEmailBoxes))]
    // [Authorize(Policy = AuthPolicy.User)]
    // [ProducesResponseType(200)]
    // [ProducesResponseType(404)]
    // public Task<IActionResult> SyncEmailBoxes(CancellationToken ct = default) =>
    //     mediator.Send(new Operations.Commands.SyncEmailBoxes(), ct);
}