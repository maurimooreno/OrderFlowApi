namespace OrderFlow.Application.Operations.Interfaces;

public interface IOperationQueueConsumer
{
    Task<IOperationQueueMessage?> DequeueAsync(CancellationToken cancellationToken);
}
