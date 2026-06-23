using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Persistence.Configurations;

public class OrderOperationConfiguration : IEntityTypeConfiguration<OrderOperation>
{
    public void Configure(EntityTypeBuilder<OrderOperation> builder)
    {
        builder.ToTable("OrderOperations");

        builder.HasKey(orderOperation => orderOperation.Id);

        builder.Property(orderOperation => orderOperation.ExternalReference)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(orderOperation => orderOperation.CustomerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(orderOperation => orderOperation.CustomerEmail)
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(orderOperation => orderOperation.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(orderOperation => orderOperation.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(orderOperation => orderOperation.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(orderOperation => orderOperation.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(orderOperation => orderOperation.LastError)
            .HasMaxLength(1000);

        builder.HasMany(orderOperation => orderOperation.History)
            .WithOne(history => history.OrderOperation)
            .HasForeignKey(history => history.OrderOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(orderOperation => orderOperation.History)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
