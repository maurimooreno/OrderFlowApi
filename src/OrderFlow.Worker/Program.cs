using OrderFlow.Infrastructure;
using OrderFlow.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
