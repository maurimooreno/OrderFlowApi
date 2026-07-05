using OrderFlow.Application.Operations.Interfaces;

namespace OrderFlow.Application.Operations.GetOperationStatus;

public class GetOperationStatusHandler(IOrderOperationRepository orderOperationRepository)
{
    private readonly IOrderOperationRepository _orderOperationRepository = orderOperationRepository;

    public async Task<GetOperationStatusResult?> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var orderOperation = await _orderOperationRepository.GetByIdAsync(id, cancellationToken);

        if (orderOperation is null)
            return null;

        return new GetOperationStatusResult(
            orderOperation.Id,
            orderOperation.Status,
            orderOperation.UpdatedAtUtc,
            orderOperation.ProcessedAtUtc);
    }
}
