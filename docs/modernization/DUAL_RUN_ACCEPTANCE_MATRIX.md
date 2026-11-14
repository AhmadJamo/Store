# Dual-run acceptance matrix

> Generated from source by scripts/generate-mvc-migration-registry.ps1. Do not edit rows manually.

- Presentation counterparts covered: 29.
- Mutation-only MVC compatibility boundaries: 1 (Language).
- Physically retired MVC presentation actions: 106 across 52 surfaces (AnalyticsCenter, AuditEvents, Branches, CustomerCreditNotes, CustomerPayments, CustomerReturnReceipts, GoodsReceiptReturns, GoodsReceipts, InventoryAdjustments, InventoryBalances, InventoryInsights, InventoryReconciliation, InventoryReservations, InventoryScanning, InventoryTracking, JournalEntries, LandedCosts, LocationMovements, PaymentMethods, ProductAttributes, ProductCategories, Products, ProductStocks, ProductTemplates, Public, PurchaseAccountingReconciliation, PurchaseAgreements, PurchaseAnalytics, PurchaseApprovalRules, PurchaseDocumentReversals, PurchaseOrders, PurchaseReplenishment, PurchaseReturns, PurchaseServiceAcceptances, PurchaseTemplates, Recipes, Replenishment, SalesApprovalRules, SalesDeliveries, SalesReturns, StockMovements, StockTransactions, StockTransfers, SupplierCommunications, SupplierCreditNotes, SupplierPayments, SupplierQuotations, Suppliers, TaxRates, UnassignedStock, WarehouseLocations, Warehouses); legacy redirects remain outside the reversible cutover catalog.
- Presentation default: AngularDefault for all presentation surfaces; mutation-only MVC compatibility: Language.
- Evidence baseline: generated MVC view/action inventory, verified Angular route, controller permission references and acceptance gate.
- Runtime dual-run samples and cutover rehearsal results are appended by the MOD-029 acceptance workflow; this static matrix performs no mutation.

