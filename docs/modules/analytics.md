# Analytics Center

> Status: IMPLEMENTED (first reusable slice)  
> Last reviewed: 2026-10-22

## Angular report-builder foundation (2026-10-25)

`/analytics/builder` now reads the authorized provider/widget catalog through
`GET /api/v1/analytics-center/builder` and saves a personal/shared workspace through
`POST /api/v1/analytics-center/workspaces`. The API reuses `AnalyticsCenterService`; it does not accept SQL,
entity names or arbitrary expressions, and it rechecks provider permission and shared-workspace authority.
The Angular editor exposes bounded layout, period, refresh and widget selection values with bilingual RTL/LTR
presentation and external SCSS.

`MOD-017B` adds a server-authored financial preview to every analytics response. `AnalyticsCenterService` selects
only money-formatted authorized widgets after provider execution and workspace filtering, rounds them using the
organization decimal setting, and returns currency, period, provider source, bilingual definitions and generation
time. Angular renders this immutable preview and performs no financial calculation.

`MOD-017C` completes the governed delegation slice. A workspace owner, or a user with
`AnalyticsCenter.Manage`, can delegate a saved workspace to an active company member or tenant-owned role with an
optional future start and expiry, then revoke it immediately. Each row preserves creator/revoker and UTC timestamps.
Availability checks active user and role delegations, while provider permission is still rechecked before every
execution; delegation therefore shares a report definition but never grants source-data authority. Angular provides a
bilingual RTL/LTR access screen backed by server-supplied user/role choices and external SCSS.

The Analytics Center is a module-neutral reporting surface. `AnalyticsCenterService` discovers registered
`IAnalyticsModuleProvider` implementations; the web layer does not depend on Sales or Purchasing DTOs.
Sales and Purchasing are the first providers and future modules can join through the same definition/data contract.

Tenant-owned `AnalyticsWorkspace` records store personal or shared dashboards. A workspace selects one provider,
default period, dimension, comparison mode, refresh interval, definition visibility and a one-to-four-column layout.
Its ordered widgets store visualization, bilingual title overrides, column/row span, Top-N limit, legend and comparison
switches, target value and accent color. Provider-owned keys keep saved layouts stable while calculations remain in the
owning Application service.

The center supports live views and saved workspaces, date/dimension/provider filters, previous-period or previous-year
metric comparison, bilingual RTL/LTR rendering and reusable metric, funnel, table, bar, line, donut and comparison
metadata. Calculations are read-only and use tenant-filtered repositories through the Application layer.

Permissions are intentionally separate:

- `AnalyticsCenter.View`: open the center and execute registered read-only providers.
- `AnalyticsCenter.Customize`: create personal workspaces and manage delegation for workspaces the user owns.
- `AnalyticsCenter.Manage`: create shared workspaces and manage delegation for any workspace in the company.

Migration 135 creates `AnalyticsWorkspaces` and `AnalyticsWorkspaceWidgets` with tenant-composite relationships,
rowversion and per-workspace widget-key/order uniqueness. The local Demo is reconciled through migration 135.
Source migration `20261025025522_AddAnalyticsWorkspaceDelegation` adds tenant-owned delegation rows with
tenant-composite workspace/role relationships and user/role active-lookup indexes; it has not been applied to Demo.

Angular exploration now reuses this service for provider/group/date/comparison selection, bounded export preview and
allowlisted Customer/Supplier drill-down. Saved refresh intervals appear in the Operations Center as refresh plans only.
Editing, cloning, archival, scheduled distribution, real file exports and a governed semantic-field builder remain later
increments. They should extend this center rather than create module-specific dashboard infrastructure.
