# Testing
> Status: FOCUSED REGRESSION EXECUTABLE IMPLEMENTED
> Source of truth: `tests/SecurityRegression` and build output
> Last reviewed: 2026-10-10

`tests/SecurityRegression` is a standalone executable test harness. It verifies tenant metadata/query filters and write guards; complete classification of mapped Domain entities; composite TenantId coverage for every relationship between tenant-bearing records; tenant-scoped Admin authorization; destructive action HTTP methods; login/registration throttling; SaaS domain rules; inventory rowversions and warehouse/POS/location invariants. The 2026-10-10 run passed 500 checks, including Vendor Bill matching evidence plus Supplier Payment allocation, immutability, numbering, tenancy and permission boundaries.

`tests/InventorySqlIntegration` creates and deletes a process-specific disposable SQL Server database. It seeds two tenants, executes the reconciliation repository on SQL Server, confirms tenant separation and proves concurrency behavior for reservations, last-unit issues and Goods Receipts. The purchasing scenario accepts exactly one of two concurrent full receipts, exactly one duplicate supplier invoice and exactly one of two concurrent full-balance supplier payments. It verifies atomic GRNI clearing, Purchase Price Variance, supplier payable recognition and later payable-to-cash settlement plus persisted match-override evidence. Two concurrent returns may consume only the remaining unbilled receipt quantity; a later attempt against the billed portion is rejected. It also covers adjustment posting, tracking, transfers, quarantine and recall. Override its local connection with `MINISTORE_WMS_TEST_CONNECTION`; the default uses local Windows authentication and explicitly disables transport encryption for this development-only fixture.

The same executable accepts `--inspect-current` for a read-only tenant-by-tenant summary of the current development database, with `MINISTORE_WMS_INSPECT_CONNECTION` as an optional override. The 2026-09-30 run found five explained demo-data exceptions caused by legacy zero-valued movement snapshots; it performed no writes.

The 2026-09-15 Release run passed 167 checks, and `dotnet build MiniStore.slnx -c Release --no-restore` completed with zero warnings/errors.

Remaining gaps include full HTTP authentication/authorization journeys, supplier credit notes for billed returns, receipt reversal/reconciliation reporting, external payment webhook tests and penetration testing.

## Guided onboarding verification (2026-09-15)
Regression coverage checks pending/completed/skipped state, normalization, one-time resolution, invalid currency and rowversion. A local HTTP journey registered a company, reached /onboarding, applied the Cafe/tax/POS template, landed on the dashboard and authenticated again by company code. SQL found 18 accounts, one warehouse, one POS and one tax rate for the test tenant.

Cafe recipe, product-catalog and managed-unit coverage validates conversion, classification, channel rules, immutable snapshots, controlled negatives and Arabic decimal handling. Moving-average coverage verifies `10@5 + 20@8 = 7`, a 4-unit issue at value 28, remaining value 182, and provisional-negative variance settlement. Posting coverage verifies immutable line COGS, balanced sale and cost-variance journals, inclusive/exclusive tax and balanced return reversals. The current executable passes 260 checks; EF reports no pending model changes after `AddProductLogisticsFoundation`.
