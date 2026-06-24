using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Persistence.Repositories;

public class OrderOperationRepository(OrderFlowDbContext dbContext) : IOrderOperationRepository
{
    private readonly OrderFlowDbContext _dbContext = dbContext;

    public async Task AddAsync(OrderOperation orderOperation, CancellationToken cancellationToken)
    {
        await _dbContext.OrderOperations.AddAsync(orderOperation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
