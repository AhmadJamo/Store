using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.Code).HasMaxLength(50).IsRequired(); b.HasIndex(x => x.Code).IsUnique();
        b.Property(x => x.NameEnglish).HasMaxLength(100).IsRequired(); b.Property(x => x.NameArabic).HasMaxLength(100).IsRequired();
        b.Property(x => x.DescriptionEnglish).HasMaxLength(1000); b.Property(x => x.DescriptionArabic).HasMaxLength(1000);
        b.Property(x => x.MonthlyPrice).HasPrecision(18, 2); b.Property(x => x.AnnualPrice).HasPrecision(18, 2); b.Property(x => x.Currency).HasMaxLength(3);
        b.Property(x => x.RowVersion).IsRowVersion(); b.HasMany(x => x.Features).WithOne().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.Limits).WithOne().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Cascade);
        b.Navigation(x => x.Features).UsePropertyAccessMode(PropertyAccessMode.Field);
        b.Navigation(x => x.Limits).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
public class PlanFeatureConfiguration : IEntityTypeConfiguration<PlanFeature> { public void Configure(EntityTypeBuilder<PlanFeature> b) { b.HasKey(x => new { x.PlanId, x.Key }); b.Property(x => x.Key).HasMaxLength(100); } }
public class PlanLimitConfiguration : IEntityTypeConfiguration<PlanLimit> { public void Configure(EntityTypeBuilder<PlanLimit> b) { b.HasKey(x => new { x.PlanId, x.Key }); b.Property(x => x.Key).HasMaxLength(100); } }
public class TenantSubscriptionConfiguration : IEntityTypeConfiguration<TenantSubscription>
{
    public void Configure(EntityTypeBuilder<TenantSubscription> b)
    {
        b.HasKey(x => x.Id);
        b.HasAlternateKey(x => new { x.Id, x.TenantId });
        b.HasIndex(x => x.TenantId).IsUnique();
        b.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<Plan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
        b.Property(x => x.ExternalCustomerId).HasMaxLength(200);
        b.Property(x => x.ExternalSubscriptionId).HasMaxLength(200);
        b.Property(x => x.RowVersion).IsRowVersion();
    }
}
public class PlatformOperatorConfiguration : IEntityTypeConfiguration<PlatformOperator>
{
    public void Configure(EntityTypeBuilder<PlatformOperator> b) { b.HasKey(x => x.UserId); b.HasOne<IdentityUser>().WithOne().HasForeignKey<PlatformOperator>(x => x.UserId).OnDelete(DeleteBehavior.Cascade); }
}
public class PromotionCodeConfiguration : IEntityTypeConfiguration<PromotionCode>
{
    public void Configure(EntityTypeBuilder<PromotionCode> b) { b.HasKey(x => x.Id); b.Property(x => x.Code).HasMaxLength(40); b.HasIndex(x => x.Code).IsUnique(); b.Property(x => x.DiscountPercentage).HasPrecision(5, 2); b.Property(x => x.RowVersion).IsRowVersion(); b.HasOne<Plan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict); }
}
public class PromotionRedemptionConfiguration : IEntityTypeConfiguration<PromotionRedemption>
{
    public void Configure(EntityTypeBuilder<PromotionRedemption> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.DiscountPercentage).HasPrecision(5, 2);
        b.HasIndex(x => new { x.PromotionCodeId, x.TenantId }).IsUnique();
        b.HasOne<PromotionCode>().WithMany().HasForeignKey(x => x.PromotionCodeId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne<TenantSubscription>().WithMany()
            .HasForeignKey(x => new { x.SubscriptionId, x.TenantId })
            .HasPrincipalKey(x => new { x.Id, x.TenantId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
public class BillingCheckoutSessionConfiguration : IEntityTypeConfiguration<BillingCheckoutSession>
{
    public void Configure(EntityTypeBuilder<BillingCheckoutSession> b) { b.HasKey(x => x.Id); b.Property(x => x.Subtotal).HasPrecision(18,2); b.Property(x => x.DiscountAmount).HasPrecision(18,2); b.Property(x => x.Total).HasPrecision(18,2); b.Property(x => x.Currency).HasMaxLength(3); b.Property(x => x.ProviderReference).HasMaxLength(200); b.Property(x => x.RowVersion).IsRowVersion(); b.HasIndex(x => new { x.TenantId, x.CreatedAt }); b.HasIndex(x => x.Status); b.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict); b.HasOne<Plan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict); b.HasOne<PromotionCode>().WithMany().HasForeignKey(x => x.PromotionCodeId).OnDelete(DeleteBehavior.Restrict); }
}
