# Configuration and runtime composition
> Status: IMPLEMENTED WITH PRODUCTION GAPS  
> Source of truth: `Program.cs`, `MiniStore.Web/Configuration/*.cs`, project files, appsettings
> Last reviewed: 2026-10-24

## Configuration
`MiniStore.Web/appsettings.json` contains `ConnectionStrings:DefaultConnection`, disabled `AdminUser`, `PlatformOwner`, `AuditEvents` and `AuditRetention` sections, `Observability`, registration rate-limit values, logging levels and wildcard `AllowedHosts`. No password, integrity key, checkpoint key or local archive path is committed. Audit allowlists and approved 7/3-year, 90-day policy values are schema/policy configuration, not secret material. Optional `AuditReadConnection` and `AuditMaintenanceConnection` are external secrets; absence falls back to the development connection and is not acceptable proof of production identity separation.

`LegacyAudit:WriteEnabled` independently controls only future generic `AuditLogs` creation. The code fallback is `true`
when configuration is absent; the completed P1-AUD-016 configuration explicitly sets it to `false`. A temporary
rollback sets it to `true` and restarts the application. Neither value changes historical rows or semantic events.

`Observability:Enabled` controls request instruments; `LogServerErrors` controls structured HTTP 500 alert logs and
`SlowRequestThresholdMilliseconds` controls the warning threshold. The defaults are true, true and 1000 ms. The
health endpoints remain available if request instrumentation is disabled. Production must connect the stable
`MiniStore.Web` meter and logs to its chosen protected collector and alert destination.

## Runtime setup
`Program.cs` only composes five explicit startup concerns. `WebPresentationExtensions` owns MVC, localization, antiforgery, cache, throttling and health registrations; `PersistenceExtensions` owns DbContext and audit interception; `SecurityExtensions` owns Identity, cookies and authorization; `BusinessServicesExtensions` groups explicit repository/application registrations by feature; and `WebApplicationExtensions` owns startup seeding, request telemetry, secret-free `/health/live` and `/health/ready`, middleware and routes. Tenant-session validation runs after authentication; subscription lifecycle enforcement runs before tenant authorization. Request culture uses the user's cookie then the cached company default. Windows Event Log output is disabled, and startup database initialization failures exit cleanly. Startup runs Identity, permission and configured platform-operator seeders. It does not call `Database.Migrate`; migration deployment remains explicit.

`P1-AUD-015` registers one retention worker. When `AuditRetention:Enabled=true`, it runs immediately and then every configured 1–168 hours (24 by default). Each run creates a signed external checkpoint before selecting candidates; no row is removed unless a signed destruction manifest exists and the maintenance transaction revalidates its ID, integrity fingerprint and absence of an active legal hold.

PUR-240 registers one hosted replenishment scanner. It checks due settings once per minute, creates a fresh service scope and explicit ambient tenant scope for each due company, then invokes the Application service. Company frequency remains data-driven (1–168 hours); the one-minute scan interval is not a replenishment frequency setting. Multi-host production deployment must coordinate scheduler ownership or add a distributed lease.

## Angular development configuration

`MiniStore Front-End/proxy.conf.json` proxies only `/api` and `/Account` to the local HTTPS ASP.NET Core host. The target is a local-development value, contains no credential and must be aligned with the backend launch profile when that profile changes. Production uses a same-origin deployment or trusted reverse proxy; enabling broad CORS is not the default. `.nvmrc` pins Node 24.20.0 and `package.json` restricts the supported major line to Node 24.

The internal API uses the existing Identity cookie plus `MiniStore.Antiforgery`, a strict same-site HttpOnly antiforgery cookie. Angular obtains the request token from `/api/v1/security/csrf` and sends it in `X-CSRF-TOKEN` for unsafe `/api/` methods. No token or secret is stored in browser local storage. OpenAPI is available at `/openapi/internal-v1.json` only after authentication.

Angular stores only versioned display preferences: `ministore.ui.preferences.v1` for theme/density and `ministore.ui.locale` for the supported UI locale. Corrupt or inaccessible storage falls back to safe defaults; reset removes the visual preference. These keys must never contain identity, permissions, API tokens or business payloads.

## Required values
- SQL Server connection string.
- Admin bootstrap credentials required only when AdminUser:Enabled is true; provide through protected external configuration.
- Platform owner username is required only when PlatformOwner:Enabled is true and must already identify an Identity user.
- Production host names should replace `*`.
- `AuditEvents:ActiveIntegrityKeyVersion` and a matching entry under `AuditEvents:IntegrityKeys:<version>` containing a Base64 key of at least 256 bits are required from protected external configuration only when `AuditEvents:Enabled=true`. Per-event change/metadata fields and reason text remain denied unless explicitly allowlisted. Rotation adds a new key/version and changes the active version; retained old key entries remain available for verification.
- Local Demo is enabled through its project User Secrets with active version `v1` and a verified 32-byte random key. The value must never be printed, documented, copied to appsettings or removed while `v1` events remain verifiable.
- `AuditRetention:ArchiveDirectory` and an independent Base64 `AuditRetention:CheckpointKey` of at least 256 bits are required when retention is enabled. Local Demo keeps both plus `AuditRetention:Enabled=true` in User Secrets. Production uses monitored off-host storage, assigns real identities to the migration-created SQL roles and supplies the optional read/maintenance connection strings.

## Update rules
DI/configuration changes → update this file, architecture/dependencies/security docs and `AGENTS.md` if operating instructions change.

## Security configuration update (2026-09-13)
AdminUser:Password is no longer stored in appsettings. AdminUser:Enabled defaults to false. For first bootstrap only, provide AdminUser__Enabled=true and AdminUser__Username, AdminUser__Email, AdminUser__Password using protected environment/secret configuration. Use a new username; existing accounts are never promoted. Disable bootstrap and remove its password after success. Existing installations must rotate the former credential separately and invalidate sessions.
The owner-declared local Demo environment completed that rotation on 2026-10-15. The replacement administrator secret is held only in the local development `User Secrets` store under id `MiniStore-Demo`; bootstrap remains false in committed configuration. A separate stale `BootstrapAdmin:Password` key identified by the owner was removed from its local secret store. No staging or production configuration is evidenced in this repository.
On 2026-10-17 the owner explicitly authorized using the existing `PlatformOwner` startup seeder once to promote that rotated Demo administrator to active Platform Owner. The local project-secret enable flag was reset to false and the temporary username key was removed after the successful startup. A real platform form login was verified; no credential value was copied into the repository.
Identity lockout is 5 failures for 15 minutes; tenant and platform login POSTs use the login limiter. Company registration defaults to 20 submissions/hour/IP and can be tuned through the `RateLimiting:Registration` configuration section. A rejected login or registration returns to its form with a bilingual wait message and `Retry-After` response header. Production AllowedHosts, proxy-aware client IP handling and trusted SQL TLS configuration remain operator responsibilities.

