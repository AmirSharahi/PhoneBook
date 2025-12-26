namespace PhoneBook.Application.DTOs;

public sealed class ContactDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = default!;
    public string PhoneNumber { get; init; } = default!;
    public string? Description { get; init; }
    public IReadOnlyCollection<string> Tags { get; init; } = Array.Empty<string>();
}
