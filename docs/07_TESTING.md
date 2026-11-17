# Testing

## ERP-003 branch-access evidence — 2026-11-16

- Release build passed with zero warnings/errors and Angular production build passed with the existing 204.70 KB warning.
- Angular passed 245/245 tests across 118 files, including workspace and grant-management API verbs/payloads.
- SecurityRegression passed 1,619 checks. It rejected the initial non-composite grant/branch relationship, then passed after (BranchId, TenantId) enforcement.
- The local database accepted migration 20261116002333_AddErp003TenantUserBranchScope and EF reports no pending model changes.
- UI/UX consistency inventory remains required in the final gate.

## ERP-002 branch-workspace evidence — 2026-11-15

- Release solution build passed with zero warnings and zero errors.
- Angular passed 245/245 tests across 118 files, including the exact read-only /api/v1/branches contract.
- SecurityRegression passed 1,613 checks, including authentication plus Administration.Access on the branch endpoint.
- Production Angular build passed; the pre-existing initial-chunk warning remains 204.70 KB against 180 KB.
- EF reports no pending model changes because ERP-002 adds no schema.

## UX-017D verification — 2026-11-15

- Release build passed with zero warnings and zero errors.
- Angular production build passed; the existing 204.70 kB initial shared-chunk budget warning remains unchanged.
- Angular unit tests passed: 243/243 across 117 files.
- Security and inventory regression passed: 1,609 checks.

## ERP-001 verification — 2026-11-15

- Release build passed with zero warnings and errors.
- Angular tests passed: 244/244 across 117 files, preserving reconciliation coverage and adding named bank-ledger selection/request payload coverage.
- Security/application regression passed: 1,612 checks, including endpoint permission, active Asset projection and server rejection of a non-Asset selection.
- Angular production and UI/UX consistency builds passed. The existing 204.70 kB shared-chunk warning remains unchanged.

> Status: FOCUSED REGRESSION EXECUTABLE IMPLEMENTED
> Source of truth: `tests/SecurityRegression` and build output
> Last reviewed: 2026-11-14

`tests/SecurityRegression` is a standalone executable test harness. It verifies tenant metadata/query filters and write guards; complete classification of mapped Domain entities; composite TenantId coverage; authorization; destructive action HTTP methods; throttling; SaaS and inventory invariants. The 2026-10-21 run passes 932 checks, including customer-invoice/payment permissions, tenant ownership, receipt evidence, reversal, actionable links and next-step guidance.

`tests/InventorySqlIntegration` creates and deletes a process-specific disposable SQL Server database and applies the full migration chain. Customer coverage proves tenant-local code uniqueness, cross-tenant code reuse and read isolation, archive visibility rules and SQL rowversion stale-update rejection. SAL-020B coverage proves price-list tenant isolation, same-code reuse across tenants, customer cross-tenant foreign-key rejection and database rejection of equivalent rules. Audit coverage includes disabled legacy writes with continuing semantic events, enabled rollback adding exactly one future legacy row, rollback/isolation/append-only/integrity, reader/export boundaries, SQL role-member impersonation, legal-hold exclusion and release, signed checkpoint plus manifest-before-delete execution, and omission of expired request evidence from destruction files. Override its local connection with `MINISTORE_WMS_TEST_CONNECTION`; `--inspect-audit` performs repeatable read-only schema, count, catalog, observed-event and unexplained-gap reconciliation.

PUR-240 regression coverage validates forecast-rule horizons and seasonality state, automation schedule advancement, dedicated controller permissions, tenant classification/relationships and the unique tenant/scheduled-time idempotency index. EF migration drift must also report no pending model changes.

PUR-250A regression coverage validates delivered versus accepted quantities, rejected/damaged/short evidence, no stock for full rejection, route persistence, state transitions, separate controller permissions, active staged-receipt uniqueness and inspection/storage next-step links. The Debug EF design-time check reports no pending model changes; the locally running Release host held an older output copy, so migration drift was verified against the fresh Debug build.

SAL-060 SQL coverage proves partial delivery and inventory/COGS effects; SAL-070 proves partial invoice eligibility; SAL-080 proves receivable/revenue posting and exact reversal. SAL-090 posts a partial customer receipt, proves the derived paid/outstanding balance and balanced settlement journal, reverses it and proves the invoice reopens. The complete disposable SQL suite passed on 2026-10-21, the Release Web build completed with zero warnings/errors, migration 126 is applied to Demo and EF reports no pending model changes.

