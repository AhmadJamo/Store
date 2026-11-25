# 15 — Branches/Warehouses migration audit

> Status: FRD-H COMPLETE — accepted 2026-11-25
> Date: 2026-11-25
> Source of truth: Angular routes/components, API clients, Application services, API controllers, permission definitions and branch-context ADRs

## Current surfaces

| Surface | Route | Current behavior | Main presentation gap |
|---|---|---|---|
| Branch Workspace | `/branches` | Assigned-branch dashboard, company/branch operations, linked warehouses and POS terminals, inline create/edit/status actions, current-branch links and user grant management | FRD-H complete: localized Administration context, SummaryStrip, FormSection/FieldCue/WorkflowFooter and separate DetailSections now distinguish company metrics, branch directory and access administration while preserving every command and link |
| Warehouse Master | `/warehouses` | Full warehouse list plus inline create/edit form, branch/account choices, operational settings, dirty guard and permission-aware delete | FRD-H slice 1 complete: localized Inventory context, three FormSections, FieldCue, WorkflowFooter, typed DataTable and corrected warehouse Delete label |
| Warehouse Locations | `/inventory/locations` | Warehouse/search filters, hierarchical location table and inline create/edit form with capabilities | FRD-H slice 2 complete: localized Inventory context, FormSections/FieldCues/WorkflowFooter, FilterBar/SummaryStrip, typed hierarchical DataTable, filtered-empty reset and focused coverage |
| Branch Switcher | Application Shell | Shows only active server-returned branch grants by name and sends the selected ID through `X-MiniStore-Branch-Id` after reload | Already migrated and verified in FRD-E; preserve it as a cross-cutting authority boundary rather than restyling it again |
| Warehouse Pickers | 45 feature-template references | Operational forms display warehouse names while retaining numeric IDs internally | Preserve name-first selection and server-provided option scope; audit affected workflows when their owning module migrates rather than bulk-editing every picker in FRD-H |

## Contracts that must not change

- Branch reads/create/edit/status/access remain separated by `Branches.View`, `Branches.Create`, `Branches.Edit`, `Branches.SetActive`, `Branches.ViewOperations` and `Branches.ManageAccess`.
- Warehouse reads/create/edit/delete remain separated by `Warehouses.View`, `Warehouses.Create`, `Warehouses.Edit` and `Warehouses.Delete`.
- Every POST/PUT/DELETE remains antiforgery-protected and server authorization remains authoritative.
- Ordinary users see only explicitly granted branches; branch access is deny-by-default. `Branches.ManageAccess` administrators retain the existing all-branch authority.
- The Shell selector may choose only active server-returned grants. Browser persistence and `X-MiniStore-Branch-Id` remain context hints, never authorization.
- Warehouse/POS operational configuration does not grant branch access.
- Warehouse master validation, accounting assignment, operational type/control/picking/receiving settings and in-use delete conflict remain Application-owned.
- Warehouse and branch selections remain name-first (code may supplement a name); numeric IDs stay internal.
- Warehouse-location hierarchy, parent eligibility, capabilities, status and warehouse-name lookup remain unchanged.
- No API, DTO, schema, branch-scope rule, inventory rule or accounting rule changes are authorized by FRD-H presentation work.

## Migration order

1. Warehouse Master List/Form: COMPLETE — shared PageHeader/FormSection/FieldCue/WorkflowFooter/typed DataTable adopted; every command, option, dirty guard, recovery state and permission preserved.
2. Warehouse Locations: COMPLETE — focused coverage plus shared form/filter/summary/typed-tree hierarchy preserve named warehouse selection, hierarchy, capabilities and create/edit commands.
3. Branch Workspace: COMPLETE — external template, localized context, shared form/summary/workflow hierarchy and separate company/directory/access sections preserve server-returned scope, operations/links, master commands and grant replacement without splitting authority or inventing routes.
4. COMPLETE — branch-context, picker-name, permission and presentation acceptance passed; the live authenticated API limitation is explicit in `acceptance/branches-warehouses-pilot-2026-11-25/`.

## First slice decision

Warehouse Master is first because it is one existing route with a dedicated external template and three focused component tests. It exercises the full View/Create/Edit/Delete permission set, branch/account name selections, dirty-state protection and delete-conflict recovery without touching server-authoritative branch grants. Branch Workspace follows only after its mixed responsibilities and missing component coverage are bounded.

## Exit criteria

- Warehouse Master retains every operational setting, named branch/account option, mutation, delete conflict and dirty/recovery behavior.
- Warehouse Locations retains every field, hierarchy rule, capability, warehouse-name selector and permission.
- Branch Workspace retains assigned-branch scope, conditional operations, current-branch links, master-data actions and grant administration.
- The Shell branch selector and context interceptor remain unchanged unless a separate security-authorized task explicitly changes them.
- EN/AR, RTL/LTR, desktop/390px, keyboard, loading/error/empty/filtered-empty/validation/conflict states pass.
- API methods, DTOs, schema, Application rules, tenant scope and permission authority do not change.

## Closure evidence

- Angular: 291/291 tests across 133 files.
- Production build: passed; only the pre-existing 204.88 kB shared-chunk warning remains.
- Static accessibility: 142 templates passed.
- UI/UX consistency: 122 feature pages accounted for.
- Browser: 7 passed, 0 failed and 3 authenticated-API skips.
- SecurityRegression: 1,668 checks passed in Release configuration.
- Machine-readable and review evidence: `acceptance/branches-warehouses-pilot-2026-11-25/`.

FRD-H is closed. Wave 4 begins with a bounded Inventory audit; this closure authorizes no bulk inventory restyle and no business-rule change.