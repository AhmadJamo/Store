# MiniStore project context
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-10-16

## Project identity
MiniStore is a server-rendered ASP.NET Core MVC store-management application. It targets .NET 10, uses EF Core 10 with SQL Server, ASP.NET Core Identity, Razor views, and a Domain/Application/Infrastructure/Web layered solution. No public API project is present.

## Current development stage
The repository implements an operational slice for catalog, warehouses, stock, suppliers, purchases, sales (wholesale and POS), configurable discounts, stock transfers, users/roles, settings and shared-database SaaS tenancy. Public signup, pricing, plans, trials, subscription access, promotion codes and a separate platform-owner control center are implemented. Product classification, generated internal codes, advanced catalog search, managed measurement units, immutable prepared-product recipes, automatic ingredient consumption and controlled kitchen-negative stock are also implemented. It is not yet a complete accounting ERP and does not yet integrate an external payment provider.

The WMS track is temporarily paused after implementing reconciliation, logistics, movements, balances, reservations, counts, lot/serial execution, removal/putaway policies, replenishment transfer/purchase-demand planning, inventory insights and safe scan workflows. The additive procure-to-pay evolution is implemented through PUR-280. Advanced purchasing analytics expose filterable spend, delay, confirmation, price-variance, supplier, seasonal, buyer-workload and open-exception evidence with source definitions and document links. Creation of new Legacy Direct Purchases is retired; historical records remain visible and users are directed to Purchase Requests for new activity.

The imported ERP SaaS Foundation Blueprint is installed under `docs/reference/erp-saas-foundation/` as a proposed reference, not implementation evidence. Its `FND-000` repository compatibility assessment is complete. The project-control audit under `docs/project-control/` now provides the single current status, feature inventory, audit set, dependency order, master roadmap and next-task gate. The local Demo scope completed credential closure, encrypted restore proof and the `P0-DB-003` 112/112 reconciliation baseline. `P1-AUD-011` added and applied migration 113 for the audit-event foundation; `P1-AUD-012` enabled local Demo writing and enrolled tenant roles/permissions plus six sensitive-setting groups. Authentication, denied, platform and external semantic coverage remains proposed in `P1-AUD-013`.

## Status classification
- IMPLEMENTED: catalog, warehouses, suppliers, stock balances/movements, purchases, sales, transfer workflow, Identity login, role-permission checks, audit-log rows, Razor UI.
- PARTIALLY IMPLEMENTED: authorization administration, auditability and inventory controls.
- IMPLEMENTED: tenant-scoped centralized numbering for wholesale sales, POS sales, stock transfers and journal entries, including safe defaults and concurrent-edit protection.
- IMPLEMENTED: database-enforced tenant relationship boundaries across all current ERP entities and tenant subscription redemptions, with automatic entity classification and relationship regression checks.
- IMPLEMENTED: posted sales and purchase returns plus tenant fiscal-period posting controls.
- PLANNED: reversal coverage outside the four supported purchase document types, external payment-provider/webhook integration, API/integrations.
- UNKNOWN: production deployment and migration process, off-host scheduled backups, CI/CD, external integrations, and production operational ownership. A local encrypted Demo restore drill is proven.

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
| Settings | PARTIALLY IMPLEMENTED | settings entities/services | General, discount, inventory, accounting, fiscal periods, POS, centralized document numbers and a bilingual role-aware work procedure catalog beginning with purchasing. |
| Identity & permissions | PARTIALLY IMPLEMENTED | Identity + `PermissionAuthorizationHandler` | Custom permission rows mapped to roles. |

## Current problems
See `06_SECURITY.md`, `04_ACCOUNTING.md`, and `TODO.md`. The historically exposed administrator was closed for the evidenced local Demo environment, an encrypted isolated restore drill reconciled without differences and all 113 current migrations match the database with no pending model change. The local Demo audit feature is enabled through User Secrets and role/permission plus selected sensitive-setting changes now write atomic semantic events. Authentication/denied/platform/external coverage, a reader, retention and an external checkpoint remain open, together with off-host scheduled backups, production deployment controls and broader integration testing.

## Recommended next steps
1. After explicit approval, implement only `P1-AUD-013` from `project-control/NEXT_TASK.md`: authentication, denied privileged action, platform and external audit coverage.
2. Do not add the audit reading/export screen or freeze the legacy interceptor in that slice.
3. Resume the customer and sales sequence only through the ordering and gates in `project-control/MASTER_ROADMAP.md`.

## Read first
Read `project-control/PROJECT_CONTEXT.md`, `project-control/PROJECT_STATUS.md`, `project-control/MASTER_ROADMAP.md` and `project-control/NEXT_TASK.md`; then read `01_CODEBASE_MAP.md`, the relevant module document and actual source.

## Guided company onboarding update (2026-09-15)
New-company registration now continues into a bilingual guided setup. Business presets atomically create a starter chart of accounts, linked branch and warehouse, payment, tax, numbering, discount, inventory and POS configuration. Owners may skip for manual setup, and tenant login accepts an optional company code (the tenant slug).

