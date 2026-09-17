# .NET API HealthTracker

> Based on the **BeCause / WikiEditor** architecture patterns.

---

## Solution Structure

```
DotNetApiHealthTracker/
+-- Domain/                  # Enterprise rules — no external deps except MediatR
+-- Application/             # CQRS: MediatR + FluentValidation + Mapperly
+-- Infrastructure/          # EF Core, Redis, MassTransit, HTTP clients
+-- Contracts/               # Public DTOs (Mapperly source)
+-- EventContracts/          # MassTransit bus event contracts
+-- Api/                     # REST presentation (Startup, versioned controllers)
+-- Api.GraphQl/             # HotChocolate GraphQL (read-only mirror)
+-- BackgroundTasks/         # MassTransit consumers / worker services
+-- DatabaseMigration/       # EF Core migrations
+-- build/                   # Nuke build automation
+-- *.UnitTests / *.IntegrationTests
```

---

## Key Patterns (from WikiEditor)

| Pattern | Where |
|---|---|
| Clean Architecture | Layer project refs direction |
| CQRS (Read/Write DbContext) | `IReadApplicationDbContext` / `IWriteApplicationDbContext` |
| MediatR Command + Handler co-located | `Application/Commands/<Aggregate>/<UseCase>/` |
| FluentValidation pipeline behaviour | `Application/Common/Behaviours/ValidationBehaviour.cs` |
| Partial controller split by HTTP verb | `Api/Controllers/<Resource>Controller_Get/Post/Put/Delete.cs` |
| Domain events post-commit | `WriteApplicationDbContext.SaveChangesAsync` |
| Cache-aside (Redis) | `ICacheRepository<T>` + `ProductCacheRepository` |
| MassTransit (RabbitMQ) | `Infrastructure/ServiceRegistrations/AsynchronousMessagingRegistration.cs` |
| HotChocolate GraphQL (projections) | `Api.GraphQl/Queries/ProductQuery.cs` |
| Mapperly (compile-time DTO mapping) | `Application/Common/Mappings/ProductMapper.cs` |
| Nuke build | `build/Build.cs` |
| Typed options + ValidateOnStart | `AddOptions<T>().Bind().ValidateDataAnnotations().ValidateOnStart()` |

---

## Getting Started

### Prerequisites
- .NET 10 SDK
- PostgreSQL
- RabbitMQ (`docker-compose up -d rabbitmq`)
- Redis (`docker-compose up -d redis`)

### Run the REST API
```bash
cd Api
dotnet run
# Swagger UI: https://localhost:5001/swagger
```

### Run the GraphQL API
```bash
cd Api.GraphQl
dotnet run
# Banana Cake Pop: https://localhost:5002/graphql
```

### Run Tests
```bash
dotnet test
```

### Database Migrations
```bash
dotnet ef migrations add InitialCreate --project DatabaseMigration --startup-project Api
dotnet ef database update --project DatabaseMigration --startup-project Api
```

---

## Renaming the HealthTracker

1. Replace all `HealthTracker.` namespace prefixes with `YourCompany.YourDomain.`
2. Replace the `Product` aggregate with your domain aggregate.
3. Update `ProductScopes` in `Api/ScopesPermissions.cs`.
4. Update connection strings in `appsettings.json`.
5. Wire your identity provider in `Startup.cs`.

---

## Architecture Decision Records

See [`wikieditor_patterns.md`](../wikieditor_patterns.md) for the full reference document extracted from the WikiEditor project.
