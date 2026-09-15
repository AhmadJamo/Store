# ADR: Shared-database tenant data isolation

- Date: 2026-09-14
- Status: Accepted

## Context
MiniStore was built for one company. Converting it to SaaS requires a company boundary before subscriptions, billing or onboarding can safely share one deployment. Existing database rows are test data and may be assigned to one demo company.

## Decision
Use one SQL Server database and one schema. Store companies in `Tenants` and user access in `TenantMemberships`. Resolve the active tenant from a server-signed Identity claim and verify the membership and tenant on every authenticated request.

Apply required shadow TenantId ownership to every business entity through AppDbContext. Use EF query filters for reads, SaveChanges enforcement for mutations, restrictive tenant foreign keys and tenant-aware unique indexes. Keep global Identity, permission definitions and tenant membership as control-plane data.

Migration `AddTenantIsolation` creates Demo Company, assigns existing test rows and users to it, and removes the temporary defaults used during backfill.

## Consequences
Repositories and application services automatically see only the active company's rows. A business write cannot proceed without an active tenant. Codes and document numbers may repeat between companies.

This phase originally left Identity roles global. `2026-09-15-tenant-owned-roles.md` supersedes that limitation with company-owned role definitions, permission sets and assignments. Company switching remains a separate workflow.
