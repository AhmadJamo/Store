# Configuration and runtime composition
> Status: IMPLEMENTED WITH PRODUCTION GAPS  
> Source of truth: `Program.cs`, project files, appsettings  
> Last reviewed: 2026-09-13

## Configuration
`MiniStore.Web/appsettings.json` contains `ConnectionStrings:DefaultConnection`, `AdminUser` values, logging levels and wildcard `AllowedHosts`. No admin password is stored in the current configuration. `launchSettings.json` is development launch metadata. Required production secret handling is Unknown / Not determined from code.

## Runtime setup
`Program.cs` registers MVC, localization/resources, memory cache, global antiforgery, HttpContext accessor, current-user/tenant/audit services, SQL Server DbContext plus interceptor, Identity, cookie routes, repositories/services, custom authorization provider/handler, static assets, authentication and authorization middleware. Tenant-session validation runs after authentication and before tenant-aware localization/authorization. Request culture uses the user's cookie then the cached tenant-company default. Windows Event Log output is disabled, and startup database initialization failures are caught, logged and returned with exit code 1 instead of escaping as an unhandled Windows application error. At startup the app runs Identity and permission seeders. It does not call `Database.Migrate`; migration deployment remains explicit.

## Required values
- SQL Server connection string.
- Admin bootstrap credentials required only when AdminUser:Enabled is true; provide through protected external configuration.
- Production host names should replace `*`.

## Update rules
DI/configuration changes → update this file, architecture/dependencies/security docs and `AGENTS.md` if operating instructions change.

## Security configuration update (2026-09-13)
AdminUser:Password is no longer stored in appsettings. AdminUser:Enabled defaults to false. For first bootstrap only, provide AdminUser__Enabled=true and AdminUser__Username, AdminUser__Email, AdminUser__Password using protected environment/secret configuration. Use a new username; existing accounts are never promoted. Disable bootstrap and remove its password after success. Existing installations must rotate the former credential separately and invalidate sessions.
Identity lockout is 5 failures for 15 minutes; login POST has a per-connection-IP 10/minute limiter. Production AllowedHosts and trusted SQL TLS configuration remain operator responsibilities.

