using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace OrderFlow.Application.Operations.RetryOperation;

public class RetryOperationHandler(
    IOrderOperationRepository orderOperationRepository,
    IOperationQueuePublisher operationQueuePublisher,
    ILogger<RetryOperationHandler> logger)
{
    private readonly IOrderOperationRepository _orderOperationRepository = orderOperationRepository;
    private readonly IOperationQueuePublisher _operationQueuePublisher = operationQueuePublisher;
    private readonly ILogger<RetryOperationHandler> _logger = logger;

    public async Task<RetryOperationResult> HandleAsync(Guid operationId, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Operation retry requested: {OperationId}",
            operationId);

        var orderOperation = await _orderOperationRepository.GetForUpdateByIdAsync(operationId, cancellationToken);

        if (orderOperation is null)
        {
            _logger.LogWarning(
                "Operation retry rejected because operation was not found: {OperationId}",
                operationId);

            return new RetryOperationResult(RetryOperationStatus.NotFound);
        }

        if (orderOperation.Status != OperationStatus.Failed)
        {
            _logger.LogWarning(
                "Operation retry rejected because status is invalid: {OperationId}, {Status}",
                orderOperation.Id,
                orderOperation.Status);

            return new RetryOperationResult(RetryOperationStatus.InvalidStatus);
        }

        orderOperation.Retry();
        await _orderOperationRepository.SaveChangesAsync(cancellationToken);
        await _operationQueuePublisher.PublishAsync(orderOperation.Id, cancellationToken);

        _logger.LogInformation(
            "Operation retry accepted and republished: {OperationId}, {RetryCount}, {Status}",
            orderOperation.Id,
            orderOperation.RetryCount,
            orderOperation.Status);

        return new RetryOperationResult(
            RetryOperationStatus.Succeeded,
            orderOperation.Id,
            orderOperation.Status,
            orderOperation.RetryCount,
            orderOperation.UpdatedAtUtc);
    }
}
