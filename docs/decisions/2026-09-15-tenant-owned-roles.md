# ADR: Tenant-owned role definitions
> Status: Accepted | Date: 2026-09-15

## Context

TenantUserRole previously pointed to global ASP.NET Identity roles. Assignments were company-scoped, but role names and non-Admin permission templates were shared. A company could therefore see roles created for another company, and two companies could not independently define the same role name.

## Decision

Identity remains the credential store. ERP authorization uses `TenantRole`, `TenantRolePermission` and `TenantUserRole`. Role names are unique by `(TenantId, NormalizedName)`, permission selections belong to the tenant role, and assignments reference `(TenantRoleId, TenantId)` through a composite foreign key. Admin is a protected system role and remains the only authority for user and role mutations; those sensitive capabilities are excluded from delegable custom-role choices.

Controllers call `TenantRoleService`; repositories perform company-filtered persistence. Assignment requires an active membership and a role owned by the same company. New-company onboarding creates independent default roles and assigns the owner to its protected Admin role.

## Consequences

Different companies may use the same role name with different permissions, and global Identity role claims no longer define ERP access. `AddTenantOwnedRoleDefinitions` copies legacy roles, permission mappings and assignments before removing RolePermissions; `EnforceTenantRoleAssignmentBoundary` adds the database-level company boundary. Existing-user assignment editing, invitations, data scopes, grant ceilings and detailed authorization audit history remain follow-up work.
