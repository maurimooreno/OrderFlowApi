# OrderFlow

OrderFlow is an operations integration platform built with .NET. It receives operations from external systems, persists them, exposes their status, processes them asynchronously, and allows failed operations to be retried.

The project is designed as a professional backend portfolio piece covering REST APIs, system integration, background processing, SQL Server, Entity Framework Core, Docker, and a straightforward enterprise architecture.

## Scope

The MVP includes:

- Operation creation.
- Full operation retrieval.
- Operation status retrieval.
- Asynchronous processing through a Worker.
- Manual retry of failed operations.
- SQL Server persistence with EF Core.
- Structured logging with `ILogger<T>`.
- Azure Service Bus Emulator as the shared local broker.
- Docker Compose for the API, Worker, and emulator.

Out of scope for the MVP:

- Frontend.
- Advanced authentication.
- Cloud Azure Service Bus and brokers other than the emulator.
- Cloud infrastructure.
- Advanced observability.

## Architecture

The solution uses a simplified Clean Architecture:

```text
OrderFlow.Api
OrderFlow.Application
OrderFlow.Domain
OrderFlow.Infrastructure
OrderFlow.Persistence
OrderFlow.Worker
```

Main responsibilities:

- `OrderFlow.Api`: REST endpoints, configuration, and dependency injection.
- `OrderFlow.Application`: use cases, DTOs, contracts, and interfaces.
- `OrderFlow.Domain`: entities, enums, and business rules.
- `OrderFlow.Infrastructure`: selectable Azure Service Bus or in-memory messaging, plus a simulated external integration.
- `OrderFlow.Persistence`: `DbContext`, EF Core mappings, migrations, and repositories.
- `OrderFlow.Worker`: message consumption and asynchronous processing.

Main flow:

```text
Client -> REST API -> SQL Server -> Azure Service Bus Emulator -> Worker -> Simulated external system
```

## Technology Stack

- .NET 10
- C#
- ASP.NET Core Web API
- Worker Service
- Entity Framework Core
- SQL Server
- OpenAPI
- Docker
- Azure Service Bus Emulator

## Prerequisites

- .NET 10 SDK
- A local or remote SQL Server instance
- `dotnet-ef` installed for migrations
- Docker for container-based execution

## Configuration

Configure the connection string in:

- `src/OrderFlow.Api/appsettings.json`
- `src/OrderFlow.Worker/appsettings.json`

Example:

```json
{
  "ConnectionStrings": {
    "OrderFlowDb": "Server=localhost;Database=OrderFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For containers, pass the connection string through an environment variable:

```powershell
-e "ConnectionStrings__OrderFlowDb=Server=host.docker.internal;Database=OrderFlowDb;User Id=sa;Password=your_password;TrustServerCertificate=True;"
```

## Database

Apply migrations:

```powershell
dotnet ef database update --project .\src\OrderFlow.Persistence\OrderFlow.Persistence.csproj --startup-project .\src\OrderFlow.Persistence\OrderFlow.Persistence.csproj
```

Create a future migration:

```powershell
dotnet ef migrations add MigrationName --project .\src\OrderFlow.Persistence\OrderFlow.Persistence.csproj --startup-project .\src\OrderFlow.Persistence\OrderFlow.Persistence.csproj --output-dir Migrations
```

Generate a SQL script:

```powershell
dotnet ef migrations script --project .\src\OrderFlow.Persistence\OrderFlow.Persistence.csproj --startup-project .\src\OrderFlow.Persistence\OrderFlow.Persistence.csproj --output .\src\OrderFlow.Persistence\Scripts\MigrationName.sql
```

## Build

From the repository root:

```powershell
dotnet build .\src\OrderFlowAPI.slnx
```

## Local Execution

API:

```powershell
dotnet run --project .\src\OrderFlow.Api\OrderFlow.Api.csproj
```

Worker:

```powershell
dotnet run --project .\src\OrderFlow.Worker\OrderFlow.Worker.csproj
```

In the Development environment, the OpenAPI document is available according to the ASP.NET Core configuration.

## Endpoints

### Create an Operation

```http
POST /api/operations
```

Request:

```json
{
  "externalReference": "EXT-1001",
  "customerName": "Jane Doe",
  "customerEmail": "jane.doe@example.com",
  "totalAmount": 1250.75,
  "currency": "USD"
}
```

Responses:

- `201 Created`
- `400 Bad Request`

### Retrieve an Operation

```http
GET /api/operations/{id}
```

Responses:

- `200 OK`
- `404 Not Found`

### Retrieve Operation Status

```http
GET /api/operations/{id}/status
```

Responses:

- `200 OK`
- `404 Not Found`

### Retry a Failed Operation

```http
POST /api/operations/{id}/retry
```

Responses:

- `200 OK`
- `404 Not Found`
- `409 Conflict`

## Operation Statuses

- `Pending`
- `Processing`
- `Completed`
- `Failed`
- `Cancelled`

## Docker

Build each image individually:

```powershell
docker build -f .\src\OrderFlow.Api\Dockerfile -t orderflow-api .
docker build -f .\src\OrderFlow.Worker\Dockerfile -t orderflow-worker .
```

For the local MVP 2 environment, define `ACCEPT_EULA`, `MSSQL_SA_PASSWORD`, and `ORDERFLOW_DB_CONNECTION_STRING`, then run:

```powershell
docker compose up --build
```

`OrderFlowDb` remains external to Docker Compose and must be available with migrations applied.

## Known Limitations

- The emulator is for local development only and is not a production broker.
- `OrderFlowDb` must be available outside the containers and have migrations applied.
- Advanced authentication is not implemented.
- There are no real third-party integrations.

## Possible Next Improvements

- Add authentication.
- Add automated tests.
- Add global error handling.
- Improve observability.
