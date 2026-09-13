using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class StockTransferHistoryConfiguration
    : IEntityTypeConfiguration<StockTransferHistory>
{
    public void Configure(
        EntityTypeBuilder<StockTransferHistory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FromStatus)
            .IsRequired();

        builder.Property(x => x.ToStatus)
            .IsRequired();

        builder.Property(x => x.Action)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(x => x.Reason)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne<StockTransfer>()
            .WithMany(x => x.History)
            .HasForeignKey(x => x.StockTransferId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.StockTransferId,
            x.CreatedAt
        });
    }
}