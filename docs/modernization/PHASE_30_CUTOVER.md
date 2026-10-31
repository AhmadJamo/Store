# MOD-030 — hosted Angular cutover and MVC retirement

> Status: IN PROGRESS — `MOD-030A` complete on 2026-10-30

## MOD-030A production hosting and first cohort

Angular production uses `/app/` as its base href. `scripts/Publish-AngularAssets.ps1` builds the production application,
verifies that base, replaces only the fixed git-ignored `MiniStore.Web/wwwroot/app` directory and copies the hashed
browser output. ASP.NET serves direct assets with one-year immutable caching and returns a no-cache index for non-file
`/app` deep links. The fallback cannot match `/api`, MVC, health or OpenAPI.

Live HTTP verification returned 200 for `/app` and `/app/design-system`, 200 plus immutable caching for the favicon,
and the existing 401 for an anonymous session API. The full operator-authenticated browser suite passed 15/15 on the
same HTTPS host, proving UI states, RTL/LTR, keyboard, responsive behavior, session/permissions, same-data reconciliation
and all five high-risk cohorts without a development proxy.

Customers is the first committed `AngularDefault` cohort. Its allowlisted GET routes preserve query strings and IDs;
mutations and unknown actions remain on their existing protected paths. A mixed 10-route rehearsal passed with Customers
redirecting to Angular and the other authorized allowlisted surfaces returning MVC 200. The generator now derives
AngularDefault surfaces from appsettings, marks Customers accurately and reports a mixed presentation mode.

No Razor view or MVC controller was deleted. `MOD-030B` must repeat hosted acceptance plus rollback before moving
Suppliers and Products. Analytics Center and Settings remain later cohorts because Settings needs an operator with
`Administration.Access` for live evidence.
