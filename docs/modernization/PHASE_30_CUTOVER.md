# MOD-030 — hosted Angular cutover and MVC retirement

> Status: IN PROGRESS — `MOD-030A/B` complete through 2026-10-31

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

No Razor view or MVC controller was deleted.

## MOD-030B supplier and product cohort

Suppliers and Products now join Customers as committed `AngularDefault` surfaces. The code-owned allowlist covers the
existing Supplier list/create/edit and Product list/create/edit GET actions; it does not invent MVC details actions or
redirect any mutation. Product search query strings and positive edit IDs are preserved.

Hosted `/app/suppliers`, `/app/suppliers/new`, `/app/products` and `/app/products/new` deep links returned 200. The full
operator-authenticated hosted suite passed 15/15 with zero skips. The mixed pilot passed 12 authorized routes with the
three cohorts redirecting to exact Angular targets and Analytics remaining MVC. A process-local rollback then returned
the same 12 routes through MVC 200. No Razor view/controller or database object changed.

The final non-live gate passed a warning-free Release build, 1,349 backend regression checks, 214/214 Angular tests,
121 static templates and the project bundle budgets (326,764 initial bytes; 81,880 largest lazy bytes). Browser evidence
is the separate hosted 15/15 run above rather than a development-host substitute.

`MOD-030C` must obtain an operator session with `Administration.Access`, then repeat hosted acceptance, mixed pilot and
MVC rollback for Analytics Center and Settings before committing either surface.
