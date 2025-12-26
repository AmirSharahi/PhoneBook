using Moq;
using PhoneBook.Application.Features.Contacts.CreateContact;
using PhoneBook.Domain.Repositories;
using Xunit;

public sealed class CreateContactTests
{
    [Fact]
    public async Task CreateContact()
    {
        var repository = new Mock<IContactWriteRepository>();
        var handler = new CreateContactHandler(repository.Object);

        var command = new CreateContactCommand("Amir", "Sharahi", "09120000000", "I know him from work", ["Work"]);

        await handler.Handle(command, CancellationToken.None);
    }
}