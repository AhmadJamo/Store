# Phase 27 — SaaS and public Angular migration

> Status: IN PROGRESS — `MOD-027A` COMPLETE on 2026-10-29
>
> Presentation default: MVC until a separate cutover approval.

## Scope

Move tenant subscription, public pricing and Platform control-plane presentation to Angular while tenant identity, plan eligibility,
pricing, promotion redemption, payment confirmation and Platform authorization remain server-owned.

## `MOD-027A` delivered — tenant subscription and checkout

- Added an Application-authored tenant subscription summary over `ISaasRepository`, including named plan, lifecycle status,
  billing cycle, period and current usability.
- Added an authenticated API for the summary and public plans plus an antiforgery-protected checkout command.
- Added a bilingual RTL-safe Angular screen for current subscription evidence, plan selection, promotion entry and the resulting
  server-calculated 30-minute checkout quote.
- The browser submits only plan, cycle and optional promotion code. `BillingCheckoutService` remains authoritative for public-plan
  eligibility, price, promotion validity, one-use-per-company enforcement, discount, total, currency, expiry and pending state.
- Subscription/session endpoints and the Angular subscription route remain reachable when entitlement middleware blocks the rest of
  the ERP, so an expired company can recover without weakening other API boundaries.
- No online provider or client-side payment confirmation was invented. Platform billing confirmation remains the operational boundary.

## Evidence

- Web Release: zero warnings/errors; SecurityRegression: 1304 checks.
- Angular: 204/204 tests across 99 files; 111 templates passed accessibility; external SCSS passed.
- Bundles: 318,742/358,400 initial bytes and 81,743/81,920 largest lazy bytes.
- Browser and performance checks were skipped by the standard local step flags; all seven applicable gate stages passed.

## Next slice

`MOD-027B` migrates the public landing and pricing surfaces while registration remains on its existing Angular flow.
