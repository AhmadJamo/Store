# MiniStore project context
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-09-15

## Project identity
MiniStore is a server-rendered ASP.NET Core MVC store-management application. It targets .NET 10, uses EF Core 10 with SQL Server, ASP.NET Core Identity, Razor views, and a Domain/Application/Infrastructure/Web layered solution. No public API project is present.

## Current development stage
The repository implements a first operational slice for catalog, warehouses, stock, suppliers, purchases, sales (wholesale and POS), configurable discounts, stock transfers, users/roles, settings and shared-database SaaS tenancy. Public signup, pricing, plans, trials, subscription access, promotion codes and a separate platform-owner control center are implemented. It is not yet a complete accounting ERP and does not yet integrate an external payment provider.

## Status classification
- IMPLEMENTED: catalog, warehouses, suppliers, stock balances/movements, purchases, sales, transfer workflow, Identity login, role-permission checks, audit-log rows, Razor UI.
- PARTIALLY IMPLEMENTED: authorization administration, auditability and inventory controls.
- IMPLEMENTED: tenant-scoped centralized numbering for wholesale sales, POS sales, stock transfers and journal entries, including safe defaults and concurrent-edit protection.
- IMPLEMENTED: database-enforced tenant relationship boundaries across all current ERP entities and tenant subscription redemptions, with automatic entity classification and relationship regression checks.
- PLANNED: returns, fiscal periods, external payment-provider/webhook integration, API/integrations.
- UNKNOWN: deployed environment, production migration process, backups, CI/CD, external integrations, and operational ownership.

## Main modules
| Module | Status | Core code | Notes |
|---|---|---|---|
| Products | IMPLEMENTED | `Product`, `ProductService`, `ProductsController` | Name/barcode and three prices. |
| Warehouses | IMPLEMENTED | `Warehouse`, `WarehouseService` | Named warehouse master data. |
| Suppliers | IMPLEMENTED | `Supplier`, `SupplierService` | Supplier master data. |
| Inventory | PARTIALLY IMPLEMENTED | `ProductStock`, `StockTransaction` | Balances and manual movements; see accounting/security risks. |
| Purchases | IMPLEMENTED | `Purchase`, `PurchaseService` | Immediately increases stock. |
| Sales | IMPLEMENTED | `Sale`, `SaleService` | Uses configured product price by channel and decreases stock. |
| Stock transfers | IMPLEMENTED | `StockTransfer`, `StockTransferService` | Draft/submitted/approved/posted/cancelled flow. |
| Settings | PARTIALLY IMPLEMENTED | settings entities/services | General, discount, inventory, accounting, POS and centralized document-number settings. |
| Identity & permissions | PARTIALLY IMPLEMENTED | Identity + `PermissionAuthorizationHandler` | Custom permission rows mapped to roles. |

## Current problems
See `06_SECURITY.md`, `04_ACCOUNTING.md`, and `TODO.md`. Highest-priority findings: rotation of previously committed admin credentials on existing deployments, missing inventory concurrency control, and no accounting ledger. Identity escalation, role-delete CSRF and login lockout have been addressed; see security assessment.

## Recommended next steps
1. Complete the security and inventory-integrity work listed in `TODO.md`.
2. Complete company switching, invitations and existing-user role assignment management on top of the tenant-owned role model.
3. Establish an immutable posted-document and double-entry accounting design before adding more ERP features.

## Read first
Read `01_CODEBASE_MAP.md`, then the relevant module document, then actual source.

## Guided company onboarding update (2026-09-15)
New-company registration now continues into a bilingual guided setup. Business presets atomically create a starter chart of accounts, linked branch and warehouse, payment, tax, numbering, discount, inventory and POS configuration. Owners may skip for manual setup, and tenant login accepts an optional company code (the tenant slug).

