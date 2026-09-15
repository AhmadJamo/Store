using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class CompanyOnboardingConfiguration : IEntityTypeConfiguration<CompanyOnboarding>
{
    public void Configure(EntityTypeBuilder<CompanyOnboarding> builder)
    {
        builder.HasKey(x => x.TenantId);
        builder.HasOne<Tenant>().WithOne()
            .HasForeignKey<CompanyOnboarding>(x => x.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.BusinessType).HasConversion<int>();
        builder.Property(x => x.Language).HasConversion<int>();
        builder.Property(x => x.InventoryControlMode).HasConversion<int>();
        builder.Property(x => x.CountryCode).HasMaxLength(3);
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.DefaultTaxRate).HasPrecision(9, 4);
        builder.Property(x => x.CompletedByUserId).HasMaxLength(450);
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}
