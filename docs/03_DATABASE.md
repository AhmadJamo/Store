# Database
> Status: IMPLEMENTED  
> Source of truth: EF Core entities, configurations and migrations  
> Last reviewed: 2026-10-05

WMS-050 adds tenant-owned `InventoryReservations` and `InventoryReservationLines`. Source identity is unique per tenant, quantities are positive, relationships are tenant-safe, exact and Unassigned dimensions are duplicate-protected, and rowversions protect concurrent reservation changes. Migrations `AddInventoryReservations` and `AddUnassignedReservationUniqueness` are additive and are applied to `AHMAD/MiniStoreDb`.

WMS-060 adds tenant-owned `InventoryAdjustments` and `InventoryAdjustmentLines`, tenant-unique document numbers, workflow rowversion, warehouse/location relationships and one product line per document. Migration `AddInventoryAdjustments` is additive and applied to `AHMAD/MiniStoreDb`.

WMS-070A adds `InventoryTrackingBalances` and immutable `InventoryTrackingTransactions`. Filtered tenant-safe indexes enforce serial identity and lot dimensional uniqueness; check constraints enforce non-negative tracked balance and serial zero/one quantity. Migration `AddInventoryLotAndSerialTracking` is additive and applied to `AHMAD/MiniStoreDb`.

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
| Products | SQL-computed internal ProductCode; optional tenant-unique barcode; raw/direct/prepared type; optional category; managed stock/weight/dimension unit FKs; nullable physical measurements; handling flags; None/Lot/Serial policy; active/POS/sales flags; controlled-negative recipe flag. |
| ProductCategories | Tenant-local unique code/name, active state and rowversion; optional tenant-safe Product relationship. |
| MeasurementUnits | Tenant-local codes for Count/Mass/Volume/Length units with conversion factor, precision and protected built-in state; referenced through tenant-safe product/recipe FKs. |
| ProductRecipes / RecipeIngredients | Immutable recipe versions with one filtered-unique active version per tenant/product. Lines reference stocked ingredients and managed authored/stock units, while freezing unit codes/factors and converted stock quantity. SaleItem optionally snapshots the recipe version used. |
| Accounts / Branches / JournalEntries / JournalEntryLines | hierarchical chart; branches optionally reference a sales-revenue subaccount; journal lines hold debit/credit plus optional branch/warehouse dimensions. A journal source type/reference has a filtered unique index to prevent a document from posting twice. |
| PurchaseReturns / PurchaseReturnItems | immutable tenant-owned supplier-return headers/lines linked to original purchase/items with proportional accounting and actual inventory-cost snapshots. |
| FiscalPeriods | tenant accounting date ranges with Open/SoftClosed/Closed status, status-change audit metadata, date/status checks and SQL rowversion concurrency. Application validation prevents overlapping ranges. |
| Warehouses, Suppliers | Warehouses link a branch/inventory account and persist operational type, control mode, picking, POS, capacity and transfer-location policies; suppliers can link a payable account. |
| InventoryTrackingTransactions | Immutable lot/serial trace rows include nullable picking-strategy provenance; null identifies history created before `AddTrackedRemovalStrategyAudit`. |
| StockTransferItems | Transfer lines retain optional manual lot/serial allocation text through draft/edit/post; `AddTransferTrackingAllocations` leaves historic rows null. |
| PutawayRules | Tenant-owned warehouse location suggestions optionally target a product or category and retain priority/active state. |
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
Migration history is chronological through `AddProductTemplatesAndVariantIdentity`, applied to `AHMAD/MiniStoreDb`. WMS-015 migrations add tenant-owned definitions, options, category applicability, typed product values and optional template grouping with duplicate-safe variant signatures without changing Product IDs. The earlier product-logistics and hierarchical-location migrations are also applied. Migrations are source-controlled under `MiniStore.Infrastructure/Migrations`; generated designers and the model snapshot are metadata, not separate runtime features.

## Transactions and concurrency
`UnitOfWork.ExecuteInTransactionAsync` starts a Serializable database transaction, executes an operation, calls one `SaveChangesAsync`, then commits. Sales, purchases, transfers, putaway and internal location relocation use it. ProductStock, ProductLocationStock and StockTransfer use rowversion concurrency tokens. General, discount, inventory, invoice and POS terminal experience settings use rowversion to varying degrees.

## Update rules
Changing an entity/configuration requires a migration, update to this file and `database/tables.md`, affected entity/module docs, and validation of existing data. Never alter a migration already applied to shared environments.

## CompanyOnboardings (2026-09-15)
CompanyOnboardings uses TenantId as its primary key and cascading FK to Tenants. It stores setup status, normalized setup choices, template/audit metadata and SQL Server rowversion. Migration 20260915140248_AddCompanyGuidedOnboarding creates the table and backfills existing companies as Skipped.
## WMS-020A physical movement kernel
Migration `20261003211115_AddPhysicalStockMovementKernel` creates tenant-owned `StockMovements`. Each row has a tenant-unique idempotency key and a tenant-safe FK to one preserved `LocationMovement`, Product, Warehouse and source/destination locations. The migration performs no historic backfill and changes no inventory quantity.

Migration `20261003212429_AddTransferMovementTransitStages` makes legacy/location links optional for document movements and adds related warehouse, source document/line/stage, planned/posted/reversed timestamps and actors. Existing kernel rows preserve their creation/posting data. No historic transfers or quantities are backfilled.
## WMS-040 inventory availability projection
Migration `20261003214320_AddInventoryBalancesAndDefaultLocations` creates tenant-owned `InventoryBalances` with filtered unique indexes for one virtual Unassigned row and exact-location rows. A guarded SQL backfill copies locations and derives Unassigned without changing legacy balances. RowVersion and reservation checks prepare WMS-050. The 2026-10-04 development reconciliation returned 8 projection rows, zero ProductStock total mismatches and zero availability invariant failures.
## Inventory recalls

`InventoryRecalls` is tenant-owned and references Product through the composite tenant boundary. It stores a normalized reference and lot/serial identifier, reason, active/closed lifecycle audit fields and rowversion. A filtered unique index permits only one active recall per tenant/product/identifier, while recall reference is tenant-unique. Existing inventory history is not copied or rewritten.

`InventoryRecallCommunications` is an append-only tenant-owned child of a recall. It records party, contact address, neutral channel/outcome codes, notes, actor and time. Restrict delete preserves evidence and the composite tenant relationship prevents cross-company attachment.
