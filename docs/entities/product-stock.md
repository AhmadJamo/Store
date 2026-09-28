# ProductStock entity
> Status: IMPLEMENTED | Last reviewed: 2026-09-28

Path: `MiniStore.Domain/Entities/Inventory/ProductStock.cs`. Holds Id, ProductId, WarehouseId, six-decimal Quantity, moving AverageUnitCost, InventoryValue, LastReferenceUnitCost and database-generated RowVersion. Receipts recalculate the weighted average; issues preserve it. Normal removal forbids below zero. `ConsumeRecipeQuantity` permits a negative balance only when the calling policy allows it, using a provisional reference cost; later receipts isolate the settlement CostVariance while valuing remaining positive stock at receipt cost.

If changed: update configuration/migration, stock transaction reconciliation, sales/purchase/transfer services, inventory docs and concurrency tests.
