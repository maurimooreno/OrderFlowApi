using OrderFlow.Application.Operations.CreateOperation;
using OrderFlow.Application.Operations.GetOperation;
using OrderFlow.Application.Operations.GetOperationStatus;
using OrderFlow.Application.Operations.RetryOperation;
using OrderFlow.Infrastructure;
using OrderFlow.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<CreateOperationHandler>();
builder.Services.AddScoped<GetOperationHandler>();
builder.Services.AddScoped<GetOperationStatusHandler>();
builder.Services.AddScoped<RetryOperationHandler>();

var connectionString = builder.Configuration.GetConnectionString("OrderFlowDb")
    ?? throw new InvalidOperationException("Connection string 'OrderFlowDb' was not found.");

builder.Services.AddPersistence(connectionString);
builder.Services.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
