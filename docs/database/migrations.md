# Migrations and seeding
> Last reviewed: 2026-09-15

Migrations evolve the schema chronologically from the initial ERP through accounting, inventory/POS, localization and tenancy. The current tail includes `EnforceTenantRoleAssignmentBoundary`, `HardenTenantIsolationRelationships` and `HardenTenantSubscriptionRedemption`. The hardening migrations add alternate tenant keys and composite foreign keys across ERP relationships and the subscription-redemption relationship. Both applied successfully to the local two-company test database. `AppDbContextModelSnapshot.cs` is the latest EF model snapshot.

Startup invokes Identity, permission and platform-operator seeders. There is no code call to `Database.Migrate`; deployment must apply migrations explicitly. `UnifyDocumentNumbering` copies legacy sales/POS/transfer settings, inserts any missing tenant defaults and then removes the legacy tables. Runtime generation also creates a missing sequence safely inside the calling document transaction.

## 20260915140248_AddCompanyGuidedOnboarding
Creates one onboarding-state row per tenant and backfills existing development companies as Skipped so their current manual setup remains unchanged. New registration inserts Pending explicitly.
