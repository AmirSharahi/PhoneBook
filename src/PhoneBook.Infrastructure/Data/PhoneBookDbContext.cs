using PhoneBook.Domain.Aggregates.ContactAggregate;

namespace PhoneBook.Infrastructure.Data;

internal static class PhoneBookDbContext
{
    internal static readonly List<Contact> Contacts = [];
}