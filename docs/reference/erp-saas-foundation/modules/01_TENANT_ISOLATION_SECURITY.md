# Tenant Isolation & Security
## Model
TenantRegistry(TenantId, Status, StampId, DbLocatorKey, Region, Version); TenantMembership(UserId,TenantId,Roles,Status); TenantContext is immutable request-scoped and derived from verified token/session and membership, never request-supplied TenantId alone. User may belong to multiple tenants but must explicitly select active membership.

## Enforcement
TenantId non-null on tenant-owned aggregates; composite unique indexes and composite FK constraints when feasible; EF Core query filters as defense-in-depth only; authorization in application services; SQL Server RLS evaluate where practical (session context must be set/reset safely with pooled connections). Raw SQL, admin scripts, exports, worker payloads, cache keys, search indexes, file paths and object ACLs all tenant-scoped. Platform support access time-bound, audited, least privilege. Tenant context must never silently default.

## Threats/tests
IDOR across tenant A/B; crafted body TenantId; cross-tenant GUID lookup; raw SQL bypass; cache collisions; background job replay; stale membership token; support impersonation; tenant-switch racing; missing tenant context must fail closed. Include OWASP ASVS aligned security review.

## Migration
Inventory ownership of all tables, add nullable TenantId, map historical ownership with evidence, backfill in chunks, quarantine ambiguous records, add constraints/indexes, switch to required TenantId only after verification. Never guess historical ownership.
