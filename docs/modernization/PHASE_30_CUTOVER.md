# MOD-030 — hosted Angular cutover and MVC retirement

> Status: RUNTIME CUTOVER COMPLETE — physical MVC retirement remains separately approval-gated

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

## MOD-030C Analytics Center cohort and Settings preparation

AnalyticsCenter now joins Customers, Suppliers and Products as AngularDefault. Its index and builder require the
existing View/Customize permissions. Hosted acceptance passed 15/15 with zero skips; the mixed pilot redirected the
authorized Analytics routes to exact Angular targets across 12 covered routes, and process-local rollback returned the
same 12 routes through MVC 200.

The final static gate passed a warning-free Release build, 1,381 backend regression checks, 214/214 Angular tests,
121 templates and the unchanged project bundle budgets.

At the end of MOD-030C, Settings still remained MVC. Its existing GET pages were mapped to proven Angular routes in
preparation for the next cohort, while the binary logo response and all unsafe actions remained outside the catalog.
Two earlier interactive captures lacked `Administration.Access`; the runner therefore requires the permission explicitly.

## MOD-030D authorized Settings cohort

The active-company Tenant Admin session now projects the complete protected administration permission set, including
`Administration.Access`, while backend authorization remains authoritative. Settings joins Customers, Suppliers,
Products and AnalyticsCenter as a committed `AngularDefault` cohort. Only the existing allowlisted GET pages redirect;
the binary OrganizationLogo action, all mutations and unknown actions remain outside presentation cutover.

The authenticated production-hosted suite passed 15/15 with zero failures or skips. The five-cohort cutover matched 29
authorized GET routes to their exact Angular targets. A process-local override then returned the same 29 probes to MVC;
the legacy `/Settings/Invoices` alias was verified as its existing local redirect to Document Numbers while every other
probe returned MVC 200.

The final static gate passed a warning-free Release build, 1,385 backend regression checks, 214/214 Angular tests across
106 files, 121 templates and the existing bundle budgets (326,764 initial bytes; 81,880 largest lazy bytes). No Razor
view, MVC controller, database object or business rule was removed. Physical MVC retirement is a separate destructive
phase and requires explicit approval for each bounded cohort.

Runtime completion evidence continues in `PHASE_30E_RUNTIME_READINESS_AUDIT.md`. MOD-030E1 adds four GET-only
inventory analysis surfaces after 15/15 production-hosted acceptance and 33-route cutover and rollback evidence.

MOD-030E2A adds StockTransactions and ProductStocks with intent-specific Angular stock routes. Hosted acceptance passed
15/15, rendered Angular/API stock data matched, and both cutover and MVC rollback passed 38 authorized GET routes.

MOD-030E2B adds InventoryAdjustments Index/Create/Details and widens the safe route-value parser to positive Int64.
Hosted acceptance passed 15/15 and both cutover and MVC rollback passed 40 authorized GET routes without test-data writes.

MOD-030E completed on 2026-11-03 with all 81 presentation surfaces AngularDefault, zero presentation gaps and Language
explicitly retained as the sole POST-only, antiforgery-protected compatibility boundary. No Razor view or MVC controller
was deleted. Physical retirement begins only after the non-destructive `MOD-030F` dependency/cohort audit and explicit
approval of each destructive cohort.

## MOD-030G physical retirement pilot — completed 2026-11-12

The separately approved first physical retirement cohort removes only InventoryBalances, InventoryReservations,
InventoryInsights, InventoryReconciliation and StockMovements MVC Index presentation and their five Razor views.
The same permission policies now protect centralized GET/HEAD legacy redirects to Angular, including explicit /Index
aliases and query preservation. These surfaces are absent from PresentationCutover settings and its reversible catalog;
rollback is a previous-build deployment. No mutation, download, financial, public or shared Razor boundary changed.

## MOD-030H financial read-only retirement — completed 2026-11-12

After separate explicit approval, `JournalEntries.Details` and `PurchaseAccountingReconciliation.Index` MVC presentation
and their two Razor views were removed. Their existing API controllers, Angular pages and Application services remain
authoritative. Protected legacy redirects preserve a positive journal ID, reconciliation filters and query strings; the
active cutover catalog/configuration no longer targets deleted code. Edge acceptance passed 146 authorized routes.
