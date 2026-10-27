# Phase 22 — Inventory and WMS Angular migration

> Status: IN PROGRESS — `MOD-022A..B` COMPLETE on 2026-10-27
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
- Security/inventory regression: 1210 checks passed.
- Angular: 155/155 tests across 54 files passed.
- Static accessibility: 62 templates passed; external-SCSS policy passed.
- Production bundles: 302,983/358,400 initial bytes and 81,017/81,920 largest lazy bytes.
- Live checks: `/inventory/stocks` returned 200; anonymous ProductStock and manual-adjustment API requests returned 401.

## Next slice

`MOD-022C` migrates dimensional inventory availability and reservation history as read-only Angular workflows while preserving
all projection, reservation, tenant, permission and concurrency authority on the server.
