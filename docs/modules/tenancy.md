# Tenancy and SaaS foundation
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: tenancy entities, AppDbContext and tenant session middleware  
> Last reviewed: 2026-10-29

## Implemented boundary
MiniStore uses a shared SQL Server database and shared schema. `Tenant` represents a company and `TenantMembership` links a global Identity user to one or more companies. Login selects the requested active company membership when a company slug is supplied; otherwise it selects the owner's membership first and then the lowest active tenant ID. MVC and Angular use the same authentication service, and the signed Identity cookie carries the selected tenant ID.

Every authenticated request revalidates both the membership and company active state. Cookies created before this feature are upgraded to the user's default active tenant. Users without an active company are signed out.

All 34 current ERP business entity types receive a required shadow `TenantId`. EF global query filters scope reads. AppDbContext stamps added rows, rejects business writes without a tenant and rejects update/delete operations whose original tenant differs. Business unique indexes include TenantId. Every relationship between tenant-bearing records uses a composite TenantId FK, including promotion redemption to subscription. Tenant and membership tables are control-plane records and do not use the business query filter.

## Current user behavior
- Existing test data belongs to `Demo Company` (`demo-company`).
- Existing users are active Demo Company members; Admin users are marked owners.
- User administration lists only active members of the current company and adds new users to that company.
- Company default language cache is separated by tenant.
- Public registration creates a new company owner, active membership, tenant-scoped Admin assignment and 14-day selected-plan trial in one transaction. Its bilingual rules popup explains the server-enforced company/address, account, password and trial requirements before submission.
- `TenantRoles`, `TenantRolePermissions` and `TenantUserRoles` scope definitions, permission sets and assignments to a company; permission and Admin checks use the active tenant rather than global Identity role claims.
- Subscription middleware permits ERP use only for active/trial subscriptions or a valid configured grace period.

## Remaining SaaS work
Explicit company switching, invitations, existing-user assignment management, role data scopes, plan versioning/overrides, external payment webhooks, storage quotas, per-tenant background-job context, platform 2FA and operational observability remain future phases.

## SaaS control plane
The public root and `/pricing` render bilingual product and pricing pages. `/platform` uses a separate short-lived strict cookie and an explicit PlatformOperator with Owner/Admin/Billing policies; tenant Admin does not grant platform access. Operators manage plans, promotions, companies/subscription states and pending payment confirmations. Tenant checkout calculates and stores a 30-minute monthly/annual quote, validates promotion scope and redeems only after confirmation. Online provider integration remains pending.

`P1-AUD-013` records rejected Platform login/lockout without the submitted username, and records company/subscription updates, payment confirmation, plan creation and promotion creation as Platform-scoped semantic events in the same save or transaction. Tenant-scoped execution cannot create these Platform events; normal tenant reads cannot see them.

Do not add a business entity without classifying it in `TenantIsolationModel`. Regression checks reject unclassified mapped Domain entities and tenant relationships that omit TenantId.

## Guided setup and company-code login (2026-09-15)
Registration creates a Pending onboarding record and keeps the owner signed in for /onboarding. Business type and localization, fiscal, tax, inventory and POS choices apply a versioned starter template atomically; Skip records an explicit manual-setup choice. Home and later logins resume pending setup. Login may accept the tenant slug as a company code, which must match the authenticated user's active membership.

Angular now covers the same complete journey. The anonymous `/api/v1/registration` GET exposes public plan DTOs, while its CSRF-protected and rate-limited POST calls the same provisioning service as MVC and issues the same HttpOnly Identity cookie with the validated tenant claim. The protected `/api/v1/company-onboarding` contract requires the tenant `Administration.Access` boundary and exposes GET plus explicit Complete and Skip mutations through `ICompanyOnboardingService`. Passwords are cleared after rejection and are never stored; pending login returns to `/onboarding`, and resolved setup returns to the Angular workspace. MVC remains the default rollback presentation.

The Angular login flow preserves that contract through the shared server service. The company code is never trusted as a tenant selector on later requests: the resulting Identity cookie carries the validated tenant claim, and normal tenant middleware revalidates membership and active company status. Angular stores neither that code as session authority nor any credential/token; its session signal is reconstructed from `/api/v1/session`.

## Angular subscription and checkout (2026-10-29)

`/api/v1/subscription` returns an Application-authored current-subscription summary and the public plan cards for the active
company. Its antiforgery-protected checkout command accepts only plan ID, billing cycle and an optional promotion code;
`BillingCheckoutService` reloads the public plan and calculates price, discount, currency, expiry and pending state on the server.
The Angular `/subscription` pilot presents the bilingual lifecycle evidence and server quote without storing payment authority.
Entitlement middleware bypasses only the session bootstrap, subscription API and subscription page needed for recovery; other ERP
APIs remain blocked when the subscription is unusable. Manual Platform confirmation remains authoritative until a provider exists.

The anonymous `/api/v1/public/plans` endpoint now projects the same public plan cards through `PlatformSaasService` for the
Angular welcome/pricing pilot. It exposes no mutation and no private plan. Registration links carry only a plan ID and the
registration service reloads plan eligibility before provisioning.

## Angular Platform control plane (2026-10-30)

The isolated Platform cookie now supports Angular session/overview, role-separated SaaS management, audit search/export and
owner retention workspaces. Audit evidence is projected by `AuditEventReadService`; the API alone evaluates
`AuditSensitive`, while export requires its separate policy, antiforgery, an approved purpose and writes its own audit event.
Legal holds, releases, external checkpoints and retention runs remain owned by `AuditRetentionService`; Angular supplies
commands only and cannot delete evidence directly. MVC remains the default rollback presentation.
