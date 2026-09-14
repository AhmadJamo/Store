# Tenancy entities
> Last reviewed: 2026-09-15

## Tenant
`Tenant` is the company boundary. It stores a validated name, a globally unique lowercase slug, active state, creation time and SQL Server rowversion. Deactivating the tenant prevents its memberships from authorizing a session.

## TenantMembership
`TenantMembership` uses `(TenantId, UserId)` as its key and links to `Tenant` and `AspNetUsers`. `IsActive` controls access and `IsOwner` identifies the company owner. Membership is control-plane data, so it is queried explicitly and is not hidden by tenant business-data filters.

## TenantUserRole
`TenantUserRole` uses `(TenantId, UserId, RoleId)` and links an active company member to an Identity role inside that company. Authorization never treats a global Admin claim as sufficient for tenant administration. Identity role definitions and non-Admin permission templates are still shared and remain a future isolation step.

Business entities use a shadow TenantId managed centrally by AppDbContext. Keeping the property shadowed in this first phase avoids duplicating tenant plumbing in all domain constructors while preserving database ownership and EF enforcement.
