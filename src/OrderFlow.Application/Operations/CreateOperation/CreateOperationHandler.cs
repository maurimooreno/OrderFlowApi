using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Application.Operations.Requests;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Operations.CreateOperation;

public class CreateOperationHandler(
    IOrderOperationRepository orderOperationRepository,
    IOperationQueuePublisher operationQueuePublisher)
{
    private readonly IOrderOperationRepository _orderOperationRepository = orderOperationRepository;
    private readonly IOperationQueuePublisher _operationQueuePublisher = operationQueuePublisher;

    public async Task<CreateOperationResult> HandleAsync(
        CreateOperationRequest request,
        CancellationToken cancellationToken)
    {
        var orderOperation = new OrderOperation(
            request.ExternalReference,
            request.CustomerName,
            request.CustomerEmail,
            request.TotalAmount,
            request.Currency);

        await _orderOperationRepository.AddAsync(orderOperation, cancellationToken);
        await _operationQueuePublisher.PublishAsync(orderOperation.Id, cancellationToken);

        return new CreateOperationResult(
            orderOperation.Id,
            orderOperation.Status,
            orderOperation.CreatedAtUtc);
    }
}