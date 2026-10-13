using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class PurchaseDocumentReversalConfiguration : IEntityTypeConfiguration<PurchaseDocumentReversal>
{
    public void Configure(EntityTypeBuilder<PurchaseDocumentReversal> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.ReversalNumber).HasMaxLength(50).IsRequired();
        b.Property(x => x.OriginalDocumentNumber).HasMaxLength(100).IsRequired(); b.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
        b.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired(); b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired();
        b.Property(x => x.ReversalJournalEntryNumber).HasMaxLength(50); b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => x.ReversalNumber).IsUnique(); b.HasIndex(x => x.IdempotencyKey).IsUnique();
        b.HasIndex(x => new { x.DocumentType, x.OriginalDocumentId }).IsUnique();
    }
}
