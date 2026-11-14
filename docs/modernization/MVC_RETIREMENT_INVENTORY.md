# MVC retirement dependency inventory

> Generated from source by `scripts/generate-mvc-migration-registry.ps1`. Do not edit inventory rows manually.
> MOD-030F created the non-destructive baseline. MOD-030G and MOD-030H record the separately approved physical retirement cohorts.

## Decision

Physical MVC retirement must follow feature-sized, reversible cohorts. A Razor view on a mixed controller remains required until every retained MVC mutation that can redisplay validation errors has an accepted Angular/API replacement. Downloads, authentication/session routing, public token flows and the Language POST compatibility endpoint are independent boundaries, not presentation leftovers.

## Reconciliation

- MVC controllers: 13.
- MVC actions classified: 82 of 82.
- Angular-default presentation actions eligible for cohort review: 32.
- Retained MVC mutation actions: 43.
- Retained download/generated-file actions: 3.
- Retained compatibility, ambiguous-verb, session or non-cutover actions: 4.
- Razor files discovered: 52; shared infrastructure files: 11.
- Presentation-only pilot surfaces: 0, including 0 anonymous public boundary.
- Mixed-controller surfaces blocked from early view deletion: 8.
- Physically retired presentation actions with legacy redirects: 158 across 69 surfaces.

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
| Accounts | /accounting/accounts | Administration.Access |
| Accounts | /accounting/accounts | Administration.Access |
| AnalyticsCenter | /analytics/builder | AnalyticsCenter.Customize |
| AnalyticsCenter | /analytics | AnalyticsCenter.View |
| AuditEvents | /audit-events | AuditEvents.View |
| Branches | /products/catalog-masters | Administration.Access |
| Branches | /products/catalog-masters | Administration.Access |
| CustomerCreditNotes | /sales/credit-notes/{id} | CustomerCreditNotes.View |
| CustomerCreditNotes | /sales/credit-notes/new?customerInvoiceId={customerInvoiceId} | CustomerCreditNotes.Create |
| CustomerCreditNotes | /sales/credit-notes | CustomerCreditNotes.View |
| CustomerInvoices | /sales/invoices/{id} | CustomerInvoices.View |
| CustomerInvoices | /sales/invoices/new?salesOrderId={salesOrderId} | CustomerInvoices.Create |
| CustomerInvoices | /sales/invoices | CustomerInvoices.View |
| CustomerPayments | /sales/payments/new?customerInvoiceId={customerInvoiceId} | CustomerPayments.Create |
| CustomerPayments | /sales/payments | CustomerPayments.View |
| CustomerPayments | /sales/payments/{id} | CustomerPayments.View |
| CustomerReturnReceipts | /sales/return-receipts | CustomerReturnReceipts.View |
| CustomerReturnReceipts | /sales/return-receipts/new?customerReturnRequestId={customerReturnRequestId} | CustomerReturnReceipts.Create |
| CustomerReturnReceipts | /sales/return-receipts/{id} | CustomerReturnReceipts.View |
| CustomerReturnRequests | /sales/return-requests | CustomerReturnRequests.View |
| CustomerReturnRequests | /sales/return-requests/new?salesDeliveryId={salesDeliveryId} | CustomerReturnRequests.Create |
| CustomerReturnRequests | /sales/return-requests/{id} | CustomerReturnRequests.View |
| Customers | /customers/{id}/edit | Customers.Edit |
| Customers | /customers/new | Customers.Create |
| Customers | /customers/{id} | Customers.View |
| Customers | /customers | Customers.View |
| DocumentDesigns | /settings/document-designs/{id} | Administration.Access |
| DocumentDesigns | /settings/document-designs/new | Administration.Access |
| DocumentDesigns | /settings/document-designs | Administration.Access |
| GoodsReceiptReturns | /purchases/receipt-returns | GoodsReceiptReturns.View |
| GoodsReceiptReturns | /purchases/receipt-returns/new?goodsReceiptId={goodsReceiptId} | GoodsReceiptReturns.Create |
| GoodsReceiptReturns | /purchases/receipt-returns/{id} | GoodsReceiptReturns.View |
| GoodsReceipts | /purchases/receipts/{id}/scan | GoodsReceipts.Create |
| GoodsReceipts | /purchases/receipts/{id} | GoodsReceipts.View |
| GoodsReceipts | /purchases/receipts | GoodsReceipts.View |
| GoodsReceipts | /purchases/receipts/new?purchaseOrderId={purchaseOrderId} | GoodsReceipts.Create |
| InventoryAdjustments | /inventory/adjustments/{id} | InventoryAdjustments.View |
| InventoryAdjustments | /inventory/adjustments/new | InventoryAdjustments.Create |
| InventoryAdjustments | /inventory/adjustments | InventoryAdjustments.View |
| InventoryBalances | /inventory/availability | InventoryBalances.View |
| InventoryInsights | /inventory/insights | InventoryInsights.View |
| InventoryReconciliation | /inventory/reconciliation | InventoryReconciliation.View |
| InventoryReservations | /inventory/reservations | InventoryReservations.View |
| InventoryScanning | /inventory/scanning | InventoryScanning.View |
| InventoryTracking | /inventory/tracking | InventoryTracking.View |
| JournalEntries | /accounting/journal-entries/{id} | JournalEntries.View |
| LandedCosts | /purchases/landed-costs/{id} | LandedCosts.View |
| LandedCosts | /purchases/landed-costs | LandedCosts.View |
| LandedCosts | /purchases/landed-costs/new?goodsReceiptId={goodsReceiptId} | LandedCosts.Create |
| LocationMovements | /inventory/location-movements | LocationMovements.View |
| PaymentMethods | /products/catalog-masters | Administration.Access |
| PaymentMethods | /products/catalog-masters | Administration.Access |
| ProductAttributes | /products/attributes | Products.View |
| ProductAttributes | /products/{productId}/attributes | Products.Edit |
| ProductCategories | /products/categories | Products.View |
| Products | /products | Products.View |
| Products | /products/new | Products.Create |
| Products | /products/{id}/edit | Products.Edit |
| ProductStocks | /inventory/stocks | ProductStock.View |
| ProductStocks | /inventory/stocks/new | ProductStock.Create |
| ProductStocks | /inventory/stocks/{id}/edit | ProductStock.Edit |
| ProductTemplates | /products/templates | Products.View |
| Public | /pricing | Anonymous |
| Public | /welcome | Anonymous |
| PurchaseAccountingReconciliation | /purchases/accounting-reconciliation | PurchaseAccountingReconciliation.View |
| PurchaseAgreements | /purchases/agreements/new?sourcingEventId={sourcingEventId} | PurchaseAgreements.Create |
| PurchaseAgreements | /purchases/agreements/{id} | PurchaseAgreements.View |
| PurchaseAgreements | /purchases/agreements | PurchaseAgreements.View |
| PurchaseAnalytics | /purchases/analytics | PurchaseAnalytics.View |
| PurchaseApprovalRules | /purchases/approval-rules | PurchaseApprovalRules.View |
| PurchaseDocumentReversals | /purchases/reversals | PurchaseDocumentReversals.View |
| PurchaseDocumentReversals | /purchases/reversals/{id} | PurchaseDocumentReversals.View |
| PurchaseDocumentReversals | /purchases/reversals/new | PurchaseDocumentReversals.Create |
| PurchaseOrders | /purchases/orders | PurchaseOrders.View |
| PurchaseOrders | /purchases/orders/{id} | PurchaseOrders.View |
| PurchaseReplenishment | /purchases/replenishment | PurchaseReplenishment.View |
| PurchaseRequests | /purchases/requests/new | PurchaseRequests.Create |
| PurchaseRequests | /purchases/requests | PurchaseRequests.View |
| PurchaseRequests | /purchases/requests/{id} | PurchaseRequests.View |
| PurchaseReturns | /purchases/legacy/{purchaseId}/return | PurchaseReturns.Create |
| PurchaseReturns | /purchases/legacy-returns/{id} | PurchaseReturns.View |
| PurchaseReturns | /purchases/legacy-returns | PurchaseReturns.View |
| Purchases | /purchases/legacy | Purchases.View |
| Purchases | /purchases/legacy | Purchases.Create |
| Purchases | /purchases/legacy/{id} | Purchases.View |
| PurchaseServiceAcceptances | /purchases/service-acceptances/{id} | PurchaseServiceAcceptances.View |
| PurchaseServiceAcceptances | /purchases/service-acceptances | PurchaseServiceAcceptances.View |
| PurchaseServiceAcceptances | /purchases/service-acceptances/new?purchaseOrderId={purchaseOrderId} | PurchaseServiceAcceptances.Create |
| PurchaseSourcing | /purchases/sourcing | PurchaseSourcing.View |
| PurchaseSourcing | /purchases/sourcing/new | PurchaseSourcing.CreateDirect |
| PurchaseSourcing | /purchases/sourcing/{id} | PurchaseSourcing.View |
| PurchaseSourcing | /purchases/sourcing/{id}/print | PurchaseSourcing.View |
| PurchaseSourcing | /purchases/sourcing/new?purchaseRequestId={purchaseRequestId} | PurchaseSourcing.Create |
| PurchaseTemplates | /purchases/templates | PurchaseTemplates.View |
| Recipes | /products/recipes | Products.View |
| Recipes | /products/recipes/{productId} | Products.Edit |
| Replenishment | /inventory/replenishment | Inventory.Replenishment.View |
| Roles | /roles/new | Roles.Create |
| Roles | /roles | Roles.View |
| Roles | /roles/{id}/edit | Roles.Edit |
| Sales | /sales/pos | Sales.Create |
| Sales | /sales/legacy | Sales.View |
| Sales | /sales/legacy/new | Sales.Create |
| Sales | /sales/legacy/{id} | Sales.View |
| SalesApprovalRules | /sales/approval-rules | SalesApprovalRules.View |
| SalesDeliveries | /sales/deliveries/{id} | SalesDeliveries.View |
| SalesDeliveries | /sales/deliveries/new?salesOrderId={salesOrderId} | SalesDeliveries.Create |
| SalesDeliveries | /sales/deliveries | SalesDeliveries.View |
| SalesOrders | /sales/orders/{id} | SalesOrders.View |
| SalesOrders | /sales/orders | SalesOrders.View |
| SalesOrders | /sales/orders/{id}/reservations | SalesOrders.View |
| SalesPriceLists | /sales/pricing | SalesPricing.View |
| SalesQuotations | /sales/quotations | SalesQuotations.View |
| SalesQuotations | /sales/quotations/new | SalesQuotations.Create |
| SalesQuotations | /sales/quotations/{id} | SalesQuotations.View |
| SalesQuotationTemplates | /sales/quotation-templates | SalesQuotationTemplates.View |
| SalesQuotationTemplates | /sales/quotation-templates/new | SalesQuotationTemplates.Manage |
| SalesQuotationTemplates | /sales/quotation-templates/{id} | SalesQuotationTemplates.View |
| SalesReturns | /sales/legacy/{saleId}/return | SalesReturns.Create |
| SalesReturns | /sales/legacy-returns/{id} | SalesReturns.View |
| SalesReturns | /sales/legacy-returns | SalesReturns.View |
| StockMovements | /inventory/location-movements | StockMovements.View |
| StockTransactions | /inventory/transactions | StockTransactions.View |
| StockTransactions | /inventory/stocks/adjustment | StockTransactions.Create |
| StockTransfers | /inventory/transfers | StockTransfers.View |
| StockTransfers | /inventory/transfers/{id}/edit | StockTransfers.Edit |
| StockTransfers | /inventory/transfers/new | StockTransfers.Create |
| StockTransfers | /inventory/transfers/{id} | StockTransfers.View |
| SupplierCommunications | /purchases/communications/{id} | SupplierCommunications.View |
| SupplierCreditNotes | /purchases/credit-notes/new?vendorBillId={vendorBillId} | SupplierCreditNotes.Create |
| SupplierCreditNotes | /purchases/credit-notes/{id} | SupplierCreditNotes.View |
| SupplierCreditNotes | /purchases/credit-notes | SupplierCreditNotes.View |
| SupplierPayments | /purchases/payments | SupplierPayments.View |
| SupplierPayments | /purchases/payments/{id} | SupplierPayments.View |
| SupplierPayments | /purchases/payments/new | SupplierPayments.Create |
| SupplierPurchasing | /purchases/supplier-terms/{id}/history | Purchases.SupplierTerms.View |
| SupplierPurchasing | /purchases/supplier-terms/{id}/price-breaks | Purchases.SupplierTerms.View |
| SupplierPurchasing | /purchases/supplier-terms | Purchases.SupplierTerms.View |
| SupplierPurchasing | /purchases/supplier-terms/exceptions | Purchases.SupplierTerms.View |
| SupplierQuotations | /purchases/sourcing/{sourcingEventId}/quotation | SupplierQuotations.Create |
| SupplierQuotations | /purchases/sourcing/{sourcingEventId}/comparison | SupplierQuotations.View |
| Suppliers | /suppliers/new | Suppliers.Create |
| Suppliers | /suppliers | Suppliers.View |
| Suppliers | /suppliers/{id}/edit | Suppliers.Edit |
| TaxRates | /products/catalog-masters | Administration.Access |
| TaxRates | /products/catalog-masters | Administration.Access |
| UnassignedStock | /inventory/putaway | ProductStock.View |
| Users | /users | Users.View |
| Users | /users/new | Users.Create |
| VendorBills | /purchases/bills/new?purchaseOrderId={purchaseOrderId} | VendorBills.Create |
| VendorBills | /purchases/bills | VendorBills.View |
| VendorBills | /purchases/bills/{id} | VendorBills.View |
| WarehouseLocations | /inventory/locations | Warehouses.View |
| WarehouseLocations | /inventory/locations | Warehouses.Create |
| WarehouseLocations | /inventory/locations | Warehouses.Edit |
| Warehouses | /warehouses | Warehouses.Create |
| Warehouses | /warehouses | Warehouses.View |
| Warehouses | /warehouses | Warehouses.Edit |

