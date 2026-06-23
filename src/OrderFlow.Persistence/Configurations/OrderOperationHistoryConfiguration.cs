using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Persistence.Configurations;

public class OrderOperationHistoryConfiguration : IEntityTypeConfiguration<OrderOperationHistory>
{
    public void Configure(EntityTypeBuilder<OrderOperationHistory> builder)
    {
        builder.ToTable("OrderOperationHistory");

        builder.HasKey(history => history.Id);

        builder.Property(history => history.OrderOperationId)
            .IsRequired();

        builder.Property(history => history.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(history => history.Message)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(history => history.CreatedAtUtc)
            .IsRequired();
    }
}
