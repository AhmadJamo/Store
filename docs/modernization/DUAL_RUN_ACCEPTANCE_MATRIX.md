# Dual-run acceptance matrix

> Generated from source by scripts/generate-mvc-migration-registry.ps1. Do not edit rows manually.

- Counterparts covered: 82.
- Presentation default: Mixed (Customers AngularDefault; all other surfaces MVC).
- Evidence baseline: generated MVC view/action inventory, verified Angular route, controller permission references and acceptance gate.
- Runtime dual-run samples and cutover rehearsal results are appended by the MOD-029 acceptance workflow; this static matrix performs no mutation.

| MVC surface | MVC route family | Angular counterpart | Operation class | Permission boundary references | Risk class | Static evidence |
|---|---|---|---|---|---|---|
| Account | /Account | /login, /register, /access-denied | Read/write | Public/session boundary | Identity/tenant | 3 view(s); 6 action(s); route/source gate |
| Accounts | /Accounts | /accounting/accounts | Read/write | Administration.Access | Financial | 2 view(s); 3 action(s); route/source gate |
| AnalyticsCenter | /AnalyticsCenter | /analytics | Read/write | AnalyticsCenter.Customize, AnalyticsCenter.View | Standard | 2 view(s); 3 action(s); route/source gate |
| AuditEvents | /AuditEvents | /audit-events | Read/write | AuditEvents.Export, AuditEvents.View | Standard | 1 view(s); 2 action(s); route/source gate |
| Branches | /Branches | /products/catalog-masters | Read/write | Administration.Access | Standard | 2 view(s); 1 action(s); route/source gate |
| CompanyOnboarding | /CompanyOnboarding | /onboarding | Read/write | Administration.Access | Identity/tenant | 1 view(s); 3 action(s); route/source gate |
| CustomerCreditNotes | /CustomerCreditNotes | /sales/credit-notes | Read/write | CustomerCreditNotes.Create, CustomerCreditNotes.View | Financial | 3 view(s); 1 action(s); route/source gate |
| CustomerInvoices | /CustomerInvoices | /sales/invoices | Read/write | CustomerInvoices.Approve, CustomerInvoices.Cancel, CustomerInvoices.Create, CustomerInvoices.Post, CustomerInvoices.Reverse, CustomerInvoices.View | Financial | 3 view(s); 8 action(s); route/source gate |
| CustomerPayments | /CustomerPayments | /sales/payments | Read/write | CustomerPayments.Create, CustomerPayments.Reverse, CustomerPayments.View | Financial | 3 view(s); 5 action(s); route/source gate |
| CustomerReturnReceipts | /CustomerReturnReceipts | /sales/return-receipts | Read/write | CustomerReturnReceipts.Cancel, CustomerReturnReceipts.Create, CustomerReturnReceipts.Inspect, CustomerReturnReceipts.PostDisposition, CustomerReturnReceipts.View | Standard | 3 view(s); 3 action(s); route/source gate |
| CustomerReturnRequests | /CustomerReturnRequests | /sales/return-requests | Read/write | CustomerReturnRequests.Approve, CustomerReturnRequests.Cancel, CustomerReturnRequests.Create, CustomerReturnRequests.Reject, CustomerReturnRequests.Submit, CustomerReturnRequests.View | Standard | 3 view(s); 8 action(s); route/source gate |
| Customers | /Customers | /customers | Read/write | Customers.Archive, Customers.Create, Customers.Edit, Customers.View | Standard | 4 view(s); 14 action(s); route/source gate |
| Dashboard | /Dashboard | / | Read/write | Public/session boundary | Standard | 1 view(s); 1 action(s); route/source gate |
| DocumentDesigns | /DocumentDesigns | /settings/document-designs | Read/write | Administration.Access | Standard | 3 view(s); 7 action(s); route/source gate |
| GoodsReceiptReturns | /GoodsReceiptReturns | /purchases/receipt-returns | Read/write | GoodsReceiptReturns.Create, GoodsReceiptReturns.View | Standard | 3 view(s); 4 action(s); route/source gate |
| GoodsReceipts | /GoodsReceipts | /purchases/receipts | Read/write | GoodsReceipts.Create, GoodsReceipts.Inspect, GoodsReceipts.PutAway, GoodsReceipts.View | Standard | 4 view(s); 10 action(s); route/source gate |
| Home | / | / | Read/write | Public/session boundary | Standard | 1 view(s); 1 action(s); route/source gate |
| InventoryAdjustments | /InventoryAdjustments | /inventory/adjustments | Read/write | InventoryAdjustments.Approve, InventoryAdjustments.Cancel, InventoryAdjustments.Count, InventoryAdjustments.Create, InventoryAdjustments.Post, InventoryAdjustments.View | Inventory | 3 view(s); 8 action(s); route/source gate |
| InventoryBalances | /InventoryBalances | /inventory/availability | Read-only | InventoryBalances.View | Inventory | 1 view(s); 1 action(s); route/source gate |
| InventoryInsights | /InventoryInsights | /inventory/insights | Read-only | InventoryInsights.View | Inventory | 1 view(s); 1 action(s); route/source gate |
| InventoryReconciliation | /InventoryReconciliation | /inventory/reconciliation | Read-only | InventoryReconciliation.View | Inventory | 1 view(s); 1 action(s); route/source gate |
| InventoryReservations | /InventoryReservations | /inventory/reservations | Read-only | InventoryReservations.View | Inventory | 1 view(s); 1 action(s); route/source gate |
| InventoryScanning | /InventoryScanning | /inventory/scanning | Read/write | InventoryAdjustments.Count, InventoryScanning.View, LocationMovements.Create, ProductStock.Edit | Inventory | 1 view(s); 4 action(s); route/source gate |
| InventoryTracking | /InventoryTracking | /inventory/tracking | Read/write | InventoryTracking.ManageQuarantine, InventoryTracking.ManageRecall, InventoryTracking.Open, InventoryTracking.View | Inventory | 1 view(s); 7 action(s); route/source gate |
| JournalEntries | /JournalEntries | /accounting/journal-entries/:id | Read-only | JournalEntries.View | Financial | 1 view(s); 1 action(s); route/source gate |
| LandedCosts | /LandedCosts | /purchases/landed-costs | Read/write | LandedCosts.Create, LandedCosts.View | Financial | 3 view(s); 1 action(s); route/source gate |
| Language | /Language/Set | / | Read/write | Public/session boundary | Identity/tenant | 0 view(s); 1 action(s); route/source gate |
| LocationMovements | /LocationMovements | /inventory/location-movements | Read/write | LocationMovements.Create, LocationMovements.View | Inventory | 1 view(s); 2 action(s); route/source gate |
| PaymentMethods | /PaymentMethods | /products/catalog-masters | Read/write | Administration.Access | Financial | 2 view(s); 3 action(s); route/source gate |
| Platform/Platform | /platform | /platform-control, /platform-management | Read/write | Public/session boundary | Identity/tenant | 7 view(s); 11 action(s); route/source gate |
| Platform/PlatformAccount | /platform/account | /platform-login | Read/write | Public/session boundary | Identity/tenant | 1 view(s); 3 action(s); route/source gate |
| Platform/PlatformAuditEvents | /platform/audit-events | /platform-audit | Read/write | Public/session boundary | Identity/tenant | 1 view(s); 2 action(s); route/source gate |
| Platform/PlatformAuditRetention | /platform/audit-retention | /platform-retention | Read/write | Public/session boundary | Identity/tenant | 1 view(s); 5 action(s); route/source gate |
| ProductAttributes | /ProductAttributes | /products/attributes | Read/write | Products.Edit, Products.View | Standard | 2 view(s); 5 action(s); route/source gate |
| ProductCategories | /ProductCategories | /products/categories | Read/write | Products.Edit, Products.View | Standard | 1 view(s); 3 action(s); route/source gate |
| Products | /Products | /products | Read/write | Products.Create, Products.Delete, Products.Edit, Products.View | Standard | 3 view(s); 6 action(s); route/source gate |
| ProductStocks | /ProductStocks | /inventory/stocks | Read/write | ProductStock.Create, ProductStock.Delete, ProductStock.Edit, ProductStock.View | Inventory | 3 view(s); 6 action(s); route/source gate |
| ProductTemplates | /ProductTemplates | /products/templates | Read/write | Products.Edit, Products.View | Standard | 1 view(s); 5 action(s); route/source gate |
| Public | / | /welcome, /pricing | Read-only | Public/session boundary | Standard | 2 view(s); 2 action(s); route/source gate |
| PurchaseAccountingReconciliation | /PurchaseAccountingReconciliation | /purchases/accounting-reconciliation | Read-only | PurchaseAccountingReconciliation.View | Financial | 1 view(s); 1 action(s); route/source gate |
| PurchaseAgreements | /PurchaseAgreements | /purchases/agreements | Read/write | PurchaseAgreements.Activate, PurchaseAgreements.Cancel, PurchaseAgreements.Create, PurchaseAgreements.Release, PurchaseAgreements.View | Standard | 3 view(s); 4 action(s); route/source gate |
| PurchaseAnalytics | /PurchaseAnalytics | /purchases/analytics | Read-only | PurchaseAnalytics.View | Standard | 1 view(s); 1 action(s); route/source gate |
| PurchaseApprovalRules | /PurchaseApprovalRules | /purchases/approval-rules | Read/write | PurchaseApprovalRules.Manage, PurchaseApprovalRules.View | Standard | 1 view(s); 3 action(s); route/source gate |
| PurchaseDocumentReversals | /PurchaseDocumentReversals | /purchases/reversals | Read/write | PurchaseDocumentReversals.Create, PurchaseDocumentReversals.View | Financial | 3 view(s); 4 action(s); route/source gate |
| PurchaseOrders | /PurchaseOrders | /purchases/orders | Read/write | PurchaseOrders.Approve, PurchaseOrders.Cancel, PurchaseOrders.Close, PurchaseOrders.ConfigureFulfillment, PurchaseOrders.Confirm, PurchaseOrders.Create, PurchaseOrders.RecordDirectDelivery, PurchaseOrders.RecordSupplierConfirmation, PurchaseOrders.Reopen, PurchaseOrders.View | Standard | 2 view(s); 11 action(s); route/source gate |
| PurchaseReplenishment | /PurchaseReplenishment | /purchases/replenishment | Read/write | PurchaseReplenishment.Manage, PurchaseReplenishment.Run, PurchaseReplenishment.View | Inventory | 1 view(s); 3 action(s); route/source gate |
| PurchaseRequests | /PurchaseRequests | /purchases/requests | Read/write | PurchaseRequests.Approve, PurchaseRequests.Cancel, PurchaseRequests.Create, PurchaseRequests.Reject, PurchaseRequests.Submit, PurchaseRequests.View | Standard | 3 view(s); 6 action(s); route/source gate |
| PurchaseReturns | /PurchaseReturns | /purchases/legacy-returns | Read/write | PurchaseReturns.Create, PurchaseReturns.View | Standard | 3 view(s); 4 action(s); route/source gate |
| Purchases | /Purchases | /purchases/legacy | Read/write | Purchases.Create, Purchases.View | Standard | 3 view(s); 5 action(s); route/source gate |
| PurchaseServiceAcceptances | /PurchaseServiceAcceptances | /purchases/service-acceptances | Read/write | PurchaseServiceAcceptances.Create, PurchaseServiceAcceptances.View | Standard | 3 view(s); 4 action(s); route/source gate |
| PurchaseSourcing | /PurchaseSourcing | /purchases/sourcing | Read/write | PurchaseSourcing.Create, PurchaseSourcing.CreateDirect, PurchaseSourcing.View | Standard | 5 view(s); 8 action(s); route/source gate |
| PurchaseTemplates | /PurchaseTemplates | /purchases/templates | Read/write | PurchaseTemplates.Create, PurchaseTemplates.Generate, PurchaseTemplates.Manage, PurchaseTemplates.View | Standard | 1 view(s); 3 action(s); route/source gate |
| Recipes | /Recipes | /products/recipes | Read/write | Products.Edit, Products.View | Standard | 2 view(s); 3 action(s); route/source gate |
| Replenishment | /Replenishment | /purchases/replenishment | Read/write | Inventory.Replenishment.Manage, Inventory.Replenishment.View, PurchaseRequests.Create, StockTransfers.Create | Inventory | 1 view(s); 5 action(s); route/source gate |
| Roles | /Roles | /roles | Read/write | Roles.Create, Roles.Delete, Roles.Edit, Roles.View | Standard | 3 view(s); 6 action(s); route/source gate |
| Sales | /Sales | /sales/legacy | Read/write | Sales.Create, Sales.View | Standard | 4 view(s); 6 action(s); route/source gate |
| SalesApprovalRules | /SalesApprovalRules | /sales/approval-rules | Read/write | SalesApprovalRules.Manage, SalesApprovalRules.View | Standard | 1 view(s); 3 action(s); route/source gate |
| SalesDeliveries | /SalesDeliveries | /sales/deliveries | Read/write | SalesDeliveries.Cancel, SalesDeliveries.Create, SalesDeliveries.Post, SalesDeliveries.View | Standard | 3 view(s); 6 action(s); route/source gate |
| SalesOrders | /SalesOrders | /sales/orders | Read/write | SalesOrders.ApproveException, SalesOrders.Cancel, SalesOrders.Confirm, SalesOrders.Create, SalesOrders.ReleaseReservation, SalesOrders.Reserve, SalesOrders.View | Standard | 3 view(s); 10 action(s); route/source gate |
| SalesPriceLists | /SalesPriceLists | /sales/pricing | Read/write | SalesPricing.Manage, SalesPricing.View | Standard | 1 view(s); 6 action(s); route/source gate |
| SalesQuotations | /SalesQuotations | /sales/quotations | Read/write | SalesQuotations.Accept, SalesQuotations.Cancel, SalesQuotations.Create, SalesQuotations.Reject, SalesQuotations.Send, SalesQuotations.View | Standard | 3 view(s); 10 action(s); route/source gate |
| SalesQuotationTemplates | /SalesQuotationTemplates | /sales/quotation-templates | Read/write | SalesQuotationTemplates.Manage, SalesQuotationTemplates.View | Standard | 3 view(s); 7 action(s); route/source gate |
| SalesReturns | /SalesReturns | /sales/legacy-returns | Read/write | SalesReturns.Create, SalesReturns.View | Standard | 3 view(s); 4 action(s); route/source gate |
| Settings | /Settings | /settings | Read/write | Administration.Access | Standard | 16 view(s); 36 action(s); route/source gate |
| StockMovements | /StockMovements | /inventory/location-movements | Read-only | StockMovements.View | Inventory | 1 view(s); 1 action(s); route/source gate |
| StockTransactions | /StockTransactions | /inventory/transactions | Read/write | StockTransactions.Create, StockTransactions.View | Inventory | 2 view(s); 3 action(s); route/source gate |
| StockTransfers | /StockTransfers | /inventory/transfers | Read/write | StockTransfers.Approve, StockTransfers.Cancel, StockTransfers.Create, StockTransfers.Edit, StockTransfers.Post, StockTransfers.Reject, StockTransfers.Submit, StockTransfers.View | Inventory | 4 view(s); 12 action(s); route/source gate |
| Subscription | /Subscription | /subscription | Read/write | Public/session boundary | Identity/tenant | 2 view(s); 2 action(s); route/source gate |
| SupplierCommunications | /SupplierCommunications | /purchases/communications/prepare | Read/write | SupplierCommunications.Prepare, SupplierCommunications.RecordActivity, SupplierCommunications.View | Standard | 1 view(s); 2 action(s); route/source gate |
| SupplierCreditNotes | /SupplierCreditNotes | /purchases/credit-notes | Read/write | SupplierCreditNotes.Create, SupplierCreditNotes.View | Financial | 3 view(s); 4 action(s); route/source gate |
| SupplierIntegrations | /SupplierIntegrations | /purchases/integrations | Read/write | SupplierIntegrations.Manage, SupplierIntegrations.Queue, SupplierIntegrations.Retry, SupplierIntegrations.View | Standard | 4 view(s); 7 action(s); route/source gate |
| SupplierPayments | /SupplierPayments | /purchases/payments | Read/write | SupplierPayments.Create, SupplierPayments.View | Financial | 3 view(s); 4 action(s); route/source gate |
| SupplierPortal | /SupplierPortal | /supplier-portal/:token | Read/write | Public/session boundary | Standard | 2 view(s); 4 action(s); route/source gate |
| SupplierPurchasing | /SupplierPurchasing | /purchases/supplier-terms | Read/write | Purchases.SupplierTerms.Manage, Purchases.SupplierTerms.View | Standard | 5 view(s); 14 action(s); route/source gate |
| SupplierQuotations | /SupplierQuotations | /purchases/sourcing | Read/write | SupplierQuotations.Award, SupplierQuotations.Create, SupplierQuotations.View | Standard | 2 view(s); 2 action(s); route/source gate |
| Suppliers | /Suppliers | /suppliers | Read/write | Suppliers.Create, Suppliers.Delete, Suppliers.Edit, Suppliers.View | Standard | 3 view(s); 6 action(s); route/source gate |
| TaxRates | /TaxRates | /products/catalog-masters | Read/write | Administration.Access | Standard | 2 view(s); 3 action(s); route/source gate |
| UnassignedStock | /UnassignedStock | /inventory/putaway | Read/write | ProductStock.Edit, ProductStock.View | Inventory | 1 view(s); 4 action(s); route/source gate |
| Users | /Users | /users | Read/write | Users.Create, Users.View | Standard | 2 view(s); 3 action(s); route/source gate |
| VendorBills | /VendorBills | /purchases/bills | Read/write | VendorBills.Approve, VendorBills.Cancel, VendorBills.Create, VendorBills.ManageAttachments, VendorBills.OverrideMatch, VendorBills.Post, VendorBills.ReviewExtraction, VendorBills.View | Financial | 3 view(s); 11 action(s); route/source gate |
| WarehouseLocations | /WarehouseLocations | /inventory/locations | Read/write | Warehouses.Create, Warehouses.Edit, Warehouses.View | Inventory | 3 view(s); 5 action(s); route/source gate |
| Warehouses | /Warehouses | /warehouses | Read/write | Warehouses.Create, Warehouses.Delete, Warehouses.Edit, Warehouses.View | Inventory | 3 view(s); 6 action(s); route/source gate |
