using MediatR;

namespace PhoneBook.Application.Features.Contacts.CreateContact;

public sealed record CreateContactCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Description,
    IReadOnlyCollection<string> Tags
) : IRequest<Guid>;
