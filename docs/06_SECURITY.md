# Security assessment
> Status: PARTIALLY VERIFIED
> Source of truth: Static inspection and focused executable regression checks
> Last reviewed: 2026-09-15

## Confirmed findings and fixes
- **Critical — privilege escalation:** Users.Create could assign Admin; Roles.Edit could grant the caller arbitrary permissions through their role. Both permission evaluators now reserve all Users/Roles mutation permissions for current database Admin members. Read access remains delegable. Stored mappings cannot override this rule.
- **High — CSRF:** Roles.Delete mutated state through GET, which global antiforgery deliberately skips. It now requires POST and antiforgery. All five list delete controls now submit POST forms with generated antiforgery tokens; other delete endpoints already required POST.
- **Critical — default credential:** Removed the committed admin password. Bootstrap is disabled by default and requires AdminUser:Enabled plus externally supplied username/email/password. It refuses to promote an existing account. Operators MUST rotate the previously published password and invalidate existing sessions; deleting source text cannot revoke a deployed credential or erase git history.
- **High — password guessing:** Failed login attempts now count toward Identity lockout (5 attempts, 15 minutes). Login POST also has a 10 requests/minute/IP fixed-window limiter, no queue, returning 429 on rejection. Existing accounts with LockoutEnabled=false require an operator update.
- **Medium — error disclosure:** Broad catch(Exception) handlers now log the exception server-side and show a generic message. Typed business-validation exceptions remain visible and require review if new infrastructure operations are added to those paths.
- **Critical — cross-company Admin claim:** Controllers no longer trust the global Identity `Admin` role for company administration. Settings, chart accounts, branches, customers, taxes and payment methods require `Administration.Access`, resolved from `TenantUserRoles` for the active tenant.

## Verified protections and boundaries
Global MVC antiforgery, local returnUrl validation, Identity password hashing, HTTPS redirection and production HSTS already exist. Inspected active data access uses EF LINQ; no raw SQL found. No confirmed XSS was identified in inspected dynamic row rendering; this is not a penetration-test guarantee.

The language switch is an antiforgery-protected POST, accepts only `en-US` and `ar-JO`, uses `LocalRedirect` after validating the return URL, and stores an essential HttpOnly SameSite=Lax culture cookie. Arabic translation values are output through Razor encoding.

Business data is isolated by an authenticated tenant claim backed by an active `TenantMembership` and active `Tenant`. Middleware validates that membership on every authenticated request, upgrades older cookies with a valid tenant claim and signs out users with no active company. EF global query filters scope reads; the DbContext stamps inserts and rejects changes made without a tenant or against another tenant. Every tenant-owned table has a restrictive tenant foreign key. Focused checks verify all 34 protected entity types and the no-tenant write guard.

Public company registration is rate limited to five attempts per hour per connection IP and creates the user, company, owner membership, tenant Admin assignment and trial inside one database transaction. Platform login has its own cookie, lockout-aware password check, rate limit and role policies. Subscription middleware denies tenant ERP routes when the subscription is suspended, cancelled or expired while preserving login, public, platform and subscription-management routes.

Platform login, plan creation and promotion creation forms emit explicit antiforgery tokens. A live GET/POST regression check covers the separate-cookie login form so missing-token failures do not appear as an unexplained HTTP 400.

## Remaining risks / deployment work
- Configure production AllowedHosts; wildcard remains because the deployment hostname is unknown.
- Supply a production SQL connection string with validated TLS certificates; local development configuration trusts the certificate.
- Rate limiting is per process and uses the connection IP. Multi-instance/reverse-proxy deployments need trusted proxy configuration and shared edge limits; do not trust arbitrary forwarded headers.
- Audit history lacks before/after values and tamper-evidence. Production secrets, TLS, backups, access controls and log retention are not verified.
- Identity role definitions and non-Admin role-permission templates remain global. Tenant user-role assignments and Admin checks are isolated, but companies cannot yet independently customize two roles with the same display name and different permission sets.
- Online payment remains unconfigured. The current checkout is a server-calculated quote followed by explicit platform confirmation; a provider adapter, signed webhook validation, idempotency store and refund/dispute handling are required before accepting card payments.
- No deployed HTTP/database penetration test performed. Focused tests use a stub Identity manager, real permission evaluators and action metadata.

## Verification
Run `dotnet run --project tests/SecurityRegression/SecurityRegression.csproj` and `dotnet build MiniStore.slnx -c Release --no-restore`. The 2026-09-15 run passed 167 focused checks.
