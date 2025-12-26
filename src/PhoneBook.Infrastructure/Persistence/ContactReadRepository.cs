using PhoneBook.Domain.Aggregates.ContactAggregate;
using PhoneBook.Domain.Exceptions;
using PhoneBook.Domain.Repositories;
using PhoneBook.Domain.ValueObjects;
using PhoneBook.Infrastructure.Data;

namespace PhoneBook.Infrastructure.Persistence;

public sealed class ContactReadRepository : IContactReadRepository
{
    public Task<IReadOnlyCollection<Contact>> GetByTagAsync(
        string tagName,
        CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyCollection<Contact>>(
            PhoneBookDbContext.Contacts
                .Where(c => c.Tags.Any(t =>
                    t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase)))
                .ToList());
}
