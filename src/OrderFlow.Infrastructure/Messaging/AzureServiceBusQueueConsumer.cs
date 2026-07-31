using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Operations.Interfaces;

namespace OrderFlow.Infrastructure.Messaging;

public sealed class AzureServiceBusQueueConsumer : IOperationQueueConsumer, IAsyncDisposable
{
    private static readonly TimeSpan ReceiveTimeout = TimeSpan.FromSeconds(1);

    private readonly ServiceBusClient _client;
    private readonly ServiceBusReceiver _receiver;
    private readonly ILogger<AzureServiceBusQueueConsumer> _logger;

    public AzureServiceBusQueueConsumer(
        ServiceBusOptions options,
        ILogger<AzureServiceBusQueueConsumer> logger)
    {
        _client = new ServiceBusClient(options.ConnectionString);
        _receiver = _client.CreateReceiver(options.QueueName);
        _logger = logger;
    }

    public async Task<IOperationQueueMessage?> DequeueAsync(CancellationToken cancellationToken)
    {
        var receivedMessage = await _receiver.ReceiveMessageAsync(
            ReceiveTimeout,
            cancellationToken);

        if (receivedMessage is null)
            return null;

        if (!TryGetOperationId(receivedMessage, out var operationId))
        {
            await _receiver.DeadLetterMessageAsync(
                receivedMessage,
                "InvalidOperationId",
                "The message does not contain a valid OperationId.",
                cancellationToken);

            _logger.LogWarning(
                "Azure Service Bus message dead-lettered because OperationId is invalid: {MessageId}",
                receivedMessage.MessageId);

            return null;
        }

        _logger.LogInformation(
            "Operation message consumed from Azure Service Bus: {OperationId}",
            operationId);

        return new AzureServiceBusOperationQueueMessage(
            operationId,
            receivedMessage,
            _receiver,
            _logger);
    }

    public async ValueTask DisposeAsync()
    {
        await _receiver.DisposeAsync();
        await _client.DisposeAsync();
    }

    private static bool TryGetOperationId(
        ServiceBusReceivedMessage message,
        out Guid operationId)
    {
        if (message.ApplicationProperties.TryGetValue("OperationId", out var operationIdProperty)
            && Guid.TryParse(operationIdProperty?.ToString(), out operationId))
        {
            return true;
        }

        return Guid.TryParse(message.Body.ToString(), out operationId);
    }

    private sealed class AzureServiceBusOperationQueueMessage(
        Guid operationId,
        ServiceBusReceivedMessage message,
        ServiceBusReceiver receiver,
        ILogger logger) : IOperationQueueMessage
    {
        private int _settled;
        private int _settling;

        public Guid OperationId { get; } = operationId;

        public async Task CompleteAsync(CancellationToken cancellationToken)
        {
            if (!TryStartSettlement())
                return;

            try
            {
                await receiver.CompleteMessageAsync(message, cancellationToken);
                MarkAsSettled();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Operation message completion failed in Azure Service Bus: {OperationId}", OperationId);
                throw;
            }
            finally
            {
                Volatile.Write(ref _settling, 0);
            }

            logger.LogInformation(
                "Operation message completed in Azure Service Bus: {OperationId}",
                OperationId);
        }

        public async Task AbandonAsync(CancellationToken cancellationToken)
        {
            if (!TryStartSettlement())
                return;

            try
            {
                await receiver.AbandonMessageAsync(
                    message,
                    cancellationToken: cancellationToken);
                MarkAsSettled();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Operation message abandon failed in Azure Service Bus: {OperationId}", OperationId);
                throw;
            }
            finally
            {
                Volatile.Write(ref _settling, 0);
            }

            logger.LogWarning(
                "Operation message abandoned in Azure Service Bus: {OperationId}",
                OperationId);
        }

        public async Task DeadLetterAsync(string reason, CancellationToken cancellationToken)
        {
            if (!TryStartSettlement())
                return;

            try
            {
                await receiver.DeadLetterMessageAsync(
                    message,
                    reason,
                    cancellationToken: cancellationToken);
                MarkAsSettled();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Operation message dead-lettering failed in Azure Service Bus: {OperationId} {Reason}", OperationId, reason);
                throw;
            }
            finally
            {
                Volatile.Write(ref _settling, 0);
            }

            logger.LogWarning(
                "Operation message sent to Azure Service Bus dead-letter queue: {OperationId} {Reason}",
                OperationId,
                reason);
        }

        private bool TryStartSettlement()
        {
            return Volatile.Read(ref _settled) == 0
                && Interlocked.CompareExchange(ref _settling, 1, 0) == 0;
        }

        private void MarkAsSettled()
        {
            Volatile.Write(ref _settled, 1);
        }
    }
}
