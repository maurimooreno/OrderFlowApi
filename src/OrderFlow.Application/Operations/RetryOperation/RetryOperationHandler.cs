using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Operations.RetryOperation;

public class RetryOperationHandler(
    IOrderOperationRepository orderOperationRepository,
    IOperationQueuePublisher operationQueuePublisher)
{
    private readonly IOrderOperationRepository _orderOperationRepository = orderOperationRepository;
    private readonly IOperationQueuePublisher _operationQueuePublisher = operationQueuePublisher;

    public async Task<RetryOperationResult> HandleAsync(Guid operationId, CancellationToken cancellationToken)
    {
        var orderOperation = await _orderOperationRepository.GetForUpdateByIdAsync(operationId, cancellationToken);

        if (orderOperation is null)
            return new RetryOperationResult(RetryOperationStatus.NotFound);

        if (orderOperation.Status != OperationStatus.Failed)
            return new RetryOperationResult(RetryOperationStatus.InvalidStatus);

        orderOperation.Retry();
        await _orderOperationRepository.SaveChangesAsync(cancellationToken);
        await _operationQueuePublisher.PublishAsync(orderOperation.Id, cancellationToken);

        return new RetryOperationResult(
            RetryOperationStatus.Succeeded,
            orderOperation.Id,
            orderOperation.Status,
            orderOperation.RetryCount,
            orderOperation.UpdatedAtUtc);
    }
}
