# Identity and permissions
> Status: IMPLEMENTED WITH FOLLOW-UPS | Last reviewed: 2026-09-15

ASP.NET Core Identity owns login credentials. Company authorization uses `TenantRole`, `TenantRolePermission` and `TenantUserRole`; startup and onboarding create Admin/WarehouseManager/Sales/Accountant independently for every tenant. Admin is protected and receives the current permission catalogue. Custom roles may reuse a name in different companies and hold different permissions. User/role screens show only the active company's data and are bilingual.

Sources: `Program.cs`, Identity/Permission seeders, tenant role entities/repositories/service, permission definitions, Infrastructure PermissionService, Web authorization classes, Users/Roles controllers and views, navigation definitions. See `05_PERMISSIONS.md` and `06_SECURITY.md` for risk and change rules.

## Security boundary update (2026-09-13)
User/role mutations are Admin-only in both permission evaluators, even when custom roles retain mutation permission rows. Read permissions remain delegable. Role deletion is POST-only with antiforgery. Bootstrap is opt-in, has no committed password and refuses existing accounts. See security/configuration docs for operator rotation requirements.
