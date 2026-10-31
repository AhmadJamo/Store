# MOD-030E — Angular runtime readiness audit

> Status: IN PROGRESS — MOD-030E2C complete
> Date: 2026-11-01

## Finding

Functional migration coverage and runtime cutover are different measures. The source-driven registry maps 82 MVC
surfaces to existing Angular routes and reports zero counterpart gaps. Before MOD-030E, committed configuration marked
five surfaces and 29 GET actions as `AngularDefault`; 77 mapped surfaces remained MVC by default.

This means Angular is ready for the five proven cohorts, but the application is not yet Angular-only and physical MVC
retirement would be premature.

## Execution order

1. `MOD-030E1` — read-only inventory: InventoryBalances, InventoryReservations, InventoryInsights and
   InventoryReconciliation.
2. `MOD-030E2` — operational inventory and WMS, split into bounded lifecycle cohorts.
3. `MOD-030E3` — catalog masters, warehouses, users/roles and audit administration.
4. `MOD-030E4` — purchasing, from read-only analysis to document lifecycles.
5. `MOD-030E5` — professional sales, receivables, legacy sales/POS and accounting evidence.
6. `MOD-030E6` — public onboarding, subscription and special-auth Platform/Supplier Portal surfaces, each behind its
   own authentication boundary.

## MOD-030E1 evidence

InventoryBalances, InventoryReservations, InventoryInsights and InventoryReconciliation now join the committed
AngularDefault set. Their four exact Index GET actions preserve query strings; no mutation was added to the catalog.
The authenticated hosted suite passed 15/15 with zero failures/skips, the expanded cutover passed 33 authorized routes,
and a process-local override returned the same 33 probes to MVC. The final gate passed Release, 1,394 backend checks,
214/214 Angular tests, 121 templates and bundle budgets. Nine surfaces are now AngularDefault and 73 mapped surfaces
remain MVC by default.

## MOD-030E2A evidence

StockTransactions and ProductStocks now join the committed AngularDefault set. The ledger Index keeps its filter query,
manual adjustment creation resolves to `/inventory/stocks/adjustment`, opening-stock creation resolves to
`/inventory/stocks/new`, and edit preserves the positive identifier at `/inventory/stocks/{id}/edit`. Angular loads that
exact record while the unified stock workspace retains independently authorized create, edit, delete and adjustment APIs.

The authenticated hosted suite passed 15/15 with zero failures/skips, including product/warehouse/quantity equality
between the rendered Angular Product Stocks screen and its API. Cutover and process-local MVC rollback both passed 38
authorized routes. The final evidence passed a warning-free Release build, 1,406 backend checks, 215/215 Angular tests
across 106 files, 121 templates and bundle budgets (327,235 initial bytes; 81,880 largest lazy bytes). Eleven surfaces are
now AngularDefault and 71 mapped surfaces remain MVC by default. No MVC file, database object or business rule changed.

## MOD-030E2B evidence

InventoryAdjustments now joins the committed AngularDefault set. Index, Create and positive-ID Details GET actions map
to `/inventory/adjustments`, `/inventory/adjustments/new` and `/inventory/adjustments/{id}`. The cutover resolver now
parses invariant positive Int64 identifiers because the adjustment aggregate uses `long`; unsafe, zero and negative route
values still fail closed. Count, Approve, Post and Cancel remain independently authorized antiforgery-protected API/MVC
mutations and are not in the presentation catalog.

The authenticated hosted suite passed 15/15 with zero failures/skips. Cutover and process-local MVC rollback both passed
40 authorized routes. Demo contained no adjustment row, so no live Details fixture was claimed; the exact Details target
was instead covered with a positive identifier above Int32 range. The final gate passed a warning-free Release build,
1,413 backend checks, 215/215 Angular tests across 106 files, 121 templates and the existing bundle budgets. Twelve
surfaces are now AngularDefault and 70 mapped surfaces remain MVC by default. No data or schema changed.

## MOD-030E2C evidence

StockTransfers now joins the committed AngularDefault set. Index, Create, Edit and Details GET actions map to
`/inventory/transfers`, `/inventory/transfers/new`, `/inventory/transfers/{id}/edit` and
`/inventory/transfers/{id}`. Submit, Approve, Reject, ReturnToDraft, Post and Cancel remain independently authorized
antiforgery-protected API/MVC mutations and are not presentation redirects.

The authenticated hosted suite passed 15/15 with zero failures/skips. Cutover and process-local MVC rollback both passed
42 authorized routes. Demo contained no transfer row, so no live Details/Edit fixture was claimed or created; exact
positive-ID resolution for both routes is covered by executable security regression checks. The final gate passed a
warning-free Release build, 1,423 backend checks, 215/215 Angular tests across 106 files, 121 templates and bundle budgets
(327,235 initial bytes; 81,880 largest lazy bytes). Thirteen surfaces are now AngularDefault and 69 mapped surfaces remain
MVC by default. No data, schema, business rule or MVC file changed.

Every cohort must add explicit GET-only action mappings, require its existing permissions, pass hosted acceptance and
exact Angular targets, then pass a process-local MVC rollback. Mutations continue through their independently authorized
API endpoints. No MVC file is removed during MOD-030E.

## First cohort rationale

The four MOD-030E1 controllers each expose one GET-only Index action and already have a permission-protected Angular
route and API contract. They do not create, update, post, approve, export or download. This makes them the lowest-risk
runtime cohort and a clean proof that query strings and permission boundaries survive broader cutover.

## Exit gate

MOD-030E is complete only when every intended user-facing MVC route family is either committed AngularDefault or
explicitly classified as an authentication/download/compatibility exception. Only then may an owner approve physical
MVC retirement with deployment-level rollback.
