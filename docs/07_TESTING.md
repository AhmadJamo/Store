# Testing
> Status: FOCUSED REGRESSION EXECUTABLE IMPLEMENTED
> Source of truth: `tests/SecurityRegression` and build output
> Last reviewed: 2026-10-16

`tests/SecurityRegression` is a standalone executable test harness. It verifies tenant metadata/query filters and write guards; complete classification of mapped Domain entities; composite TenantId coverage for every relationship between tenant-bearing records; tenant-scoped Admin authorization; destructive action HTTP methods; login/registration throttling; SaaS domain rules; inventory rowversions and warehouse/POS/location invariants. The 2026-10-16 run passes 756 checks, including mixed-scope AuditEvent classification, language-neutral codes, fail-closed query filtering, allowlist redaction, forbidden secret/reason rejection, HMAC-SHA256 integrity and verification across active-key rotation. SQL integration additionally exercises semantic role create/permission-change/delete and purchase-matching setting updates, verifies exactly four valid events, and proves a rejected audit payload persists neither setting nor success event.

`tests/InventorySqlIntegration` creates and deletes a process-specific disposable SQL Server database. It seeds two tenants, confirms tenant separation and proves concurrency behavior for reservations, last-unit issues and Goods Receipts. Audit coverage proves disabled no-op, atomic commit/rollback, two-tenant isolation, explicit platform bypass, append-only enforcement, OperationId uniqueness and integrity verification after a SQL round-trip. The purchasing scenario covers replenishment-source deduplication, receipt, bill, payment, controlled payment reversal and retry, restored bill availability, unbilled return, landed cost, reconciliation and order closure. It also covers adjustment posting, tracking, transfers, quarantine and recall. Override its local connection with `MINISTORE_WMS_TEST_CONNECTION`; the default uses local Windows authentication and explicitly disables transport encryption for this development-only fixture. `--inspect-audit` performs a read-only count/constraint/index inspection of the configured Demo database.

PUR-240 regression coverage validates forecast-rule horizons and seasonality state, automation schedule advancement, dedicated controller permissions, tenant classification/relationships and the unique tenant/scheduled-time idempotency index. EF migration drift must also report no pending model changes.

PUR-250A regression coverage validates delivered versus accepted quantities, rejected/damaged/short evidence, no stock for full rejection, route persistence, state transitions, separate controller permissions, active staged-receipt uniqueness and inspection/storage next-step links. The Debug EF design-time check reports no pending model changes; the locally running Release host held an older output copy, so migration drift was verified against the fresh Debug build.

The 2026-10-13 PUR-220 run built the SQL integration project successfully, but execution stopped before database creation because the configured SQL Server Windows connection could not generate an SSPI context. The mixed-source domain/model regression and EF pending-model check passed; the database-backed flow must be rerun when that local authentication issue is resolved.

The same executable accepts `--inspect-current` for a read-only tenant-by-tenant summary of the current development database, with `MINISTORE_WMS_INSPECT_CONNECTION` as an optional override. The 2026-09-30 run found five explained demo-data exceptions caused by legacy zero-valued movement snapshots; it performed no writes.

The 2026-09-15 Release run passed 167 checks, and `dotnet build MiniStore.slnx -c Release --no-restore` completed with zero warnings/errors.

Remaining gaps include full HTTP authentication/authorization journeys, semantic AuditEvent coverage/readers/retention, external payment webhook tests, performance/load coverage and penetration testing.

## Guided onboarding verification (2026-09-15)
Regression coverage checks pending/completed/skipped state, normalization, one-time resolution, invalid currency and rowversion. A local HTTP journey registered a company, reached /onboarding, applied the Cafe/tax/POS template, landed on the dashboard and authenticated again by company code. SQL found 18 accounts, one warehouse, one POS and one tax rate for the test tenant.

Cafe recipe, product-catalog and managed-unit coverage validates conversion, classification, channel rules, immutable snapshots, controlled negatives and Arabic decimal handling. Moving-average coverage verifies `10@5 + 20@8 = 7`, a 4-unit issue at value 28, remaining value 182, and provisional-negative variance settlement. Posting coverage verifies immutable line COGS, balanced sale and cost-variance journals, inclusive/exclusive tax and balanced return reversals. The current executable passes 260 checks; EF reports no pending model changes after `AddProductLogisticsFoundation`.
