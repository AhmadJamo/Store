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

### UX-013 — AR/AP aging and subledger reconciliation — COMPLETED 2026-11-06

- [x] Reconstruct open receivables and payables from posted source evidence at a selected date.
- [x] Apply frozen company-currency payments/credits and dated reversal evidence.
- [x] Reconcile complete subledgers to every used control account without display-filter distortion.
- [x] Preserve safe date/search/tab context and source/journal drill-down.

### UX-014 — Cash flow and period-close readiness — COMPLETED 2026-11-06

- [x] Derive opening, movement and closing cash from active payment-method settlement accounts and posted journals.
- [x] Classify operating, investing and financing movement server-side while exposing ambiguous movement for review.
- [x] Add a read-only close checklist for trial balance, balance sheet, AR/AP, inventory, drafts and fiscal-period coverage.
- [x] Preserve safe date/tab context and direct evidence links without adding a close/post/payment mutation.
- [x] Verify Release, accounting/security, Angular, accessibility, consistency, budgets and authenticated Edge acceptance.

### UX-015 — Controlled period-close execution — COMPLETED 2026-11-07

- [x] Enforce Open → SoftClosed → Closed and one-step reopen transitions in the domain.
- [x] Rerun server-authored close readiness inside the close transaction and reject blocking checks.
- [x] Add independent View/SoftClose/Close/Reopen permissions, explicit CSRF-protected endpoints and semantic audit evidence.
- [x] Require a reason and rowversion, preserve user/time metadata, and remove unavailable/repeated actions from Angular.
- [x] Keep automatic closing journals, posted-entry mutation, consolidation, budgets and FX revaluation out of scope.
- [x] Verify Release, 1,872 security/accounting checks, 240 Angular tests, accessibility, bundle budgets and Edge 15/15.

### UX-016 — Recurring journal templates and controlled runs — COMPLETED 2026-11-07

- [x] Add balanced tenant-owned monthly, quarterly and annual templates with named accounts and optional dimensions.
- [x] Generate one separate Draft per explicit run with atomic retry safety and no automatic posting.
- [x] Add independent View/Manage/Run permissions, antiforgery, rowversion and semantic audit evidence.
- [x] Preserve unsaved create context and show next run, last result and generated Draft evidence.
- [x] Keep fiscal periods authoritative at the existing posting gateway; add no background scheduler or closing entry.
- [x] Verify Release, 1,883 security/accounting checks, 242 Angular tests, accessibility, bundle budgets and Edge 15/15.

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
| UX-014 | 240/240 Angular; 1,867 security/accounting checks; 136 templates; production 337,026 B initial and 81,877 B largest lazy | Deterministic classification and repository boundaries; 77 destinations; 46/46 routes; authenticated Edge 15 passed/0 failed | COMPLETE |
| UX-015 | 240/240 Angular; 1,872 security/accounting checks; 136 templates; production 337,026 B initial and 81,877 B largest lazy | Stepwise transitions, readiness blocking, separate permissions/CSRF/audit; authenticated Edge 15 passed/0 failed | COMPLETE |
| UX-016 | 242/242 Angular; 1,883 security/accounting checks; 137 templates; production 337,191 B initial and 81,877 B largest lazy | Balanced templates, idempotent Draft runs, 78 destinations, 47/47 routes; authenticated Edge 15 passed/0 failed | COMPLETE |

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

### UX-014 — Completed

- Added ledger-derived cash opening, movement and closing totals from configured settlement accounts.
- Added transparent operating/investing/financing classification and a separate unclassified review total.
- Added a read-only close-readiness checklist across core accounting and inventory reconciliations with evidence links.
- Preserved date/tab context and kept all posting, payment and fiscal-period mutations outside the surface.
- Full evidence is recorded in docs/modernization/UI_UX_CASH_FLOW_CLOSE.md.

### UX-015 — Completed

