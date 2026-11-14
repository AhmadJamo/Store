# MVC to Angular migration registry

> Generated from source by scripts/generate-mvc-migration-registry.ps1. Do not edit rows manually.

## Reconciliation

- Controllers discovered: 103 total; 15 MVC; 88 API.
- User-facing MVC views discovered: 45.
- MVC actions discovered: 91.
- Razor files included in the control inventory: 50.
- Angular application routes discovered: 194.
- Presentation surfaces with a verified Angular counterpart: 14.
- Remaining presentation surfaces without a mapped Angular counterpart: 0.
- Mutation-only MVC compatibility surfaces: 1 (Language).
- Physically retired presentation actions with legacy redirects: 153 across 67 surfaces (Accounts, AnalyticsCenter, AuditEvents, Branches, CustomerCreditNotes, CustomerInvoices, CustomerPayments, CustomerReturnReceipts, CustomerReturnRequests, Customers, DocumentDesigns, GoodsReceiptReturns, GoodsReceipts, InventoryAdjustments, InventoryBalances, InventoryInsights, InventoryReconciliation, InventoryReservations, InventoryScanning, InventoryTracking, JournalEntries, LandedCosts, LocationMovements, PaymentMethods, ProductAttributes, ProductCategories, Products, ProductStocks, ProductTemplates, Public, PurchaseAccountingReconciliation, PurchaseAgreements, PurchaseAnalytics, PurchaseApprovalRules, PurchaseDocumentReversals, PurchaseOrders, PurchaseReplenishment, PurchaseRequests, PurchaseReturns, Purchases, PurchaseServiceAcceptances, PurchaseSourcing, PurchaseTemplates, Recipes, Replenishment, Sales, SalesApprovalRules, SalesDeliveries, SalesOrders, SalesPriceLists, SalesQuotations, SalesQuotationTemplates, SalesReturns, StockMovements, StockTransactions, StockTransfers, SupplierCommunications, SupplierCreditNotes, SupplierPayments, SupplierPurchasing, SupplierQuotations, Suppliers, TaxRates, UnassignedStock, VendorBills, WarehouseLocations, Warehouses).
- Allowlisted Angular-default cutover actions: 116.

Remaining presentation surfaces: none.

Statuses follow the migration lifecycle plus MVC Compatibility (mutation-only). A mutation-only boundary has no GET presentation surface and is intentionally excluded from AngularDefault redirects.

## View inventory

