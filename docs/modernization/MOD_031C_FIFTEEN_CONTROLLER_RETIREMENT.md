# MOD-031C — Fifteen-controller commercial retirement

Completed 2026-11-14. Retired MVC/Razor presentation for Branches, PaymentMethods, TaxRates, Products,
CustomerCreditNotes, CustomerReturnReceipts, LandedCosts, PurchaseAgreements, PurchaseAnalytics, PurchaseOrders,
PurchaseReturns, PurchaseTemplates, SalesReturns, SupplierCommunications and SupplierQuotations.

Angular and authenticated APIs remain authoritative. Application services, UnitOfWork, DTO validation, permissions,
antiforgery and business rules remain intact. 34 protected GET/HEAD routes preserve navigation; retired writes return
405 and are never replayed. Supplier communication attachments remain API-only.

Verification: 30 MVC surfaces, 214 actions, 100 Razor rows, 93 views; Release zero warnings/errors; 1,638 regression
checks; 242/242 Angular tests; bundle budgets and browser acceptance passed.
