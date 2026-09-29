using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;
namespace MiniStore.Infrastructure.Persistence.Configurations;
public sealed class PurchaseReturnConfiguration : IEntityTypeConfiguration<PurchaseReturn>
{
    public void Configure(EntityTypeBuilder<PurchaseReturn> builder)
    {
        builder.HasKey(x => x.Id); builder.Property(x => x.ReturnNumber).HasMaxLength(50).IsRequired(); builder.HasIndex(x => x.ReturnNumber).IsUnique();
        builder.Property(x => x.Reason).HasMaxLength(500).IsRequired();
        builder.Property(x => x.PayableAmount).HasPrecision(18,2); builder.Property(x => x.DiscountAmount).HasPrecision(18,2); builder.Property(x => x.TaxAmount).HasPrecision(18,2);
        builder.Property(x => x.OriginalInventoryAmount).HasPrecision(18,2); builder.Property(x => x.RemovedInventoryCost).HasPrecision(24,8);
        builder.HasOne<Purchase>().WithMany().HasForeignKey(x => x.PurchaseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.PurchaseReturnId).OnDelete(DeleteBehavior.Cascade);
    }
}
public sealed class PurchaseReturnItemConfiguration : IEntityTypeConfiguration<PurchaseReturnItem>
{
    public void Configure(EntityTypeBuilder<PurchaseReturnItem> builder)
    {
        builder.HasKey(x => x.Id); builder.HasIndex(x => new { x.PurchaseReturnId, x.PurchaseItemId }).IsUnique();
        builder.Property(x => x.Quantity).HasPrecision(18,6); builder.Property(x => x.OriginalInventoryAmount).HasPrecision(18,2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18,2); builder.Property(x => x.TaxAmount).HasPrecision(18,2); builder.Property(x => x.PayableAmount).HasPrecision(18,2); builder.Property(x => x.RemovedInventoryCost).HasPrecision(24,8);
        builder.HasOne<PurchaseItem>().WithMany().HasForeignKey(x => x.PurchaseItemId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
    }
}
