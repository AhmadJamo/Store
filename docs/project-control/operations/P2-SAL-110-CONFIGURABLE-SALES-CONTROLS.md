# P2-SAL-110 — Configurable Sales Controls

> Completed in source: 2026-10-22  
> Database verification: blocked before execution by host SQL encryption/SSPI connectivity

## Delivered outcome

Sales Order confirmation now evaluates tenant-owned rules that can apply company-wide or to one customer. Administrators configure metric, comparison, threshold, action, priority, effective dates, activation and ordered approver roles. Supported metrics are order company amount, derived credit exposure, overdue days, discount percentage and margin percentage. Supported actions are warning, required approval and block.

Each review freezes the rule name, metric, measured value, threshold and action. Multiple approval triggers combine into one sequential role workflow. The strictest triggered policy controls requester self-approval and whether one user may approve multiple steps. Warning-only reviews can confirm with evidence; blocked and pending reviews keep the order Draft. Final approval confirms the order without bypassing the Application layer. No inventory, delivery, invoice, journal or cash state changes during evaluation.

## Security and integrity

- Separate rule View/Manage and Sales Order exception-approval permissions.
- POST antiforgery on rule-state and approval decisions.
- Server-side current-role authorization per step.
- Configurable requester and distinct-approver separation of duties.
- Tenant query filters/composite relationships and one Pending review per order.
- Immutable finding and decision evidence protects history from later rule changes.

## Verification

- Release solution build: passed with 0 warnings and 0 errors.
- Focused regression: passed 1004 checks.
- EF model reconciliation: no pending model changes.
- Migration: `20261021222759_AddConfigurableSalesControls` (131).
- Disposable SQL scenario: added for persisted finding, Pending/Draft boundary and final approval; execution is unverified because the host SQL connection fails before tests begin.

## Operational follow-up

After SQL connectivity is repaired, apply migrations 129-131 to Demo and run the complete disposable SQL suite. Do not record the migration or SQL evidence as passed until those commands execute successfully.
