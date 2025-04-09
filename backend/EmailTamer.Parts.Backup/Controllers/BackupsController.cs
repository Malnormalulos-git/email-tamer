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
    public Task<IActionResult>
        BackUpEmailBoxMessages([FromRoute(Name = "id")] Guid id, CancellationToken ct = default) =>
        mediator.Send(new BackUpEmailBoxMessages(id), ct);

    [HttpPost(Name = nameof(BackUpEmailBoxesMessages))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(200)]
    public Task<IActionResult> BackUpEmailBoxesMessages(CancellationToken ct = default) =>
        mediator.Send(new BackUpEmailBoxesMessages(), ct);

    [HttpGet("message", Name = nameof(GetMessageDetails))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(MessageDetailsDto), 200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> GetMessageDetails([FromBody] GetMessageDetailsDto messageDetailsDto,
        CancellationToken ct = default) =>
        mediator.Send(new GetMessageDetails(messageDetailsDto), ct);

    [HttpGet("attachment", Name = nameof(GetMessageAttachment))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> GetMessageAttachment([FromBody] GetMessageAttachmentDto messageAttachmentDto,
        CancellationToken ct = default) =>
        mediator.Send(new GetMessageAttachment(messageAttachmentDto), ct);

    [HttpGet("thread", Name = nameof(GetMessagesThread))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(MessagesThreadDto), 200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> GetMessagesThread(
        [FromBody] GetMessageDetailsDto messageDetailsDto, 
        CancellationToken ct = default) =>
        mediator.Send(new GetMessagesThread(messageDetailsDto), ct);

    [HttpGet(Name = nameof(GetMessagesThreads))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(PagedResult<MessagesThreadShortDto>), 200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> GetMessagesThreads(
        [FromQuery(Name = "foldersIds")] Guid[]? foldersIds,
        [FromQuery(Name = "emailBoxesIds")] Guid[]? emailBoxesIds,
        // TODO: [FromQuery(Name = "searchTerm")] string? searchTerm,
        // TODO: [FromQuery(Name = "sortBy")] string sortBy = "byDate",
        // TODO: [FromQuery(Name = "isByDescending")] bool isByDescending = true,
        [FromQuery(Name = "page")] int page,
        [FromQuery(Name = "size")] int size,
        CancellationToken ct = default) =>
        mediator.Send(new GetMessagesThreads(foldersIds, emailBoxesIds, page, size), ct);

    [HttpGet("folders", Name = nameof(GetFolders))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(List<FolderDto>), 200)]
    public Task<IActionResult> GetFolders(CancellationToken ct = default) => 
        mediator.Send(new GetFolders(), ct);
}