# Phase 27 — SaaS and public Angular migration

> Status: IN PROGRESS — `MOD-027A..D` COMPLETE on 2026-10-30
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

## `MOD-027B` delivered — public landing and pricing

- Added an anonymous read-only versioned API that projects public plan cards through `PlatformSaasService`.
- Added bilingual RTL-safe Angular welcome and pricing routes outside the authenticated ERP shell, with localized navigation,
  features, plan names/descriptions, known limit labels and monthly/annual prices.
- Registration destinations remain local and pass only the selected plan ID; registration reloads and validates plan authority.
- Authenticated tenants with unusable subscriptions may still read the public-plan endpoint, without bypassing any business API.

## MOD-027B evidence

- Web Release: zero warnings/errors; SecurityRegression: 1305 checks.
- Angular: 205/205 tests across 100 files; 112 templates passed accessibility; external SCSS passed.
- Bundles: 318,986/358,400 initial bytes and 81,743/81,920 largest lazy bytes.
- Browser and performance checks were skipped by the standard local step flags; all seven applicable gate stages passed.

## Next slice

## `MOD-027C` delivered — Platform sign-in and session

- Extracted the existing lockout and denied-login audit behavior into one shared Platform authentication service used by MVC and API.
- Added isolated Platform-cookie login, session, overview and logout endpoints; login remains rate-limited and both mutations retain antiforgery.
- Added bilingual Angular sign-in and read-only overview/plan evidence outside the tenant shell. Tenant cookies never satisfy Platform authorization.
- Platform cookie redirects now return API-native 401/403 responses, while MVC redirects remain unchanged.

## MOD-027C evidence

- Web Release: zero warnings/errors; SecurityRegression: 1307 checks.
- Angular: 207/207 tests across 101 files; 114 templates passed accessibility; external SCSS passed.
- Bundles: 323,961/358,400 initial bytes and 81,744/81,920 largest lazy bytes.

## Next slice

## `MOD-027D` delivered — Platform management operations

- Added a Platform-cookie management API covering companies/subscriptions, plans, promotions and pending checkout reads.
- Added Management-protected company update, plan creation and promotion creation commands plus Billing-protected payment confirmation; every mutation retains antiforgery.
- Added a bilingual role-aware Angular management workspace. The browser never supplies a trusted checkout amount or payment state.
- Existing SaaS services and the serializable payment-completion transaction remain authoritative for subscription activation and promotion redemption.

## MOD-027D evidence

- Web Release: zero warnings/errors; SecurityRegression: 1309 checks.
- Angular: 207/207 tests across 101 files; 115 templates passed accessibility; external SCSS passed.
- Bundles: 324,362/358,400 initial bytes and 81,744/81,920 largest lazy bytes.

## Next slice

`MOD-027E` migrates Platform audit-event reading/export and retention/legal-hold operations.
