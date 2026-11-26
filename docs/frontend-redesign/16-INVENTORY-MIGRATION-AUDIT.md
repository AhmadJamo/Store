# 16 — Inventory migration audit

> Status: FRD-I implementation in progress — Inventory Availability complete; Reservations safety net next
> Date: 2026-11-25  
> Source of truth: Angular routes/components and clients, Internal API controllers, Application services/DTOs, repositories and inventory module documentation

## Scope inventory

The Inventory application currently exposes 19 Angular routes through 15 page templates. Warehouse Locations is already migrated by FRD-H and remains in this inventory only as a protected dependency/reference surface.

| Surface | Route(s) | Authority and permissions | Presentation state / risk |
|---|---|---|---|
| Stock Transactions | `/inventory/transactions` | GET-only immutable ledger; `StockTransactions.View`; server search/type/direction/date/sort/paging | Legacy filter hierarchy and hard-coded eyebrow; no component spec |
| Product Stock | `/inventory/stocks`, `/new`, `/adjustment`, `/:id/edit` | ProductStock View/Create/Edit/Delete plus `StockTransactions.Create`; server owns negative stock, valuation, tracking and exact-location removal | Mixed list/create/edit/manual-adjustment page; high mutation and dirty-state risk |
| Inventory Availability | `/inventory/availability` | GET-only `InventoryBalances.View`; `InventoryBalanceService` owns product search, named warehouse filter, paging and OnHand/Reserved/Available | Best first slice: bounded read-only list, one focused spec, legacy filters/projected table |
| Reservations | `/inventory/reservations` | GET-only `InventoryReservations.View`; lifecycle facts are server-authored | Read-only list with one focused spec; follows Availability |
| Stock Transfers | `/inventory/transfers`, `/new`, `/:id/edit`, `/:id` | View/Create/Edit/Submit/Approve/Reject/Dispatch/Receive/Cancel; reservations, locations, tracking, valuation and posting remain server-owned | Complex workflow; already uses many shared V4 primitives; migrate only after dedicated state/action evidence |
| Inventory Scanning | `/inventory/scanning` | `InventoryScanning.View` plus ProductStock.Edit, LocationMovements.Create or InventoryAdjustments.Count per command; server idempotency | Touch/keyboard command workspace; no component spec; high operational risk |
| Replenishment | `/inventory/replenishment` | View/Manage plus StockTransfers.Create or PurchaseRequests.Create; server creates draft documents | Cross-module mutations and dirty settings; two focused tests |
| Insights | `/inventory/insights` | GET-only `InventoryInsights.View`; classifications and KPIs are server-authored | Shares one large analysis component with Reconciliation; no component spec |
| Reconciliation | `/inventory/reconciliation` | GET-only `InventoryReconciliation.View`; exceptions/tolerances/paging are server-authored | Shared analysis component; no component spec |
| Adjustment List | `/inventory/adjustments` | GET `InventoryAdjustments.View` | Read-only directory but no component spec; depends on workflow terminology |
| Adjustment Create | `/inventory/adjustments/new` | Create form/POST under `InventoryAdjustments.Create`; named warehouse/location/product choices | Dirty form and blind-count semantics; no component spec |
| Adjustment Record | `/inventory/adjustments/:id` | View/Count/Approve/Post/Cancel split; frozen expected quantity and separation-of-duties remain server-owned | High-risk workflow; one command-focused spec only |
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

- Current baseline after FRD-H: 291 Angular tests across 133 files, production build, 142 templates, 122-page consistency, Playwright 7/7 and 1,668 SecurityRegression checks.
- Existing Inventory page component coverage: Availability 1, Reservations 1, Product Stock 3, Adjustment Record 1, Tracking 2, Replenishment 2, Warehouse Locations 4.
- No page component spec currently protects Stock Transactions, Adjustments List/Create, Insights/Reconciliation, Scanning, Putaway or Location Movements.
- API client specs already protect the principal HTTP methods and query parameters; they do not replace page-state and permission-aware action coverage.
- Live authenticated API reconciliation remains external until an API URL and authenticated storage state are supplied.

## Migration order

1. **Inventory Availability COMPLETE.** Localized Inventory context, FilterBar, SummaryStrip and typed DataTable now distinguish true-empty/filtered-empty while preserving named server rows, debounced search, warehouse/paging query, bounded reset and all server-authored calculations.
2. **Reservations and Stock Transactions read-only directories — NEXT.** Expand the existing Reservations spec before presentation changes, then establish missing Stock Transactions component coverage; preserve server paging, immutable evidence and independent permissions.
3. **Insights and Reconciliation read-only analysis pair.** Split their shared presentation concerns only if it reduces risk; keep classifications and tolerances server-authored.
4. **Adjustment List/Create/Record.** Add list/form/state-action matrices before any visual change; keep frozen count and separation-of-duties rules unchanged.
5. **Product Stock.** Treat list, opening stock/edit and manual adjustment as separate presentation slices despite the current shared component.
6. **Stock Transfers.** Audit the already-adopted shared primitives against every workflow state and receipt exception before additional restyling.
7. **Operational command workspaces.** Scanning, Tracking, Replenishment, Putaway and Location Movements require dedicated command/idempotency/permission evidence and browser scenarios before migration.
8. Warehouse Locations remains complete and is excluded from further FRD-I restyling unless a verified dependency regression requires a fix.

## First slice decision

Inventory Availability is first because it is GET-only, independently permissioned, has an existing typed API client and focused component test, and cannot post or reserve stock. Its current page already displays server-authored quantities and named warehouses but still uses a hand-built filter card, projected table markup, hard-coded `INVENTORY` eyebrow and one generic empty state. The five-test safety net is committed before the visual migration.

## Exit criteria

- Every in-scope page has a recorded route, action, permission, API/Application authority and rollback path.
- Read-only screens preserve server query/paging and distinguish true empty, filtered empty, loading and recovery.
- Workflow screens preserve every state-dependent action, dirty guard, confirmation, permission, antiforgery, idempotency and conflict path.
- EN/AR, RTL/LTR, desktop/390px, keyboard, theme/density and static accessibility pass.
- Angular, production build, UI consistency, Playwright and SecurityRegression pass at wave closure.
- Authenticated live-API skips remain explicit; presentation acceptance never claims stock-posting E2E without domain evidence.
