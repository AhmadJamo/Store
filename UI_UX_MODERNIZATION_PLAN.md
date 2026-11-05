# MiniStore Angular UI/UX Modernization Plan

> Status: IN PROGRESS  
> Started: 2026-11-04  
> Source of truth: implemented Angular code, backend contracts, permissions and executable verification  
> Rule: update this file at the end of every implementation phase

## Objective

Transform the existing Angular ERP into a cohesive, compact, accessible and workflow-oriented workspace without
rewriting business rules, weakening permissions, inventing APIs or removing the proven MVC rollback path. Visual polish
is not sufficient: navigation, context preservation, page architecture, tables, forms, workflows and related records
must converge on shared standards and existing functionality must remain verified.

## Baseline audit

Source inventory captured on 2026-11-04:

| Measure | Baseline |
|---|---:|
| Angular route declarations | 186 |
| Feature page components | 113 |
| Feature HTML templates | 113 |
| Feature SCSS files | 84 |
| Shared non-test TypeScript UI/form artifacts | 10 |
| Pages using `app-page-header` | 101 |
| Pages using `app-data-table` | 36 |
| Feature pages integrated with `WorkspaceService` | 85 |
| Pages with `canDeactivate` form protection | 69 |
| Feature SCSS files defining local `.filters` patterns | 24 |
| Feature SCSS files defining local `.actions` patterns | 30 |

### Existing strengths to preserve

- Standalone Angular architecture, lazy routes, strict typing and feature-based folders.
- Central design tokens with light/dark, compact/comfortable, RTL/LTR, reduced-motion and forced-colors support.
- Persistent permission-aware workspace tabs, bounded per-tab history, recently closed tabs and dirty-tab protection.
- Central session, CSRF, Problem Details, localization and permission boundaries.
- Shared page header, feedback, recovery, pager, form error and data-table wrappers.
- Server-side business authority and permission checks; Angular does not own workflow truth.
- 81 presentation surfaces already default to Angular with MVC rollback retained.

### Priority problems

1. The sidebar is a long flat list. Business domains and operational intent are not visually separated.
2. Workspace tabs persist routes, but page-context preservation is inconsistent. Only Customers and Suppliers use the
   current safe list-preference service; search, page, selected subtab and draft policies are not centralized.
3. The shared data-table component is currently an accessible overflow wrapper, not a configurable enterprise table.
   Sorting, filter layout, column preferences, selection and row actions are repeatedly page-owned.
4. Page headers are broadly adopted, but breadcrumbs, context actions and workflow footers are not standardized.
5. Twenty-four filter layouts and thirty action layouts repeat local SCSS, causing density and alignment drift.
6. Many feature files are highly compact/minified, increasing change risk and slowing consistent UX refactoring.
7. Workflow actions exist, but progress, next-action guidance, missing prerequisites and related-document navigation vary
   by module.
8. The disabled topbar search looks unfinished. It must become a permission-filtered command/page search or be removed
   until a supported implementation exists.
9. Mobile behavior exists but must be revalidated after each shell/table change; desktop ERP productivity remains the
   primary layout target.

## Target navigation hierarchy

The hierarchy is permission-filtered and never grants access:

1. Overview — Home, My Work, Operations Center.
2. Customers and suppliers — customer and supplier workspaces.
3. Catalog — products and future catalog masters surfaced through registered routes.
4. Inventory and warehouses — stock, availability, reservations, transfers, scanning, replenishment, insights,
   reconciliation, adjustments, tracking, locations, putaway and movements.
5. Purchasing — purchase requests first, followed incrementally by the existing purchase-document routes.
6. Sales — quotations, orders, deliveries, invoices, payments, returns and legacy/POS routes when registered.
7. Accounting and finance — accounts, journals and financial reconciliation routes when registered.
8. Documents and analytics — document evidence and analytics workspaces.
9. Administration and settings — settings, users, roles, audit and design-system reference.

The initial implementation groups all currently registered shell items. Later phases expand the registry only with real,
permission-backed routes already present in `app.routes.ts`.

## Architecture decisions

