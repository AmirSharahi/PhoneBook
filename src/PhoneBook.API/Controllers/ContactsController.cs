using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PhoneBook.API.Models.Requests;
using PhoneBook.Application.DTOs;
using PhoneBook.Application.Features.Contacts.CreateContact;
using PhoneBook.Application.Features.Contacts.DeleteContact;
using PhoneBook.Application.Features.Contacts.GetContactsByTag;
using PhoneBook.Application.Features.Contacts.UpdateContact;

namespace PhoneBook.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public sealed class ContactsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Get contacts filtered by tag
    /// </summary>
    /// <param name="tag">Tag name to filter contacts</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of contacts that contain the given tag</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ContactDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTag(
        [FromQuery] string tag,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetContactsByTagQuery(tag), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Create a new contact
    /// </summary>
    /// <param name="model">Contact creation data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Created contact identifier</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [FromBody] CreateContactRequest model,
        CancellationToken cancellationToken)
    {
        var command = model.Adapt<CreateContactCommand>();
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Update an existing contact
    /// </summary>
    /// <param name="id">Contact Id</param>
    /// <param name="model">Updated contact data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Updated contact</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateContactRequest model,
        CancellationToken cancellationToken)
    {
        var command = model.Adapt<UpdateContactCommand>() with { ContactId = id };
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Delete a contact by id
    /// </summary>
    /// <param name="id">Contact Id</param>
    /// <param name="cancellationToken"></param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteContactCommand(id), cancellationToken);
        return Ok();
    }
}
