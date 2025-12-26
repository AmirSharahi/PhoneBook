using Moq;
using PhoneBook.Application.Features.Contacts.GetContactsByTag;
using PhoneBook.Domain.Repositories;
using Xunit;

public sealed class GetContactsByTagTests
{
    [Fact]
    public async Task GetContactsByTag()
    {
        var repository = new Mock<IContactReadRepository>();

        repository
            .Setup(r => r.GetByTagAsync("Work", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = new GetContactsByTagHandler(repository.Object);

        await handler.Handle(new GetContactsByTagQuery("Work"), CancellationToken.None);
    }
}
