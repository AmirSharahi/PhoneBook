# Architecture

PhoneBook is a REST API for managing contacts, implemented with ASP.NET Core on .NET 10. The solution keeps the domain model, the application use cases, the in-memory infrastructure, and the HTTP API in separate projects.

## What the API does

- Create a contact with a first name, a last name, a phone number, an optional description, and tags.
- Update an existing contact.
- List contacts that have a given tag.
- Delete a contact.

Contacts are stored in memory. The solution has no database and no ORM.

## Projects and dependencies

| Project | Responsibility |
| --- | --- |
| `PhoneBook.Domain` | Aggregate, value objects, domain exceptions, repository interfaces |
| `PhoneBook.Application` | Commands, queries, handlers, validators, DTOs, pipeline behaviors |
| `PhoneBook.Infrastructure` | In-memory repositories, unit of work, composition root |
| `PhoneBook.API` | Controllers, HTTP request models, exception middleware, Swagger |

`PhoneBook.Domain` has no project or package references. `PhoneBook.Application` references the domain. `PhoneBook.Infrastructure` references the application and the domain. `PhoneBook.API` references the infrastructure and the domain, and uses application types through the infrastructure project reference.

`PhoneBook.Domain.Tests` references the domain. `PhoneBook.Application.Tests` references the application.

## Domain model

`Contact` is the aggregate root. It extends `RemovableEntity`, which extends `BaseEntity`. `BaseEntity` assigns a `Guid` identifier at construction. `RemovableEntity` adds no members of its own. Deleting a contact removes it from the in-memory collection.

`Contact` keeps its state private and changes it through methods:

- `FullName Name`
- `PhoneNumber PhoneNumber`
- `string? Description`
- `IReadOnlyCollection<Tag> Tags`, backed by a private list

`UpdateTags` adds tags whose names are missing and removes tags whose names are absent from the replacement set. That comparison uses ordinary string equality.

`FullName` and `PhoneNumber` are value objects.

- `FullName` requires a non-whitespace first name and last name. `ToString` returns `"<first> <last>"`.
- `PhoneNumber` requires a non-whitespace value matching `^(?:0|\+98|98)?9\d{9}$`. Equality compares the stored number string. The value is kept as submitted; the constructor does not rewrite prefixes.

`Tag` is part of the contact aggregate. Its constructor rejects a null, empty, or whitespace name.

Domain exceptions:

- `ContactNotFoundException` — no contact exists for the given id.
- `DuplicatePhoneNumberException` — the phone number is already stored.

Repository contracts live in the domain so the application can depend on them without referencing infrastructure:

- `IContactWriteRepository` — add, update, delete, get by id, and check whether a phone number exists.
- `IContactReadRepository` — get contacts by tag name.

## Application layer

Use cases are vertical slices under `Features/Contacts`. A slice owns one request and its handler. Create and update also own a FluentValidation validator.

| Slice | Request | Result |
| --- | --- | --- |
| `CreateContact` | `CreateContactCommand` | `Guid` |
| `UpdateContact` | `UpdateContactCommand` | `ContactDto` |
| `DeleteContact` | `DeleteContactCommand` | no value |
| `GetContactsByTag` | `GetContactsByTagQuery` | `IReadOnlyCollection<ContactDto>` |

Commands carry the data needed to change state. The query carries the tag to filter on. Handlers are the only application types that call repositories.

`CreateContactHandler` builds a `PhoneNumber`, rejects it with `DuplicatePhoneNumberException` when `PhoneNumberExistsAsync` is true, then constructs the `Contact` and adds it. `UpdateContactHandler` loads the contact, applies the new name, phone number, description, and tags, calls `UpdateAsync`, and then `IUnitOfWork.SaveChangesAsync`. `DeleteContactHandler` loads the contact, deletes it by id, and saves. A missing contact produces `ContactNotFoundException` from the update and delete handlers. `GetContactsByTagHandler` reads through `IContactReadRepository` and maps the result.

`ContactDto` is the type returned to the API: `Id`, `FullName`, `PhoneNumber`, `Description`, and `Tags`. `ContactMappings` configures Mapster so `FullName` becomes the value object's string form, `PhoneNumber` becomes its `Number`, and tags become the tag names. The composition root calls `ContactMappings.Register` and scans the application assembly for Mapster configuration.

`IUnitOfWork` lives in the application layer and exposes `SaveChangesAsync`. Update and delete call it. Create does not; `AddAsync` inserts the contact directly.

### Pipeline behaviors

Handlers stay focused on the use case. Cross-cutting work is registered as MediatR `IPipelineBehavior<,>` implementations in this order:

