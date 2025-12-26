T﻿his cover letter was drafted with the assistance of an AI tool to improve clarity and structure;
however, the project itself was fully designed and implemented based on my own knowledge, experience, and architectural decisions.

---

# PhoneBook API – Architecture & Implementation Notes

This project is a simple Phone Book RESTful API implemented with **ASP.NET Core**.
The main focus of the implementation is **clean architecture, correct domain modeling, and clear separation of responsibilities**, rather than data persistence or UI complexity.

---

## Project Overview

The API provides the following features:

- Create a contact with first name, last name, phone number and tags
- Update an existing contact
- Retrieve contacts by a specific tag
- Delete a contact

All data is stored **in memory**, as requested, without using any database or ORM.

---

## Architectural Approach

### Domain-Driven Design (DDD)

The core of the system is modeled using DDD principles:

- The `Contact` is implemented as an **Aggregate Root**
- Important concepts such as **FullName** and **PhoneNumber** are modeled as **Value Objects**
- Business rules and validations are placed inside the domain, not in controllers or services
- Domain entities are not exposed directly to the API layer

This helps keep the business logic consistent and easy to reason about.

---

### CQRS

The application follows **Command Query Responsibility Segregation**:

- **Commands** handle state changes (Create, Update, Delete)
- **Queries** handle read operations
- Each use case has its own handler
- Controllers only send commands/queries and do not access repositories directly

This approach keeps the codebase clear and avoids mixing read and write concerns.

---

### Vertical Slice Structure

The project is organized using a **Vertical Slice approach**, where each feature is grouped by use case rather than technical layers.

Each slice typically contains:
- Command or Query
- Handler
- Validation

This structure makes features easier to maintain and extend, and reduces unnecessary dependencies between unrelated parts of the system.

---

### In-Memory Persistence

Repositories are implemented using **in-memory collections**.
The repository interfaces are designed in a way that allows replacing the in-memory implementation with a real database in the future without changing the application or domain logic.

---

### Cross-Cutting Concerns

Concerns such as **validation and logging** are handled centrally using MediatR pipeline behaviors.
This avoids repeating validation or logging logic in each handler and keeps the handlers focused on business logic.

---

## API & Documentation

- All endpoints are designed in a RESTful manner
- Swagger is integrated and available for exploring and testing the API

---

## Testing

Basic tests are included to demonstrate:
- Domain behavior
- Command and query execution flow

The goal of the tests is to show architectural correctness rather than full production-level coverage.

---

## Final Notes

Although the project is intentionally simple, the architecture is designed to be **scalable and extensible**.
It can be easily extended with:
- Persistent storage
- Additional business rules
- More advanced validation and authorization

The focus was on writing clean, understandable code that reflects real-world backend development practices.
