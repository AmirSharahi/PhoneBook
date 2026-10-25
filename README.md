# PhoneBook API

[![CI](https://github.com/AmirSharahi/PhoneBook/actions/workflows/ci.yml/badge.svg)](https://github.com/AmirSharahi/PhoneBook/actions/workflows/ci.yml)

PhoneBook is an ASP.NET Core REST API for storing and retrieving contacts. Each contact has a name, a phone number, an optional description, and a set of tags. The API creates, updates, and deletes contacts, and lists the contacts that carry a given tag.

The solution targets .NET 10 and is organized with domain-driven design, CQRS through MediatR, vertical slices, and FluentValidation. Contacts live in memory for the lifetime of the process. The design is described in more detail in [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## Architecture

`PhoneBook.Infrastructure` references `PhoneBook.Application` and `PhoneBook.Domain`. `PhoneBook.API` references `PhoneBook.Infrastructure` and `PhoneBook.Domain`, and compiles against the application types through that infrastructure reference. `PhoneBook.Application` references only the domain. The domain project references nothing else.

**Aggregates and value objects.** `Contact` is the aggregate root. Its name and phone number are the value objects `FullName` and `PhoneNumber`, and both reject invalid values in their constructors. Tags belong to the contact; a `Tag` rejects an empty name. Callers outside the domain receive `ContactDto`, which is mapped with Mapster.

**CQRS.** Create, update, and delete are commands. Listing contacts by tag is a query. Each use case has its own MediatR request and handler. The contacts controller sends those requests through `IMediator`.

**MediatR pipeline.** MediatR runs two pipeline behaviors, registered in this order:

1. `ValidationBehavior` runs the FluentValidation validators for the request and throws `ValidationException` when a rule fails.
2. `LoggingBehavior` logs the request name and payload, invokes the rest of the pipeline, and logs success.

**Vertical slices.** Application code is grouped by use case under `Features/Contacts`. Create and update each contain a command, a handler, and a validator. Delete contains a command and a handler. Get-by-tag contains a query and a handler.

**Validation and error handling.** Create and update require a non-empty first and last name of at most 50 characters, and a phone number matching `^\+?\d{7,15}$`. The domain phone number is stricter: `^(?:0|\+98|98)?9\d{9}$`. `ExceptionHandlerMiddleware` writes a JSON body with `error` and `statusCode`:

| Exception | Status |
| --- | --- |
| `ContactNotFoundException` | 404 |
| `DuplicatePhoneNumberException` | 409 |
| `ArgumentException` | 400 |
| Anything else, including `FluentValidation.ValidationException` | 500, with the message "An unexpected error occurred." |

**Persistence.** `PhoneBookDbContext` holds a static list of contacts. The read repository, the write repository, and the unit of work are singletons over that list. Adding a contact appends to the list. Deleting a contact removes it. Update changes the `Contact` instance already in the list, and the in-memory `SaveChangesAsync` completes without further work.

## Project layout

```text
PhoneBook.slnx
global.json
src/
  PhoneBook.API/                 Controllers, request models, exception middleware, Swagger
  PhoneBook.Application/         Commands, queries, handlers, validators, DTOs, pipeline behaviors
  PhoneBook.Domain/              Contact aggregate, value objects, repository interfaces, exceptions
  PhoneBook.Infrastructure/      In-memory repositories, unit of work, dependency injection
tests/
  PhoneBook.Domain.Tests/        Contact and tag behavior
  PhoneBook.Application.Tests/   Create, update, and get-by-tag handlers
docs/
  ARCHITECTURE.md                Architecture and request flow
.github/workflows/
  ci.yml                         Build and test on push and pull request
```

## Run the API

Install the .NET 10 SDK. `global.json` selects that SDK, rolling forward within the 10.0 feature band. From the repository root:

```bash
dotnet run --project src/PhoneBook.API
```

The `http` launch profile listens on `http://localhost:5075` and sets `ASPNETCORE_ENVIRONMENT` to `Development`. Swagger UI is enabled only when the environment is Development:

[http://localhost:5075/swagger](http://localhost:5075/swagger)

The `https` profile also listens on `https://localhost:7210`:

```bash
dotnet run --project src/PhoneBook.API --launch-profile https
```

Swagger UI is then at [https://localhost:7210/swagger](https://localhost:7210/swagger). On startup in Development, the host tries to open `https://localhost:7210/swagger` in a browser. That address is served by the `https` profile.

### Endpoints

The controller route template is `api/[controller]`. Routing is case-insensitive, so the paths below match `/api/Contacts` as well.

| Method | Path | Success response |
| --- | --- | --- |
| `GET` | `/api/contacts?tag={tag}` | `200` and the matching contacts. Tag comparison is ordinal and case-insensitive. |
| `POST` | `/api/contacts` | `200` and the new contact id (`Guid`). |
| `PUT` | `/api/contacts/{id}` | `200` and the updated contact. |
| `DELETE` | `/api/contacts/{id}` | `200` with an empty body. |

Create and update take this JSON body:

```json
{
  "firstName": "Ada",
  "lastName": "Lovelace",
  "phoneNumber": "09120000000",
  "description": "Colleague",
  "tags": ["Work"]
}
```

`09120000000` satisfies both the command validator and the domain phone-number rule. The number is stored as submitted. Create rejects a number that is already present.

A contact returned by the API has `id`, `fullName` (`"<first> <last>"`), `phoneNumber`, `description`, and `tags`.

## Tests

```bash
dotnet test PhoneBook.slnx
```

`dotnet test` restores and builds the solution, then runs both test projects. `PhoneBook.Domain.Tests` covers creating a contact, replacing its tags, and rejecting an empty tag name. `PhoneBook.Application.Tests` executes the create, update, and get-by-tag handlers against Moq repositories. The same build and test commands run in GitHub Actions on every push and pull request:

```bash
dotnet build PhoneBook.slnx --configuration Release
dotnet test PhoneBook.slnx --configuration Release --no-build
```

## Tech stack

| Area | Choice |
| --- | --- |
| Runtime | .NET 10 (`net10.0`) |
| HTTP | ASP.NET Core controllers |
| Application bus | MediatR 14.0.0 |
| Validation | FluentValidation 12.1.1 |
| Mapping | Mapster 7.4.0 |
| OpenAPI | Swashbuckle.AspNetCore 10.1.0 and Microsoft.AspNetCore.OpenApi 10.0.1 |
| Tests | xUnit 2.9.3, Moq 4.20.72, Microsoft.NET.Test.Sdk 18.0.1, xunit.runner.visualstudio 3.1.5 |
| Storage | Static in-memory list, held for the process lifetime |
