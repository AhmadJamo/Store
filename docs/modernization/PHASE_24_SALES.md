# Phase 24 — Sales Angular migration

> Status: IN PROGRESS — `MOD-024A` COMPLETE on 2026-10-29
>
> Presentation default: MVC until a separate cutover approval.

## Scope

Move the existing professional quote-to-cash and receivables workflows to Angular in bounded slices. Pricing precedence, units,
currency, tax, numbering, approvals, reservations, inventory/accounting posting, concurrency and tenant authority remain in
Application/Domain and the server.

## `MOD-024A` delivered — sales price lists and pricing evaluation

- Added an authenticated API over `SalesPriceListService` for page data, price-list creation/lifecycle, rule creation/lifecycle and
  read-only price evaluation. View and Manage permissions remain independent; every POST retains antiforgery.
- Added a bilingual RTL-safe lazy Angular workspace for list validity/currency/rounding, deterministic scoped rules and server-authored
  evaluation evidence covering source, unit conversion, exchange rate, transaction/company price, net, tax and total.
- Rule precedence, target/unit/date/quantity eligibility, customer defaults/exemption, exchange-rate direction and rounding remain
  server-owned. Evaluation creates no document, stock movement, journal or payment.

## Evidence

- Web Release: zero warnings/errors; SecurityRegression: 1284 checks.
- Angular: 187/187 tests across 83 files; 93 templates passed accessibility; external SCSS passed.
- Bundles: 313,179/358,400 initial bytes and 81,743/81,920 largest lazy bytes.
- Live checks: Angular route returned 200; anonymous API read returned 401.

## Next slice

`MOD-024B` migrates Sales Quotation list/details/create, lifecycle transitions and PDF preview/download while keeping pricing,
snapshot, template, numbering and state authority server-side.
