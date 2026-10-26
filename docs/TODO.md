# Project TODO
> Source of truth: Current repository scan  
> Last reviewed: 2026-10-27

The approved numbered Arabic/English delivery sequence is maintained in `MASTER_IMPLEMENTATION_ROADMAP_AR_EN.md`. Detailed working status, business rules and the rolling suggestion queue remain in `ARABIC_SHARED_ROADMAP.md`. This file remains the concise technical backlog.

The PUR-000 through PUR-280 purchase evolution is implemented. The approved completion sequence is maintained in `PURCHASE_COMPLETION_ROADMAP_AR_EN.md`. Incoming customer-management and sales requirements must be reconciled with master-roadmap steps 7 through 11 before implementation.

The ERP SaaS Foundation Blueprint is imported as a non-authoritative planning reference. `FND-000` reconciles it with current code; later `FND` slices are not implemented until explicitly approved and evidenced.

- [x] MOD-000 complete the read-only MVC-to-API/Angular repository audit, target architecture, dependency graph, workspace design, MVP, feature matrix and risk assessment. Completed 2026-10-24.
- [x] MOD-001 create the isolated modernization branch and Angular 22 foundation in `MiniStore Front-End`, with strict standalone architecture, lazy routing, RTL/LTR shell, design tokens, restricted dev proxy, Signals workspace boundary, documentation, rollback and verified frontend/backend builds. Completed 2026-10-24.
- [x] MOD-002 add the versioned internal API foundation: same-origin session/CSRF contracts, permission/tenant enforcement, standardized ProblemDetails, authenticated internal-only OpenAPI, bounded paging conventions, Angular clients and focused security/live HTTP checks. Completed 2026-10-24.
- [x] MOD-003 establish the reusable visual-system foundation: semantic tokens, system/light/dark themes, comfortable/compact density, persisted RTL/LTR preferences, focus/motion/forced-color rules and a bilingual state preview. Completed 2026-10-24.
- [x] MOD-004 build the reusable workspace engine: permission-aware sidebar metadata, tab identity/deduplication, per-tab history, close/close-others, refresh restoration and dirty-state navigation guard. Completed 2026-10-24 with 12 Angular tests and unchanged backend behavior.
- [x] MOD-005 deliver the first Customers Angular vertical slice with explicit API DTOs, server-side paging/search, validation, concurrency and MVC parity. Completed 2026-10-24 with 17 Angular tests and 1076 backend checks.
- [x] MOD-006 extract only proven shared form/table primitives and validate them against a second real feature consumer. Completed 2026-10-24 with Customers/Suppliers, 22 Angular tests and 1079 backend checks.
- [x] MOD-007 complete Suppliers as the next Angular migration slice: details/create/edit, validation, concurrency and MVC parity. Completed 2026-10-24 with 26 Angular tests and 1081 backend checks.
- [x] MOD-008 add a permission-aware Command Center foundation with reusable quick preview, favorites and bounded user snapshots. Completed 2026-10-24 with 30 Angular tests.
- [x] MOD-009 add reusable smart-form productivity: explicit draft policy, saved non-sensitive filters and keyboard-first entry without automatic posting. Completed 2026-10-24 with 36 Angular tests.
- [x] MOD-010 complete responsive dashboard, density, focus and layout preferences with safe reset and accessibility evidence. Completed 2026-10-24 with visual review and 39 Angular tests.
- [x] MOD-011 add reusable duplicate/conflict/data-quality and recovery presentation over authoritative backend evidence. Completed 2026-10-24 with stable API codes, typed duplicate outcomes, reusable Customer/Supplier recovery, 52 Angular tests and 1084 backend checks.
- [x] MOD-012 add reusable document journey, related-record evidence and Explain This Number presentation over authoritative backend evidence. Completed 2026-10-24 on Sales Orders and Customer Invoices without database changes.
- [x] MOD-013 add a permission-aware action/exception center and contextual help over existing server decisions. Completed 2026-10-24.
- [x] MOD-014 add allowlisted Focused/Operator/Analyst workspace profiles and a responsive split context panel without changing permissions. Completed 2026-10-24.
- [x] MOD-015 expose the reusable Analytics Center through Angular with server grouping/comparison, allowlisted drill-down and bounded export preview. Completed 2026-10-24.
- [x] MOD-016 add permission-filtered runtime monitoring for real hosted workers, SLA state and truthful analytics refresh plans without pretending report delivery exists. Completed 2026-10-24.
- [x] MOD-017 governed Angular workspace builder/save, backend-authored financial preview and tenant-safe user/role delegation with optional start/expiry, revocation evidence and source-permission rechecks. Completed 2026-10-25.
- [x] MOD-018A reusable acceptance gate: Release/regression, Angular tests/build, external SCSS, explicit initial/lazy budgets, static accessibility and real Edge RTL/LTR/keyboard/state/mobile checks with one passed/failed/skipped report. Completed 2026-10-25.
- [x] MOD-018B prove the real authenticated session and three permission-allowed read endpoints with an external short-lived storage state; the final gate passed 11/11 with zero skips and deleted the temporary state. Completed 2026-10-25.
- [x] MOD-019 generate the complete 89-controller/189-view/344-MVC-action/198-Razor-file registry and add reversible, GET-only, allowlisted per-surface cutover controls with MVC committed as the default. Completed 2026-10-25.
- [x] MOD-020 complete all current Settings/Document Design, account/session, tenant-user/role, public registration and
  guided onboarding operations in Angular. Identity, tenant membership, provisioning, lockout/audit, CSRF and HttpOnly
  cookies remain server-owned; MVC remains the default until a later cutover gate. Completed 2026-10-27.
