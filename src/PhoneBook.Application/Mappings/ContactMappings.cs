using Mapster;
using PhoneBook.Application.DTOs;
using PhoneBook.Domain.Aggregates.ContactAggregate;

namespace PhoneBook.Application.Mappings;

public static class ContactMappings
{
    public static void Register()
    {
        TypeAdapterConfig<Contact, ContactDto>
            .NewConfig()
            .Map(dest => dest.FullName, src => src.Name.ToString())
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber.Number)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Tags, src => src.Tags.Select(t => t.Name).ToList().AsReadOnly());
    }
}