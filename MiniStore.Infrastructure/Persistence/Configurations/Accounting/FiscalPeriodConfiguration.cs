using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class FiscalPeriodConfiguration : IEntityTypeConfiguration<FiscalPeriod>
{
    public void Configure(EntityTypeBuilder<FiscalPeriod> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.StatusChangedByUserId).HasMaxLength(450);
        builder.Property(x => x.StatusChangeReason).HasMaxLength(500);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.Property<int>("TenantId");
        builder.HasIndex("TenantId", nameof(FiscalPeriod.StartDate), nameof(FiscalPeriod.EndDate));
        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_FiscalPeriods_DateRange", "[EndDate] >= [StartDate]");
            table.HasCheckConstraint("CK_FiscalPeriods_Status", "[Status] IN (1, 2, 3)");
        });
    }
}