## Surface inventory

| MVC surface | Presentation candidates | Mutations retained | Downloads retained | Other boundaries retained | Razor files | Cohort |
|---|---:|---:|---:|---:|---:|---|
| Account | 3 | 3 | 0 | 0 | 3 | R2 - mixed controller; retain views until commands/downloads retire |
| CompanyOnboarding | 1 | 2 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Dashboard | 0 | 0 | 0 | 1 | 1 | R3 - retained MVC boundary; API/session decision required |
| Home | 0 | 0 | 0 | 1 | 1 | R3 - retained MVC boundary; API/session decision required |
| Language | 0 | 0 | 0 | 1 | 0 | R4 - permanent compatibility review |
| Platform/Platform | 7 | 4 | 0 | 0 | 7 | R2 - mixed controller; retain views until commands/downloads retire |
| Platform/PlatformAccount | 1 | 2 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Platform/PlatformAuditEvents | 1 | 0 | 1 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Platform/PlatformAuditRetention | 1 | 4 | 0 | 0 | 1 | R2 - mixed controller; retain views until commands/downloads retire |
| Settings | 17 | 18 | 1 | 0 | 17 | R2 - mixed controller; retain views until commands/downloads retire |
| Subscription | 1 | 1 | 0 | 0 | 2 | R2 - mixed controller; retain views until commands/downloads retire |
| SupplierIntegrations | 0 | 6 | 1 | 0 | 4 | R3 - retained MVC boundary; API/session decision required |
| SupplierPortal | 0 | 3 | 0 | 1 | 2 | R3 - retained MVC boundary; API/session decision required |

