using EmailTamer.Database.Tenant.Entities;
using EmailTamer.Infrastructure.Auth;
using EmailTamer.Parts.EmailBox.Models;
using EmailTamer.Parts.EmailBox.Operations.Commands;
using EmailTamer.Parts.EmailBox.Operations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailTamer.Parts.EmailBox.Controllers;

[Route("api/emailBoxes")]
public class EmailBoxesController(IMediator mediator) : Controller
{
    [HttpPost(Name = nameof(CreateEmailBox))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(200)]
    [ProducesResponseType(409)]
    public Task<IActionResult> CreateEmailBox([FromBody] CreateEmailBoxDto createEmailBoxDto, CancellationToken ct = default) =>
        mediator.Send(new CreateEmailBox(createEmailBoxDto), ct);
    
    [HttpGet(Name = nameof(GetEmailBoxes))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(List<EmailBoxDto>), 200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> GetEmailBoxes(CancellationToken ct = default)
        => mediator.Send(new GetEmailBoxes(), ct);
    
    [HttpGet("{id:guid}", Name = nameof(GetEmailBoxDetails))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(typeof(EmailBoxDetailsDto), 200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> GetEmailBoxDetails(Guid id, CancellationToken ct = default)
        => mediator.Send(new GetEmailBoxDetails(id), ct);
    
    [HttpPatch(Name = nameof(EditEmailBox))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(200)]
    [ProducesResponseType(304)]
    [ProducesResponseType(409)]
    public Task<IActionResult> EditEmailBox([FromBody] EditEmailBoxDto editEmailBoxDto, CancellationToken ct = default) =>
        mediator.Send(new EditEmailBox(editEmailBoxDto), ct);
    
    [HttpDelete("{id:guid}", Name = nameof(DeleteEmailBox))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public Task<IActionResult> DeleteEmailBox([FromRoute(Name = "id")] Guid id, CancellationToken ct = default) =>
        mediator.Send(new DeleteEmailBox(id), ct);
    
    [HttpPost("testConnection", Name = nameof(TestConnection))]
    [Authorize(Policy = AuthPolicy.User)]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ConnectionFault), 400)]
    [ProducesResponseType(404)]
    public Task<IActionResult> TestConnection([FromBody] TestConnectionDto testConnectionDto, CancellationToken ct = default) =>
        mediator.Send(new TestConnection(testConnectionDto), ct);
}