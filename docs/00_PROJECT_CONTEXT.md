# MiniStore project context
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-10-24

## Project identity
MiniStore is a server-rendered ASP.NET Core MVC store-management application under incremental Angular modernization. It targets .NET 10, uses EF Core 10 with SQL Server, ASP.NET Core Identity, Razor views, and a Domain/Application/Infrastructure/Web layered solution. The existing Web host now includes a protected versioned internal API foundation; no separate public API project is present.

## Current development stage
The repository implements an operational slice for catalog, warehouses, stock, suppliers, purchases, sales (wholesale and POS), configurable discounts, stock transfers, users/roles, settings and shared-database SaaS tenancy. Public signup, pricing, plans, trials, subscription access, promotion codes and a separate platform-owner control center are implemented. Product classification, generated internal codes, advanced catalog search, managed measurement units, immutable prepared-product recipes, automatic ingredient consumption and controlled kitchen-negative stock are also implemented. It is not yet a complete accounting ERP and does not yet integrate an external payment provider.

The WMS track is temporarily paused after implementing reconciliation, logistics, movements, balances, reservations, counts, lot/serial execution, removal/putaway policies, replenishment transfer/purchase-demand planning, inventory insights and safe scan workflows. The additive procure-to-pay evolution is implemented through PUR-280. Advanced purchasing analytics expose filterable spend, delay, confirmation, price-variance, supplier, seasonal, buyer-workload and open-exception evidence with source definitions and document links. Creation of new Legacy Direct Purchases is retired; historical records remain visible and users are directed to Purchase Requests for new activity.

The imported ERP SaaS Foundation Blueprint remains a proposed reference. The project-control audit provides current control evidence. CUS-010/020/030A/030B provide the professional Customer master and receivable policy. SAL-020A/B through SAL-100D provide pricing through collection and controlled returns/credits. SAL-110 adds tenant-configurable order controls and sequential approvals. SAL-120A/B add versioned quotation templates, central organization identity, a reusable document designer/PDF engine and frozen sent-document snapshots. SAL-130A now adds a reusable provider-based Analytics Center with personal/shared workspaces and Sales/Purchasing providers. Angular modernization is complete through MOD-019: MOD-018 provides a fully evidenced 11/11 acceptance gate, while MOD-019 adds complete generated view/action/control inventory and reversible allowlisted cutover. Every committed surface still defaults to MVC. Migration source/model and the local Demo database are reconciled through migration 135. Legacy Sale/POS remains unchanged.

## Status classification
- IMPLEMENTED: catalog, warehouses, suppliers, stock balances/movements, purchases, sales, transfer workflow, Identity login, role-permission checks, audit-log rows, Razor UI.
- PARTIALLY IMPLEMENTED: authorization administration, auditability and inventory controls.
- IMPLEMENTED: tenant-scoped centralized numbering for wholesale sales, POS sales, stock transfers and journal entries, including safe defaults and concurrent-edit protection.
- IMPLEMENTED: database-enforced tenant relationship boundaries across all current ERP entities and tenant subscription redemptions, with automatic entity classification and relationship regression checks.
- IMPLEMENTED: posted sales and purchase returns plus tenant fiscal-period posting controls.
- PLANNED: reversal coverage outside the four supported purchase document types, external payment-provider/webhook integration, API/integrations.
- UNKNOWN: production deployment and migration process, off-host scheduled backups, hosted monitoring/alerts, external integrations, and production operational ownership. Local encrypted restore and observability drills are proven.

## Main modules
| Module | Status | Core code | Notes |
|---|---|---|---|
| Products | IMPLEMENTED | `Product`, `ProductService`, `ProductsController` | Generated code, optional barcode, raw/direct/prepared type, managed stock unit, channel controls and advanced catalog search. |
| Warehouses | IMPLEMENTED | `Warehouse`, `WarehouseService` | Named warehouse master data. |
| Suppliers | IMPLEMENTED | `Supplier`, `SupplierService` | Supplier master data. |
| Inventory | PARTIALLY IMPLEMENTED | `ProductStock`, `StockTransaction`, recipes, measurement units | Balances, moving-average valuation, auditable cost movements, managed-unit conversion, prepared recipes, controlled negative variance and purchase-time variance settlement; manual adjustment posting remains pending. |
| Purchases | IMPLEMENTED | `Purchase`, `PurchaseService`, `PurchaseReturnService` | Receipts increase stock; posted partial/full supplier returns remove available stock at moving-average cost and reverse accounting. |
| Sales | PARTIALLY IMPLEMENTED | `Sale`, pricing/order/delivery/invoice/payment, return, approval and quotation-template aggregates | Legacy Sale/POS remains operational; the professional lifecycle includes configurable controls, advanced quotation templates, branded PDF and an Analytics Center provider. Customer collaboration and cutover remain staged. |
| Analytics | PARTIALLY IMPLEMENTED | `AnalyticsWorkspace`, `AnalyticsCenterService`, module providers | Reusable personal/shared workspaces, advanced widget settings and central time comparison; Sales and Purchasing are the first providers. Editing, scheduled delivery and governed self-service fields remain staged. |
| Stock transfers | IMPLEMENTED | `StockTransfer`, `StockTransferService` | Draft/submitted/approved/posted/cancelled flow. |
| Settings | PARTIALLY IMPLEMENTED | settings entities/services | General, discount, inventory, accounting, fiscal periods, POS, centralized document numbers and a complete bilingual role-aware 62-step work procedure catalog across seven ERP modules. |
| Identity & permissions | PARTIALLY IMPLEMENTED | Identity + `PermissionAuthorizationHandler` | Custom permission rows mapped to roles. |

## Current problems
See `06_SECURITY.md`, `04_ACCOUNTING.md`, and `TODO.md`. The historically exposed administrator was closed for the evidenced local Demo environment, an encrypted isolated restore drill reconciled without differences and migrations through 135 are applied with no pending EF model change. The local Demo audit and retention features are enabled through User Secrets. Coverage reconciliation is complete, new legacy writes are frozen behind a reversible flag, and pre/post signed checkpoints preserved all 710 historical legacy rows. Real deployment identities, off-host backup/checkpoint storage, production deployment controls and broader integration testing remain open.

## Recommended next steps
1. Execute the full MVC-to-Angular module waves `MOD-020..028` using the completed `MOD-018` gate and `MOD-019` registry/cutover guard.
2. Prove full dual-run parity in `MOD-029`; keep legacy Sale/POS cutover inside its dedicated `MOD-025` gate.
3. Make Angular the only presentation only in explicit `MOD-030`, preserving audit, observability and rollback controls.

## Read first
Read `project-control/PROJECT_CONTEXT.md`, `project-control/PROJECT_STATUS.md`, `project-control/MASTER_ROADMAP.md` and `project-control/NEXT_TASK.md`; then read `01_CODEBASE_MAP.md`, the relevant module document and actual source.

## Guided company onboarding update (2026-09-15)
New-company registration now continues into a bilingual guided setup. Business presets atomically create a starter chart of accounts, linked branch and warehouse, payment, tax, numbering, discount, inventory and POS configuration. Owners may skip for manual setup, and tenant login accepts an optional company code (the tenant slug).

