using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id); builder.Property(x => x.Code).HasMaxLength(30).IsRequired(); builder.Property(x => x.Name).HasMaxLength(200).IsRequired(); builder.HasIndex(x => x.Code).IsUnique();
        builder.HasOne<Account>().WithMany().HasForeignKey(x => x.ParentAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder) { builder.HasKey(x => x.Id); builder.Property(x => x.Code).HasMaxLength(30).IsRequired(); builder.Property(x => x.Name).HasMaxLength(200).IsRequired(); builder.HasIndex(x => x.Code).IsUnique(); builder.HasOne<Account>().WithMany().HasForeignKey(x => x.SalesRevenueAccountId).OnDelete(DeleteBehavior.Restrict); }
}
public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.HasKey(x => x.Id); builder.Property(x => x.EntryNumber).HasMaxLength(50).IsRequired(); builder.HasIndex(x => x.EntryNumber).IsUnique(); builder.Property(x => x.Description).HasMaxLength(500); builder.Property(x => x.SourceType).HasMaxLength(50); builder.Property(x => x.SourceReference).HasMaxLength(100); builder.HasIndex(x => new { x.SourceType, x.SourceReference }).IsUnique().HasFilter("[SourceType] IS NOT NULL AND [SourceReference] IS NOT NULL");
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.JournalEntryId).OnDelete(DeleteBehavior.Restrict);
    }
}
public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
    {
        builder.HasKey(x => x.Id); builder.Property(x => x.Debit).HasPrecision(18, 2); builder.Property(x => x.Credit).HasPrecision(18, 2); builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
    }
}
