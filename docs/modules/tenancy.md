# Tenancy and SaaS foundation
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: tenancy entities, AppDbContext and tenant session middleware  
> Last reviewed: 2026-09-15

## Implemented boundary
MiniStore uses a shared SQL Server database and shared schema. `Tenant` represents a company and `TenantMembership` links a global Identity user to one or more companies. Login currently selects the owner's membership first and otherwise the lowest active tenant ID. The signed Identity cookie carries the selected tenant ID.

Every authenticated request revalidates both the membership and company active state. Cookies created before this feature are upgraded to the user's default active tenant. Users without an active company are signed out.

All 34 current ERP business entity types receive a required shadow `TenantId`. EF global query filters scope reads. AppDbContext stamps added rows, rejects business writes without a tenant and rejects update/delete operations whose original tenant differs. Business unique indexes include TenantId. Tenant and membership tables are control-plane records and do not use the business query filter.

## Current user behavior
- Existing test data belongs to `Demo Company` (`demo-company`).
- Existing users are active Demo Company members; Admin users are marked owners.
- User administration lists only active members of the current company and adds new users to that company.
- Company default language cache is separated by tenant.
- Public registration creates a new company owner, active membership, tenant-scoped Admin assignment and 14-day selected-plan trial in one transaction.
- `TenantUserRoles` scopes each user's role assignment to a company; permission and Admin checks use the active tenant rather than global Identity role claims.
- Subscription middleware permits ERP use only for active/trial subscriptions or a valid configured grace period.

## Remaining SaaS work
Explicit company switching, invitations, fully tenant-owned custom role definitions/permission sets, plan versioning/overrides, external payment webhooks, storage quotas, per-tenant background-job context, platform 2FA and operational observability remain future phases.

## SaaS control plane
The public root and `/pricing` render bilingual product and pricing pages. `/platform` uses a separate short-lived strict cookie and an explicit PlatformOperator with Owner/Admin/Billing policies; tenant Admin does not grant platform access. Operators manage plans, promotions, companies/subscription states and pending payment confirmations. Tenant checkout calculates and stores a 30-minute monthly/annual quote, validates promotion scope and redeems only after confirmation. Online provider integration remains pending.

Do not add a business entity without adding it to the tenant-owned model set and the tenant metadata regression checks.
