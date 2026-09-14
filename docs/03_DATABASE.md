# Database
> Status: IMPLEMENTED  
> Source of truth: EF Core entities, configurations and migrations  
> Last reviewed: 2026-09-14

`AppDbContext` derives from `IdentityDbContext<IdentityUser, IdentityRole, string>`, so Identity tables coexist with ERP tables. Provider: SQL Server.

## Application tables
| Table/entity | Key relationships and notable constraints |
|---|---|
| Products | `Id`; barcode required but no database unique index. |
| Accounts / Branches / JournalEntries / JournalEntryLines | hierarchical chart; branches optionally reference a sales-revenue subaccount; journal lines hold debit/credit plus optional branch/warehouse dimensions. A journal source type/reference has a filtered unique index to prevent a document from posting twice. |
| Warehouses, Suppliers | Warehouses link a branch/inventory account and persist operational type, control mode, picking, POS, capacity and transfer-location policies; suppliers can link a payable account. |
| TaxRates / AccountingSettings | Tax rates require input/output tax accounts; singleton accounting settings reference optional discount, revenue and COGS accounts. |
| PaymentMethods / Sales | Each payment method references a settlement account; new sales capture a required payment method and optional customer. |
| StorageLocations | Warehouse FK Restrict; unique `(WarehouseId, Code)`; zone/aisle/rack/level/bin, type, status and optional quantity capacity. |
| BranchWarehouseAccesses | composite branch/warehouse key; priority and operation flags; branch cascades, warehouse restricts. |
| PosTerminalWarehouses | composite terminal/warehouse key and priority; terminal cascades, warehouse restricts. PosTerminal names are unique inside a branch. |
| ProductStocks | product + warehouse FKs Restrict; unique `(ProductId, WarehouseId)`; quantity decimal(18,3); SQL Server rowversion optimistic-concurrency token. |
| ProductLocationStocks | product/warehouse/location FKs Restrict; unique `(ProductId, StorageLocationId)`; quantity decimal(18,3) and rowversion. Sum of location quantities cannot exceed warehouse balance through application allocation rules. |
| LocationMovements | immutable putaway/relocation history with product, warehouse, optional source location, required destination location, quantity, type, reference, notes, user and time; Restrict FKs and product/warehouse date indexes. |
| StockTransactions | product + warehouse FKs Restrict; quantity decimal(18,3); indexed `(ProductId, WarehouseId)`. |
| Purchases / PurchaseItems | supplier/header warehouse FKs Restrict; each item has its own optional-for-legacy warehouse FK, discount and tax. New items require warehouse selection. |
| Sales / SaleItems | warehouse FK Restrict; items cascade; sale invoice number unique. |
| StockTransfers / items/history | warehouse FKs Restrict; transfer number unique; StockTransfer rowversion; item/history FKs Restrict; item unique `(StockTransferId, ProductId)`. |
| StockTransferItem locations | Optional source/destination StorageLocation FKs; when selected, Application validates active locations in the matching warehouses. |
| Permissions / RolePermissions | permission name unique; role-permission unique `(RoleId, PermissionId)`; permission deletion cascades mapping. |
| AuditLogs | indexed by CreatedAt and `(EntityName, EntityId)`. |
| GeneralSettings / DiscountSettings / InventorySettings | singleton key check/index and rowversion. InventorySettings supplies new-warehouse policy defaults. |
| InvoiceSettings / DocumentNumberSettings | no singleton constraint; InvoiceSettings has rowversion; DocumentNumberSettings does not. |

## Migrations
Migration history is chronological from initial create through POS sale terminal audit (`AddSalePosTerminalAudit`). Migrations are source-controlled under `MiniStore.Infrastructure/Migrations`; generated `*.Designer.cs` and `AppDbContextModelSnapshot.cs` describe EF model snapshots, not separate runtime features.

## Transactions and concurrency
`UnitOfWork.ExecuteInTransactionAsync` starts a Serializable database transaction, executes an operation, calls one `SaveChangesAsync`, then commits. Sales, purchases, transfers, putaway and internal location relocation use it. ProductStock, ProductLocationStock and StockTransfer use rowversion concurrency tokens. General/discount/invoice settings use rowversion to varying degrees.

## Update rules
Changing an entity/configuration requires a migration, update to this file and `database/tables.md`, affected entity/module docs, and validation of existing data. Never alter a migration already applied to shared environments.