- Added explicit soft-close, final-close and one-step reopen commands with independent permissions and CSRF protection.
- The command reruns UX-014 readiness inside the transaction and refuses any blocking control before changing status.
- Reason, actor, time and rowversion remain on the fiscal-period record; a registered semantic audit event records the status delta.
- No automatic close journal, source-document mutation or posted-entry rewrite was introduced.
- Full evidence is recorded in docs/modernization/UI_UX_CONTROLLED_PERIOD_CLOSE.md.

### UX-016 — Completed

- Added balanced tenant-owned recurring templates with explicit monthly, quarterly or annual next-run dates.
- Each authorized run creates a separate reviewable Draft and advances exactly once under rowversion and retry-key control.
- Reused manual-journal validation and the existing posting gateway; no scheduler or automatic posting was introduced.
- Full evidence is recorded in `docs/modernization/UI_UX_RECURRING_JOURNALS.md`.

### Next

- UX-017 bank-statement reconciliation foundation is proposed and requires owner approval before adding statement import
  and matching records. Automatic journal creation and external bank connections remain separate policy decisions.

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

## MOD-030F — MVC retirement dependency inventory — COMPLETED 2026-11-12

- Classified all 411 discovered MVC actions across 82 surfaces as Angular-default presentation candidates, retained
  mutations, downloads/generated files or session/compatibility boundaries.
- Accounted for all 217 Razor files and blocked early view deletion on 54 mixed-controller surfaces.
- Added generated R1..R4 cohorts and made stale retirement evidence fail the unified modernization gate.
- Identified five read-only inventory surfaces as the lowest-risk first deletion pilot; financial and anonymous/public
  R1 surfaces are deliberately deferred.
- Removed no MVC controller, action, view, route or rollback path. Every physical deletion batch remains separately
  approval-gated.
- Verification: deterministic regeneration passed twice; Release built with 0 warnings/errors; 1,883 security/inventory
  checks, 242/242 Angular tests, 137 static templates, the 120-page consistency inventory and production bundle budgets
  passed. Browser and performance gates were intentionally not claimed because MOD-030F changed no runtime or UI.

## MOD-030G — first physical MVC retirement pilot — COMPLETED 2026-11-12

- Removed five read-only inventory MVC controllers and their five Index Razor views.
- Removed their reversible cutover entries and AngularDefault settings so configuration cannot target deleted code.
- Added one reusable, permission-protected legacy GET/HEAD redirect catalog preserving both URL forms and query strings.
- Kept the existing Angular feature components and server-authored APIs unchanged; no business or database rule moved.
- Established deployment rollback as the rollback mechanism after physical deletion.
- Release built cleanly, 1,866 security/inventory checks passed and authenticated Edge acceptance passed 145 routes.

## MOD-030H — financial read-only MVC retirement — COMPLETED 2026-11-12

- Retired `JournalEntries.Details` and `PurchaseAccountingReconciliation.Index` MVC controllers/views after explicit approval.
- Kept the existing versioned API controllers, Angular pages, Application services and financial calculations unchanged.
- Extended the reusable retired-route catalog with exact action mapping and positive journal-ID substitution.
- Preserved reconciliation filters and journal query strings under the original permission policies.
- Active inventory is 75 MVC controllers, 404 actions and 210 Razor files; seven surfaces are retired and Public is the sole remaining R1 surface.
- Release built cleanly, 1,860 security/inventory checks passed and authenticated Edge acceptance passed 146 routes.

## MOD-030I — anonymous Public boundary audit — COMPLETED 2026-11-12

- Audited `Public.Index` and `Public.Pricing`, their exact `/` and `/pricing` routes, Angular/API parity and registration dependencies without deleting or changing runtime code.
- Confirmed that pricing remains server-authored and Angular passes only `planId` into registration, where eligibility is revalidated.
- Classified Public as `R1-A`: the current retired-route catalog requires permission-protected conventional paths and cannot safely represent anonymous root routes unchanged.
- Preserved `_PublicLayout`, Account login/registration, onboarding, culture switching, cookies, rate limiting and the public plans API.
- A later physical retirement is `MOD-030J`, requires separate owner approval and must add exact anonymous-safe route evidence first.