- [ ] MOD-021 IN PROGRESS: `MOD-021A..B` completed Angular product list/details/create/edit/delete and category
  list/create/activate/deactivate with separate permissions, CSRF and tests. Continue with attributes/values, templates,
  recipes and the remaining catalog masters. Completed A/B on 2026-10-27.
- [ ] MOD-022 through MOD-028 migrate every remaining tenant, public and platform MVC workflow to Angular by the approved module waves, including actions, permissions, localization, files, PDF and edge states.
- [ ] MOD-029 prove full-system parity through dual-run reconciliation, performance/security/accessibility evidence and cutover/rollback rehearsals.
- [ ] MOD-030 make Angular the only presentation and retire MVC incrementally only after explicit approval and zero registry gaps.

- [x] FND-000 import the ERP SaaS Foundation Blueprint with provenance and classify its ten capabilities against the actual MiniStore repository.
- [ ] FND-010 inventory production environments and operational ownership, approve RPO/RTO and backup retention, run an isolated restore drill, reconcile restored financial/inventory/tenant/outbox state and record evidence.
- [ ] FND-020 through FND-080 incrementally harden audit/security, integrity/idempotency, observability/performance, configuration/approval reuse, provisioning/billing, exception/work guidance and safe import/export after their individual approvals.
- [x] CTRL-000 complete the repository recovery audit and create the consolidated project-control status, inventory, audits, conflict register, dependency map, single roadmap, decision/debt/test registers and handoff context.
- [x] P0-SEC-001 close the historically published administrator credential on the owner-declared local Demo environment, invalidate sessions, prove old-secret rejection and verify a replacement administrator.
- [x] P0-OPS-002 create an AES-256 encrypted local Demo backup, verify and restore it in isolation, reconcile 147 tables plus business/accounting totals, and clean up only the restore database.
- [x] P0-DB-003 prove all 112 EF Core migrations match the local Demo database, no model change is pending and no disabled/untrusted constraint or tenant relationship drift exists.
- [x] P1-AUD-010 approve the audit-security design for correlation, outcomes, redaction, access, retention classes, staged tamper evidence, additive migration, rollback and tests without runtime changes.
- [x] P1-AUD-011 add and apply the AuditEvent schema, core Application writer, allowlist redactor and keyed integrity hash behind a disabled-by-default feature flag; legacy audit history remained unchanged.
- [x] P1-AUD-012 enabled first semantic audit coverage for tenant role/permission and six sensitive-setting groups with local Demo key material held only in User Secrets; SQL proves one valid event per successful operation and no success event on rejection/rollback.
- [x] P1-AUD-013 add authentication/lockout, actual denied privileged action, platform administration and external integration semantic events with an independent failure-visible security writer and masked request evidence.
- [x] P1-AUD-014 add permission-aware tenant/platform audit readers, deterministic pagination, sensitive-access controls and a bounded purpose-required audited export.
- [x] P1-AUD-015 add SQL writer/reader/maintainer roles and separate connection hooks, approved 7/3-year and 90-day retention, Owner-only legal holds, signed external checkpoints and manifest-before-delete execution.
- [x] P1-AUD-016 reconcile 37 critical controls, preserve 710 historical `AuditLogs` rows, freeze new generic writes behind a reversible independent flag and prove semantic continuity plus rollback in SQL.
- [x] P1-OBS-020 add the owner-approved local build/SQL gate, secret-free health checks, vendor-neutral metrics, testable log alerts and a measured p95/p99 availability workload.
- [x] P1-ARC-030 complete the first bounded architecture slice: WarehouseLocations now receives projected Application DTOs and no longer injects warehouse/location repositories; continue the pattern incrementally when other slices are touched.
- [x] P2-CUS-040A / CUS-005 preserve CustomerId/SupplierId, expand Customer first and defer BusinessPartner unification to a separately approved CUS-110 gate.
- [x] CUS-010 implement the additive professional customer master with stable ID, code/profile/archive/concurrency, DTO-only paged queries, permissions, localization and regression coverage.
- [x] CUS-020 add tenant-safe customer contacts and billing/shipping addresses with DTO snapshot contracts, defaults, archive and concurrency.
- [x] CUS-030A establish the configurable customer-receivable policy: shared control, automatic individual account or manual selection, with prospective effect and transition guard.
- [x] CUS-030B add prospective customer payment-term, configured-currency and tax/exemption defaults; defer price list and salesperson/team to their owning SAL-020+ capabilities.

