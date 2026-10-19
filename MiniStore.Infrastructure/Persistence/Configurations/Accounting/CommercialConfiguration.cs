using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id); builder.Property(x => x.CustomerCode).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired(); builder.Property(x => x.LegalName).HasMaxLength(200);
        builder.Property(x => x.Phone).HasMaxLength(50); builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.TaxNumber).HasMaxLength(100); builder.Property(x => x.RegistrationNumber).HasMaxLength(100);
        builder.Property(x => x.Website).HasMaxLength(500); builder.Property(x => x.PreferredLanguage).HasMaxLength(10).IsRequired();
        builder.Property(x => x.TimeZoneId).HasMaxLength(100).IsRequired(); builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.RowVersion).IsRowVersion(); builder.HasIndex(x => x.CustomerCode).IsUnique();
        builder.HasIndex(x => x.AccountId); builder.HasIndex(x => new { x.IsActive, x.Name });
        builder.HasOne(x=>x.Account).WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
    }
}
public class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
{
    public void Configure(EntityTypeBuilder<CustomerContact> b)
    { b.HasKey(x=>x.Id); b.Property(x=>x.Name).HasMaxLength(200).IsRequired(); b.Property(x=>x.Role).HasMaxLength(100); b.Property(x=>x.Phone).HasMaxLength(50); b.Property(x=>x.Email).HasMaxLength(256); b.Property(x=>x.RowVersion).IsRowVersion(); b.HasIndex(x=>new{x.CustomerId,x.IsPrimary}).IsUnique().HasFilter("[IsPrimary] = 1 AND [IsActive] = 1"); b.HasOne<Customer>().WithMany().HasForeignKey(x=>x.CustomerId).OnDelete(DeleteBehavior.Restrict); }
}
public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> b)
    { b.HasKey(x=>x.Id); b.Property(x=>x.Label).HasMaxLength(100).IsRequired(); b.Property(x=>x.AddressLine1).HasMaxLength(250).IsRequired(); b.Property(x=>x.AddressLine2).HasMaxLength(250); b.Property(x=>x.City).HasMaxLength(100).IsRequired(); b.Property(x=>x.StateOrProvince).HasMaxLength(100); b.Property(x=>x.PostalCode).HasMaxLength(30); b.Property(x=>x.CountryCode).HasMaxLength(2).IsRequired(); b.Property(x=>x.RowVersion).IsRowVersion(); b.HasIndex(x=>new{x.CustomerId,x.Type,x.IsDefault}).IsUnique().HasFilter("[IsDefault] = 1 AND [IsActive] = 1"); b.HasOne<Customer>().WithMany().HasForeignKey(x=>x.CustomerId).OnDelete(DeleteBehavior.Restrict); }
}
public class TaxRateConfiguration : IEntityTypeConfiguration<TaxRate>
{
    public void Configure(EntityTypeBuilder<TaxRate> builder) { builder.HasKey(x => x.Id); builder.Property(x => x.Name).HasMaxLength(100).IsRequired(); builder.Property(x => x.Rate).HasPrecision(9, 4); builder.HasOne<Account>().WithMany().HasForeignKey(x => x.OutputAccountId).OnDelete(DeleteBehavior.Restrict); builder.HasOne<Account>().WithMany().HasForeignKey(x => x.InputAccountId).OnDelete(DeleteBehavior.Restrict); }
}
public class PosTerminalConfiguration : IEntityTypeConfiguration<PosTerminal>
{
    public void Configure(EntityTypeBuilder<PosTerminal> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.BranchId, x.Name }).IsUnique();
        builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.DefaultWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Warehouses).WithOne()
            .HasForeignKey(x => x.PosTerminalId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Warehouses).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
public class PosTerminalProductConfiguration : IEntityTypeConfiguration<PosTerminalProduct>
{
    public void Configure(EntityTypeBuilder<PosTerminalProduct> builder) { builder.HasKey(x => new { x.PosTerminalId, x.ProductId }); builder.HasOne<PosTerminal>().WithMany().HasForeignKey(x => x.PosTerminalId).OnDelete(DeleteBehavior.Cascade); builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict); }
}
public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder) { builder.HasKey(x => x.Id); builder.Property(x => x.Name).HasMaxLength(100).IsRequired(); builder.HasIndex(x => x.Name).IsUnique(); builder.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict); }
}
