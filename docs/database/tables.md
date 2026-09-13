# Database tables reference
> Last reviewed: 2026-09-13

Schema source is `AppDbContext`, configurations and migrations. Identity's standard `AspNet*` tables are supplied by IdentityDbContext.

- **Products:** Id, Name, Barcode, PurchasePrice, SalePrice, WholesalePrice; all prices decimal(18,2).
- **Warehouses:** Id, Name.
- **Suppliers:** Id, Name, Phone?, Address?.
- **ProductStocks:** Id, ProductId, WarehouseId, Quantity decimal(18,3); unique product/warehouse.
- **StockTransactions:** Id, ProductId, WarehouseId, Quantity decimal(18,3), Type, Reference?, CreatedAt.
- **Purchases:** Id, SupplierId, WarehouseId, InvoiceNumber, Date, Notes?, TotalAmount; items contain ProductId, Quantity, PurchasePrice, Total.
- **Sales:** Id, InvoiceNumber, Channel, CreatedByUserId, CreatedAt, WarehouseId, Date, Notes?, subtotal/discount/total fields; items contain product/quantity/price/discount/totals.
- **StockTransfers:** identity, transfer number, source/destination warehouse, status, actor/time/reason fields; child item and history tables.
- **Permissions / RolePermissions:** permission catalogue and Identity role mapping.
- **AuditLogs:** EntityName, EntityId, Action, UserId, CreatedAt.
- **GeneralSettings / DiscountSettings / InvoiceSettings / DocumentNumberSettings:** application settings.

See `03_DATABASE.md` for constraints and `entities/*.md` for behaviour.
