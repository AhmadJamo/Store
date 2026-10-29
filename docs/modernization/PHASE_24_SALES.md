# Phase 24 — Sales Angular migration

> Status: IN PROGRESS — `MOD-024A..D` COMPLETE on 2026-10-29
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

## `MOD-024B` delivered — sales quotations

- Added authenticated, permission-separated list/details/create-form/create and PDF endpoints plus independent Send, Accept,
  Reject, Expire and Cancel commands. Every mutation retains antiforgery and optimistic-concurrency evidence.
- Added bilingual RTL-safe Angular list/filter, complete quotation composer and detail/history workspaces. Creation supports named
  customers and addresses, active templates/designs/price lists, product/unit/tax selection, template option evidence and custom lines.
- `SalesQuotationService` remains authoritative for idempotency, numbering, template/design eligibility, repricing, units, currency,
  tax, immutable commercial snapshots, transition eligibility and frozen PDF evidence. Quotations have no inventory, accounting or
  payment effect.

## `MOD-024C` delivered — sales orders and control approvals

- Added authenticated list/details, accepted-quotation conversion, confirmation, cancellation and approval-decision APIs. Each
  mutation retains its independent permission, antiforgery and the existing concurrency contract.
- Added bilingual RTL-safe Angular list/detail workspaces with source quotation navigation, immutable commercial lines, lifecycle
  history, control findings/steps, eligible approval decisions and reservation/delivery/invoice progress evidence.
- `SalesOrderService` and `SalesOrderApprovalService` remain authoritative for one-time conversion, frozen source snapshots,
  numbering, control evaluation, approval order, confirmation and cancellation eligibility. Confirmation creates no reservation,
  stock movement or accounting entry.

## `MOD-024D` delivered — sales order inventory reservations

- Added authenticated reservation-page, Reserve and Release APIs with independent permissions and antiforgery.
- Added a bilingual RTL-safe Angular reservation workspace with named warehouses/locations, shortage policy, per-line remaining
  quantities, stock-position evidence and active/history release controls.
- `SalesOrderReservationService` remains authoritative for confirmed-order eligibility, warehouse/location eligibility, rowversion,
  cumulative committed quantities, balance locking and shortage handling. Reservations change Reserved/Available only; they do not
  reduce OnHand or create accounting entries.

## Evidence

- Web Release: zero warnings/errors; SecurityRegression: 1293 checks.
- Angular: 190/190 tests across 86 files; 98 templates passed accessibility; external SCSS passed.
- Bundles: 314,140/358,400 initial bytes and 81,743/81,920 largest lazy bytes.

## Next slice

`MOD-024E` migrates Sales Delivery list/details/create, posting and cancellation while keeping reservations, tracking, valuation,
physical stock movement and COGS posting authority server-side.
