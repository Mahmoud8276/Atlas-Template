# 01 — Architecture

Atlas Template is organized into four projects, each with a clearly defined responsibility. The separation is enforced through project dependencies, keeping infrastructure concerns isolated from the core of the application and making the template easier to extend and maintain.

## Dependency Rule

The projects follow a one-way dependency flow:

```text
Atlas.Template.Api
        │
        ▼
Atlas.Template.Services ──────► Atlas.Template.Infrastructure
        │                              │
        ▼                              ▼
Atlas.Template.Core ◄──────────────────┘
```

In terms of project references:

```text
Api            → Services
Services       → Core, Infrastructure
Infrastructure → Core
Core           → Nothing
```

`Atlas.Template.Core` is therefore the innermost layer and has no dependency on any other project.

You can verify these boundaries by inspecting the `<ProjectReference>` entries in each `.csproj` file.

![Architecture layers](../assets/01-architecture-layers.svg)

### Why does this matter?

The dependency structure prevents higher-level application logic from becoming tightly coupled to implementation details.

* **Core must remain independent** of EF Core, ASP.NET Core, and other infrastructure frameworks.
* **Services should contain application logic**, rather than HTTP-specific concerns such as `HttpContext` or status-code handling.
* **Infrastructure contains implementation details**, such as EF Core, database access, and external services.
* **API acts as the composition and HTTP layer**, exposing the application's functionality through controllers and configuring the application pipeline.

A useful rule when adding new code is:

> If a class needs to directly depend on an outer layer, reconsider whether it belongs in that layer or whether the dependency should be represented by an abstraction.

---

## What Belongs in Each Layer

### `Atlas.Template.Core`

The innermost layer containing the application's core models, contracts, and shared abstractions.

It contains:

* Domain models such as `AppUser`, `UserRole`, `RefreshToken`, and `BaseModel<TKey>`
* Interfaces such as `IGenericRepository`, `IUnitOfWork`, `ISpecification`, and `IDataSeeder`
* DTOs and shared data contracts
* Application exceptions and the `AppException` hierarchy
* Options classes
* Enums and other shared abstractions

**Core should not depend on EF Core, ASP.NET Core, or Infrastructure.**

---

### `Atlas.Template.Infrastructure`

Contains implementations that interact with external systems and persistence.

It contains:

* `AppDbContext`
* ASP.NET Core Identity persistence
* `GenericRepository<TModel, TKey>`
* `UnitOfWork`
* `SpecificationEvaluator`
* Entity configurations
* EF Core migrations
* Data seeders

Infrastructure implements the contracts defined by Core.

For example:

```text
Core
└── IGenericRepository
        ▲
        │ implements
        │
Infrastructure
└── GenericRepository
```

This allows application code to depend on repository behavior without depending directly on its implementation.

---

### `Atlas.Template.Services`

Contains the application's service and business logic.

It contains:

* Generic `ReadService` and `WriteService` base classes
* Feature-specific services such as `AccountService`, `UserService`, `TokenService`, and `EmailService`
* FluentValidation validators
* Mapster mappings
* `Response` and `Response<T>` models
* Service and infrastructure registration extensions

This is where most application features should be implemented.

Services coordinate the application's use cases while relying on abstractions and registered infrastructure components rather than handling HTTP concerns directly.

---

### `Atlas.Template.Api`

The HTTP and composition layer of the application.

It contains:

* API controllers
* Middleware such as `GlobalExceptionHandler`
* Swagger/OpenAPI configuration
* API versioning configuration
* Application startup and dependency injection composition
* HTTP-specific configuration

Controllers should remain thin:

```text
HTTP Request
     │
     ▼
Controller
     │
     ▼
Service
     │
     ▼
Infrastructure / Data
     │
     ▼
HTTP Response
```

Business logic should live in the Services layer rather than inside controllers.

---

# Adding a New Feature

To see how the architecture works in practice, consider adding a `Product` feature.

### 1. Core

Define the model and contracts required by the feature:

```csharp
public class Product : BaseModel<int>
{
    // ...
}
```

Add any required DTOs, enums, or abstractions.

### 2. Infrastructure

Add persistence-specific configuration if required:

```text
ProductConfiguration
```

Then create and apply the corresponding EF Core migration.

A dedicated repository is usually unnecessary for standard CRUD operations because the template's generic repository already provides the required functionality.

### 3. Services

Create the feature's application service:

```csharp
ProductService : WriteService<...>
```

Add validators and mappings required by the feature.

This is where feature-specific business rules should live.

### 4. API

Expose the feature through a controller:

```text
ProductsController
```

The controller should handle HTTP concerns and delegate the actual application logic to `ProductService`.

The resulting flow is:

```text
ProductController
       │
       ▼
ProductService
       │
       ▼
Generic Repository
       │
       ▼
AppDbContext
       │
       ▼
SQL Server
```

For a complete implementation walkthrough, see [13 — Building a New Feature](13-building-a-new-feature.md).

---

## Key Takeaway

The architecture is designed around **separation of responsibilities and explicit dependencies**.

When extending Atlas Template:

1. Put core models and abstractions in **Core**.
2. Put persistence and external-system implementations in **Infrastructure**.
3. Put application and business logic in **Services**.
4. Keep HTTP concerns inside **API**.

Following these boundaries keeps new features predictable and prevents infrastructure, business logic, and presentation concerns from becoming unnecessarily coupled.

---

**Next:** [02 — Generic Repository & Unit of Work](02-generic-repository-and-uow.md)
