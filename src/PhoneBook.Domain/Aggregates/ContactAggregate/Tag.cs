namespace PhoneBook.Domain.Aggregates.ContactAggregate;

public sealed class Tag
{
    public string Name { get; }

    public Tag(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name cannot be empty.", nameof(name));
        Name = name;
    }
}