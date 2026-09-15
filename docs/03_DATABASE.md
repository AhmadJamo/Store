# Database
> Status: IMPLEMENTED  
> Source of truth: EF Core entities, configurations and migrations  
> Last reviewed: 2026-09-15

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
| Products | `Id`; barcode required but no database unique index. |
| Accounts / Branches / JournalEntries / JournalEntryLines | hierarchical chart; branches optionally reference a sales-revenue subaccount; journal lines hold debit/credit plus optional branch/warehouse dimensions. A journal source type/reference has a filtered unique index to prevent a document from posting twice. |
| Warehouses, Suppliers | Warehouses link a branch/inventory account and persist operational type, control mode, picking, POS, capacity and transfer-location policies; suppliers can link a payable account. |
| TaxRates / AccountingSettings | Tax rates require input/output tax accounts; singleton accounting settings reference optional discount, revenue and COGS accounts. |
| PaymentMethods / Sales | Each payment method references a settlement account; new sales capture a required payment method and optional customer. |
| StorageLocations | Warehouse FK Restrict; unique `(WarehouseId, Code)`; zone/aisle/rack/level/bin, type, status and optional quantity capacity. |
| BranchWarehouseAccesses | composite branch/warehouse key; priority and operation flags; branch cascades, warehouse restricts. |
| PosTerminalWarehouses | composite terminal/warehouse key and priority; terminal cascades, warehouse restricts. PosTerminal names are unique inside a branch. |
| PosTerminalSettings | one-to-one shared primary/foreign key with PosTerminal and cascade delete; stores per-terminal profile/layout and enabled/default order workflow preferences with SQL Server rowversion concurrency. |
| ProductStocks | product + warehouse FKs Restrict; unique `(ProductId, WarehouseId)`; quantity decimal(18,3); SQL Server rowversion optimistic-concurrency token. |
| ProductLocationStocks | product/warehouse/location FKs Restrict; unique `(ProductId, StorageLocationId)`; quantity decimal(18,3) and rowversion. Sum of location quantities cannot exceed warehouse balance through application allocation rules. |
| LocationMovements | immutable putaway/relocation history with product, warehouse, optional source location, required destination location, quantity, type, reference, notes, user and time; Restrict FKs and product/warehouse date indexes. |
| StockTransactions | product + warehouse FKs Restrict; quantity decimal(18,3); indexed `(ProductId, WarehouseId)`. |
| Purchases / PurchaseItems | supplier/header warehouse FKs Restrict; each item has its own optional-for-legacy warehouse FK, discount and tax. New items require warehouse selection. |
| Sales / SaleItems | warehouse FK Restrict; items cascade; sale invoice number unique. POS rows optionally store order type, service reference and guest count; item preparation notes are optional and bounded to 200 characters. |
| StockTransfers / items/history | warehouse FKs Restrict; transfer number unique; StockTransfer rowversion; item/history FKs Restrict; item unique `(StockTransferId, ProductId)`. |
| StockTransferItem locations | Optional source/destination StorageLocation FKs; when selected, Application validates active locations in the matching warehouses. |
| Permissions | global permission catalogue with unique technical name; company-specific selections live in TenantRolePermissions and restrict permission deletion. |
| AuditLogs | indexed by CreatedAt and `(EntityName, EntityId)`. |
| GeneralSettings / DiscountSettings / InventorySettings | singleton key check/index and rowversion. GeneralSettings stores a required English/Arabic default-language enum; InventorySettings supplies new-warehouse policy defaults. |
| DocumentSequences | one row per tenant/document type; unique `(TenantId, DocumentType)`; customizable template, prefix, suffix, padding, next/reset counters and period state; rowversion protects settings updates. |

## Migrations
Migration history is chronological through `EnforceTenantRoleAssignmentBoundary`, `HardenTenantIsolationRelationships` and `HardenTenantSubscriptionRedemption`. The two hardening migrations replace single-ID business relationships with composite tenant relationships and cover tenant subscription redemptions. Migrations are source-controlled under `MiniStore.Infrastructure/Migrations`; generated designers and the model snapshot are metadata, not separate runtime features.

## Transactions and concurrency
`UnitOfWork.ExecuteInTransactionAsync` starts a Serializable database transaction, executes an operation, calls one `SaveChangesAsync`, then commits. Sales, purchases, transfers, putaway and internal location relocation use it. ProductStock, ProductLocationStock and StockTransfer use rowversion concurrency tokens. General, discount, inventory, invoice and POS terminal experience settings use rowversion to varying degrees.

## Update rules
Changing an entity/configuration requires a migration, update to this file and `database/tables.md`, affected entity/module docs, and validation of existing data. Never alter a migration already applied to shared environments.

## CompanyOnboardings (2026-09-15)
CompanyOnboardings uses TenantId as its primary key and cascading FK to Tenants. It stores setup status, normalized setup choices, template/audit metadata and SQL Server rowversion. Migration 20260915140248_AddCompanyGuidedOnboarding creates the table and backfills existing companies as Skipped.
