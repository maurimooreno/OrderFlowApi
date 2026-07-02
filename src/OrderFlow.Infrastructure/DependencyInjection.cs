using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Infrastructure.Messaging;

namespace OrderFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IOperationQueuePublisher, InMemoryQueuePublisher>();

        return services;
    }
}
