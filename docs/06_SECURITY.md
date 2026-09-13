# Security assessment
> Status: PARTIALLY VERIFIED
> Source of truth: Static inspection and focused executable regression checks
> Last reviewed: 2026-09-13

## Confirmed findings and fixes
- **Critical — privilege escalation:** Users.Create could assign Admin; Roles.Edit could grant the caller arbitrary permissions through their role. Both permission evaluators now reserve all Users/Roles mutation permissions for current database Admin members. Read access remains delegable. Stored mappings cannot override this rule.
- **High — CSRF:** Roles.Delete mutated state through GET, which global antiforgery deliberately skips. It now requires POST and antiforgery. All five list delete controls now submit POST forms with generated antiforgery tokens; other delete endpoints already required POST.
- **Critical — default credential:** Removed the committed admin password. Bootstrap is disabled by default and requires AdminUser:Enabled plus externally supplied username/email/password. It refuses to promote an existing account. Operators MUST rotate the previously published password and invalidate existing sessions; deleting source text cannot revoke a deployed credential or erase git history.
- **High — password guessing:** Failed login attempts now count toward Identity lockout (5 attempts, 15 minutes). Login POST also has a 10 requests/minute/IP fixed-window limiter, no queue, returning 429 on rejection. Existing accounts with LockoutEnabled=false require an operator update.
- **Medium — error disclosure:** Broad catch(Exception) handlers now log the exception server-side and show a generic message. Typed business-validation exceptions remain visible and require review if new infrastructure operations are added to those paths.

## Verified protections and boundaries
Global MVC antiforgery, local returnUrl validation, Identity password hashing, HTTPS redirection and production HSTS already exist. Inspected active data access uses EF LINQ; no raw SQL found. No confirmed XSS was identified in inspected dynamic row rendering; this is not a penetration-test guarantee.

## Remaining risks / deployment work
- Configure production AllowedHosts; wildcard remains because the deployment hostname is unknown.
- Supply a production SQL connection string with validated TLS certificates; local development configuration trusts the certificate.
- Rate limiting is per process and uses the connection IP. Multi-instance/reverse-proxy deployments need trusted proxy configuration and shared edge limits; do not trust arbitrary forwarded headers.
- Stock updates lack concurrency control; manual stock movements permit document-owned types. These existing financial-integrity issues remain tracked in TODO and require coordinated inventory changes.
- Audit history lacks before/after values and tamper-evidence. Production secrets, TLS, backups, access controls and log retention are not verified.
- No deployed HTTP/database penetration test performed. Focused tests use a stub Identity manager, real permission evaluators and action metadata.

## Verification
Run `dotnet run --project tests/SecurityRegression/SecurityRegression.csproj` and `dotnet build MiniStore.Web/MiniStore.Web.csproj --no-restore`.
