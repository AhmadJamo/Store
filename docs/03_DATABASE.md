# Database
> Status: IMPLEMENTED  
> Source of truth: EF Core entities, configurations and migrations  
> Last reviewed: 2026-09-29

`AppDbContext` derives from `IdentityDbContext<IdentityUser, IdentityRole, string>`, so Identity tables coexist with ERP tables. Provider: SQL Server.

All business tables have a required `TenantId` foreign key to `Tenants`. EF query filters use the authenticated tenant context, and SaveChanges enforces tenant ownership for inserts, updates and deletes. Business unique indexes include `TenantId`; the same invoice number, account code or settings singleton may therefore exist independently in different companies. Every relationship between records carrying tenant ownership also includes `TenantId` on both sides, so SQL rejects a reference to another company's record.

## Application tables
| Table/entity | Key relationships and notable constraints |
|---|---|
| Tenants / TenantMemberships / TenantRoles / TenantRolePermissions / TenantUserRoles | Tenant identity has globally unique slug and rowversion. Roles have tenant-local normalized-name uniqueness and rowversion; permissions use `(TenantRoleId, PermissionId)` and user assignment uses `(TenantId, UserId, TenantRoleId)`. A composite FK from `(TenantRoleId, TenantId)` to the role prevents cross-company assignment at database level. |
| Plans / PlanFeatures / PlanLimits | bilingual public commercial plans with monthly/annual prices, feature flags and nullable unlimited numeric limits. |
| TenantSubscriptions | one current subscription per tenant with trial/active/dunning states, billing cycle/period, provider references and rowversion. |
| PlatformOperators | explicit allow-list and platform role linked to Identity; separate from tenant Admin membership. |
| PromotionCodes / PromotionRedemptions | unique codes with percentage, UTC validity, optional plan and redemption cap; one redemption per tenant and concurrency-protected counter. |
| BillingCheckoutSessions | immutable tenant/plan/cycle quote amounts plus optional promotion, 30-minute expiry, status, payment reference/time and rowversion. Subscription activation occurs only on successful confirmation. |
| Products | SQL-computed internal ProductCode; optional tenant-unique barcode; raw/direct/prepared type; stocked/prepared behavior; managed stock-unit FK; active/POS/sales flags; controlled-negative recipe flag. |
| MeasurementUnits | Tenant-local codes for Count/Mass/Volume units with conversion factor, precision and protected built-in state; referenced through tenant-safe product/recipe FKs. |
| ProductRecipes / RecipeIngredients | Immutable recipe versions with one filtered-unique active version per tenant/product. Lines reference stocked ingredients and managed authored/stock units, while freezing unit codes/factors and converted stock quantity. SaleItem optionally snapshots the recipe version used. |
| Accounts / Branches / JournalEntries / JournalEntryLines | hierarchical chart; branches optionally reference a sales-revenue subaccount; journal lines hold debit/credit plus optional branch/warehouse dimensions. A journal source type/reference has a filtered unique index to prevent a document from posting twice. |
| PurchaseReturns / PurchaseReturnItems | immutable tenant-owned supplier-return headers/lines linked to original purchase/items with proportional accounting and actual inventory-cost snapshots. |
| FiscalPeriods | tenant accounting date ranges with Open/SoftClosed/Closed status, status-change audit metadata, date/status checks and SQL rowversion concurrency. Application validation prevents overlapping ranges. |
| Warehouses, Suppliers | Warehouses link a branch/inventory account and persist operational type, control mode, picking, POS, capacity and transfer-location policies; suppliers can link a payable account. |
| TaxRates / AccountingSettings | Tax rates require input/output tax accounts; singleton accounting settings reference optional discount, revenue and COGS accounts. |
| PaymentMethods / Sales | Each payment method references a settlement account; new sales capture a required payment method, optional customer and optional immutable invoice-tax snapshot. |
| StorageLocations | Warehouse FK Restrict; unique `(WarehouseId, Code)`; zone/aisle/rack/level/bin, type, status and optional quantity capacity. |
| BranchWarehouseAccesses | composite branch/warehouse key; priority and operation flags; branch cascades, warehouse restricts. |
| PosTerminalWarehouses | composite terminal/warehouse key and priority; terminal cascades, warehouse restricts. PosTerminal names are unique inside a branch. |
| PosTerminalSettings | one-to-one shared primary/foreign key with PosTerminal and cascade delete; stores per-terminal profile/layout and enabled/default order workflow preferences with SQL Server rowversion concurrency. |
| ProductStocks | product + warehouse FKs Restrict; unique `(ProductId, WarehouseId)`; quantity decimal(18,6), average/value/reference cost decimal(24,8), controlled negative recipe valuation and rowversion concurrency. |
| ProductLocationStocks | product/warehouse/location FKs Restrict; unique `(ProductId, StorageLocationId)`; quantity decimal(18,3) and rowversion. Sum of location quantities cannot exceed warehouse balance through application allocation rules. |
| LocationMovements | immutable putaway/relocation history with product, warehouse, optional source location, required destination location, quantity, type, reference, notes, user and time; Restrict FKs and product/warehouse date indexes. |
| StockTransactions | product + warehouse FKs Restrict; quantity decimal(18,6), quantity/average/value before and after, unit cost, transaction value and settlement variance; includes RecipeConsumption and KitchenVariance; indexed `(ProductId, WarehouseId)`. |
| Purchases / PurchaseItems | supplier/header warehouse FKs Restrict; each item has its own optional-for-legacy warehouse FK, discount and tax. New items require warehouse selection. |
| Sales / SaleItems / SalesReturns / SalesReturnItems | sale warehouse and optional TaxRate FKs Restrict; items cascade; sale invoice number unique. Sale freezes tax percentage, output account, inclusive policy and amount. Returns reference the original sale/items, warehouse/payment and freeze refund, tax, revenue, discount and historical restock cost; return numbers are tenant-unique. |
| StockTransfers / items/history | warehouse FKs Restrict; transfer number unique; StockTransfer rowversion; item/history FKs Restrict; item unique `(StockTransferId, ProductId)`. |
| StockTransferItem locations | Optional source/destination StorageLocation FKs; when selected, Application validates active locations in the matching warehouses. |
| Permissions | global permission catalogue with unique technical name; company-specific selections live in TenantRolePermissions and restrict permission deletion. |
| AuditLogs | indexed by CreatedAt and `(EntityName, EntityId)`. |
| GeneralSettings / DiscountSettings / InventorySettings | singleton key check/index and rowversion. GeneralSettings stores a required English/Arabic default-language enum; InventorySettings supplies new-warehouse policy defaults. |
| DocumentSequences | one row per tenant/document type; unique `(TenantId, DocumentType)`; customizable template, prefix, suffix, padding, next/reset counters and period state; rowversion protects settings updates. |