| MVC surface | View | Migration status | Angular counterpart | Source |
|---|---|---|---|---|
| Account | AccessDenied.cshtml | Angular Default | /login, /register, /access-denied | MiniStore.Web/Views/Account/AccessDenied.cshtml |
| Account | Login.cshtml | Angular Default | /login, /register, /access-denied | MiniStore.Web/Views/Account/Login.cshtml |
| Account | Register.cshtml | Angular Default | /login, /register, /access-denied | MiniStore.Web/Views/Account/Register.cshtml |
| CompanyOnboarding | Index.cshtml | Angular Default | /onboarding | MiniStore.Web/Views/CompanyOnboarding/Index.cshtml |
| Dashboard | Index.cshtml | Angular Default | / | MiniStore.Web/Views/Dashboard/Index.cshtml |
| Home | Index.cshtml | Angular Default | / | MiniStore.Web/Views/Home/Index.cshtml |
| Platform/Platform | Companies.cshtml | Angular Pilot | /platform-control, /platform-management | MiniStore.Web/Areas/Platform/Views/Platform/Companies.cshtml |
| Platform/Platform | CreatePlan.cshtml | Angular Pilot | /platform-control, /platform-management | MiniStore.Web/Areas/Platform/Views/Platform/CreatePlan.cshtml |
| Platform/Platform | CreatePromotion.cshtml | Angular Pilot | /platform-control, /platform-management | MiniStore.Web/Areas/Platform/Views/Platform/CreatePromotion.cshtml |
| Platform/Platform | Index.cshtml | Angular Pilot | /platform-control, /platform-management | MiniStore.Web/Areas/Platform/Views/Platform/Index.cshtml |
| Platform/Platform | PendingPayments.cshtml | Angular Pilot | /platform-control, /platform-management | MiniStore.Web/Areas/Platform/Views/Platform/PendingPayments.cshtml |
| Platform/Platform | Plans.cshtml | Angular Pilot | /platform-control, /platform-management | MiniStore.Web/Areas/Platform/Views/Platform/Plans.cshtml |
| Platform/Platform | Promotions.cshtml | Angular Pilot | /platform-control, /platform-management | MiniStore.Web/Areas/Platform/Views/Platform/Promotions.cshtml |
| Platform/PlatformAccount | Login.cshtml | Angular Pilot | /platform-login | MiniStore.Web/Areas/Platform/Views/PlatformAccount/Login.cshtml |
| Platform/PlatformAuditEvents | Index.cshtml | Angular Pilot | /platform-audit | MiniStore.Web/Areas/Platform/Views/PlatformAuditEvents/Index.cshtml |
| Platform/PlatformAuditRetention | Index.cshtml | Angular Pilot | /platform-retention | MiniStore.Web/Areas/Platform/Views/PlatformAuditRetention/Index.cshtml |
| Roles | Create.cshtml | Angular Default | /roles | MiniStore.Web/Views/Roles/Create.cshtml |
| Roles | Edit.cshtml | Angular Default | /roles | MiniStore.Web/Views/Roles/Edit.cshtml |
| Roles | Index.cshtml | Angular Default | /roles | MiniStore.Web/Views/Roles/Index.cshtml |
| Settings | Accounting.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/Accounting.cshtml |
| Settings | BranchSalesAccounts.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/BranchSalesAccounts.cshtml |
| Settings | Currencies.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/Currencies.cshtml |
| Settings | Discounts.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/Discounts.cshtml |
| Settings | DocumentNumbers.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/DocumentNumbers.cshtml |
| Settings | FiscalPeriods.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/FiscalPeriods.cshtml |
| Settings | General.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/General.cshtml |
| Settings | Index.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/Index.cshtml |
| Settings | Inventory.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/Inventory.cshtml |
| Settings | InventoryAccess.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/InventoryAccess.cshtml |
| Settings | OrganizationIdentity.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/OrganizationIdentity.cshtml |
| Settings | Pos.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/Pos.cshtml |
| Settings | PurchaseMatching.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/PurchaseMatching.cshtml |
| Settings | PurchaseSourcingPolicy.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/PurchaseSourcingPolicy.cshtml |
| Settings | Units.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/Units.cshtml |
| Settings | WorkflowCatalog.cshtml | Angular Default | /settings | MiniStore.Web/Views/Settings/WorkflowCatalog.cshtml |
| Subscription | Checkout.cshtml | Angular Default | /subscription | MiniStore.Web/Views/Subscription/Checkout.cshtml |
| Subscription | Index.cshtml | Angular Default | /subscription | MiniStore.Web/Views/Subscription/Index.cshtml |
| SupplierIntegrations | CredentialCreated.cshtml | Angular Default | /purchases/integrations | MiniStore.Web/Views/SupplierIntegrations/CredentialCreated.cshtml |
| SupplierIntegrations | Index.cshtml | Angular Default | /purchases/integrations | MiniStore.Web/Views/SupplierIntegrations/Index.cshtml |
| SupplierIntegrations | PortalAccessCreated.cshtml | Angular Default | /purchases/integrations | MiniStore.Web/Views/SupplierIntegrations/PortalAccessCreated.cshtml |
| SupplierIntegrations | Submission.cshtml | Angular Default | /purchases/integrations | MiniStore.Web/Views/SupplierIntegrations/Submission.cshtml |
| SupplierPortal | Index.cshtml | Angular Default | /supplier-portal/:token | MiniStore.Web/Views/SupplierPortal/Index.cshtml |
| SupplierPortal | Unavailable.cshtml | Angular Default | /supplier-portal/:token | MiniStore.Web/Views/SupplierPortal/Unavailable.cshtml |
| Users | Create.cshtml | Angular Default | /users | MiniStore.Web/Views/Users/Create.cshtml |
| Users | Index.cshtml | Angular Default | /users | MiniStore.Web/Views/Users/Index.cshtml |

## Action inventory

