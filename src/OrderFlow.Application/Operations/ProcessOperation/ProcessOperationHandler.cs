using OrderFlow.Application.Operations.Interfaces;

namespace OrderFlow.Application.Operations.ProcessOperation;

public class ProcessOperationHandler(
    IOrderOperationRepository orderOperationRepository,
    IExternalOperationProcessor externalOperationProcessor)
{
    private readonly IOrderOperationRepository _orderOperationRepository = orderOperationRepository;
    private readonly IExternalOperationProcessor _externalOperationProcessor = externalOperationProcessor;

    public async Task<bool> HandleAsync(Guid operationId, CancellationToken cancellationToken)
    {
        var orderOperation = await _orderOperationRepository.GetForUpdateByIdAsync(operationId, cancellationToken);

        if (orderOperation is null)
            return false;

        orderOperation.MarkAsProcessing();
        await _orderOperationRepository.SaveChangesAsync(cancellationToken);

        var processedSuccessfully = await _externalOperationProcessor.ProcessAsync(orderOperation, cancellationToken);

        if (processedSuccessfully)
            orderOperation.MarkAsCompleted();
        else
            orderOperation.MarkAsFailed("External operation processing failed.");

        await _orderOperationRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
