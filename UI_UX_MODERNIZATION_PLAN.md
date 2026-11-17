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

## MOD-030O — AnalyticsCenter MVC retirement — COMPLETED 2026-11-13

- Removed the approved AnalyticsCenter controller and its Index/Create Razor views; service, providers, API, Angular and permissions remain authoritative.
- Added permission-protected GET/HEAD redirects for Index and builder with query preservation.
- Retired legacy MVC workspace POST without redirect; the typed API workspace command remains the only creation path.
- Active inventory is 72 MVC controllers, 397 actions and 205 Razor files with 52 mixed-controller blockers.
- Release, 1,845 regression checks, 242/242 Angular tests, 137 templates, 120-page consistency, bundle budgets and 147-route Edge acceptance passed.

## MOD-030P — next low-count cohort audit — COMPLETED 2026-11-13

- Compared Recipes, StockTransactions, Subscription and Users across MVC, API, Angular, Application services,
  permissions, mutation boundaries and acceptance coverage without changing data or runtime behavior.
- Selected Recipes as the next bounded candidate because Angular/API reuse the same `RecipeService`, preserve
  Products.View/Edit, antiforgery and immutable-version validation, and do not post stock or accounting entries.
- Deferred StockTransactions, Subscription and Users behind inventory-, billing- and identity-specific mutation evidence.
- Proposed `MOD-030Q` for separate approval of the exact Recipes controller/two-view deletion scope.

## MOD-030Q — Recipes MVC retirement — COMPLETED 2026-11-13

- Removed the approved Recipes controller and its Index/Edit Razor views while retaining API, Application service,
  repositories, Angular pages, permissions and immutable recipe behavior.
- Added permission-protected legacy GET/HEAD redirects, including safe positive `productId` query-to-path conversion.
- Invalid identifiers return 404; the legacy MVC save POST returns 405 without mutation.
- Release, 1,842 regression checks, 242/242 Angular tests, bundle gates and 147-route Edge acceptance passed.
- Active inventory is 71 MVC controllers, 394 actions and 203 Razor files.

## MOD-030R — configuration-candidate audit — COMPLETED 2026-11-13

- Compared ProductCategories, PurchaseApprovalRules and SalesApprovalRules without mutation or runtime changes.
- Selected ProductCategories as the next bounded candidate because MVC/API/Angular reuse `ProductCategoryService` and
  preserve Products.View/Edit, antiforgery and server validation without stock, accounting, identity or workflow effects.
- Deferred purchase and sales approval rules because activation directly changes document approval routing.
- Authenticated GET probes returned 200 for all three APIs; the 147-route Edge run covered their legacy redirects.

## MOD-030S — ProductCategories MVC retirement — COMPLETED 2026-11-13

- Removed the approved ProductCategories controller and its Index Razor view.
- Protected legacy GET/HEAD routes now resolve to Angular with query preservation.
- Legacy Create/SetActive POST routes return 405; API POST/PUT retain Products.Edit and antiforgery.
- Release, 1,840 regression checks, 242/242 Angular tests, bundle gates and 147-route Edge acceptance passed.
- Active inventory is 70 MVC controllers, 391 actions and 202 Razor files.

## MOD-030T — approval-rule lifecycle audit — COMPLETED 2026-11-13

- Traced purchase and sales approval rules through MVC/API/Angular, Application services, domain snapshots, repositories,
  permissions, antiforgery, rowversion and document submit/confirm/approve/reject behavior without mutation.
- Confirmed both replacements preserve server authority and existing business rules.
- Selected SalesApprovalRules as the stronger separately approval-gated candidate because it has an isolated Angular
  feature, explicit client contract tests and SQL lifecycle evidence.
- Deferred PurchaseApprovalRules because its UI is coupled to the purchase-request feature and its rule-resolution SQL
  evidence is less isolated. No runtime file or inventory count changed.

## MOD-030U — SalesApprovalRules MVC retirement — COMPLETED 2026-11-13

- Removed the separately approved SalesApprovalRules controller and single Razor view while retaining API, Angular,
  Application/domain authority, permissions, antiforgery, rowversion and order-approval behavior.
- Protected legacy GET/HEAD routes now resolve to Angular with query preservation; legacy Create/SetActive POSTs return
  405 and are never redirected or replayed.
