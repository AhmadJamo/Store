# MVC to Angular migration registry

> Generated from source by `scripts/generate-mvc-migration-registry.ps1`. Do not edit rows manually.

## Reconciliation

- Controllers discovered: 89 total; 79 MVC; 10 API.
- User-facing MVC views discovered: 189.
- MVC actions discovered: 344.
- Razor files included in the control inventory: 198.
- Allowlisted Angular cutover actions: 11.

Statuses follow the migration lifecycle: Inventoried, API Ready, Angular Pilot, Parity Proven, Angular Default, MVC Retired. A pilot status does not change the default MVC route.

## View inventory

| MVC surface | View | Migration status | Angular route | Source |
|---|---|---|---|---|
| Account | AccessDenied.cshtml | Inventoried | — | `MiniStore.Web/Views/Account/AccessDenied.cshtml` |
| Account | Login.cshtml | Inventoried | — | `MiniStore.Web/Views/Account/Login.cshtml` |
| Account | Register.cshtml | Inventoried | — | `MiniStore.Web/Views/Account/Register.cshtml` |
| Accounts | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/Accounts/Create.cshtml` |
| Accounts | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Accounts/Index.cshtml` |
| AnalyticsCenter | Create.cshtml | Angular Pilot | /analytics | `MiniStore.Web/Views/AnalyticsCenter/Create.cshtml` |
| AnalyticsCenter | Index.cshtml | Angular Pilot | /analytics | `MiniStore.Web/Views/AnalyticsCenter/Index.cshtml` |
| AuditEvents | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/AuditEvents/Index.cshtml` |
| Branches | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/Branches/Create.cshtml` |
| Branches | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Branches/Index.cshtml` |
| CompanyOnboarding | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/CompanyOnboarding/Index.cshtml` |
| CustomerCreditNotes | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerCreditNotes/Create.cshtml` |
| CustomerCreditNotes | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerCreditNotes/Details.cshtml` |
| CustomerCreditNotes | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerCreditNotes/Index.cshtml` |
| CustomerInvoices | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerInvoices/Create.cshtml` |
| CustomerInvoices | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerInvoices/Details.cshtml` |
| CustomerInvoices | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerInvoices/Index.cshtml` |
| CustomerPayments | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerPayments/Create.cshtml` |
| CustomerPayments | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerPayments/Details.cshtml` |
| CustomerPayments | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerPayments/Index.cshtml` |
| CustomerReturnReceipts | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerReturnReceipts/Create.cshtml` |
| CustomerReturnReceipts | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerReturnReceipts/Details.cshtml` |
| CustomerReturnReceipts | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerReturnReceipts/Index.cshtml` |
| CustomerReturnRequests | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerReturnRequests/Create.cshtml` |
| CustomerReturnRequests | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerReturnRequests/Details.cshtml` |
| CustomerReturnRequests | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/CustomerReturnRequests/Index.cshtml` |
| Customers | Create.cshtml | Angular Pilot | /customers | `MiniStore.Web/Views/Customers/Create.cshtml` |
| Customers | Details.cshtml | Angular Pilot | /customers | `MiniStore.Web/Views/Customers/Details.cshtml` |
| Customers | Edit.cshtml | Angular Pilot | /customers | `MiniStore.Web/Views/Customers/Edit.cshtml` |
| Customers | Index.cshtml | Angular Pilot | /customers | `MiniStore.Web/Views/Customers/Index.cshtml` |
| Dashboard | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Dashboard/Index.cshtml` |
| DocumentDesigns | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/DocumentDesigns/Create.cshtml` |
| DocumentDesigns | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/DocumentDesigns/Details.cshtml` |
| DocumentDesigns | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/DocumentDesigns/Index.cshtml` |
| GoodsReceiptReturns | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/GoodsReceiptReturns/Create.cshtml` |
| GoodsReceiptReturns | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/GoodsReceiptReturns/Details.cshtml` |
| GoodsReceiptReturns | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/GoodsReceiptReturns/Index.cshtml` |
| GoodsReceipts | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/GoodsReceipts/Create.cshtml` |
| GoodsReceipts | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/GoodsReceipts/Details.cshtml` |
| GoodsReceipts | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/GoodsReceipts/Index.cshtml` |
| GoodsReceipts | Scan.cshtml | Inventoried | — | `MiniStore.Web/Views/GoodsReceipts/Scan.cshtml` |
| Home | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Home/Index.cshtml` |
| InventoryAdjustments | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/InventoryAdjustments/Create.cshtml` |
| InventoryAdjustments | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/InventoryAdjustments/Details.cshtml` |
| InventoryAdjustments | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/InventoryAdjustments/Index.cshtml` |
| InventoryBalances | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/InventoryBalances/Index.cshtml` |
| InventoryInsights | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/InventoryInsights/Index.cshtml` |
| InventoryReconciliation | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/InventoryReconciliation/Index.cshtml` |
| InventoryReservations | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/InventoryReservations/Index.cshtml` |
| InventoryScanning | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/InventoryScanning/Index.cshtml` |
| InventoryTracking | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/InventoryTracking/Index.cshtml` |
| JournalEntries | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/JournalEntries/Details.cshtml` |
| LandedCosts | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/LandedCosts/Create.cshtml` |
| LandedCosts | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/LandedCosts/Details.cshtml` |
| LandedCosts | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/LandedCosts/Index.cshtml` |
| LocationMovements | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/LocationMovements/Index.cshtml` |
| PaymentMethods | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/PaymentMethods/Create.cshtml` |
| PaymentMethods | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PaymentMethods/Index.cshtml` |
| ProductAttributes | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/ProductAttributes/Index.cshtml` |
| ProductAttributes | Values.cshtml | Inventoried | — | `MiniStore.Web/Views/ProductAttributes/Values.cshtml` |
| ProductCategories | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/ProductCategories/Index.cshtml` |
| Products | Create.cshtml | Angular Pilot | /products | `MiniStore.Web/Views/Products/Create.cshtml` |
| Products | Edit.cshtml | Angular Pilot | /products | `MiniStore.Web/Views/Products/Edit.cshtml` |
| Products | Index.cshtml | Angular Pilot | /products | `MiniStore.Web/Views/Products/Index.cshtml` |
| ProductStocks | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/ProductStocks/Create.cshtml` |
| ProductStocks | Edit.cshtml | Inventoried | — | `MiniStore.Web/Views/ProductStocks/Edit.cshtml` |
| ProductStocks | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/ProductStocks/Index.cshtml` |
| ProductTemplates | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/ProductTemplates/Index.cshtml` |
| Public | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Public/Index.cshtml` |
| Public | Pricing.cshtml | Inventoried | — | `MiniStore.Web/Views/Public/Pricing.cshtml` |
| PurchaseAccountingReconciliation | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseAccountingReconciliation/Index.cshtml` |
| PurchaseAgreements | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseAgreements/Create.cshtml` |
| PurchaseAgreements | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseAgreements/Details.cshtml` |
| PurchaseAgreements | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseAgreements/Index.cshtml` |
| PurchaseAnalytics | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseAnalytics/Index.cshtml` |
| PurchaseApprovalRules | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseApprovalRules/Index.cshtml` |
| PurchaseDocumentReversals | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseDocumentReversals/Create.cshtml` |
| PurchaseDocumentReversals | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseDocumentReversals/Details.cshtml` |
| PurchaseDocumentReversals | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseDocumentReversals/Index.cshtml` |
| PurchaseOrders | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseOrders/Details.cshtml` |
| PurchaseOrders | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseOrders/Index.cshtml` |
| PurchaseReplenishment | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseReplenishment/Index.cshtml` |
| PurchaseRequests | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseRequests/Create.cshtml` |
| PurchaseRequests | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseRequests/Details.cshtml` |
| PurchaseRequests | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseRequests/Index.cshtml` |
| PurchaseReturns | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseReturns/Create.cshtml` |
| PurchaseReturns | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseReturns/Details.cshtml` |
| PurchaseReturns | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseReturns/Index.cshtml` |
| Purchases | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/Purchases/Create.cshtml` |
| Purchases | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/Purchases/Details.cshtml` |
| Purchases | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Purchases/Index.cshtml` |
| PurchaseServiceAcceptances | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseServiceAcceptances/Create.cshtml` |
| PurchaseServiceAcceptances | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseServiceAcceptances/Details.cshtml` |
| PurchaseServiceAcceptances | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseServiceAcceptances/Index.cshtml` |
| PurchaseSourcing | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseSourcing/Create.cshtml` |
| PurchaseSourcing | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseSourcing/Details.cshtml` |
| PurchaseSourcing | Direct.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseSourcing/Direct.cshtml` |
| PurchaseSourcing | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseSourcing/Index.cshtml` |
| PurchaseSourcing | Print.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseSourcing/Print.cshtml` |
| PurchaseTemplates | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/PurchaseTemplates/Index.cshtml` |
| Recipes | Edit.cshtml | Inventoried | — | `MiniStore.Web/Views/Recipes/Edit.cshtml` |
| Recipes | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Recipes/Index.cshtml` |
| Replenishment | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Replenishment/Index.cshtml` |
| Roles | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/Roles/Create.cshtml` |
| Roles | Edit.cshtml | Inventoried | — | `MiniStore.Web/Views/Roles/Edit.cshtml` |
| Roles | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Roles/Index.cshtml` |
| Sales | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/Sales/Create.cshtml` |
| Sales | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/Sales/Details.cshtml` |
| Sales | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Sales/Index.cshtml` |
| Sales | Pos.cshtml | Inventoried | — | `MiniStore.Web/Views/Sales/Pos.cshtml` |
| SalesApprovalRules | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesApprovalRules/Index.cshtml` |
| SalesDeliveries | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesDeliveries/Create.cshtml` |
| SalesDeliveries | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesDeliveries/Details.cshtml` |
| SalesDeliveries | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesDeliveries/Index.cshtml` |
| SalesOrders | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesOrders/Details.cshtml` |
| SalesOrders | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesOrders/Index.cshtml` |
| SalesOrders | Reserve.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesOrders/Reserve.cshtml` |
| SalesPriceLists | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesPriceLists/Index.cshtml` |
| SalesQuotations | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesQuotations/Create.cshtml` |
| SalesQuotations | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesQuotations/Details.cshtml` |
| SalesQuotations | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesQuotations/Index.cshtml` |
| SalesQuotationTemplates | Create.cshtml | Angular Pilot | /sales/quotation-templates/new | `MiniStore.Web/Views/SalesQuotationTemplates/Create.cshtml` |
| SalesQuotationTemplates | Details.cshtml | Angular Pilot | /sales/quotation-templates/{id} | `MiniStore.Web/Views/SalesQuotationTemplates/Details.cshtml` |
| SalesQuotationTemplates | Index.cshtml | Angular Pilot | /sales/quotation-templates | `MiniStore.Web/Views/SalesQuotationTemplates/Index.cshtml` |
| SalesReturns | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesReturns/Create.cshtml` |
| SalesReturns | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesReturns/Details.cshtml` |
| SalesReturns | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/SalesReturns/Index.cshtml` |
| Settings | Accounting.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/Accounting.cshtml` |
| Settings | BranchSalesAccounts.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/BranchSalesAccounts.cshtml` |
| Settings | Currencies.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/Currencies.cshtml` |
| Settings | Discounts.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/Discounts.cshtml` |
| Settings | DocumentNumbers.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/DocumentNumbers.cshtml` |
| Settings | FiscalPeriods.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/FiscalPeriods.cshtml` |
| Settings | General.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/General.cshtml` |
| Settings | Index.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/Index.cshtml` |
| Settings | Inventory.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/Inventory.cshtml` |
| Settings | InventoryAccess.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/InventoryAccess.cshtml` |
| Settings | OrganizationIdentity.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/OrganizationIdentity.cshtml` |
| Settings | Pos.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/Pos.cshtml` |
| Settings | PurchaseMatching.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/PurchaseMatching.cshtml` |
| Settings | PurchaseSourcingPolicy.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/PurchaseSourcingPolicy.cshtml` |
| Settings | Units.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/Units.cshtml` |
| Settings | WorkflowCatalog.cshtml | Angular Pilot | /settings | `MiniStore.Web/Views/Settings/WorkflowCatalog.cshtml` |
| StockMovements | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/StockMovements/Index.cshtml` |
| StockTransactions | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/StockTransactions/Create.cshtml` |
| StockTransactions | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/StockTransactions/Index.cshtml` |
| StockTransfers | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/StockTransfers/Create.cshtml` |
| StockTransfers | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/StockTransfers/Details.cshtml` |
| StockTransfers | Edit.cshtml | Inventoried | — | `MiniStore.Web/Views/StockTransfers/Edit.cshtml` |
| StockTransfers | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/StockTransfers/Index.cshtml` |
| Subscription | Checkout.cshtml | Inventoried | — | `MiniStore.Web/Views/Subscription/Checkout.cshtml` |
| Subscription | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Subscription/Index.cshtml` |
| SupplierCommunications | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierCommunications/Details.cshtml` |
| SupplierCreditNotes | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierCreditNotes/Create.cshtml` |
| SupplierCreditNotes | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierCreditNotes/Details.cshtml` |
| SupplierCreditNotes | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierCreditNotes/Index.cshtml` |
| SupplierIntegrations | CredentialCreated.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierIntegrations/CredentialCreated.cshtml` |
| SupplierIntegrations | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierIntegrations/Index.cshtml` |
| SupplierIntegrations | PortalAccessCreated.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierIntegrations/PortalAccessCreated.cshtml` |
| SupplierIntegrations | Submission.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierIntegrations/Submission.cshtml` |
| SupplierPayments | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierPayments/Create.cshtml` |
| SupplierPayments | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierPayments/Details.cshtml` |
| SupplierPayments | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierPayments/Index.cshtml` |
| SupplierPortal | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierPortal/Index.cshtml` |
| SupplierPortal | Unavailable.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierPortal/Unavailable.cshtml` |
| SupplierPurchasing | ImportPreview.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierPurchasing/ImportPreview.cshtml` |
| SupplierPurchasing | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierPurchasing/Index.cshtml` |
| SupplierPurchasing | PriceBreaks.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierPurchasing/PriceBreaks.cshtml` |
| SupplierPurchasing | PriceExceptions.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierPurchasing/PriceExceptions.cshtml` |
| SupplierPurchasing | PriceHistory.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierPurchasing/PriceHistory.cshtml` |
| SupplierQuotations | Comparison.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierQuotations/Comparison.cshtml` |
| SupplierQuotations | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/SupplierQuotations/Create.cshtml` |
| Suppliers | Create.cshtml | Angular Pilot | /suppliers | `MiniStore.Web/Views/Suppliers/Create.cshtml` |
| Suppliers | Edit.cshtml | Angular Pilot | /suppliers | `MiniStore.Web/Views/Suppliers/Edit.cshtml` |
| Suppliers | Index.cshtml | Angular Pilot | /suppliers | `MiniStore.Web/Views/Suppliers/Index.cshtml` |
| TaxRates | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/TaxRates/Create.cshtml` |
| TaxRates | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/TaxRates/Index.cshtml` |
| UnassignedStock | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/UnassignedStock/Index.cshtml` |
| Users | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/Users/Create.cshtml` |
| Users | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Users/Index.cshtml` |
| VendorBills | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/VendorBills/Create.cshtml` |
| VendorBills | Details.cshtml | Inventoried | — | `MiniStore.Web/Views/VendorBills/Details.cshtml` |
| VendorBills | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/VendorBills/Index.cshtml` |
| WarehouseLocations | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/WarehouseLocations/Create.cshtml` |
| WarehouseLocations | Edit.cshtml | Inventoried | — | `MiniStore.Web/Views/WarehouseLocations/Edit.cshtml` |
| WarehouseLocations | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/WarehouseLocations/Index.cshtml` |
| Warehouses | Create.cshtml | Inventoried | — | `MiniStore.Web/Views/Warehouses/Create.cshtml` |
| Warehouses | Edit.cshtml | Inventoried | — | `MiniStore.Web/Views/Warehouses/Edit.cshtml` |
| Warehouses | Index.cshtml | Inventoried | — | `MiniStore.Web/Views/Warehouses/Index.cshtml` |

