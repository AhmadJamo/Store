# ProductStock entity
> Status: IMPLEMENTED | Last reviewed: 2026-09-28

Path: `MiniStore.Domain/Entities/Inventory/ProductStock.cs`. Holds Id, ProductId, WarehouseId, six-decimal Quantity and database-generated RowVersion; constructed at zero. Normal removal forbids below zero. `ConsumeRecipeQuantity` permits a negative balance only when the calling application policy explicitly allows it for that ingredient. RecipeConsumption and reason-required KitchenVariance record those exceptional outflows; normal sales, transfers and ordinary adjustments remain strict. Negative rows are reconciliation exceptions and their financial valuation is deferred to ACC-001.

If changed: update configuration/migration, stock transaction reconciliation, sales/purchase/transfer services, inventory docs and concurrency tests.