- Release passed cleanly, 1,837 regression checks and 242/242 Angular tests passed, and bundle budgets remained within
  project thresholds. Hosted anonymous boundaries passed; Edge window automation was unavailable due to a Windows helper
  ACL failure, so no authenticated Edge claim is made.
- Active inventory is 69 MVC controllers, 388 actions and 201 Razor files.

## MOD-030V — PurchaseApprovalRules SQL lifecycle evidence — COMPLETED 2026-11-14

- Added a disposable SQL scenario over the real PurchaseApprovalService/repositories/UnitOfWork.
- Proved specific rule resolution, frozen ordered steps, role-gated approve/reject, atomic request/instance transitions,
  rollback on invalid rules, duplicate-instance prevention and stale-rowversion rejection.
- Updated the SQL test harness to current FiscalPeriodService/ProductService contracts without changing production code.
- The complete disposable SQL suite passed; no operational data or runtime behavior changed.
- PurchaseApprovalRules MVC is now eligible only for a separately approved MOD-030W bounded retirement.

## MOD-030W — PurchaseApprovalRules MVC retirement — COMPLETED 2026-11-14

- Removed the approved PurchaseApprovalRules controller and its single Razor view after the isolated SQL lifecycle gate.
- Protected legacy GET/HEAD routes now resolve to `/purchases/approval-rules`; legacy Create/SetActive POSTs return 405.
- API, Angular, `PurchaseApprovalService`, permissions, antiforgery, rowversion and request approval behavior are unchanged.
- Release passed cleanly, 1,833 regression checks and 242/242 Angular tests passed, and bundle budgets remained within limits.
- Hosted boundaries returned 302 for protected legacy reads, 401 for the anonymous API, 200 for Angular and 405 for retired writes.
- Active inventory is 68 MVC controllers, 385 actions and 200 Razor files (182 user-facing views).
- Subsequent presentation-retirement work will be planned and verified as explicit three-controller cohorts.

## MOD-030X — first three-controller purchasing retirement — COMPLETED 2026-11-14

- Retired `GoodsReceiptReturnsController`, `PurchaseServiceAcceptancesController` and `SupplierCreditNotesController` together.
- Removed their nine Razor list/create/details views and active cutover/configuration mappings.
- Preserved all three Angular features, authenticated APIs, Application services, permissions, antiforgery and posting rules.
- Legacy list/create/details GET/HEAD routes now resolve to exact Angular destinations; retired MVC Create POSTs return 405.
- Release passed with zero warnings/errors, 1,803 regression checks, 242/242 Angular tests and bundle/browser gates.
- Active inventory is 65 MVC controllers, 373 actions, 191 Razor files and 173 user-facing views.

## MOD-030Y — first ten-controller cohort audit — COMPLETED 2026-11-14

- Changed future execution size from three to ten controllers per owner direction.
- Selected SupplierPayments, CustomerPayments, PurchaseReplenishment, ProductAttributes, ProductStocks, SalesDeliveries,
  ProductTemplates, Replenishment, Suppliers and InventoryScanning as the first ten-controller cohort.
- Verified each has Angular and authenticated API coverage over the same Application service and permission boundaries.
- Excluded Account, Subscription, Users and Platform controllers because identity/session/platform evidence needs a separate cohort.
- No runtime file, schema or operational data changed; physical execution is recorded as MOD-030Z.

## MOD-030Z — first ten-controller retirement — COMPLETED 2026-11-14

- Retired the ten controllers named by MOD-030Y and removed their 21 user-facing Razor views plus related page-model/partial files.
- Protected legacy GET/HEAD routes now resolve to the exact Angular list/create/detail/edit workspaces.
- Retained authenticated APIs, Application services, permissions, antiforgery and all accounting/inventory/domain authority.
- Release passed cleanly with 1,725 regression checks, 242/242 Angular tests and bundle/browser gates.
- Active inventory is 55 MVC controllers, 324 actions, 170 Razor files and 152 user-facing views.

## MOD-031B — inventory and warehouse ten-controller retirement — COMPLETED 2026-11-14

- Retired LocationMovements, StockTransactions, UnassignedStock, InventoryAdjustments, InventoryTracking,
  WarehouseLocations, Warehouses, StockTransfers, GoodsReceipts and PurchaseDocumentReversals.
