# MVC to Angular migration registry

> Generated from source by scripts/generate-mvc-migration-registry.ps1. Do not edit rows manually.

## Reconciliation

- Controllers discovered: 118 total; 30 MVC; 88 API.
- User-facing MVC views discovered: 93.
- MVC actions discovered: 214.
- Razor files included in the control inventory: 100.
- Angular application routes discovered: 194.
- Presentation surfaces with a verified Angular counterpart: 29.
- Remaining presentation surfaces without a mapped Angular counterpart: 0.
- Mutation-only MVC compatibility surfaces: 1 (Language).
- Physically retired presentation actions with legacy redirects: 106 across 52 surfaces (AnalyticsCenter, AuditEvents, Branches, CustomerCreditNotes, CustomerPayments, CustomerReturnReceipts, GoodsReceiptReturns, GoodsReceipts, InventoryAdjustments, InventoryBalances, InventoryInsights, InventoryReconciliation, InventoryReservations, InventoryScanning, InventoryTracking, JournalEntries, LandedCosts, LocationMovements, PaymentMethods, ProductAttributes, ProductCategories, Products, ProductStocks, ProductTemplates, Public, PurchaseAccountingReconciliation, PurchaseAgreements, PurchaseAnalytics, PurchaseApprovalRules, PurchaseDocumentReversals, PurchaseOrders, PurchaseReplenishment, PurchaseReturns, PurchaseServiceAcceptances, PurchaseTemplates, Recipes, Replenishment, SalesApprovalRules, SalesDeliveries, SalesReturns, StockMovements, StockTransactions, StockTransfers, SupplierCommunications, SupplierCreditNotes, SupplierPayments, SupplierQuotations, Suppliers, TaxRates, UnassignedStock, WarehouseLocations, Warehouses).
- Allowlisted Angular-default cutover actions: 116.

Remaining presentation surfaces: none.

Statuses follow the migration lifecycle plus MVC Compatibility (mutation-only). A mutation-only boundary has no GET presentation surface and is intentionally excluded from AngularDefault redirects.

## View inventory

