using Microsoft.EntityFrameworkCore;using Microsoft.EntityFrameworkCore.Metadata.Builders;using MiniStore.Domain.Entities;
namespace MiniStore.Infrastructure.Persistence.Configurations;
public sealed class InventoryRecallConfiguration:IEntityTypeConfiguration<InventoryRecall>
{
 public void Configure(EntityTypeBuilder<InventoryRecall>b){b.ToTable("InventoryRecalls");b.HasKey(x=>x.Id);b.Property(x=>x.Reference).HasMaxLength(50).IsRequired();b.Property(x=>x.Identifier).HasMaxLength(100).IsRequired();b.Property(x=>x.Reason).HasMaxLength(250).IsRequired();b.Property(x=>x.CreatedByUserId).HasMaxLength(450).IsRequired();b.Property(x=>x.ClosedByUserId).HasMaxLength(450);b.Property(x=>x.ClosureNotes).HasMaxLength(250);b.Property(x=>x.RowVersion).IsRowVersion();b.HasIndex(x=>x.Reference).IsUnique();b.HasIndex(x=>new{x.ProductId,x.Identifier,x.Status}).IsUnique().HasFilter("[Status] = 1");b.HasOne<Product>().WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);}
}
public sealed class InventoryRecallCommunicationConfiguration:IEntityTypeConfiguration<InventoryRecallCommunication>
{
 public void Configure(EntityTypeBuilder<InventoryRecallCommunication>b){b.ToTable("InventoryRecallCommunications");b.HasKey(x=>x.Id);b.Property(x=>x.PartyName).HasMaxLength(200).IsRequired();b.Property(x=>x.ChannelAddress).HasMaxLength(200).IsRequired();b.Property(x=>x.Notes).HasMaxLength(500).IsRequired();b.Property(x=>x.CreatedByUserId).HasMaxLength(450).IsRequired();b.HasIndex(x=>new{x.InventoryRecallId,x.CreatedAt});b.HasOne<InventoryRecall>().WithMany().HasForeignKey(x=>x.InventoryRecallId).OnDelete(DeleteBehavior.Restrict);}
}