The 2026-10-13 PUR-220 run built the SQL integration project successfully, but execution stopped before database creation because the configured SQL Server Windows connection could not generate an SSPI context. The mixed-source domain/model regression and EF pending-model check passed; the database-backed flow must be rerun when that local authentication issue is resolved.

The same executable accepts `--inspect-current` for a read-only tenant-by-tenant summary of the current development database, with `MINISTORE_WMS_INSPECT_CONNECTION` as an optional override. The 2026-09-30 run found five explained demo-data exceptions caused by legacy zero-valued movement snapshots; it performed no writes.

The 2026-09-15 Release run passed 167 checks, and `dotnet build MiniStore.slnx -c Release --no-restore` completed with zero warnings/errors.

`tests/PerformanceSmoke` is a dependency-free HTTP workload. The local gate runs 20 warmups followed by 500 requests at concurrency 20 against `/health/live`, fails above p95 250 ms or p99 500 ms and writes `artifacts/verification/performance.json`. The final run completed all 500 requests with zero failures, p95 16.85 ms and p99 28.35 ms. This is an availability regression baseline, not a production-sizing or business-transaction claim.

Run the complete owner-approved local gate with `pwsh -NoProfile -File scripts/verify-local.ps1`. It builds Release, runs the focused executable, uses a disposable SQL database, starts the application, requires healthy readiness and then runs the workload. Remaining gaps include full HTTP authentication/authorization journeys, business-transaction/soak workloads, external payment webhook tests, production monitoring and penetration testing.

## Angular migration acceptance

The current MOD-031H baseline is zero ordinary MVC presentation surfaces, 13 allowlisted runtime boundaries and Language
as the sole mutation-only compatibility endpoint. `verify-angular-modernization.ps1` regenerates the inventory, rejects
stale evidence, runs `Test-MvcPresentationInvariant.ps1`, builds Release, executes 1,601 focused checks, runs 242 Angular
tests, enforces accessibility/consistency/bundle budgets and performs browser state/direction/keyboard/mobile acceptance.
Environment-dependent authenticated API checks remain explicitly skipped unless an external storage-state is supplied.

Run `pwsh -NoProfile -File scripts/verify-angular-modernization.ps1` before an Angular cutover increment. The command
writes one ignored JSON report under `artifacts/verification/angular-acceptance`, fails on real build, regression,
style, accessibility, bundle or browser failures, and records unavailable environment-dependent evidence as skipped.
Its Playwright pass covers RTL/LTR, keyboard skip focus, representative loading/empty/error states and a 390px viewport.
Pass `-ApiUrl` for the real anonymous session and bounded performance checks; pass an external
`-AuthenticatedStorageState` file for the authorized session plus allowlisted read probes. Never commit that file.

The final 2026-10-25 MOD-018B run passed eleven outer results with zero failures and zero skips. The browser passed four
UI checks plus the real anonymous 401 session contract, the authenticated session and three permitted read probes.
Initial bundle size was 290,987/358,400 bytes and the largest lazy chunk was 52,866/81,920 bytes. The 100-request,
concurrency-10 live availability smoke had zero failures, p95 40.37 ms and p99 61.85 ms. The interactive wrapper deleted
its temporary storage state after completion; credentials and cookies were not written to the repository or report.

MOD-019 raised the focused regression total to 1113. Its resolver checks prove that MVC and pilot modes do not redirect,
Angular-default GET preserves a query string and resolves allowlisted route values, POST never redirects, and an
unlisted action remains MVC. Regenerate `MVC_MIGRATION_REGISTRY.md` before reviewing migration coverage.

MOD-020A..L pass 85/85 Angular tests across 25 files. Coverage verifies preferences, identity/custom fields, logo
upload/delete, discount and purchase-matching mutations, including submitted/refreshed rowversions, clean dirty state
after success and retained input after rejection. Fiscal-period coverage proves GET, POST create and reasoned status PUT,
including the current rowversion and preserved drafts after rejection. Currency coverage proves GET/POST upsert,
client normalization, refreshed results and preserved values after validation rejection. Security regression passes
Measurement-unit coverage proves GET/create/deactivate, full field submission, system-unit protection and retained
failed create input. Security regression passes 1144 checks and fixes every settings
route to its intended GET/PUT/POST/DELETE verb under authenticated `Administration.Access`. Web Release and Angular
production builds pass; 34 templates pass static accessibility. The production initial bundle is 293,121/358,400
bytes and the largest lazy chunk is 54,668/81,920 bytes.

## Guided onboarding verification (updated 2026-10-27)
Regression coverage checks pending/completed/skipped state, normalization, one-time resolution, invalid currency and rowversion. A local HTTP journey registered a company, reached /onboarding, applied the Cafe/tax/POS template, landed on the dashboard and authenticated again by company code. SQL found 18 accounts, one warehouse, one POS and one tax rate for the test tenant.