| MVC surface | View | Migration status | Angular counterpart | Source |
|---|---|---|---|---|
| Account | AccessDenied.cshtml | Angular Default | /login, /register, /access-denied | MiniStore.Web/Views/Account/AccessDenied.cshtml |
| Account | Login.cshtml | Angular Default | /login, /register, /access-denied | MiniStore.Web/Views/Account/Login.cshtml |
| Account | Register.cshtml | Angular Default | /login, /register, /access-denied | MiniStore.Web/Views/Account/Register.cshtml |
| Accounts | Create.cshtml | Angular Default | /accounting/accounts | MiniStore.Web/Views/Accounts/Create.cshtml |
| Accounts | Index.cshtml | Angular Default | /accounting/accounts | MiniStore.Web/Views/Accounts/Index.cshtml |
| CompanyOnboarding | Index.cshtml | Angular Default | /onboarding | MiniStore.Web/Views/CompanyOnboarding/Index.cshtml |
| CustomerInvoices | Create.cshtml | Angular Default | /sales/invoices | MiniStore.Web/Views/CustomerInvoices/Create.cshtml |
| CustomerInvoices | Details.cshtml | Angular Default | /sales/invoices | MiniStore.Web/Views/CustomerInvoices/Details.cshtml |
| CustomerInvoices | Index.cshtml | Angular Default | /sales/invoices | MiniStore.Web/Views/CustomerInvoices/Index.cshtml |
| CustomerReturnRequests | Create.cshtml | Angular Default | /sales/return-requests | MiniStore.Web/Views/CustomerReturnRequests/Create.cshtml |
| CustomerReturnRequests | Details.cshtml | Angular Default | /sales/return-requests | MiniStore.Web/Views/CustomerReturnRequests/Details.cshtml |
| CustomerReturnRequests | Index.cshtml | Angular Default | /sales/return-requests | MiniStore.Web/Views/CustomerReturnRequests/Index.cshtml |
| Customers | Create.cshtml | Angular Default | /customers | MiniStore.Web/Views/Customers/Create.cshtml |
| Customers | Details.cshtml | Angular Default | /customers | MiniStore.Web/Views/Customers/Details.cshtml |
| Customers | Edit.cshtml | Angular Default | /customers | MiniStore.Web/Views/Customers/Edit.cshtml |
| Customers | Index.cshtml | Angular Default | /customers | MiniStore.Web/Views/Customers/Index.cshtml |
| Dashboard | Index.cshtml | Angular Default | / | MiniStore.Web/Views/Dashboard/Index.cshtml |
| DocumentDesigns | Create.cshtml | Angular Default | /settings/document-designs | MiniStore.Web/Views/DocumentDesigns/Create.cshtml |
| DocumentDesigns | Details.cshtml | Angular Default | /settings/document-designs | MiniStore.Web/Views/DocumentDesigns/Details.cshtml |
| DocumentDesigns | Index.cshtml | Angular Default | /settings/document-designs | MiniStore.Web/Views/DocumentDesigns/Index.cshtml |
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
| PurchaseRequests | Create.cshtml | Angular Default | /purchases/requests | MiniStore.Web/Views/PurchaseRequests/Create.cshtml |
| PurchaseRequests | Details.cshtml | Angular Default | /purchases/requests | MiniStore.Web/Views/PurchaseRequests/Details.cshtml |
| PurchaseRequests | Index.cshtml | Angular Default | /purchases/requests | MiniStore.Web/Views/PurchaseRequests/Index.cshtml |
| Purchases | Create.cshtml | Angular Default | /purchases/legacy | MiniStore.Web/Views/Purchases/Create.cshtml |
| Purchases | Details.cshtml | Angular Default | /purchases/legacy | MiniStore.Web/Views/Purchases/Details.cshtml |
| Purchases | Index.cshtml | Angular Default | /purchases/legacy | MiniStore.Web/Views/Purchases/Index.cshtml |
| PurchaseSourcing | Create.cshtml | Angular Default | /purchases/sourcing | MiniStore.Web/Views/PurchaseSourcing/Create.cshtml |
| PurchaseSourcing | Details.cshtml | Angular Default | /purchases/sourcing | MiniStore.Web/Views/PurchaseSourcing/Details.cshtml |
| PurchaseSourcing | Direct.cshtml | Angular Default | /purchases/sourcing | MiniStore.Web/Views/PurchaseSourcing/Direct.cshtml |
| PurchaseSourcing | Index.cshtml | Angular Default | /purchases/sourcing | MiniStore.Web/Views/PurchaseSourcing/Index.cshtml |
| PurchaseSourcing | Print.cshtml | Angular Default | /purchases/sourcing | MiniStore.Web/Views/PurchaseSourcing/Print.cshtml |
| Roles | Create.cshtml | Angular Default | /roles | MiniStore.Web/Views/Roles/Create.cshtml |
| Roles | Edit.cshtml | Angular Default | /roles | MiniStore.Web/Views/Roles/Edit.cshtml |
| Roles | Index.cshtml | Angular Default | /roles | MiniStore.Web/Views/Roles/Index.cshtml |
| Sales | Create.cshtml | Angular Default | /sales/legacy | MiniStore.Web/Views/Sales/Create.cshtml |
| Sales | Details.cshtml | Angular Default | /sales/legacy | MiniStore.Web/Views/Sales/Details.cshtml |
| Sales | Index.cshtml | Angular Default | /sales/legacy | MiniStore.Web/Views/Sales/Index.cshtml |
| Sales | Pos.cshtml | Angular Default | /sales/legacy | MiniStore.Web/Views/Sales/Pos.cshtml |
| SalesOrders | Details.cshtml | Angular Default | /sales/orders | MiniStore.Web/Views/SalesOrders/Details.cshtml |
| SalesOrders | Index.cshtml | Angular Default | /sales/orders | MiniStore.Web/Views/SalesOrders/Index.cshtml |
| SalesOrders | Reserve.cshtml | Angular Default | /sales/orders | MiniStore.Web/Views/SalesOrders/Reserve.cshtml |
| SalesPriceLists | Index.cshtml | Angular Default | /sales/pricing | MiniStore.Web/Views/SalesPriceLists/Index.cshtml |
| SalesQuotations | Create.cshtml | Angular Default | /sales/quotations | MiniStore.Web/Views/SalesQuotations/Create.cshtml |
| SalesQuotations | Details.cshtml | Angular Default | /sales/quotations | MiniStore.Web/Views/SalesQuotations/Details.cshtml |
| SalesQuotations | Index.cshtml | Angular Default | /sales/quotations | MiniStore.Web/Views/SalesQuotations/Index.cshtml |
| SalesQuotationTemplates | Create.cshtml | Angular Default | /sales/quotation-templates | MiniStore.Web/Views/SalesQuotationTemplates/Create.cshtml |
| SalesQuotationTemplates | Details.cshtml | Angular Default | /sales/quotation-templates | MiniStore.Web/Views/SalesQuotationTemplates/Details.cshtml |
| SalesQuotationTemplates | Index.cshtml | Angular Default | /sales/quotation-templates | MiniStore.Web/Views/SalesQuotationTemplates/Index.cshtml |
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
| SupplierPurchasing | ImportPreview.cshtml | Angular Default | /purchases/supplier-terms | MiniStore.Web/Views/SupplierPurchasing/ImportPreview.cshtml |
| SupplierPurchasing | Index.cshtml | Angular Default | /purchases/supplier-terms | MiniStore.Web/Views/SupplierPurchasing/Index.cshtml |
| SupplierPurchasing | PriceBreaks.cshtml | Angular Default | /purchases/supplier-terms | MiniStore.Web/Views/SupplierPurchasing/PriceBreaks.cshtml |
| SupplierPurchasing | PriceExceptions.cshtml | Angular Default | /purchases/supplier-terms | MiniStore.Web/Views/SupplierPurchasing/PriceExceptions.cshtml |
| SupplierPurchasing | PriceHistory.cshtml | Angular Default | /purchases/supplier-terms | MiniStore.Web/Views/SupplierPurchasing/PriceHistory.cshtml |
| Users | Create.cshtml | Angular Default | /users | MiniStore.Web/Views/Users/Create.cshtml |
| Users | Index.cshtml | Angular Default | /users | MiniStore.Web/Views/Users/Index.cshtml |
| VendorBills | Create.cshtml | Angular Default | /purchases/bills | MiniStore.Web/Views/VendorBills/Create.cshtml |
| VendorBills | Details.cshtml | Angular Default | /purchases/bills | MiniStore.Web/Views/VendorBills/Details.cshtml |
| VendorBills | Index.cshtml | Angular Default | /purchases/bills | MiniStore.Web/Views/VendorBills/Index.cshtml |

