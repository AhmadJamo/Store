# MVC retirement dependency inventory

> Generated from source by `scripts/generate-mvc-migration-registry.ps1`. Do not edit inventory rows manually.
> MOD-030F created the non-destructive baseline. MOD-030G and MOD-030H record the separately approved physical retirement cohorts.

## Decision

Physical MVC retirement must follow feature-sized, reversible cohorts. A Razor view on a mixed controller remains required until every retained MVC mutation that can redisplay validation errors has an accepted Angular/API replacement. Downloads, authentication/session routing, public token flows and the Language POST compatibility endpoint are independent boundaries, not presentation leftovers.

## Reconciliation

- MVC controllers: 75.
- MVC actions classified: 404 of 404.
- Angular-default presentation actions eligible for cohort review: 136.
- Retained MVC mutation actions: 226.
- Retained download/generated-file actions: 9.
- Retained compatibility, ambiguous-verb, session or non-cutover actions: 33.
- Razor files discovered: 210; shared infrastructure files: 11.
- Presentation-only pilot surfaces: 1, including 1 anonymous public boundary.
- Mixed-controller surfaces blocked from early view deletion: 54.
- Physically retired R1 surfaces with protected legacy redirects: 7.

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

MOD-030G physically retired five read-only inventory surfaces. MOD-030H then retired `JournalEntries.Details` and `PurchaseAccountingReconciliation.Index` after separate owner approval and API/Angular parity review. Their controller actions, Razor views and reversible cutover entries are gone; permission-protected GET/HEAD legacy redirects preserve exact actions, positive journal IDs and query strings. Anonymous `Public` remains the only structurally R1 surface and is not approved for deletion.

| Retired MVC surface | Angular destination | Required permission |
|---|---|---|
| InventoryBalances | /inventory/availability | InventoryBalances.View |
| InventoryInsights | /inventory/insights | InventoryInsights.View |
| InventoryReconciliation | /inventory/reconciliation | InventoryReconciliation.View |
| InventoryReservations | /inventory/reservations | InventoryReservations.View |
| JournalEntries | /accounting/journal-entries/{id} | JournalEntries.View |
| PurchaseAccountingReconciliation | /purchases/accounting-reconciliation | PurchaseAccountingReconciliation.View |
| StockMovements | /inventory/location-movements | StockMovements.View |

## Surface inventory

