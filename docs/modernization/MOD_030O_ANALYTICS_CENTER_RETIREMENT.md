# MOD-030O — AnalyticsCenter MVC retirement

> Status: COMPLETE — 2026-11-13  
> Approval: explicit owner approval received  
> Database/domain/API impact: none

## Retired scope

- Deleted `MiniStore.Web/Controllers/Analytics/AnalyticsCenterController.cs`.
- Deleted `MiniStore.Web/Views/AnalyticsCenter/Index.cshtml` and `Create.cshtml`.
- Removed `AnalyticsCenter:Index/Create` from the reversible cutover catalog and removed the obsolete
  `AnalyticsCenter=AngularDefault` setting.
- Removed AnalyticsCenter from the active MVC counterpart generator input.

## Preserved behavior

- GET/HEAD `/AnalyticsCenter` and `/AnalyticsCenter/Index` require `AnalyticsCenter.View`, redirect to `/app/analytics`
  and preserve the complete query string.
- GET/HEAD `/AnalyticsCenter/Create` requires `AnalyticsCenter.Customize` and redirects to `/app/analytics/builder`.
- `AnalyticsCenterService`, module providers, DTO validation, tenant filtering, Manage-controlled shared visibility,
  delegation behavior, API and Angular remain unchanged.
- Workspace creation remains `POST /api/v1/analytics-center/workspaces`; global unsafe-method antiforgery and the existing
  Customize/Manage authorization boundary remain authoritative.

## Deliberately retired boundary

Legacy MVC `POST /AnalyticsCenter/Create` is not redirected. A hosted authenticated probe returned HTTP 405 because the
compatibility catalog exposes only safe GET/HEAD endpoints. Acceptance did not create or modify a workspace.

## Inventory result

- Active MVC controllers: 72.
- Active MVC actions: 397.
- Total Razor files: 205; user-facing views: 187; Razor control rows: 197.
- Redirected retired presentation GET actions: 12 across 10 surfaces.
- Twelve Razor views are retired. The legacy AuditEvents export and AnalyticsCenter workspace POST are separately retired
  without redirects. Mixed-controller blockers are reduced to 52.

## Verification

- Release build: zero warnings and zero errors.
- Security/inventory regression: 1,845 checks passed.
- Angular: 242/242 tests passed across 116 files; 137 templates and the 120-page consistency inventory passed.
- Bundle budgets: 337,191/358,400 initial bytes and 81,877/81,920 largest lazy bytes.
- Microsoft Edge authenticated Angular acceptance passed all 147 permission-eligible allowlisted GET routes.
- Hosted probes: Index, explicit Index and Create returned exact 302 Angular targets; legacy Create POST returned 405;
  analytics page and builder API reads returned 200.

## Rollback

Deploy the previous committed build. No schema rollback or runtime MVC switch applies to physically deleted files.
