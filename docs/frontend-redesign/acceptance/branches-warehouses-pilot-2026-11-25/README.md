# FRD-H Branches/Warehouses acceptance

> Date: 2026-11-25  
> Result: Passed for the presentation and existing contract scope  
> Scope: Branch Workspace, Warehouse Master and Warehouse Locations

## Parity evidence

| Requirement | Evidence | Result |
|---|---|---|
| Branch Workspace | Localized Administration context, SummaryStrip, shared branch form hierarchy and separate company, branch-directory and access-management sections preserve the existing workspace responsibilities. | Passed by focused component coverage and source inspection. |
| Branch authority | Only server-returned branch scope is rendered. Current-branch links, create/edit/status commands and grant replacement remain permission-aware; internal IDs are not presented as primary choices. | Passed by focused component coverage and 1,668 SecurityRegression checks. |
| Warehouse Master | Three FormSections, required FieldCues, WorkflowFooter and typed DataTable preserve every setting, named branch/account choice, create/edit/delete command, dirty guard and conflict recovery. | Passed by focused component coverage. |
| Warehouse Locations | Shared form/filter/summary/table hierarchy preserves named warehouse selection, parent hierarchy, physical coordinates, capabilities, status and create/edit behavior. | Passed by focused component coverage. |
| Shell branch selector | The already-verified server-permitted selector, scoped persistence and request context were not changed by FRD-H. | Passed by diff and architecture review. |
| EN/AR, RTL/LTR and responsive layout | New presentation text exists in both cultures and layout uses external, logical RTL-safe styles. | Playwright 7 passed, 0 failed, including Desktop/390px. |
| API/schema/business invariants | No API, DTO, schema, domain, Application, repository, accounting or inventory behavior changed. | Passed by diff and architecture review. |

## Verification snapshot

- Angular: 133 files, 291/291 tests passed.
- Production build: passed; the pre-existing 204.88 kB initial shared-chunk warning remains tracked as `FRD-ISS-002`.
- Static accessibility: 142 templates passed.
- UI consistency: 122 feature pages accounted for.
- Browser presentation: 7 passed, 0 failed, 3 authenticated-API checks skipped.
- Security and inventory regression: 1,668 checks passed in Release configuration.

`browser-acceptance.json` is the machine-readable browser result. Its three skipped checks require an externally supplied live API URL and authenticated storage state. This acceptance proves presentation, frontend contract and server permission parity, but does not claim a new live authenticated E2E run.

## Cutover and rollback

The Angular routes and APIs predated FRD-H. This wave changed presentation only, without changing URLs, request contracts or branch authority. Rollback is deployment/Git rollback to the preceding Angular version.

## Decision

FRD-H is closed. Branches, Warehouses and Warehouse Locations are migrated V4 operational-context surfaces. Wave 4 starts with a bounded Inventory audit before any additional UI change.
