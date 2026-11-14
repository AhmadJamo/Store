# MVC retirement dependency inventory

> Generated from source by `scripts/generate-mvc-migration-registry.ps1`. Do not edit inventory rows manually.
> MOD-030F created the non-destructive baseline. MOD-030G and MOD-030H record the separately approved physical retirement cohorts.

## Decision

Physical MVC retirement must follow feature-sized, reversible cohorts. A Razor view on a mixed controller remains required until every retained MVC mutation that can redisplay validation errors has an accepted Angular/API replacement. Downloads, authentication/session routing, public token flows and the Language POST compatibility endpoint are independent boundaries, not presentation leftovers.

## Reconciliation

- MVC controllers: 45.
- MVC actions classified: 263 of 263.
- Angular-default presentation actions eligible for cohort review: 76.
- Retained MVC mutation actions: 151.
- Retained download/generated-file actions: 8.
- Retained compatibility, ambiguous-verb, session or non-cutover actions: 28.
- Razor files discovered: 143; shared infrastructure files: 11.
- Presentation-only pilot surfaces: 0, including 0 anonymous public boundary.
- Mixed-controller surfaces blocked from early view deletion: 25.
- Physically retired presentation actions with legacy redirects: 72 across 37 surfaces.

The exact action and Razor paths remain in `MVC_MIGRATION_REGISTRY.md`; this file adds retirement dependency classification and cohort gates without duplicating that source inventory.

## Cohort rules

| Cohort | Meaning | Deletion gate |
|---|---|---|
| R1 | Presentation-only controller surface with every discovered action covered by Angular-default cutover. | Owner approval, exact file/reference proof, MVC-mode retirement decision, Release/regression/browser evidence and deployment rollback plan. |
| R1-A | Anonymous presentation-only surface with custom legacy paths or identity-adjacent behavior. | All R1 gates plus explicit anonymous route patterns, query preservation, isolation from login/registration/session/rate-limit behavior and independent anonymous browser evidence. |
| R2 | Mixed presentation plus MVC mutation/download/session behavior. | Retire command/download dependencies first; keep Razor views while any retained path can render them. |
| R3 | No presentation candidate; MVC still owns a route, session decision, ambiguous verb or other compatibility behavior. | Explicit replacement or permanent-boundary decision before deletion. |
| R4 | Declared mutation-only compatibility surface. | Preserve unless a separately approved architecture change replaces it. |

## Completed physical retirement cohorts

Physical retirement cohorts through MOD-030Z removed the initial inventory, accounting, public, analytics, recipe, catalog, approval-rule and first ten-controller presentations. MOD-031B then retired LocationMovements, StockTransactions, UnassignedStock, InventoryAdjustments, InventoryTracking, WarehouseLocations, Warehouses, StockTransfers, GoodsReceipts and PurchaseDocumentReversals as one inventory/warehouse cohort. Their MVC controllers, Razor views and reversible cutover entries are gone; protected GET/HEAD routes retain permission policies, while exact `/` and `/pricing` redirects are anonymous. Mapped redirects preserve queries; retired POST/download actions are not redirected.

