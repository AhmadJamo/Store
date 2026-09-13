# Project TODO
> Source of truth: Current repository scan  
> Last reviewed: 2026-09-13

## Completed
- Core catalog, warehouse, supplier, inventory, purchase, sale and stock-transfer code exists.

## In Progress
- Unknown / Not determined from code.

## High Priority
- Rotate previously committed admin credentials and invalidate sessions on existing deployments (operator action).



## Bugs / Technical Debt
- Missing database unique constraints for barcode and master-data names.
- Invoice/document settings do not have consistent singleton/concurrency guarantees.
- Document number settings may be absent, blocking transfer creation.
- Direct EF use in RolesController; inconsistent UnitOfWork ownership.
- Duplicate `PermissionsCodeExport` tree can drift.

## Accounting Gaps
- Ledger, accounts, payments, customers, taxes, COGS/valuation, returns, fiscal periods, financial statements: not implemented.

## Testing Gaps
- Security regression executable added under `tests/SecurityRegression`; HTTP/database integration and penetration tests remain outstanding.

## Future ERP/SaaS Features
- Tenancy/subscriptions: not implemented; must be designed before SaaS launch.

## Security fixes completed (2026-09-13)
- Admin-only identity mutations enforced in both permission evaluators.
- Role deletion changed to antiforgery-protected POST; delete links replaced with forms.
- Login lockout and per-IP rate limiting enabled.
- Default admin password removed; bootstrap opt-in and existing-account promotion blocked.
- Broad exception handlers log details server-side and show generic errors.

## Inventory integrity completed (2026-09-13)
- Added ProductStock rowversion and Serializable UnitOfWork transactions for concurrent writes.
- Restricted manual movements to adjustments with a required reason.
- Rejected duplicate product lines in purchase and sale aggregates.
- Added focused metadata and domain regression checks. Database-backed concurrent-operation and reconciliation tests remain outstanding.