- [x] PUR-000 complete purchase architecture review, legacy compatibility boundary, migration strategy and vertical-slice roadmap.
- [x] PUR-005 fix context ownership, quantity authority, staged currency, approval and GRNI boundaries before runtime implementation.
- [x] PUR-010 add supplier-product purchasing information and lead time; migration applied to `AHMAD/MiniStoreDb`.
- [x] PUR-020 add the Purchase Request lifecycle with Draft/Submit/Cancel, frozen units, history, permissions and bilingual UI.
- [x] PUR-025 add configurable purchase approval rules, frozen instances, ordered decisions, permissions and bilingual UI.
- [x] PUR-030 add sourcing events and supplier invitations from approved Purchase Requests without stock changes.
- [x] PUR-040 add supplier quotations, comparison and a reasoned award decision.
- [x] PUR-050 add the Purchase Order lifecycle without stock mutation. Receipt/bill-based closure is deferred to later slices.
- [x] PUR-060 complete partial Goods Receipts, history, physical movement facts and SQL concurrency coverage.
- [x] PUR-065 complete Inventory/GRNI posting, Vendor Bill clearing, unbilled receipt-return reversal and PUR-160 reconciliation reporting with SQL coverage.
- [x] PUR-070 add receipt-based supplier returns with unbilled-quantity caps, inventory/tracking removal and GRNI/PPV reversal while preserving legacy returns.
- [x] PUR-080 add receipt-allocated Vendor Bills with normalized duplicate protection and GRNI/Input Tax/PPV/Accounts Payable posting.
- [x] PUR-090 add three-way matching tolerances, persisted exceptions and controlled override.
- [x] PUR-100 add partial supplier payments and Accounts Payable settlement.
- [x] PUR-110 convert reviewed purchase-sourced replenishment into a traceable, duplicate-safe Draft Purchase Request.
- [x] PUR-120 add awarded-quotation Purchase Agreements with capped Draft order releases and repeat templates that generate traced Draft Purchase Requests.
- [x] PUR-130 allocate receipt-linked landed costs with auditable AVCO valuation, consumed-cost expense and balanced clearing journals.
- [x] PUR-140 add bounded purchasing dashboards and supplier-performance projections over authoritative tenant-owned operational and financial records.
- [x] PUR-150 add idempotent barcode-assisted Draft Goods Receipt capture with explicit posting through the existing inventory/accounting path.
- [x] Complete the approved PUR-000 through PUR-150 purchase-management evolution plan.
- [x] PUR-160 add inventory-to-GL and GRNI reconciliation and close the remaining PUR-065 reporting gap.
- [x] PUR-165 add derived Purchase Order closure, controlled remaining-quantity closure and permission-gated reopen.
- [x] PUR-170 add Supplier Credit Notes for already billed receipt returns.
- [x] PUR-180 add controlled reversal for posted Goods Receipts, Vendor Bills, Supplier Payments and Landed Costs.
- [x] PUR-190 add multi-currency purchase, receipt, bill and payment accounting.
- [x] PUR-200 add a policy-controlled direct RFQ workspace and fast path.
- [x] PUR-210 add line-level and split supplier awards.
- [x] PUR-220 add service procurement, per-product billing policy, stock-neutral service completion and reviewed mixed-source Draft Vendor Bills with policy-aware GRNI/Expense posting.
- [x] PUR-230 complete bulk vendor price management with controlled CSV/native Excel templates, 500-row preview, atomic import, quantity price breaks/discounts, immutable full-snapshot history and expired/duplicate exception reporting.
- [x] PUR-240 add scheduled tenant-safe replenishment, historical-demand/seasonality suggestions, grouped Draft Purchase Requests or policy-bounded direct RFQs, idempotency and traceability.
- [x] PUR-250 add configurable receiving/quality routes, demand allocation and dropshipping contracts.
  - [x] PUR-250A configure one-step or receive/inspect/store routes, quality hold, accepted/rejected/damaged/short evidence and delayed stock/accounting posting.
  - [x] PUR-250B allocate incoming demand and define the dropshipping contract without internal stock.
