using PhoneBook.Domain.Aggregates.ContactAggregate;
using PhoneBook.Domain.ValueObjects;
using Xunit;

public sealed class ContactTests
{
    [Fact]
    public void CreateContact()
    {
        var contact = new Contact(
            new FullName("Amir", "Sharahi"),
            new PhoneNumber("09120000000"),
            "From Work");

        Assert.NotNull(contact);
        Assert.Equal("Amir Sharahi", contact.Name.ToString());
        Assert.Equal("09120000000", contact.PhoneNumber.ToString());
        Assert.Equal("From Work", contact.Description);
    }

    [Fact]
    public void UpdateTags()
    {
        var contact = new Contact(
            new FullName("Amir", "Sharahi"),
            new PhoneNumber("09120000000"),
            "From Work");

        IReadOnlyCollection<Tag> tags = [new Tag("Work")];

        contact.UpdateTags(tags);

        Assert.Single(contact.Tags);
    }
}