## Action inventory

| MVC surface | Action | HTTP verbs | Migration status | Angular counterpart | Angular-default cutover route | Source |
|---|---|---|---|---|---|---|
| Account | AccessDenied | GET | Angular Default | /login, /register, /access-denied | /access-denied | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Login | GET | Angular Default | /login, /register, /access-denied | /login | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Login | POST | Angular Default | /login, /register, /access-denied | /login | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Logout | POST | Angular Default | /login, /register, /access-denied | — | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Register | GET | Angular Default | /login, /register, /access-denied | /register | MiniStore.Web/Controllers/Security/AccountController.cs |
| Account | Register | POST | Angular Default | /login, /register, /access-denied | /register | MiniStore.Web/Controllers/Security/AccountController.cs |
| Accounts | Create | GET | Angular Default | /accounting/accounts | — | MiniStore.Web/Controllers/Accounting/AccountsController.cs |
| Accounts | Create | GET, POST | Angular Default | /accounting/accounts | — | MiniStore.Web/Controllers/Accounting/AccountsController.cs |
| Accounts | Index | ANY | Angular Default | /accounting/accounts | /accounting/accounts | MiniStore.Web/Controllers/Accounting/AccountsController.cs |
| CompanyOnboarding | Complete | POST | Angular Default | /onboarding | — | MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs |
| CompanyOnboarding | Index | GET | Angular Default | /onboarding | /onboarding | MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs |
| CompanyOnboarding | Skip | POST | Angular Default | /onboarding | — | MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs |
| CustomerInvoices | Approve | POST | Angular Default | /sales/invoices | — | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Cancel | POST | Angular Default | /sales/invoices | — | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Create | GET | Angular Default | /sales/invoices | /sales/invoices/new?salesOrderId={salesOrderId} | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Create | POST | Angular Default | /sales/invoices | /sales/invoices/new?salesOrderId={salesOrderId} | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Details | GET | Angular Default | /sales/invoices | /sales/invoices/{id} | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Index | GET | Angular Default | /sales/invoices | /sales/invoices | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Post | POST | Angular Default | /sales/invoices | — | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerInvoices | Reverse | POST | Angular Default | /sales/invoices | — | MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs |
| CustomerReturnRequests | Approve | POST | Angular Default | /sales/return-requests | — | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| CustomerReturnRequests | Cancel | POST | Angular Default | /sales/return-requests | — | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| CustomerReturnRequests | Create | GET | Angular Default | /sales/return-requests | /sales/return-requests/new?salesDeliveryId={salesDeliveryId} | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| CustomerReturnRequests | Create | POST | Angular Default | /sales/return-requests | /sales/return-requests/new?salesDeliveryId={salesDeliveryId} | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| CustomerReturnRequests | Details | GET | Angular Default | /sales/return-requests | /sales/return-requests/{id} | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| CustomerReturnRequests | Index | GET | Angular Default | /sales/return-requests | /sales/return-requests | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| CustomerReturnRequests | Reject | POST | Angular Default | /sales/return-requests | — | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| CustomerReturnRequests | Submit | POST | Angular Default | /sales/return-requests | — | MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs |
| Customers | Archive | POST | Angular Default | /customers | — | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | ArchiveAddress | POST | Angular Default | /customers | — | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | ArchiveContact | POST | Angular Default | /customers | — | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | Create | GET | Angular Default | /customers | /customers/new | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | Create | POST | Angular Default | /customers | /customers/new | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | Details | GET | Angular Default | /customers | /customers/{id} | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | Edit | GET | Angular Default | /customers | /customers/{id}/edit | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | Edit | POST | Angular Default | /customers | /customers/{id}/edit | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | Index | GET | Angular Default | /customers | /customers | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | Restore | POST | Angular Default | /customers | — | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | RestoreAddress | POST | Angular Default | /customers | — | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | RestoreContact | POST | Angular Default | /customers | — | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | SaveAddress | POST | Angular Default | /customers | — | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Customers | SaveContact | POST | Angular Default | /customers | — | MiniStore.Web/Controllers/Customers/CustomersController.cs |
| Dashboard | Index | ANY | Angular Default | / | / | MiniStore.Web/Controllers/Home/DashboardController.cs |
| DocumentDesigns | Activate | POST | Angular Default | /settings/document-designs | — | MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs |
| DocumentDesigns | Archive | POST | Angular Default | /settings/document-designs | — | MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs |
| DocumentDesigns | Create | GET | Angular Default | /settings/document-designs | /settings/document-designs/new | MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs |
| DocumentDesigns | Create | POST | Angular Default | /settings/document-designs | /settings/document-designs/new | MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs |
| DocumentDesigns | Details | GET | Angular Default | /settings/document-designs | /settings/document-designs/{id} | MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs |
| DocumentDesigns | Index | GET | Angular Default | /settings/document-designs | /settings/document-designs | MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs |
| DocumentDesigns | SetDefault | POST | Angular Default | /settings/document-designs | — | MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs |
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
| PurchaseRequests | Index | GET | Angular Default | /purchases/requests | /purchases/requests | MiniStore.Web/Controllers/Purchases/PurchaseRequestsController.cs |
| PurchaseRequests | RedirectToAction | GET, POST | Angular Default | /purchases/requests | — | MiniStore.Web/Controllers/Purchases/PurchaseRequestsController.cs |
| PurchaseRequests | RedirectToAction | GET, POST | Angular Default | /purchases/requests | — | MiniStore.Web/Controllers/Purchases/PurchaseRequestsController.cs |
| PurchaseRequests | RedirectToAction | GET, POST | Angular Default | /purchases/requests | — | MiniStore.Web/Controllers/Purchases/PurchaseRequestsController.cs |
| PurchaseRequests | RedirectToAction | GET, POST | Angular Default | /purchases/requests | — | MiniStore.Web/Controllers/Purchases/PurchaseRequestsController.cs |
| PurchaseRequests | RedirectToAction | GET, POST | Angular Default | /purchases/requests | — | MiniStore.Web/Controllers/Purchases/PurchaseRequestsController.cs |
| Purchases | Create | GET | Angular Default | /purchases/legacy | — | MiniStore.Web/Controllers/Purchases/PurchasesController.cs |
| Purchases | Create | POST | Angular Default | /purchases/legacy | — | MiniStore.Web/Controllers/Purchases/PurchasesController.cs |
| Purchases | Details | GET | Angular Default | /purchases/legacy | /purchases/legacy/{id} | MiniStore.Web/Controllers/Purchases/PurchasesController.cs |
| Purchases | Index | GET | Angular Default | /purchases/legacy | /purchases/legacy | MiniStore.Web/Controllers/Purchases/PurchasesController.cs |
| Purchases | Post | POST | Angular Default | /purchases/legacy | — | MiniStore.Web/Controllers/Purchases/PurchasesController.cs |
| PurchaseSourcing | Attachment | GET | Angular Default | /purchases/sourcing | — | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Create | GET | Angular Default | /purchases/sourcing | — | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Create | POST | Angular Default | /purchases/sourcing | — | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Details | GET | Angular Default | /purchases/sourcing | /purchases/sourcing/{id} | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Direct | GET | Angular Default | /purchases/sourcing | — | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Direct | POST | Angular Default | /purchases/sourcing | — | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Index | GET | Angular Default | /purchases/sourcing | /purchases/sourcing | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| PurchaseSourcing | Print | GET | Angular Default | /purchases/sourcing | /purchases/sourcing/{id}/print | MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs |
| Roles | Create | GET | Angular Default | /roles | /roles/new | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Create | POST | Angular Default | /roles | /roles/new | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Delete | POST | Angular Default | /roles | — | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Edit | GET | Angular Default | /roles | /roles/{id}/edit | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Edit | POST | Angular Default | /roles | /roles/{id}/edit | MiniStore.Web/Controllers/Security/RolesController.cs |
| Roles | Index | GET | Angular Default | /roles | /roles | MiniStore.Web/Controllers/Security/RolesController.cs |
| Sales | Create | ANY | Angular Default | /sales/legacy | /sales/legacy/new | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Create | POST | Angular Default | /sales/legacy | /sales/legacy/new | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Details | ANY | Angular Default | /sales/legacy | /sales/legacy/{id} | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Index | ANY | Angular Default | /sales/legacy | /sales/legacy | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Pos | GET | Angular Default | /sales/legacy | /sales/pos | MiniStore.Web/Controllers/Sales/SalesController.cs |
| Sales | Post | POST | Angular Default | /sales/legacy | — | MiniStore.Web/Controllers/Sales/SalesController.cs |
| SalesOrders | ApproveException | POST | Angular Default | /sales/orders | — | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | Cancel | POST | Angular Default | /sales/orders | — | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | Confirm | POST | Angular Default | /sales/orders | — | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | CreateFromQuotation | POST | Angular Default | /sales/orders | — | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | Details | GET | Angular Default | /sales/orders | /sales/orders/{id} | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | Index | GET | Angular Default | /sales/orders | /sales/orders | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | RejectException | POST | Angular Default | /sales/orders | — | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | ReleaseReservation | POST | Angular Default | /sales/orders | — | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | Reserve | GET | Angular Default | /sales/orders | /sales/orders/{id}/reservations | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesOrders | Reserve | POST | Angular Default | /sales/orders | /sales/orders/{id}/reservations | MiniStore.Web/Controllers/Sales/SalesOrdersController.cs |
| SalesPriceLists | AddRule | POST | Angular Default | /sales/pricing | — | MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs |
| SalesPriceLists | Create | POST | Angular Default | /sales/pricing | — | MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs |
| SalesPriceLists | Evaluate | POST | Angular Default | /sales/pricing | — | MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs |
| SalesPriceLists | Index | GET | Angular Default | /sales/pricing | /sales/pricing | MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs |
| SalesPriceLists | SetListActive | POST | Angular Default | /sales/pricing | — | MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs |
| SalesPriceLists | SetRuleActive | POST | Angular Default | /sales/pricing | — | MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs |
| SalesQuotations | Accept | POST | Angular Default | /sales/quotations | — | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Cancel | POST | Angular Default | /sales/quotations | — | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Create | GET | Angular Default | /sales/quotations | /sales/quotations/new | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Create | POST | Angular Default | /sales/quotations | /sales/quotations/new | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Details | GET | Angular Default | /sales/quotations | /sales/quotations/{id} | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | DownloadPdf | GET | Angular Default | /sales/quotations | — | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Expire | POST | Angular Default | /sales/quotations | — | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Index | GET | Angular Default | /sales/quotations | /sales/quotations | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Reject | POST | Angular Default | /sales/quotations | — | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotations | Send | POST | Angular Default | /sales/quotations | — | MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs |
| SalesQuotationTemplates | Activate | POST | Angular Default | /sales/quotation-templates | — | MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs |
| SalesQuotationTemplates | Archive | POST | Angular Default | /sales/quotation-templates | — | MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs |
| SalesQuotationTemplates | Create | GET | Angular Default | /sales/quotation-templates | /sales/quotation-templates/new | MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs |
| SalesQuotationTemplates | Create | POST | Angular Default | /sales/quotation-templates | /sales/quotation-templates/new | MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs |
| SalesQuotationTemplates | Details | GET | Angular Default | /sales/quotation-templates | /sales/quotation-templates/{id} | MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs |
| SalesQuotationTemplates | Index | GET | Angular Default | /sales/quotation-templates | /sales/quotation-templates | MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs |
| SalesQuotationTemplates | SetDefault | POST | Angular Default | /sales/quotation-templates | — | MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs |
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
| SupplierPurchasing | ExportCsv | GET | Angular Default | /purchases/supplier-terms | — | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | ExportExcel | GET | Angular Default | /purchases/supplier-terms | — | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | ImportCsv | POST | Angular Default | /purchases/supplier-terms | — | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | ImportExcel | POST | Angular Default | /purchases/supplier-terms | — | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | Index | GET | Angular Default | /purchases/supplier-terms | /purchases/supplier-terms | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | PreviewCsv | POST | Angular Default | /purchases/supplier-terms | — | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | PreviewExcel | POST | Angular Default | /purchases/supplier-terms | — | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | PriceBreaks | GET | Angular Default | /purchases/supplier-terms | /purchases/supplier-terms/{id}/price-breaks | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | PriceExceptions | GET | Angular Default | /purchases/supplier-terms | /purchases/supplier-terms/exceptions | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | PriceHistory | GET | Angular Default | /purchases/supplier-terms | /purchases/supplier-terms/{id}/history | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | Save | POST | Angular Default | /purchases/supplier-terms | — | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | SaveBulk | POST | Angular Default | /purchases/supplier-terms | — | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | SavePriceBreaks | POST | Angular Default | /purchases/supplier-terms | — | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| SupplierPurchasing | SetActive | POST | Angular Default | /purchases/supplier-terms | — | MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs |
| Users | Create | GET | Angular Default | /users | /users/new | MiniStore.Web/Controllers/Security/UsersController.cs |
| Users | Create | POST | Angular Default | /users | /users/new | MiniStore.Web/Controllers/Security/UsersController.cs |
| Users | Index | GET | Angular Default | /users | /users | MiniStore.Web/Controllers/Security/UsersController.cs |
| VendorBills | Approve | POST | Angular Default | /purchases/bills | — | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | ApproveOverride | POST | Angular Default | /purchases/bills | — | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | Cancel | POST | Angular Default | /purchases/bills | — | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | Create | GET | Angular Default | /purchases/bills | /purchases/bills/new?purchaseOrderId={purchaseOrderId} | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | Create | POST | Angular Default | /purchases/bills | /purchases/bills/new?purchaseOrderId={purchaseOrderId} | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | Details | GET | Angular Default | /purchases/bills | /purchases/bills/{id} | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | DownloadAttachment | GET | Angular Default | /purchases/bills | — | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | Index | GET | Angular Default | /purchases/bills | /purchases/bills | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | Post | POST | Angular Default | /purchases/bills | — | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | RedirectToAction | POST | Angular Default | /purchases/bills | — | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |
| VendorBills | RedirectToAction | POST | Angular Default | /purchases/bills | — | MiniStore.Web/Controllers/Purchases/VendorBillsController.cs |

