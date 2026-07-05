using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Operations.CreateOperation;
using OrderFlow.Application.Operations.GetOperation;
using OrderFlow.Application.Operations.GetOperationStatus;
using OrderFlow.Application.Operations.RetryOperation;
using OrderFlow.Application.Operations.Requests;
using OrderFlow.Application.Operations.Responses;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/operations")]
public class OperationsController(
    CreateOperationHandler createOperationHandler,
    GetOperationHandler getOperationHandler,
    GetOperationStatusHandler getOperationStatusHandler,
    RetryOperationHandler retryOperationHandler) : ControllerBase
{
    private readonly CreateOperationHandler _createOperationHandler = createOperationHandler;
    private readonly GetOperationHandler _getOperationHandler = getOperationHandler;
    private readonly GetOperationStatusHandler _getOperationStatusHandler = getOperationStatusHandler;
    private readonly RetryOperationHandler _retryOperationHandler = retryOperationHandler;

    [HttpPost]
    [ProducesResponseType(typeof(CreateOperationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateOperationResponse>> CreateAsync(
        CreateOperationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _createOperationHandler.HandleAsync(request, cancellationToken);

        var response = new CreateOperationResponse(
            result.Id,
            result.Status,
            result.CreatedAtUtc);

        return Created($"/api/operations/{response.Id}", response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getOperationHandler.HandleAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        var response = new OperationResponse(
            result.Id,
            result.ExternalReference,
            result.CustomerName,
            result.CustomerEmail,
            result.TotalAmount,
            result.Currency,
            result.Type,
            result.Status,
            result.RetryCount,
            result.LastError,
            result.CreatedAtUtc,
            result.UpdatedAtUtc,
            result.ProcessedAtUtc);

        return Ok(response);
    }

    [HttpGet("{id:guid}/status")]
    [ProducesResponseType(typeof(OperationStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationStatusResponse>> GetStatusByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _getOperationStatusHandler.HandleAsync(id, cancellationToken);

        if (result is null)
            return NotFound();

        return Ok(new OperationStatusResponse(
            result.Id,
            result.Status,
            result.UpdatedAtUtc,
            result.ProcessedAtUtc));
    }

    [HttpPost("{id:guid}/retry")]
    [ProducesResponseType(typeof(OperationStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OperationStatusResponse>> RetryAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _retryOperationHandler.HandleAsync(id, cancellationToken);

        return result.ResultStatus switch
        {
            RetryOperationStatus.NotFound => NotFound(),
            RetryOperationStatus.InvalidStatus => Conflict(),
            _ => Ok(new OperationStatusResponse(
                result.Id!.Value,
                result.Status!.Value,
                result.UpdatedAtUtc,
                null))
        };
    }
}
