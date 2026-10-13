using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class PurchaseServiceAcceptanceConfiguration : IEntityTypeConfiguration<PurchaseServiceAcceptance>, IEntityTypeConfiguration<PurchaseServiceAcceptanceLine>
{
    public void Configure(EntityTypeBuilder<PurchaseServiceAcceptance> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.AcceptanceNumber).HasMaxLength(50).IsRequired();
        b.Property(x => x.Notes).HasMaxLength(1000); b.Property(x => x.CompletedByUserId).HasMaxLength(450).IsRequired();
        b.HasIndex(x => x.AcceptanceNumber).IsUnique();
        b.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.PurchaseServiceAcceptanceId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<PurchaseServiceAcceptanceLine> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.CompletedQuantity).HasPrecision(18, 6);
        b.Property(x => x.ProductCodeSnapshot).HasMaxLength(100).IsRequired(); b.Property(x => x.ProductNameSnapshot).HasMaxLength(250).IsRequired(); b.Property(x => x.UnitNameSnapshot).HasMaxLength(100).IsRequired();
        b.HasIndex(x => new { x.PurchaseServiceAcceptanceId, x.PurchaseOrderLineId }).IsUnique();
        b.HasOne<PurchaseOrderLine>().WithMany().HasForeignKey(x => x.PurchaseOrderLineId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
