using PhoneBook.Application.Interfaces.Persistence;

namespace PhoneBook.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
