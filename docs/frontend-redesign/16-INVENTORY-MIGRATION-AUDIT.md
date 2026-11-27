# 16 — Inventory migration audit

> Status: FRD-I implementation in progress — Putaway Workspace V4 complete; Location Movements safety audit next
> Date: 2026-11-27
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
| Inventory Scanning | `/inventory/scanning` | `InventoryScanning.View` plus ProductStock.Edit, LocationMovements.Create or InventoryAdjustments.Count per command; server idempotency | Workspace V4: six-test matrix, structured resolver/evidence and permission-gated command forms |
| Replenishment | `/inventory/replenishment` | View/Manage plus StockTransfers.Create or PurchaseRequests.Create; server creates draft documents | Workspace V4: five-test matrix, structured rule/filter controls and typed 11-column suggestions/actions |
| Insights | `/inventory/insights` | GET-only `InventoryInsights.View`; classifications and KPIs are server-authored | V4 localized FilterBar/SummaryStrip/typed DataTable; five focused tests |
| Reconciliation | `/inventory/reconciliation` | GET-only `InventoryReconciliation.View`; exceptions/tolerances/paging are server-authored | V4 localized FilterBar/SummaryStrip/typed DataTable/Pager; five focused tests |
| Adjustment List | `/inventory/adjustments` | GET `InventoryAdjustments.View`; service returns the complete newest-first directory without filter/paging inputs | V4 localized header/three-item SummaryStrip/typed eight-column DataTable; five focused tests |
| Adjustment Create | `/inventory/adjustments/new` | Create form/POST under `InventoryAdjustments.Create`; named warehouse/location/product choices | V4 localized three-section form, FieldCues and WorkflowFooter; five focused tests |
| Adjustment Record | `/inventory/adjustments/:id` | View/Count/Approve/Post/Cancel split; frozen expected quantity and separation-of-duties remain server-owned | V4 WorkflowProgress/DetailSection/FormSection/typed results/WorkflowFooter; five-state focused matrix |
| Tracking | `/inventory/tracking` | View/Open/ManageQuarantine/ManageRecall; server owns lot/serial/expiration eligibility | Workspace V4: five-test matrix, structured commands, typed expiration/balance/history evidence |
| Warehouse Locations | `/inventory/locations` | Warehouses View/Create/Edit; hierarchy/cycle/capability validation remains server-owned | Migrated by FRD-H; four focused tests; do not restyle again in FRD-I |
| Putaway / Unassigned Stock | `/inventory/putaway` | ProductStock.View/Edit; server suggestions, capacity, eligibility and idempotency | Safety-audited workspace; Workspace V4 complete: forms, filters and both typed tables |
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

- Current FRD-I baseline: 360 Angular tests across 141 files, production build, 142 templates, 122-page consistency and Playwright 7/7. The last security baseline remains 1,668 checks because these slices change presentation only; the independent 84.55 kB core-localization lazy-budget issue remains tracked as FRD-ISS-012.
- Existing Inventory page component coverage: Availability 5, Reservations 5, Stock Transactions 5, Insights 5, Reconciliation 5, Adjustment List 5, Adjustment Create 5, Adjustment Record 5, Product Stock 7, Stock Transfer 6, Scanning 6, Tracking 5, Replenishment 5, Putaway 5, Warehouse Locations 4.
- No page component spec currently protects Location Movements.
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
8. **Scanning Workspace COMPLETE.** Six focused tests protect four resolver states, server identities/balances, independent Putaway/Relocate/Count permissions, exact commands and idempotency keys, quantity validation, blind count and server errors. The resolver now uses localized Inventory context, a described FormSection, four optional FieldCues and WorkflowFooter while preserving autofocus, four query values, GET normalization and recovery. The four server-resolved identities now sit inside a localized DetailSection while preserving statuses 0/1/2/3, display names and secondary evidence. The conditional Putaway/Relocate/Count workspaces now use described FormSections, required quantity FieldCues and WorkflowFooters while preserving permission/state gates, validation, idempotency, commands and refresh. Tracking is audited separately in the next step.
9. **Tracking Workspace COMPLETE.** Five focused tests protect server balance/expiration/recall/communication/transaction evidence, lot/serial mapping, complete opening allocation, four independent permissions, recall/quarantine/release/communication/close commands, validation, server errors and dirty navigation. Opening allocation uses localized Inventory context, two described FormSections, nine required/optional FieldCues and a WorkflowFooter. Expiration metrics use a two-item SummaryStrip and alerts use a typed nine-column DataTable. Recall creation and active record actions use the shared FormSection/FieldCue/WorkflowFooter hierarchy, and recall evidence uses DetailSection. Tracked balances use a typed permission-aware DataTable with the exact quarantine/release boundary. Tracking history now uses a typed six-column DataTable for date, product, identifier, type, quantity and reference, preserving server order and a localized empty state. API/Application authority is unchanged.
10. **Replenishment Workspace V4 COMPLETE.** Five focused tests protect named server rows and balance/projection/suggestion/open-request evidence; filters; named options; validation and dirty guard; independent permissions; exact commands, navigation and errors. The rule editor uses localized Inventory context, FormSection, ten FieldCues and WorkflowFooter; filters use FilterBar and three FieldCues. Suggestions now use a typed 11-column DataTable preserving grouped available/reserved and projected evidence, source availability, server order, empty state, status, open-request link, toggle and conditional transfer/purchase actions. Runtime/API/Application authority is unchanged.
11. **Putaway Workspace V4 COMPLETE; Location Movements safety audit NEXT.** Five focused contract tests protect server-authored unassigned quantities and suggestion evidence, warehouse/query/sort requests, ProductStock.Edit gating, named options, exact assignment/rule/toggle commands, idempotency, validation, server errors and dirty navigation. The assignment form uses localized Inventory context, a described FormSection, four required FieldCues and WorkflowFooter. The rule form uses a described FormSection, four default cues plus a conditional named product/category cue and WorkflowFooter while preserving default/product/category targeting, priority and the exact command. Filters use FilterBar, three FieldCues and explicit apply/reset actions while preserving the supported product/barcode/warehouse sort contract. Unassigned Stock uses a typed eight-column DataTable preserving complete warehouse/assigned/unassigned quantity evidence, suggestion, server order, empty state and the permission-gated assign action. Rules use a typed six-column DataTable preserving named evidence, priority, explicit status, server order, empty state and the permission-gated toggle command. Runtime/API/Application authority is unchanged.
12. Warehouse Locations remains complete and is excluded from further FRD-I restyling unless a verified dependency regression requires a fix.

## First slice decision

Inventory Availability was selected first because it is GET-only, independently permissioned, has an existing typed API client and cannot post or reserve stock. Its safety net and V4 migration established the bounded pattern now reused by Reservations and Stock Transactions; all three retain their server and permission authorities.

## Exit criteria

- Every in-scope page has a recorded route, action, permission, API/Application authority and rollback path.
- Read-only screens preserve server query/paging and distinguish true empty, filtered empty, loading and recovery.
- Workflow screens preserve every state-dependent action, dirty guard, confirmation, permission, antiforgery, idempotency and conflict path.
- EN/AR, RTL/LTR, desktop/390px, keyboard, theme/density and static accessibility pass.
- Angular, production build, UI consistency, Playwright and SecurityRegression pass at wave closure.
- Authenticated live-API skips remain explicit; presentation acceptance never claims stock-posting E2E without domain evidence.
