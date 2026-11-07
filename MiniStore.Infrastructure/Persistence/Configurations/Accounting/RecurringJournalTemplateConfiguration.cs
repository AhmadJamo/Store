using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class RecurringJournalTemplateConfiguration : IEntityTypeConfiguration<RecurringJournalTemplate>
{
    public void Configure(EntityTypeBuilder<RecurringJournalTemplate> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.LastRunKey).HasMaxLength(32);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.RecurringJournalTemplateId).OnDelete(DeleteBehavior.Cascade);
        builder.ToTable(table => table.HasCheckConstraint("CK_RecurringJournalTemplates_DateRange", "[EndDate] IS NULL OR [EndDate] >= [StartDate]"));
    }
}

public sealed class RecurringJournalTemplateLineConfiguration : IEntityTypeConfiguration<RecurringJournalTemplateLine>
{
    public void Configure(EntityTypeBuilder<RecurringJournalTemplateLine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Debit).HasPrecision(18, 2);
        builder.Property(x => x.Credit).HasPrecision(18, 2);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
    }
}
