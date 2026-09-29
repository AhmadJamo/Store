using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Name).IsRequired().HasMaxLength(100);
        builder.Property(category => category.Code).IsRequired().HasMaxLength(30);
        builder.Property(category => category.IsActive).IsRequired();
        builder.Property(category => category.RowVersion).IsRowVersion();
        builder.HasIndex(category => category.Code).IsUnique();
        builder.HasIndex(category => category.Name).IsUnique();
    }
}
