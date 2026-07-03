namespace OrderFlow.Application.Operations.Interfaces;

public interface IOperationQueueConsumer
{
    Task<Guid?> DequeueAsync(CancellationToken cancellationToken);
}
