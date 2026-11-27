# FRD-I Inventory acceptance

> Date: 2026-11-27
>
> Result: Accepted for presentation, existing contracts, backend build and security regression with one independent open bundle-budget issue
>
> Scope: 22 Angular Inventory routes across 15 page templates

## Scope reconciliation

Source review of `MiniStore Front-End/src/app/app.routes.ts` confirms 22 routes: Stock Transactions (1), Product Stock (4), Availability (1), Reservations (1), Stock Transfers (4), Scanning (1), Replenishment (1), Insights (1), Reconciliation (1), Adjustments (3), Tracking (1), Warehouse Locations (1), Putaway (1) and Location Movements (1). The previous audit count of 19 was a documentation arithmetic error; no route or runtime surface was missing. The feature directory contains 15 `*page.html` templates.

## Contract evidence

| Requirement | Evidence | Result |
|---|---|---|
| Server authority | Inventory calculations, posting, valuation, reservations, tracking, capacity, suggestions and workflow transitions remain in existing Application services and repositories. | Passed by source review and focused component/API tests. |
| Permissions | Read, create, edit and workflow actions retain their independent permission checks, including StockMovements.View evidence separation. | Passed by source review and 1,668 SecurityRegression checks. |
| Named selections | Product, warehouse and location choices remain name/code-first while IDs stay internal command values. | Passed by focused form and command tests. |
| Idempotency and dirty recovery | Putaway, relocation, transfer, scanning and other operational commands preserve server-issued/generated keys, validation and dirty guards. | Passed by focused workspace tests. |
| V4 hierarchy | Scoped pages use localized PageHeader, FormSection/FieldCue/WorkflowFooter, FilterBar, SummaryStrip, DetailSection, WorkflowProgress and typed DataTable patterns as appropriate. | Passed by 142-template accessibility and 122-page consistency gates. |
| EN/AR, RTL/LTR and responsive | Changed user-facing copy is bilingual and shared layouts use logical direction-safe styling. | Browser presentation passed 7/7 including direction and 390px viewport. |
| API/schema/business invariants | FRD-I introduced no API, DTO, schema, permission, accounting or inventory-rule change. | Passed by diff and architecture review. |

## Verification snapshot

- Backend Release build: passed with 0 warnings and 0 errors.
- Security and inventory regression: 1,668 checks passed.
- Angular: 142 files, 370/370 tests passed.
- Production Angular build: completed successfully.
- Static accessibility: 142 templates passed.
- UI consistency: 122 feature pages accounted for.
- Browser presentation: 7 passed, 0 failed; live API/session checks were skipped because no API URL or authenticated storage state was supplied.
- Performance: skipped because no API URL was supplied; no performance claim is made.
- Unified gate: 10 passed, 1 failed, 4 skipped. The only failure is `FRD-ISS-012`: core-localization lazy chunk 84.55 kB versus the 80 kB budget. The budget was not relaxed.

`angular-acceptance-report.json` is the machine-readable gate result.

## Cutover and rollback

Inventory Angular routes and Application-backed APIs predated FRD-I. This program changed presentation and test coverage without changing URLs or business contracts. Legacy presentation is already retired, so rollback is deployment/Git rollback to the preceding Angular version rather than a hidden MVC switch.

## Decision

FRD-I is closed for its authorized presentation and existing-contract scope. `FRD-ISS-012` remains an independent medium-priority optimization and live authenticated API/performance evidence remains external until an environment URL and storage state are supplied. The next migration wave is Purchases, beginning with a bounded source and contract audit before UI changes.