`MOD-020U` adds API metadata checks for anonymous/versioned registration, CSRF, the independent registration rate limit, required secret/slug validation, the authenticated tenant-Admin onboarding boundary and antiforgery on both Complete and Skip. Angular tests cover every registration/onboarding endpoint, rejected-password clearing with non-secret field preservation, automatic session reload/routing, the complete payload and confirmed skip. Final evidence is 1182 backend checks, 122/122 Angular tests across 37 files, 50 accessible templates, external SCSS, and production bundles of 297,465 initial / 80,866 largest lazy bytes.

Cafe recipe, product-catalog and managed-unit coverage validates conversion, classification, channel rules, immutable snapshots, controlled negatives and Arabic decimal handling. Moving-average coverage verifies `10@5 + 20@8 = 7`, a 4-unit issue at value 28, remaining value 182, and provisional-negative variance settlement. Posting coverage verifies immutable line COGS, balanced sale and cost-variance journals, inclusive/exclusive tax and balanced return reversals. The current executable passes 260 checks; EF reports no pending model changes after `AddProductLogisticsFoundation`.
## MOD-021A product-master evidence (2026-10-27)

- Web Release passed with zero warnings/errors.
- SecurityRegression passed 1186 checks, including product form permissions, independent mutation permissions,
  antiforgery metadata and the Application-owned form contract.
- Angular passed 127/127 tests across 38 files; API tests cover GET/form/POST/PUT/DELETE and component tests cover
  successful create/update, retained validation input and raw/prepared product rules.
- 52 Angular templates passed static accessibility; external SCSS and production bundle budgets passed.
- Bundle evidence: 297,865/358,400 initial bytes and 80,866/81,920 largest lazy bytes.

## MOD-021B product-category evidence (2026-10-27)

- Web Release passed with zero warnings/errors; SecurityRegression passed 1189 checks.
- Angular passed 131/131 tests across 40 files, including category GET/POST/status PUT, duplicate-input retention and
  confirmed status changes.
- 53 templates passed static accessibility; external SCSS passed.
- Bundle evidence: 298,021/358,400 initial bytes and 80,866/81,920 largest lazy bytes.

## MOD-021C product-attribute evidence (2026-10-27)

- Web Release passed with zero warnings/errors; SecurityRegression passed 1192 checks.
- Angular passed 137/137 tests across 43 files. Coverage includes definition GET/POST/status PUT, per-product value
  GET/PUT, normalization, category selection, status confirmation, typed value save and rejected-input preservation.
- 55 templates passed static accessibility; external SCSS passed.
- Bundle evidence: 298,342/358,400 initial bytes and 80,866/81,920 largest lazy bytes.

## MOD-021D product-template evidence (2026-10-27)

- Web Release passed with zero warnings/errors; SecurityRegression passed 1195 checks.
- Angular passed 141/141 tests across 45 files, including all GET/create/assignment/preview/generate requests, normalized
  template creation, assignment and rejected-form retention.
- 56 templates passed static accessibility; external SCSS passed.
- Bundle evidence: 298,693/358,400 initial bytes and 80,866/81,920 largest lazy bytes.

## MOD-021E recipe evidence (2026-10-27)

- Web Release passed with zero warnings/errors; SecurityRegression passed 1198 checks.
- Angular passed 144/144 tests across 47 files, including recipe list/editor/save endpoints, complete immutable-version
  payload and rejected-row preservation.
- 58 templates passed static accessibility; external SCSS passed.
- Bundle evidence: 298,961/358,400 initial bytes and 80,866/81,920 largest lazy bytes.

## MOD-021F catalog-master evidence (2026-10-27)

- Web Release passed with zero warnings/errors; SecurityRegression passed 1201 checks.
- Angular passed 147/147 tests across 49 files, including GET and all three create requests, normalized branch code and
  rejected-input retention.
- 59 templates passed static accessibility; external SCSS passed.
- Bundle evidence: 299,119/358,400 initial bytes and 80,866/81,920 largest lazy bytes.

## MOD-021G warehouse evidence (2026-10-27)

- Web Release passed with zero warnings/errors; SecurityRegression passed 1204 checks.
- Angular passed 150/150 tests across 51 files, including complete GET/form/POST/PUT/DELETE contracts and rejected-edit
  preservation.
- 60 templates passed static accessibility; external SCSS passed.
- Bundle evidence: 299,259/358,400 initial bytes and 80,866/81,920 largest lazy bytes.