## Action inventory

| MVC surface | Action | HTTP verbs | Migration status | Allowlisted Angular route | Source |
|---|---|---|---|---|---|
| Account | AccessDenied | GET | Inventoried | — | `MiniStore.Web/Controllers/Security/AccountController.cs` |
| Account | Login | GET | Inventoried | — | `MiniStore.Web/Controllers/Security/AccountController.cs` |
| Account | Login | POST | Inventoried | — | `MiniStore.Web/Controllers/Security/AccountController.cs` |
| Account | Logout | POST | Inventoried | — | `MiniStore.Web/Controllers/Security/AccountController.cs` |
| Account | Register | GET | Inventoried | — | `MiniStore.Web/Controllers/Security/AccountController.cs` |
| Account | Register | POST | Inventoried | — | `MiniStore.Web/Controllers/Security/AccountController.cs` |
| Accounts | Index | ANY | Inventoried | — | `MiniStore.Web/Controllers/Accounting/AccountsController.cs` |
| AnalyticsCenter | Create | GET | Angular Pilot | /analytics/builder | `MiniStore.Web/Controllers/Analytics/AnalyticsCenterController.cs` |
| AnalyticsCenter | Create | POST | Angular Pilot | /analytics/builder | `MiniStore.Web/Controllers/Analytics/AnalyticsCenterController.cs` |
| AnalyticsCenter | Index | GET | Angular Pilot | /analytics | `MiniStore.Web/Controllers/Analytics/AnalyticsCenterController.cs` |
| AuditEvents | Export | POST | Inventoried | — | `MiniStore.Web/Controllers/Security/AuditEventsController.cs` |
| AuditEvents | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Security/AuditEventsController.cs` |
| CompanyOnboarding | Complete | POST | Inventoried | — | `MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs` |
| CompanyOnboarding | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs` |
| CompanyOnboarding | Skip | POST | Inventoried | — | `MiniStore.Web/Controllers/Saas/CompanyOnboardingController.cs` |
| CustomerInvoices | Approve | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs` |
| CustomerInvoices | Cancel | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs` |
| CustomerInvoices | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs` |
| CustomerInvoices | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs` |
| CustomerInvoices | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs` |
| CustomerInvoices | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs` |
| CustomerInvoices | Post | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs` |
| CustomerInvoices | Reverse | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerInvoicesController.cs` |
| CustomerPayments | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerPaymentsController.cs` |
| CustomerPayments | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerPaymentsController.cs` |
| CustomerPayments | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerPaymentsController.cs` |
| CustomerPayments | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerPaymentsController.cs` |
| CustomerPayments | Reverse | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerPaymentsController.cs` |
| CustomerReturnRequests | Approve | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs` |
| CustomerReturnRequests | Cancel | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs` |
| CustomerReturnRequests | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs` |
| CustomerReturnRequests | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs` |
| CustomerReturnRequests | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs` |
| CustomerReturnRequests | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs` |
| CustomerReturnRequests | Reject | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs` |
| CustomerReturnRequests | Submit | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/CustomerReturnRequestsController.cs` |
| Customers | Archive | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | ArchiveAddress | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | ArchiveContact | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | Create | GET | Angular Pilot | /customers/new | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | Create | POST | Angular Pilot | /customers/new | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | Details | GET | Angular Pilot | /customers/{id} | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | Edit | GET | Angular Pilot | /customers/{id}/edit | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | Edit | POST | Angular Pilot | /customers/{id}/edit | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | Index | GET | Angular Pilot | /customers | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | Restore | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | RestoreAddress | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | RestoreContact | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | SaveAddress | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Customers | SaveContact | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Customers/CustomersController.cs` |
| Dashboard | Index | ANY | Inventoried | — | `MiniStore.Web/Controllers/Home/DashboardController.cs` |
| DocumentDesigns | Activate | POST | Inventoried | — | `MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs` |
| DocumentDesigns | Archive | POST | Inventoried | — | `MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs` |
| DocumentDesigns | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs` |
| DocumentDesigns | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs` |
| DocumentDesigns | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs` |
| DocumentDesigns | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs` |
| DocumentDesigns | SetDefault | POST | Inventoried | — | `MiniStore.Web/Controllers/Settings/DocumentDesignsController.cs` |
| GoodsReceiptReturns | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptReturnsController.cs` |
| GoodsReceiptReturns | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptReturnsController.cs` |
| GoodsReceiptReturns | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptReturnsController.cs` |
| GoodsReceiptReturns | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptReturnsController.cs` |
| GoodsReceipts | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs` |
| GoodsReceipts | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs` |
| GoodsReceipts | CreateScanDraft | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs` |
| GoodsReceipts | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs` |
| GoodsReceipts | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs` |
| GoodsReceipts | Inspect | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs` |
| GoodsReceipts | PostDraft | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs` |
| GoodsReceipts | PutAway | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs` |
| GoodsReceipts | RecordScan | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs` |
| GoodsReceipts | Scan | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/GoodsReceiptsController.cs` |
| Home | Index | ANY | Inventoried | — | `MiniStore.Web/Controllers/Home/HomeController.cs` |
| InventoryAdjustments | Approve | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs` |
| InventoryAdjustments | Cancel | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs` |
| InventoryAdjustments | Count | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs` |
| InventoryAdjustments | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs` |
| InventoryAdjustments | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs` |
| InventoryAdjustments | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs` |
| InventoryAdjustments | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs` |
| InventoryAdjustments | Post | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryAdjustmentsController.cs` |
| InventoryBalances | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryBalancesController.cs` |
| InventoryInsights | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryInsightsController.cs` |
| InventoryReconciliation | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryReconciliationController.cs` |
| InventoryReservations | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryReservationsController.cs` |
| InventoryScanning | Count | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryScanningController.cs` |
| InventoryScanning | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryScanningController.cs` |
| InventoryScanning | Putaway | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryScanningController.cs` |
| InventoryScanning | Relocate | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryScanningController.cs` |
| InventoryTracking | CloseRecall | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryTrackingController.cs` |
| InventoryTracking | CreateRecall | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryTrackingController.cs` |
| InventoryTracking | Open | GET, POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryTrackingController.cs` |
| InventoryTracking | Quarantine | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryTrackingController.cs` |
| InventoryTracking | RecordRecallCommunication | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryTrackingController.cs` |
| InventoryTracking | Release | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/InventoryTrackingController.cs` |
| JournalEntries | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Accounting/JournalEntriesController.cs` |
| Language | Set | POST | Inventoried | — | `MiniStore.Web/Controllers/Settings/LanguageController.cs` |
| LocationMovements | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/LocationMovementsController.cs` |
| LocationMovements | Move | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/LocationMovementsController.cs` |
| PaymentMethods | Index | ANY | Inventoried | — | `MiniStore.Web/Controllers/Accounting/PaymentMethodsController.cs` |
| ProductAttributes | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductAttributesController.cs` |
| ProductAttributes | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductAttributesController.cs` |
| ProductAttributes | SetActive | POST | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductAttributesController.cs` |
| ProductAttributes | Values | GET | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductAttributesController.cs` |
| ProductAttributes | Values | POST | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductAttributesController.cs` |
| ProductCategories | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductCategoriesController.cs` |
| ProductCategories | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductCategoriesController.cs` |
| ProductCategories | SetActive | POST | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductCategoriesController.cs` |
| Products | Create | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Catalog/ProductsController.cs` |
| Products | Create | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Catalog/ProductsController.cs` |
| Products | Delete | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Catalog/ProductsController.cs` |
| Products | Edit | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Catalog/ProductsController.cs` |
| Products | Edit | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Catalog/ProductsController.cs` |
| Products | Index | ANY | Angular Pilot | /products | `MiniStore.Web/Controllers/Catalog/ProductsController.cs` |
| ProductStocks | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/ProductStocksController.cs` |
| ProductStocks | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/ProductStocksController.cs` |
| ProductStocks | Delete | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/ProductStocksController.cs` |
| ProductStocks | Edit | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/ProductStocksController.cs` |
| ProductStocks | Edit | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/ProductStocksController.cs` |
| ProductStocks | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/ProductStocksController.cs` |
| ProductTemplates | Assign | POST | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductTemplatesController.cs` |
| ProductTemplates | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductTemplatesController.cs` |
| ProductTemplates | Generate | POST | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductTemplatesController.cs` |
| ProductTemplates | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductTemplatesController.cs` |
| ProductTemplates | Preview | POST | Inventoried | — | `MiniStore.Web/Controllers/Catalog/ProductTemplatesController.cs` |
| Public | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/PublicController.cs` |
| Public | Pricing | GET | Inventoried | — | `MiniStore.Web/Controllers/PublicController.cs` |
| PurchaseAccountingReconciliation | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseAccountingReconciliationController.cs` |
| PurchaseApprovalRules | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseApprovalRulesController.cs` |
| PurchaseApprovalRules | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseApprovalRulesController.cs` |
| PurchaseApprovalRules | SetActive | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseApprovalRulesController.cs` |
| PurchaseDocumentReversals | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseDocumentReversalsController.cs` |
| PurchaseDocumentReversals | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseDocumentReversalsController.cs` |
| PurchaseDocumentReversals | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseDocumentReversalsController.cs` |
| PurchaseDocumentReversals | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseDocumentReversalsController.cs` |
| PurchaseReplenishment | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseReplenishmentController.cs` |
| PurchaseReplenishment | RunNow | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseReplenishmentController.cs` |
| PurchaseReplenishment | SaveSettings | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseReplenishmentController.cs` |
| PurchaseReturns | Create | ANY | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseReturnsController.cs` |
| PurchaseReturns | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseReturnsController.cs` |
| PurchaseReturns | Details | ANY | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseReturnsController.cs` |
| PurchaseReturns | Index | ANY | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseReturnsController.cs` |
| Purchases | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchasesController.cs` |
| Purchases | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchasesController.cs` |
| Purchases | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchasesController.cs` |
| Purchases | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchasesController.cs` |
| Purchases | Post | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchasesController.cs` |
| PurchaseServiceAcceptances | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseServiceAcceptancesController.cs` |
| PurchaseServiceAcceptances | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseServiceAcceptancesController.cs` |
| PurchaseServiceAcceptances | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseServiceAcceptancesController.cs` |
| PurchaseServiceAcceptances | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseServiceAcceptancesController.cs` |
| PurchaseSourcing | Attachment | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs` |
| PurchaseSourcing | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs` |
| PurchaseSourcing | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs` |
| PurchaseSourcing | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs` |
| PurchaseSourcing | Direct | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs` |
| PurchaseSourcing | Direct | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs` |
| PurchaseSourcing | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs` |
| PurchaseSourcing | Print | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/PurchaseSourcingController.cs` |
| Recipes | Edit | GET | Inventoried | — | `MiniStore.Web/Controllers/Catalog/RecipesController.cs` |
| Recipes | Edit | POST | Inventoried | — | `MiniStore.Web/Controllers/Catalog/RecipesController.cs` |
| Recipes | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Catalog/RecipesController.cs` |
| Replenishment | CreatePurchaseRequestDraft | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/ReplenishmentController.cs` |
| Replenishment | CreateTransferDraft | GET, POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/ReplenishmentController.cs` |
| Roles | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Security/RolesController.cs` |
| Roles | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Security/RolesController.cs` |
| Roles | Delete | POST | Inventoried | — | `MiniStore.Web/Controllers/Security/RolesController.cs` |
| Roles | Edit | GET | Inventoried | — | `MiniStore.Web/Controllers/Security/RolesController.cs` |
| Roles | Edit | POST | Inventoried | — | `MiniStore.Web/Controllers/Security/RolesController.cs` |
| Roles | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Security/RolesController.cs` |
| Sales | Create | ANY | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesController.cs` |
| Sales | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesController.cs` |
| Sales | Details | ANY | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesController.cs` |
| Sales | Index | ANY | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesController.cs` |
| Sales | Pos | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesController.cs` |
| Sales | Post | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesController.cs` |
| SalesApprovalRules | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesApprovalRulesController.cs` |
| SalesApprovalRules | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesApprovalRulesController.cs` |
| SalesApprovalRules | SetActive | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesApprovalRulesController.cs` |
| SalesDeliveries | Cancel | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesDeliveriesController.cs` |
| SalesDeliveries | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesDeliveriesController.cs` |
| SalesDeliveries | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesDeliveriesController.cs` |
| SalesDeliveries | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesDeliveriesController.cs` |
| SalesDeliveries | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesDeliveriesController.cs` |
| SalesDeliveries | Post | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesDeliveriesController.cs` |
| SalesOrders | ApproveException | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesOrdersController.cs` |
| SalesOrders | Cancel | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesOrdersController.cs` |
| SalesOrders | Confirm | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesOrdersController.cs` |
| SalesOrders | CreateFromQuotation | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesOrdersController.cs` |
| SalesOrders | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesOrdersController.cs` |
| SalesOrders | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesOrdersController.cs` |
| SalesOrders | RejectException | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesOrdersController.cs` |
| SalesOrders | ReleaseReservation | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesOrdersController.cs` |
| SalesOrders | Reserve | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesOrdersController.cs` |
| SalesOrders | Reserve | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesOrdersController.cs` |
| SalesPriceLists | AddRule | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs` |
| SalesPriceLists | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs` |
| SalesPriceLists | Evaluate | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs` |
| SalesPriceLists | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs` |
| SalesPriceLists | SetListActive | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs` |
| SalesPriceLists | SetRuleActive | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesPriceListsController.cs` |
| SalesQuotations | Accept | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs` |
| SalesQuotations | Cancel | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs` |
| SalesQuotations | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs` |
| SalesQuotations | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs` |
| SalesQuotations | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs` |
| SalesQuotations | DownloadPdf | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs` |
| SalesQuotations | Expire | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs` |
| SalesQuotations | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs` |
| SalesQuotations | Reject | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs` |
| SalesQuotations | Send | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesQuotationsController.cs` |
| SalesQuotationTemplates | Activate | POST | Angular Pilot | /sales/quotation-templates/{id} | `MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs` |
| SalesQuotationTemplates | Archive | POST | Angular Pilot | /sales/quotation-templates/{id} | `MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs` |
| SalesQuotationTemplates | Create | GET | Angular Pilot | /sales/quotation-templates/new | `MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs` |
| SalesQuotationTemplates | Create | POST | Angular Pilot | /sales/quotation-templates/new | `MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs` |
| SalesQuotationTemplates | Details | GET | Angular Pilot | /sales/quotation-templates/{id} | `MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs` |
| SalesQuotationTemplates | Index | GET | Angular Pilot | /sales/quotation-templates | `MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs` |
| SalesQuotationTemplates | SetDefault | POST | Angular Pilot | /sales/quotation-templates/{id} | `MiniStore.Web/Controllers/Sales/SalesQuotationTemplatesController.cs` |
| SalesReturns | Create | ANY | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesReturnsController.cs` |
| SalesReturns | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesReturnsController.cs` |
| SalesReturns | Details | ANY | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesReturnsController.cs` |
| SalesReturns | Index | ANY | Inventoried | — | `MiniStore.Web/Controllers/Sales/SalesReturnsController.cs` |
| Settings | Accounting | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Accounting | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | BranchSalesAccounts | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | BranchSalesAccounts | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | ChangeFiscalPeriodStatus | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | ConfigureBranchWarehouse | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | ConfigurePosTerminalWarehouse | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | CreatePosTerminal | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Currencies | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Currencies | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | DeactivateUnit | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Discounts | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Discounts | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | DocumentNumbers | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | DocumentNumbers | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | FiscalPeriods | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | FiscalPeriods | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | General | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | General | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Index | GET | Angular Pilot | /settings | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Inventory | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Inventory | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | InventoryAccess | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Invoices | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | OrganizationIdentity | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | OrganizationIdentity | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | OrganizationLogo | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Pos | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Pos | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | PurchaseMatching | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | PurchaseMatching | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | PurchaseSourcingPolicy | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | PurchaseSourcingPolicy | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Units | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | Units | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| Settings | WorkflowCatalog | GET | Angular Pilot | — | `MiniStore.Web/Controllers/Settings/SettingsController.cs` |
| StockMovements | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockMovementsController.cs` |
| StockTransactions | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransactionsController.cs` |
| StockTransactions | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransactionsController.cs` |
| StockTransactions | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransactionsController.cs` |
| StockTransfers | Approve | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransfersController.cs` |
| StockTransfers | Cancel | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransfersController.cs` |
| StockTransfers | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransfersController.cs` |
| StockTransfers | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransfersController.cs` |
| StockTransfers | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransfersController.cs` |
| StockTransfers | Edit | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransfersController.cs` |
| StockTransfers | Edit | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransfersController.cs` |
| StockTransfers | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransfersController.cs` |
| StockTransfers | Post | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransfersController.cs` |
| StockTransfers | Reject | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransfersController.cs` |
| StockTransfers | ReturnToDraft | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransfersController.cs` |
| StockTransfers | Submit | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/StockTransfersController.cs` |
| Subscription | Checkout | GET, POST | Inventoried | — | `MiniStore.Web/Controllers/Saas/SubscriptionController.cs` |
| SupplierCreditNotes | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierCreditNotesController.cs` |
| SupplierCreditNotes | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierCreditNotesController.cs` |
| SupplierCreditNotes | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierCreditNotesController.cs` |
| SupplierCreditNotes | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierCreditNotesController.cs` |
| SupplierIntegrations | File | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs` |
| SupplierIntegrations | RedirectToAction | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs` |
| SupplierIntegrations | RedirectToAction | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs` |
| SupplierIntegrations | RedirectToAction | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs` |
| SupplierIntegrations | RedirectToDocument | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs` |
| SupplierIntegrations | View | GET, POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs` |
| SupplierIntegrations | View | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierIntegrationsController.cs` |
| SupplierPayments | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPaymentsController.cs` |
| SupplierPayments | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPaymentsController.cs` |
| SupplierPayments | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPaymentsController.cs` |
| SupplierPayments | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPaymentsController.cs` |
| SupplierPortal | RecordDenied | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPortalController.cs` |
| SupplierPortal | RecordDenied | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPortalController.cs` |
| SupplierPortal | RecordDenied | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPortalController.cs` |
| SupplierPortal | RecordDenied | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPortalController.cs` |
| SupplierPurchasing | ExportCsv | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | ExportExcel | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | ImportCsv | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | ImportExcel | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | PreviewCsv | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | PreviewExcel | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | PriceBreaks | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | PriceExceptions | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | PriceHistory | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | Save | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | SaveBulk | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | SavePriceBreaks | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| SupplierPurchasing | SetActive | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/SupplierPurchasingController.cs` |
| Suppliers | Create | GET | Angular Pilot | /suppliers/new | `MiniStore.Web/Controllers/Suppliers/SuppliersController.cs` |
| Suppliers | Create | POST | Angular Pilot | /suppliers/new | `MiniStore.Web/Controllers/Suppliers/SuppliersController.cs` |
| Suppliers | Delete | POST | Angular Pilot | — | `MiniStore.Web/Controllers/Suppliers/SuppliersController.cs` |
| Suppliers | Edit | GET | Angular Pilot | /suppliers/{id}/edit | `MiniStore.Web/Controllers/Suppliers/SuppliersController.cs` |
| Suppliers | Edit | POST | Angular Pilot | /suppliers/{id}/edit | `MiniStore.Web/Controllers/Suppliers/SuppliersController.cs` |
| Suppliers | Index | GET | Angular Pilot | /suppliers | `MiniStore.Web/Controllers/Suppliers/SuppliersController.cs` |
| TaxRates | Index | ANY | Inventoried | — | `MiniStore.Web/Controllers/Accounting/TaxRatesController.cs` |
| UnassignedStock | Assign | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/UnassignedStockController.cs` |
| UnassignedStock | CreateRule | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/UnassignedStockController.cs` |
| UnassignedStock | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/UnassignedStockController.cs` |
| UnassignedStock | SetRuleActive | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/UnassignedStockController.cs` |
| Users | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Security/UsersController.cs` |
| Users | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Security/UsersController.cs` |
| Users | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Security/UsersController.cs` |
| VendorBills | Approve | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/VendorBillsController.cs` |
| VendorBills | ApproveOverride | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/VendorBillsController.cs` |
| VendorBills | Cancel | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/VendorBillsController.cs` |
| VendorBills | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/VendorBillsController.cs` |
| VendorBills | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/VendorBillsController.cs` |
| VendorBills | Details | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/VendorBillsController.cs` |
| VendorBills | DownloadAttachment | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/VendorBillsController.cs` |
| VendorBills | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Purchases/VendorBillsController.cs` |
| VendorBills | Post | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/VendorBillsController.cs` |
| VendorBills | RedirectToAction | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/VendorBillsController.cs` |
| VendorBills | RedirectToAction | POST | Inventoried | — | `MiniStore.Web/Controllers/Purchases/VendorBillsController.cs` |
| WarehouseLocations | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/WarehouseLocationsController.cs` |
| WarehouseLocations | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/WarehouseLocationsController.cs` |
| WarehouseLocations | Edit | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/WarehouseLocationsController.cs` |
| WarehouseLocations | Edit | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/WarehouseLocationsController.cs` |
| WarehouseLocations | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/WarehouseLocationsController.cs` |
| Warehouses | Create | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/WarehousesController.cs` |
| Warehouses | Create | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/WarehousesController.cs` |
| Warehouses | Delete | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/WarehousesController.cs` |
| Warehouses | Edit | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/WarehousesController.cs` |
| Warehouses | Edit | POST | Inventoried | — | `MiniStore.Web/Controllers/Inventory/WarehousesController.cs` |
| Warehouses | Index | GET | Inventoried | — | `MiniStore.Web/Controllers/Inventory/WarehousesController.cs` |

## Control inventory

| MVC surface | Razor file | Forms | Buttons | Links | Referenced MVC actions | Source |
|---|---|---:|---:|---:|---|---|
| Account | AccessDenied.cshtml | 0 | 0 | 1 | Index | `MiniStore.Web/Views/Account/AccessDenied.cshtml` |
| Account | Login.cshtml | 1 | 1 | 0 | Login | `MiniStore.Web/Views/Account/Login.cshtml` |
| Account | Register.cshtml | 1 | 4 | 0 | — | `MiniStore.Web/Views/Account/Register.cshtml` |
| Accounts | Create.cshtml | 1 | 1 | 0 | — | `MiniStore.Web/Views/Accounts/Create.cshtml` |
| Accounts | Index.cshtml | 0 | 0 | 1 | Create | `MiniStore.Web/Views/Accounts/Index.cshtml` |
| AnalyticsCenter | Create.cshtml | 1 | 1 | 1 | Index | `MiniStore.Web/Views/AnalyticsCenter/Create.cshtml` |
| AnalyticsCenter | Index.cshtml | 1 | 1 | 2 | Create, Index | `MiniStore.Web/Views/AnalyticsCenter/Index.cshtml` |
| AuditEvents | Index.cshtml | 2 | 2 | 3 | Export, Index | `MiniStore.Web/Views/AuditEvents/Index.cshtml` |
| Branches | Create.cshtml | 1 | 1 | 0 | — | `MiniStore.Web/Views/Branches/Create.cshtml` |
| Branches | Index.cshtml | 0 | 0 | 1 | Create | `MiniStore.Web/Views/Branches/Index.cshtml` |
| CompanyOnboarding | Index.cshtml | 1 | 2 | 0 | — | `MiniStore.Web/Views/CompanyOnboarding/Index.cshtml` |
| CustomerCreditNotes | Create.cshtml | 1 | 1 | 1 | Create, Details | `MiniStore.Web/Views/CustomerCreditNotes/Create.cshtml` |
| CustomerCreditNotes | Details.cshtml | 0 | 0 | 3 | Details, Index | `MiniStore.Web/Views/CustomerCreditNotes/Details.cshtml` |
| CustomerCreditNotes | Index.cshtml | 0 | 0 | 1 | Details | `MiniStore.Web/Views/CustomerCreditNotes/Index.cshtml` |
| CustomerInvoices | Create.cshtml | 1 | 1 | 1 | Create, Details | `MiniStore.Web/Views/CustomerInvoices/Create.cshtml` |
| CustomerInvoices | Details.cshtml | 4 | 4 | 7 | Approve, Cancel, Create, Details, Index, Post, Reverse | `MiniStore.Web/Views/CustomerInvoices/Details.cshtml` |
| CustomerInvoices | Index.cshtml | 1 | 1 | 1 | Details | `MiniStore.Web/Views/CustomerInvoices/Index.cshtml` |
| CustomerPayments | Create.cshtml | 1 | 1 | 2 | Create, Details | `MiniStore.Web/Views/CustomerPayments/Create.cshtml` |
| CustomerPayments | Details.cshtml | 1 | 1 | 4 | Details, Index, Reverse | `MiniStore.Web/Views/CustomerPayments/Details.cshtml` |
| CustomerPayments | Index.cshtml | 1 | 1 | 1 | Details | `MiniStore.Web/Views/CustomerPayments/Index.cshtml` |
| CustomerReturnReceipts | Create.cshtml | 1 | 1 | 1 | Create, Details | `MiniStore.Web/Views/CustomerReturnReceipts/Create.cshtml` |
| CustomerReturnReceipts | Details.cshtml | 3 | 3 | 3 | Cancel, Details, Index, Inspect, Post | `MiniStore.Web/Views/CustomerReturnReceipts/Details.cshtml` |
| CustomerReturnReceipts | Index.cshtml | 1 | 1 | 2 | Details, Index | `MiniStore.Web/Views/CustomerReturnReceipts/Index.cshtml` |
| CustomerReturnRequests | Create.cshtml | 1 | 1 | 1 | Create, Details | `MiniStore.Web/Views/CustomerReturnRequests/Create.cshtml` |
| CustomerReturnRequests | Details.cshtml | 4 | 4 | 4 | Approve, Cancel, Create, Details, Index, Reject, Submit | `MiniStore.Web/Views/CustomerReturnRequests/Details.cshtml` |
| CustomerReturnRequests | Index.cshtml | 1 | 1 | 2 | Details, Index | `MiniStore.Web/Views/CustomerReturnRequests/Index.cshtml` |
| Customers | _Form.cshtml | 0 | 0 | 0 | — | `MiniStore.Web/Views/Customers/_Form.cshtml` |
| Customers | Create.cshtml | 1 | 1 | 2 | Create, Index | `MiniStore.Web/Views/Customers/Create.cshtml` |
| Customers | Details.cshtml | 8 | 8 | 2 | @(a.IsActive?, @(c.IsActive?, Archive, Edit, Index, Restore, SaveAddress, SaveContact | `MiniStore.Web/Views/Customers/Details.cshtml` |
| Customers | Edit.cshtml | 1 | 1 | 2 | Details, Edit | `MiniStore.Web/Views/Customers/Edit.cshtml` |
| Customers | Index.cshtml | 1 | 1 | 4 | Create, Details, Edit | `MiniStore.Web/Views/Customers/Index.cshtml` |
| Dashboard | Index.cshtml | 0 | 0 | 2 | @item.Action, @primary.Action | `MiniStore.Web/Views/Dashboard/Index.cshtml` |
| DocumentDesigns | _Section.cshtml | 0 | 1 | 0 | — | `MiniStore.Web/Views/DocumentDesigns/_Section.cshtml` |
| DocumentDesigns | Create.cshtml | 1 | 2 | 1 | Create, Index | `MiniStore.Web/Views/DocumentDesigns/Create.cshtml` |
| DocumentDesigns | Details.cshtml | 3 | 3 | 2 | Activate, Archive, Create, Index, SetDefault | `MiniStore.Web/Views/DocumentDesigns/Details.cshtml` |
| DocumentDesigns | Index.cshtml | 1 | 1 | 4 | Create, Details, Index | `MiniStore.Web/Views/DocumentDesigns/Index.cshtml` |
| GoodsReceiptReturns | Create.cshtml | 1 | 1 | 1 | Create, Details | `MiniStore.Web/Views/GoodsReceiptReturns/Create.cshtml` |
| GoodsReceiptReturns | Details.cshtml | 0 | 0 | 1 | Index | `MiniStore.Web/Views/GoodsReceiptReturns/Details.cshtml` |
| GoodsReceiptReturns | Index.cshtml | 0 | 0 | 1 | Details | `MiniStore.Web/Views/GoodsReceiptReturns/Index.cshtml` |
| GoodsReceipts | Create.cshtml | 2 | 2 | 1 | Create, CreateScanDraft, Details | `MiniStore.Web/Views/GoodsReceipts/Create.cshtml` |
| GoodsReceipts | Details.cshtml | 2 | 2 | 6 | Create, Details, Index, Inspect, PutAway, Scan | `MiniStore.Web/Views/GoodsReceipts/Details.cshtml` |
| GoodsReceipts | Index.cshtml | 1 | 1 | 1 | Details | `MiniStore.Web/Views/GoodsReceipts/Index.cshtml` |
| GoodsReceipts | Scan.cshtml | 2 | 2 | 1 | Details, PostDraft, RecordScan | `MiniStore.Web/Views/GoodsReceipts/Scan.cshtml` |
| Home | Index.cshtml | 0 | 0 | 0 | — | `MiniStore.Web/Views/Home/Index.cshtml` |
| InventoryAdjustments | Create.cshtml | 1 | 1 | 1 | Create, Index | `MiniStore.Web/Views/InventoryAdjustments/Create.cshtml` |
| InventoryAdjustments | Details.cshtml | 4 | 4 | 1 | Approve, Cancel, Count, Index, Post | `MiniStore.Web/Views/InventoryAdjustments/Details.cshtml` |
| InventoryAdjustments | Index.cshtml | 0 | 0 | 2 | Create, Details | `MiniStore.Web/Views/InventoryAdjustments/Index.cshtml` |
| InventoryBalances | Index.cshtml | 1 | 1 | 0 | — | `MiniStore.Web/Views/InventoryBalances/Index.cshtml` |
| InventoryInsights | Index.cshtml | 1 | 1 | 0 | — | `MiniStore.Web/Views/InventoryInsights/Index.cshtml` |
| InventoryReconciliation | Index.cshtml | 1 | 1 | 1 | — | `MiniStore.Web/Views/InventoryReconciliation/Index.cshtml` |
| InventoryReservations | Index.cshtml | 1 | 1 | 0 | — | `MiniStore.Web/Views/InventoryReservations/Index.cshtml` |
| InventoryScanning | Index.cshtml | 4 | 4 | 0 | Count, Putaway, Relocate | `MiniStore.Web/Views/InventoryScanning/Index.cshtml` |
| InventoryTracking | Index.cshtml | 5 | 7 | 0 | @(x.Status==InventoryTrackingStatus.Available?, CloseRecall, CreateRecall, Open, RecordRecallCommunication | `MiniStore.Web/Views/InventoryTracking/Index.cshtml` |
| JournalEntries | Details.cshtml | 0 | 1 | 0 | — | `MiniStore.Web/Views/JournalEntries/Details.cshtml` |
| LandedCosts | Create.cshtml | 1 | 1 | 1 | Create, Index | `MiniStore.Web/Views/LandedCosts/Create.cshtml` |
| LandedCosts | Details.cshtml | 0 | 0 | 3 | Create, Details, Index | `MiniStore.Web/Views/LandedCosts/Details.cshtml` |
| LandedCosts | Index.cshtml | 0 | 0 | 1 | Details | `MiniStore.Web/Views/LandedCosts/Index.cshtml` |
| LocationMovements | Index.cshtml | 2 | 2 | 1 | Index, Move | `MiniStore.Web/Views/LocationMovements/Index.cshtml` |
| PaymentMethods | Create.cshtml | 1 | 1 | 0 | — | `MiniStore.Web/Views/PaymentMethods/Create.cshtml` |
| PaymentMethods | Index.cshtml | 0 | 0 | 1 | Create | `MiniStore.Web/Views/PaymentMethods/Index.cshtml` |
| ProductAttributes | Index.cshtml | 2 | 2 | 1 | Create, Index, SetActive | `MiniStore.Web/Views/ProductAttributes/Index.cshtml` |
| ProductAttributes | Values.cshtml | 1 | 1 | 1 | Edit, Values | `MiniStore.Web/Views/ProductAttributes/Values.cshtml` |
| ProductCategories | Index.cshtml | 2 | 2 | 1 | Create, Index, SetActive | `MiniStore.Web/Views/ProductCategories/Index.cshtml` |
| Products | _LogisticsFields.cshtml | 0 | 0 | 0 | — | `MiniStore.Web/Views/Products/_LogisticsFields.cshtml` |
| Products | Create.cshtml | 1 | 1 | 1 | Create, Index | `MiniStore.Web/Views/Products/Create.cshtml` |
| Products | Edit.cshtml | 1 | 1 | 1 | Edit, Index | `MiniStore.Web/Views/Products/Edit.cshtml` |
| Products | Index.cshtml | 2 | 2 | 8 | Create, Delete, Edit, Index, Values | `MiniStore.Web/Views/Products/Index.cshtml` |
| ProductStocks | Create.cshtml | 1 | 1 | 1 | Create, Index | `MiniStore.Web/Views/ProductStocks/Create.cshtml` |
| ProductStocks | Edit.cshtml | 1 | 1 | 1 | Edit, Index | `MiniStore.Web/Views/ProductStocks/Edit.cshtml` |
| ProductStocks | Index.cshtml | 1 | 1 | 2 | Create, Delete, Edit | `MiniStore.Web/Views/ProductStocks/Index.cshtml` |
| ProductTemplates | Index.cshtml | 4 | 4 | 4 | Assign, Create, Generate, Index, Preview, Values | `MiniStore.Web/Views/ProductTemplates/Index.cshtml` |
| Public | Index.cshtml | 0 | 0 | 2 | Register | `MiniStore.Web/Views/Public/Index.cshtml` |
| Public | Pricing.cshtml | 0 | 0 | 1 | Register | `MiniStore.Web/Views/Public/Pricing.cshtml` |
| PurchaseAccountingReconciliation | Index.cshtml | 1 | 1 | 6 | Accounting, Details, Edit, Index | `MiniStore.Web/Views/PurchaseAccountingReconciliation/Index.cshtml` |
| PurchaseAgreements | Create.cshtml | 1 | 1 | 0 | Create | `MiniStore.Web/Views/PurchaseAgreements/Create.cshtml` |
| PurchaseAgreements | Details.cshtml | 3 | 3 | 1 | Activate, Cancel, Index, Release | `MiniStore.Web/Views/PurchaseAgreements/Details.cshtml` |
| PurchaseAgreements | Index.cshtml | 0 | 0 | 1 | Details | `MiniStore.Web/Views/PurchaseAgreements/Index.cshtml` |
| PurchaseAnalytics | Index.cshtml | 1 | 1 | 3 | @x.Action, Details, Index | `MiniStore.Web/Views/PurchaseAnalytics/Index.cshtml` |
| PurchaseApprovalRules | Index.cshtml | 2 | 2 | 0 | Create, SetActive | `MiniStore.Web/Views/PurchaseApprovalRules/Index.cshtml` |
| PurchaseDocumentReversals | Create.cshtml | 1 | 1 | 2 | Create, Details | `MiniStore.Web/Views/PurchaseDocumentReversals/Create.cshtml` |
| PurchaseDocumentReversals | Details.cshtml | 0 | 0 | 2 | Details, Index | `MiniStore.Web/Views/PurchaseDocumentReversals/Details.cshtml` |
| PurchaseDocumentReversals | Index.cshtml | 0 | 0 | 1 | Details | `MiniStore.Web/Views/PurchaseDocumentReversals/Index.cshtml` |
| PurchaseOrders | Details.cshtml | 14 | 14 | 7 | Approve, Cancel, CloseRemaining, ConfigureDropship, Confirm, Create, CreateFromOrder, CreatePortalAccess, Details, Index, Prepare, PurchaseOrderUbl, QueuePurchaseOrder, RecordDirectDelivery, RecordSupplierConfirmation, Reopen, RevokePortalAccess | `MiniStore.Web/Views/PurchaseOrders/Details.cshtml` |
| PurchaseOrders | Index.cshtml | 1 | 1 | 1 | Details | `MiniStore.Web/Views/PurchaseOrders/Index.cshtml` |
| PurchaseReplenishment | Index.cshtml | 2 | 2 | 2 | Details, RunNow, SaveSettings | `MiniStore.Web/Views/PurchaseReplenishment/Index.cshtml` |
| PurchaseRequests | Create.cshtml | 1 | 3 | 1 | Create, Index | `MiniStore.Web/Views/PurchaseRequests/Create.cshtml` |
| PurchaseRequests | Details.cshtml | 4 | 4 | 2 | Approve, Cancel, Create, Index, Reject, Submit | `MiniStore.Web/Views/PurchaseRequests/Details.cshtml` |
| PurchaseRequests | Index.cshtml | 1 | 1 | 2 | Create, Details | `MiniStore.Web/Views/PurchaseRequests/Index.cshtml` |
| PurchaseReturns | Create.cshtml | 1 | 1 | 1 | Details | `MiniStore.Web/Views/PurchaseReturns/Create.cshtml` |
| PurchaseReturns | Details.cshtml | 0 | 0 | 1 | Index | `MiniStore.Web/Views/PurchaseReturns/Details.cshtml` |
| PurchaseReturns | Index.cshtml | 0 | 0 | 1 | Details | `MiniStore.Web/Views/PurchaseReturns/Index.cshtml` |
| Purchases | Create.cshtml | 1 | 3 | 1 | Create, Index | `MiniStore.Web/Views/Purchases/Create.cshtml` |
| Purchases | Details.cshtml | 1 | 1 | 2 | Create, Index, Post | `MiniStore.Web/Views/Purchases/Details.cshtml` |
| Purchases | Index.cshtml | 0 | 0 | 2 | Create, Details | `MiniStore.Web/Views/Purchases/Index.cshtml` |
| PurchaseServiceAcceptances | Create.cshtml | 1 | 1 | 1 | Create, Details | `MiniStore.Web/Views/PurchaseServiceAcceptances/Create.cshtml` |
| PurchaseServiceAcceptances | Details.cshtml | 0 | 0 | 2 | Details, Index | `MiniStore.Web/Views/PurchaseServiceAcceptances/Details.cshtml` |
| PurchaseServiceAcceptances | Index.cshtml | 0 | 0 | 1 | Details | `MiniStore.Web/Views/PurchaseServiceAcceptances/Index.cshtml` |
| PurchaseSourcing | Create.cshtml | 1 | 1 | 1 | Create, Details | `MiniStore.Web/Views/PurchaseSourcing/Create.cshtml` |
| PurchaseSourcing | Details.cshtml | 4 | 4 | 6 | Attachment, Comparison, Create, CreatePortalAccess, Details, Index, Prepare, Print, RevokePortalAccess | `MiniStore.Web/Views/PurchaseSourcing/Details.cshtml` |
| PurchaseSourcing | Direct.cshtml | 1 | 4 | 3 | Create, Direct, Index | `MiniStore.Web/Views/PurchaseSourcing/Direct.cshtml` |
| PurchaseSourcing | Index.cshtml | 1 | 1 | 2 | Details, Direct | `MiniStore.Web/Views/PurchaseSourcing/Index.cshtml` |
| PurchaseSourcing | Print.cshtml | 0 | 1 | 0 | — | `MiniStore.Web/Views/PurchaseSourcing/Print.cshtml` |
| PurchaseTemplates | Index.cshtml | 2 | 2 | 0 | Generate, SetActive | `MiniStore.Web/Views/PurchaseTemplates/Index.cshtml` |
| Recipes | Edit.cshtml | 1 | 3 | 1 | Edit, Index | `MiniStore.Web/Views/Recipes/Edit.cshtml` |
| Recipes | Index.cshtml | 0 | 0 | 2 | Edit, Index | `MiniStore.Web/Views/Recipes/Index.cshtml` |
| Replenishment | Index.cshtml | 5 | 5 | 1 | CreatePurchaseRequestDraft, CreateTransferDraft, Details, Save, SetActive | `MiniStore.Web/Views/Replenishment/Index.cshtml` |
| Roles | _Form.cshtml | 1 | 1 | 1 | @(editing ? , Index | `MiniStore.Web/Views/Roles/_Form.cshtml` |
| Roles | Create.cshtml | 0 | 0 | 0 | — | `MiniStore.Web/Views/Roles/Create.cshtml` |
| Roles | Edit.cshtml | 0 | 0 | 0 | — | `MiniStore.Web/Views/Roles/Edit.cshtml` |
| Roles | Index.cshtml | 1 | 1 | 2 | Create, Delete, Edit | `MiniStore.Web/Views/Roles/Index.cshtml` |
| Sales | Create.cshtml | 1 | 3 | 1 | Create, Index | `MiniStore.Web/Views/Sales/Create.cshtml` |
| Sales | Details.cshtml | 1 | 1 | 2 | Create, Index, Post | `MiniStore.Web/Views/Sales/Details.cshtml` |
| Sales | Index.cshtml | 0 | 0 | 3 | Create, Details, Pos | `MiniStore.Web/Views/Sales/Index.cshtml` |
| Sales | Pos.cshtml | 1 | 6 | 1 | Create, Index | `MiniStore.Web/Views/Sales/Pos.cshtml` |
| SalesApprovalRules | Index.cshtml | 2 | 3 | 0 | Create, SetActive | `MiniStore.Web/Views/SalesApprovalRules/Index.cshtml` |
| SalesDeliveries | Create.cshtml | 1 | 1 | 3 | Create, Details, Reserve | `MiniStore.Web/Views/SalesDeliveries/Create.cshtml` |
| SalesDeliveries | Details.cshtml | 2 | 2 | 5 | Cancel, Create, Details, Index, Post | `MiniStore.Web/Views/SalesDeliveries/Details.cshtml` |
| SalesDeliveries | Index.cshtml | 1 | 1 | 1 | Details | `MiniStore.Web/Views/SalesDeliveries/Index.cshtml` |
| SalesOrders | Details.cshtml | 4 | 4 | 8 | ApproveException, Cancel, Confirm, Create, Details, Index, RejectException, Reserve | `MiniStore.Web/Views/SalesOrders/Details.cshtml` |
| SalesOrders | Index.cshtml | 1 | 1 | 1 | Details | `MiniStore.Web/Views/SalesOrders/Index.cshtml` |
| SalesOrders | Reserve.cshtml | 2 | 2 | 1 | Details, ReleaseReservation, Reserve | `MiniStore.Web/Views/SalesOrders/Reserve.cshtml` |
| SalesPriceLists | Index.cshtml | 5 | 5 | 1 | AddRule, Create, Evaluate, Index, SetListActive, SetRuleActive | `MiniStore.Web/Views/SalesPriceLists/Index.cshtml` |
| SalesQuotations | Create.cshtml | 2 | 5 | 1 | Create, Index | `MiniStore.Web/Views/SalesQuotations/Create.cshtml` |
| SalesQuotations | Details.cshtml | 6 | 6 | 3 | Accept, Cancel, CreateFromQuotation, Details, DownloadPdf, Expire, Index, Reject, Send | `MiniStore.Web/Views/SalesQuotations/Details.cshtml` |
| SalesQuotations | Index.cshtml | 1 | 1 | 2 | Create, Details | `MiniStore.Web/Views/SalesQuotations/Index.cshtml` |
| SalesQuotationTemplates | Create.cshtml | 1 | 4 | 2 | Create, Index | `MiniStore.Web/Views/SalesQuotationTemplates/Create.cshtml` |
| SalesQuotationTemplates | Details.cshtml | 3 | 3 | 2 | Activate, Archive, Create, Index, SetDefault | `MiniStore.Web/Views/SalesQuotationTemplates/Details.cshtml` |
| SalesQuotationTemplates | Index.cshtml | 1 | 1 | 2 | Create, Details | `MiniStore.Web/Views/SalesQuotationTemplates/Index.cshtml` |
| SalesReturns | Create.cshtml | 1 | 1 | 1 | Details | `MiniStore.Web/Views/SalesReturns/Create.cshtml` |
| SalesReturns | Details.cshtml | 0 | 0 | 1 | Index | `MiniStore.Web/Views/SalesReturns/Details.cshtml` |
| SalesReturns | Index.cshtml | 0 | 0 | 1 | Details | `MiniStore.Web/Views/SalesReturns/Index.cshtml` |
| Settings | _OrganizationIdentityField.cshtml | 0 | 1 | 0 | — | `MiniStore.Web/Views/Settings/_OrganizationIdentityField.cshtml` |
| Settings | Accounting.cshtml | 1 | 1 | 2 | Accounting, BranchSalesAccounts, Index | `MiniStore.Web/Views/Settings/Accounting.cshtml` |
| Settings | BranchSalesAccounts.cshtml | 1 | 1 | 1 | Accounting, BranchSalesAccounts | `MiniStore.Web/Views/Settings/BranchSalesAccounts.cshtml` |
| Settings | Currencies.cshtml | 1 | 1 | 1 | Currencies, Index | `MiniStore.Web/Views/Settings/Currencies.cshtml` |
| Settings | Discounts.cshtml | 1 | 1 | 1 | Discounts, Index | `MiniStore.Web/Views/Settings/Discounts.cshtml` |
| Settings | DocumentNumbers.cshtml | 1 | 1 | 0 | DocumentNumbers | `MiniStore.Web/Views/Settings/DocumentNumbers.cshtml` |
| Settings | FiscalPeriods.cshtml | 2 | 2 | 1 | ChangeFiscalPeriodStatus, FiscalPeriods, Index | `MiniStore.Web/Views/Settings/FiscalPeriods.cshtml` |
| Settings | General.cshtml | 1 | 1 | 2 | General, Index, OrganizationIdentity | `MiniStore.Web/Views/Settings/General.cshtml` |
| Settings | Index.cshtml | 0 | 1 | 1 | @card.Action | `MiniStore.Web/Views/Settings/Index.cshtml` |
| Settings | Inventory.cshtml | 1 | 1 | 3 | Index, Inventory, InventoryAccess | `MiniStore.Web/Views/Settings/Inventory.cshtml` |
| Settings | InventoryAccess.cshtml | 3 | 3 | 1 | ConfigureBranchWarehouse, ConfigurePosTerminalWarehouse, CreatePosTerminal, Inventory | `MiniStore.Web/Views/Settings/InventoryAccess.cshtml` |
| Settings | OrganizationIdentity.cshtml | 1 | 2 | 1 | Index, OrganizationIdentity | `MiniStore.Web/Views/Settings/OrganizationIdentity.cshtml` |
| Settings | Pos.cshtml | 2 | 2 | 1 | InventoryAccess, Pos | `MiniStore.Web/Views/Settings/Pos.cshtml` |
| Settings | PurchaseMatching.cshtml | 1 | 1 | 1 | Index, PurchaseMatching | `MiniStore.Web/Views/Settings/PurchaseMatching.cshtml` |
| Settings | PurchaseSourcingPolicy.cshtml | 1 | 1 | 1 | Index, PurchaseSourcingPolicy | `MiniStore.Web/Views/Settings/PurchaseSourcingPolicy.cshtml` |
| Settings | Units.cshtml | 2 | 2 | 1 | DeactivateUnit, Index, Units | `MiniStore.Web/Views/Settings/Units.cshtml` |
| Settings | WorkflowCatalog.cshtml | 0 | 0 | 3 | @step.Action, Index | `MiniStore.Web/Views/Settings/WorkflowCatalog.cshtml` |
| StockMovements | Index.cshtml | 0 | 0 | 1 | Index | `MiniStore.Web/Views/StockMovements/Index.cshtml` |
| StockTransactions | Create.cshtml | 1 | 1 | 1 | Create, Index | `MiniStore.Web/Views/StockTransactions/Create.cshtml` |
| StockTransactions | Index.cshtml | 1 | 1 | 13 | Index | `MiniStore.Web/Views/StockTransactions/Index.cshtml` |
| StockTransfers | Create.cshtml | 1 | 3 | 2 | Create, Index | `MiniStore.Web/Views/StockTransfers/Create.cshtml` |
| StockTransfers | Details.cshtml | 6 | 12 | 2 | Approve, Cancel, Edit, Index, Post, Reject, ReturnToDraft, Submit | `MiniStore.Web/Views/StockTransfers/Details.cshtml` |
| StockTransfers | Edit.cshtml | 1 | 4 | 2 | Details, Edit | `MiniStore.Web/Views/StockTransfers/Edit.cshtml` |
| StockTransfers | Index.cshtml | 1 | 1 | 4 | Create, Details, Edit, Index | `MiniStore.Web/Views/StockTransfers/Index.cshtml` |
| Subscription | Checkout.cshtml | 0 | 0 | 0 | — | `MiniStore.Web/Views/Subscription/Checkout.cshtml` |
| Subscription | Index.cshtml | 1 | 1 | 0 | Checkout | `MiniStore.Web/Views/Subscription/Index.cshtml` |
| SupplierCommunications | Details.cshtml | 1 | 1 | 3 | Attachment, Details, RecordActivity | `MiniStore.Web/Views/SupplierCommunications/Details.cshtml` |
| SupplierCreditNotes | Create.cshtml | 1 | 1 | 1 | Create, Details | `MiniStore.Web/Views/SupplierCreditNotes/Create.cshtml` |
| SupplierCreditNotes | Details.cshtml | 0 | 0 | 2 | Details, Index | `MiniStore.Web/Views/SupplierCreditNotes/Details.cshtml` |
| SupplierCreditNotes | Index.cshtml | 0 | 0 | 1 | Details | `MiniStore.Web/Views/SupplierCreditNotes/Index.cshtml` |
| SupplierIntegrations | CredentialCreated.cshtml | 0 | 0 | 1 | Index | `MiniStore.Web/Views/SupplierIntegrations/CredentialCreated.cshtml` |
| SupplierIntegrations | Index.cshtml | 3 | 3 | 1 | CreateCredential, DeactivateCredential, Retry, Submission | `MiniStore.Web/Views/SupplierIntegrations/Index.cshtml` |
| SupplierIntegrations | PortalAccessCreated.cshtml | 0 | 0 | 1 | Index | `MiniStore.Web/Views/SupplierIntegrations/PortalAccessCreated.cshtml` |
| SupplierIntegrations | Submission.cshtml | 0 | 0 | 2 | DownloadSubmission, Index | `MiniStore.Web/Views/SupplierIntegrations/Submission.cshtml` |
| SupplierPayments | Create.cshtml | 1 | 1 | 1 | Create, Index | `MiniStore.Web/Views/SupplierPayments/Create.cshtml` |
| SupplierPayments | Details.cshtml | 0 | 0 | 4 | Create, Details, Index | `MiniStore.Web/Views/SupplierPayments/Details.cshtml` |
| SupplierPayments | Index.cshtml | 1 | 1 | 1 | Details | `MiniStore.Web/Views/SupplierPayments/Index.cshtml` |
| SupplierPortal | Index.cshtml | 3 | 3 | 0 | ConfirmOrder, DeliveryNotice, Quotation | `MiniStore.Web/Views/SupplierPortal/Index.cshtml` |
| SupplierPortal | Unavailable.cshtml | 0 | 0 | 0 | — | `MiniStore.Web/Views/SupplierPortal/Unavailable.cshtml` |
| SupplierPurchasing | ImportPreview.cshtml | 1 | 1 | 1 | @(Model.FileFormat == , Index | `MiniStore.Web/Views/SupplierPurchasing/ImportPreview.cshtml` |
| SupplierPurchasing | Index.cshtml | 5 | 12 | 7 | Edit, ExportCsv, ExportExcel, Index, PreviewCsv, PreviewExcel, PriceBreaks, PriceExceptions, PriceHistory, SaveBulk, SetActive | `MiniStore.Web/Views/SupplierPurchasing/Index.cshtml` |
| SupplierPurchasing | PriceBreaks.cshtml | 1 | 8 | 1 | Index, SavePriceBreaks | `MiniStore.Web/Views/SupplierPurchasing/PriceBreaks.cshtml` |
| SupplierPurchasing | PriceExceptions.cshtml | 0 | 0 | 10 | Index, PriceExceptions, PriceHistory | `MiniStore.Web/Views/SupplierPurchasing/PriceExceptions.cshtml` |
| SupplierPurchasing | PriceHistory.cshtml | 0 | 0 | 1 | Index | `MiniStore.Web/Views/SupplierPurchasing/PriceHistory.cshtml` |
| SupplierQuotations | Comparison.cshtml | 2 | 2 | 3 | Award, Create, CreateFromAward, Details | `MiniStore.Web/Views/SupplierQuotations/Comparison.cshtml` |
| SupplierQuotations | Create.cshtml | 1 | 1 | 0 | Create | `MiniStore.Web/Views/SupplierQuotations/Create.cshtml` |
| Suppliers | _FormFields.cshtml | 0 | 0 | 0 | — | `MiniStore.Web/Views/Suppliers/_FormFields.cshtml` |
| Suppliers | _SupplierFormScripts.cshtml | 0 | 0 | 0 | — | `MiniStore.Web/Views/Suppliers/_SupplierFormScripts.cshtml` |
| Suppliers | Create.cshtml | 1 | 1 | 2 | Create, Index | `MiniStore.Web/Views/Suppliers/Create.cshtml` |
| Suppliers | Edit.cshtml | 1 | 1 | 3 | Edit, Index | `MiniStore.Web/Views/Suppliers/Edit.cshtml` |
| Suppliers | Index.cshtml | 1 | 1 | 3 | Create, Delete, Edit, Index | `MiniStore.Web/Views/Suppliers/Index.cshtml` |
| TaxRates | Create.cshtml | 1 | 1 | 0 | — | `MiniStore.Web/Views/TaxRates/Create.cshtml` |
| TaxRates | Index.cshtml | 0 | 0 | 1 | Create | `MiniStore.Web/Views/TaxRates/Index.cshtml` |
| UnassignedStock | Index.cshtml | 4 | 4 | 1 | Assign, CreateRule, Index, SetRuleActive | `MiniStore.Web/Views/UnassignedStock/Index.cshtml` |
| Users | Create.cshtml | 1 | 1 | 1 | Create, Index | `MiniStore.Web/Views/Users/Create.cshtml` |
| Users | Index.cshtml | 0 | 0 | 1 | Create | `MiniStore.Web/Views/Users/Index.cshtml` |
| VendorBills | Create.cshtml | 1 | 1 | 1 | Create, Details | `MiniStore.Web/Views/VendorBills/Create.cshtml` |
| VendorBills | Details.cshtml | 7 | 8 | 6 | AddAttachment, Approve, ApproveOverride, Cancel, Create, Details, DownloadAttachment, GenerateExtraction, Index, Post, ReviewExtraction | `MiniStore.Web/Views/VendorBills/Details.cshtml` |
| VendorBills | Index.cshtml | 1 | 1 | 1 | Details | `MiniStore.Web/Views/VendorBills/Index.cshtml` |
| Views | _ViewImports.cshtml | 0 | 0 | 0 | — | `MiniStore.Web/Views/_ViewImports.cshtml` |
| Views | _ViewStart.cshtml | 0 | 0 | 0 | — | `MiniStore.Web/Views/_ViewStart.cshtml` |
| WarehouseLocations | Create.cshtml | 1 | 1 | 1 | Create, Index | `MiniStore.Web/Views/WarehouseLocations/Create.cshtml` |
| WarehouseLocations | Edit.cshtml | 1 | 1 | 1 | Edit, Index | `MiniStore.Web/Views/WarehouseLocations/Edit.cshtml` |
| WarehouseLocations | Index.cshtml | 1 | 1 | 2 | Create, Edit | `MiniStore.Web/Views/WarehouseLocations/Index.cshtml` |
| Warehouses | Create.cshtml | 1 | 1 | 1 | Create, Index | `MiniStore.Web/Views/Warehouses/Create.cshtml` |
| Warehouses | Edit.cshtml | 1 | 1 | 1 | Edit, Index | `MiniStore.Web/Views/Warehouses/Edit.cshtml` |
| Warehouses | Index.cshtml | 1 | 1 | 3 | Create, Delete, Edit, Index | `MiniStore.Web/Views/Warehouses/Index.cshtml` |

The cutover allowlist applies only to GET/HEAD requests. Mutations, downloads, and every action without an explicit Angular route remain on MVC.
