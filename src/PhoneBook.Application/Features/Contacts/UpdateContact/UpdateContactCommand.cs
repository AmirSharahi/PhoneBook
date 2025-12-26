using MediatR;
using PhoneBook.Application.DTOs;

namespace PhoneBook.Application.Features.Contacts.UpdateContact;

public sealed record UpdateContactCommand(
    Guid ContactId,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Description,
    IReadOnlyCollection<string> Tags
) : IRequest<ContactDto>;
