namespace PhoneBook.Domain.Exceptions;

public class DuplicatePhoneNumberException : Exception
{
    public DuplicatePhoneNumberException(string phoneNumber)
        : base($"Phone number '{phoneNumber}' already exists.") { }
}
