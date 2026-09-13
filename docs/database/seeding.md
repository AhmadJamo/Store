# Database seeding
> Source of truth: startup seeders | Last reviewed: 2026-09-13

At application startup, `IdentitySeeder` ensures roles Admin, WarehouseManager, Sales and Accountant and creates/configures an Admin user from configuration. `PermissionSeeder` ensures permission definitions and observed default role permission mappings. The exact existing-database result depends on data and code execution.

No seed was found for products, warehouses, suppliers, stock, general/discount/invoice settings, or document number settings. Secrets in Identity configuration are intentionally not repeated here.
