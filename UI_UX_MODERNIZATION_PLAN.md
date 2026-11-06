# MiniStore Angular UI/UX Modernization Plan

> Status: COMPLETE WITH DOCUMENTED LAYOUT EXCEPTIONS
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

### UX-004 — Enterprise data-table infrastructure — COMPLETED 2026-11-05

- [x] Evolve `app-data-table` into configurable infrastructure for loading/empty/error states, sticky headers,
  type-aware alignment, status cells and row actions.
- [x] Keep server-side paging/filter/sort authoritative; the component emits sort intent and never reorders rows.
- [x] Define typed entity-specific column configurations and feature-owned cell templates.
- [x] Pilot on Products, Purchase Requests and Stock Transfers before module-wide migration.

### UX-005 — Forms and detail pages — COMPLETED 2026-11-05

- [x] Add reusable form sections and localized required/optional field cues.
- [x] Add reusable detail sections without inventing decorative tabs or new information groupings.
- [x] Pilot Product create/edit/details, Purchase Request create/details and Stock Transfer create/details.
- [x] Preserve existing validation, safe draft adapters, dirty guards, permission gates and submission locking.
- [x] Verify unit tests, production build, static accessibility and hosted RTL/LTR/mobile behavior.

### UX-006 — Workflow integration — COMPLETED 2026-11-05

- [x] Add a reusable display-only workflow progress component driven by backend status.
- [x] Add localized next-step and prerequisite guidance without calculating transitions in the shared component.
- [x] Pilot Stock Transfers, Purchase Requests, Purchase Orders and Sales Orders.
- [x] Reuse the workflow footer for Purchase and Sales Order action zones while preserving feature permission/state checks.
- [x] Verify tests, production build, static accessibility and hosted RTL/LTR/mobile behavior.

### UX-007 — Productivity and connected information — COMPLETED 2026-11-05

- [x] Replace the disabled shell search with permission-filtered page/command destination search.
- [x] Route every result through the existing Router/workspace-tab authority and permission recheck.
- [x] Add `Ctrl+K` and `/` discovery, keyboard activation, responsive results and safe memory-only recent destinations.
- [x] Keep queries memory-only and avoid entity indexing or new API/business-data storage.
- [x] Verify tests, production build, custom budgets, static accessibility and hosted RTL/LTR/mobile behavior.

### UX-008 — Consistency rollout and validation — COMPLETED 2026-11-05

- [x] Add a reproducible inventory gate for all 113 feature pages and the reusable UI contracts.
- [x] Close the remaining ERP-workspace header inconsistency on Customer create/edit.
- [x] Record 11 intentional non-workspace layout exceptions by exact path and rationale.
- [x] Integrate consistency inventory into the modernization acceptance command.
- [x] Run Angular, Release, security, responsive, RTL/LTR, keyboard, accessibility and bundle verification.

### UX-009 — Navigation completeness and module hubs — COMPLETED 2026-11-05

- [x] Add concise Purchasing, Sales and Accounting groups to the professional workspace navigation.
- [x] Add one reusable permission-aware module hub instead of duplicating module-specific shells.
- [x] Register all 41 non-detail operational routes for search, favorites, tabs, breadcrumbs and safe restoration.
- [x] Keep detailed destinations out of the sidebar while preserving permission filtering everywhere.
- [x] Add executable route-coverage, unit, accessibility, bundle and hosted RTL/LTR/mobile evidence.

### UX-010 — Accounting workspace foundation — COMPLETED 2026-11-05

- [x] Project the chart of accounts as a safe server-authored parent/child hierarchy.
- [x] Add expandable, searchable bilingual Angular tree presentation with preserved safe context.
- [x] Add a read-only, repository-paged journal directory with search/date/status filters and immutable-detail links.
- [x] Preserve tenant isolation, existing permissions and all posting/fiscal-period authority.
- [x] Verify .NET, security, Angular, accessibility, bundle and hosted RTL/LTR/mobile gates.

### UX-011 — Read-only financial reporting foundation — COMPLETED 2026-11-06