| Retired MVC surface | Angular destination | Required permission |
|---|---|---|
| AnalyticsCenter | /analytics | AnalyticsCenter.View |
| AnalyticsCenter | /analytics/builder | AnalyticsCenter.Customize |
| AuditEvents | /audit-events | AuditEvents.View |
| CustomerPayments | /sales/payments/{id} | CustomerPayments.View |
| CustomerPayments | /sales/payments/new?customerInvoiceId={customerInvoiceId} | CustomerPayments.Create |
| CustomerPayments | /sales/payments | CustomerPayments.View |
| GoodsReceiptReturns | /purchases/receipt-returns/new?goodsReceiptId={goodsReceiptId} | GoodsReceiptReturns.Create |
| GoodsReceiptReturns | /purchases/receipt-returns/{id} | GoodsReceiptReturns.View |
| GoodsReceiptReturns | /purchases/receipt-returns | GoodsReceiptReturns.View |
| GoodsReceipts | /purchases/receipts/{id}/scan | GoodsReceipts.Create |
| GoodsReceipts | /purchases/receipts/{id} | GoodsReceipts.View |
| GoodsReceipts | /purchases/receipts/new?purchaseOrderId={purchaseOrderId} | GoodsReceipts.Create |
| GoodsReceipts | /purchases/receipts | GoodsReceipts.View |
| InventoryAdjustments | /inventory/adjustments | InventoryAdjustments.View |
| InventoryAdjustments | /inventory/adjustments/new | InventoryAdjustments.Create |
| InventoryAdjustments | /inventory/adjustments/{id} | InventoryAdjustments.View |
| InventoryBalances | /inventory/availability | InventoryBalances.View |
| InventoryInsights | /inventory/insights | InventoryInsights.View |
| InventoryReconciliation | /inventory/reconciliation | InventoryReconciliation.View |
| InventoryReservations | /inventory/reservations | InventoryReservations.View |
| InventoryScanning | /inventory/scanning | InventoryScanning.View |
| InventoryTracking | /inventory/tracking | InventoryTracking.View |
| JournalEntries | /accounting/journal-entries/{id} | JournalEntries.View |
| LocationMovements | /inventory/location-movements | LocationMovements.View |
| ProductAttributes | /products/{productId}/attributes | Products.Edit |
| ProductAttributes | /products/attributes | Products.View |
| ProductCategories | /products/categories | Products.View |
| ProductStocks | /inventory/stocks/new | ProductStock.Create |
| ProductStocks | /inventory/stocks | ProductStock.View |
| ProductStocks | /inventory/stocks/{id}/edit | ProductStock.Edit |
| ProductTemplates | /products/templates | Products.View |
| Public | /pricing | Anonymous |
| Public | /welcome | Anonymous |
| PurchaseAccountingReconciliation | /purchases/accounting-reconciliation | PurchaseAccountingReconciliation.View |
| PurchaseApprovalRules | /purchases/approval-rules | PurchaseApprovalRules.View |
| PurchaseDocumentReversals | /purchases/reversals/new | PurchaseDocumentReversals.Create |
| PurchaseDocumentReversals | /purchases/reversals/{id} | PurchaseDocumentReversals.View |
| PurchaseDocumentReversals | /purchases/reversals | PurchaseDocumentReversals.View |
| PurchaseReplenishment | /purchases/replenishment | PurchaseReplenishment.View |
| PurchaseServiceAcceptances | /purchases/service-acceptances | PurchaseServiceAcceptances.View |
| PurchaseServiceAcceptances | /purchases/service-acceptances/new?purchaseOrderId={purchaseOrderId} | PurchaseServiceAcceptances.Create |
| PurchaseServiceAcceptances | /purchases/service-acceptances/{id} | PurchaseServiceAcceptances.View |
| Recipes | /products/recipes | Products.View |
| Recipes | /products/recipes/{productId} | Products.Edit |
| Replenishment | /inventory/replenishment | Inventory.Replenishment.View |
| SalesApprovalRules | /sales/approval-rules | SalesApprovalRules.View |
| SalesDeliveries | /sales/deliveries/new?salesOrderId={salesOrderId} | SalesDeliveries.Create |
| SalesDeliveries | /sales/deliveries | SalesDeliveries.View |
| SalesDeliveries | /sales/deliveries/{id} | SalesDeliveries.View |
| StockMovements | /inventory/location-movements | StockMovements.View |
| StockTransactions | /inventory/stocks/adjustment | StockTransactions.Create |
| StockTransactions | /inventory/transactions | StockTransactions.View |
| StockTransfers | /inventory/transfers/{id} | StockTransfers.View |
| StockTransfers | /inventory/transfers/{id}/edit | StockTransfers.Edit |
| StockTransfers | /inventory/transfers | StockTransfers.View |
| StockTransfers | /inventory/transfers/new | StockTransfers.Create |
| SupplierCreditNotes | /purchases/credit-notes | SupplierCreditNotes.View |
| SupplierCreditNotes | /purchases/credit-notes/{id} | SupplierCreditNotes.View |
| SupplierCreditNotes | /purchases/credit-notes/new?vendorBillId={vendorBillId} | SupplierCreditNotes.Create |
| SupplierPayments | /purchases/payments/new | SupplierPayments.Create |
| SupplierPayments | /purchases/payments | SupplierPayments.View |
| SupplierPayments | /purchases/payments/{id} | SupplierPayments.View |
| Suppliers | /suppliers/new | Suppliers.Create |
| Suppliers | /suppliers | Suppliers.View |
| Suppliers | /suppliers/{id}/edit | Suppliers.Edit |
| UnassignedStock | /inventory/putaway | ProductStock.View |
| WarehouseLocations | /inventory/locations | Warehouses.Create |
| WarehouseLocations | /inventory/locations | Warehouses.View |
| WarehouseLocations | /inventory/locations | Warehouses.Edit |
| Warehouses | /warehouses | Warehouses.View |
| Warehouses | /warehouses | Warehouses.Create |
| Warehouses | /warehouses | Warehouses.Edit |