- Removed ten MVC controllers, 25 user-facing Razor views and ten page-model companions.
- Added 25 permission-preserving retired GET/HEAD routes; legacy mutations return 405 and are never replayed.
- Preserved authenticated Angular APIs, Application services, antiforgery, UnitOfWork and inventory/accounting authority.
- Release passed cleanly with 1,656 regression checks, 242/242 Angular tests and bundle/browser gates.
- Active inventory is 45 MVC controllers, 263 actions, 143 Razor files and 127 user-facing views.

## UX-017B — bank-statement evidence foundation — COMPLETED 2026-11-14

- Added tenant bank accounts and immutable CSV-import evidence with SHA-256 duplicate fingerprints; raw files are not retained.
- Added `BankStatements.View` / `BankStatements.Import`, a CSRF-protected API and one cohesive bilingual Angular workspace.
- Matching, unmatching, adjustments, posting and close-readiness remain UX-017C/D work.

## UX-017C — controlled partial matching — COMPLETED 2026-11-15

- Added allocation persistence, remaining-balance calculations and a statement-line capacity guard.
- Added posted same-account/same-direction journal-line candidates, atomic partial Match, reasoned Unmatch, rowversion, semantic audit, independent permissions and a cohesive Angular workflow.
- Release, Angular build, 243 Angular tests and 1,609 security checks passed.

## UX-017D — close integration and controlled bank adjustments — COMPLETED 2026-11-15

- Period-close readiness now blocks on every statement line with an unreconciled amount through the server-authored close checklist.
- Added an explicit, independently permissioned command for bank fees, interest and bank-to-bank transfers. It posts only through `JournalPostingService`, therefore retaining fiscal-period, numbering and balanced-entry authority.
- The command validates rowversion, remaining amount, transaction date, active named counterpart and adjustment-specific account type; journal creation, allocation and semantic audit evidence commit atomically.
- Adjustment account options are exposed only under `BankStatements.Adjust`. Suggestions still never create or post journals automatically.
- Release and Angular builds passed, together with 243/243 Angular tests and 1,609 security checks.

## ERP-001 — enterprise gap audit and named bank-ledger selector — IMPLEMENTED 2026-11-15

- Audited branch, transfer, POS cash, shared UX, error, accounting and test boundaries against actual source.
- Recorded the evidence matrix and risk-ordered vertical-slice backlog in `docs/modernization/ENTERPRISE_ERP_GAP_ANALYSIS_2026-11-15.md`.
- Replaced manual ledger-account ID entry in bank-account setup with permission-scoped active Asset options by code/name.
- Added server-side active Asset validation and focused Angular/application/security coverage without schema, journal or inventory mutation.

## MOD-031C — fifteen-controller commercial retirement — COMPLETED 2026-11-14

- Retired 15 MVC controllers and 39 Razor/page-model/partial files.
- Preserved Angular, APIs, Application/domain authority and 34 protected legacy reads; writes return 405.
- Inventory is 30 MVC surfaces, 214 actions, 100 Razor rows and 93 views.
- Release passed with zero warnings/errors, 1,638 checks, 242/242 Angular tests and bundle/browser gates.

## MOD-031C — fifteen-controller commercial retirement — COMPLETED 2026-11-14

- Retired 15 MVC controllers spanning catalog masters, products, purchasing and sales returns/credits.
- Removed 39 Razor/page-model/partial files while preserving Angular, APIs and Application/domain authority.
- Added 34 protected legacy GET/HEAD routes; writes return 405 and supplier attachment downloads remain API-only.
- Inventory is 30 MVC surfaces, 214 actions, 100 Razor rows and 93 views.
- Release passed with zero warnings/errors, 1,638 regression checks, 242/242 Angular tests and bundle/browser gates.

## ERP-002 — Branch workspace and context policy — COMPLETED 2026-11-15

- Added one cohesive /branches Angular feature instead of fragmenting branch, warehouse and POS summaries.
- Added bilingual responsive branch cards, operational counts and links to existing inventory-access and branch-account settings.
- The page is read-only and permission-filtered; it does not store a selected branch or mistake browser state for authorization.
- Preserved persistent workspace navigation and reused shared recovery and page-header patterns.
- The accepted ADR reserves user branch grants and server request context for ERP-003 before any branch switcher.

## ERP-003 — Granular branch access and verified context — COMPLETED 2026-11-16

- Replaced Administration.Access on the Branches destination with independent Branches.View and Branches.ManageAccess permissions.
- Added grant administration inside the cohesive Branches page without splitting it into element-level components.
- Ordinary users receive only granted branch cards; management controls render only with the independent permission.
- Added verified request context but deliberately deferred a visible branch switcher to ERP-004.