| MVC surface | Action | HTTP verbs | Migration status | Angular counterpart | Angular-default cutover route | Source |
|---|---|---|---|---|---|---|
| Account | AccessDenied | GET | Angular Default | /login, /register, /access-denied | /access-denied | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Login | GET | Angular Default | /login, /register, /access-denied | /login | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Login | POST | Angular Default | /login, /register, /access-denied | /login | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Logout | POST | Angular Default | /login, /register, /access-denied | — | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Register | GET | Angular Default | /login, /register, /access-denied | /register | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Register | POST | Angular Default | /login, /register, /access-denied | /register | MiniStore.Web/Controllers/Security/AccountController.cs |
| CompanyOnboarding | Complete | POST | Angular Default | /onboarding | — | MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs |
| CompanyOnboarding | Index | GET | Angular Default | /onboarding | /onboarding | MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs |
| CompanyOnboarding | Skip | POST | Angular Default | /onboarding | — | MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs |
| Dashboard | Index | ANY | Angular Default | / | / | MiniStore.Web/Controllers/Home/DashboardController.cs |
| Home | Index | ANY | Angular Default | / | / | MiniStore.Web/Controllers/Home/HomeController.cs |
| Language | Set | POST | MVC Compatibility (mutation-only) | Not applicable (mutation-only) | — | MiniStore.Web/Controllers/Settings/LanguageController.cs |
| Platform/Platform | Companies | GET | Angular Pilot | /platform-control, /platform-management | /platform-management | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | ConfirmPayment | GET, POST | Angular Pilot | /platform-control, /platform-management | — | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | CreatePlan | GET | Angular Pilot | /platform-control, /platform-management | /platform-management | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | CreatePlan | GET, POST | Angular Pilot | /platform-control, /platform-management | /platform-management | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | CreatePromotion | GET | Angular Pilot | /platform-control, /platform-management | /platform-management | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | CreatePromotion | GET, POST | Angular Pilot | /platform-control, /platform-management | /platform-management | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | Index | GET | Angular Pilot | /platform-control, /platform-management | /platform-control | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | PendingPayments | GET | Angular Pilot | /platform-control, /platform-management | /platform-management | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | Plans | GET | Angular Pilot | /platform-control, /platform-management | /platform-management | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | Promotions | GET | Angular Pilot | /platform-control, /platform-management | /platform-management | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/Platform | UpdateCompany | GET, POST | Angular Pilot | /platform-control, /platform-management | — | MiniStore.Web/Areas/Platform/Controllers/PlatformController.cs |
| Platform/PlatformAccount | Login | GET | Angular Pilot | /platform-login | /platform-login | MiniStore.Web/Areas/Platform/Controllers/PlatformAccountController.cs |
| Platform/PlatformAccount | Login | POST | Angular Pilot | /platform-login | /platform-login | MiniStore.Web/Areas/Platform/Controllers/PlatformAccountController.cs |
| Platform/PlatformAccount | Logout | POST | Angular Pilot | /platform-login | — | MiniStore.Web/Areas/Platform/Controllers/PlatformAccountController.cs |
| Platform/PlatformAuditEvents | Export | POST | Angular Pilot | /platform-audit | — | MiniStore.Web/Areas/Platform/Controllers/PlatformAuditEventsController.cs |
| Platform/PlatformAuditEvents | Index | GET | Angular Pilot | /platform-audit | /platform-audit | MiniStore.Web/Areas/Platform/Controllers/PlatformAuditEventsController.cs |
| Platform/PlatformAuditRetention | Checkpoint | POST | Angular Pilot | /platform-retention | — | MiniStore.Web/Areas/Platform/Controllers/PlatformAuditRetentionController.cs |
| Platform/PlatformAuditRetention | CreateHold | POST | Angular Pilot | /platform-retention | — | MiniStore.Web/Areas/Platform/Controllers/PlatformAuditRetentionController.cs |
| Platform/PlatformAuditRetention | Index | GET | Angular Pilot | /platform-retention | /platform-retention | MiniStore.Web/Areas/Platform/Controllers/PlatformAuditRetentionController.cs |
| Platform/PlatformAuditRetention | ReleaseHold | POST | Angular Pilot | /platform-retention | — | MiniStore.Web/Areas/Platform/Controllers/PlatformAuditRetentionController.cs |
| Platform/PlatformAuditRetention | Run | POST | Angular Pilot | /platform-retention | — | MiniStore.Web/Areas/Platform/Controllers/PlatformAuditRetentionController.cs |
| Roles | Create | GET | Angular Default | /roles | /roles/new | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Create | POST | Angular Default | /roles | /roles/new | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Delete | POST | Angular Default | /roles | — | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Edit | GET | Angular Default | /roles | /roles/{id}/edit | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Edit | POST | Angular Default | /roles | /roles/{id}/edit | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Index | GET | Angular Default | /roles | /roles | MiniStore.Web/Controllers/Security/RolesController.cs |
| Settings | Accounting | GET | Angular Default | /settings | /settings/accounting | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Accounting | POST | Angular Default | /settings | /settings/accounting | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | BranchSalesAccounts | GET | Angular Default | /settings | /settings/branch-sales-accounts | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | BranchSalesAccounts | POST | Angular Default | /settings | /settings/branch-sales-accounts | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | ChangeFiscalPeriodStatus | POST | Angular Default | /settings | — | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | ConfigureBranchWarehouse | POST | Angular Default | /settings | — | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | ConfigurePosTerminalWarehouse | POST | Angular Default | /settings | — | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | CreatePosTerminal | POST | Angular Default | /settings | — | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Currencies | GET | Angular Default | /settings | /settings/currency-exchange-rates | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Currencies | POST | Angular Default | /settings | /settings/currency-exchange-rates | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | DeactivateUnit | POST | Angular Default | /settings | — | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Discounts | GET | Angular Default | /settings | /settings/discounts | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Discounts | POST | Angular Default | /settings | /settings/discounts | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | DocumentNumbers | GET | Angular Default | /settings | /settings/document-numbers | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | DocumentNumbers | POST | Angular Default | /settings | /settings/document-numbers | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | FiscalPeriods | GET | Angular Default | /settings | /settings/fiscal-periods | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | FiscalPeriods | POST | Angular Default | /settings | /settings/fiscal-periods | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | General | GET | Angular Default | /settings | /settings | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | General | POST | Angular Default | /settings | /settings | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Index | GET | Angular Default | /settings | /settings | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Inventory | GET | Angular Default | /settings | /settings/inventory | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Inventory | POST | Angular Default | /settings | /settings/inventory | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | InventoryAccess | GET | Angular Default | /settings | /settings/inventory-access | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Invoices | GET | Angular Default | /settings | /settings/document-numbers | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | OrganizationIdentity | GET | Angular Default | /settings | /settings/identity | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | OrganizationIdentity | POST | Angular Default | /settings | /settings/identity | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | OrganizationLogo | GET | Angular Default | /settings | — | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Pos | GET | Angular Default | /settings | /settings/pos-experience | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Pos | POST | Angular Default | /settings | /settings/pos-experience | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | PurchaseMatching | GET | Angular Default | /settings | /settings/purchase-matching | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | PurchaseMatching | POST | Angular Default | /settings | /settings/purchase-matching | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | PurchaseSourcingPolicy | GET | Angular Default | /settings | /settings/purchase-sourcing | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | PurchaseSourcingPolicy | POST | Angular Default | /settings | /settings/purchase-sourcing | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Units | GET | Angular Default | /settings | /settings/measurement-units | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | Units | POST | Angular Default | /settings | /settings/measurement-units | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Settings | WorkflowCatalog | GET | Angular Default | /settings | /settings/workflow-catalog | MiniStore.Web/Controllers/Settings/SettingsController.cs |
| Subscription | Checkout | GET, POST | Angular Default | /subscription | — | MiniStore.Web/Controllers/Saas/SubscriptionController.cs |
| Subscription | Index | GET | Angular Default | /subscription | /subscription | MiniStore.Web/Controllers/Saas/SubscriptionController.cs |
| SupplierIntegrations | File | GET | Angular Default | /purchases/integrations | — | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | RedirectToAction | POST | Angular Default | /purchases/integrations | — | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | RedirectToAction | POST | Angular Default | /purchases/integrations | — | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | RedirectToAction | POST | Angular Default | /purchases/integrations | — | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | RedirectToDocument | POST | Angular Default | /purchases/integrations | — | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | View | GET, POST | Angular Default | /purchases/integrations | — | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierIntegrations | View | POST | Angular Default | /purchases/integrations | — | MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs |
| SupplierPortal | RecordDenied | GET | Angular Default | /supplier-portal/:token | — | MiniStore.Web/Controllers/Purchases/SupplierPortalController.cs |
| SupplierPortal | RecordDenied | POST | Angular Default | /supplier-portal/:token | — | MiniStore.Web/Controllers/Purchases/SupplierPortalController.cs |
| SupplierPortal | RecordDenied | POST | Angular Default | /supplier-portal/:token | — | MiniStore.Web/Controllers/Purchases/SupplierPortalController.cs |
| SupplierPortal | RecordDenied | POST | Angular Default | /supplier-portal/:token | — | MiniStore.Web/Controllers/Purchases/SupplierPortalController.cs |
| Users | Create | GET | Angular Default | /users | /users/new | MiniStore.Web/Controllers/Security/UsersController.cs |
| Users | Create | POST | Angular Default | /users | /users/new | MiniStore.Web/Controllers/Security/UsersController.cs |
| Users | Index | GET | Angular Default | /users | /users | MiniStore.Web/Controllers/Security/UsersController.cs |

