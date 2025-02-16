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
    
    // [HttpPost(Name = nameof(BackUpEmailBoxesMessages))]
    // [Authorize(Policy = AuthPolicy.User)]
    // [ProducesResponseType(200)]
    // [ProducesResponseType(404)]
    // public Task<IActionResult> BackUpEmailBoxesMessages(CancellationToken ct = default) =>
    //     mediator.Send(new Operations.Commands.BackUpEmailBoxesMessages(), ct);
    
    [HttpGet(Name = nameof(GetMessages))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(PagedResult<MessageDto>), 200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> GetMessages(
        [FromQuery(Name = "folderName")] string? folderName,
        // TODO: [FromQuery(Name = "searchTerm")] string? searchTerm,
        // TODO: [FromQuery(Name = "sortBy")] string sortBy = "byDate",
        // TODO: [FromQuery(Name = "isByDescending")] bool isByDescending = true,
        [FromQuery(Name = "page")] int page,
        [FromQuery(Name = "size")] int size,
        CancellationToken ct = default) =>
        mediator.Send(new GetMessages(folderName, page, size), ct);
}