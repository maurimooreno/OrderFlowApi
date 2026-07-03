using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.ExternalServices;

public class SimulatedExternalOperationProcessor : IExternalOperationProcessor
{
    public async Task<bool> ProcessAsync(OrderOperation orderOperation, CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken);

        return Random.Shared.Next(0, 100) >= 20;
    }
}