## ERP-004 — Permitted branch switcher and operational workspace — COMPLETE 2026-11-16

- Added a persistent shell selector using only active branches returned by the server for the current user.
- Scoped the saved ID by tenant and user, rejected stale values and limited header propagation to internal APIs.
- Protected dirty tabs before switching and reloaded the workspace to avoid cross-branch screen state.
- Highlighted the active branch and added permission-aware operational shortcuts without changing business authorization.
- Verified 249 Angular tests, production build, Release, 1,619 security checks and UI/UX consistency.
- Preserved reusable recovery, workspace navigation, bilingual RTL/LTR layout and server authorization authority.

## MOD-031D — fifteen-surface operational retirement — COMPLETED 2026-11-14

- Retired 15 operational MVC controllers and 48 user-facing Razor views plus supporting page models/partials.
- Added 47 protected compatibility reads; retired writes return 405 and downloads remain API-only.
- Preserved Angular/API and all Application, UnitOfWork, accounting, inventory and workflow authority.
- Inventory is now 15 MVC surfaces, 91 actions, 50 Razor rows and 45 views.
- Release passed with zero warnings/errors, 1,605 checks, 242/242 Angular tests and bundle/browser gates.

## MOD-031E — final presentation-boundary audit — COMPLETED 2026-11-14

- Retired the Users and Roles MVC controllers and 11 Razor/page-model/partial artifacts after proving complete Angular/API parity.
- Added five permission-protected compatibility reads for list/create/edit; retired POST mutations are not redirected.
- The generated registry now reports 0 remaining ordinary presentation surfaces, 13 classified operational boundaries,
  82 MVC actions, 44 Razor control rows and 40 views.
- Account/session, onboarding, Settings download, Subscription, Supplier Portal, Language POST and Platform boundaries remain
  intentionally present pending separate live-session or public-token evidence; they are not modernization omissions.
- Release passed with zero warnings/errors, 1,601 regression checks, 242/242 Angular tests and bundle/browser gates.

## MOD-031F — retained runtime-boundary verification — COMPLETED 2026-11-14

- Classified and live-probed all 13 retained boundaries after the ordinary presentation count reached zero.
- Proved distinct tenant and Platform cookie challenges, safe invalid supplier-token presentation, and CSRF rejection.
- Recorded the closure decision in `docs/modernization/RETAINED_RUNTIME_BOUNDARIES.md`; no runtime deletion was made.

## MOD-031G — zero MVC-presentation drift gate — COMPLETED 2026-11-14

- Added `Test-MvcPresentationInvariant.ps1` to require zero ordinary MVC presentation surfaces.
- Allowlisted exactly the 13 reviewed runtime boundaries and asserted Language remains POST-only.
- Integrated the invariant into the standard modernization gate; no runtime route or business behavior changed.

## MOD-031H — MVC retirement stream closure — COMPLETED 2026-11-14

- Reconciled current project, architecture, security and testing documentation with the generated final baseline.
- Closed ordinary MVC presentation retirement at zero surfaces; 13 reviewed server runtime boundaries remain intentional.
- Future work returns to feature delivery; no further controller batch deletion is implied by the modernization plan.

## UX-017A — bank-reconciliation readiness audit — COMPLETED 2026-11-14

- Audited payment methods, settlement accounts, customer/supplier references, journal authority, periods and close readiness.
- Found no current bank-account, statement/import, immutable line or partial-allocation model.
- Defined UX-017B as the smallest safe foundation: bounded CSV evidence and read-only journal candidates, without matching,
  journal creation, adjustments or close-readiness mutation.

## UX-017B — bank-statement evidence foundation — COMPLETED 2026-11-14

- Added tenant bank accounts and immutable CSV-import evidence with SHA-256 duplicate fingerprints; raw files are not retained.
- Added `BankStatements.View` / `BankStatements.Import`, a CSRF-protected API and one cohesive bilingual Angular workspace.
- Matching, unmatching, adjustments, posting and close-readiness remain UX-017C/D work.

## MOD-031C — fifteen-controller commercial retirement — COMPLETED 2026-11-14

