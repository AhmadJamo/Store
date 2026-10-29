# Phase 25 — Legacy immediate sales and POS Angular migration

> Status: IN PROGRESS — `MOD-025A` COMPLETE on 2026-10-29
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

## Evidence

- Web Release: zero warnings/errors; SecurityRegression: 1302 checks.
- Angular: 199/199 tests across 95 files; 107 templates passed accessibility; external SCSS passed.
- Bundles: 317,629/358,400 initial bytes and 81,743/81,920 largest lazy bytes.

## Next slice

`MOD-025B` migrates the retail POS composer and its terminal-specific warehouse, order-context, layout and tracked-selection behavior.
