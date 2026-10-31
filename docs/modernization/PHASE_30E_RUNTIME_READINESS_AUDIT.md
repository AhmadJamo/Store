# MOD-030E — Angular runtime readiness audit

> Status: APPROVED — execution starts with MOD-030E1
> Date: 2026-10-31

## Finding

Functional migration coverage and runtime cutover are different measures. The source-driven registry maps 82 MVC
surfaces to existing Angular routes and reports zero counterpart gaps, but committed configuration currently marks only
Customers, Suppliers, Products, AnalyticsCenter and Settings as `AngularDefault`. The action allowlist redirects 29
GET actions. The other 77 mapped surfaces remain MVC by default.

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
