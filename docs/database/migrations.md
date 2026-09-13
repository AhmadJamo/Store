# Migrations and seeding
> Last reviewed: 2026-09-13

Migrations in `MiniStore.Infrastructure/Migrations` evolve the schema in this observed order: initial core; stock transactions; suppliers; purchases; Identity; permissions; sales; sales invoicing; wholesale pricing; audit log; general settings and its singleton/concurrency cleanup; discount settings; sale discounts; stock transfers. `AppDbContextModelSnapshot.cs` is the latest EF model snapshot.

Startup invokes `IdentitySeeder` (roles and configured admin) and `PermissionSeeder` (permission definitions and default role assignments). There is no code call to `Database.Migrate`; production migration execution is Unknown / Not determined from code. `DocumentNumberSettings` has no observed seeder.
