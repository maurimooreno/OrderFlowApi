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
            .Include(orderOperation => orderOperation.History)
            .FirstOrDefaultAsync(orderOperation => orderOperation.Id == id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        MarkAppendedHistoryAsAdded();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private void MarkAppendedHistoryAsAdded()
    {
        foreach (var entry in _dbContext.ChangeTracker.Entries<OrderOperationHistory>())
        {
            // History is append-only. EF discovers new Guid-keyed collection members as Modified.
            if (entry.State == EntityState.Modified)
                entry.State = EntityState.Added;
        }
    }
}
