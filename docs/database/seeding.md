# Database seeding
> Source of truth: startup seeders | Last reviewed: 2026-09-15

At startup, `IdentitySeeder` ensures Admin, WarehouseManager, Sales and Accountant and optionally creates a configured Admin user. It attaches that bootstrap Admin to the active tenant. `PermissionSeeder` ensures the permission catalogue and Admin template. `PlatformOperatorSeeder` promotes only an explicitly configured existing username when enabled. Migration `BootstrapPlatformOwner` creates one initial platform owner for an upgraded test database when none exists; future tenant Admin users are never promoted implicitly.

No seed was found for products, warehouses, suppliers, stock, general/discount/invoice settings, or document number settings. Secrets in Identity configuration are intentionally not repeated here.
