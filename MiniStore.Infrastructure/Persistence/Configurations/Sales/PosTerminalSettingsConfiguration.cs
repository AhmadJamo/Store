using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class PosTerminalSettingsConfiguration
    : IEntityTypeConfiguration<PosTerminalSettings>
{
    public void Configure(EntityTypeBuilder<PosTerminalSettings> builder)
    {
        builder.HasKey(x => x.PosTerminalId);
        builder.HasOne<PosTerminal>().WithOne()
            .HasForeignKey<PosTerminalSettings>(x => x.PosTerminalId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.Profile).HasConversion<int>().IsRequired();
        builder.Property(x => x.ProductLayout).HasConversion<int>().IsRequired();
        builder.Property(x => x.Theme).HasConversion<int>().IsRequired();
        builder.Property(x => x.CartPosition).HasConversion<int>().IsRequired();
        builder.Property(x => x.EnabledOrderTypes).HasConversion<int>().IsRequired();
        builder.Property(x => x.DefaultOrderType).HasConversion<int>().IsRequired();
        builder.Property(x => x.AccentColor).HasMaxLength(7).IsRequired();
        builder.Property(x => x.HeaderTitle).HasMaxLength(80).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}
