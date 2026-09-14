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

    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();

    public DbSet<StockTransferItem> StockTransferItems => Set<StockTransferItem>();

    public DbSet<StockTransferHistory> StockTransferHistories => Set<StockTransferHistory>();

    public DbSet<DocumentNumberSettings> DocumentNumberSettings => Set<DocumentNumberSettings>();

    public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();

    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();
    public DbSet<PosTerminal> PosTerminals => Set<PosTerminal>();
    public DbSet<PosTerminalProduct> PosTerminalProducts => Set<PosTerminalProduct>();
    public DbSet<PosTerminalWarehouse> PosTerminalWarehouses => Set<PosTerminalWarehouse>();
    public DbSet<BranchWarehouseAccess> BranchWarehouseAccesses => Set<BranchWarehouseAccess>();
    public DbSet<AccountingSettings> AccountingSettings => Set<AccountingSettings>();
    public DbSet<InventorySettings> InventorySettings => Set<InventorySettings>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<StorageLocation> StorageLocations => Set<StorageLocation>();
    public DbSet<ProductLocationStock> ProductLocationStocks => Set<ProductLocationStock>();

    public DbSet<LocationMovement> LocationMovements => Set<LocationMovement>();







    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}
