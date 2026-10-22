# Analytics Center

> Status: IMPLEMENTED (first reusable slice)  
> Last reviewed: 2026-10-22

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
- `AnalyticsCenter.Customize`: create personal workspaces.
- `AnalyticsCenter.Manage`: create shared workspaces in addition to personal ones.

Migration 135 creates `AnalyticsWorkspaces` and `AnalyticsWorkspaceWidgets` with tenant-composite relationships,
rowversion and per-workspace widget-key/order uniqueness. The local Demo is reconciled through migration 135.

Editing, cloning, archival, scheduled distribution, exports and a governed semantic-field builder remain later
increments. They should extend this center rather than create module-specific dashboard infrastructure.
