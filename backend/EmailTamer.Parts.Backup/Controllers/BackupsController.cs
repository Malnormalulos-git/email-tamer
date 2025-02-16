using EmailTamer.Database.Utilities.Paging;
using EmailTamer.Infrastructure.Auth;
using EmailTamer.Parts.Sync.Models;
using EmailTamer.Parts.Sync.Operations.Commands;
using EmailTamer.Parts.Sync.Operations.Queries;
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
    
    // [HttpGet("{id:guid}", Name = nameof(GetMailBoxMessages))]
    // [Authorize(Policy = AuthPolicy.User)]
    // [ProducesResponseType(typeof(PagedResult<MessageDto>), 200)]
    // [ProducesResponseType(404)]
    // public Task<IActionResult> GetMailBoxMessages([FromRoute(Name = "id")] Guid id, CancellationToken ct = default) =>
    //     mediator.Send(new GetMailBoxMessages(id), ct);
    
    // TODO: pass here the name of the folder?
    [HttpGet(Name = nameof(GetEmailBoxesMessages))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(PagedResult<MessageDto>), 200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> GetEmailBoxesMessages(
        [FromQuery(Name = "folder")] string? folder,
        [FromQuery(Name = "page")] int page,
        [FromQuery(Name = "size")] int size,
        CancellationToken ct = default) =>
        mediator.Send(new GetEmailBoxesMessages(folder, page, size), ct);
    
}