## Control inventory

| MVC surface | Razor file | Forms | Buttons | Links | Referenced MVC actions | Source |
|---|---|---:|---:|---:|---|---|
| Account | AccessDenied.cshtml | 0 | 0 | 1 | Index | MiniStore.Web/Views/Account/AccessDenied.cshtml |
| Account | Login.cshtml | 1 | 1 | 0 | Login | MiniStore.Web/Views/Account/Login.cshtml |
| Account | Register.cshtml | 1 | 4 | 0 | — | MiniStore.Web/Views/Account/Register.cshtml |
| CompanyOnboarding | Index.cshtml | 1 | 2 | 0 | — | MiniStore.Web/Views/CompanyOnboarding/Index.cshtml |
| Dashboard | Index.cshtml | 0 | 0 | 2 | @item.Action, @primary.Action | MiniStore.Web/Views/Dashboard/Index.cshtml |
| Home | Index.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Views/Home/Index.cshtml |
| Platform/_ViewStart.cshtml | _ViewStart.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Areas/Platform/Views/_ViewStart.cshtml |
| Platform/Platform | Companies.cshtml | 1 | 1 | 0 | — | MiniStore.Web/Areas/Platform/Views/Platform/Companies.cshtml |
| Platform/Platform | CreatePlan.cshtml | 1 | 1 | 0 | — | MiniStore.Web/Areas/Platform/Views/Platform/CreatePlan.cshtml |
| Platform/Platform | CreatePromotion.cshtml | 1 | 1 | 0 | — | MiniStore.Web/Areas/Platform/Views/Platform/CreatePromotion.cshtml |
| Platform/Platform | Index.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Areas/Platform/Views/Platform/Index.cshtml |
| Platform/Platform | PendingPayments.cshtml | 1 | 1 | 0 | — | MiniStore.Web/Areas/Platform/Views/Platform/PendingPayments.cshtml |
| Platform/Platform | Plans.cshtml | 0 | 0 | 1 | — | MiniStore.Web/Areas/Platform/Views/Platform/Plans.cshtml |
| Platform/Platform | Promotions.cshtml | 0 | 0 | 1 | — | MiniStore.Web/Areas/Platform/Views/Platform/Promotions.cshtml |
| Platform/PlatformAccount | Login.cshtml | 2 | 2 | 0 | — | MiniStore.Web/Areas/Platform/Views/PlatformAccount/Login.cshtml |
| Platform/PlatformAuditEvents | Index.cshtml | 3 | 4 | 1 | — | MiniStore.Web/Areas/Platform/Views/PlatformAuditEvents/Index.cshtml |
| Platform/PlatformAuditRetention | Index.cshtml | 4 | 4 | 0 | Checkpoint, CreateHold, ReleaseHold, Run | MiniStore.Web/Areas/Platform/Views/PlatformAuditRetention/Index.cshtml |
| Roles | _Form.cshtml | 1 | 1 | 1 | @(editing ? , Index | MiniStore.Web/Views/Roles/_Form.cshtml |
| Roles | Create.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Views/Roles/Create.cshtml |
| Roles | Edit.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Views/Roles/Edit.cshtml |
| Roles | Index.cshtml | 1 | 1 | 2 | Create, Delete, Edit | MiniStore.Web/Views/Roles/Index.cshtml |
| Settings | _OrganizationIdentityField.cshtml | 0 | 1 | 0 | — | MiniStore.Web/Views/Settings/_OrganizationIdentityField.cshtml |
| Settings | Accounting.cshtml | 1 | 1 | 2 | Accounting, BranchSalesAccounts, Index | MiniStore.Web/Views/Settings/Accounting.cshtml |
| Settings | BranchSalesAccounts.cshtml | 1 | 1 | 1 | Accounting, BranchSalesAccounts | MiniStore.Web/Views/Settings/BranchSalesAccounts.cshtml |
| Settings | Currencies.cshtml | 1 | 1 | 1 | Currencies, Index | MiniStore.Web/Views/Settings/Currencies.cshtml |
| Settings | Discounts.cshtml | 1 | 1 | 1 | Discounts, Index | MiniStore.Web/Views/Settings/Discounts.cshtml |
| Settings | DocumentNumbers.cshtml | 1 | 1 | 0 | DocumentNumbers | MiniStore.Web/Views/Settings/DocumentNumbers.cshtml |
| Settings | FiscalPeriods.cshtml | 2 | 2 | 1 | ChangeFiscalPeriodStatus, FiscalPeriods, Index | MiniStore.Web/Views/Settings/FiscalPeriods.cshtml |
| Settings | General.cshtml | 1 | 1 | 2 | General, Index, OrganizationIdentity | MiniStore.Web/Views/Settings/General.cshtml |
| Settings | Index.cshtml | 0 | 1 | 1 | @card.Action | MiniStore.Web/Views/Settings/Index.cshtml |
| Settings | Inventory.cshtml | 1 | 1 | 3 | Index, Inventory, InventoryAccess | MiniStore.Web/Views/Settings/Inventory.cshtml |
| Settings | InventoryAccess.cshtml | 3 | 3 | 1 | ConfigureBranchWarehouse, ConfigurePosTerminalWarehouse, CreatePosTerminal, Inventory | MiniStore.Web/Views/Settings/InventoryAccess.cshtml |
| Settings | OrganizationIdentity.cshtml | 1 | 2 | 1 | Index, OrganizationIdentity | MiniStore.Web/Views/Settings/OrganizationIdentity.cshtml |
| Settings | Pos.cshtml | 2 | 2 | 1 | InventoryAccess, Pos | MiniStore.Web/Views/Settings/Pos.cshtml |
| Settings | PurchaseMatching.cshtml | 1 | 1 | 1 | Index, PurchaseMatching | MiniStore.Web/Views/Settings/PurchaseMatching.cshtml |
| Settings | PurchaseSourcingPolicy.cshtml | 1 | 1 | 1 | Index, PurchaseSourcingPolicy | MiniStore.Web/Views/Settings/PurchaseSourcingPolicy.cshtml |
| Settings | Units.cshtml | 2 | 2 | 1 | DeactivateUnit, Index, Units | MiniStore.Web/Views/Settings/Units.cshtml |
| Settings | WorkflowCatalog.cshtml | 0 | 0 | 3 | @step.Action, Index | MiniStore.Web/Views/Settings/WorkflowCatalog.cshtml |
| Subscription | Checkout.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Views/Subscription/Checkout.cshtml |
| Subscription | Index.cshtml | 1 | 1 | 0 | Checkout | MiniStore.Web/Views/Subscription/Index.cshtml |
| SupplierIntegrations | CredentialCreated.cshtml | 0 | 0 | 1 | Index | MiniStore.Web/Views/SupplierIntegrations/CredentialCreated.cshtml |
| SupplierIntegrations | Index.cshtml | 3 | 3 | 1 | CreateCredential, DeactivateCredential, Retry, Submission | MiniStore.Web/Views/SupplierIntegrations/Index.cshtml |
| SupplierIntegrations | PortalAccessCreated.cshtml | 0 | 0 | 1 | Index | MiniStore.Web/Views/SupplierIntegrations/PortalAccessCreated.cshtml |
| SupplierIntegrations | Submission.cshtml | 0 | 0 | 2 | DownloadSubmission, Index | MiniStore.Web/Views/SupplierIntegrations/Submission.cshtml |
| SupplierPortal | Index.cshtml | 3 | 3 | 0 | ConfirmOrder, DeliveryNotice, Quotation | MiniStore.Web/Views/SupplierPortal/Index.cshtml |
| SupplierPortal | Unavailable.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Views/SupplierPortal/Unavailable.cshtml |
| Users | Create.cshtml | 1 | 1 | 1 | Create, Index | MiniStore.Web/Views/Users/Create.cshtml |
| Users | Index.cshtml | 0 | 0 | 1 | Create | MiniStore.Web/Views/Users/Index.cshtml |
| Views | _ViewImports.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Views/_ViewImports.cshtml |
| Views | _ViewStart.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Views/_ViewStart.cshtml |

The cutover allowlist applies only to GET/HEAD requests. Mutations, downloads, and every action without an explicit cutover route remain on MVC.