## Migrations
Migration history is chronological through `AddSalesReturns`. It adds tenant-isolated return headers/items and the SaleItem tenant alternate key needed by the protected relationship, and was applied to `AHMAD/MiniStoreDb` on 2026-09-29. Migrations are source-controlled under `MiniStore.Infrastructure/Migrations`; generated designers and the model snapshot are metadata, not separate runtime features.

## Transactions and concurrency
`UnitOfWork.ExecuteInTransactionAsync` starts a Serializable database transaction, executes an operation, calls one `SaveChangesAsync`, then commits. Sales, purchases, transfers, putaway and internal location relocation use it. ProductStock, ProductLocationStock and StockTransfer use rowversion concurrency tokens. General, discount, inventory, invoice and POS terminal experience settings use rowversion to varying degrees.

## Update rules
Changing an entity/configuration requires a migration, update to this file and `database/tables.md`, affected entity/module docs, and validation of existing data. Never alter a migration already applied to shared environments.

## CompanyOnboardings (2026-09-15)
CompanyOnboardings uses TenantId as its primary key and cascading FK to Tenants. It stores setup status, normalized setup choices, template/audit metadata and SQL Server rowversion. Migration 20260915140248_AddCompanyGuidedOnboarding creates the table and backfills existing companies as Skipped.
