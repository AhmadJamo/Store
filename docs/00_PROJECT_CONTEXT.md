# MiniStore project context
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-09-30

## Project identity
MiniStore is a server-rendered ASP.NET Core MVC store-management application. It targets .NET 10, uses EF Core 10 with SQL Server, ASP.NET Core Identity, Razor views, and a Domain/Application/Infrastructure/Web layered solution. No public API project is present.

## Current development stage
The repository implements an operational slice for catalog, warehouses, stock, suppliers, purchases, sales (wholesale and POS), configurable discounts, stock transfers, users/roles, settings and shared-database SaaS tenancy. Public signup, pricing, plans, trials, subscription access, promotion codes and a separate platform-owner control center are implemented. Product classification, generated internal codes, advanced catalog search, managed measurement units, immutable prepared-product recipes, automatic ingredient consumption and controlled kitchen-negative stock are also implemented. It is not yet a complete accounting ERP and does not yet integrate an external payment provider.

The active product-development track is now the additive WMS evolution documented in `WMS_EVOLUTION_PLAN.md`. WMS-000 provides reconciliation and SQL concurrency verification. WMS-005 adds product logistics metadata and WMS-010 adds hierarchical locations. WMS-015 provides typed attributes and controlled variants. WMS-020A introduces the additive StockMovement kernel and dual-writes new Putaway/Relocation operations while preserving legacy history. InventoryBalance is not yet implemented.

## Status classification
- IMPLEMENTED: catalog, warehouses, suppliers, stock balances/movements, purchases, sales, transfer workflow, Identity login, role-permission checks, audit-log rows, Razor UI.
- PARTIALLY IMPLEMENTED: authorization administration, auditability and inventory controls.
- IMPLEMENTED: tenant-scoped centralized numbering for wholesale sales, POS sales, stock transfers and journal entries, including safe defaults and concurrent-edit protection.
- IMPLEMENTED: database-enforced tenant relationship boundaries across all current ERP entities and tenant subscription redemptions, with automatic entity classification and relationship regression checks.
- IMPLEMENTED: posted sales and purchase returns plus tenant fiscal-period posting controls.
- PLANNED: generalized reversals, external payment-provider/webhook integration, API/integrations.
- UNKNOWN: deployed environment, production migration process, backups, CI/CD, external integrations, and operational ownership.

## Main modules
| Module | Status | Core code | Notes |
|---|---|---|---|
| Products | IMPLEMENTED | `Product`, `ProductService`, `ProductsController` | Generated code, optional barcode, raw/direct/prepared type, managed stock unit, channel controls and advanced catalog search. |
| Warehouses | IMPLEMENTED | `Warehouse`, `WarehouseService` | Named warehouse master data. |
| Suppliers | IMPLEMENTED | `Supplier`, `SupplierService` | Supplier master data. |
| Inventory | PARTIALLY IMPLEMENTED | `ProductStock`, `StockTransaction`, recipes, measurement units | Balances, moving-average valuation, auditable cost movements, managed-unit conversion, prepared recipes, controlled negative variance and purchase-time variance settlement; manual adjustment posting remains pending. |
| Purchases | IMPLEMENTED | `Purchase`, `PurchaseService`, `PurchaseReturnService` | Receipts increase stock; posted partial/full supplier returns remove available stock at moving-average cost and reverse accounting. |
| Sales | IMPLEMENTED | `Sale`, `SaleService`, `SalePostingService` | Uses configured channel price, optional frozen inclusive/exclusive tax, decreases stock and posts revenue/output tax/COGS. |
| Stock transfers | IMPLEMENTED | `StockTransfer`, `StockTransferService` | Draft/submitted/approved/posted/cancelled flow. |
| Settings | PARTIALLY IMPLEMENTED | settings entities/services | General, discount, inventory, accounting, fiscal periods, POS and centralized document-number settings. |
| Identity & permissions | PARTIALLY IMPLEMENTED | Identity + `PermissionAuthorizationHandler` | Custom permission rows mapped to roles. |

## Current problems
See `06_SECURITY.md`, `04_ACCOUNTING.md`, and `TODO.md`. Highest-priority findings include rotating previously committed admin credentials on existing deployments, completing inventory valuation and accounting posting, and establishing production deployment/backup/integration-test controls. Identity escalation, role-delete CSRF, login lockout and core inventory concurrency have been addressed; see the security and inventory documentation.

## Recommended next steps
1. Smoke-test WMS-010 through the authenticated UI, then continue to the next WMS vertical slice.
2. Complete company switching, invitations and existing-user role assignment management on top of the tenant-owned role model.
3. Establish an immutable posted-document and double-entry accounting design before adding more ERP features.

## Read first
Read `01_CODEBASE_MAP.md`, then the relevant module document, then actual source.

## Guided company onboarding update (2026-09-15)
New-company registration now continues into a bilingual guided setup. Business presets atomically create a starter chart of accounts, linked branch and warehouse, payment, tax, numbering, discount, inventory and POS configuration. Owners may skip for manual setup, and tenant login accepts an optional company code (the tenant slug).

