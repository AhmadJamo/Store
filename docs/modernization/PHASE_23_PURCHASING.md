# Phase 23 — Purchasing Angular migration

> Status: IN PROGRESS — `MOD-023A..B` COMPLETE on 2026-10-27
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

## Next slice

`MOD-023C` migrates Purchase Sourcing events, supplier invitations, Supplier Quotations and reasoned award/comparison while preserving
request eligibility, frozen commercial snapshots, quantity/currency rules and server-owned state transitions.
