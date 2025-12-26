using PhoneBook.Domain.Aggregates.ContactAggregate;
using Xunit;

public sealed class TagTests
{
    [Fact]
    public void CreateTag()
    {
        var tag = new Tag("Work");

        Assert.Equal("Work", tag.Name);
    }

    [Fact]
    public void EmptyTagName()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new Tag("");
        });
    }
}