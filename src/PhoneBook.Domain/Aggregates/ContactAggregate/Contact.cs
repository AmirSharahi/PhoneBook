using PhoneBook.Domain.Common;
using PhoneBook.Domain.ValueObjects;
using System.Collections.ObjectModel;

namespace PhoneBook.Domain.Aggregates.ContactAggregate;

public sealed class Contact : RemovableEntity
{
    #region Props
    public FullName Name { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    private readonly List<Tag> _tags = new();
    #endregion
    #region CTors
    private Contact()
    {
    }

    public Contact(FullName name, PhoneNumber phoneNumber, string? description = null, IEnumerable<Tag>? tags = null)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        Description = description;
        if (tags != null)
            _tags.AddRange(tags);
    }
    #endregion
    #region Setters
    public void UpdateName(FullName name) => Name = name;
    public void UpdatePhoneNumber(PhoneNumber phoneNumber) => PhoneNumber = phoneNumber;
    public void UpdateDescription(string? description) => Description = description;

    public void UpdateTags(IReadOnlyCollection<Tag> updatedTags)
    {
        var toAdd = updatedTags
            .Where(t => !_tags.Any(existing => existing.Name == t.Name))
            .ToList();

        var toRemove = _tags
            .Where(existing => !updatedTags.Any(t => t.Name == existing.Name))
            .ToList();

        foreach (var t in toAdd)
            _tags.Add(t);

        foreach (var t in toRemove)
            _tags.Remove(t);
    }
    #endregion
}
