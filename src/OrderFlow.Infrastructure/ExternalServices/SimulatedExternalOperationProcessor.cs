using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace OrderFlow.Infrastructure.ExternalServices;

public class SimulatedExternalOperationProcessor(ILogger<SimulatedExternalOperationProcessor> logger) : IExternalOperationProcessor
{
    private readonly ILogger<SimulatedExternalOperationProcessor> _logger = logger;

    public async Task<bool> ProcessAsync(OrderOperation orderOperation, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Simulated external processing started: {OperationId}",
            orderOperation.Id);

        await Task.Delay(500, cancellationToken);

        var processedSuccessfully = Random.Shared.Next(0, 100) >= 20;

        _logger.LogInformation(
            "Simulated external processing finished: {OperationId}, {Succeeded}",
            orderOperation.Id,
            processedSuccessfully);

        return processedSuccessfully;
    }
}
