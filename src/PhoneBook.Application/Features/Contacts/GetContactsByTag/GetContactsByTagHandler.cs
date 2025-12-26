using Mapster;
using MediatR;
using PhoneBook.Application.DTOs;
using PhoneBook.Domain.Repositories;

namespace PhoneBook.Application.Features.Contacts.GetContactsByTag;

public sealed class GetContactsByTagHandler
    (IContactReadRepository repository)
    : IRequestHandler<GetContactsByTagQuery , IReadOnlyCollection<ContactDto>>
{
    private readonly IContactReadRepository _repository = repository;

    public async Task<IReadOnlyCollection<ContactDto>> Handle(
        GetContactsByTagQuery request,
        CancellationToken cancellationToken)
    {
        var contacts = await _repository.GetByTagAsync(request.Tag, cancellationToken);

        return contacts.Adapt<IReadOnlyCollection<ContactDto>>();
    }
}
