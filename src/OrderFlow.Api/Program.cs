using OrderFlow.Application.Operations.CreateOperation;
using OrderFlow.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<CreateOperationHandler>();

var connectionString = builder.Configuration.GetConnectionString("OrderFlowDb")
    ?? throw new InvalidOperationException("Connection string 'OrderFlowDb' was not found.");

builder.Services.AddPersistence(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
