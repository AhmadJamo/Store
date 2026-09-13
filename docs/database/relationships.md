# Database relationships
> Source of truth: EF configurations | Last reviewed: 2026-09-13

- ProductStock → Product and Warehouse: required FK, Restrict deletion.
- StockTransaction → Product and Warehouse: required FK, Restrict deletion.
- Purchase → Supplier and Warehouse: required FK, Restrict; Purchase → PurchaseItems: cascade. PurchaseItem → Product: Restrict.
- Sale → Warehouse: Restrict; Sale → SaleItems: cascade. SaleItem → Product: Restrict.
- StockTransfer → FromWarehouse and ToWarehouse: Restrict; StockTransfer → Items and History: Restrict. Item → Product: Restrict.
- RolePermission → Permission: cascade; RoleId is an Identity role ID but no explicit EF navigation to IdentityRole is configured.

Identity relationships are supplied by IdentityDbContext. AuditLog has no foreign-key relationships. No tenant relationship exists.
