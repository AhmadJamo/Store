using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration
    : IEntityTypeConfiguration<Supplier>
{
    public void Configure(
        EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Phone)
            .HasMaxLength(50);

        builder.Property(x => x.LegalName).HasMaxLength(200);
        builder.Property(x => x.ContactPerson).HasMaxLength(150);
        builder.Property(x => x.Email).HasMaxLength(256);

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.Property(x => x.TaxNumber).HasMaxLength(100);
        builder.Property(x => x.RegistrationNumber).HasMaxLength(100);
        builder.Property(x => x.Website).HasMaxLength(500);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
