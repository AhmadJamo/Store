using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class ProductAttributeDefinitionConfiguration : IEntityTypeConfiguration<ProductAttributeDefinition>
{
    public void Configure(EntityTypeBuilder<ProductAttributeDefinition> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(40).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public sealed class ProductAttributeOptionConfiguration : IEntityTypeConfiguration<ProductAttributeOption>
{
    public void Configure(EntityTypeBuilder<ProductAttributeOption> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(40).IsRequired();
        builder.HasIndex(x => new { x.ProductAttributeDefinitionId, x.Code }).IsUnique();
        builder.HasOne<ProductAttributeDefinition>().WithMany()
            .HasForeignKey(x => x.ProductAttributeDefinitionId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ProductCategoryAttributeConfiguration : IEntityTypeConfiguration<ProductCategoryAttribute>
{
    public void Configure(EntityTypeBuilder<ProductCategoryAttribute> builder)
    {
        builder.HasKey(x => new { x.ProductCategoryId, x.ProductAttributeDefinitionId });
        builder.HasOne<ProductCategory>().WithMany().HasForeignKey(x => x.ProductCategoryId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ProductAttributeDefinition>().WithMany().HasForeignKey(x => x.ProductAttributeDefinitionId).OnDelete(DeleteBehavior.Cascade);
    }
}