## Surface inventory

| MVC surface | Presentation candidates | Mutations retained | Downloads retained | Other boundaries retained | Razor files | Cohort |
|---|---:|---:|---:|---:|---:|---|
| Account | 3 | 3 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| Accounts | 0 | 1 | 0 | 2 | 2 | R3 - retained MVC boundary; API/session decision required |
| Branches | 0 | 0 | 0 | 1 | 2 | R3 - retained MVC boundary; API/session decision required |
| CompanyOnboarding | 1 | 2 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| CustomerCreditNotes | 0 | 1 | 0 | 0 | 3 | R3 - retained MVC boundary; API/session decision required |
| CustomerInvoices | 3 | 5 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| CustomerReturnReceipts | 0 | 1 | 0 | 2 | 3 | R3 - retained MVC boundary; API/session decision required |
| CustomerReturnRequests | 3 | 5 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| Customers | 4 | 10 | 0 | 0 | 5 | R2 - mixed controller; retain views until commands/downloads retire |
| Dashboard | 0 | 0 | 0 | 1 | 1 | R3 - retained MVC boundary; API/session decision required |
| DocumentDesigns | 3 | 4 | 0 | 0 | 4 | R2 - mixed controller; retain views until commands/downloads retire |
| Home | 0 | 0 | 0 | 1 | 1 | R3 - retained MVC boundary; API/session decision required |
| LandedCosts | 0 | 1 | 0 | 0 | 3 | R3 - retained MVC boundary; API/session decision required |
| Language | 0 | 0 | 0 | 1 | 0 | R4 - permanent compatibility review |
| PaymentMethods | 0 | 1 | 0 | 2 | 2 | R3 - retained MVC boundary; API/session decision required |
| Platform/Platform | 7 | 4 | 0 | 0 | 7 | R2 - mixed controller; retain views until commands/downloads retire |
| Platform/PlatformAccount | 1 | 2 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Platform/PlatformAuditEvents | 1 | 0 | 1 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Platform/PlatformAuditRetention | 1 | 4 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Products | 2 | 3 | 0 | 1 | 4 | R2 - mixed controller; retain views until commands/downloads retire |
| PurchaseAgreements | 0 | 4 | 0 | 0 | 3 | R3 - retained MVC boundary; API/session decision required |
| PurchaseAnalytics | 0 | 0 | 0 | 1 | 1 | R3 - retained MVC boundary; API/session decision required |
| PurchaseOrders | 0 | 10 | 0 | 1 | 2 | R3 - retained MVC boundary; API/session decision required |
| PurchaseRequests | 1 | 5 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| PurchaseReturns | 0 | 1 | 0 | 3 | 3 | R3 - retained MVC boundary; API/session decision required |
| Purchases | 2 | 2 | 0 | 1 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| PurchaseSourcing | 3 | 2 | 1 | 2 | 5 | R2 - mixed controller; retain views until commands/downloads retire |
| PurchaseTemplates | 0 | 3 | 0 | 0 | 1 | R3 - retained MVC boundary; API/session decision required |
| Roles | 3 | 3 | 0 | 0 | 4 | R2 - mixed controller; retain views until commands/downloads retire |
| Sales | 1 | 2 | 0 | 3 | 4 | R2 - mixed controller; retain views until commands/downloads retire |
| SalesOrders | 3 | 7 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| SalesPriceLists | 1 | 5 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| SalesQuotations | 3 | 6 | 1 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| SalesQuotationTemplates | 3 | 4 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| SalesReturns | 0 | 1 | 0 | 3 | 3 | R3 - retained MVC boundary; API/session decision required |
| Settings | 17 | 18 | 1 | 0 | 17 | R2 - mixed controller; retain views until commands/downloads retire |
| Subscription | 1 | 1 | 0 | 0 | 2 | R2 - mixed controller; retain views until commands/downloads retire |
| SupplierCommunications | 0 | 2 | 0 | 0 | 1 | R3 - retained MVC boundary; API/session decision required |
| SupplierIntegrations | 0 | 6 | 1 | 0 | 4 | R3 - retained MVC boundary; API/session decision required |
| SupplierPortal | 0 | 3 | 0 | 1 | 2 | R3 - retained MVC boundary; API/session decision required |
| SupplierPurchasing | 4 | 8 | 2 | 0 | 5 | R2 - mixed controller; retain views until commands/downloads retire |
| SupplierQuotations | 0 | 2 | 0 | 0 | 2 | R3 - retained MVC boundary; API/session decision required |
| TaxRates | 0 | 1 | 0 | 2 | 2 | R3 - retained MVC boundary; API/session decision required |
| Users | 2 | 1 | 0 | 0 | 2 | R2 - mixed controller; retain views until commands/downloads retire |
| VendorBills | 3 | 7 | 1 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |

