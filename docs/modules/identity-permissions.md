# Identity and permissions
> Status: IMPLEMENTED WITH FOLLOW-UPS | Last reviewed: 2026-10-16

ASP.NET Core Identity owns login credentials. Company authorization uses `TenantRole`, `TenantRolePermission` and `TenantUserRole`; startup and onboarding create Admin/WarehouseManager/Sales/Accountant independently for every tenant. Admin is protected and receives the current permission catalogue. Custom roles may reuse a name in different companies and hold different permissions. User/role screens show only the active company's data and are bilingual.

Sources: `Program.cs`, Identity/Permission seeders, tenant role entities/repositories/service, permission definitions, Infrastructure PermissionService, Web authorization classes, Users/Roles controllers and views, navigation definitions. See `05_PERMISSIONS.md` and `06_SECURITY.md` for risk and change rules.

`P1-AUD-012` records custom tenant-role creation, ordinary updates, permission-set changes and deletion as fixed semantic event types. Role, permission links and the audit event now share one `UnitOfWork` (وحدة العمل), so a failed operation leaves no success event. The payload records the role name, permission count and a stable permission-set fingerprint (بصمة مجموعة الصلاحيات), avoiding a long copied permission list and all user identity data or credentials. System-role protections remain unchanged.

`P1-AUD-013` records rejected company and Platform login attempts, including lockout as a distinct fixed reason code, without storing the submitted username or password. It also records an authenticated request only when endpoint authorization actually returns Forbidden (مرفوض), avoiding audit noise from UI visibility checks. These denied events commit through an independent bounded writer; persistence failure is visible instead of silently losing security evidence. Source IP addresses are reduced to `/24` for IPv4 or `/64` for IPv6 and user-agent text is stored only as a SHA-256 fingerprint.

## Security boundary update (2026-09-13)
User/role mutations are Admin-only in both permission evaluators, even when custom roles retain mutation permission rows. Read permissions remain delegable. Role deletion is POST-only with antiforgery. Bootstrap is opt-in, has no committed password and refuses existing accounts. See security/configuration docs for operator rotation requirements.
