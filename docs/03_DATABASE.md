# Database
> Status: IMPLEMENTED  
> Source of truth: EF Core entities, configurations and migrations  
> Last reviewed: 2026-09-13

`AppDbContext` derives from `IdentityDbContext<IdentityUser, IdentityRole, string>`, so Identity tables coexist with ERP tables. Provider: SQL Server.

## Application tables
| Table/entity | Key relationships and notable constraints |
|---|---|
| Products | `Id`; barcode required but no database unique index. |
| Warehouses, Suppliers | `Id`; required names; no database unique name index. |
| ProductStocks | product + warehouse FKs Restrict; unique `(ProductId, WarehouseId)`; quantity decimal(18,3); SQL Server rowversion optimistic-concurrency token. |
| StockTransactions | product + warehouse FKs Restrict; quantity decimal(18,3); indexed `(ProductId, WarehouseId)`. |
| Purchases / PurchaseItems | supplier/warehouse FKs Restrict; items cascade from purchase; purchase invoice number unique. |
| Sales / SaleItems | warehouse FK Restrict; items cascade; sale invoice number unique. |
| StockTransfers / items/history | warehouse FKs Restrict; transfer number unique; StockTransfer rowversion; item/history FKs Restrict; item unique `(StockTransferId, ProductId)`. |
| Permissions / RolePermissions | permission name unique; role-permission unique `(RoleId, PermissionId)`; permission deletion cascades mapping. |
| AuditLogs | indexed by CreatedAt and `(EntityName, EntityId)`. |
| GeneralSettings / DiscountSettings | singleton key check/index and rowversion. |
| InvoiceSettings / DocumentNumberSettings | no singleton constraint; InvoiceSettings has rowversion; DocumentNumberSettings does not. |

## Migrations
Migration history is chronological from initial create through stock transfers (`20260912072804_AddStockTransfers`). Migrations are source-controlled under `MiniStore.Infrastructure/Migrations`; generated `*.Designer.cs` and `AppDbContextModelSnapshot.cs` describe EF model snapshots, not separate runtime features.

## Transactions and concurrency
`UnitOfWork.ExecuteInTransactionAsync` starts a database transaction, executes an operation, calls one `SaveChangesAsync`, then commits. Sales, purchases and transfers use it. General/discount/invoice settings use rowversion to varying degrees. Product stock has no concurrency token: concurrent write safety is not verified.

## Update rules
Changing an entity/configuration requires a migration, update to this file and `database/tables.md`, affected entity/module docs, and validation of existing data. Never alter a migration already applied to shared environments.
