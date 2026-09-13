# Database indexes and constraints
> Source of truth: EF configurations | Last reviewed: 2026-09-13

Unique: ProductStock(ProductId, WarehouseId), Purchase.InvoiceNumber, Sale.InvoiceNumber, Permission.Name, RolePermission(RoleId, PermissionId), StockTransfer.TransferNumber, StockTransferItem(StockTransferId, ProductId), GeneralSettings.SingletonKey, DiscountSettings.SingletonKey.

Non-unique: StockTransaction(ProductId, WarehouseId), AuditLog.CreatedAt, AuditLog(EntityName, EntityId), StockTransfer(Status, CreatedAt), StockTransferHistory(StockTransferId, CreatedAt).

Check constraints: GeneralSettings.SingletonKey = 1 and DiscountSettings.SingletonKey = 1. Product barcode and warehouse/supplier names have no observed unique constraint. InvoiceSettings/DocumentNumberSettings lack singleton constraints.