- [x] PUR-260 add supplier email, attachments, confirmation tracking and reviewed OCR capture.
- [x] PUR-270 add a supplier portal plus secure API/EDI/UBL exchange and reliable outbox delivery.
- [x] PUR-280 add advanced purchase analytics and make a documented Legacy Direct Purchase retirement decision.
- [x] Add the bilingual Settings work procedure catalog with the complete purchasing sequence, role ownership, review responsibility, control principles, completion evidence and permission-aware links.
- [x] Extend the work procedure catalog into a complete 62-step ERP guide covering setup/administration, customers, sales/receivables, purchasing/payables, inventory/warehouses, accounting/close and analytics/management review. Completed 2026-10-23 with permission-aware real-action links and full Arabic resource coverage.
- [x] SAL-000 audit the actual Sales, Inventory, Invoicing, Accounting and Payment foundations; document the legacy immediate-sale compatibility boundary and an additive phased implementation plan.
- [x] SAL-005 approve the Sales context-ownership, invoicing-policy, COGS-timing, currency, reservation and POS boundaries before runtime implementation.
- [x] CUS-000 audit the Customer and Business Partner requirements against the actual repository and document compatibility, migration safeguards, ownership, tests and a phased implementation plan.
- [x] CUS-005 approve the Business Partner compatibility decision, customer-code policy, receivable-account model and archive rules before runtime implementation.
- [x] SAL-010 extend the Customer foundation additively while preserving current Customer IDs and Legacy Immediate Sale compatibility (delivered through CUS-010..030B).
- [x] SAL-020A approve deterministic price-rule priority, unit conversion, dated currency, rounding, tax/exemption snapshots and explainable result contract.
- [x] SAL-020B implement tenant-safe price-list/rule storage, customer default selection and read-only explainable evaluation without changing Legacy Sale/POS.
- [x] SAL-030 implement independent tenant-safe sales quotations with immutable commercial snapshots and no stock/accounting effect.
- [x] SAL-040 implement one-time accepted-quotation conversion to an independent sales order with no stock/accounting effect.
- [x] SAL-050 implement full/partial sales-order inventory reservation, release/reallocation, tenant-safe source-line audit and concurrent last-unit protection without changing OnHand or accounting.
- [x] SAL-060 implement partial/full reservation-backed delivery, exact-position stock/tracking issue, physical movement and one-time delivery COGS posting without creating an invoice or payment.
- [x] SAL-070 implement policy-based partial/multiple customer invoice documents with controlled source quantities, Draft/Approved/Cancelled lifecycle, exact delivery allocations and no premature accounting/payment effect. Completed 2026-10-20.
- [x] SAL-080 post Approved customer invoices to receivables/revenue/output tax with idempotent Posted/Reversed lifecycle, branch/warehouse dimensions, frozen journal evidence and no payment/inventory effect. Completed 2026-10-21.
- [x] SAL-090 add independent customer payments with capped partial/multi-invoice allocation, derived balances, foreign-exchange posting, journal evidence and controlled reversal. Completed 2026-10-21.
- [x] SAL-100A add delivery-backed customer return request Draft/Submit/Approve/Reject/Cancel control with cumulative quantity protection and no stock/accounting effect. Completed 2026-10-21.
- [x] SAL-100B receive and inspect Approved customer returns, recording explicit Restock/Quarantine/Reject/Destroy disposition and exact tracked identities without stock/accounting/credit-note effect. Completed 2026-10-21.
- [x] SAL-100C post Inspected return dispositions to inventory/tracking and reverse COGS where appropriate, while keeping credit notes separate. Completed 2026-10-21; migration applied to Demo, with the disposable SQL scenario still pending.
- [x] SAL-100D add independent retry-safe customer credit notes allocated to posted invoice/return quantities, reversing receivable/revenue/output tax and reducing balances without automatic cash refund or duplicate inventory mutation. Completed 2026-10-22; migrations applied to Demo, with the disposable SQL scenario still pending.
- [x] SAL-110 add configurable company/customer sales-credit, overdue, price-below-cost, exceptional-discount and margin controls with Warn/RequireApproval/Block actions, arbitrary role steps, frozen evidence and configurable self/distinct-approver safeguards. Completed 2026-10-22; migration applied to Demo.
- [x] SAL-120A add advanced versioned quotation templates with searchable categories/tags, reusable content, required/optional/alternative products, bounded option groups, quantity policy, optional extra lines, customer/date repricing and frozen applied-template evidence. Completed 2026-10-22; migration applied to Demo.
- [x] SAL-120B central organization identity, highly configurable versioned Document Designs, reusable central HTML-to-PDF engine, bilingual RTL/LTR quotation PDF and hash-protected identity/design/content/logo snapshot frozen at Send. Completed 2026-10-22 with migrations 133-134, Demo application and visual PDF verification. Customer portal, signature and payment remain later provider-dependent gates.
- [x] SAL-130A reusable provider-based Analytics Center with tenant-owned personal/shared workspaces, advanced widget settings, central previous-period/year comparison, and Sales/Purchasing providers. Completed 2026-10-22 with migration 135 applied to Demo and 1049 passing regression checks.
- [x] UI-AUDIT-001 verify every navigation, dashboard and static Razor link/form target, enforce antiforgery on every public MVC POST action, complete shared Arabic coverage, and protect customer create/edit binding plus visible validation feedback. Audit completed 2026-10-23; customer save, optional generated/policy fields and culture-safe decimal validation were verified 2026-10-24 with 1066 passing checks.
- [ ] SAL-130B add safe workspace edit/clone/archive, presets, central export and optional scheduled distribution after explicit approval.

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
- [x] WMS-080C3 add policy-driven untracked exact-location removal.
- [x] WMS-090A add reviewed min/max replenishment rules and suggestions.
- [x] WMS-090B1 add Approved-transfer incoming/outgoing forecasting.
- [x] WMS-090B2 add reviewed replenishment conversion into a Draft stock transfer.
- [x] WMS-090D add read-only slow/dead inventory insights and no-movement-history visibility.
- [ ] WMS-090C add purchase-order intent after a purchase-order lifecycle exists.
- [x] WMS-100A add safe read-only product/location barcode resolution and availability lookup.
- [x] WMS-100B1 add idempotent scanned putaway through the controlled Unassigned Stock workflow.
- [x] WMS-100B2 add idempotent scanned relocation between exact locations.
- [x] WMS-100B3 add retry-safe absolute scanned counts to Draft inventory adjustments.
- [ ] WMS-100B4 add receiving scans after a real purchase-order/receipt workflow exists.
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
- PUR-280 is implemented. The purchase completion roadmap is complete; continue from the master roadmap after reconciling customer-management and sales requirements.
- WMS delivery is paused by product-owner direction. Its remaining items stay documented in `WMS_EVOLUTION_PLAN.md` and resume after the current purchase-management priority.
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
- [x] Disabled the historically exposed local Demo administrator, invalidated existing Demo sessions and verified a replacement administrator; reopen this action if an external deployment is discovered.



## Bugs / Technical Debt
- Missing database unique constraints for barcode and master-data names.
- Duplicate `PermissionsCodeExport` tree can drift.

## Accounting Gaps
- Sales/purchase posting, immutable posted returns, fiscal-period enforcement and controlled reversal for four purchase document types are implemented; provisional-negative variance settles during purchase posting. Mixed-rate product tax, reversal coverage outside the current purchase scope and financial statements remain pending.

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
