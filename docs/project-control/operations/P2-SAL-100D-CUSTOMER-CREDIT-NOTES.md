# SAL-100D Customer Credit Notes Evidence

## Outcome

- Added independent tenant-owned customer credit-note headers and lines linked to Posted customer invoices and Posted customer-return receipt lines.
- Eligibility is capped cumulatively by invoice quantity, exact delivered allocation and remaining received return quantity.
- A tenant-unique request key returns the original document on a repeated submission and the database index protects concurrent duplication.
- Posting debits frozen sales revenue and output tax and credits frozen receivables in company currency with branch/warehouse dimensions.
- Posted credits reduce derived invoice outstanding/payment eligibility and block invoice reversal.
- The flow calls no inventory, tracking, stock movement or cash-refund authority.
- Dedicated View/Create permissions, antiforgery, bilingual RTL-safe screens, navigation and evidence links were added.

## Schema and verification

- Migration 130: `20261021220324_AddCustomerCreditNotes`.
- EF reports no pending model changes.
- Release build: passed with zero warnings/errors.
- Security/inventory regression: 980 checks passed.
- Disposable SQL scenario compiles and asserts balanced credit posting, reduced invoice balance and zero new inventory transaction/movement.

## Open operational evidence

The 2026-10-22 disposable SQL run stopped before test execution with `Cannot generate SSPI context`. Applying migrations 129-130 to local Demo stopped before execution because the host SQL Server required encryption unsupported by the current machine connection. Demo therefore remains evidenced at migration 128; no operational success is claimed for the new schema until connectivity is repaired and both commands are rerun.

## Next gate

`SAL-110` sales-credit/warning/approval controls are now completed in source. Cash refund or applying a customer credit to another balance remains a separate controlled capability.
