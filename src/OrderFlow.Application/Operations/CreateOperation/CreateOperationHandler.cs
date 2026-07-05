using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Application.Operations.Requests;
using OrderFlow.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace OrderFlow.Application.Operations.CreateOperation;

public class CreateOperationHandler(
    IOrderOperationRepository orderOperationRepository,
    IOperationQueuePublisher operationQueuePublisher,
    ILogger<CreateOperationHandler> logger)
{
    private readonly IOrderOperationRepository _orderOperationRepository = orderOperationRepository;
    private readonly IOperationQueuePublisher _operationQueuePublisher = operationQueuePublisher;
    private readonly ILogger<CreateOperationHandler> _logger = logger;

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

        _logger.LogInformation(
            "Operation created: {OperationId}, {Status}",
            orderOperation.Id,
            orderOperation.Status);

        await _operationQueuePublisher.PublishAsync(orderOperation.Id, cancellationToken);

        _logger.LogInformation(
            "Operation published for processing: {OperationId}",
            orderOperation.Id);

        return new CreateOperationResult(
            orderOperation.Id,
            orderOperation.Status,
            orderOperation.CreatedAtUtc);
    }
}
