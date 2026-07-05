using System.Collections.Concurrent;
using OrderFlow.Application.Operations.Interfaces;
using Microsoft.Extensions.Logging;

namespace OrderFlow.Infrastructure.Messaging;

public class InMemoryQueuePublisher(ILogger<InMemoryQueuePublisher> logger) : IOperationQueuePublisher, IOperationQueueConsumer
{
    private readonly ConcurrentQueue<Guid> _operationIds = new();
    private readonly ILogger<InMemoryQueuePublisher> _logger = logger;

    public Task PublishAsync(Guid operationId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _operationIds.Enqueue(operationId);

        _logger.LogInformation(
            "Operation message published to in-memory queue: {OperationId}",
            operationId);

        return Task.CompletedTask;
    }

    public Task<Guid?> DequeueAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_operationIds.TryDequeue(out var operationId))
            return Task.FromResult<Guid?>(null);

        _logger.LogInformation(
            "Operation message consumed from in-memory queue: {OperationId}",
            operationId);

        return Task.FromResult<Guid?>(operationId);
    }
}