## MOD-022A inventory-ledger evidence (2026-10-27)

- Web Release passed with zero warnings/errors; SecurityRegression passed 1206 checks, including authenticated route,
  `StockTransactions.View` and read-only endpoint assertions.
- Angular passed 151/151 tests across 52 files; the API test covers search, type, movement, date, paging and sorting query values.
- 61 templates passed static accessibility; external SCSS passed.
- Bundle evidence: 302,834/358,400 initial bytes and 80,941/81,920 largest lazy bytes.
- Live Angular route returned 200 and anonymous API access returned 401.

## MOD-022B product-stock operations evidence (2026-10-27)

- Web Release passed with zero warnings/errors; SecurityRegression passed 1210 checks covering route authorization, operation
  permissions, antiforgery and invariant quantity validation.
- Angular passed 155/155 tests across 54 files, including typed endpoints, opening stock, edit rejection recovery and manual adjustment.
- Static accessibility passed 62 templates and the external-SCSS policy passed.
- Production build passed at 302,983/358,400 initial bytes and 81,017/81,920 largest lazy bytes.
- Live `/inventory/stocks` returned 200; anonymous ProductStock and manual-adjustment API calls returned 401.

## MOD-022C availability and reservation evidence (2026-10-27)

- Web Release passed with zero warnings/errors; isolated SecurityRegression passed 1213 checks covering both authenticated routes,
  their independent View permissions and Application-owned paging.
- Angular passed 159/159 tests across 57 files, including query serialization and page-level warehouse/status filtering.
- Static accessibility passed 64 templates and the external-SCSS policy passed.
- Production build passed at 303,235/358,400 initial bytes and 81,211/81,920 largest lazy bytes.
- Both live Angular routes returned 200; anonymous availability and reservation API calls returned 401.

## MOD-022D inventory-adjustment evidence (2026-10-27)

- Web Release passed with zero warnings/errors; isolated SecurityRegression passed 1216 checks covering authenticated routes, all
  six operation permissions, antiforgery and invariant count/named-option DTO contracts.
- Angular passed 161/161 tests across 59 files, including the complete typed API workflow and full-line count/action separation.
- Static accessibility passed 67 templates and the external-SCSS policy passed.
- Production build passed at 303,702/358,400 initial bytes and 81,296/81,920 largest lazy bytes.
- Live adjustment list/create routes returned 200; anonymous list/create-form API calls returned 401.

## ERP-002 branch-workspace evidence (2026-11-15)

- Release solution build passed with zero warnings and zero errors.
- Angular passed 245/245 tests across 118 files, including the exact read-only /api/v1/branches contract.
- SecurityRegression passed 1,613 checks, including authentication plus Administration.Access on the branch endpoint.
- Production Angular build passed; the pre-existing initial-chunk warning remains 204.70 KB against 180 KB.
- EF reports no pending model changes because ERP-002 adds no schema.

## ERP-004 verification (2026-11-16)

- Angular: 120 files and 249 tests passed, including tenant/user-scoped restoration, stale-choice rejection and internal-only header behavior.
- Angular production build passed; the existing 204.70 kB shared-chunk warning is unchanged.
- Release build passed with zero warnings/errors.
- SecurityRegression passed 1,619 checks; UI/UX consistency passed for 121 feature pages and 80 navigation items.
## ERP-005/006 verification (2026-11-16)

- Release build passed with zero warnings/errors.
- Angular passed 249 tests across 120 files and the production build passed with the unchanged shared-chunk warning.
- SecurityRegression passed 1,625 checks, including independent dispatch/receive permissions, antiforgery, partial receipt, discrepancy and completion invariants.
- EF reports no pending model changes; migrations ERP-003 and 20261116012206 were applied successfully to the local development database.
- UI/UX consistency passed for 121 feature pages and 80 navigation items.


## ERP-007 verification (2026-11-16)

- Release build passed with zero warnings/errors.
- SecurityRegression passed 1,641 checks, including tenant isolation, signed expected cash, blind count, threshold escalation and self-approval rejection.
- EF reports no pending model changes after migration `20261116113058_AddErp007PosCashSessionFoundation`.
- No Angular test count changed because ERP-007 deliberately adds no UI or API.

## ERP-008 verification — 2026-11-16

Web Release passed with zero warnings/errors; 1,645 security checks and 250/250 Angular tests passed; the production bundle built (with the existing initial-chunk warning); EF reported no pending model changes; and migration `20261116123034_AddErp008PosCashOperations` applied successfully.

## ERP-009 verification — 2026-11-16

