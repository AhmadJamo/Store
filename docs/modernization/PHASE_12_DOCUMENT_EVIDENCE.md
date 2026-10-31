# MOD-012 — Document journey, related records and Explain This Number

> Status: COMPLETED  
> Completed: 2026-10-24  
> Database changes: none

## Outcome

Sales Orders and Customer Invoices now expose explicit, permission-protected evidence endpoints. A shared Application
contract supplies the document journey, server-known related records and explained-number parts. Angular renders one
responsive bilingual evidence experience and never reconstructs totals, settlements or relationships from labels.

## Authority and permissions

- `SalesOrders.View` and `CustomerInvoices.View` independently protect their list/detail evidence endpoints.
- Cross-document links are included only when the current user has the target document's View permission.
- Quotation and delivery sources remain visible as evidence but are not linked until their Angular routes exist.
- Order total is explained from the frozen net/tax snapshot. Invoice total and outstanding balance are explained from
  invoice snapshots plus authoritative posted payment/credit amounts.
- Lists are bounded to the newest 100 records per permitted source; no generic entity or graph traversal endpoint exists.

## Verification

- Web Release build: zero warnings/errors.
- Backend regression: 1087 checks passed, including route and permission contracts.
- Angular production compilation and the final 55-test suite passed, including the locale-independent shared-component coverage.
- No migration or business-state mutation.

## Rollback

Remove the four read-only evidence endpoints and Angular Documents route. Existing MVC sales documents, calculations,
posting and persistence remain unchanged.
