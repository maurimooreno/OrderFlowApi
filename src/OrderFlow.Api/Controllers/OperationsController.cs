using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Operations.CreateOperation;
using OrderFlow.Application.Operations.Requests;
using OrderFlow.Application.Operations.Responses;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/operations")]
public class OperationsController(CreateOperationHandler createOperationHandler) : ControllerBase
{
    private readonly CreateOperationHandler _createOperationHandler = createOperationHandler;

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
}
