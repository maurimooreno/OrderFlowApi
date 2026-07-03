using OrderFlow.Application.Operations.ProcessOperation;
using OrderFlow.Infrastructure;
using OrderFlow.Persistence;
using OrderFlow.Worker;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OrderFlowDb")
    ?? throw new InvalidOperationException("Connection string 'OrderFlowDb' was not found.");

builder.Services.AddPersistence(connectionString);
builder.Services.AddInfrastructure();
builder.Services.AddScoped<ProcessOperationHandler>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
