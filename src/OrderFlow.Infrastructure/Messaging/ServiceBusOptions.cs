namespace OrderFlow.Infrastructure.Messaging;

public sealed class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    public const string InMemoryProvider = "InMemory";

    public const string AzureServiceBusProvider = "AzureServiceBus";

    public string Provider { get; init; } = string.Empty;

    public string ConnectionString { get; init; } = string.Empty;

    public string QueueName { get; init; } = string.Empty;

    public void Validate()
    {
        if (Provider is not InMemoryProvider and not AzureServiceBusProvider)
        {
            throw new InvalidOperationException(
                $"ServiceBus:Provider must be '{InMemoryProvider}' or '{AzureServiceBusProvider}'.");
        }

        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            throw new InvalidOperationException("ServiceBus:ConnectionString configuration is required.");
        }

        if (string.IsNullOrWhiteSpace(QueueName))
        {
            throw new InvalidOperationException("ServiceBus:QueueName configuration is required.");
        }
    }
}
