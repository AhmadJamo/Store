using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder) { builder.HasKey(x => x.Id); builder.Property(x => x.Name).HasMaxLength(200).IsRequired(); builder.HasIndex(x => x.AccountId).IsUnique(); builder.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict); }
}
public class TaxRateConfiguration : IEntityTypeConfiguration<TaxRate>
{
    public void Configure(EntityTypeBuilder<TaxRate> builder) { builder.HasKey(x => x.Id); builder.Property(x => x.Name).HasMaxLength(100).IsRequired(); builder.Property(x => x.Rate).HasPrecision(9, 4); builder.HasOne<Account>().WithMany().HasForeignKey(x => x.OutputAccountId).OnDelete(DeleteBehavior.Restrict); builder.HasOne<Account>().WithMany().HasForeignKey(x => x.InputAccountId).OnDelete(DeleteBehavior.Restrict); }
}
public class PosTerminalConfiguration : IEntityTypeConfiguration<PosTerminal>
{
    public void Configure(EntityTypeBuilder<PosTerminal> builder) { builder.HasKey(x => x.Id); builder.Property(x => x.Name).HasMaxLength(100).IsRequired(); builder.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict); builder.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.DefaultWarehouseId).OnDelete(DeleteBehavior.Restrict); }
}
public class PosTerminalProductConfiguration : IEntityTypeConfiguration<PosTerminalProduct>
{
    public void Configure(EntityTypeBuilder<PosTerminalProduct> builder) { builder.HasKey(x => new { x.PosTerminalId, x.ProductId }); builder.HasOne<PosTerminal>().WithMany().HasForeignKey(x => x.PosTerminalId).OnDelete(DeleteBehavior.Cascade); builder.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict); }
}
public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder) { builder.HasKey(x => x.Id); builder.Property(x => x.Name).HasMaxLength(100).IsRequired(); builder.HasIndex(x => x.Name).IsUnique(); builder.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict); }
}
