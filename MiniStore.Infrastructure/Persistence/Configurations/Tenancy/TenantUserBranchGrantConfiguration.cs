using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class TenantUserBranchGrantConfiguration : IEntityTypeConfiguration<TenantUserBranchGrant>
{
    public void Configure(EntityTypeBuilder<TenantUserBranchGrant> builder)
    {
        builder.HasKey(x => new { x.TenantId, x.UserId, x.BranchId });
        builder.Property(x => x.UserId).HasMaxLength(450);
        builder.Property(x => x.GrantedByUserId).HasMaxLength(450).IsRequired();
        builder.HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<IdentityUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Branch>().WithMany()
            .HasForeignKey(x => new { x.BranchId, x.TenantId })
            .HasPrincipalKey("Id", "TenantId")
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.TenantId, x.BranchId });
    }
}
