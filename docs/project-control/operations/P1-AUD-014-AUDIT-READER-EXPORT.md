# P1-AUD-014 — Audit reader and export evidence

> Executed: 2026-10-16  
> Environment: local Demo code and SQL integration harness  
> Secrets: none recorded

## Implemented boundary

- Added an Application-layer query/export service and an Infrastructure read repository; controllers do not access EF directly.
- Added tenant `/AuditEvents` and Platform `/platform/audit-events` readers with deterministic newest-first pagination, a default 30-day window and a maximum 366-day window.
- Tenant reads always require an active tenant and return only that tenant. Platform reads require the absence of a tenant and return only Platform/System scopes.
- Added separate tenant permissions for view, export and sensitive evidence. Platform view/export/sensitive policies use separate role sets.
- Added a responsive Arabic/English card UI with filters and no horizontal table dependency.
- Added CSV (comma-separated values) export capped at 5,000 rows. It requires a fixed review purpose, prevents spreadsheet formula execution, and records exactly one `DataAccess.AuditEventsExported` event containing only purpose, row count, scope and SHA-256 file fingerprint.
- The export snapshot is read before its audit event is written, so the export does not recursively include itself.
- No audit-event update/delete action, retention rule, archive worker, SQL identity split or external checkpoint was added.

## Verification

- `dotnet build MiniStore.Web/MiniStore.Web.csproj --no-restore`: passed, 0 warnings and 0 errors.
- `dotnet build tests/InventorySqlIntegration/InventorySqlIntegration.csproj --no-restore`: passed, 0 warnings and 0 errors.
- `dotnet run --project tests/SecurityRegression/SecurityRegression.csproj --no-restore`: passed 765 checks.
- `dotnet run --project tests/InventorySqlIntegration/InventorySqlIntegration.csproj --no-restore`: passed.
- SQL evidence covers tenant/platform isolation, default suppression of masked request evidence, explicit sensitive projection and one tenant-scoped export event with a file fingerprint.

## Schema and compatibility

- No schema change or migration was required; migration count remains 113.
- Legacy `AuditLogs` and specialized accounting, inventory and workflow histories remain unchanged.
- Committed audit configuration remains disabled and contains no key; local Demo activation remains in User Secrets.

## Next gate

`P1-AUD-015` is proposed only. It needs owner/legal/hosting decisions for retention, legal hold, archive destination, external checkpoint storage and separate SQL identities before implementation approval.
