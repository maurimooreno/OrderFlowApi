using System.Collections.Concurrent;
using OrderFlow.Application.Operations.Interfaces;

namespace OrderFlow.Infrastructure.Messaging;

public class InMemoryQueuePublisher : IOperationQueuePublisher, IOperationQueueConsumer
{
    private readonly ConcurrentQueue<Guid> _operationIds = new();

    public Task PublishAsync(Guid operationId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _operationIds.Enqueue(operationId);

        return Task.CompletedTask;
    }

    public Task<Guid?> DequeueAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(
            _operationIds.TryDequeue(out var operationId)
                ? operationId
                : (Guid?)null);
    }
}
