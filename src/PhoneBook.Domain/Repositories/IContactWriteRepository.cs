using PhoneBook.Domain.Aggregates.ContactAggregate;
using PhoneBook.Domain.ValueObjects;

namespace PhoneBook.Domain.Repositories;

public interface IContactWriteRepository
{
    Task<Guid> AddAsync(Contact contact, CancellationToken cancellationToken = default);
    Task UpdateAsync(Contact contact, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid contactId, CancellationToken cancellationToken = default);
    Task<Contact?> GetByIdAsync(Guid contactId, CancellationToken cancellationToken = default);
    Task<bool> PhoneNumberExistsAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);
}