1. `ValidationBehavior` collects every `IValidator<TRequest>` for the request. When any rule fails it throws `ValidationException` and does not call the handler.
2. `LoggingBehavior` writes an information log with the request type name and the request object, calls the next delegate, then writes an information log that the request completed.

`DependencyInjections.AddDependencyInjections` registers both behaviors, then registers MediatR handlers and FluentValidation validators from the application assembly (`CreateContactCommand`'s assembly).

### Command validation

`CreateContactValidator` and `UpdateContactValidator` apply the same rules:

- `FirstName` and `LastName` are not empty and are at most 50 characters.
- `PhoneNumber` is not empty and matches `^\+?\d{7,15}$`.

Description, tags, and the contact id have no FluentValidation rules. Tag names are checked by the `Tag` constructor when a handler builds them. Names and phone numbers that reach a value object are checked again there. A value can pass the command validator and still fail the domain phone-number pattern; that failure is an `ArgumentException` from `PhoneNumber`.

The get-by-tag query has no validator.

## Infrastructure

`PhoneBookDbContext` is a static class with one field: a `List<Contact>`.

`ContactWriteRepository` and `ContactReadRepository` use that list.

- `AddAsync` appends the contact and returns its id.
- `GetByIdAsync` returns the contact with that id, or null.
- `DeleteAsync` removes the matching contact when one is present.
- `PhoneNumberExistsAsync` uses `PhoneNumber.Equals`.
- `UpdateAsync` returns a completed task. The handler has already mutated the instance stored in the list.
- `GetByTagAsync` returns contacts that have a tag of that name, compared with `StringComparison.OrdinalIgnoreCase`.

`UnitOfWork.SaveChangesAsync` returns a completed task. With this store, the list already contains the change when a handler finishes mutating it.

All three services are registered as singletons, so they share one list for the life of the process. The repository interfaces and `IUnitOfWork` are the seam a persistent implementation would replace. Replacing them does not require a change to the handlers.

## HTTP API

`ContactsController` uses the route template `api/[controller]`, which is `/api/Contacts`. ASP.NET Core matches that route without regard to case, so `/api/contacts` addresses the same resource. The controller depends on `IMediator`. Request bodies are `CreateContactRequest` and `UpdateContactRequest`. Mapster adapts those records onto the matching commands. For update, the route id is copied onto `UpdateContactCommand.ContactId` after that adaptation.

| Method | Path | Handler result | Success status |
| --- | --- | --- | --- |
| `GET` | `/api/contacts?tag={tag}` | `IReadOnlyCollection<ContactDto>` | 200 |
| `POST` | `/api/contacts` | `Guid` | 200 |
| `PUT` | `/api/contacts/{id}` | `ContactDto` | 200 |
| `DELETE` | `/api/contacts/{id}` | none | 200, empty body |

`Program` registers controllers, Swagger, and `AddDependencyInjections`. The middleware pipeline is `ExceptionHandlerMiddleware`, then Swagger when the host environment is Development, then the controllers. Swagger is configured by `AddSwaggerDocumentation`: document `v1` titled "PhoneBook API", with the XML comments generated from the API assembly.

Launch profiles in `Properties/launchSettings.json` set the environment to Development.

- `http` listens on `http://localhost:5075`.
- `https` listens on `https://localhost:7210` and `http://localhost:5075`.

`dotnet run` uses the first profile, `http`. In Development, startup also tries to open `https://localhost:7210/swagger`.

### Error responses

`ExceptionHandlerMiddleware` catches exceptions from the rest of the pipeline and writes JSON:

```json
{
  "error": "Contact with ID '<id>' was not found.",
  "statusCode": 404
}
```

| Exception | HTTP status | `error` value |
| --- | --- | --- |
| `ContactNotFoundException` | 404 | exception message |
| `DuplicatePhoneNumberException` | 409 | exception message |
| `ArgumentException` | 400 | exception message |
| Any other exception | 500 | "An unexpected error occurred." |

`ValidationException` inherits from `Exception`. The middleware's `ArgumentException` branch does not match it, so a FluentValidation failure uses the 500 response and the validation messages stay out of the body. Domain constructor failures that surface as `ArgumentException`, such as a phone number outside the domain pattern or an empty tag, use the 400 response.

Duplicate phone numbers are checked only in `CreateContactHandler`. Update can store a number that another contact already has.

## Tests

The test projects use xUnit and Moq.

`PhoneBook.Domain.Tests` constructs a `Contact`, replaces its tags, constructs a `Tag`, and expects `ArgumentException` for an empty tag name.

`PhoneBook.Application.Tests` calls the create, update, and get-by-tag handlers directly. The create and get-by-tag tests supply a Moq repository. The update test supplies a repository that returns an existing contact, and a Moq unit of work. These tests exercise the handler flow; they do not host the HTTP pipeline.
