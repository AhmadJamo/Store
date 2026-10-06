# Project TODO
> Source of truth: Current repository scan  
> Last reviewed: 2026-10-04

The ordered Arabic execution plan, workflow, business rules and rolling suggestion queue are maintained in `ARABIC_SHARED_ROADMAP.md`. This file remains the concise technical backlog.

- [x] WMS-050 reservations and exact transfer allocation with concurrent last-unit protection.
- [x] WMS-060 blind cycle counts and controlled inventory adjustment posting; scheduling remains deferred.
- [x] WMS-070A opening lot/serial allocation and traceability foundation.
- [x] WMS-070B tracked purchases, FEFO sales/recipe issues, transfer/cancellation and identifier-aware sales/purchase returns.
- [x] Product shelf-life defaults, mandatory receipt expiration, expiry blocking and warning dashboard.
- [x] Add permission-protected, reasoned lot/serial quarantine and release with immutable trace events.
- [x] Add tracked-identity recall creation/closure, automatic quarantine and affected-document trace.
- [x] Add a live expiration alert center sourced from current tracked balances and per-product warning horizons.
- [x] Resolve recall history into affected document types, supplier/customer identities, contacts and quantities.
- [x] Add immutable manual recall communication history with channel and outcome.
- [x] WMS-080A apply warehouse FIFO/FEFO/location-priority/minimize-locations policy to tracked issues and transfers.
- [x] WMS-080B1 add auditable manual tracked-identity override to wholesale sales.
- [x] WMS-080B2 extend audited identity override to transfers and preserve exact identities on cancellation.
- [x] WMS-080B3 extend audited identity override to POS tracked products.
- [x] WMS-080C1 add the tenant-safe putaway-rule model and deterministic suggestion service.
- [x] WMS-080C2 add putaway-rule administration UI and capacity-aware suggestions.
- [ ] WMS-080C3 add policy-driven untracked exact-location removal.
- [ ] Add scheduled/automated delivery channels and delivery-provider callbacks.

## Completed
- Refactored the Web composition root into focused presentation, persistence, security, business-service and application-pipeline extensions; removed duplicate sale registrations without changing runtime behavior.
- Core catalog, warehouse, supplier, inventory, purchase, sale and stock-transfer code exists.
- Shared-database tenant isolation is implemented for all current business entities, including active membership validation, tenant-scoped reads/writes/unique indexes and demo-data backfill.
- Added separately authenticated platform control center, bilingual public landing/pricing pages, plans/features/limits, tenant subscription states, user/warehouse limit enforcement and bounded percentage promotion codes.
- Added atomic self-service company registration with a selected-plan trial, tenant subscription self-service, persisted monthly/annual checkout quotes, promotion calculation/redemption and protected manual payment confirmation.
- Added tenant-scoped user-role assignments and removed cross-company global Admin checks from tenant administration controllers.
- Replaced separate invoice/transfer counters with centralized, tenant-scoped, rowversion-protected document sequences for wholesale, POS, transfers and journals.
- Added tenant-owned role definitions and permission sets, protected company Admin roles and tenant-local user assignments; removed direct EF access from RolesController.
- Reworked registration throttling to a configurable 20 requests/hour/IP default and added bilingual retry feedback instead of exposing a raw HTTP 429 page.
- Added a bilingual registration-rules popup covering company name/address, owner username/email, password complexity, selected plan and the 14-day trial.
- Added centralized immutable entity classification and database-enforced composite TenantId foreign keys for every tenant-to-tenant ERP/SaaS relationship; 59 protected relationships and no unsafe relationship remain in the local schema.
- Added and locally migrated the cafe recipe foundation: immutable recipes, unit conversion, automatic ingredient consumption, explicit kitchen variance and per-ingredient controlled negative stock.
- Added the authenticated Dashboard landing flow and locally migrated product classification, generated internal codes, optional barcodes, POS/sales channel controls and the advanced product catalogue.
- Replaced product and recipe unit entry with the tenant-managed measurement catalogue, including safe legacy backfill and immutable unit-code/factor snapshots on recipe versions.
- Added moving weighted-average valuation per product/warehouse, immutable cost snapshots, recipe-derived sale cost and provisional-negative settlement variance.
- Added idempotent sale posting for settlement, invoice discount, revenue, COGS and warehouse inventory.
- Added provisional-negative cost-variance settlement to purchase posting and excluded recoverable inclusive input tax from moving-average inventory cost.
- Added immutable invoice-level sales-tax snapshots, inclusive/exclusive calculation and output-tax journal posting.
- Added immutable partial/full sales returns with cumulative quantity control, proportional refund/tax reversal, direct-item restocking at historical cost and an atomic posted reversal journal.
- Added a central journal-posting gateway used by purchase, sale and sales-return workflows for source idempotency, numbering, balancing, posting and persistence.
- Added tenant fiscal periods with overlap prevention, Open/Soft Closed/Closed states, status audit metadata, concurrency protection and central posting-date enforcement.
- Added immutable partial/full purchase returns with cumulative and available-stock controls, moving-average issues, proportional supplier/tax/discount reversal and COGS cost-variance posting.
- Completed the repository-wide WMS gap analysis, target authority model, safe migration strategy, ADR, phased vertical-slice roadmap and file change map without changing runtime inventory code.
- Added WMS-000 read-only bilingual inventory reconciliation, dedicated permission/navigation, structured movement identity/cutover contract and a disposable two-tenant SQL fixture for query translation and concurrent last-unit protection.
- Implemented WMS-005 product categories, Mass/Length logistics metadata, handling requirements, tracking-policy guard, catalogue filters and additive migration; the migration is applied to the development database.
- Implemented WMS-010 hierarchical location metadata, barcode, sequence, tree ordering, warehouse-filtered parent selection, operational capabilities, edit workflow and cycle/same-warehouse validation. Putaway/transfers/relocations enforce receive/pick capabilities. Its additive migration is applied to `AHMAD/MiniStoreDb`; authenticated UI smoke remains because no login secret is stored in the repository.

