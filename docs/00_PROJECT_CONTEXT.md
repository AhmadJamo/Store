# MiniStore project context
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-09-13

## Project identity
MiniStore is a server-rendered ASP.NET Core MVC store-management application. It targets .NET 10, uses EF Core 10 with SQL Server, ASP.NET Core Identity, Razor views, and a Domain/Application/Infrastructure/Web layered solution. No public API project is present.

## Current development stage
The repository implements a first operational slice for catalog, warehouses, stock, suppliers, purchases, sales (wholesale and POS), configurable discounts, stock transfers, users/roles, and basic settings. It is not a complete accounting ERP and is not multi-tenant/SaaS in code.

## Status classification
- IMPLEMENTED: catalog, warehouses, suppliers, stock balances/movements, purchases, sales, transfer workflow, Identity login, role-permission checks, audit-log rows, Razor UI.
- PARTIALLY IMPLEMENTED: authorization administration, settings concurrency, auditability, inventory controls, invoice/document numbering.
- PLANNED: accounting ledger, customers, payments, taxes, returns, fiscal periods, SaaS tenancy/subscriptions, API/integrations. No implementation found.
- BLOCKED: stock-transfer creation requires a `DocumentNumberSettings` row; code does not seed or create one when absent.
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
| Settings | PARTIALLY IMPLEMENTED | settings entities/services | General/discount/invoice settings. |
| Identity & permissions | PARTIALLY IMPLEMENTED | Identity + `PermissionAuthorizationHandler` | Custom permission rows mapped to roles. |

## Current problems
See `06_SECURITY.md`, `04_ACCOUNTING.md`, and `TODO.md`. Highest-priority findings: rotation of previously committed admin credentials on existing deployments, missing inventory concurrency control, and no accounting ledger. Identity escalation, role-delete CSRF and login lockout have been addressed; see security assessment.

## Recommended next steps
1. Complete the security and inventory-integrity work listed in `TODO.md`.
2. Decide and document a tenant boundary before SaaS development; no `TenantId` exists.
3. Establish an immutable posted-document and double-entry accounting design before adding more ERP features.

## Read first
Read `01_CODEBASE_MAP.md`, then the relevant module document, then actual source.

