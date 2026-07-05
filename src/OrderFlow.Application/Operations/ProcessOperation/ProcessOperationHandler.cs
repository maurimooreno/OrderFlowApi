using OrderFlow.Application.Operations.Interfaces;
using Microsoft.Extensions.Logging;

namespace OrderFlow.Application.Operations.ProcessOperation;

public class ProcessOperationHandler(
    IOrderOperationRepository orderOperationRepository,
    IExternalOperationProcessor externalOperationProcessor,
    ILogger<ProcessOperationHandler> logger)
{
    private readonly IOrderOperationRepository _orderOperationRepository = orderOperationRepository;
    private readonly IExternalOperationProcessor _externalOperationProcessor = externalOperationProcessor;
    private readonly ILogger<ProcessOperationHandler> _logger = logger;

    public async Task<bool> HandleAsync(Guid operationId, CancellationToken cancellationToken)
    {
        var orderOperation = await _orderOperationRepository.GetForUpdateByIdAsync(operationId, cancellationToken);

        if (orderOperation is null)
        {
            _logger.LogWarning(
                "Operation processing skipped because operation was not found: {OperationId}",
                operationId);

            return false;
        }

        orderOperation.MarkAsProcessing();
        await _orderOperationRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Operation status changed: {OperationId}, {Status}",
            orderOperation.Id,
            orderOperation.Status);

        var processedSuccessfully = await _externalOperationProcessor.ProcessAsync(orderOperation, cancellationToken);

        if (processedSuccessfully)
        {
            orderOperation.MarkAsCompleted();
        }
        else
        {
            orderOperation.MarkAsFailed("External operation processing failed.");
        }

        await _orderOperationRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Operation processing result: {OperationId}, {Status}",
            orderOperation.Id,
            orderOperation.Status);

        return true;
    }
}
