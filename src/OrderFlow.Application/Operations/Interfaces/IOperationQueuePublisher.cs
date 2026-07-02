namespace OrderFlow.Application.Operations.Interfaces;

public interface IOperationQueuePublisher
{
    Task PublishAsync(Guid operationId, CancellationToken cancellationToken);
}
