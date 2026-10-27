# Phase 22 — Inventory and WMS Angular migration

> Status: IN PROGRESS — `MOD-022A..E` COMPLETE on 2026-10-27
>
> Presentation default: MVC until a separate cutover approval.

## Scope

Move the existing inventory and WMS workflows to Angular in bounded slices while preserving ProductStock, StockTransaction,
InventoryBalance, reservation, tracking, valuation and accounting authority on the server. Angular consumes explicit DTOs and
never recomputes authoritative quantity, cost or value.

## `MOD-022A` delivered — inventory ledger

- Added authenticated `GET /api/v1/stock-transactions` protected by `StockTransactions.View`.
- The API delegates paging, search, type/direction/date filters and allowlisted sorting to `StockTransactionService`; controllers do
  not access Entity Framework and the historical ledger exposes no mutation endpoint.
- Added permission-aware Angular route `/inventory/transactions` with debounced search, all 18 current transaction types,
  incoming/outgoing and date filters, page sizes, sortable columns, paging and server-authored unit-cost/value/variance display.
- Added Arabic/English labels, RTL-safe logical layout, external SCSS, responsive filters and accessible sortable headings.
- No database, valuation, balance, tracking, posting or accounting behavior changed.

## Evidence

- Web Release build: zero warnings and zero errors.
- Security/inventory regression: 1206 checks passed.
- Angular: 151/151 tests across 52 files passed.
- Static accessibility: 61 templates passed; external-SCSS policy passed.
- Production bundles: 302,834/358,400 initial bytes and 80,941/81,920 largest lazy bytes.
- Live checks: Angular route returned 200; anonymous API request returned 401.

## `MOD-022B` delivered — product stock and manual adjustments

- Added authenticated, versioned ProductStock list/form/create/update/delete contracts. Every operation delegates to
  `ProductStockService`; the existing server-owned delete refusal remains visible to Angular instead of inventing a second rule.
- Added the manual adjustment form/command contract over `StockTransactionService` for Adjustment In, Adjustment Out and
  Kitchen Variance, with a required reason and server-owned balance, location-removal, tracking and AVCO behavior.
- Added permission-aware `/inventory/stocks` Angular operations for balance review, opening stock, current-quantity correction,
  delete attempt feedback and manual adjustment. Product and warehouse names are user-facing; identifiers stay internal.
- Added invariant quantity validation, explicit form option DTOs, CSRF protection, Problem Details recovery, dirty-form protection,
  Arabic/English localization, RTL-safe responsive layout and permission-filtered navigation.
- No database, accounting, valuation, tracking or inventory-domain rule changed; MVC remains the default presentation.

## Current evidence

- Web Release build: zero warnings and zero errors.
- Security/inventory regression: 1216 checks passed.
- Angular: 161/161 tests across 59 files passed.
- Static accessibility: 67 templates passed; external-SCSS policy passed.
- Production bundles: 303,702/358,400 initial bytes and 81,296/81,920 largest lazy bytes.
- Live checks: adjustment list/create Angular routes returned 200; anonymous adjustment APIs returned 401.

## `MOD-022C` delivered — availability and reservation history

- Added authenticated, versioned, GET-only availability and reservation endpoints protected independently by
  `InventoryBalances.View` and `InventoryReservations.View`.
- Paging remains behind `InventoryBalanceService` and `InventoryReservationService`; controllers do not access repositories or EF.
- Added permission-aware `/inventory/availability` with product search, named warehouse filtering, paging and server-authored
  On Hand, Reserved and Available quantities by named location, including the protected Unassigned position.
- Added permission-aware `/inventory/reservations` with reference search, Active/Consumed/Released filtering, paging, named source
  types, line counts, remaining total quantity and lifecycle timestamps.
- Both responsive screens are Arabic/English, RTL-safe and read-only. No schema, balance, reservation, tracking, valuation or
  accounting behavior changed; MVC remains the default presentation.

## `MOD-022D` delivered — inventory adjustments and cycle counting

- Added an authenticated versioned API for adjustment list, named create options, create, details, count, approve, post and cancel.
- Each mutation keeps its independent `InventoryAdjustments.*` permission and CSRF protection and delegates to
  `InventoryAdjustmentService`; the controller does not access repositories or EF.
- Added permission-aware Angular list, create and details routes with warehouse/location/product names, blind-count behavior,
  dynamic full-line count entry, variance evidence, completed-action states and separate approve/post/cancel controls.
- Rejected commands retain correctable input and render normalized Problem Details. Dirty count/create forms are protected.
- Separation of duties, frozen expected quantity, reservations, location/tracking updates, AVCO, stock transactions/movements and
  atomic posting remain server-owned. No schema or domain/accounting rule changed; MVC remains default.

## `MOD-022E` delivered — inventory tracking and quality controls

- Added authenticated `GET` plus CSRF-protected opening allocation, quarantine/release, recall create/close and recall-communication
  endpoints under `/api/v1/inventory-tracking`; each operation preserves its independent `InventoryTracking.*` permission.
- The API maps tuple-backed service choices to explicit named DTOs and delegates every command to `InventoryTrackingService`; no
  controller accesses EF or repositories and no tracking, expiration, dimensional balance or recall rule moved into Angular.
- Added permission-aware `/inventory/tracking` with dynamic opening allocations, live expiration alerts, current balances and trace
  history, reasoned quarantine/release, recall impact evidence, manual communication logging and explicit closure guidance.
- Product, warehouse and location names are user-facing while identifiers remain transport values. Rejected and dirty opening/recall
  forms remain recoverable. Arabic/English, RTL, responsive external SCSS and accessibility contracts are included.
- Recall closure deliberately does not release quarantined inventory. MVC remains the default presentation and no schema changed.

## Current evidence

- Web Release build: zero warnings and zero errors.
- Security/inventory regression: 1219 checks passed.
- Angular: 164/164 tests across 61 files passed.
- Static accessibility: 68 templates passed; external-SCSS policy passed.
- Production bundles: 303,857/358,400 initial bytes and 81,401/81,920 largest lazy bytes.
- Live checks: Angular tracking route returned 200; anonymous tracking read/write requests returned 401.

## Next slice

`MOD-022F` migrates warehouse locations, Unassigned Stock putaway and exact-location relocation/history operations while preserving
capacity, receive/pick capability, idempotency, dimensional balance and physical-movement authority on the server.
