# Phase 23 — Purchasing Angular migration

> Status: IN PROGRESS — `MOD-023A..E` COMPLETE on 2026-10-28
>
> Presentation default: MVC until a separate cutover approval.

## Scope

Move the existing procure-to-pay workflows to Angular in bounded slices. Document numbering, managed units, approval snapshots,
supplier sourcing, inventory/accounting posting, concurrency and tenant authority remain in Application/Domain and the server.

## `MOD-023A` delivered — Purchase Requests

- Added authenticated, versioned list/details/create-form/create/submit/cancel APIs over `PurchaseRequestService` only.
- Preserved independent `PurchaseRequests.View/Create/Submit/Cancel` permissions, antiforgery on every command and row-version checks.
- Added permission-aware Angular `/purchases/requests`, `/new` and `/:id` views with search/status filter, named warehouse/product/unit/
  supplier options, dynamic lines, details, approval evidence, history, submit and reasoned cancel.
- Rejected input remains editable and create forms are dirty-route protected. All presentation is Arabic/English, RTL-safe, responsive
  and uses external SCSS. Numbering, purchasability, unit-dimension conversion and lifecycle transitions remain server-owned.
- No schema, inventory, sourcing or accounting behavior changed; MVC remains default.

## Evidence

- Web Release build: zero warnings and zero errors.
- Security regression: 1227 checks passed.
- Angular: 166/166 tests across 63 files passed.
- Static accessibility: 72 templates passed; external-SCSS policy passed.
- Production bundles: 304,833/358,400 initial bytes and 81,742/81,920 largest lazy bytes.
- Live checks: list/create/details Angular routes returned 200; anonymous API read/write returned 401.

## `MOD-023B` delivered — approval rules and decisions

- Added an authenticated API for approval-rule list/create/activate/deactivate plus separately permissioned Purchase Request
  approve/reject commands. Every command validates antiforgery; rule activation retains row-version concurrency.
- Extended the Angular purchasing workspace with a permission-linked rules route, ordered role steps, warehouse/priority scope,
  active-state control, current-role evidence and eligible approve/reject controls with bounded notes.
- `PurchaseApprovalService` still resolves and freezes the matching rule, enforces the current ordered role and advances/rejects the
  instance and request atomically. Angular cannot skip steps or decide from UI permission state alone.
- Arabic/English, RTL, dirty-form recovery, external SCSS and accessibility are preserved. No schema or business rule changed.

## Current evidence

- Web Release build: zero warnings and zero errors.
- Security regression: 1230 checks passed.
- Angular: 167/167 tests across 63 files passed.
- Static accessibility: 72 templates passed; external-SCSS policy passed.
- Production bundles: 305,003/358,400 initial bytes and 81,742/81,920 largest lazy bytes.
- Live checks: approval-rules Angular route returned 200; anonymous approval/request reads returned 401.

## `MOD-023C` delivered — sourcing, quotations and award

- Added authenticated versioned APIs and Angular routes for sourcing list/details/create-from-approved-request, supplier invitation,
  quotation capture/comparison and one-time reasoned line allocation.
- Every command uses the existing Application services, independent `PurchaseSourcing.*` / `SupplierQuotations.*` permissions and
  antiforgery. Request eligibility, invited-supplier checks, commercial snapshots, exchange rates, exact demand allocation and state
  transitions remain server-owned.
- The bilingual RTL-safe workspace exposes names and commercial evidence rather than user-facing IDs, retains rejected input, hides the
  award action after completion and displays the frozen allocation evidence. No schema or business rule changed; MVC remains default.

## Current evidence

- Web Release: zero warnings/errors; SecurityRegression: 1233 checks.
- Angular: 168/168 tests across 64 files; 73 templates passed accessibility; external SCSS passed.
- Production bundles: 305,736/358,400 initial bytes and 81,742/81,920 largest lazy bytes.

## `MOD-023D` delivered — Purchase Orders

- Added authenticated list/details and create-from-award APIs plus approve, confirm, cancel, close/reopen remaining quantity,
  direct-delivery configuration/evidence and supplier delivery-date confirmation.
- Every command has its own existing `PurchaseOrders.*` permission, antiforgery and validated concurrency evidence. The Angular screen
  hides completed actions and exposes supplier, warehouse, currency, fulfillment progress, commercial lines and lifecycle history.
- `PurchaseOrderService` and Domain remain authoritative for award materialization, snapshots, status, direct delivery and quantities.
  Approval/confirmation still changes neither stock nor accounting. No schema/business-rule change; MVC remains default.

## Current `MOD-023D` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1236 checks.
- Angular: 169/169 tests across 65 files; 74 templates passed accessibility; external SCSS passed.
- Bundles: 306,260/358,400 initial bytes and 81,742/81,920 largest lazy bytes.

## `MOD-023E` delivered — Goods Receipts

- Added authenticated Goods Receipt list/details/create-form APIs plus direct create/post, scanning draft, idempotent product
  scan, final post, staged quality inspection and accepted-stock put-away operations.
- Every mutation retains antiforgery and its existing `GoodsReceipts.Create/Inspect/PutAway` permission. Confirmed-order
  eligibility, remaining quantities, managed units, lot/serial/date requirements, inventory valuation, physical movements,
  GRNI journal posting, staged status transitions and exact accepted/rejected/damaged/short totals remain server-owned.
- Added bilingual RTL-safe Angular `/purchases/receipts`, `/new`, `/:id` and `/:id/scan` routes with rejected-input
  preservation, dirty navigation protection and a permission-aware Receive Goods link from eligible confirmed orders.
- No schema or business-rule change; MVC remains default.

## Current `MOD-023E` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1239 checks.
- Angular: 170/170 tests across 66 files; 75 templates passed accessibility; external SCSS passed.
- Bundles: 306,587/358,400 initial bytes and 81,742/81,920 largest lazy bytes.

## Next slice

`MOD-023F` migrates Vendor Bill list/details/create, approval/match override/post/cancel and current attachment/extraction-review
operations while keeping receipt allocation, tolerance, tax, PPV, GRNI, payable and journal authority on the server.
