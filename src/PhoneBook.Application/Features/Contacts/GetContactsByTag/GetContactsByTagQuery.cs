using MediatR;
using PhoneBook.Application.DTOs;

namespace PhoneBook.Application.Features.Contacts.GetContactsByTag;

public sealed record GetContactsByTagQuery(string Tag) : IRequest<IReadOnlyCollection<ContactDto>>;
