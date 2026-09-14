# Tenancy and SaaS foundation
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: tenancy entities, AppDbContext and tenant session middleware  
> Last reviewed: 2026-09-14

## Implemented boundary
MiniStore uses a shared SQL Server database and shared schema. `Tenant` represents a company and `TenantMembership` links a global Identity user to one or more companies. Login currently selects the owner's membership first and otherwise the lowest active tenant ID. The signed Identity cookie carries the selected tenant ID.

Every authenticated request revalidates both the membership and company active state. Cookies created before this feature are upgraded to the user's default active tenant. Users without an active company are signed out.

All 34 current ERP business entity types receive a required shadow `TenantId`. EF global query filters scope reads. AppDbContext stamps added rows, rejects business writes without a tenant and rejects update/delete operations whose original tenant differs. Business unique indexes include TenantId. Tenant and membership tables are control-plane records and do not use the business query filter.

## Current user behavior
- Existing test data belongs to `Demo Company` (`demo-company`).
- Existing users are active Demo Company members; Admin users are marked owners.
- User administration lists only active members of the current company and adds new users to that company.
- Company default language cache is separated by tenant.

## Remaining SaaS work
Tenant administration and signup/onboarding screens, explicit company switching, invitations, tenant-specific role assignments, subscription plans and entitlements, billing/webhooks, platform-operator support, storage quotas, per-tenant background-job context and operational observability remain future phases.

Do not add a business entity without adding it to the tenant-owned model set and the tenant metadata regression checks.
