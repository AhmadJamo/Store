# Phase 22 — Inventory and WMS Angular migration

> Status: IN PROGRESS — `MOD-022A` COMPLETE on 2026-10-27
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

## Next slice

`MOD-022B` migrates current product/warehouse stock balances and authorized manual adjustment entry while keeping balance,
location-removal and moving-average valuation rules in the Application layer.