Release passed with zero warnings/errors, 1,649 regression checks passed, 250/250 Angular tests passed and production build passed with the existing initial-chunk warning. Authenticated Microsoft Edge acceptance is pending: the Windows automation kernel exited on an ACL helper error before a browser window could be selected; no visual acceptance is claimed.

## Browser-session capture hardening — 2026-11-16

The Edge acceptance capture now saves storage state only after `/api/v1/session` returns success. A presentation redirect away from MVC login no longer produces a false authenticated fixture. The project builds cleanly; the ERP-009 interactive attempt timed out without login, so hosted acceptance remains open.

## ERP-009 acceptance closure — 2026-11-16

Authenticated Microsoft Edge passed 15/15: UI states, RTL/LTR, keyboard skip navigation, desktop/390px responsiveness, Angular/API inventory and account reconciliation, anonymous/authenticated session boundaries, retired Accounts presentation redirect and five high-risk read cohorts. No failures or skips remained.

### ERP-010 verification (2026-11-16)

Release build passed with zero warnings/errors; security/inventory regression passed 1,649 checks; Angular passed 250/250 tests and production build. The known initial-chunk budget warning remains unchanged at 24.88 kB.

## REC-001 branch lifecycle — 2026-11-17

- Web Release: passed with 0 warnings and 0 errors.
- SecurityRegression: 1,650 checks passed, including lifecycle permissions and antiforgery metadata.
- Angular: 250/250 tests passed across 120 files; typed branch API POST/PUT/status contracts are covered.
- Production build passed. Largest lazy chunk is 81,895 bytes against the 81,920-byte policy.
- Browser E2E is not claimed: the shared acceptance fixture stopped at an older `#preview-title` selector before proving the branch workflow.
## REC-002 warehouse delete safety — 2026-11-17

- Release build passed with zero warnings/errors.
- InventorySqlIntegration passed against a temporary migrated SQL database: referenced Warehouse and ProductStock remained; an unused Warehouse was removed.
- Angular passed 251/251 tests; the warehouse page retains a rejected row and renders API detail.
- SecurityRegression passed 1,650 checks; independent warehouse permissions and antiforgery remain covered.
## REC-003 route/function inventory — 2026-11-17

`pwsh scripts/Test-RouteFunctionCoverage.ps1` passed with 196 unique screen routes, zero unresolved P1 wiring gaps and seven P2 manual authorization reviews. Accounting, Sales and Inventory are mandatory gate inputs. The generator is deterministic and updates the project-control matrix from source; business-rule and browser verification remain separate evidence.
## REC-004A operation error toasts — 2026-11-17

Angular passed 256/256 tests across 122 files. New tests prove mutation conflicts create a concise toast with correlation ID, named field validation does not create a toast, 401 is not duplicated, the queue is bounded to three messages and errors expire. Production build passed; initial size is 340,416/358,400 bytes and largest lazy chunk is 81,895/81,920 bytes.
## REC-004B1 verification — 2026-11-17

- Angular: 258/258 tests across 122 files, including stable `conflict` code and untyped 409 fallback.
- Regression executable: 1,652 checks, including 409 conflict and 400 validation invariants.
- Release solution build: passed with 0 warnings and 0 errors before the running host locked Release output; subsequent focused regression rebuild used Debug.
- Production Angular build passed; bundle gate passed at 340,433/358,400 initial bytes and 81,895/81,920 largest lazy bytes.
## REC-004B2 verification — 2026-11-17

- Release solution build in isolated artifacts: 0 warnings, 0 errors while the live host remained running.
- Regression executable: 1,656 checks, including all four typed mappings and correlation IDs.
- Angular: 258/258 across 122 files.
- Production build and bundle gate: 340,433/358,400 initial; 81,895/81,920 largest lazy.
## REC-004B3 verification — 2026-11-17

- Release isolated solution build: 0 warnings, 0 errors.
- Regression executable: 1,658 checks, including source guards for typed Purchase Request and Stock Transfer semantics.
- Angular: 258/258 across 122 files.
- Production and bundle gate: 340,433/358,400 initial; 81,895/81,920 largest lazy.
- Measured remaining baseline: 70 API controllers with legacy InvalidOperation catches and 959 Application InvalidOperation throw sites; 51 typed throw sites are now present.
## REC-004B4 verification — 2026-11-17

- Isolated Release regression build and executable: 1,661 checks passed.
- Guards verify customer/supplier exception inheritance and prohibit product duplicate message matching.
- Angular: 258/258 across 122 files.
- Production and bundle gate: 340,433/358,400 initial; 81,895/81,920 largest lazy.
- Application baseline: 952 legacy InvalidOperation throw sites and 58 typed sites.