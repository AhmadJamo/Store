using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public sealed class SupplierIntegrationConfiguration :
    IEntityTypeConfiguration<SupplierIntegrationCredential>, IEntityTypeConfiguration<SupplierPortalAccess>,
    IEntityTypeConfiguration<SupplierExternalSubmission>, IEntityTypeConfiguration<SupplierIntegrationOutbox>,
    IEntityTypeConfiguration<SupplierExternalAuditEvent>
{
    public void Configure(EntityTypeBuilder<SupplierIntegrationCredential> b)
    {
        b.HasKey(x=>x.Id); b.Property(x=>x.Name).HasMaxLength(150).IsRequired(); b.Property(x=>x.KeyPrefix).HasMaxLength(20).IsRequired();
        b.Property(x=>x.KeyHash).HasMaxLength(64).IsRequired(); b.Property(x=>x.Scopes).HasMaxLength(500).IsRequired(); b.Property(x=>x.CallbackUrl).HasMaxLength(1000);
        b.Property(x=>x.ProtectedCallbackSecret).HasMaxLength(4000); b.Property(x=>x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x=>x.RowVersion).IsRowVersion();
        b.HasIndex(x=>x.KeyHash).IsUnique(); b.HasOne<Supplier>().WithMany().HasForeignKey(x=>x.SupplierId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<SupplierPortalAccess> b)
    {
        b.HasKey(x=>x.Id); b.Property(x=>x.DocumentNumberSnapshot).HasMaxLength(50).IsRequired(); b.Property(x=>x.TokenHash).HasMaxLength(64).IsRequired(); b.Property(x=>x.CreatedByUserId).HasMaxLength(450).IsRequired();
        b.HasIndex(x=>x.TokenHash).IsUnique(); b.HasIndex(x=>new{x.DocumentType,x.DocumentId,x.SupplierId}); b.HasOne<Supplier>().WithMany().HasForeignKey(x=>x.SupplierId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<SupplierExternalSubmission> b)
    {
        b.HasKey(x=>x.Id); b.Property(x=>x.IdempotencyKey).HasMaxLength(100).IsRequired(); b.Property(x=>x.RequestHash).HasMaxLength(64).IsRequired(); b.Property(x=>x.Payload).HasMaxLength(2_000_000).IsRequired();
        b.Property(x=>x.ExternalReference).HasMaxLength(250); b.Property(x=>x.ResultType).HasMaxLength(100); b.Property(x=>x.CreatedByActor).HasMaxLength(200).IsRequired();
        b.HasIndex(x=>new{x.Channel,x.ChannelIdentityId,x.IdempotencyKey}).IsUnique(); b.HasIndex(x=>new{x.DocumentType,x.DocumentId,x.CreatedAtUtc}); b.HasOne<Supplier>().WithMany().HasForeignKey(x=>x.SupplierId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<SupplierIntegrationOutbox> b)
    {
        b.HasKey(x=>x.Id); b.Property(x=>x.MessageType).HasMaxLength(100).IsRequired(); b.Property(x=>x.AggregateType).HasMaxLength(100).IsRequired(); b.Property(x=>x.Destination).HasMaxLength(1000).IsRequired(); b.Property(x=>x.ProtectedAuthorizationSecret).HasMaxLength(4000);
        b.Property(x=>x.Payload).HasMaxLength(2_000_000).IsRequired(); b.Property(x=>x.IdempotencyKey).HasMaxLength(100).IsRequired(); b.Property(x=>x.LastError).HasMaxLength(2000); b.Property(x=>x.CreatedByUserId).HasMaxLength(450).IsRequired();
        b.HasIndex(x=>x.IdempotencyKey).IsUnique(); b.HasIndex(x=>new{x.Status,x.NextAttemptAtUtc}); b.HasOne<SupplierIntegrationCredential>().WithMany().HasForeignKey(x=>x.SupplierIntegrationCredentialId).OnDelete(DeleteBehavior.Restrict);
    }
    public void Configure(EntityTypeBuilder<SupplierExternalAuditEvent> b)
    {
        b.HasKey(x=>x.Id); b.Property(x=>x.Actor).HasMaxLength(200).IsRequired(); b.Property(x=>x.Action).HasMaxLength(150).IsRequired(); b.Property(x=>x.Outcome).HasMaxLength(100).IsRequired(); b.Property(x=>x.CorrelationId).HasMaxLength(100).IsRequired(); b.Property(x=>x.RemoteAddress).HasMaxLength(100);
        b.HasIndex(x=>new{x.CreatedAtUtc,x.Channel}); b.HasOne<Supplier>().WithMany().HasForeignKey(x=>x.SupplierId).OnDelete(DeleteBehavior.Restrict);
    }
}
