using System.Collections.Frozen;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence;

public static class TenantIsolationModel
{
    public static IReadOnlySet<Type> TenantOwnedTypes { get; } = new Type[]
    {
        typeof(Product), typeof(Warehouse), typeof(ProductStock), typeof(StockTransaction),
        typeof(Supplier), typeof(Purchase), typeof(PurchaseItem), typeof(Sale), typeof(SaleItem),
        typeof(DocumentSequence), typeof(AuditLog), typeof(GeneralSettings), typeof(DiscountSettings),
        typeof(StockTransfer), typeof(StockTransferItem), typeof(StockTransferHistory),
        typeof(Branch), typeof(Account), typeof(JournalEntry), typeof(JournalEntryLine),
        typeof(Customer), typeof(TaxRate), typeof(PosTerminal), typeof(PosTerminalProduct),
        typeof(PosTerminalWarehouse), typeof(PosTerminalSettings), typeof(BranchWarehouseAccess),
        typeof(AccountingSettings), typeof(InventorySettings), typeof(PaymentMethod),
        typeof(StorageLocation), typeof(ProductLocationStock), typeof(LocationMovement)
    }.ToFrozenSet();

    public static IReadOnlySet<Type> NonBusinessTypes { get; } = new Type[]
    {
        typeof(Tenant), typeof(TenantMembership), typeof(TenantUserRole),
        typeof(TenantRole), typeof(TenantRolePermission), typeof(Permission),
        typeof(Plan), typeof(PlanFeature), typeof(PlanLimit), typeof(TenantSubscription),
        typeof(PlatformOperator), typeof(PromotionCode), typeof(PromotionRedemption),
        typeof(BillingCheckoutSession), typeof(CompanyOnboarding)
    }.ToFrozenSet();
}