| ID | Decision | Rationale |
|---|---|---|
| UX-ADR-001 | Keep Angular Router as the navigation source of truth; workspace tabs wrap routes rather than render duplicate router outlets. | Preserves guards, lazy loading, deep links and browser behavior. |
| UX-ADR-002 | Keep persistent multi-page tabs and improve them incrementally. | The existing implementation is safe, tested and already permission-aware. |
| UX-ADR-003 | Persist only allowlisted, non-secret UI state. | Search terms, credentials, tokens, unrestricted form payloads and sensitive document values must not leak into browser storage. |
| UX-ADR-004 | Use URL query parameters for shareable server-supported filters; use bounded in-memory tab state for transient context; use explicit feature draft adapters only where business-safe. | Avoids a generic unsafe form dump while preserving useful context. |
| UX-ADR-005 | Add no UI library during the foundation phases. | Existing tokens and standalone components are sufficient; a dependency requires measured benefit and explicit review. |
| UX-ADR-006 | Shared components provide infrastructure and configuration; entity columns, workflow actions and business meaning stay feature-owned. | Prevents a generic table/workflow abstraction from hiding domain rules. |
| UX-ADR-007 | Navigation grouping and breadcrumbs are shell concerns; authorization remains endpoint/session driven. | Visual organization must never become an authorization boundary. |

## Phased roadmap

### UX-001 — Audit and workspace navigation foundation — COMPLETED 2026-11-04

- [x] Capture the initial code-derived frontend inventory and priority findings.
- [x] Create this continuously maintained roadmap.
- [x] Preserve the existing persistent workspace-tab architecture.
- [x] Add permission-filtered, collapsible business navigation groups with safe local preference persistence.
- [x] Add shell-level bilingual breadcrumbs for registered routes.
- [x] Verify unit tests, production build, static accessibility, bundle budgets and hosted RTL/LTR behavior.
- [x] Document verification results and commit the phase.

### UX-002 — Context preservation contract — COMPLETED 2026-11-05

- Define a reusable typed page-context contract for search, filters, sorting, pagination and selected subtabs.
- Separate URL-shareable state, safe persistent preferences and memory-only transient state.
- Add explicit draft adapters only to approved forms; never persist passwords, tokens, attachments or unrestricted notes.
- Migrate representative Customers, Suppliers, Products, Purchase Requests and Stock Transfers pages.
- Verify back/forward, tab switching, reload behavior, dirty guards and tenant/user boundaries.

### UX-003 — Unified page architecture and action zones — COMPLETED 2026-11-05

- [x] Extend the page-header system with primary/secondary action slots and compact context.
- [x] Add reusable filter bar, summary strip, section header and sticky workflow footer.
- [x] Replace repeated local `.filters` and `.actions` patterns on Products, Purchase Requests and Stock Transfers.
- [x] Establish responsive desktop/mobile rules and RTL logical spacing.

### UX-004 — Enterprise data-table infrastructure — PENDING

- Evolve `app-data-table` into configurable infrastructure for loading/empty/error states, sticky headers, alignment,
  status cells, row actions, selection and column preferences.
- Keep server-side paging/filter/sort where APIs support them; do not fake backend capabilities.
- Define typed entity-specific table configurations.
- Pilot on Products, Purchase Requests and Stock Transfers before module-wide migration.

### UX-005 — Forms and detail pages — PENDING

- Standardize field groups, required/optional cues, validation summaries, keyboard flow and submission locking.
- Standardize detail tabs: Overview, Details, Related Records, Activity and Available Actions where supported.
- Preserve safe draft state on approved high-value forms and verify duplicate-submit protection.

### UX-006 — Workflow integration — PENDING

- Add a reusable display-only workflow progress component driven by backend status/actions.
- Add a permission/state-aware workflow footer and prerequisite explanations.
- Pilot Stock Transfers, Purchase Requests, Purchase Orders and Sales Orders.
- Verify every existing transition, alternative path and next-step link without changing domain rules.

### UX-007 — Productivity and connected information — PENDING

- Replace the disabled search with permission-filtered page/command search; add entity search only where backend APIs
  provide bounded endpoints.
- Add contextual quick actions, keyboard shortcut discovery and safe recent destinations.
- Connect related records using existing identifiers and APIs; do not duplicate source-of-truth data.

### UX-008 — Consistency rollout and validation — PENDING

- Migrate every implemented module to the shared page/table/form/workflow standards.
- Run route, permission, workflow, responsive, RTL/LTR, keyboard, accessibility and performance verification.
- Record exceptions explicitly; modernization is not complete while implemented modules use conflicting standards or
  existing functionality lacks evidence.

