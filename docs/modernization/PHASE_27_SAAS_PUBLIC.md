# Phase 27 — SaaS and public Angular migration

> Status: COMPLETE — `MOD-027A..E` delivered on 2026-10-30
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

## `MOD-027E` delivered — Platform audit and retention

- Added isolated Platform-cookie APIs for filtered/paged audit evidence and purpose-bound CSV export. The server derives
  sensitive-field inclusion from `AuditSensitive`; Angular cannot request or infer it.
- Preserved separate `AuditView` and `AuditExport` authorization, antiforgery on export, the 5,000-record limit,
  redaction and the semantic export event.
- Added an owner-only retention API and bilingual RTL-safe workspace for policy evidence, legal-hold creation/release,
  external integrity checkpoints and retention execution. Every mutation retains antiforgery and uses
  `AuditRetentionService`, including audit receipts and legal-hold rechecks.
- Platform navigation exposes management, audit and retention destinations only as session guidance; every API
  independently authorizes the isolated Platform cookie and policy. MVC remains the default presentation.

## MOD-027E evidence

- Web Release: zero warnings/errors; SecurityRegression: 1311 checks.
- Angular: 207/207 tests across 101 files; 117 templates passed accessibility; external SCSS passed.
- Bundles: 324,732/358,400 initial bytes and 81,744/81,920 largest lazy bytes.
- Browser and performance checks were skipped by the standard local step flags; all seven applicable gate stages passed.

`MOD-027` is complete. Execution advances to `MOD-028A`, a source-authoritative reconciliation of shared and remaining
MVC surfaces before selecting their implementation slices.

## `MOD-030E5M` runtime cutover

- Tenant Subscription Index GET now defaults to the existing Angular `/app/subscription` workspace and preserves query values.
- Checkout POST, server-authored pricing, promotion validation and subscription activation remain outside presentation cutover.
- Hosted acceptance passed 15/15; Angular cutover and process-local MVC rollback each passed 129 authorized GET routes;
  1,776 backend checks passed without creating a checkout or changing subscription state.

## `MOD-030E5N` runtime cutover

- Anonymous Public landing and pricing GET now default to `/app/welcome` and `/app/pricing`, preserving query values.
- Registration, selected-plan validation and every SaaS mutation remain outside presentation cutover.
- Hosted acceptance passed 15/15; anonymous Angular cutover and process-local MVC rollback each passed within 131 GET
  routes; 1,782 backend checks passed without registration or plan/subscription mutation.

## `MOD-030I` physical-retirement audit

- Confirmed the Angular welcome/pricing and anonymous public-plan API remain the functional replacements.
- Classified the MVC Public surface as R1-A because its exact `/` and `/pricing` paths are anonymous and cannot use the
  current permission-protected conventional retired-route catalog unchanged.
- No controller, view, API or route was deleted. Registration, selected-plan revalidation, rate limiting, cookies,
  onboarding, culture switching and shared `_PublicLayout` remain intact.
- Any physical retirement is deferred to a separately approved MOD-030J with exact anonymous-safe routes and browser proof.

## `MOD-030J` physical retirement

- Removed only `PublicController` and its two landing/pricing Razor views after explicit approval.
- Exact anonymous GET/HEAD redirects now preserve `/`, `/pricing` and query values while targeting Angular.
- Registration, plan revalidation, authentication, rate limiting, cookies, onboarding, the public-plan API and shared
  `_PublicLayout` remain unchanged.
- Hosted route probes passed and Microsoft Edge acceptance passed 146 authorized routes; rollback deploys the previous build.

## `MOD-030E5O` runtime cutover

- Anonymous PlatformAccount Login GET now defaults to `/app/platform-login`, preserving returnUrl and rate-limit evidence.
- Credential submission, login rate limiting, Platform-cookie issue and logout remain outside presentation cutover.
- Hosted acceptance passed 15/15; anonymous Angular cutover and process-local MVC rollback each passed within 132 GET
  routes; 1,786 backend checks passed without a credential attempt or Platform session mutation.

## `MOD-030E5P` runtime cutover

- Platform overview and management GET actions now default to `/app/platform-control` and `/app/platform-management`.
- Company, plan, promotion and payment mutations retain their Platform policies, antiforgery and server authority outside cutover.
- Hosted acceptance passed 15/15; the existing authorized tenant route set passed cutover/MVC rollback 132/132 and proved
  live tenant-cookie isolation from Platform. Exact Platform mappings and GET-only boundaries passed within 1,802 backend
  checks. No Platform session fixture was available for authorized live route probes, and no Platform data changed.

## `MOD-030E5Q` runtime cutover

- PlatformAuditEvents Index GET now defaults to `/app/platform-audit`, preserving audit filters.
- Export, sensitive-field authorization and semantic export-event authority remain outside presentation cutover.
- Hosted 15/15, existing route-set cutover/MVC rollback 132/132, live tenant-cookie isolation and 1,806 backend checks
  passed without export or audit mutation. No Platform session fixture was available for an authorized live route probe.

## `MOD-030E5R` runtime cutover

- PlatformAuditRetention Index GET now defaults to `/app/platform-retention`.
- Legal holds, checkpoints and retention execution remain Owner-only, antiforgery-protected and outside cutover.
- Hosted 15/15, existing route-set cutover/MVC rollback 132/132, live tenant-cookie isolation and 1,810 backend checks
  passed without mutation. No Platform session fixture was available for an authorized live route probe.
