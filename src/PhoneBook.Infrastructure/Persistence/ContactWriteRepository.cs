using PhoneBook.Domain.Aggregates.ContactAggregate;
using PhoneBook.Domain.Repositories;
using PhoneBook.Domain.ValueObjects;
using PhoneBook.Infrastructure.Data;

namespace PhoneBook.Infrastructure.Persistence;

public sealed class ContactWriteRepository : IContactWriteRepository
{
    public Task<Guid> AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        PhoneBookDbContext.Contacts.Add(contact);
        return Task.FromResult(contact.Id);
    }

    public Task UpdateAsync(Contact contact, CancellationToken cancellationToken = default) 
        => Task.CompletedTask;

    public Task DeleteAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        var contact = PhoneBookDbContext.Contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact != null)
            PhoneBookDbContext.Contacts.Remove(contact);

        return Task.CompletedTask;
    }

    public Task<Contact?> GetByIdAsync(Guid contactId, CancellationToken cancellationToken = default)
        => Task.FromResult(PhoneBookDbContext.Contacts.FirstOrDefault(c => c.Id == contactId));

    public Task<bool> PhoneNumberExistsAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
        => Task.FromResult(PhoneBookDbContext.Contacts.Any(c => c.PhoneNumber.Equals(phoneNumber)));
}
