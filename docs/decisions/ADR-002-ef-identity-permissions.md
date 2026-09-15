# ADR-002: ASP.NET Core Identity with database role permissions
> Status: SUPERSEDED by `2026-09-15-tenant-owned-roles.md`
> Last reviewed: 2026-09-15

## Context and decision
Identity manages users/roles. A permission catalogue is seeded; RolePermissions maps Identity role IDs to permissions. Dynamic policies and a handler check those mappings; Admin role bypasses them.

## Consequences
Permission checks are centralized for annotated backend actions. Settings instead uses Admin role directly, and privilege boundaries around role assignment need strengthening. Original alternatives/rationale: Unknown / Not determined from code.
