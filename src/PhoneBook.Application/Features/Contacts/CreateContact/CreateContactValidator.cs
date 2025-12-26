using FluentValidation;

namespace PhoneBook.Application.Features.Contacts.CreateContact;

public sealed class CreateContactValidator
    : AbstractValidator<CreateContactCommand>
{
    public CreateContactValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?\d{7,15}$");
    }
}
