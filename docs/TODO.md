# Project TODO
> Source of truth: Current repository scan  
> Last reviewed: 2026-09-14

## Completed
- Core catalog, warehouse, supplier, inventory, purchase, sale and stock-transfer code exists.
- Shared-database tenant isolation is implemented for all current business entities, including active membership validation, tenant-scoped reads/writes/unique indexes and demo-data backfill.

## In Progress
- Add sale tax and journal posting, weighted-average COGS calculation, purchase reversal and fiscal-period controls.
- Migrate remaining legacy feature screens and existing validation messages to the shared English/Arabic resources before making further functional changes to those screens.

## High Priority
- Rotate previously committed admin credentials and invalidate sessions on existing deployments (operator action).



## Bugs / Technical Debt
- Missing database unique constraints for barcode and master-data names.
- Invoice/document settings do not have consistent singleton/concurrency guarantees.
- Document number settings may be absent, blocking transfer creation.
- Direct EF use in RolesController; inconsistent UnitOfWork ownership.
- Duplicate `PermissionsCodeExport` tree can drift.

## Accounting Gaps
- Sale posting, moving weighted-average valuation/COGS calculation, payments, returns, fiscal periods and financial statements are not implemented. Purchase posting, chart accounts, journal-entry validation, branches, customer/supplier account links, tax account links and posting-account settings are implemented.

## Testing Gaps
- Security regression executable added under `tests/SecurityRegression`; HTTP/database integration and penetration tests remain outstanding.

## Future ERP/SaaS Features
- Add tenant administration/onboarding, company switching for multi-company users, tenant-specific role assignments, subscription plans/limits, billing provider integration and platform-operator controls. The data-isolation foundation is implemented.

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

## Warehouse location workflows completed (2026-09-14)
- Added unassigned-stock putaway, exact location balances, internal location-to-location movement and immutable putaway/relocation history.
- Enforced active same-warehouse locations, source availability and total destination-location capacity inside Serializable transactions.
- Added configurable warehouse operational types, Simple/LocationManaged/Hybrid control, POS eligibility, picking strategy, capacity enforcement and optional transfer-location requirements.
- Added rowversion-protected Inventory Settings defaults for new warehouses. Automatic FIFO/FEFO allocation and replenishment tasks remain follow-up work.
- Added branch warehouse operation permissions/priorities and per-POS default/alternative warehouse allow-lists. Automatic picking and replenishment tasks remain follow-up work.

## POS experience completed (2026-09-14)
- Added rowversion-protected per-terminal POS profiles and custom layout settings with a live administration preview.
- Added Retail, Grocery, Cafe, Restaurant and Quick Service presentation presets and runtime application on terminal selection.
- Added configurable Walk-in/Dine-in/Takeaway/Delivery modes, persisted service references, guest counts and per-item preparation notes with server-side terminal-policy validation.
- Restaurant table occupancy, kitchen tickets/routing, modifiers, combo meals, delivery dispatch and scale integration remain future POS workflow phases.

## Localization foundation completed (2026-09-14)
- Added company-default and per-user English/Arabic culture selection, centralized Arabic resources, RTL/LTR layout switching and Bootstrap RTL.
- Localized shared navigation, login/access-denied, notifications, delete confirmation, Settings hub and General Settings; all future UI changes must include both languages under `AGENTS.md`.

