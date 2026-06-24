using OrderFlow.Application.Operations.Interfaces;

namespace OrderFlow.Application.Operations.GetOperation;

public class GetOperationHandler(IOrderOperationRepository orderOperationRepository)
{
    private readonly IOrderOperationRepository _orderOperationRepository = orderOperationRepository;

    public async Task<GetOperationResult?> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var orderOperation = await _orderOperationRepository.GetByIdAsync(id, cancellationToken);

        if (orderOperation is null)
            return null;

        return new GetOperationResult(
            orderOperation.Id,
            orderOperation.ExternalReference,
            orderOperation.CustomerName,
            orderOperation.CustomerEmail,
            orderOperation.TotalAmount,
            orderOperation.Currency,
            orderOperation.Type,
            orderOperation.Status,
            orderOperation.RetryCount,
            orderOperation.LastError,
            orderOperation.CreatedAtUtc,
            orderOperation.UpdatedAtUtc,
            orderOperation.ProcessedAtUtc);
    }
}
