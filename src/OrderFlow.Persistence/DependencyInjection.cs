using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Persistence.Repositories;

namespace OrderFlow.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<OrderFlowDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IOrderOperationRepository, OrderOperationRepository>();

        return services;
    }
}