## MOD-030J — anonymous Public physical retirement — COMPLETED 2026-11-12

- Removed `PublicController` and its landing/pricing Razor views after explicit owner approval.
- Replaced only exact `/` and `/pricing` GET/HEAD routes with explicitly anonymous Angular redirects that preserve query values.
- Removed Public from reversible cutover configuration/catalog; rollback is deployment of the previous build.
- Preserved registration, login, plan validation, antiforgery, rate limiting, cookies, onboarding, culture switching, the public plans API and shared `_PublicLayout`.
- Active inventory is 74 MVC controllers, 402 actions and 208 Razor files; nine actions across eight surfaces and nine views are physically retired.
- Hosted route probes and Microsoft Edge acceptance on 146 authorized routes passed.
- Release built with zero warnings/errors; 1,856 security/inventory checks, 242/242 Angular tests, 137 templates,
  120-page consistency and fixed bundle budgets passed.

## MOD-030K — mixed-controller prioritization audit — COMPLETED 2026-11-12

- Added a generated dependency-score ordering for all 54 R2 surfaces; the score is triage evidence, never deletion approval.
- Reviewed the lowest-count AuditEvents, PlatformAuditEvents, LocationMovements and AnalyticsCenter boundaries against actual MVC/API/Angular code.
- Selected tenant `AuditEvents` as the smallest next proposed cohort because Angular/API already preserve filters, sensitive evidence and permission-separated CSRF export.
- Deferred Platform audit until isolated-session live evidence exists and deferred LocationMovements because it performs stock mutation.
- No controller, view, route, permission, API or runtime behavior changed; active counts remain 74/402/208.
- Release, 1,856 security/inventory checks, 242/242 Angular tests, 137 templates, 120-page consistency and bundle budgets passed.

## MOD-030L — tenant AuditEvents MVC retirement — COMPLETED 2026-11-13

- Removed the approved tenant `AuditEventsController` and its Razor view; Application service, API, Angular and permissions remain authoritative.
- Replaced only GET/HEAD `/AuditEvents` and `/AuditEvents/Index` with `AuditEvents.View`-protected Angular redirects that preserve queries.
- Retired legacy MVC `POST /AuditEvents/Export` without redirect; Angular continues through the CSRF-protected API export.
- Active inventory is 73 MVC controllers, 400 actions and 207 Razor files. Ten redirected presentation actions across nine surfaces and ten views are retired; the AuditEvents export is an additional deliberately non-redirected retired action.
- Release, 1,851 security/inventory checks, 242/242 Angular tests, 137 templates, 120-page consistency, bundle budgets and 147-route authenticated Edge acceptance passed.

## MOD-030M — PlatformAuditEvents readiness audit — COMPLETED 2026-11-13

- Confirmed MVC/API/Angular parity and isolated Platform View/Export/Sensitive policy separation without changing runtime code.
- Confirmed that every available stored session is tenant-only and contains no `MiniStore.Platform` cookie.
- Hosted read-only probes returned 401 for tenant/anonymous Platform API access and redirected tenant MVC access to Platform login.
- Deferred physical retirement until an authorized Platform session proves same-scope reads and role separation; no deletion is approved.
- Active inventory remains 73 MVC controllers, 400 actions and 207 Razor files.

## MOD-030N — next mixed-candidate comparison — COMPLETED 2026-11-13

- Compared LocationMovements and AnalyticsCenter against actual MVC/API/Angular, permissions, validation and mutation boundaries.
- Deferred LocationMovements because it moves exact-location stock and requires database-backed idempotency/rejection evidence.
- Selected AnalyticsCenter as the next proposed bounded cohort: its Index, builder and workspace creation have complete governed replacements without stock/accounting effects.
- Hosted read-only probes passed for both APIs and all three legacy GET redirects; no POST or business mutation ran.
- No runtime file changed; active counts remain 73 MVC controllers, 400 actions and 207 Razor files.
