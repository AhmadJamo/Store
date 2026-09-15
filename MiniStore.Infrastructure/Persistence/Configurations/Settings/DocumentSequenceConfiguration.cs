using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class DocumentSequenceConfiguration : IEntityTypeConfiguration<DocumentSequence>
{
    public void Configure(EntityTypeBuilder<DocumentSequence> builder)
    {
        builder.ToTable("DocumentSequences");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DocumentType).IsRequired();
        builder.Property(x => x.Prefix).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Suffix).HasMaxLength(30).IsRequired();
        builder.Property(x => x.FormatTemplate).HasMaxLength(120).IsRequired();
        builder.Property(x => x.NumberLength).IsRequired();
        builder.Property(x => x.NextNumber).IsRequired();
        builder.Property(x => x.ResetStartNumber).IsRequired();
        builder.Property(x => x.ResetPeriod).IsRequired();
        builder.Property(x => x.CurrentPeriodKey).HasMaxLength(8);
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        builder.HasIndex(x => x.DocumentType).IsUnique();
    }
}
