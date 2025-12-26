using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PhoneBook.Application.Common.Behaviors;
using PhoneBook.Application.Features.Contacts.CreateContact;
using PhoneBook.Application.Interfaces.Persistence;
using PhoneBook.Application.Mappings;
using PhoneBook.Domain.Repositories;
using PhoneBook.Infrastructure.Persistence;

namespace PhoneBook.Infrastructure.Configurations;

public static class DependencyInjections
{
    public static IServiceCollection AddDependencyInjections(this IServiceCollection services)
    {
        #region InternalServices
        services.AddSingleton<IContactWriteRepository, ContactWriteRepository>();
        services.AddSingleton<IContactReadRepository, ContactReadRepository>();
        services.AddSingleton<IUnitOfWork, UnitOfWork>();
        #endregion

        #region Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        #endregion

        #region Packages
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateContactCommand).Assembly));
        ContactMappings.Register();
        TypeAdapterConfig.GlobalSettings.Scan(typeof(ContactMappings).Assembly);
        services.AddValidatorsFromAssembly(typeof(CreateContactCommand).Assembly);
        #endregion

        return services;
    }
}
