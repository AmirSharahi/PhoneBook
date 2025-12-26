namespace PhoneBook.Domain.ValueObjects;

public sealed class FullName
{
    public string FirstName { get; }
    public string LastName { get; }

    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName cannot be empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName cannot be empty.", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
    }

    public override string ToString() => $"{FirstName} {LastName}";
}