## R2 prioritization signals

This is a dependency-count signal, not deletion approval. Score = mutations × 3 + downloads × 2 + other boundaries × 4 + Razor files. Business criticality, session isolation, validation recovery and live evidence can override the numeric order.

| Priority | MVC surface | Score | Presentation candidates | Mutations | Downloads | Other boundaries | Razor files |
|---:|---|---:|---:|---:|---:|---:|---:|
| 1 | Platform/PlatformAuditEvents | 3 | 1 | 0 | 1 | 0 | 1 |
| 2 | Subscription | 5 | 1 | 1 | 0 | 0 | 2 |
| 3 | CompanyOnboarding | 7 | 1 | 2 | 0 | 0 | 1 |
| 4 | Platform/PlatformAccount | 7 | 1 | 2 | 0 | 0 | 1 |
| 5 | Account | 12 | 3 | 3 | 0 | 0 | 3 |
| 6 | Platform/PlatformAuditRetention | 13 | 1 | 4 | 0 | 0 | 1 |
| 7 | Platform/Platform | 19 | 7 | 4 | 0 | 0 | 7 |
| 8 | Settings | 73 | 17 | 18 | 1 | 0 | 17 |

## Retained action dependencies

| MVC surface | Action | Verb | Reason | Source |
|---|---|---|---|---|
| Account | Login | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Logout | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Register | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Security/AccountController.cs |
| CompanyOnboarding | Complete | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs |
| CompanyOnboarding | Skip | POST | Retain - MVC mutation | MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs |
| Dashboard | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Home/DashboardController.cs |
| Home | Index | ANY | Retain - ambiguous verb or session routing | MiniStore.Web/Controllers/Home/HomeController.cs |
| Language | Set | POST | Retain - explicit compatibility boundary | MiniStore.Web/Controllers/Settings/LanguageController.cs |
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

## Mandatory batch checklist

1. Name the exact controller actions, Razor files and shared references in the proposed batch.
2. Prove Angular/API functional parity for reads, validation recovery, commands, downloads and permissions that the batch owns.
3. Search runtime, Razor, tests, navigation and documentation references; the batch must leave no dangling route or view return.
4. Decide how `PresentationCutover` and MVC rollback behave after deletion; do not leave a configuration switch that points to removed code.
5. Obtain explicit owner approval for that bounded deletion batch.
6. Run Release, regression, generated-inventory, Angular and authenticated browser acceptance, then record deployment rollback evidence.

## Current outcome

Retirement cohorts through MOD-031B are complete. Seventy-two MVC presentation GET actions across thirty-seven surfaces and seventy-two Razor views are physically retired. Public exact `/` and `/pricing` routes are anonymous; other retired GET/HEAD routes retain their original permission policies and preserve queries. Retired MVC POST/download actions are not redirected or replayed; Angular continues through authenticated, permission-protected and antiforgery-protected APIs. No additional deletion is approved outside the next named ten-controller audit.
