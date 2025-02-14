using EmailTamer.Infrastructure.Auth;
using EmailTamer.Parts.Sync.Operations.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailTamer.Parts.Sync.Controllers;

[Route("api/backup")]
public class BackupsController(IMediator mediator) : Controller
{
    [HttpPost("{id:guid}", Name = nameof(BackUpEmailBoxMessages))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> BackUpEmailBoxMessages([FromRoute(Name = "id")] Guid id, CancellationToken ct = default) =>
        mediator.Send(new BackUpEmailBoxMessages(id), ct);
    
    // [HttpPost(Name = nameof(SyncEmailBoxes))]
    // [Authorize(Policy = AuthPolicy.User)]
    // [ProducesResponseType(200)]
    // [ProducesResponseType(404)]
    // public Task<IActionResult> SyncEmailBoxes(CancellationToken ct = default) =>
    //     mediator.Send(new Operations.Commands.SyncEmailBoxes(), ct);
}