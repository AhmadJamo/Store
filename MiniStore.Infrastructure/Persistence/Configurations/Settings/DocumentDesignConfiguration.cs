using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class DocumentDesignConfiguration : IEntityTypeConfiguration<DocumentDesign>, IEntityTypeConfiguration<DocumentDesignSection>
{
    public void Configure(EntityTypeBuilder<DocumentDesign> b)
    {
        b.ToTable("DocumentDesigns", t =>
        {
            t.HasCheckConstraint("CK_DocumentDesigns_Version", "[Version] BETWEEN 1 AND 9999");
            t.HasCheckConstraint("CK_DocumentDesigns_Margins", "[MarginTopMm] BETWEEN 0 AND 50 AND [MarginEndMm] BETWEEN 0 AND 50 AND [MarginBottomMm] BETWEEN 0 AND 50 AND [MarginStartMm] BETWEEN 0 AND 50");
            t.HasCheckConstraint("CK_DocumentDesigns_LogoHeight", "[LogoMaximumHeightPx] BETWEEN 24 AND 300");
            t.HasCheckConstraint("CK_DocumentDesigns_WatermarkOpacity", "[WatermarkOpacity] BETWEEN 0.02 AND 0.5");
        });
        b.HasKey(x => x.Id); b.Property(x => x.Code).HasMaxLength(50).IsRequired(); b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.Description).HasMaxLength(1000); b.Property(x => x.CustomPageWidthMm).HasPrecision(6, 2); b.Property(x => x.CustomPageHeightMm).HasPrecision(6, 2);
        b.Property(x => x.MarginTopMm).HasPrecision(5, 2); b.Property(x => x.MarginEndMm).HasPrecision(5, 2); b.Property(x => x.MarginBottomMm).HasPrecision(5, 2); b.Property(x => x.MarginStartMm).HasPrecision(5, 2);
        b.Property(x => x.PrimaryColor).HasMaxLength(7).IsRequired(); b.Property(x => x.AccentColor).HasMaxLength(7).IsRequired(); b.Property(x => x.TextColor).HasMaxLength(7).IsRequired(); b.Property(x => x.TableHeaderTextColor).HasMaxLength(7).IsRequired();
        b.Property(x => x.CoverTitle).HasMaxLength(250); b.Property(x => x.CoverTitleArabic).HasMaxLength(250); b.Property(x => x.CoverSubtitle).HasMaxLength(1000); b.Property(x => x.CoverSubtitleArabic).HasMaxLength(1000);
        b.Property(x => x.HeaderText).HasMaxLength(500); b.Property(x => x.HeaderTextArabic).HasMaxLength(500); b.Property(x => x.FooterText).HasMaxLength(1000); b.Property(x => x.FooterTextArabic).HasMaxLength(1000);
        b.Property(x => x.WatermarkText).HasMaxLength(100); b.Property(x => x.WatermarkTextArabic).HasMaxLength(100); b.Property(x => x.WatermarkOpacity).HasPrecision(4, 3);
        b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.DocumentType, x.Code, x.Version }).IsUnique();
        b.HasIndex(x => new { x.DocumentType, x.IsDefault }).HasFilter("[IsDefault] = 1 AND [Status] = 2").IsUnique();
        b.HasOne<DocumentDesign>().WithMany().HasForeignKey(x => x.SourceDesignId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(x => x.Sections).WithOne().HasForeignKey(x => x.DocumentDesignId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<DocumentDesignSection> b)
    {
        b.HasKey(x => x.Id); b.Property(x => x.Title).HasMaxLength(150).IsRequired(); b.Property(x => x.TitleArabic).HasMaxLength(150);
        b.Property(x => x.Content).HasMaxLength(4000); b.Property(x => x.ContentArabic).HasMaxLength(4000);
        b.HasIndex(x => new { x.DocumentDesignId, x.Sequence }).IsUnique();
    }
}