## In Progress
- WMS is the active delivery track until its planned inventory/warehouse phases are complete. Next: smoke-test WMS-010, then proceed to WMS-015; see `WMS_EVOLUTION_PLAN.md`.
- WMS-015A/B/C/D typed definitions, values, ProductTemplate grouping and controlled selection-based variant generation are implemented. Product remains the SKU; proceed to the next WMS slice.
- WMS-020A physical StockMovement kernel is implemented for idempotent Putaway/Relocation dual-write. Next: authenticated pilot smoke test and WMS-030 transfer/transit integration; do not cut over legacy reads yet.
- WMS-030 transfer movement stages are implemented without historic backfill. Next: authenticated transfer lifecycle smoke test, then WMS-040 explicit balances and availability; partial transit receipt remains deferred.
- WMS-040 InventoryBalance projection is implemented and reconciled (8 rows, zero ProductStock mismatches in the demo database). Next: authenticated workflow smoke tests, then WMS-050 reservations; keep ProductStock/ProductLocationStock authoritative during observation.
- Run an authenticated HTTP/SQL cafe recipe sale and reconciliation journey. Batch production, actual yields and negative-cost settlement remain follow-up work.
- Add per-terminal product assortment UI/enforcement and per-user branch/POS data scope.
- Build the central preview/validation/template/audit engine for Excel and CSV import/export, then onboard modules incrementally.
- Build generalized reversal metadata and controlled cancellation workflows on the central posting and fiscal-period foundation.
- Migrate remaining legacy feature screens and existing validation messages to the shared English/Arabic resources before making further functional changes to those screens.

## High Priority
- Do not begin movement/balance cutover before reconciling ProductStock, ProductLocationStock, implicit Unassigned Stock and latest StockTransaction snapshots per tenant/product/warehouse.
- Rotate previously committed admin credentials and invalidate sessions on existing deployments (operator action).



## Bugs / Technical Debt
- Missing database unique constraints for barcode and master-data names.
- Duplicate `PermissionsCodeExport` tree can drift.

## Accounting Gaps
- Sales/purchase posting, immutable posted sales/purchase returns and fiscal-period enforcement are implemented; provisional-negative variance settles during purchase posting. Mixed-rate product tax, generalized reversals and financial statements remain pending.

## Testing Gaps
- Security regression executable checks complete entity classification and composite tenant relationships. A live two-company SQL mutation was rejected by the database; comprehensive repository HTTP/database integration and penetration tests remain outstanding.

## Future ERP/SaaS Features
- Add company switching/invitations, editing existing user assignments, role templates/data scopes, plan versioning/overrides, external provider checkout/webhooks/refunds, 2FA for platform operators and controlled support impersonation.

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
- Added focused metadata/domain regression checks and a SQL-backed two-tenant reconciliation/concurrent-last-unit fixture. Reservation and transfer retry concurrency follow their WMS phases.

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


## Completed 2026-09-15: guided company onboarding
- [x] Continue registration into signed-in business setup.
- [x] Add presets for retail, grocery, cafe, restaurant, quick service, wholesale and services.
- [x] Create linked accounting, branch, warehouse, payment, tax, numbering, discount and POS foundations transactionally.
- [x] Support explicit Skip and resumption of Pending setup.
- [x] Accept and membership-validate the company slug during login.