- Retired 15 MVC controllers spanning catalog masters, products, purchasing and sales returns/credits.
- Removed 39 Razor/page-model/partial files while preserving Angular, APIs and Application/domain authority.
- Added 34 protected legacy GET/HEAD routes; writes return 405 and supplier attachment downloads remain API-only.
- Inventory is 30 MVC surfaces, 214 actions, 100 Razor rows and 93 views.
- Release passed with zero warnings/errors, 1,638 regression checks, 242/242 Angular tests and bundle/browser gates.
## ERP-005/006 — Staged transfer receipt workspace — COMPLETE 2026-11-16

- Replaced the one-click new-transfer presentation with explicit dispatch and destination receipt actions.
- Added bilingual partial receipt inputs, remaining quantities and visible shortage/damage evidence in the cohesive Stock Transfers feature.
- Retained server authority for permissions, branch context, quantities, valuation, tracking, locations and reservation consumption.
- Verified 249 Angular tests, production build, Release, 1,625 security checks, EF consistency and UI/UX consistency.

## ERP-007 — POS cash policy and domain foundation — COMPLETE 2026-11-16

- Accepted the drawer exclusivity, blind-close, variance threshold and independent approval policy.
- Added server-side foundation only; no premature POS controls or accounting behavior were exposed.
- ERP-008 will add the cohesive operator/manager workflow using the existing feature-based Angular structure.

## ERP-008 — operational POS cash sessions — COMPLETE 2026-11-16

Added the complete operator/manager cash workflow to the existing cohesive sales feature without granular component splitting. Named choices, bilingual RTL/LTR behavior, branch context and persistent workspace behavior are preserved. Release, 1,645 checks, 250 Angular tests, production build and EF consistency passed. ERP progress: 8/10; ERP-009 is next.

## ERP-009 — touch POS and terminal-profile presentation — IMPLEMENTED / ACCEPTANCE PENDING 2026-11-16

The cohesive sales feature now honors all existing terminal presentation settings and adds fast exact scan/search and quantity controls without component over-fragmentation or business-rule changes. Automated gates pass: Release, 1,649 checks, 250 Angular tests and production build. Risk: authenticated Edge desktop/mobile/RTL acceptance is pending due to a Windows automation ACL helper failure. ERP plan remains 8/10 complete until that evidence closes.

## ERP-009 acceptance closure — COMPLETE 2026-11-16

Authenticated Edge passed 15/15 across RTL/LTR, keyboard, desktop/390px responsiveness, live reconciliation, session boundaries, retired-route protection and five high-risk cohorts. ERP modernization progress is 9/10; ERP-010 is the final planned slice.

## ERP-010 — branch/company operational dashboard — COMPLETED 2026-11-16

- Extended the cohesive Branches Angular feature; no granular wrapper components were introduced.
- Added server-owned, permitted-branch projections for period sales, stock quantity/value, open/exception transfers and POS cash close exceptions.
- Added the independent `Branches.ViewOperations` permission; users without it still retain the configuration-only branch workspace.
- Metric definitions: sales are invoices created in the selected UTC period and attributed by POS terminal, otherwise by the highest-priority branch/warehouse link; stock is the current on-hand quantity/value; transfer exceptions are receipt-exception documents; cash exceptions are sessions awaiting approval and recorded variance.
- Company totals mean the caller's permitted branches, not unrestricted tenant totals. Angular renders returned totals and does not recalculate financial authority.
- Verified Release with zero warnings/errors, 1,649 protection checks, 250 Angular tests and production build. Existing initial-chunk warning remains 24.88 kB.

ERP expansion roadmap: 10/10 planned phases complete. Future work is backlog-driven hardening, not an unverified completion claim for every possible ERP capability.

## V3 REC-001 — Branch lifecycle management — COMPLETED 2026-11-17

- Kept one primary feature component for the MVC/API-to-Angular branch flow; no granular wrapper-component split.
- Added permission-aware create, edit and active-status actions with a compact inline form, retained values on API failure, invalid-field focus and bilingual RTL-safe styling.
- Moved branch-only messages into feature-owned localization and externalized page styling to satisfy modernization policy.
- Verification: Release clean, SecurityRegression 1,650, Angular 250/250, production build passed, lazy budget 81,895/81,920 bytes.
- Browser acceptance is explicitly not claimed because the shared fixture failed earlier at stale selector `#preview-title`.
- Next phase: REC-002 warehouse delete-safety evidence and creation/edit discoverability review.
## V3 REC-002 — Warehouse delete safety — COMPLETED 2026-11-17

