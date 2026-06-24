using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OrderFlow.Persistence;

#nullable disable

namespace OrderFlow.Persistence.Migrations;

[DbContext(typeof(OrderFlowDbContext))]
[Migration("20260618000000_InitialCreate")]
partial class InitialCreate
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.9")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        modelBuilder.Entity("OrderFlow.Domain.Entities.OrderOperation", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime>("CreatedAtUtc")
                    .HasColumnType("datetime2");

                b.Property<string>("Currency")
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnType("nvarchar(3)");

                b.Property<string>("CustomerEmail")
                    .IsRequired()
                    .HasMaxLength(254)
                    .HasColumnType("nvarchar(254)");

                b.Property<string>("CustomerName")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                b.Property<string>("ExternalReference")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.Property<string>("LastError")
                    .HasMaxLength(1000)
                    .HasColumnType("nvarchar(1000)");

                b.Property<DateTime?>("ProcessedAtUtc")
                    .HasColumnType("datetime2");

                b.Property<int>("RetryCount")
                    .HasColumnType("int");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.Property<decimal>("TotalAmount")
                    .HasPrecision(18, 2)
                    .HasColumnType("decimal(18,2)");

                b.Property<string>("Type")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.Property<DateTime?>("UpdatedAtUtc")
                    .HasColumnType("datetime2");

                b.HasKey("Id");

                b.ToTable("OrderOperations", (string)null);
            });

        modelBuilder.Entity("OrderFlow.Domain.Entities.OrderOperationHistory", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime>("CreatedAtUtc")
                    .HasColumnType("datetime2");

                b.Property<string>("Message")
                    .IsRequired()
                    .HasMaxLength(1000)
                    .HasColumnType("nvarchar(1000)");

                b.Property<Guid>("OrderOperationId")
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.HasKey("Id");

                b.HasIndex("OrderOperationId");

                b.ToTable("OrderOperationHistory", (string)null);
            });

        modelBuilder.Entity("OrderFlow.Domain.Entities.OrderOperationHistory", b =>
            {
                b.HasOne("OrderFlow.Domain.Entities.OrderOperation", "OrderOperation")
                    .WithMany("History")
                    .HasForeignKey("OrderOperationId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("OrderOperation");
            });

        modelBuilder.Entity("OrderFlow.Domain.Entities.OrderOperation", b =>
            {
                b.Navigation("History")
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
            });
#pragma warning restore 612, 618
    }
}
