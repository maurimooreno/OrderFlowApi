using System.Collections.Concurrent;
using OrderFlow.Application.Operations.Interfaces;
using Microsoft.Extensions.Logging;

namespace OrderFlow.Infrastructure.Messaging;

public class InMemoryQueuePublisher(ILogger<InMemoryQueuePublisher> logger) : IOperationQueuePublisher, IOperationQueueConsumer
{
    private readonly ConcurrentQueue<Guid> _operationIds = new();
    private readonly ConcurrentQueue<Guid> _deadLetterOperationIds = new();
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

    public Task<IOperationQueueMessage?> DequeueAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_operationIds.TryDequeue(out var operationId))
            return Task.FromResult<IOperationQueueMessage?>(null);

        _logger.LogInformation(
            "Operation message consumed from in-memory queue: {OperationId}",
            operationId);

        return Task.FromResult<IOperationQueueMessage?>(
            new InMemoryOperationQueueMessage(
                operationId,
                _operationIds,
                _deadLetterOperationIds,
                _logger));
    }

    private sealed class InMemoryOperationQueueMessage(
        Guid operationId,
        ConcurrentQueue<Guid> operationIds,
        ConcurrentQueue<Guid> deadLetterOperationIds,
        ILogger logger) : IOperationQueueMessage
    {
        private int _settled;

        public Guid OperationId { get; } = operationId;

        public Task CompleteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (Interlocked.Exchange(ref _settled, 1) == 1)
                return Task.CompletedTask;

            logger.LogInformation(
                "Operation message completed in in-memory queue: {OperationId}",
                OperationId);

            return Task.CompletedTask;
        }

        public Task AbandonAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (Interlocked.Exchange(ref _settled, 1) == 1)
                return Task.CompletedTask;

            operationIds.Enqueue(OperationId);

            logger.LogWarning(
                "Operation message abandoned and requeued in in-memory queue: {OperationId}",
                OperationId);

            return Task.CompletedTask;
        }

        public Task DeadLetterAsync(string reason, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (Interlocked.Exchange(ref _settled, 1) == 1)
                return Task.CompletedTask;

            deadLetterOperationIds.Enqueue(OperationId);

            logger.LogWarning(
                "Operation message sent to in-memory dead-letter queue: {OperationId} {Reason}",
                OperationId,
                reason);

            return Task.CompletedTask;
        }
    }
}
