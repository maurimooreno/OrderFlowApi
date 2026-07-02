using System.Collections.Concurrent;
using OrderFlow.Application.Operations.Interfaces;

namespace OrderFlow.Infrastructure.Messaging;

public class InMemoryQueuePublisher : IOperationQueuePublisher
{
    private readonly ConcurrentQueue<Guid> _operationIds = new();

    public Task PublishAsync(Guid operationId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _operationIds.Enqueue(operationId);

        return Task.CompletedTask;
    }
}
