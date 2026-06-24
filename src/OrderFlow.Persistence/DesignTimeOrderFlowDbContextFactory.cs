using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OrderFlow.Persistence;

public class DesignTimeOrderFlowDbContextFactory : IDesignTimeDbContextFactory<OrderFlowDbContext>
{
    public OrderFlowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderFlowDbContext>();
        var connectionString = GetConnectionString();

        optionsBuilder.UseSqlServer(connectionString);

        return new OrderFlowDbContext(optionsBuilder.Options);
    }

    private static string GetConnectionString()
    {
        const string connectionStringName = "OrderFlowDb";

        var apiProjectPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "OrderFlow.Api"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var connectionString = configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Connection string '{connectionStringName}' was not found.");

        return connectionString;
    }
}
