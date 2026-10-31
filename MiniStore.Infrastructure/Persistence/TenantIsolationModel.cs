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
        typeof(Supplier), typeof(SupplierProductPurchasingInfo), typeof(SupplierProductPriceBreak), typeof(SupplierProductPriceHistory), typeof(SupplierProductPriceBreakSnapshot), typeof(PurchaseRequest), typeof(PurchaseRequestLine), typeof(PurchaseRequestHistory), typeof(PurchaseApprovalRule), typeof(PurchaseApprovalRuleStep), typeof(PurchaseApprovalInstance), typeof(PurchaseApprovalStep), typeof(PurchaseSourcingEvent), typeof(PurchaseSourcingLine), typeof(PurchaseSupplierInvitation), typeof(PurchaseSourcingAttachment), typeof(PurchaseSourcingPolicy), typeof(SupplierQuotation), typeof(SupplierQuotationLine), typeof(PurchaseQuotationAward), typeof(PurchaseQuotationAwardAllocation), typeof(PurchaseOrder), typeof(PurchaseOrderLine), typeof(PurchaseOrderLifecycleHistory), typeof(PurchaseDemandAllocation), typeof(PurchaseDirectDeliveryEvent), typeof(PurchaseOrderSupplierConfirmation), typeof(SupplierCommunication), typeof(SupplierCommunicationActivity), typeof(SupplierCommunicationAttachment), typeof(SupplierIntegrationCredential), typeof(SupplierPortalAccess), typeof(SupplierExternalSubmission), typeof(SupplierIntegrationOutbox), typeof(SupplierExternalAuditEvent), typeof(PurchaseServiceAcceptance), typeof(PurchaseServiceAcceptanceLine), typeof(PurchaseAgreement), typeof(PurchaseAgreementLine), typeof(PurchaseTemplate), typeof(PurchaseTemplateLine), typeof(LandedCost), typeof(LandedCostComponent), typeof(LandedCostAllocation), typeof(PurchaseDocumentReversal), typeof(GoodsReceipt), typeof(GoodsReceiptLine), typeof(GoodsReceiptScan), typeof(GoodsReceiptReturn), typeof(GoodsReceiptReturnLine), typeof(VendorBill), typeof(VendorBillLine), typeof(VendorBillAttachment), typeof(VendorBillExtractionSuggestion), typeof(SupplierCreditNote), typeof(SupplierCreditNoteLine), typeof(PurchaseMatchRun), typeof(PurchaseMatchException), typeof(PurchaseMatchingSettings), typeof(SupplierPayment), typeof(SupplierPaymentLine), typeof(Purchase), typeof(PurchaseItem), typeof(PurchaseReturn), typeof(PurchaseReturnItem), typeof(Sale), typeof(SaleItem),
        typeof(SalesReturn), typeof(SalesReturnItem), typeof(SalesPriceList), typeof(SalesPriceRule),
        typeof(SalesQuotation), typeof(SalesQuotationLine), typeof(SalesQuotationHistory), typeof(SalesQuotationDocumentSnapshot),
        typeof(SalesQuotationTemplate), typeof(SalesQuotationTemplateItem),
        typeof(DocumentDesign), typeof(DocumentDesignSection),
        typeof(AnalyticsWorkspace), typeof(AnalyticsWorkspaceWidget), typeof(AnalyticsWorkspaceDelegation),
        typeof(SalesOrder), typeof(SalesOrderLine), typeof(SalesOrderHistory),
        typeof(SalesDelivery), typeof(SalesDeliveryLine), typeof(SalesDeliveryHistory),
        typeof(CustomerInvoice), typeof(CustomerInvoiceLine), typeof(CustomerInvoiceSourceAllocation), typeof(CustomerInvoiceHistory),
        typeof(CustomerPayment), typeof(CustomerPaymentLine),
        typeof(CustomerReturnRequest), typeof(CustomerReturnRequestLine), typeof(CustomerReturnRequestHistory),
        typeof(CustomerReturnReceipt), typeof(CustomerReturnReceiptLine), typeof(CustomerReturnReceiptHistory),
        typeof(CustomerCreditNote), typeof(CustomerCreditNoteLine),
        typeof(SalesApprovalRule), typeof(SalesApprovalRuleStep), typeof(SalesOrderApproval), typeof(SalesOrderApprovalFinding), typeof(SalesOrderApprovalStep),
        typeof(DocumentSequence), typeof(AuditLog), typeof(GeneralSettings), typeof(OrganizationIdentityField), typeof(OrganizationLogo), typeof(CurrencyExchangeRate), typeof(DiscountSettings),
        typeof(StockTransfer), typeof(StockTransferItem), typeof(StockTransferHistory),
        typeof(Branch), typeof(Account), typeof(JournalEntry), typeof(JournalEntryLine), typeof(FiscalPeriod),
        typeof(Customer), typeof(CustomerContact), typeof(CustomerAddress), typeof(TaxRate), typeof(PosTerminal), typeof(PosTerminalProduct),
        typeof(PosTerminalWarehouse), typeof(PosTerminalSettings), typeof(BranchWarehouseAccess),
        typeof(AccountingSettings), typeof(InventorySettings), typeof(PaymentMethod),
        typeof(StorageLocation), typeof(ProductLocationStock), typeof(LocationMovement), typeof(StockMovement), typeof(InventoryBalance),
        typeof(InventoryReservation), typeof(InventoryReservationLine), typeof(InventoryAdjustment), typeof(InventoryAdjustmentLine),
        typeof(InventoryTrackingBalance), typeof(InventoryTrackingTransaction), typeof(InventoryRecall), typeof(InventoryRecallCommunication), typeof(PutawayRule), typeof(ReplenishmentRule),
        typeof(ReplenishmentAutomationSettings), typeof(ReplenishmentRun), typeof(ReplenishmentSuggestion)
    }.ToFrozenSet();

    public static IReadOnlySet<Type> NonBusinessTypes { get; } = new Type[]
    {
        typeof(Tenant), typeof(TenantMembership), typeof(TenantUserRole),
        typeof(TenantRole), typeof(TenantRolePermission), typeof(Permission),
        typeof(Plan), typeof(PlanFeature), typeof(PlanLimit), typeof(TenantSubscription),
        typeof(PlatformOperator), typeof(PromotionCode), typeof(PromotionRedemption),
        typeof(BillingCheckoutSession), typeof(CompanyOnboarding)
    }.ToFrozenSet();

    public static IReadOnlySet<Type> MixedScopeTypes { get; } = new Type[]
    {
        typeof(AuditEvent), typeof(AuditLegalHold)
    }.ToFrozenSet();
}
