using EmailTamer.Database.Tenant.Entities;
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
    [ProducesResponseType(400)]
    [ProducesResponseType(typeof(ConnectionFault), 400)]
    public Task<IActionResult> BackUpEmailBoxMessages([FromRoute(Name = "id")] Guid id/*, CancellationToken ct = default*/) =>
        mediator.Send(new BackUpEmailBoxMessages(id), CancellationToken.None);

    [HttpPost(Name = nameof(BackUpEmailBoxesMessages))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(200)]
    public Task<IActionResult> BackUpEmailBoxesMessages([FromQuery(Name = "emailBoxesIds")] string? emailBoxesIds/*, CancellationToken ct = default*/)
    {
        Guid[]? parsedEmailBoxesIds = null;
        if (!string.IsNullOrEmpty(emailBoxesIds))
        {
            parsedEmailBoxesIds = emailBoxesIds.Split(',')
                .Select(id => Guid.Parse(id.Trim()))
                .ToArray();
        }

        return mediator.Send(new BackUpEmailBoxesMessages(parsedEmailBoxesIds));
    }

    [HttpGet("emailBoxesStatuses", Name = nameof(GetEmailBoxesStatuses))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(List<EmailBoxStatusDto>), 200)]
    public Task<IActionResult> GetEmailBoxesStatuses(CancellationToken ct = default) =>
        mediator.Send(new GetEmailBoxesStatuses(), ct);

    [HttpGet("message", Name = nameof(GetMessageDetails))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(MessageDetailsDto), 200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> GetMessageDetails([FromQuery] string messageId, CancellationToken ct = default) =>
        mediator.Send(new GetMessageDetails(messageId), ct);

    [HttpGet("attachment", Name = nameof(GetMessageAttachment))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> GetMessageAttachment(
        [FromQuery] string messageId,
        [FromQuery] string attachmentId,
        CancellationToken ct = default) =>
        mediator.Send(new GetMessageAttachment(messageId, attachmentId), ct);

    [HttpGet("thread", Name = nameof(GetMessagesThread))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(MessagesThreadDto), 200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> GetMessagesThread(
        [FromQuery] string messageId,
        CancellationToken ct = default) =>
        mediator.Send(new GetMessagesThread(messageId), ct);

    [HttpGet(Name = nameof(GetMessagesThreads))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(PagedResult<MessagesThreadShortDto>), 200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> GetMessagesThreads(
        [FromQuery(Name = "folderId")] Guid? folderId,
        [FromQuery(Name = "emailBoxesIds")] string? emailBoxesIds,
        [FromQuery(Name = "searchTerm")] string? searchTerm,
        [FromQuery(Name = "page")] int page,
        [FromQuery(Name = "size")] int size,
        [FromQuery(Name = "isByDescending")] bool isByDescending = true,
        CancellationToken ct = default)
    {
        Guid[]? parsedEmailBoxesIds = null;
        if (!string.IsNullOrEmpty(emailBoxesIds))
        {
            parsedEmailBoxesIds = emailBoxesIds.Split(',')
                .Select(id => Guid.Parse(id.Trim()))
                .ToArray();
        }

        return mediator.Send(new GetMessagesThreads(folderId, parsedEmailBoxesIds, searchTerm, page, size, isByDescending), ct);
    }

    [HttpGet("folders", Name = nameof(GetFolders))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(List<FolderDto>), 200)]
    public Task<IActionResult> GetFolders(CancellationToken ct = default) =>
        mediator.Send(new GetFolders(), ct);
}