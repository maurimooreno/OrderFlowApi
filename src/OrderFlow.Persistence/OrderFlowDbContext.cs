using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Persistence;

public class OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options) : DbContext(options)
{
    public DbSet<OrderOperation> OrderOperations => Set<OrderOperation>();

    public DbSet<OrderOperationHistory> OrderOperationHistory => Set<OrderOperationHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderFlowDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
