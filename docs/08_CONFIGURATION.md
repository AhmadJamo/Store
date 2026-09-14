# Configuration and runtime composition
> Status: IMPLEMENTED WITH PRODUCTION GAPS  
> Source of truth: `Program.cs`, project files, appsettings  
> Last reviewed: 2026-09-15

## Configuration
`MiniStore.Web/appsettings.json` contains `ConnectionStrings:DefaultConnection`, disabled `AdminUser` and `PlatformOwner` bootstrap sections, logging levels and wildcard `AllowedHosts`. No password is stored in current configuration. `PlatformOwner` may explicitly promote a named existing Identity user into the platform allow-list; it is disabled by default.

## Runtime setup
`Program.cs` registers MVC, localization/resources, memory cache, global antiforgery, Identity tenant and platform cookies, tenant/onboarding/SaaS/billing services, platform policies and named login/registration limits. Tenant-session validation runs after authentication; subscription lifecycle enforcement runs before tenant authorization. Request culture uses the user's cookie then the cached company default. Windows Event Log output is disabled, and startup database initialization failures exit cleanly. Startup runs Identity, permission and configured platform-operator seeders. It does not call `Database.Migrate`; migration deployment remains explicit.

## Required values
- SQL Server connection string.
- Admin bootstrap credentials required only when AdminUser:Enabled is true; provide through protected external configuration.
- Platform owner username is required only when PlatformOwner:Enabled is true and must already identify an Identity user.
- Production host names should replace `*`.

## Update rules
DI/configuration changes → update this file, architecture/dependencies/security docs and `AGENTS.md` if operating instructions change.

## Security configuration update (2026-09-13)
AdminUser:Password is no longer stored in appsettings. AdminUser:Enabled defaults to false. For first bootstrap only, provide AdminUser__Enabled=true and AdminUser__Username, AdminUser__Email, AdminUser__Password using protected environment/secret configuration. Use a new username; existing accounts are never promoted. Disable bootstrap and remove its password after success. Existing installations must rotate the former credential separately and invalidate sessions.
Identity lockout is 5 failures for 15 minutes; tenant and platform login POSTs use the login limiter, and company registration permits 5 attempts/hour/IP. Production AllowedHosts, proxy-aware client IP handling and trusted SQL TLS configuration remain operator responsibilities.

