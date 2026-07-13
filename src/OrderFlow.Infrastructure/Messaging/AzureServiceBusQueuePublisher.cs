using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Operations.Interfaces;

namespace OrderFlow.Infrastructure.Messaging;

public sealed class AzureServiceBusQueuePublisher : IOperationQueuePublisher, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly ILogger<AzureServiceBusQueuePublisher> _logger;

    public AzureServiceBusQueuePublisher(
        ServiceBusOptions options,
        ILogger<AzureServiceBusQueuePublisher> logger)
    {
        _client = new ServiceBusClient(options.ConnectionString);
        _sender = _client.CreateSender(options.QueueName);
        _logger = logger;
    }

    public async Task PublishAsync(Guid operationId, CancellationToken cancellationToken)
    {
        var operationIdValue = operationId.ToString("D");
        var message = new ServiceBusMessage(operationIdValue)
        {
            MessageId = operationIdValue,
            ContentType = "text/plain",
            Subject = "OrderOperationCreated"
        };

        message.ApplicationProperties["OperationId"] = operationIdValue;

        await _sender.SendMessageAsync(message, cancellationToken);

        _logger.LogInformation(
            "Operation message published to Azure Service Bus: {OperationId}",
            operationId);
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}
