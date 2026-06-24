using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderFlow.Persistence;

public class DesignTimeOrderFlowDbContextFactory : IDesignTimeDbContextFactory<OrderFlowDbContext>
{
    public OrderFlowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderFlowDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=DESKTOP-QVF2JDD;Database=OrderFlowDb;Trusted_Connection=True;TrustServerCertificate=True;");

        return new OrderFlowDbContext(optionsBuilder.Options);
    }
}