| MVC surface | Presentation candidates | Mutations retained | Downloads retained | Other boundaries retained | Razor files | Cohort |
|---|---:|---:|---:|---:|---:|---|
| Account | 3 | 3 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| Accounts | 0 | 1 | 0 | 2 | 2 | R3 - retained MVC boundary; API/session decision required |
| AnalyticsCenter | 2 | 1 | 0 | 0 | 2 | R2 - mixed controller; retain views until commands/downloads retire |
| AuditEvents | 1 | 0 | 1 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Branches | 0 | 0 | 0 | 1 | 2 | R3 - retained MVC boundary; API/session decision required |
| CompanyOnboarding | 1 | 2 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| CustomerCreditNotes | 0 | 1 | 0 | 0 | 3 | R3 - retained MVC boundary; API/session decision required |
| CustomerInvoices | 3 | 5 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| CustomerPayments | 3 | 2 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| CustomerReturnReceipts | 0 | 1 | 0 | 2 | 3 | R3 - retained MVC boundary; API/session decision required |
| CustomerReturnRequests | 3 | 5 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| Customers | 4 | 10 | 0 | 0 | 5 | R2 - mixed controller; retain views until commands/downloads retire |
| Dashboard | 0 | 0 | 0 | 1 | 1 | R3 - retained MVC boundary; API/session decision required |
| DocumentDesigns | 3 | 4 | 0 | 0 | 4 | R2 - mixed controller; retain views until commands/downloads retire |
| GoodsReceiptReturns | 3 | 1 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| GoodsReceipts | 4 | 6 | 0 | 0 | 4 | R2 - mixed controller; retain views until commands/downloads retire |
| Home | 0 | 0 | 0 | 1 | 1 | R3 - retained MVC boundary; API/session decision required |
| InventoryAdjustments | 3 | 5 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| InventoryScanning | 1 | 3 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| InventoryTracking | 1 | 6 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| LandedCosts | 0 | 1 | 0 | 0 | 3 | R3 - retained MVC boundary; API/session decision required |
| Language | 0 | 0 | 0 | 1 | 0 | R4 - permanent compatibility review |
| LocationMovements | 1 | 1 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| PaymentMethods | 0 | 1 | 0 | 2 | 2 | R3 - retained MVC boundary; API/session decision required |
| Platform/Platform | 7 | 4 | 0 | 0 | 7 | R2 - mixed controller; retain views until commands/downloads retire |
| Platform/PlatformAccount | 1 | 2 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Platform/PlatformAuditEvents | 1 | 0 | 1 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Platform/PlatformAuditRetention | 1 | 4 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| ProductAttributes | 2 | 3 | 0 | 0 | 2 | R2 - mixed controller; retain views until commands/downloads retire |
| ProductCategories | 1 | 2 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Products | 2 | 3 | 0 | 1 | 4 | R2 - mixed controller; retain views until commands/downloads retire |
| ProductStocks | 3 | 3 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| ProductTemplates | 1 | 4 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Public | 2 | 0 | 0 | 0 | 2 | R1-A - anonymous public boundary; explicit route contract required |
| PurchaseAgreements | 0 | 4 | 0 | 0 | 3 | R3 - retained MVC boundary; API/session decision required |
| PurchaseAnalytics | 0 | 0 | 0 | 1 | 1 | R3 - retained MVC boundary; API/session decision required |
| PurchaseApprovalRules | 1 | 2 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| PurchaseDocumentReversals | 2 | 1 | 0 | 1 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| PurchaseOrders | 0 | 10 | 0 | 1 | 2 | R3 - retained MVC boundary; API/session decision required |
| PurchaseReplenishment | 1 | 2 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| PurchaseRequests | 1 | 5 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| PurchaseReturns | 0 | 1 | 0 | 3 | 3 | R3 - retained MVC boundary; API/session decision required |
| Purchases | 2 | 2 | 0 | 1 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| PurchaseServiceAcceptances | 3 | 1 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| PurchaseSourcing | 3 | 2 | 1 | 2 | 5 | R2 - mixed controller; retain views until commands/downloads retire |
| PurchaseTemplates | 0 | 3 | 0 | 0 | 1 | R3 - retained MVC boundary; API/session decision required |
| Recipes | 2 | 1 | 0 | 0 | 2 | R2 - mixed controller; retain views until commands/downloads retire |
| Replenishment | 1 | 4 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Roles | 3 | 3 | 0 | 0 | 4 | R2 - mixed controller; retain views until commands/downloads retire |
| Sales | 1 | 2 | 0 | 3 | 4 | R2 - mixed controller; retain views until commands/downloads retire |
| SalesApprovalRules | 1 | 2 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| SalesDeliveries | 3 | 3 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| SalesOrders | 3 | 7 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| SalesPriceLists | 1 | 5 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| SalesQuotations | 3 | 6 | 1 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| SalesQuotationTemplates | 3 | 4 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| SalesReturns | 0 | 1 | 0 | 3 | 3 | R3 - retained MVC boundary; API/session decision required |
| Settings | 17 | 18 | 1 | 0 | 17 | R2 - mixed controller; retain views until commands/downloads retire |
| StockTransactions | 2 | 1 | 0 | 0 | 2 | R2 - mixed controller; retain views until commands/downloads retire |
| StockTransfers | 4 | 8 | 0 | 0 | 4 | R2 - mixed controller; retain views until commands/downloads retire |
| Subscription | 1 | 1 | 0 | 0 | 2 | R2 - mixed controller; retain views until commands/downloads retire |
| SupplierCommunications | 0 | 2 | 0 | 0 | 1 | R3 - retained MVC boundary; API/session decision required |
| SupplierCreditNotes | 3 | 1 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| SupplierIntegrations | 0 | 6 | 1 | 0 | 4 | R3 - retained MVC boundary; API/session decision required |
| SupplierPayments | 3 | 1 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| SupplierPortal | 0 | 3 | 0 | 1 | 2 | R3 - retained MVC boundary; API/session decision required |
| SupplierPurchasing | 4 | 8 | 2 | 0 | 5 | R2 - mixed controller; retain views until commands/downloads retire |
| SupplierQuotations | 0 | 2 | 0 | 0 | 2 | R3 - retained MVC boundary; API/session decision required |
| Suppliers | 3 | 3 | 0 | 0 | 5 | R2 - mixed controller; retain views until commands/downloads retire |
| TaxRates | 0 | 1 | 0 | 2 | 2 | R3 - retained MVC boundary; API/session decision required |
| UnassignedStock | 1 | 3 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Users | 2 | 1 | 0 | 0 | 2 | R2 - mixed controller; retain views until commands/downloads retire |
| VendorBills | 3 | 7 | 1 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| WarehouseLocations | 1 | 2 | 0 | 2 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| Warehouses | 1 | 3 | 0 | 2 | 3 | R2 - mixed controller; retain views until commands/downloads retire |

## Retained action dependencies

