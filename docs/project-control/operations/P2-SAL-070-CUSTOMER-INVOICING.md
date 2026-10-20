# P2-SAL-070 — Customer Invoicing Evidence
> Completed: 2026-10-20

## Delivered

- Tenant-owned CustomerInvoice header, line, delivery-allocation and history records with `INV-` numbering and rowversion.
- Company-configurable Delivered/Ordered quantity policy; Delivered is the safe stocked-product default and services use ordered quantity.
- Idempotent partial/multiple Draft creation, review approval and reasoned cancellation that releases eligibility.
- Frozen customer, address, product, unit, price, exchange-rate, tax and source evidence with proportional amount reconciliation.
- Four permissions, bilingual responsive screens, dashboard/navigation entry, actionable source links and permission-aware next-step guidance.
- Completed one-time actions are replaced by Approved/Cancelled state. Fully committed source quantity hides the create action.
- No stock, COGS, revenue, tax, receivable or payment effect in SAL-070.

## Database

- Migration `20261020202938_AddCustomerInvoicingFoundation` (124).
- Applied successfully to local `MiniStoreDb`; source, model and Demo are aligned at 124.
- Four tenant-owned tables use composite company foreign keys and tenant-unique invoice/request identities.

## Verification

- Build: zero warnings and zero errors.
- SecurityRegression: 908 checks passed.
- Disposable SQL integration: passed partial 0.4/0.6 invoicing, exact allocations, over-invoice rejection, cancellation release, idempotency, tenant isolation and no premature stock/accounting effects.
- EF Core: no pending model changes.

## Boundary / next gate

SAL-080 is the sole proposed next task. It will add customer receivable, revenue and output-tax posting/reversal to the same invoice; it must not treat posting as payment. Explicit approval is required before implementation.
