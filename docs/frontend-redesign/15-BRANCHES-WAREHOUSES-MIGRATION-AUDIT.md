# 15 — Branches/Warehouses migration audit

> Status: FRD-H implementation in progress — Warehouse Master complete; Warehouse Locations next
> Date: 2026-11-25
> Source of truth: Angular routes/components, API clients, Application services, API controllers, permission definitions and branch-context ADRs

## Current surfaces

| Surface | Route | Current behavior | Main presentation gap |
|---|---|---|---|
| Branch Workspace | `/branches` | Assigned-branch dashboard, company/branch operations, linked warehouses and POS terminals, inline create/edit/status actions, current-branch links and user grant management | One very large inline template mixes dashboard, master-data form, access administration and operational navigation; hard-coded `ADMIN`; no component spec and inline markup is outside the HTML-only static accessibility inventory |
| Warehouse Master | `/warehouses` | Full warehouse list plus inline create/edit form, branch/account choices, operational settings, dirty guard and permission-aware delete | FRD-H slice 1 complete: localized Inventory context, three FormSections, FieldCue, WorkflowFooter, typed DataTable and corrected warehouse Delete label |
| Warehouse Locations | `/inventory/locations` | Warehouse/search filters, hierarchical location table and inline create/edit form with capabilities | Form, filters and tree table are visually compressed; hard-coded `INVENTORY` and no distinct filtered-empty state; FRD-H safety-net component spec now protects hierarchy, named warehouses and create/edit commands |
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
2. Warehouse Locations: SAFETY NET COMPLETE — focused coverage protects named warehouses, hierarchy and create/edit commands; next organize the form/filter/tree table with shared primitives while preserving the same rules.
3. Branch Workspace: first extract the inline template and add component coverage, then separate dashboard, branch master actions and access management visually without splitting authority or inventing routes.
4. Run branch-context, picker-name, permission and presentation acceptance; record the live authenticated API limitation explicitly, then close FRD-H.

## First slice decision

Warehouse Master is first because it is one existing route with a dedicated external template and three focused component tests. It exercises the full View/Create/Edit/Delete permission set, branch/account name selections, dirty-state protection and delete-conflict recovery without touching server-authoritative branch grants. Branch Workspace follows only after its mixed responsibilities and missing component coverage are bounded.

## Exit criteria

- Warehouse Master retains every operational setting, named branch/account option, mutation, delete conflict and dirty/recovery behavior.
- Warehouse Locations retains every field, hierarchy rule, capability, warehouse-name selector and permission.
- Branch Workspace retains assigned-branch scope, conditional operations, current-branch links, master-data actions and grant administration.
- The Shell branch selector and context interceptor remain unchanged unless a separate security-authorized task explicitly changes them.
- EN/AR, RTL/LTR, desktop/390px, keyboard, loading/error/empty/filtered-empty/validation/conflict states pass.
- API methods, DTOs, schema, Application rules, tenant scope and permission authority do not change.
