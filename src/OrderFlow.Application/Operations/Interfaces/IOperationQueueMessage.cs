namespace OrderFlow.Application.Operations.Interfaces;

public interface IOperationQueueMessage
{
    Guid OperationId { get; }

    Task CompleteAsync(CancellationToken cancellationToken);

    Task AbandonAsync(CancellationToken cancellationToken);

    Task DeadLetterAsync(string reason, CancellationToken cancellationToken);
}
