# MOD-031D — Fifteen-surface operational retirement

> Completed: 2026-11-14

Retired MVC/Razor presentation for Accounts, CustomerInvoices, CustomerReturnRequests, Customers, DocumentDesigns,
PurchaseRequests, Purchases, PurchaseSourcing, Sales, SalesOrders, SalesPriceLists, SalesQuotations,
SalesQuotationTemplates, SupplierPurchasing and VendorBills.

Angular and authenticated APIs remain authoritative. Application services, UnitOfWork, permissions, antiforgery,
posting, inventory, pricing and workflow rules remain intact. Forty-seven protected GET/HEAD routes preserve legacy
navigation. Retired writes return 405. PDF, attachment and CSV/Excel downloads remain on their permission-protected APIs
and are not redirected from old MVC download URLs.

Verification: 15 MVC surfaces, 91 actions, 50 Razor control rows and 45 views remain. Release passed with zero warnings
and errors, 1,605 regression checks, 242/242 Angular tests, accessibility, bundle and browser gates.
