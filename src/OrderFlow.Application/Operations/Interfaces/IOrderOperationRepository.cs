using OrderFlow.Domain.Entities;

namespace OrderFlow.Application.Operations.Interfaces;

public interface IOrderOperationRepository
{
    Task AddAsync(OrderOperation orderOperation, CancellationToken cancellationToken);

    Task<OrderOperation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<OrderOperation?> GetForUpdateByIdAsync(Guid id, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
