# Migrations and seeding
> Last reviewed: 2026-09-15

Migrations evolve the schema chronologically from the initial ERP through accounting, organized inventory/POS, localization and tenant isolation. The current SaaS tail is `AddSaasControlPlane`, `AddBillingCheckout`, `AddTenantScopedRoles` and `BootstrapPlatformOwner`. `AppDbContextModelSnapshot.cs` is the latest EF model snapshot; the final bootstrap migration is data-only.

Startup invokes Identity, permission and platform-operator seeders. There is no code call to `Database.Migrate`; deployment must apply migrations explicitly. `DocumentNumberSettings` has no observed seeder.
