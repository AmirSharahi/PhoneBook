using MediatR;

namespace PhoneBook.Application.Features.Contacts.DeleteContact;

public sealed record DeleteContactCommand(Guid ContactId) : IRequest;