## Reusable component target

- `EnterprisePageHeader`: title, description, breadcrumb, status, primary and secondary actions.
- `EnterpriseFilterBar`: compact responsive filters, apply/reset and active-filter summary.
- `EnterpriseDataTable`: infrastructure only; typed columns and business actions remain feature configuration.
- `WorkflowProgress`: display current server state and valid stages without calculating transitions.
- `WorkflowActionBar`: Back/Cancel on the logical start side and safe primary next action on the end side.
- `EntitySummary` and `RelatedRecords`: consistent metadata and relationship navigation.
- `PageContextService`: typed URL/persistent/memory context with explicit allowlists.
- `UnsavedDraftAdapter`: opt-in, feature-owned serialization with sensitive-field exclusions.

## Verification policy

Every phase must record:

- TypeScript/Angular unit results.
- Production compilation and bundle budgets.
- Static template/accessibility results.
- Affected route and permission checks.
- Desktop, tablet/mobile and Arabic RTL/English LTR evidence where layout changed.
- Existing business workflow regression evidence for any changed feature page.

## Verification ledger

| Phase | Build/tests | UX evidence | Result |
|---|---|---|---|
| UX-001 | 220/220 Angular; 122 templates; production 328,867 B initial and 81,880 B largest lazy, both custom budgets green | Browser states, RTL/LTR, keyboard and 390px responsive checks passed; Windows interactive helper failed independently; authenticated reconciliation later hit a stale hidden “طحين” option | COMPLETE WITH RECORDED EXTERNAL FIXTURE RISK |
| UX-002 | 224/224 Angular; 122 templates; production 328,867 B initial and 81,880 B largest lazy, custom budgets green | URL normalization, back/forward observation, workspace-memory restoration, tenant/user isolation and memory-only draft tests passed across five pilot pages | COMPLETE |
| UX-003 | 225/225 Angular; 126 templates; production 330,806 B initial and 81,880 B largest lazy, custom budgets green | Hosted states, RTL/LTR, keyboard and 390px responsive checks passed; shared page-region accessibility contracts passed | COMPLETE |

## Latest phase report

### UX-003 — Completed

- Extended `app-page-header` with explicit primary, secondary and context projection zones while retaining compatibility
  with existing consumers.
- Added reusable accessible `app-filter-bar`, `app-summary-strip`, `app-section-header` and sticky
  `app-workflow-footer` primitives using logical CSS and mobile fallbacks.
- Applied the architecture to Products, Purchase Requests and Stock Transfers without changing permissions, routes,
  API contracts, workflow states or business rules.
- Moved high-value create/save and lifecycle actions into consistent action zones, reducing separated action clusters.

### Next

- UX-004: evolve the shared data-table infrastructure and pilot typed table configuration on the same three pages.

### Recorded risks

- The built-in Angular framework chunk still emits its pre-existing 180 kB warning; the project release budgets remain
  green at 330,806/358,400 initial bytes and 81,880/81,920 largest lazy bytes.
- UX-003 intentionally does not redesign table semantics or infer workflow actions; those remain feature-owned and are
  addressed separately by UX-004 and UX-006.

## Risks

| Risk | Control |
|---|---|
| Persisting sensitive search/form content | Allowlist safe preferences; keep transient state in memory; feature-specific draft adapters. |
| Tabs diverging from Router state | Router remains authoritative; tab state stores only registered safe routes. |
| Generic components hiding domain rules | Shared infrastructure only; feature configuration owns columns/actions/status meaning. |
| Visual refactor breaking workflows | Migrate bounded modules and rerun existing permission/workflow tests each phase. |
| Sidebar hierarchy hiding permitted pages | Groups contain only session-permitted registered routes and active groups auto-expand. |
| Bundle growth | Lazy feature boundaries and existing initial/lazy budgets remain release gates. |
| RTL/mobile regressions | Logical CSS, keyboard tests and hosted viewport/direction checks after shell changes. |

## Definition of complete

The program is complete only when all implemented modules use the same approved shell, page, table, form, feedback and
workflow standards; common operations require fewer verified interactions; safe page context survives appropriate
navigation; permissions and backend workflow rules remain authoritative; and the complete regression, accessibility,
responsive, RTL/LTR and production-build gates pass. A visually improved subset is not completion.
