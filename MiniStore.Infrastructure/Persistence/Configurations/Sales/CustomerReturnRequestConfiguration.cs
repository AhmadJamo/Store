using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class CustomerReturnRequestConfiguration : IEntityTypeConfiguration<CustomerReturnRequest>, IEntityTypeConfiguration<CustomerReturnRequestLine>, IEntityTypeConfiguration<CustomerReturnRequestHistory>
{
    public void Configure(EntityTypeBuilder<CustomerReturnRequest> b)
    {
        b.ToTable("CustomerReturnRequests"); b.HasKey(x => x.Id); b.Property(x => x.ReturnNumber).HasMaxLength(50).IsRequired(); b.HasIndex(x => x.ReturnNumber).IsUnique();
        b.Property(x => x.OrderNumberSnapshot).HasMaxLength(50).IsRequired(); b.Property(x => x.CustomerNameSnapshot).HasMaxLength(200).IsRequired();
        b.Property(x => x.Reason).HasMaxLength(500).IsRequired(); b.Property(x => x.Notes).HasMaxLength(1000); b.Property(x => x.DecisionReason).HasMaxLength(500); b.Property(x => x.CancellationReason).HasMaxLength(500);
        b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.SubmittedByUserId).HasMaxLength(450); b.Property(x => x.DecidedByUserId).HasMaxLength(450); b.Property(x => x.CancelledByUserId).HasMaxLength(450);
        b.Property(x => x.RowVersion).IsRowVersion(); b.Ignore(x => x.TotalRequestedQuantity); b.HasIndex(x => new { x.SalesDeliveryId, x.Status });
        b.HasOne<SalesDelivery>().WithMany().HasForeignKey(x => x.SalesDeliveryId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<SalesOrder>().WithMany().HasForeignKey(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.CustomerReturnRequestId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.History).WithOne().HasForeignKey(x => x.CustomerReturnRequestId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<CustomerReturnRequestLine> b)
    {
        b.ToTable("CustomerReturnRequestLines", t => t.HasCheckConstraint("CK_CustomerReturnRequestLines_Quantity", "[StockQuantity] > 0")); b.HasKey(x => x.Id);
        b.Property(x => x.StockQuantity).HasPrecision(18, 6); b.Property(x => x.UnitCostSnapshot).HasPrecision(18, 8); b.Property(x => x.CostOfGoodsSoldSnapshot).HasPrecision(18, 8);
        b.Property(x => x.ProductCodeSnapshot).HasMaxLength(100).IsRequired(); b.Property(x => x.ProductNameSnapshot).HasMaxLength(200).IsRequired(); b.Property(x => x.TrackingAllocationsSnapshot).HasMaxLength(4000);
        b.HasIndex(x => new { x.CustomerReturnRequestId, x.SalesDeliveryLineId }).IsUnique();
        b.HasOne<SalesDeliveryLine>().WithMany().HasForeignKey(x => x.SalesDeliveryLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<SalesOrderLine>().WithMany().HasForeignKey(x => x.SalesOrderLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<StorageLocation>().WithMany().HasForeignKey(x => x.StorageLocationId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<CustomerReturnRequestHistory> b)
    {
        b.ToTable("CustomerReturnRequestHistories"); b.HasKey(x => x.Id); b.Property(x => x.UserId).HasMaxLength(450).IsRequired(); b.Property(x => x.Reason).HasMaxLength(500); b.HasIndex(x => new { x.CustomerReturnRequestId, x.CreatedAtUtc });
    }
}
