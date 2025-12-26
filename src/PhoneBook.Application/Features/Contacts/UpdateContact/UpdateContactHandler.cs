using Mapster;
using MediatR;
using PhoneBook.Application.DTOs;
using PhoneBook.Application.Interfaces.Persistence;
using PhoneBook.Domain.Aggregates.ContactAggregate;
using PhoneBook.Domain.Exceptions;
using PhoneBook.Domain.Repositories;
using PhoneBook.Domain.ValueObjects;

namespace PhoneBook.Application.Features.Contacts.UpdateContact;

public sealed class UpdateContactHandler
    (IContactWriteRepository repository,
     IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateContactCommand, ContactDto>
{
    private readonly IContactWriteRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;


    public async Task<ContactDto> Handle(
        UpdateContactCommand request,
        CancellationToken cancellationToken)
    {
        var contact = await _repository.GetByIdAsync(request.ContactId, cancellationToken)
                      ?? throw new ContactNotFoundException(request.ContactId);

        contact.UpdateName(new FullName(request.FirstName, request.LastName));
        contact.UpdatePhoneNumber(new PhoneNumber(request.PhoneNumber));
        contact.UpdateDescription(request.Description);
        contact.UpdateTags(request.Tags.Select(x => new Tag(x)).ToList().AsReadOnly());
         

        await _repository.UpdateAsync(contact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return contact.Adapt<ContactDto>();
    }
}