- [x] Add server-calculated trial balance movement and closing balances by inclusive period.
- [x] Add server-calculated balance sheet and income statement with explicit normal-balance definitions.
- [x] Exclude drafts and preserve correct separately-posted reversal treatment.
- [x] Add exact account/date drill-down to the tenant-filtered journal directory.
- [x] Preserve report period/tab context and expose the workspace through the existing accounting read permission.
- [x] Verify golden accounting data, .NET, Angular, accessibility, route coverage, bundles and hosted RTL/LTR/mobile gates.

### UX-012 — Controlled manual journal foundation — COMPLETED 2026-11-06

- [x] Add independent View/Create/Post/Reverse accounting permissions and protected API commands.
- [x] Add balanced tenant-validated Draft composition with named accounts and optional dimensions.
- [x] Route posting and exact reversal through the existing journal gateway, fiscal periods and Serializable UnitOfWork.
- [x] Preserve unsaved content and the stable retry key in memory only; clear it after success.
- [x] Keep posted/reversed evidence immutable and provide direct next-step detail navigation.
- [x] Verify golden lifecycle, isolation, idempotency, closed periods, Angular, accessibility and bundle gates.

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
| UX-004 | 227/227 Angular; 126 templates; production 333,336 B initial and 81,880 B largest lazy, custom budgets green | Typed columns, bounded states and server-sort intent tests passed; hosted RTL/LTR, keyboard and 390px responsive checks passed | COMPLETE |
| UX-005 | 229/229 Angular; 129 templates; production 333,336 B initial and 81,880 B largest lazy, custom budgets green | Shared form/detail semantics passed; hosted states, RTL/LTR, keyboard and 390px responsive checks passed | COMPLETE |
| UX-006 | 230/230 Angular; 130 templates; production 333,336 B initial and 81,880 B largest lazy, custom budgets green | Workflow stage/current/guidance semantics passed; hosted states, RTL/LTR, keyboard and 390px responsive checks passed | COMPLETE |
| UX-007 | 232/232 Angular; 130 templates; production 335,312 B initial and 81,880 B largest lazy, custom budgets green | Permission-filtered results, Ctrl+K focus and memory-only query tests passed; hosted states, RTL/LTR, keyboard and 390px responsive checks passed | COMPLETE |
| UX-008 | 232/232 Angular; 130 templates; isolated Release build clean; 1,843 security/inventory checks; production 335,312 B initial and 81,880 B largest lazy | 113-page inventory: 102 shared headers + 11 named layout exceptions; 5/5 hosted states, RTL/LTR, keyboard and 390px checks passed | COMPLETE WITH DOCUMENTED LAYOUT EXCEPTIONS |
| UX-009 | 234/234 Angular; 131 templates; production 335,946 B initial and 81,880 B largest lazy | 72 registered destinations; 41/41 operational module routes; 5/5 hosted RTL/LTR, keyboard, states and 390px checks passed | COMPLETE |
| UX-010 | 236/236 Angular; 1,843 security checks; 132 templates; production 336,053 B initial and 81,880 B largest lazy | Expandable account hierarchy; 73 destinations; 42/42 operational routes; 5/5 hosted acceptance | COMPLETE |
| UX-011 | 237/237 Angular; 1,847 security/accounting checks; 133 templates; production 336,179 B initial and 81,880 B largest lazy | Golden balanced ledger; 74 destinations; 43/43 operational routes; hosted acceptance 6 passed/0 failed | COMPLETE |
| UX-012 | 238/238 Angular; 1,855 security/accounting checks; 134 templates; production 336,483 B initial and 81,880 B largest lazy | Golden Draft/Post/Reverse lifecycle; 75 destinations; 44/44 routes; hosted 6 passed/0 failed | COMPLETE |
| UX-013 | 239/239 Angular; 1,864 security/accounting checks; 135 templates; production 336,904 B initial and 81,877 B largest lazy | Golden bucket boundaries and due-date snapshot; 76 destinations; 45/45 routes; hosted presentation 6 passed/0 failed | COMPLETE |

