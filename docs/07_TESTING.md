# Testing
> Status: FOCUSED REGRESSION EXECUTABLE IMPLEMENTED
> Source of truth: `tests/SecurityRegression` and build output
> Last reviewed: 2026-10-27

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
