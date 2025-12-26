using MediatR;
using PhoneBook.Application.Interfaces.Persistence;
using PhoneBook.Domain.Exceptions;
using PhoneBook.Domain.Repositories;

namespace PhoneBook.Application.Features.Contacts.DeleteContact;

public sealed class DeleteContactHandler
    (IContactWriteRepository repository,
     IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteContactCommand>
{
    private readonly IContactWriteRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(
        DeleteContactCommand request,
        CancellationToken cancellationToken)
    {
        var contact = await _repository.GetByIdAsync(request.ContactId, cancellationToken) 
            ?? throw new ContactNotFoundException(request.ContactId);

        await _repository.DeleteAsync(request.ContactId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
