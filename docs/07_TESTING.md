# Testing
> Status: FOCUSED REGRESSION EXECUTABLE IMPLEMENTED
> Source of truth: `tests/SecurityRegression` and build output
> Last reviewed: 2026-09-15

`tests/SecurityRegression` is a standalone executable test harness. It verifies tenant metadata/query filters and write guards; tenant-scoped Admin authorization in both permission evaluators and controller metadata; destructive action HTTP methods; login/registration rate-limit metadata; subscription/promotion/checkout domain rules; inventory rowversions and core warehouse/POS/location invariants.

The 2026-09-15 Release run passed 167 checks, and `dotnet build MiniStore.slnx -c Release --no-restore` completed with zero warnings/errors.

Remaining gaps include database-backed concurrent operations, full HTTP authentication/authorization journeys, billing confirmation integration tests, stock reconciliation, invoice-number concurrency, external payment webhook tests and penetration testing.
