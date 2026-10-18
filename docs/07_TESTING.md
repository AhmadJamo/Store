# Testing
> Status: FOCUSED REGRESSION EXECUTABLE IMPLEMENTED
> Source of truth: `tests/SecurityRegression` and build output
> Last reviewed: 2026-10-18

`tests/SecurityRegression` is a standalone executable test harness. It verifies tenant metadata/query filters and write guards; complete classification of mapped Domain entities; composite TenantId coverage; authorization; destructive action HTTP methods; throttling; SaaS and inventory invariants. The 2026-10-18 run passes 780 checks, including audit/observability coverage and three WarehouseLocations boundary checks: no repository injection, DTO-projected rows and Application-owned page options.

`tests/InventorySqlIntegration` creates and deletes a process-specific disposable SQL Server database and applies the full migration chain. Audit coverage includes disabled legacy writes with continuing semantic events, enabled rollback adding exactly one future legacy row, rollback/isolation/append-only/integrity, reader/export boundaries, SQL role-member impersonation, legal-hold exclusion and release, signed checkpoint plus manifest-before-delete execution, and omission of expired request evidence from destruction files. Override its local connection with `MINISTORE_WMS_TEST_CONNECTION`; `--inspect-audit` performs repeatable read-only schema, count, catalog, observed-event and unexplained-gap reconciliation.

PUR-240 regression coverage validates forecast-rule horizons and seasonality state, automation schedule advancement, dedicated controller permissions, tenant classification/relationships and the unique tenant/scheduled-time idempotency index. EF migration drift must also report no pending model changes.

PUR-250A regression coverage validates delivered versus accepted quantities, rejected/damaged/short evidence, no stock for full rejection, route persistence, state transitions, separate controller permissions, active staged-receipt uniqueness and inspection/storage next-step links. The Debug EF design-time check reports no pending model changes; the locally running Release host held an older output copy, so migration drift was verified against the fresh Debug build.

The 2026-10-13 PUR-220 run built the SQL integration project successfully, but execution stopped before database creation because the configured SQL Server Windows connection could not generate an SSPI context. The mixed-source domain/model regression and EF pending-model check passed; the database-backed flow must be rerun when that local authentication issue is resolved.

The same executable accepts `--inspect-current` for a read-only tenant-by-tenant summary of the current development database, with `MINISTORE_WMS_INSPECT_CONNECTION` as an optional override. The 2026-09-30 run found five explained demo-data exceptions caused by legacy zero-valued movement snapshots; it performed no writes.

The 2026-09-15 Release run passed 167 checks, and `dotnet build MiniStore.slnx -c Release --no-restore` completed with zero warnings/errors.

`tests/PerformanceSmoke` is a dependency-free HTTP workload. The local gate runs 20 warmups followed by 500 requests at concurrency 20 against `/health/live`, fails above p95 250 ms or p99 500 ms and writes `artifacts/verification/performance.json`. The final run completed all 500 requests with zero failures, p95 16.85 ms and p99 28.35 ms. This is an availability regression baseline, not a production-sizing or business-transaction claim.

Run the complete owner-approved local gate with `pwsh -NoProfile -File scripts/verify-local.ps1`. It builds Release, runs the focused executable, uses a disposable SQL database, starts the application, requires healthy readiness and then runs the workload. Remaining gaps include full HTTP authentication/authorization journeys, business-transaction/soak workloads, external payment webhook tests, production monitoring and penetration testing.

## Guided onboarding verification (2026-09-15)
Regression coverage checks pending/completed/skipped state, normalization, one-time resolution, invalid currency and rowversion. A local HTTP journey registered a company, reached /onboarding, applied the Cafe/tax/POS template, landed on the dashboard and authenticated again by company code. SQL found 18 accounts, one warehouse, one POS and one tax rate for the test tenant.

Cafe recipe, product-catalog and managed-unit coverage validates conversion, classification, channel rules, immutable snapshots, controlled negatives and Arabic decimal handling. Moving-average coverage verifies `10@5 + 20@8 = 7`, a 4-unit issue at value 28, remaining value 182, and provisional-negative variance settlement. Posting coverage verifies immutable line COGS, balanced sale and cost-variance journals, inclusive/exclusive tax and balanced return reversals. The current executable passes 260 checks; EF reports no pending model changes after `AddProductLogisticsFoundation`.