## Latest phase report

### UX-008 — Completed

- Added `scripts/Test-UiUxConsistency.ps1` and made it a required modernization gate.
- The gate accounts for every feature page: 102 ERP workspace pages use `app-page-header`; 11 public, authentication,
  onboarding, platform-operator or design-reference pages retain intentionally independent layouts.
- Customer create/edit now uses the same shared header as the rest of the ERP workspace.
- The command search now exposes an explicit search landmark without changing routing, permissions or query storage.
- Full evidence is recorded in `docs/modernization/UI_UX_CONSISTENCY_AUDIT.md`.

### UX-009 — Completed

- Purchasing, Sales and Accounting now have concise sidebar groups and reusable permission-aware module hubs.
- Search, favorites and persistent tabs can discover 41 operational module routes without placing every page in the sidebar.
- Navigation-only localization was split from the global resource to keep the existing strict lazy budget unchanged.
- Full evidence is recorded in `docs/modernization/UI_UX_NAVIGATION_HUBS.md`.

### UX-011 — Completed

- Added a reconciliable trial balance, balance sheet and income statement calculated only in the Application/repository layers.
- Added exact account/date journal drill-down and preserved safe report context in workspace tabs and browser history.
- Documented draft exclusion, reversal evidence, normal balances, current earnings and company-currency boundaries.
- Full evidence is recorded in `docs/modernization/UI_UX_FINANCIAL_REPORTING.md`.

### UX-012 — Completed

- Added balanced manual Draft creation, explicit posting and exact opposite-entry reversal without direct EF access.
- Added separate permissions, CSRF protection, tenant dimension validation and stable retry behavior.
- Added memory-only unsaved form recovery and removed mutation actions after their one-time state transition.
- Full evidence is recorded in `docs/modernization/UI_UX_MANUAL_JOURNALS.md`.

### UX-013 — Completed

- Added server-authored customer and supplier aging from frozen company-currency carrying values.
- Added full control-account reconciliation that remains complete when display search is applied.
- Added safe document/payment/credit/journal drill-down and explicit missing-account differences.
- New vendor bills freeze payment terms and due date; historical rows remain untouched with a documented bill-date fallback.
- Full evidence is recorded in docs/modernization/UI_UX_AGING_RECONCILIATION.md.

### Next

- UX-014 is proposed for a server-authored cash-flow statement and period-close readiness workspace; owner approval is pending.

### Recorded risks

- The built-in Angular framework chunk still emits its pre-existing 180 kB warning; the project release budgets remain
  green at 335,312/358,400 initial bytes and 81,880/81,920 largest lazy bytes.
- Column visibility persistence was intentionally deferred: a generic preference could hide identity or action evidence.
  Any future column chooser must use a feature-owned allowlist and keep required columns visible.
- Required/optional cues improve scanning but do not replace validation. Existing Angular validators and server Problem
  Details remain authoritative, and a future rollout must classify each module field explicitly.
- Workflow guidance is descriptive, not authoritative. It must remain synchronized with server statuses; future workflows
  should prefer explicit server-provided available-action metadata where contracts already expose it.
- Shell search covers registered navigation destinations, not business entities. Entity search remains bounded to existing
  feature APIs and must not become an unrestricted browser-side index.
- Eleven pages intentionally do not use the ERP workspace header because they are authentication, public/onboarding,
  platform-operator or design-reference surfaces. Their exact allowlist is executable and must be reviewed if paths move.

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

The program is complete only when all applicable implemented modules use the same approved shell, page, table, form, feedback and
workflow standards; common operations require fewer verified interactions; safe page context survives appropriate
navigation; permissions and backend workflow rules remain authoritative; and the complete regression, accessibility,
responsive, RTL/LTR and production-build gates pass. UX-008 satisfies this definition for the approved presentation
scope with 11 executable layout exceptions. UX-009 additionally closes the identified navigation-discovery gap for the three
largest underrepresented modules; production deployment readiness and physical MVC retirement remain separate.
