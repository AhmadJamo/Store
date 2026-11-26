# 16 — Inventory migration audit

> Status: FRD-I implementation in progress — Scanning result evidence complete; operation workspaces next
> Date: 2026-11-26
> Source of truth: Angular routes/components and clients, Internal API controllers, Application services/DTOs, repositories and inventory module documentation

## Scope inventory

The Inventory application currently exposes 19 Angular routes through 15 page templates. Warehouse Locations is already migrated by FRD-H and remains in this inventory only as a protected dependency/reference surface.

| Surface | Route(s) | Authority and permissions | Presentation state / risk |
|---|---|---|---|
| Stock Transactions | `/inventory/transactions` | GET-only immutable ledger; `StockTransactions.View`; server search/type/direction/date/sort/paging | V4 localized FilterBar/SummaryStrip/typed sortable DataTable; five focused tests |
| Product Stock | `/inventory/stocks`, `/new`, `/adjustment`, `/:id/edit` | ProductStock View/Create/Edit/Delete plus `StockTransactions.Create`; server owns negative stock, valuation, tracking and exact-location removal | Workspace V4: localized SummaryStrip/typed table plus structured opening/edit and manual-adjustment forms; all authority preserved |
| Inventory Availability | `/inventory/availability` | GET-only `InventoryBalances.View`; `InventoryBalanceService` owns product search, named warehouse filter, paging and OnHand/Reserved/Available | V4 localized FilterBar/SummaryStrip/typed DataTable; five focused tests |
| Reservations | `/inventory/reservations` | GET-only `InventoryReservations.View`; lifecycle facts are server-authored | V4 localized FilterBar/SummaryStrip/typed DataTable; five focused tests |
| Stock Transfers | `/inventory/transfers`, `/new`, `/:id/edit`, `/:id` | View/Create/Edit/Submit/Approve/Reject/Dispatch/Receive/Cancel; reservations, locations, tracking, valuation and posting remain server-owned | Workspace V4: six-test matrix, localized form, typed record evidence and structured permission-gated receipt inputs |
| Inventory Scanning | `/inventory/scanning` | `InventoryScanning.View` plus ProductStock.Edit, LocationMovements.Create or InventoryAdjustments.Count per command; server idempotency | Touch/keyboard command workspace; no component spec; high operational risk |
| Replenishment | `/inventory/replenishment` | View/Manage plus StockTransfers.Create or PurchaseRequests.Create; server creates draft documents | Cross-module mutations and dirty settings; two focused tests |
| Insights | `/inventory/insights` | GET-only `InventoryInsights.View`; classifications and KPIs are server-authored | V4 localized FilterBar/SummaryStrip/typed DataTable; five focused tests |
| Reconciliation | `/inventory/reconciliation` | GET-only `InventoryReconciliation.View`; exceptions/tolerances/paging are server-authored | V4 localized FilterBar/SummaryStrip/typed DataTable/Pager; five focused tests |
| Adjustment List | `/inventory/adjustments` | GET `InventoryAdjustments.View`; service returns the complete newest-first directory without filter/paging inputs | V4 localized header/three-item SummaryStrip/typed eight-column DataTable; five focused tests |
| Adjustment Create | `/inventory/adjustments/new` | Create form/POST under `InventoryAdjustments.Create`; named warehouse/location/product choices | V4 localized three-section form, FieldCues and WorkflowFooter; five focused tests |
| Adjustment Record | `/inventory/adjustments/:id` | View/Count/Approve/Post/Cancel split; frozen expected quantity and separation-of-duties remain server-owned | V4 WorkflowProgress/DetailSection/FormSection/typed results/WorkflowFooter; five-state focused matrix |
| Tracking | `/inventory/tracking` | View/Open/ManageQuarantine/ManageRecall; server owns lot/serial/expiration eligibility | Large multi-command workspace; two focused tests |
| Warehouse Locations | `/inventory/locations` | Warehouses View/Create/Edit; hierarchy/cycle/capability validation remains server-owned | Migrated by FRD-H; four focused tests; do not restyle again in FRD-I |
| Putaway / Unassigned Stock | `/inventory/putaway` | ProductStock.View/Edit; server suggestions, capacity, eligibility and idempotency | Mixed assignment/rule workflow; no component spec |
| Location Movements | `/inventory/location-movements` | LocationMovements View/Create and independent StockMovements.View evidence | Relocation plus two ledgers, idempotency and capacity rules; no component spec |

## Cross-layer contracts that must not change

- Angular never calculates trusted OnHand, Reserved, Available, valuation, capacity, reconciliation, tracking eligibility or workflow transitions.
- All stock mutations continue through the Application layer, existing UnitOfWork and repositories; controllers never gain direct EF access.
- Product and warehouse selectors remain name-first; barcode/name search is preserved and numeric IDs remain internal transport values.
- Branch and warehouse scope remain server-authoritative. The current branch header is a context hint, never authorization.
- ProductStock, InventoryBalance, exact-location balances, reservations, StockTransaction, StockMovement and tracking ledgers retain their existing ownership and reconciliation boundaries.
- Stock transaction evidence remains immutable. Manual creation remains limited to the already-authorized adjustment types and reason requirements.
- Transfer, adjustment, scanning, tracking, replenishment, putaway and relocation commands retain independent permissions, antiforgery, idempotency/concurrency and current-state checks.
- No API, DTO, schema, migration, accounting entry, inventory posting rule or permission expansion is authorized by FRD-I presentation work.
- Every changed label/state/message must remain bilingual and every layout must work in RTL/LTR using logical CSS.

