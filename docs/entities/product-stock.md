# ProductStock entity
> Status: IMPLEMENTED | Last reviewed: 2026-09-13

Path: `MiniStore.Domain/Entities/Inventory/ProductStock.cs`. Holds Id, ProductId, WarehouseId, Quantity and database-generated RowVersion; constructed at zero. Add/Remove require positive amounts and removal forbids below zero. Table has unique product/warehouse and Restrict FKs. RowVersion is an EF concurrency token used by sales, purchases, transfers and stock services. Navigation properties are not defined.

If changed: update configuration/migration, stock transaction reconciliation, sales/purchase/transfer services, inventory docs and concurrency tests.
