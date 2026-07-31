# OrderFlow

OrderFlow es una plataforma de integracion de operaciones construida con .NET. Permite recibir operaciones desde sistemas externos, persistirlas, consultar su estado, procesarlas de forma asincronica y reintentar operaciones fallidas.

El proyecto esta pensado como portfolio profesional de backend, APIs REST, integracion entre sistemas, procesamiento en segundo plano, SQL Server, Entity Framework Core, Docker y arquitectura empresarial simple.

## Alcance

El MVP implementa:

- Creacion de operaciones.
- Consulta completa de una operacion.
- Consulta de estado.
- Procesamiento asincronico mediante Worker.
- Retry manual de operaciones fallidas.
- Persistencia en SQL Server con EF Core.
- Logging estructurado con `ILogger<T>`.
- Azure Service Bus Emulator como broker local compartido.
- Docker Compose para API, Worker y el emulador.

Fuera de alcance del MVP:

- Frontend.
- Autenticacion compleja.
- Azure Service Bus real en cloud y brokers distintos del emulador.
- Infraestructura cloud real.
- Observabilidad avanzada.

## Arquitectura

La solucion utiliza una Clean Architecture simplificada:

```text
OrderFlow.Api
OrderFlow.Application
OrderFlow.Domain
OrderFlow.Infrastructure
OrderFlow.Persistence
OrderFlow.Worker
```

Responsabilidades principales:

- `OrderFlow.Api`: endpoints REST, configuracion y dependency injection.
- `OrderFlow.Application`: casos de uso, DTOs, contratos e interfaces.
- `OrderFlow.Domain`: entidades, enums y reglas de negocio.
- `OrderFlow.Infrastructure`: mensajeria Azure Service Bus o in-memory seleccionable e integracion externa simulada.
- `OrderFlow.Persistence`: `DbContext`, configuraciones EF Core, migraciones y repositorios.
- `OrderFlow.Worker`: consumo de mensajes y procesamiento asincronico.

Flujo principal:

```text
Client -> API REST -> SQL Server -> Azure Service Bus Emulator -> Worker -> Simulated external system
```

## Tecnologias

- .NET 10
- C#
- ASP.NET Core Web API
- Worker Service
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- Docker
- Azure Service Bus Emulator

## Requisitos

- .NET 10 SDK
- SQL Server disponible localmente o remoto
- `dotnet-ef` instalado para migraciones
- Docker, opcional para ejecucion en contenedores

## Configuracion

Configurar la cadena de conexion en:

- `src/OrderFlow.Api/appsettings.json`
- `src/OrderFlow.Worker/appsettings.json`

Ejemplo:

```json
{
  "ConnectionStrings": {
    "OrderFlowDb": "Server=localhost;Database=OrderFlowDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Para contenedores, se recomienda pasarla por variable de entorno:

```powershell
-e "ConnectionStrings__OrderFlowDb=Server=host.docker.internal;Database=OrderFlowDb;User Id=sa;Password=your_password;TrustServerCertificate=True;"
```

## Base de datos

Aplicar migraciones:

```powershell
dotnet ef database update --project .\src\OrderFlow.Persistence\OrderFlow.Persistence.csproj --startup-project .\src\OrderFlow.Persistence\OrderFlow.Persistence.csproj
```

Crear una migracion futura:

```powershell
dotnet ef migrations add NombreDeLaMigracion --project .\src\OrderFlow.Persistence\OrderFlow.Persistence.csproj --startup-project .\src\OrderFlow.Persistence\OrderFlow.Persistence.csproj --output-dir Migrations
```

Generar script SQL:

```powershell
dotnet ef migrations script --project .\src\OrderFlow.Persistence\OrderFlow.Persistence.csproj --startup-project .\src\OrderFlow.Persistence\OrderFlow.Persistence.csproj --output .\src\OrderFlow.Persistence\Scripts\NombreDeLaMigracion.sql
```

## Compilacion

Desde la raiz del repositorio:

```powershell
dotnet build .\src\OrderFlowAPI.slnx
```

## Ejecucion local

API:

```powershell
dotnet run --project .\src\OrderFlow.Api\OrderFlow.Api.csproj
```

Worker:

```powershell
dotnet run --project .\src\OrderFlow.Worker\OrderFlow.Worker.csproj
```

En ambiente de desarrollo, OpenAPI queda disponible segun la configuracion de ASP.NET Core.

## Endpoints

### Crear operacion

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

Respuestas:

- `201 Created`
- `400 Bad Request`

### Consultar operacion

```http
GET /api/operations/{id}
```

Respuestas:

- `200 OK`
- `404 Not Found`

### Consultar estado

```http
GET /api/operations/{id}/status
```

Respuestas:

- `200 OK`
- `404 Not Found`

### Reintentar operacion fallida

```http
POST /api/operations/{id}/retry
```

Respuestas:

- `200 OK`
- `404 Not Found`
- `409 Conflict`

## Estados de operacion

- `Pending`
- `Processing`
- `Completed`
- `Failed`
- `Cancelled`

## Docker

Build:

```powershell
docker build -f .\src\OrderFlow.Api\Dockerfile -t orderflow-api .
docker build -f .\src\OrderFlow.Worker\Dockerfile -t orderflow-worker .
```

Para el entorno local MVP 2, definir `ACCEPT_EULA`, `MSSQL_SA_PASSWORD` y `ORDERFLOW_DB_CONNECTION_STRING`, y ejecutar:

```powershell
docker compose up --build
```

La base `OrderFlowDb` permanece externa al Compose y debe tener las migraciones aplicadas.

## Limitaciones conocidas

- El emulador es solo para desarrollo local; no es un broker de produccion.
- `OrderFlowDb` debe estar disponible fuera de los contenedores y con migraciones aplicadas.
- No hay autenticacion avanzada.
- No hay integraciones reales con terceros.

## Proximas mejoras posibles

- Agregar autenticacion.
- Agregar tests automatizados.
- Agregar manejo global de errores.
- Mejorar observabilidad.
