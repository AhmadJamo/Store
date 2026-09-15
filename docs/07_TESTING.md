# Testing
> Status: FOCUSED REGRESSION EXECUTABLE IMPLEMENTED
> Source of truth: `tests/SecurityRegression` and build output
> Last reviewed: 2026-09-15

`tests/SecurityRegression` is a standalone executable test harness. It verifies tenant metadata/query filters and write guards; complete classification of mapped Domain entities; composite TenantId coverage for every relationship between tenant-bearing records; tenant-scoped Admin authorization; destructive action HTTP methods; login/registration throttling; SaaS domain rules; inventory rowversions and warehouse/POS/location invariants. The 2026-09-15 run passed 191 checks. A separate live SQL transaction verified an actual cross-company stock mutation is rejected and rolled back.

The 2026-09-15 Release run passed 167 checks, and `dotnet build MiniStore.slnx -c Release --no-restore` completed with zero warnings/errors.

Remaining gaps include database-backed concurrent operations, full HTTP authentication/authorization journeys, billing confirmation integration tests, stock reconciliation, invoice-number concurrency, external payment webhook tests and penetration testing.

## Guided onboarding verification (2026-09-15)
Regression coverage checks pending/completed/skipped state, normalization, one-time resolution, invalid currency and rowversion. A local HTTP journey registered a company, reached /onboarding, applied the Cafe/tax/POS template, landed on the dashboard and authenticated again by company code. SQL found 18 accounts, one warehouse, one POS and one tax rate for the test tenant.