| MVC surface | Action | Verb | Reason | Source |
|---|---|---|---|---|
| Account | Login | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Logout | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Register | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/AccountController.cs |
| Accounts | Create | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Accounting/AccountsController.cs |
| Accounts | Create | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Accounting/AccountsController.cs |
| Accounts | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Accounting/AccountsController.cs |
| AnalyticsCenter | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Analytics/AnalyticsCenterController.cs |
| AuditEvents | Export | POST | Retain - download or generated file | MiniStore.Web/Controllers/Security/AuditEventsController.cs |
| Branches | BranchesController | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Accounting/BranchesController.cs |
| CompanyOnboarding | Complete | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs |
| CompanyOnboarding | Skip | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs |
| CustomerCreditNotes | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerCreditNotesController.cs |
| CustomerInvoices | Approve | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Cancel | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Post | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Reverse | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerPayments | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerPaymentsController.cs |
| CustomerPayments | Reverse | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/CustomerPaymentsController.cs |
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
| GoodsReceiptReturns | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/GoodsReceiptReturnsController.cs |
| GoodsReceipts | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs |
| GoodsReceipts | CreateScanDraft | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs |
| GoodsReceipts | Inspect | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs |
| GoodsReceipts | PostDraft | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs |
| GoodsReceipts | PutAway | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs |
| GoodsReceipts | RecordScan | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs |
| Home | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Home/HomeController.cs |
| InventoryAdjustments | Approve | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs |
| InventoryAdjustments | Cancel | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs |
| InventoryAdjustments | Count | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs |
| InventoryAdjustments | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs |
| InventoryAdjustments | Post | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs |
| InventoryScanning | Count | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryScanningController.cs |
| InventoryScanning | Putaway | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryScanningController.cs |
| InventoryScanning | Relocate | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryScanningController.cs |
| InventoryTracking | CloseRecall | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryTrackingController.cs |
| InventoryTracking | CreateRecall | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryTrackingController.cs |
| InventoryTracking | Open | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryTrackingController.cs |
| InventoryTracking | Quarantine | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryTrackingController.cs |
| InventoryTracking | RecordRecallCommunication | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryTrackingController.cs |
| InventoryTracking | Release | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/InventoryTrackingController.cs |
| LandedCosts | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/LandedCostsController.cs |
| Language | Set | POST | Retain - explicit compatibility boundary | MiniStore.Web/Controllers/Settings/LanguageController.cs |
| LocationMovements | Move | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/LocationMovementsController.cs |
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
| ProductAttributes | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductAttributesController.cs |
| ProductAttributes | SetActive | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductAttributesController.cs |
| ProductAttributes | Values | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductAttributesController.cs |
| ProductCategories | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductCategoriesController.cs |
| ProductCategories | SetActive | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductCategoriesController.cs |
| Products | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductsController.cs |
| Products | Delete | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductsController.cs |
| Products | Edit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductsController.cs |
| Products | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Catalog/ProductsController.cs |
| ProductStocks | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/ProductStocksController.cs |
| ProductStocks | Delete | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/ProductStocksController.cs |
| ProductStocks | Edit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/ProductStocksController.cs |
| ProductTemplates | Assign | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductTemplatesController.cs |
| ProductTemplates | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductTemplatesController.cs |
| ProductTemplates | Generate | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductTemplatesController.cs |
| ProductTemplates | Preview | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/ProductTemplatesController.cs |
| PurchaseAgreements | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseAgreementsController.cs |
| PurchaseAgreements | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseAgreementsController.cs |
| PurchaseAgreements | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseAgreementsController.cs |
| PurchaseAgreements | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseAgreementsController.cs |
| PurchaseAnalytics | View | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Purchases/PurchaseAnalyticsController.cs |
| PurchaseApprovalRules | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseApprovalRulesController.cs |
| PurchaseApprovalRules | SetActive | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseApprovalRulesController.cs |
| PurchaseDocumentReversals | Create | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Purchases/PurchaseDocumentReversalsController.cs |
| PurchaseDocumentReversals | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseDocumentReversalsController.cs |
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
| PurchaseReplenishment | RunNow | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseReplenishmentController.cs |
| PurchaseReplenishment | SaveSettings | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseReplenishmentController.cs |
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
| PurchaseServiceAcceptances | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseServiceAcceptancesController.cs |
| PurchaseSourcing | Attachment | GET | Retain - download or generated file | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Create | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Direct | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Direct | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseTemplates | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseTemplatesController.cs |
| PurchaseTemplates | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseTemplatesController.cs |
| PurchaseTemplates | RedirectToAction | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/PurchaseTemplatesController.cs |
| Recipes | Edit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Catalog/RecipesController.cs |
| Replenishment | CreatePurchaseRequestDraft | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/ReplenishmentController.cs |
| Replenishment | CreateTransferDraft | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/ReplenishmentController.cs |
| Replenishment | Save | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/ReplenishmentController.cs |
| Replenishment | SetActive | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/ReplenishmentController.cs |
| Roles | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Delete | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Edit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/RolesController.cs |
| Sales | Create | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Details | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Post | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesController.cs |
| SalesApprovalRules | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesApprovalRulesController.cs |
| SalesApprovalRules | SetActive | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesApprovalRulesController.cs |
| SalesDeliveries | Cancel | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesDeliveriesController.cs |
| SalesDeliveries | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesDeliveriesController.cs |
| SalesDeliveries | Post | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Sales/SalesDeliveriesController.cs |
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
| StockTransactions | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/StockTransactionsController.cs |
| StockTransfers | Approve | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/StockTransfersController.cs |
| StockTransfers | Cancel | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/StockTransfersController.cs |
| StockTransfers | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/StockTransfersController.cs |
| StockTransfers | Edit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/StockTransfersController.cs |
| StockTransfers | Post | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/StockTransfersController.cs |
| StockTransfers | Reject | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/StockTransfersController.cs |
| StockTransfers | ReturnToDraft | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/StockTransfersController.cs |
| StockTransfers | Submit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/StockTransfersController.cs |
| Subscription | Checkout | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Saas/SubscriptionController.cs |
| SupplierCommunications | RedirectToAction | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierCommunicationsController.cs |
| SupplierCommunications | RedirectToAction | POST, GET | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierCommunicationsController.cs |
| SupplierCreditNotes | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierCreditNotesController.cs |
| SupplierIntegrations | File | GET | Retain - download or generated file | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | RedirectToAction | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | RedirectToAction | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | RedirectToAction | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | RedirectToDocument | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | View | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | View | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierPayments | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/SupplierPaymentsController.cs |
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
| Suppliers | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Suppliers/SuppliersController.cs |
| Suppliers | Delete | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Suppliers/SuppliersController.cs |
| Suppliers | Edit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Suppliers/SuppliersController.cs |
| TaxRates | Create | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Accounting/TaxRatesController.cs |
| TaxRates | Create | GET, POST | Retain - MVC mutation | MiniStore.Web/Controllers/Accounting/TaxRatesController.cs |
| TaxRates | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Accounting/TaxRatesController.cs |
| UnassignedStock | Assign | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/UnassignedStockController.cs |
| UnassignedStock | CreateRule | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/UnassignedStockController.cs |
| UnassignedStock | SetRuleActive | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/UnassignedStockController.cs |
| Users | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/UsersController.cs |
| VendorBills | Approve | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | ApproveOverride | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | Cancel | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | DownloadAttachment | GET | Retain - download or generated file | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | Post | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | RedirectToAction | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | RedirectToAction | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| WarehouseLocations | Create | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Inventory/WarehouseLocationsController.cs |
| WarehouseLocations | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/WarehouseLocationsController.cs |
| WarehouseLocations | Edit | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Inventory/WarehouseLocationsController.cs |
| WarehouseLocations | Edit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/WarehouseLocationsController.cs |
| Warehouses | Create | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Inventory/WarehousesController.cs |
| Warehouses | Create | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/WarehousesController.cs |
| Warehouses | Delete | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/WarehousesController.cs |
| Warehouses | Edit | GET | Retain - non-cutover route or session boundary | MiniStore.Web/Controllers/Inventory/WarehousesController.cs |
| Warehouses | Edit | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Inventory/WarehousesController.cs |

## Mandatory batch checklist

1. Name the exact controller actions, Razor files and shared references in the proposed batch.
2. Prove Angular/API functional parity for reads, validation recovery, commands, downloads and permissions that the batch owns.
3. Search runtime, Razor, tests, navigation and documentation references; the batch must leave no dangling route or view return.
4. Decide how `PresentationCutover` and MVC rollback behave after deletion; do not leave a configuration switch that points to removed code.
5. Obtain explicit owner approval for that bounded deletion batch.
6. Run Release, regression, generated-inventory, Angular and authenticated browser acceptance, then record deployment rollback evidence.

## Current outcome

MOD-030F inventory and the separately approved MOD-030G/MOD-030H cohorts are complete. MOD-030I audited `Public` without deletion and classified it as the sole R1-A anonymous boundary: Angular/API parity exists, but the current permission-protected conventional retired-route catalog cannot own `/` and `/pricing` safely. Seven MVC presentation actions and seven Razor views are physically retired; no Public or R2/R3/R4 deletion is approved.
