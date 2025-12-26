using PhoneBook.Domain.Aggregates.ContactAggregate;

namespace PhoneBook.Domain.Repositories;

public interface IContactReadRepository
{
    Task<IReadOnlyCollection<Contact>> GetByTagAsync(string tagName, CancellationToken cancellationToken = default);
}
