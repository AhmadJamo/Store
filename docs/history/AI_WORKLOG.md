# AI Work Log

## 2026-09-13
### Completed
Initial documentation scan and creation of the documentation system.

### Changed Files
Markdown documentation under `docs/` and root `AGENTS.md` documentation instructions only.

### Documentation Updated
Initial set created.

### Tests
No test project found. No tests executed.

### Build
Earlier repository review recorded a successful `dotnet build MiniStore.sln --no-restore` on 2026-09-12; this documentation task does not change application source.

### Problems Found
See `TODO.md`, security and accounting documentation.

### Next Steps
Address security/inventory integrity priorities before SaaS expansion.

## 2026-09-13 — Security remediation
Inspected authentication, permission evaluators, role/user management, seeders, MVC mutations, dynamic views and error handling. Added Admin-only identity mutation policy in Application and enforced it in both evaluators; fixed role-delete GET CSRF and all delete controls; enabled lockout/rate limiting; removed default bootstrap password and blocked existing-account promotion; sanitized broad exception responses while logging details. Added focused executable regression checks. No deployment/database credentials changed. Remaining credential rotation, hosting configuration, inventory concurrency and audit limitations are recorded in security/TODO docs.

Validation completed: Web build succeeded with zero warnings/errors. `dotnet run --project tests/SecurityRegression/SecurityRegression.csproj --no-restore` passed 35 checks covering both permission evaluators for Admin/non-Admin, all five delete HTTP methods and login limiter metadata. These are focused checks, not deployed HTTP or SQL integration tests.

## 2026-09-13 — Inventory integrity phase 1
Added ProductStock and StockTransfer rowversions with migration `AddInventoryConcurrency`, changed UnitOfWork inventory transactions to Serializable isolation and mapped stale writes to a retry message. The transfer token prevents concurrent duplicate posting/status changes. Restricted manual stock movements to adjustment-in/out with a required reason and transactional balance/movement saving. Purchase and sale aggregates now reject duplicate product lines. The Web Release build passed with zero warnings/errors; focused security/inventory checks passed from an isolated output directory. Migration `20260913112156_AddInventoryConcurrency` was applied successfully to the local MiniStoreDb, adding both RowVersion columns. Database-backed concurrent-write/reconciliation tests remain follow-up work.
