using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;
namespace MiniStore.Infrastructure.Persistence.Configurations;
public sealed class SupplierCommunicationConfiguration : IEntityTypeConfiguration<SupplierCommunication>, IEntityTypeConfiguration<SupplierCommunicationActivity>, IEntityTypeConfiguration<SupplierCommunicationAttachment>
{
    public void Configure(EntityTypeBuilder<SupplierCommunication> b){b.HasKey(x=>x.Id);b.Property(x=>x.DocumentNumberSnapshot).HasMaxLength(50).IsRequired();b.Property(x=>x.RecipientEmail).HasMaxLength(256).IsRequired();b.Property(x=>x.PreferredLanguage).HasMaxLength(10).IsRequired();b.Property(x=>x.Subject).HasMaxLength(250).IsRequired();b.Property(x=>x.Body).HasMaxLength(10000).IsRequired();b.Property(x=>x.CreatedByUserId).HasMaxLength(450).IsRequired();b.HasIndex(x=>new{x.DocumentType,x.DocumentId,x.CreatedAtUtc});b.HasOne<Supplier>().WithMany().HasForeignKey(x=>x.SupplierId).OnDelete(DeleteBehavior.Restrict);b.HasMany(x=>x.Activities).WithOne().HasForeignKey(x=>x.SupplierCommunicationId).OnDelete(DeleteBehavior.Cascade);b.HasMany(x=>x.Attachments).WithOne().HasForeignKey(x=>x.SupplierCommunicationId).OnDelete(DeleteBehavior.Cascade);}
    public void Configure(EntityTypeBuilder<SupplierCommunicationActivity> b){b.HasKey(x=>x.Id);b.Property(x=>x.Note).HasMaxLength(2000);b.Property(x=>x.EvidenceReference).HasMaxLength(500);b.Property(x=>x.RecordedByUserId).HasMaxLength(450).IsRequired();b.HasIndex(x=>new{x.SupplierCommunicationId,x.OccurredAtUtc});}
    public void Configure(EntityTypeBuilder<SupplierCommunicationAttachment> b){b.HasKey(x=>x.Id);b.Property(x=>x.FileName).HasMaxLength(255).IsRequired();b.Property(x=>x.ContentType).HasMaxLength(150).IsRequired();b.Property(x=>x.Content).IsRequired();b.Property(x=>x.UploadedByUserId).HasMaxLength(450).IsRequired();b.HasIndex(x=>new{x.SupplierCommunicationId,x.Id}).IsUnique();}
}
