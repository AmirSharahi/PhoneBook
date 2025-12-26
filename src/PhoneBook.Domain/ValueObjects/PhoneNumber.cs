using System.Text.RegularExpressions;

namespace PhoneBook.Domain.ValueObjects;

public sealed class PhoneNumber
{
    public string Number { get; }

    public PhoneNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Phone number cannot be empty.", nameof(number));

        if (!Regex.IsMatch(number, @"^(?:0|\+98|98)?9\d{9}$"))
            throw new ArgumentException("Invalid phone number format.", nameof(number));

        Number = number;
    }

    public override string ToString() => Number;

    public override bool Equals(object? obj)
    {
        if (obj is not PhoneNumber other) return false;
        return Number == other.Number;
    }
}