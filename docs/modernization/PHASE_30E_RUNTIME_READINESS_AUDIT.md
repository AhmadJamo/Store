# MOD-030E — Angular runtime readiness audit

> Status: IN PROGRESS — MOD-030E1 complete
> Date: 2026-10-31

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
