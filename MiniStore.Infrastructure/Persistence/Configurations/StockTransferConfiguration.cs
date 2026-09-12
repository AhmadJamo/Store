using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class StockTransferConfiguration
    : IEntityTypeConfiguration<StockTransfer>
{
    public void Configure(
        EntityTypeBuilder<StockTransfer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TransferNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(x => x.Reference)
            .HasMaxLength(200);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.SubmittedByUserId)
            .HasMaxLength(450);

        builder.Property(x => x.ApprovedByUserId)
            .HasMaxLength(450);

        builder.Property(x => x.RejectedByUserId)
            .HasMaxLength(450);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(x => x.PostedByUserId)
            .HasMaxLength(450);

        builder.Property(x => x.CancelledByUserId)
            .HasMaxLength(450);

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.TransferNumber)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.Status,
            x.CreatedAt
        });

        builder.HasOne<Warehouse>()
            .WithMany()
            .HasForeignKey(x => x.FromWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Warehouse>()
            .WithMany()
            .HasForeignKey(x => x.ToWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.StockTransferId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.History)
            .WithOne()
            .HasForeignKey(x => x.StockTransferId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}