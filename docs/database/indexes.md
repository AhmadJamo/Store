# Database indexes and constraints
> Source of truth: EF configurations | Last reviewed: 2026-09-13

Unique: ProductStock(ProductId, WarehouseId), Purchase.InvoiceNumber, Sale.InvoiceNumber, Permission.Name, TenantRole(TenantId, NormalizedName), TenantRolePermission(TenantRoleId, PermissionId), TenantUserRole(TenantId, UserId, TenantRoleId), StockTransfer.TransferNumber, StockTransferItem(StockTransferId, ProductId), GeneralSettings.SingletonKey and DiscountSettings.SingletonKey.

Non-unique: StockTransaction(ProductId, WarehouseId), AuditLog.CreatedAt, AuditLog(EntityName, EntityId), StockTransfer(Status, CreatedAt), StockTransferHistory(StockTransferId, CreatedAt).

Check constraints: GeneralSettings.SingletonKey = 1 and DiscountSettings.SingletonKey = 1. DocumentSequences has a unique `(TenantId, DocumentType)` index and a rowversion concurrency token. Product barcode and warehouse/supplier names have no observed unique constraint.

Tenant relationship principals receive alternate keys containing their primary key plus `TenantId`; dependent lookup indexes contain the business FK plus `TenantId`. These support the database-enforced same-company composite foreign keys.
