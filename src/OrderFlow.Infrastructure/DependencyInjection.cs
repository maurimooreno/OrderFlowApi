using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Infrastructure.ExternalServices;
using OrderFlow.Infrastructure.Messaging;

namespace OrderFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryQueuePublisher>();
        services.AddSingleton<IOperationQueuePublisher>(serviceProvider =>
            serviceProvider.GetRequiredService<InMemoryQueuePublisher>());
        services.AddSingleton<IOperationQueueConsumer>(serviceProvider =>
            serviceProvider.GetRequiredService<InMemoryQueuePublisher>());
        services.AddScoped<IExternalOperationProcessor, SimulatedExternalOperationProcessor>();

        return services;
    }
}
