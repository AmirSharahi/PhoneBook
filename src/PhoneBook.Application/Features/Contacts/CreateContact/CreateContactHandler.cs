using MediatR;
using PhoneBook.Domain.Aggregates.ContactAggregate;
using PhoneBook.Domain.Exceptions;
using PhoneBook.Domain.Repositories;
using PhoneBook.Domain.ValueObjects;

namespace PhoneBook.Application.Features.Contacts.CreateContact;

public sealed class CreateContactHandler
    (IContactWriteRepository repository)
    : IRequestHandler<CreateContactCommand, Guid>
{
    private readonly IContactWriteRepository _repository = repository;

    public async Task<Guid> Handle(
        CreateContactCommand request,
        CancellationToken cancellationToken)
    {
        if (await _repository.PhoneNumberExistsAsync(new PhoneNumber(request.PhoneNumber)))
            throw new DuplicatePhoneNumberException(request.PhoneNumber);

        var contact = new Contact(
            new FullName(request.FirstName, request.LastName),
            new PhoneNumber(request.PhoneNumber),
            request.Description,
            request.Tags.Select(x => new Tag(x)));

        await _repository.AddAsync(contact, cancellationToken);
        return contact.Id;
    }
}
