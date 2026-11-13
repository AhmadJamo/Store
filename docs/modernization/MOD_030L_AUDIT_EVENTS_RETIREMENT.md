# MOD-030L — Tenant AuditEvents MVC retirement

> Status: COMPLETE — 2026-11-13  
> Approval: explicit owner approval received  
> Database/domain/API impact: none

## Retired scope

- Deleted `MiniStore.Web/Controllers/Security/AuditEventsController.cs`, including the legacy `Index` GET and `Export` POST.
- Deleted `MiniStore.Web/Views/AuditEvents/Index.cshtml`.
- Removed `AuditEvents:Index` from the reversible presentation-cutover catalog and removed the obsolete
  `AuditEvents=AngularDefault` setting.
- Removed AuditEvents from the active MVC counterpart generator input.

## Preserved behavior

- Exact GET/HEAD `/AuditEvents` and `/AuditEvents/Index` routes remain permission-protected by `AuditEvents.View` and
  redirect to `/app/audit-events`, preserving the complete query string.
- `AuditEventReadService`, tenant isolation, query validation, the 5,000-row export cap and semantic export evidence are
  unchanged.
- `AuditEventsApiController` remains the only tenant audit HTTP boundary: GET requires `AuditEvents.View`; export requires
  both `AuditEvents.View` and `AuditEvents.Export`, validates antiforgery and decides sensitive evidence server-side through
  `AuditEvents.ViewSensitive`.
- Angular navigation, localization, filters, paging and CSV handling are unchanged.
- Platform audit and retention are outside this cohort.

## Deliberately retired boundary

Legacy MVC `POST /AuditEvents/Export` is not redirected. A hosted authenticated probe returns HTTP 405 because only the
declared GET/HEAD compatibility endpoint exists. Angular continues to use `POST /api/v1/audit-events/export`; acceptance did
not create an export or mutate business data.

## Inventory result

- Active MVC controllers: 73.
- Active MVC actions: 400.
- Total Razor files: 207; user-facing views: 189; Razor control rows: 199.
- Redirected retired presentation actions: 10 across 9 surfaces, plus the intentionally non-redirected legacy AuditEvents
  export. Ten Razor views are physically retired.

## Verification

- Release build: zero warnings and zero errors.
- Security/inventory regression: 1,851 checks passed.
- Angular: 242/242 tests passed across 116 files; 137 templates and the 120-page consistency inventory passed.
- Bundle budgets: 337,191/358,400 initial bytes and 81,877/81,920 largest lazy bytes.
- Microsoft Edge authenticated Angular cutover acceptance passed all 147 permission-eligible allowlisted GET routes.
- Hosted probes: both legacy GET forms returned exact 302 targets with query preservation; the retired MVC export POST
  returned 405; `GET /api/v1/audit-events` returned 200.

## Rollback

Deploy the previous committed build. There is no runtime MVC switch for a physically deleted controller/view and no schema
rollback is involved.