- Confirmed `/warehouses` already provides cohesive create/edit and named branch/account selection; no duplicate component flow was introduced.
- Converted relational rejection of a referenced warehouse delete into localized HTTP 409 ProblemDetails without exposing provider details.
- Preserved the visible warehouse row and error context after rejection; added Angular regression coverage.
- SQL integration proves referenced warehouse/stock preservation and successful deletion of an unused warehouse.
- Verification: Release clean, InventorySqlIntegration passed, Angular 251/251, SecurityRegression 1,650.
- Next phase: REC-003 cross-layer route/function inventory, beginning with Accounting, Sales and Inventory.
## V3 REC-003 — Cross-layer route/function inventory — COMPLETED 2026-11-17

- Added a source-generated matrix for 196 unique Angular screen routes, including component, API contract, Controller, Application service, permission evidence and adjacent Angular test.
- Added a repeatable gate that rejects unresolved P1 wiring and requires Accounting, Sales and Inventory coverage.
- Results: 0 unresolved P1; 7 P2 manual authorization reviews; Accounting 11 routes, Sales 37, Inventory 22.
- Visibility is not treated as functional completion; the matrix explicitly separates wiring evidence from business-rule and browser evidence.
- Next phase: REC-004 ProblemDetails and business-conflict normalization.
## V3 REC-004A — Operation error toast layer — COMPLETED 2026-11-17

- Added one reusable shell-level toast outlet and bounded signal-backed service; no feature-level wrapper proliferation.
- Failed non-GET operations show concise server-localized title/detail and support reference.
- Named field validation stays inside the form; GET recovery, retries and unsaved values remain feature-owned; 401 remains with the authentication flow.
- Toasts are RTL-safe, keyboard-dismissible, responsive, limited to three and auto-expire.
- Verification: Angular 256/256, production build passed, initial 340,416/358,400 bytes, lazy 81,895/81,920 bytes.
- REC-004 continues with server ProblemDetails classification.
## REC-004B1 — Stable business-conflict classification (2026-11-17)

- Added the stable `conflict` ProblemDetails code for non-concurrency HTTP 409 operations.
- Product and warehouse delete restrictions plus supplier integration, communication and portal state conflicts now use `conflict`; field validation remains HTTP 400 and optimistic concurrency remains `concurrency`.
- Angular no longer treats every untyped 409 as optimistic concurrency. Mutation ProblemDetails continue through the bounded toast while forms preserve entered values.
- Verified: Release solution build 0/0, 258 Angular tests, 1,652 regression checks, production build and bundle gates (340,433/358,400 initial; 81,895/81,920 lazy).
- Remaining REC-004 work: classify the broader InvalidOperationException inventory into typed validation, duplicate, not-found and conflict responses without message guessing or business-rule changes.
## REC-004B2 — Typed accounting and sales problems (2026-11-17)

- Added reusable Application-layer problem semantics with one Web ProblemDetails mapper; no controller parses exception messages.
- Manual-journal missing resources now return 404 and invalid state/replay conflicts return 409, while line/date/balance validation remains 400 and preserves the draft.
- Sales-order missing quotation/order returns 404, stale rowversion returns `409/concurrency`, posted-delivery cancellation returns `409/conflict`, and authorization rejection returns 403.
- Verified: Release 0/0, 1,656 regression checks, 258 Angular tests, production build and fixed bundle gates.
- Next: migrate the next confirmed Inventory/Purchasing cohort and eliminate remaining ambiguous controller catches incrementally.
## REC-004B3 — Inventory transfer and purchase-request problems (2026-11-17)

- Stock-transfer APIs now distinguish missing transfer/warehouse/product/location resources (404), duplicate replenishment transfers (`409/duplicate`) and stock, capacity, branch or reversal-state conflicts (`409/conflict`). Input structure validation remains 400.
- Purchase requests now distinguish missing master data/request (404), duplicate replenishment demand (`409/duplicate`), stale rowversion (`409/concurrency`) and changed master-data state (`409/conflict`).
- The services select semantics through typed Application exceptions; controllers only use the shared mapper and preserve Angular forms.
- Verified: Release 0/0, 1,658 regression checks, 258 Angular tests, production build and bundle gates.
- Remaining inventory baseline: 70 API controllers still catch `InvalidOperationException`; 959 legacy Application throw sites remain versus 51 typed sites. Continue by bounded functional cohorts, not a blind bulk replacement.