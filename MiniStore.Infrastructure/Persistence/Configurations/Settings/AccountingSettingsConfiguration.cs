using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;
namespace MiniStore.Infrastructure.Persistence.Configurations;
public class AccountingSettingsConfiguration : IEntityTypeConfiguration<AccountingSettings>
{
    public void Configure(EntityTypeBuilder<AccountingSettings> builder)
    {
        builder.HasKey(x=>x.Id); builder.HasIndex(x=>x.SingletonKey).IsUnique();
        builder.HasOne<Account>().WithMany().HasForeignKey(x=>x.PurchaseDiscountAccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany().HasForeignKey(x=>x.SalesDiscountAccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany().HasForeignKey(x=>x.SalesRevenueAccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Account>().WithMany().HasForeignKey(x=>x.CostOfSalesAccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
