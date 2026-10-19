# P2-SAL-050C / SAL-030 — Sales quotations
> Completed: 2026-10-20

## Delivered

- Tenant-owned quotation header, immutable commercial lines and semantic lifecycle history.
- Draft, Send, Accept, Reject, Expire and Cancel transitions with rowversion concurrency.
- Customer/address, product/unit, price-list/rule, currency/exchange-rate and tax snapshots.
- Multi-line bilingual responsive screens with name/code/barcode product search and no horizontal table scrolling.
- Six dedicated permissions, antiforgery on every mutation, permission-aware errors and workflow next-step guidance.
- Tenant-local `SQT-` document numbering and request-key idempotency.
- Migration `20261019222054_AddSalesQuotations` (migration 120).

## Safety boundary

Quotation creation and lifecycle transitions do not reserve, issue or receive stock and do not create journals, invoices or payments. Acceptance closes the quotation action and shows the completed state; Sales Order creation is intentionally deferred to the separately authorized `SAL-040` slice.

## Verification

- Final Release build: zero warnings and zero errors after code, localization and documentation updates.
- Security/inventory regression: 836 checks passed.
- EF Core model reconciliation: no pending model changes.
- SQL integration project compiles and now contains quotation tenant-isolation, request-key uniqueness and zero-stock/zero-journal assertions. The disposable run was blocked before database creation by the local Windows SSPI authentication context.
- Applying migration 120 to local Demo was attempted but blocked by the local SQL Server encryption/connection environment; source and model are at 120 while Demo remains evidenced at 119 until that connection is repaired.