## Test baseline and gaps

- Current FRD-I baseline: 343 Angular tests across 140 files, production build, 142 templates, 122-page consistency and Playwright 7/7. The last security baseline remains 1,668 checks because these slices change presentation only; the independent 84.55 kB core-localization lazy-budget issue remains tracked as FRD-ISS-012.
- Existing Inventory page component coverage: Availability 5, Reservations 5, Stock Transactions 5, Insights 5, Reconciliation 5, Adjustment List 5, Adjustment Create 5, Adjustment Record 5, Product Stock 7, Stock Transfer 6, Scanning 5, Tracking 2, Replenishment 2, Warehouse Locations 4.
- No page component spec currently protects Putaway or Location Movements.
- API client specs already protect the principal HTTP methods and query parameters; they do not replace page-state and permission-aware action coverage.
- Live authenticated API reconciliation remains external until an API URL and authenticated storage state are supplied.

## Migration order

1. **Inventory Availability COMPLETE.** Localized Inventory context, FilterBar, SummaryStrip and typed DataTable now distinguish true-empty/filtered-empty while preserving named server rows, debounced search, warehouse/paging query, bounded reset and all server-authored calculations.
2. **Inventory Reservations COMPLETE.** Localized Inventory context, FilterBar, SummaryStrip and a typed seven-column DataTable distinguish true-empty/filtered-empty while preserving server-authored reference/source/status/quantity/lifecycle evidence and search/status/paging/reset.
3. **Stock Transactions COMPLETE.** Localized Inventory context, FilterBar, SummaryStrip and a typed sortable ten-column DataTable preserve immutable identity/name/quantity/cost/type/reference/date evidence, all server filter/sort/page contracts and distinct true-empty/filtered-empty behavior.
4. **Inventory Insights + Reconciliation COMPLETE.** Insights preserves all server-authored KPIs, named rows, quantities/values/activity classifications and threshold validation through a localized FilterBar, five-item SummaryStrip and typed seven-column DataTable. Reconciliation now uses the same Inventory hierarchy with a FilterBar, two-item SummaryStrip, typed nine-column DataTable, shared Pager and distinct true-empty/filtered-empty guidance while preserving counters, named evidence, all seven server issue mappings, consistency, server filters/paging and the independent permission-aware Insights link.
5. **Adjustment List + Create + Record COMPLETE.** Record now uses localized WorkflowProgress, DetailSection summary, FormSection count entry, typed result DataTable and WorkflowFooter actions. Five tests protect server evidence, all five states, blind expected-quantity hiding, independent permissions, completed evidence, exact commands, validation and dirty guard. Frozen count, stale-stock checks and separation of duties remain server-owned. **Product Stock safety audit is next.**
6. **Product Stock workspace COMPLETE; Stock Transfers NEXT.** The localized list preserves named rows, quantity/AVCO/value, response order, empty state and Edit/Delete permissions. Both operation forms now use localized FormSections, required FieldCues and WorkflowFooters while preserving locked edit identity, separate create/update commands, manual types 6/7/8, positive quantity, required reason, validation/focus, dirty guard and independent permissions. API/Application valuation and location authority remain unchanged.
7. **Stock Transfer Workspace COMPLETE; operational command workspaces NEXT.** Form, record evidence and receipt now use the shared hierarchy. Receipt states 7/8/9 expose described received/short/damaged cues and a dedicated footer while preserving remaining limits, destination, permission and exact receive payload. Six tests protect all ten states and commands; reservation, branch, tracking, valuation and posting authority remain server-owned.
8. **Scanning resolve and result evidence COMPLETE; operation workspaces NEXT.** Five focused tests protect four resolver states, server identities/balances, independent Putaway/Relocate/Count permissions, exact commands and idempotency keys, quantity validation, blind count and server errors. The resolver now uses localized Inventory context, a described FormSection, four optional FieldCues and WorkflowFooter while preserving autofocus, four query values, GET normalization and recovery. The four server-resolved identities now sit inside a localized DetailSection while preserving statuses 0/1/2/3, display names and secondary evidence. Migrate only the conditional Putaway/Relocate/Count workspaces next.
9. Warehouse Locations remains complete and is excluded from further FRD-I restyling unless a verified dependency regression requires a fix.

## First slice decision

Inventory Availability was selected first because it is GET-only, independently permissioned, has an existing typed API client and cannot post or reserve stock. Its safety net and V4 migration established the bounded pattern now reused by Reservations and Stock Transactions; all three retain their server and permission authorities.

## Exit criteria

- Every in-scope page has a recorded route, action, permission, API/Application authority and rollback path.
- Read-only screens preserve server query/paging and distinguish true empty, filtered empty, loading and recovery.
- Workflow screens preserve every state-dependent action, dirty guard, confirmation, permission, antiforgery, idempotency and conflict path.
- EN/AR, RTL/LTR, desktop/390px, keyboard, theme/density and static accessibility pass.
- Angular, production build, UI consistency, Playwright and SecurityRegression pass at wave closure.
- Authenticated live-API skips remain explicit; presentation acceptance never claims stock-posting E2E without domain evidence.
