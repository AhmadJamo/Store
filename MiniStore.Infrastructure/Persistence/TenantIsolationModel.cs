using System.Collections.Frozen;
using MiniStore.Domain.Entities;

namespace MiniStore.Infrastructure.Persistence;

public static class TenantIsolationModel
{
    public static IReadOnlySet<Type> TenantOwnedTypes { get; } = new Type[]
    {
        typeof(Product), typeof(ProductTemplate), typeof(ProductCategory), typeof(ProductAttributeDefinition), typeof(ProductAttributeOption),
        typeof(ProductCategoryAttribute), typeof(ProductAttributeValue), typeof(ProductRecipe), typeof(RecipeIngredient), typeof(MeasurementUnit),
        typeof(Warehouse), typeof(ProductStock), typeof(StockTransaction),
        typeof(Supplier), typeof(SupplierProductPurchasingInfo), typeof(PurchaseRequest), typeof(PurchaseRequestLine), typeof(PurchaseRequestHistory), typeof(PurchaseApprovalRule), typeof(PurchaseApprovalRuleStep), typeof(PurchaseApprovalInstance), typeof(PurchaseApprovalStep), typeof(PurchaseSourcingEvent), typeof(PurchaseSourcingLine), typeof(PurchaseSupplierInvitation), typeof(SupplierQuotation), typeof(SupplierQuotationLine), typeof(PurchaseQuotationAward), typeof(PurchaseOrder), typeof(PurchaseOrderLine), typeof(GoodsReceipt), typeof(GoodsReceiptLine), typeof(GoodsReceiptReturn), typeof(GoodsReceiptReturnLine), typeof(VendorBill), typeof(VendorBillLine), typeof(Purchase), typeof(PurchaseItem), typeof(PurchaseReturn), typeof(PurchaseReturnItem), typeof(Sale), typeof(SaleItem),
        typeof(SalesReturn), typeof(SalesReturnItem),
        typeof(DocumentSequence), typeof(AuditLog), typeof(GeneralSettings), typeof(DiscountSettings),
        typeof(StockTransfer), typeof(StockTransferItem), typeof(StockTransferHistory),
        typeof(Branch), typeof(Account), typeof(JournalEntry), typeof(JournalEntryLine), typeof(FiscalPeriod),
        typeof(Customer), typeof(TaxRate), typeof(PosTerminal), typeof(PosTerminalProduct),
        typeof(PosTerminalWarehouse), typeof(PosTerminalSettings), typeof(BranchWarehouseAccess),
        typeof(AccountingSettings), typeof(InventorySettings), typeof(PaymentMethod),
        typeof(StorageLocation), typeof(ProductLocationStock), typeof(LocationMovement), typeof(StockMovement), typeof(InventoryBalance),
        typeof(InventoryReservation), typeof(InventoryReservationLine), typeof(InventoryAdjustment), typeof(InventoryAdjustmentLine),
        typeof(InventoryTrackingBalance), typeof(InventoryTrackingTransaction), typeof(InventoryRecall), typeof(InventoryRecallCommunication), typeof(PutawayRule), typeof(ReplenishmentRule)
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
