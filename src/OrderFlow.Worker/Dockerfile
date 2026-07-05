FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/OrderFlowAPI.slnx ./src/
COPY src/OrderFlow.Worker/OrderFlow.Worker.csproj ./src/OrderFlow.Worker/
COPY src/OrderFlow.Application/OrderFlow.Application.csproj ./src/OrderFlow.Application/
COPY src/OrderFlow.Domain/OrderFlow.Domain.csproj ./src/OrderFlow.Domain/
COPY src/OrderFlow.Infrastructure/OrderFlow.Infrastructure.csproj ./src/OrderFlow.Infrastructure/
COPY src/OrderFlow.Persistence/OrderFlow.Persistence.csproj ./src/OrderFlow.Persistence/

RUN dotnet restore ./src/OrderFlow.Worker/OrderFlow.Worker.csproj

COPY src/ ./src/

RUN dotnet publish ./src/OrderFlow.Worker/OrderFlow.Worker.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrderFlow.Worker.dll"]
