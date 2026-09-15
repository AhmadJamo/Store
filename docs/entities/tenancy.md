# Tenancy entities
> Last reviewed: 2026-09-15

## Tenant
`Tenant` is the company boundary. It stores a validated name, a globally unique lowercase slug, active state, creation time and SQL Server rowversion. Deactivating the tenant prevents its memberships from authorizing a session.

## TenantMembership
`TenantMembership` uses `(TenantId, UserId)` as its key and links to `Tenant` and `AspNetUsers`. `IsActive` controls access and `IsOwner` identifies the company owner. Membership is control-plane data, so it is queried explicitly and is not hidden by tenant business-data filters.

## TenantUserRole
`TenantRole` stores a company's role name, normalized tenant-local key, description, protected-system flag and rowversion. `TenantRolePermission` selects from the global technical permission catalogue. `TenantUserRole` uses `(TenantId, UserId, TenantRoleId)` and links an active member to a role owned by the same company. Authorization never treats a global Identity role claim as tenant access. Two companies may use the same role name with different permissions.

Business entities use a shadow TenantId managed centrally by AppDbContext. Keeping the property shadowed avoids duplicating tenant plumbing in all domain constructors while preserving database ownership and EF enforcement. `TenantIsolationModel` is the immutable classification source; composite foreign keys append TenantId to every relationship between tenant-owned records.

## CompanyOnboarding
One control-plane row per tenant stores Pending, Completed or Skipped state, the selected business type, country, currency, language, fiscal-year start, tax, POS and inventory choices, template version, completing user/time and rowversion. A resolved record rejects reapplication. Existing companies are backfilled as Skipped to preserve prior manual configuration.
