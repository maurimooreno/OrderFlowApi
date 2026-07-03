using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Operations.Interfaces;

public interface IExternalOperationProcessor
{
    Task<bool> ProcessAsync(OrderOperation orderOperation, CancellationToken cancellationToken);
}
