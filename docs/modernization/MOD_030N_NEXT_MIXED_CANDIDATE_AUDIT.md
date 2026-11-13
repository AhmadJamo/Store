# MOD-030N — Next mixed-candidate comparison

> Status: COMPLETE — audit only, 2026-11-13  
> Runtime impact: none  
> Deletion approval: not granted

## Candidates reviewed

| Candidate | MVC shape | Angular/API parity | Risk decision |
|---|---|---|---|
| `LocationMovements` | Index GET + stock-moving POST + one view | Complete named filters, form, history, idempotency key, antiforgery and rejected-input handling | Defer. The command changes exact-location stock and immutable movement evidence; physical retirement needs database-backed replay, insufficient-quantity and rejected-input acceptance. |
| `AnalyticsCenter` | Index GET + Create GET/POST + two views | Complete analytics page, builder, typed workspace POST and additional governed delegation API | Recommend as the next separately approved cohort. Workspace creation changes configuration only and does not post accounting or move stock. |

## Read-only live evidence

Using the authorized tenant state without executing any POST:

- `GET /api/v1/location-movements`: 200.
- `GET /api/v1/analytics-center`: 200.
- `GET /api/v1/analytics-center/builder`: 200.
- Legacy LocationMovements and both AnalyticsCenter GET forms returned exact 302 Angular targets with query preservation.

## Recommended MOD-030O scope

After separate explicit approval only:

- Delete `MiniStore.Web/Controllers/Analytics/AnalyticsCenterController.cs`.
- Delete `MiniStore.Web/Views/AnalyticsCenter/Index.cshtml` and `Create.cshtml`.
- Preserve GET/HEAD `/AnalyticsCenter` and `/AnalyticsCenter/Index` under `AnalyticsCenter.View` to `/app/analytics`.
- Preserve GET/HEAD `/AnalyticsCenter/Create` under `AnalyticsCenter.Customize` to `/app/analytics/builder`.
- Retire legacy MVC `POST /AnalyticsCenter/Create` without redirect; Angular continues through
  `POST /api/v1/analytics-center/workspaces`.
- Keep `AnalyticsCenterService`, API, Angular, provider contracts, permissions and delegation boundaries unchanged.

## Required implementation gate

1. Prove Index/builder reads, permission separation and invalid-query recovery.
2. Prove typed workspace validation, Manage-controlled shared visibility and antiforgery through existing tests. Do not
   create a workspace solely for retirement acceptance.
3. Confirm no static Razor caller requires the MVC POST response or TempData notification.
4. Add exact protected retired GET/HEAD routes and explicit non-redirected POST evidence.
5. Obtain owner approval naming all three files and the retired legacy POST.

## Inventory impact

Current counts remain 73 MVC controllers, 400 actions and 207 Razor files with 53 mixed-controller blockers. If separately
approved and completed, MOD-030O would reduce active counts to 72 controllers, 397 actions and 205 Razor files. Those future
counts are not active.

## Verification

- Release build succeeded with zero warnings and zero errors after the running host lock was removed.
- 1,851 security/inventory checks, 242/242 Angular tests, 137 templates and the 120-page consistency inventory passed.
- Bundle budgets remained 337,191/358,400 initial bytes and 81,877/81,920 largest lazy bytes.
- Browser/performance gates were not rerun for this audit-only documentation phase; the targeted hosted GET probes above passed.
