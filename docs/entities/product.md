# Product entity
> Status: IMPLEMENTED | Last reviewed: 2026-09-13

Path: `MiniStore.Domain/Entities/Product.cs`. Holds `Id`, required `Name`/`Barcode`, and `PurchasePrice`, `SalePrice`, `WholesalePrice`. Constructor validates non-negative values and ordering. It has ChangeName/Barcode/PurchasePrice/SalePrice/WholesalePrice behaviours. Table/configuration: Products, `ProductConfiguration`; barcode is required but not unique in DB. Used by product service/DTOs/screens and referenced by ProductStock, StockTransaction, PurchaseItem, SaleItem and StockTransferItem.

If changed: review pricing invariant, product config/migration, all dependent inventory/document modules and `modules/products.md`.