| MVC surface | MVC route family | Angular counterpart | Operation class | Permission boundary references | Risk class | Static evidence |
|---|---|---|---|---|---|---|
| Account | /Account | /login, /register, /access-denied | Read/write | Public/session boundary | Identity/tenant | 3 view(s); 6 action(s); route/source gate |
| Accounts | /Accounts | /accounting/accounts | Read/write | Administration.Access | Financial | 2 view(s); 3 action(s); route/source gate |
| CompanyOnboarding | /CompanyOnboarding | /onboarding | Read/write | Administration.Access | Identity/tenant | 1 view(s); 3 action(s); route/source gate |
| CustomerInvoices | /CustomerInvoices | /sales/invoices | Read/write | CustomerInvoices.Approve, CustomerInvoices.Cancel, CustomerInvoices.Create, CustomerInvoices.Post, CustomerInvoices.Reverse, CustomerInvoices.View | Financial | 3 view(s); 8 action(s); route/source gate |
| CustomerReturnRequests | /CustomerReturnRequests | /sales/return-requests | Read/write | CustomerReturnRequests.Approve, CustomerReturnRequests.Cancel, CustomerReturnRequests.Create, CustomerReturnRequests.Reject, CustomerReturnRequests.Submit, CustomerReturnRequests.View | Standard | 3 view(s); 8 action(s); route/source gate |
| Customers | /Customers | /customers | Read/write | Customers.Archive, Customers.Create, Customers.Edit, Customers.View | Standard | 4 view(s); 14 action(s); route/source gate |
| Dashboard | /Dashboard | / | Read/write | Public/session boundary | Standard | 1 view(s); 1 action(s); route/source gate |
| DocumentDesigns | /DocumentDesigns | /settings/document-designs | Read/write | Administration.Access | Standard | 3 view(s); 7 action(s); route/source gate |
| Home | / | / | Read/write | Public/session boundary | Standard | 1 view(s); 1 action(s); route/source gate |
| Language | /Language/Set | Not applicable (mutation-only) | Mutation-only compatibility | Public/session boundary | Identity/tenant | 0 view(s); 1 POST action(s); antiforgery/source gate |
| Platform/Platform | /platform | /platform-control, /platform-management | Read/write | Public/session boundary | Identity/tenant | 7 view(s); 11 action(s); route/source gate |
| Platform/PlatformAccount | /platform/account | /platform-login | Read/write | Public/session boundary | Identity/tenant | 1 view(s); 3 action(s); route/source gate |
| Platform/PlatformAuditEvents | /platform/audit-events | /platform-audit | Read/write | Public/session boundary | Identity/tenant | 1 view(s); 2 action(s); route/source gate |
| Platform/PlatformAuditRetention | /platform/audit-retention | /platform-retention | Read/write | Public/session boundary | Identity/tenant | 1 view(s); 5 action(s); route/source gate |
| PurchaseRequests | /PurchaseRequests | /purchases/requests | Read/write | PurchaseRequests.Approve, PurchaseRequests.Cancel, PurchaseRequests.Create, PurchaseRequests.Reject, PurchaseRequests.Submit, PurchaseRequests.View | Standard | 3 view(s); 6 action(s); route/source gate |
| Purchases | /Purchases | /purchases/legacy | Read/write | Purchases.Create, Purchases.View | Standard | 3 view(s); 5 action(s); route/source gate |
| PurchaseSourcing | /PurchaseSourcing | /purchases/sourcing | Read/write | PurchaseSourcing.Create, PurchaseSourcing.CreateDirect, PurchaseSourcing.View | Standard | 5 view(s); 8 action(s); route/source gate |
| Roles | /Roles | /roles | Read/write | Roles.Create, Roles.Delete, Roles.Edit, Roles.View | Standard | 3 view(s); 6 action(s); route/source gate |
| Sales | /Sales | /sales/legacy | Read/write | Sales.Create, Sales.View | Standard | 4 view(s); 6 action(s); route/source gate |
| SalesOrders | /SalesOrders | /sales/orders | Read/write | SalesOrders.ApproveException, SalesOrders.Cancel, SalesOrders.Confirm, SalesOrders.Create, SalesOrders.ReleaseReservation, SalesOrders.Reserve, SalesOrders.View | Standard | 3 view(s); 10 action(s); route/source gate |
| SalesPriceLists | /SalesPriceLists | /sales/pricing | Read/write | SalesPricing.Manage, SalesPricing.View | Standard | 1 view(s); 6 action(s); route/source gate |
| SalesQuotations | /SalesQuotations | /sales/quotations | Read/write | SalesQuotations.Accept, SalesQuotations.Cancel, SalesQuotations.Create, SalesQuotations.Reject, SalesQuotations.Send, SalesQuotations.View | Standard | 3 view(s); 10 action(s); route/source gate |
| SalesQuotationTemplates | /SalesQuotationTemplates | /sales/quotation-templates | Read/write | SalesQuotationTemplates.Manage, SalesQuotationTemplates.View | Standard | 3 view(s); 7 action(s); route/source gate |
| Settings | /Settings | /settings | Read/write | Administration.Access | Standard | 16 view(s); 36 action(s); route/source gate |
| Subscription | /Subscription | /subscription | Read/write | Public/session boundary | Identity/tenant | 2 view(s); 2 action(s); route/source gate |
| SupplierIntegrations | /SupplierIntegrations | /purchases/integrations | Read/write | SupplierIntegrations.Manage, SupplierIntegrations.Queue, SupplierIntegrations.Retry, SupplierIntegrations.View | Standard | 4 view(s); 7 action(s); route/source gate |
| SupplierPortal | /SupplierPortal | /supplier-portal/:token | Read/write | Public/session boundary | Standard | 2 view(s); 4 action(s); route/source gate |
| SupplierPurchasing | /SupplierPurchasing | /purchases/supplier-terms | Read/write | Purchases.SupplierTerms.Manage, Purchases.SupplierTerms.View | Standard | 5 view(s); 14 action(s); route/source gate |
| Users | /Users | /users | Read/write | Users.Create, Users.View | Standard | 2 view(s); 3 action(s); route/source gate |
| VendorBills | /VendorBills | /purchases/bills | Read/write | VendorBills.Approve, VendorBills.Cancel, VendorBills.Create, VendorBills.ManageAttachments, VendorBills.OverrideMatch, VendorBills.Post, VendorBills.ReviewExtraction, VendorBills.View | Financial | 3 view(s); 11 action(s); route/source gate |
