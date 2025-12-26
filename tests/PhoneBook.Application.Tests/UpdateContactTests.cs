using Moq;
using PhoneBook.Application.Features.Contacts.UpdateContact;
using PhoneBook.Application.Interfaces.Persistence;
using PhoneBook.Domain.Aggregates.ContactAggregate;
using PhoneBook.Domain.Repositories;
using PhoneBook.Domain.ValueObjects;
using Xunit;

public sealed class UpdateContactTests
{
    [Fact]
    public async Task UpdateContact()
    {
        var repository = new Mock<IContactWriteRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var contact = new Contact(
            new FullName("Amir", "Sharahi"),
            new PhoneNumber("09120000000"),
            "Old friend");

        repository
            .Setup(r => r.GetByIdAsync(contact.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contact);

        var handler = new UpdateContactHandler(repository.Object, unitOfWork.Object);

        var command = new UpdateContactCommand(contact.Id, "Amir", "Sharahi", "09120000000", "I know him from work");

        await handler.Handle(command, CancellationToken.None);
    }
}