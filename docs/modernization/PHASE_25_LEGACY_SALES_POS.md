# Phase 25 — Legacy immediate sales and POS Angular migration

> Status: COMPLETE — `MOD-025A..C` COMPLETE on 2026-10-29
>
> Presentation default: MVC until a separate cutover approval.

## Scope

Move the existing immediate-sale, POS, return, printing and shift/closure surfaces to Angular without reinterpreting `Sale` as a
professional Sales Order or receivable invoice. `SaleService`, inventory/tracking services and `SalePostingService` retain pricing,
discount, tax, stock, valuation, numbering, transaction and accounting authority.

## `MOD-025A` delivered — wholesale immediate sale

- Added an Application-owned workspace service and authenticated list/details/create-form/create/post APIs. The API never accesses
  persistence directly; View/Create permissions and antiforgery remain authoritative.
- Added a bilingual RTL-safe Angular workspace for immediate-sale history, immutable details, named warehouse/customer/payment/tax
  options, product name/code/barcode search, wholesale composition, optional tracked-identity evidence and one-time GL posting.
- The client shows the server-projected wholesale price but `SaleService` resolves it again from the product, validates discounts,
  consumes stock/tracking, freezes cost and numbering, and commits atomically. `SalePostingService` retains balanced, idempotent GL
  posting and source evidence.

## `MOD-025B` delivered — retail POS composer

- Extended the Application-owned workspace with active terminals, terminal warehouse allow-lists, runtime experience settings,
  per-warehouse stock evidence and an explicit RetailPos create command.
- Added a bilingual RTL-safe POS route with touch-friendly product cards, configured columns/density, price/barcode/stock visibility,
  name/code/barcode search, quick barcode add, cart quantities/discounts/tracking and terminal-enabled order context.
- `SaleService` revalidates terminal/warehouse/product access, order-type requirements, recipes, tracking, server retail price,
  inventory and cost inside the existing transaction. Browser settings grant no authority.

## `MOD-025C` delivered — immediate-sale returns

- Added an Application-authored create projection and authenticated list/details/create-form/create-and-post API with independent
  View/Create permissions and antiforgery.
- Added a bilingual RTL-safe Angular return workspace with original-sale continuation, sold/returned/remaining quantities, tracked
  identity capture and immutable refund/tax/restock/cost evidence.
- `SalesReturnService` remains authoritative for cumulative quantities, original issued identities, proportional commercial values,
  inventory restoration, historical cost and balanced posting in one transaction.

## Evidence

- Web Release: zero warnings/errors; SecurityRegression: 1302 checks.
- Angular: 201/201 tests across 96 files; 108 templates passed accessibility; external SCSS passed.
- Bundles: 318,180/358,400 initial bytes and 81,743/81,920 largest lazy bytes.

## Scope closure

The repository has no independent POS shift-close or print controller/view/action to migrate. No synthetic workflow was introduced.
`MOD-026A` starts accounting presentation migration with the current chart-of-accounts list/create surface.