## R2 prioritization signals

This is a dependency-count signal, not deletion approval. Score = mutations × 3 + downloads × 2 + other boundaries × 4 + Razor files. Business criticality, session isolation, validation recovery and live evidence can override the numeric order.

| Priority | MVC surface | Score | Presentation candidates | Mutations | Downloads | Other boundaries | Razor files |
|---:|---|---:|---:|---:|---:|---:|---:|
| 1 | Platform/PlatformAuditEvents | 3 | 1 | 0 | 1 | 0 | 1 |
| 2 | Subscription | 5 | 1 | 1 | 0 | 0 | 2 |
| 3 | Users | 5 | 2 | 1 | 0 | 0 | 2 |
| 4 | CompanyOnboarding | 7 | 1 | 2 | 0 | 0 | 1 |
| 5 | Platform/PlatformAccount | 7 | 1 | 2 | 0 | 0 | 1 |
| 6 | Account | 12 | 3 | 3 | 0 | 0 | 3 |
| 7 | Purchases | 13 | 2 | 2 | 0 | 1 | 3 |
| 8 | Roles | 13 | 3 | 3 | 0 | 0 | 4 |
| 9 | Platform/PlatformAuditRetention | 13 | 1 | 4 | 0 | 0 | 1 |
| 10 | SalesQuotationTemplates | 15 | 3 | 4 | 0 | 0 | 3 |
| 11 | DocumentDesigns | 16 | 3 | 4 | 0 | 0 | 4 |
| 12 | SalesPriceLists | 16 | 1 | 5 | 0 | 0 | 1 |
| 13 | Products | 17 | 2 | 3 | 0 | 1 | 4 |
| 14 | CustomerInvoices | 18 | 3 | 5 | 0 | 0 | 3 |
| 15 | CustomerReturnRequests | 18 | 3 | 5 | 0 | 0 | 3 |
| 16 | PurchaseRequests | 18 | 1 | 5 | 0 | 0 | 3 |
| 17 | Platform/Platform | 19 | 7 | 4 | 0 | 0 | 7 |
| 18 | PurchaseSourcing | 21 | 3 | 2 | 1 | 2 | 5 |
| 19 | Sales | 22 | 1 | 2 | 0 | 3 | 4 |
| 20 | SalesQuotations | 23 | 3 | 6 | 1 | 0 | 3 |
| 21 | SalesOrders | 24 | 3 | 7 | 0 | 0 | 3 |
| 22 | VendorBills | 26 | 3 | 7 | 1 | 0 | 3 |
| 23 | SupplierPurchasing | 33 | 4 | 8 | 2 | 0 | 5 |
| 24 | Customers | 35 | 4 | 10 | 0 | 0 | 5 |
| 25 | Settings | 73 | 17 | 18 | 1 | 0 | 17 |

## Retained action dependencies

