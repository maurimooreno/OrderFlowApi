using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Persistence.Repositories;

public class OrderOperationRepository(OrderFlowDbContext dbContext) : IOrderOperationRepository
{
    private readonly OrderFlowDbContext _dbContext = dbContext;

    public async Task AddAsync(OrderOperation orderOperation, CancellationToken cancellationToken)
    {
        await _dbContext.OrderOperations.AddAsync(orderOperation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<OrderOperation?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.OrderOperations
            .AsNoTracking()
            .FirstOrDefaultAsync(orderOperation => orderOperation.Id == id, cancellationToken);
    }

    public async Task<OrderOperation?> GetForUpdateByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.OrderOperations
            .FirstOrDefaultAsync(orderOperation => orderOperation.Id == id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
