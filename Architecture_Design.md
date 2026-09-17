# Architecture Design: Daily Exposome Tracker

## 1. System Overview
The solution relies on a decoupled architecture featuring a React Native mobile client, an ASP.NET Core API exposing a GraphQL endpoint, and a PostgreSQL database optimized for hybrid relational/document storage.

## 2. Technology Stack
* **Mobile Client:** React Native for iOS and Android.
* **API Gateway / Backend:** ASP.NET Core with GraphQL (e.g., HotChocolate).
* **Database:** PostgreSQL.
* **ORM:** Entity Framework Core.
* **Background Processing:** .NET Hosted Services (`IHostedService` / `BackgroundService`) or task orchestrators (e.g., Hangfire/Quartz) for executing third-party API calls.

## 3. Database Schema Strategy
To support a highly flexible, evolving data model while maintaining queryability, the system utilizes PostgreSQL's `JSONB` capabilities mapped through EF Core JSON columns. 

**Table: `DailyRecords`**
* `Id` (UUID, Primary Key)
* `UserId` (UUID, Foreign Key)
* `RecordDate` (Date)
* `RawInput` (Text)
* `FoodData` (JSONB) - Contains arrays of extracted ingredients and macros.
* `LocationEnvironmentData` (JSONB) - Contains weather, pollution, and coordinates.
* `MoodFeelingData` (JSONB) - Contains mood scores and temporal tagging.
* `MedicationData` (JSONB) - Contains drug interactions and active chemicals.

*Implementation Note: EF Core provides native support for mapping strongly typed C# objects to JSON columns, making this performant and clean for domain-driven design.*

## 4. API Design (GraphQL)
GraphQL allows the mobile client to request exactly the data it needs for the dashboard (e.g., querying only the `MoodFeelingData` across a month for a chart, without over-fetching dietary data).

**Key Mutation:**
* `SubmitDailyEntry(input: DailyEntryInput!): DailyEntryPayload!`
  * *Action:* Persists the raw input and publishes an event/command to queue the background enrichment task. Returns an immediate success acknowledgment to the mobile client.

## 5. Background Task Orchestration
When `SubmitDailyEntry` is invoked, the workload is offloaded to a background process to handle the orchestration saga:
1. Call LLM for entity extraction.
2. Execute parallel HTTP calls to external services (Edamam, RxNorm, Open-Meteo).
3. Hydrate the domain entities.
4. Update the `DailyRecords` table with the serialized JSON results and commit via EF Core.

## 6. Template & Library Integration
*This architecture is designed to be scaffolding-ready.* The specific solution folder structure, dependency injection setups, and preferred microservice or messaging patterns (e.g., CQRS implementations) will be injected dynamically once the custom Markdown template and library manifest are provided.
