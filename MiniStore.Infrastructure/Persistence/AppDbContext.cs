using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence;

public class AppDbContext
    : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Warehouse> Warehouses
        => Set<Warehouse>();

    public DbSet<ProductStock> ProductStocks
        => Set<ProductStock>();

    public DbSet<StockTransaction> StockTransactions
        => Set<StockTransaction>();

    public DbSet<Supplier> Suppliers
        => Set<Supplier>();

    public DbSet<Purchase> Purchases
        => Set<Purchase>();

    public DbSet<PurchaseItem> PurchaseItems
        => Set<PurchaseItem>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<Sale> Sales => Set<Sale>();

    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    public DbSet<InvoiceSettings> InvoiceSettings => Set<InvoiceSettings>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<GeneralSettings> GeneralSettings=> Set<GeneralSettings>();

    public DbSet<DiscountSettings> DiscountSettings { get; set; }







    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}