## Control inventory

| MVC surface | Razor file | Forms | Buttons | Links | Referenced MVC actions | Source |
|---|---|---:|---:|---:|---|---|
| Account | AccessDenied.cshtml | 0 | 0 | 1 | Index | MiniStore.Web/Views/Account/AccessDenied.cshtml |
| Account | Login.cshtml | 1 | 1 | 0 | Login | MiniStore.Web/Views/Account/Login.cshtml |
| Account | Register.cshtml | 1 | 4 | 0 | — | MiniStore.Web/Views/Account/Register.cshtml |
| Accounts | Create.cshtml | 1 | 1 | 0 | — | MiniStore.Web/Views/Accounts/Create.cshtml |
| Accounts | Index.cshtml | 0 | 0 | 1 | Create | MiniStore.Web/Views/Accounts/Index.cshtml |
| CompanyOnboarding | Index.cshtml | 1 | 2 | 0 | — | MiniStore.Web/Views/CompanyOnboarding/Index.cshtml |
| CustomerInvoices | Create.cshtml | 1 | 1 | 1 | Create, Details | MiniStore.Web/Views/CustomerInvoices/Create.cshtml |
| CustomerInvoices | Details.cshtml | 4 | 4 | 7 | Approve, Cancel, Create, Details, Index, Post, Reverse | MiniStore.Web/Views/CustomerInvoices/Details.cshtml |
| CustomerInvoices | Index.cshtml | 1 | 1 | 1 | Details | MiniStore.Web/Views/CustomerInvoices/Index.cshtml |
| CustomerReturnRequests | Create.cshtml | 1 | 1 | 1 | Create, Details | MiniStore.Web/Views/CustomerReturnRequests/Create.cshtml |
| CustomerReturnRequests | Details.cshtml | 4 | 4 | 4 | Approve, Cancel, Create, Details, Index, Reject, Submit | MiniStore.Web/Views/CustomerReturnRequests/Details.cshtml |
| CustomerReturnRequests | Index.cshtml | 1 | 1 | 2 | Details, Index | MiniStore.Web/Views/CustomerReturnRequests/Index.cshtml |
| Customers | _Form.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Views/Customers/_Form.cshtml |
| Customers | Create.cshtml | 1 | 1 | 2 | Create, Index | MiniStore.Web/Views/Customers/Create.cshtml |
| Customers | Details.cshtml | 8 | 8 | 2 | @(a.IsActive?, @(c.IsActive?, Archive, Edit, Index, Restore, SaveAddress, SaveContact | MiniStore.Web/Views/Customers/Details.cshtml |
| Customers | Edit.cshtml | 1 | 1 | 2 | Details, Edit | MiniStore.Web/Views/Customers/Edit.cshtml |
| Customers | Index.cshtml | 1 | 1 | 4 | Create, Details, Edit | MiniStore.Web/Views/Customers/Index.cshtml |
| Dashboard | Index.cshtml | 0 | 0 | 2 | @item.Action, @primary.Action | MiniStore.Web/Views/Dashboard/Index.cshtml |
| DocumentDesigns | _Section.cshtml | 0 | 1 | 0 | — | MiniStore.Web/Views/DocumentDesigns/_Section.cshtml |
| DocumentDesigns | Create.cshtml | 1 | 2 | 1 | Create, Index | MiniStore.Web/Views/DocumentDesigns/Create.cshtml |
| DocumentDesigns | Details.cshtml | 3 | 3 | 2 | Activate, Archive, Create, Index, SetDefault | MiniStore.Web/Views/DocumentDesigns/Details.cshtml |
| DocumentDesigns | Index.cshtml | 1 | 1 | 4 | Create, Details, Index | MiniStore.Web/Views/DocumentDesigns/Index.cshtml |
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
| PurchaseRequests | Create.cshtml | 1 | 3 | 1 | Create, Index | MiniStore.Web/Views/PurchaseRequests/Create.cshtml |
| PurchaseRequests | Details.cshtml | 4 | 4 | 2 | Approve, Cancel, Create, Index, Reject, Submit | MiniStore.Web/Views/PurchaseRequests/Details.cshtml |
| PurchaseRequests | Index.cshtml | 1 | 1 | 2 | Create, Details | MiniStore.Web/Views/PurchaseRequests/Index.cshtml |
| Purchases | Create.cshtml | 1 | 3 | 1 | Create, Index | MiniStore.Web/Views/Purchases/Create.cshtml |
| Purchases | Details.cshtml | 1 | 1 | 2 | Create, Index, Post | MiniStore.Web/Views/Purchases/Details.cshtml |
| Purchases | Index.cshtml | 0 | 0 | 2 | Create, Details | MiniStore.Web/Views/Purchases/Index.cshtml |
| PurchaseSourcing | Create.cshtml | 1 | 1 | 1 | Create, Details | MiniStore.Web/Views/PurchaseSourcing/Create.cshtml |
| PurchaseSourcing | Details.cshtml | 4 | 4 | 6 | Attachment, Comparison, Create, CreatePortalAccess, Details, Index, Prepare, Print, RevokePortalAccess | MiniStore.Web/Views/PurchaseSourcing/Details.cshtml |
| PurchaseSourcing | Direct.cshtml | 1 | 4 | 3 | Create, Direct, Index | MiniStore.Web/Views/PurchaseSourcing/Direct.cshtml |
| PurchaseSourcing | Index.cshtml | 1 | 1 | 2 | Details, Direct | MiniStore.Web/Views/PurchaseSourcing/Index.cshtml |
| PurchaseSourcing | Print.cshtml | 0 | 1 | 0 | — | MiniStore.Web/Views/PurchaseSourcing/Print.cshtml |
| Roles | _Form.cshtml | 1 | 1 | 1 | @(editing ? , Index | MiniStore.Web/Views/Roles/_Form.cshtml |
| Roles | Create.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Views/Roles/Create.cshtml |
| Roles | Edit.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Views/Roles/Edit.cshtml |
| Roles | Index.cshtml | 1 | 1 | 2 | Create, Delete, Edit | MiniStore.Web/Views/Roles/Index.cshtml |
| Sales | Create.cshtml | 1 | 3 | 1 | Create, Index | MiniStore.Web/Views/Sales/Create.cshtml |
| Sales | Details.cshtml | 1 | 1 | 2 | Create, Index, Post | MiniStore.Web/Views/Sales/Details.cshtml |
| Sales | Index.cshtml | 0 | 0 | 3 | Create, Details, Pos | MiniStore.Web/Views/Sales/Index.cshtml |
| Sales | Pos.cshtml | 1 | 6 | 1 | Create, Index | MiniStore.Web/Views/Sales/Pos.cshtml |
| SalesOrders | Details.cshtml | 4 | 4 | 8 | ApproveException, Cancel, Confirm, Create, Details, Index, RejectException, Reserve | MiniStore.Web/Views/SalesOrders/Details.cshtml |
| SalesOrders | Index.cshtml | 1 | 1 | 1 | Details | MiniStore.Web/Views/SalesOrders/Index.cshtml |
| SalesOrders | Reserve.cshtml | 2 | 2 | 1 | Details, ReleaseReservation, Reserve | MiniStore.Web/Views/SalesOrders/Reserve.cshtml |
| SalesPriceLists | Index.cshtml | 5 | 5 | 1 | AddRule, Create, Evaluate, Index, SetListActive, SetRuleActive | MiniStore.Web/Views/SalesPriceLists/Index.cshtml |
| SalesQuotations | Create.cshtml | 2 | 5 | 1 | Create, Index | MiniStore.Web/Views/SalesQuotations/Create.cshtml |
| SalesQuotations | Details.cshtml | 6 | 6 | 3 | Accept, Cancel, CreateFromQuotation, Details, DownloadPdf, Expire, Index, Reject, Send | MiniStore.Web/Views/SalesQuotations/Details.cshtml |
| SalesQuotations | Index.cshtml | 1 | 1 | 2 | Create, Details | MiniStore.Web/Views/SalesQuotations/Index.cshtml |
| SalesQuotationTemplates | Create.cshtml | 1 | 4 | 2 | Create, Index | MiniStore.Web/Views/SalesQuotationTemplates/Create.cshtml |
| SalesQuotationTemplates | Details.cshtml | 3 | 3 | 2 | Activate, Archive, Create, Index, SetDefault | MiniStore.Web/Views/SalesQuotationTemplates/Details.cshtml |
| SalesQuotationTemplates | Index.cshtml | 1 | 1 | 2 | Create, Details | MiniStore.Web/Views/SalesQuotationTemplates/Index.cshtml |
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
| SupplierPurchasing | ImportPreview.cshtml | 1 | 1 | 1 | @(Model.FileFormat == , Index | MiniStore.Web/Views/SupplierPurchasing/ImportPreview.cshtml |
| SupplierPurchasing | Index.cshtml | 5 | 12 | 7 | Edit, ExportCsv, ExportExcel, Index, PreviewCsv, PreviewExcel, PriceBreaks, PriceExceptions, PriceHistory, SaveBulk, SetActive | MiniStore.Web/Views/SupplierPurchasing/Index.cshtml |
| SupplierPurchasing | PriceBreaks.cshtml | 1 | 8 | 1 | Index, SavePriceBreaks | MiniStore.Web/Views/SupplierPurchasing/PriceBreaks.cshtml |
| SupplierPurchasing | PriceExceptions.cshtml | 0 | 0 | 10 | Index, PriceExceptions, PriceHistory | MiniStore.Web/Views/SupplierPurchasing/PriceExceptions.cshtml |
| SupplierPurchasing | PriceHistory.cshtml | 0 | 0 | 1 | Index | MiniStore.Web/Views/SupplierPurchasing/PriceHistory.cshtml |
| Users | Create.cshtml | 1 | 1 | 1 | Create, Index | MiniStore.Web/Views/Users/Create.cshtml |
| Users | Index.cshtml | 0 | 0 | 1 | Create | MiniStore.Web/Views/Users/Index.cshtml |
| VendorBills | Create.cshtml | 1 | 1 | 1 | Create, Details | MiniStore.Web/Views/VendorBills/Create.cshtml |
| VendorBills | Details.cshtml | 7 | 8 | 6 | AddAttachment, Approve, ApproveOverride, Cancel, Create, Details, DownloadAttachment, GenerateExtraction, Index, Post, ReviewExtraction | MiniStore.Web/Views/VendorBills/Details.cshtml |
| VendorBills | Index.cshtml | 1 | 1 | 1 | Details | MiniStore.Web/Views/VendorBills/Index.cshtml |
| Views | _ViewImports.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Views/_ViewImports.cshtml |
| Views | _ViewStart.cshtml | 0 | 0 | 0 | — | MiniStore.Web/Views/_ViewStart.cshtml |

The cutover allowlist applies only to GET/HEAD requests. Mutations, downloads, and every action without an explicit cutover route remain on MVC.
