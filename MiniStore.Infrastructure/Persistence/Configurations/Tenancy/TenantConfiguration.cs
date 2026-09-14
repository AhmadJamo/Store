using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(63).IsRequired();
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
    }
}

public class TenantMembershipConfiguration : IEntityTypeConfiguration<TenantMembership>
{
    public void Configure(EntityTypeBuilder<TenantMembership> builder)
    {
        builder.HasKey(x => new { x.TenantId, x.UserId });
        builder.Property(x => x.UserId).HasMaxLength(450);
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.UserId, x.IsActive });
    }
}
