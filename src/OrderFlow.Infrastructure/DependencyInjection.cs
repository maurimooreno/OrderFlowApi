using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Infrastructure.ExternalServices;
using OrderFlow.Infrastructure.Messaging;

namespace OrderFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var serviceBusOptions = configuration
            .GetSection(ServiceBusOptions.SectionName)
            .Get<ServiceBusOptions>()
            ?? throw new InvalidOperationException("ServiceBus configuration section is required.");

        serviceBusOptions.Validate();

        services.AddSingleton(serviceBusOptions);

        if (serviceBusOptions.Provider == ServiceBusOptions.InMemoryProvider)
        {
            services.AddSingleton<InMemoryQueuePublisher>();
            services.AddSingleton<IOperationQueuePublisher>(serviceProvider =>
                serviceProvider.GetRequiredService<InMemoryQueuePublisher>());
            services.AddSingleton<IOperationQueueConsumer>(serviceProvider =>
                serviceProvider.GetRequiredService<InMemoryQueuePublisher>());
        }
        else
        {
            services.AddSingleton<AzureServiceBusQueuePublisher>();
            services.AddSingleton<AzureServiceBusQueueConsumer>();
            services.AddSingleton<IOperationQueuePublisher>(serviceProvider =>
                serviceProvider.GetRequiredService<AzureServiceBusQueuePublisher>());
            services.AddSingleton<IOperationQueueConsumer>(serviceProvider =>
                serviceProvider.GetRequiredService<AzureServiceBusQueueConsumer>());
        }

        services.AddScoped<IExternalOperationProcessor, SimulatedExternalOperationProcessor>();

        return services;
    }
}
