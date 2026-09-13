# Identity and permissions
> Status: PARTIALLY IMPLEMENTED | Last reviewed: 2026-09-13

Identity users/roles use ASP.NET Core Identity. Startup seed creates default Admin/WarehouseManager/Sales/Accountant roles, configured admin and permission catalogue/default mappings. MVC custom permission policy checks database mapping; Admin bypasses checks. Users screens list/create users; Roles screens CRUD non-Admin roles and their permissions.

Sources: `Program.cs`, Identity/Permission seeders, Permission entity/mapping, application definitions/service contract, Infrastructure PermissionService, Web authorization classes, Users/Roles controllers and views, navigation definitions. See `05_PERMISSIONS.md` and `06_SECURITY.md` for risk and change rules.

## Security boundary update (2026-09-13)
User/role mutations are Admin-only in both permission evaluators, even when custom roles retain mutation permission rows. Read permissions remain delegable. Role deletion is POST-only with antiforgery. Bootstrap is opt-in, has no committed password and refuses existing accounts. See security/configuration docs for operator rotation requirements.
