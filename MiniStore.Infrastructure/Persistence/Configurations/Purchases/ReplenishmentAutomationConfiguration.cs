using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class ReplenishmentAutomationSettingsConfiguration : IEntityTypeConfiguration<ReplenishmentAutomationSettings>
{
    public void Configure(EntityTypeBuilder<ReplenishmentAutomationSettings> b)
    {
        b.HasKey(x => x.Id); b.Property<int>("TenantId"); b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex("TenantId", nameof(ReplenishmentAutomationSettings.SingletonKey)).IsUnique();
    }
}

public sealed class ReplenishmentRunConfiguration : IEntityTypeConfiguration<ReplenishmentRun>
{
    public void Configure(EntityTypeBuilder<ReplenishmentRun> b)
    {
        b.HasKey(x => x.Id); b.Property<int>("TenantId"); b.Property(x => x.Error).HasMaxLength(1000);
        b.Property(x => x.TriggeredByUserId).HasMaxLength(450);
        b.HasIndex("TenantId", nameof(ReplenishmentRun.ScheduledForUtc)).IsUnique();
        b.HasMany(x => x.Suggestions).WithOne().HasForeignKey(x => x.ReplenishmentRunId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class ReplenishmentSuggestionConfiguration : IEntityTypeConfiguration<ReplenishmentSuggestion>
{
    public void Configure(EntityTypeBuilder<ReplenishmentSuggestion> b)
    {
        b.HasKey(x => x.Id); b.Property<int>("TenantId");
        foreach (var property in new[] { nameof(ReplenishmentSuggestion.AvailableQuantity), nameof(ReplenishmentSuggestion.ReservedQuantity), nameof(ReplenishmentSuggestion.IncomingQuantity), nameof(ReplenishmentSuggestion.HistoricalDemandQuantity), nameof(ReplenishmentSuggestion.SeasonalFactor), nameof(ReplenishmentSuggestion.TargetQuantity), nameof(ReplenishmentSuggestion.SuggestedStockQuantity), nameof(ReplenishmentSuggestion.SuggestedPurchaseQuantity) }) b.Property<decimal>(property).HasPrecision(18, 6);
        b.Property(x => x.ProductCodeSnapshot).HasMaxLength(50); b.Property(x => x.ProductNameSnapshot).HasMaxLength(200);
        b.Property(x => x.WarehouseNameSnapshot).HasMaxLength(200); b.Property(x => x.PurchaseUnitSymbolSnapshot).HasMaxLength(30); b.Property(x => x.CurrencyCode).HasMaxLength(3);
        b.HasOne<ReplenishmentRule>().WithMany().HasForeignKey(x => x.ReplenishmentRuleId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PurchaseRequest>().WithMany().HasForeignKey(x => x.PurchaseRequestId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<PurchaseSourcingEvent>().WithMany().HasForeignKey(x => x.PurchaseSourcingEventId).OnDelete(DeleteBehavior.Restrict);
    }
}
