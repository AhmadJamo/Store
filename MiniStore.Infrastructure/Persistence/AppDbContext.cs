using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniStore.Domain.Entities;
using MiniStore.Domain.Interfaces;
using System.Linq.Expressions;

namespace MiniStore.Infrastructure.Persistence;

public class AppDbContext
    : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    private readonly ITenantContext? _tenantContext;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ITenantContext? tenantContext = null)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public int CurrentTenantId => _tenantContext?.TenantId ?? 0;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();
    public DbSet<TenantUserRole> TenantUserRoles => Set<TenantUserRole>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<PlanLimit> PlanLimits => Set<PlanLimit>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<PlatformOperator> PlatformOperators => Set<PlatformOperator>();
    public DbSet<PromotionCode> PromotionCodes => Set<PromotionCode>();
    public DbSet<PromotionRedemption> PromotionRedemptions => Set<PromotionRedemption>();
    public DbSet<BillingCheckoutSession> BillingCheckoutSessions => Set<BillingCheckoutSession>();

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
    public DbSet<PosTerminalSettings> PosTerminalSettings => Set<PosTerminalSettings>();
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

        ConfigureTenantIsolation(modelBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplyTenantBoundary();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ApplyTenantBoundary();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplyTenantBoundary()
    {
        var entries = ChangeTracker.Entries()
            .Where(x => TenantOwnedTypes.Contains(x.Metadata.ClrType) &&
                        x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

        foreach (var entry in entries)
        {
            if (CurrentTenantId <= 0)
                throw new InvalidOperationException("An active tenant is required for business data changes.");

            var tenantProperty = entry.Property("TenantId");
            if (entry.State == EntityState.Added)
            {
                tenantProperty.CurrentValue = CurrentTenantId;
                continue;
            }

            if (!Equals(tenantProperty.OriginalValue, CurrentTenantId))
                throw new InvalidOperationException("Cross-tenant data changes are not allowed.");
            tenantProperty.CurrentValue = CurrentTenantId;
        }
    }

    private void ConfigureTenantIsolation(ModelBuilder modelBuilder)
    {
        foreach (var clrType in TenantOwnedTypes)
        {
            var entity = modelBuilder.Entity(clrType);
            entity.Property<int>("TenantId").IsRequired();

            var uniqueIndexes = entity.Metadata.GetIndexes()
                .Where(x => x.IsUnique && x.Properties.All(p => p.Name != "TenantId"))
                .ToList();
            foreach (var uniqueIndex in uniqueIndexes)
            {
                var propertyNames = uniqueIndex.Properties.Select(x => x.Name).ToArray();
                var filter = uniqueIndex.GetFilter();
                entity.Metadata.RemoveIndex(uniqueIndex.Properties);
                var replacement = entity.HasIndex(["TenantId", .. propertyNames]).IsUnique();
                if (!string.IsNullOrWhiteSpace(filter)) replacement.HasFilter(filter);
            }

            entity.HasIndex("TenantId");
            entity.HasOne(typeof(Tenant), null)
                .WithMany()
                .HasForeignKey("TenantId")
                .OnDelete(DeleteBehavior.Restrict);

            var parameter = Expression.Parameter(clrType, "entity");
            var tenantProperty = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                [typeof(int)],
                parameter,
                Expression.Constant("TenantId"));
            var currentTenant = Expression.Property(
                Expression.Constant(this),
                nameof(CurrentTenantId));
            entity.HasQueryFilter(Expression.Lambda(
                Expression.Equal(tenantProperty, currentTenant),
                parameter));
        }
    }

    private static readonly Type[] TenantOwnedTypes =
    [
        typeof(Product), typeof(Warehouse), typeof(ProductStock), typeof(StockTransaction),
        typeof(Supplier), typeof(Purchase), typeof(PurchaseItem), typeof(Sale), typeof(SaleItem),
        typeof(InvoiceSettings), typeof(AuditLog), typeof(GeneralSettings), typeof(DiscountSettings),
        typeof(StockTransfer), typeof(StockTransferItem), typeof(StockTransferHistory),
        typeof(DocumentNumberSettings), typeof(Branch), typeof(Account), typeof(JournalEntry),
        typeof(JournalEntryLine), typeof(Customer), typeof(TaxRate), typeof(PosTerminal),
        typeof(PosTerminalProduct), typeof(PosTerminalWarehouse), typeof(PosTerminalSettings),
        typeof(BranchWarehouseAccess), typeof(AccountingSettings), typeof(InventorySettings),
        typeof(PaymentMethod), typeof(StorageLocation), typeof(ProductLocationStock),
        typeof(LocationMovement)
    ];
}
