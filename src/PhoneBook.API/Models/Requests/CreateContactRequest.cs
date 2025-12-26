namespace PhoneBook.API.Models.Requests;

public sealed record CreateContactRequest(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string? Description,
    IReadOnlyCollection<string> Tags
);
