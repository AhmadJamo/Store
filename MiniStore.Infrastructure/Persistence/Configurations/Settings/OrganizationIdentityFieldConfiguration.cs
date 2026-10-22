using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class OrganizationIdentityFieldConfiguration : IEntityTypeConfiguration<OrganizationIdentityField>
{
    public void Configure(EntityTypeBuilder<OrganizationIdentityField> builder)
    {
        builder.ToTable("OrganizationIdentityFields", table =>
        {
            table.HasCheckConstraint("CK_OrganizationIdentityFields_Sequence", "[Sequence] BETWEEN 1 AND 100");
            table.HasCheckConstraint("CK_OrganizationIdentityFields_DocumentScope", "[DocumentScope] BETWEEN 0 AND 15");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Section).HasConversion<int>().IsRequired();
        builder.Property(x => x.Label).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LabelArabic).HasMaxLength(100);
        builder.Property(x => x.Value).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ValueArabic).HasMaxLength(500);
        builder.Property(x => x.DocumentScope).HasConversion<int>().IsRequired();
        builder.HasIndex(x => new { x.GeneralSettingsId, x.Sequence }).IsUnique();
    }
}

public sealed class OrganizationLogoConfiguration : IEntityTypeConfiguration<OrganizationLogo>
{
    public void Configure(EntityTypeBuilder<OrganizationLogo> builder)
    {
        builder.ToTable("OrganizationLogos");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Content).HasColumnType("varbinary(max)").IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}
