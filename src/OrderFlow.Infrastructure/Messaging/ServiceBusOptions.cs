namespace OrderFlow.Infrastructure.Messaging;

public sealed class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    public string ConnectionString { get; init; } = string.Empty;

    public string QueueName { get; init; } = string.Empty;

    public void Validate()
    {
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