| MVC surface | Action | Verb | Reason | Source |
|---|---|---|---|---|
| Account | Login | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Logout | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Register | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/AccountController.cs |
| Accounts | Create | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Accounting/AccountsController.cs |
| Accounts | Create | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Accounting/AccountsController.cs |
| Accounts | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Accounting/AccountsController.cs |
| Branches | BranchesController | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Accounting/BranchesController.cs |
| CompanyOnboarding | Complete | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs |
| CompanyOnboarding | Skip | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs |
| CustomerCreditNotes | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerCreditNotesController.cs |
| CustomerInvoices | Approve | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Cancel | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Post | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Reverse | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerReturnReceipts | RedirectToAction | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Sales/CustomerReturnReceiptsController.cs |
| CustomerReturnReceipts | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerReturnReceiptsController.cs |
| CustomerReturnReceipts | View | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Sales/CustomerReturnReceiptsController.cs |
| CustomerReturnRequests | Approve | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| CustomerReturnRequests | Cancel | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| CustomerReturnRequests | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| CustomerReturnRequests | Reject | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| CustomerReturnRequests | Submit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| Customers | Archive | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | ArchiveAddress | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | ArchiveContact | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | Edit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | Restore | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | RestoreAddress | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | RestoreContact | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | SaveAddress | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | SaveContact | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Dashboard | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Home/DashboardController.cs |
| DocumentDesigns | Activate | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs |
| DocumentDesigns | Archive | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs |
| DocumentDesigns | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs |
| DocumentDesigns | SetDefault | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs |
| Home | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Home/HomeController.cs |
| LandedCosts | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/LandedCostsController.cs |
| Language | Set | POST | Retain - explicit compatibility boundary | MiniStore.Web/Controllers/Settings/LanguageController.cs |
| PaymentMethods | Create | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Accounting/PaymentMethodsController.cs |
| PaymentMethods | Create | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Accounting/PaymentMethodsController.cs |
| PaymentMethods | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Accounting/PaymentMethodsController.cs |
| Platform/Platform | ConfirmPayment | GET, POST | Retain - MVC mutation | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | CreatePlan | GET, POST | Retain - MVC mutation | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | CreatePromotion | GET, POST | Retain - MVC mutation | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | UpdateCompany | GET, POST | Retain - MVC mutation | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/PlatformAccount | Login | POST | Retain - MVC mutation | MiniStore.Web/Areas/Platform/Controllers/PlatformAccountController.cs |
| Platform/PlatformAccount | Logout | POST | Retain - MVC mutation | MiniStore.Web/Areas/Platform/Controllers/PlatformAccountController.cs |
| Platform/PlatformAuditEvents | Export | POST | Retain - download or generated file | MiniStore.Web/Areas/Platform/Controllers/PlatformAuditEventsController.cs |
| Platform/PlatformAuditRetention | Checkpoint | POST | Retain - MVC mutation | MiniStore.Web/Areas/Platform/Controllers/PlatformAuditRetentionController.cs |
| Platform/PlatformAuditRetention | CreateHold | POST | Retain - MVC mutation | MiniStore.Web/Areas/Platform/Controllers/PlatformAuditRetentionController.cs |
| Platform/PlatformAuditRetention | ReleaseHold | POST | Retain - MVC mutation | MiniStore.Web/Areas/Platform/Controllers/PlatformAuditRetentionController.cs |
| Platform/PlatformAuditRetention | Run | POST | Retain - MVC mutation | MiniStore.Web/Areas/Platform/Controllers/PlatformAuditRetentionController.cs |
| Products | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductsController.cs |
| Products | Delete | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductsController.cs |
| Products | Edit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductsController.cs |
| Products | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Catalog/ProductsController.cs |
| PurchaseAgreements | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseAgreementsController.cs |
| PurchaseAgreements | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseAgreementsController.cs |
| PurchaseAgreements | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseAgreementsController.cs |
| PurchaseAgreements | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseAgreementsController.cs |
| PurchaseAnalytics | View | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Purchases/PurchaseAnalyticsController.cs |
| PurchaseOrders | Details | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseOrdersController.cs |
| PurchaseOrders | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseOrdersController.cs |
| PurchaseOrders | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseOrdersController.cs |
| PurchaseOrders | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseOrdersController.cs |
| PurchaseOrders | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseOrdersController.cs |
| PurchaseOrders | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseOrdersController.cs |
| PurchaseOrders | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseOrdersController.cs |
| PurchaseOrders | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseOrdersController.cs |
| PurchaseOrders | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseOrdersController.cs |
| PurchaseOrders | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseOrdersController.cs |
| PurchaseOrders | View | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Purchases/PurchaseOrdersController.cs |
| PurchaseRequests | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseRequestsController.cs |
| PurchaseRequests | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseRequestsController.cs |
| PurchaseRequests | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseRequestsController.cs |
| PurchaseRequests | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseRequestsController.cs |
| PurchaseRequests | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseRequestsController.cs |
| PurchaseReturns | Create | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Purchases/PurchaseReturnsController.cs |
| PurchaseReturns | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseReturnsController.cs |
| PurchaseReturns | Details | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Purchases/PurchaseReturnsController.cs |
| PurchaseReturns | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Purchases/PurchaseReturnsController.cs |
| Purchases | Create | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Purchases/PurchasesController.cs |
| Purchases | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchasesController.cs |
| Purchases | Post | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchasesController.cs |
| PurchaseSourcing | Attachment | GET | Retain - download or generated file | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Create | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Direct | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Direct | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseTemplates | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseTemplatesController.cs |
| PurchaseTemplates | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseTemplatesController.cs |
| PurchaseTemplates | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseTemplatesController.cs |
| Roles | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Delete | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Edit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/RolesController.cs |
| Sales | Create | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Details | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Post | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesController.cs |
| SalesOrders | ApproveException | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | Cancel | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | Confirm | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | CreateFromQuotation | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | RejectException | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | ReleaseReservation | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | Reserve | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesPriceLists | AddRule | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs |
| SalesPriceLists | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs |
| SalesPriceLists | Evaluate | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs |
| SalesPriceLists | SetListActive | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs |
| SalesPriceLists | SetRuleActive | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs |
| SalesQuotations | Accept | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Cancel | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | DownloadPdf | GET | Retain - download or generated file | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Expire | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Reject | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Send | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotationTemplates | Activate | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs |
| SalesQuotationTemplates | Archive | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs |
| SalesQuotationTemplates | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs |
| SalesQuotationTemplates | SetDefault | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs |
| SalesReturns | Create | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Sales/SalesReturnsController.cs |
| SalesReturns | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesReturnsController.cs |
| SalesReturns | Details | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Sales/SalesReturnsController.cs |
| SalesReturns | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Sales/SalesReturnsController.cs |
| Settings | Accounting | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | BranchSalesAccounts | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | ChangeFiscalPeriodStatus | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | ConfigureBranchWarehouse | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | ConfigurePosTerminalWarehouse | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | CreatePosTerminal | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Currencies | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | DeactivateUnit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Discounts | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | DocumentNumbers | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | FiscalPeriods | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | General | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Inventory | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | OrganizationIdentity | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | OrganizationLogo | GET | Retain - download or generated file | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Pos | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | PurchaseMatching | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | PurchaseSourcingPolicy | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Units | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Subscription | Checkout | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Saas/SubscriptionController.cs |
| SupplierCommunications | RedirectToAction | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierCommunicationsController.cs |
| SupplierCommunications | RedirectToAction | POST, GET | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierCommunicationsController.cs |
| SupplierIntegrations | File | GET | Retain - download or generated file | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | RedirectToAction | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | RedirectToAction | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | RedirectToAction | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | RedirectToDocument | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | View | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | View | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierPortal | RecordDenied | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Purchases/SupplierPortalController.cs |
| SupplierPortal | RecordDenied | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierPortalController.cs |
| SupplierPortal | RecordDenied | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierPortalController.cs |
| SupplierPortal | RecordDenied | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierPortalController.cs |
| SupplierPurchasing | ExportCsv | GET | Retain - download or generated file | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | ExportExcel | GET | Retain - download or generated file | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | ImportCsv | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | ImportExcel | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | PreviewCsv | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | PreviewExcel | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | Save | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | SaveBulk | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | SavePriceBreaks | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | SetActive | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierQuotations | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierQuotationsController.cs |
| SupplierQuotations | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierQuotationsController.cs |
| TaxRates | Create | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Accounting/TaxRatesController.cs |
| TaxRates | Create | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Accounting/TaxRatesController.cs |
| TaxRates | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Accounting/TaxRatesController.cs |
| Users | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/UsersController.cs |
| VendorBills | Approve | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | ApproveOverride | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | Cancel | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | DownloadAttachment | GET | Retain - download or generated file | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | Post | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | RedirectToAction | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | RedirectToAction | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |

## Mandatory batch checklist

1. Name the exact controller actions, Razor files and shared references in the proposed batch.
2. Prove Angular/API functional parity for reads, validation recovery, commands, downloads and permissions that the batch owns.
3. Search runtime, Razor, tests, navigation and documentation references; the batch must leave no dangling route or view return.
4. Decide how `PresentationCutover` and MVC rollback behave after deletion; do not leave a configuration switch that points to removed code.
5. Obtain explicit owner approval for that bounded deletion batch.
6. Run Release, regression, generated-inventory, Angular and authenticated browser acceptance, then record deployment rollback evidence.

## Current outcome

Retirement cohorts through MOD-031B are complete. Seventy-two MVC presentation GET actions across thirty-seven surfaces and seventy-two Razor views are physically retired. Public exact `/` and `/pricing` routes are anonymous; other retired GET/HEAD routes retain their original permission policies and preserve queries. Retired MVC POST/download actions are not redirected or replayed; Angular continues through authenticated, permission-protected and antiforgery-protected APIs. No additional deletion is approved outside the next named ten-controller audit.
