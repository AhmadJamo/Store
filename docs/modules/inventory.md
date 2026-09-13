# Inventory and stock movements
> Status: IMPLEMENTED WITH REMAINING CONTROLS | Last reviewed: 2026-09-13

Balances are held by product/warehouse in `ProductStock`; immutable-by-convention movement rows are `StockTransaction`. ProductStock and StockTransfer have SQL Server rowversion concurrency tokens. Inventory UnitOfWork operations use Serializable transactions, and a stale update rolls back the document, balance and movement together with a retry message.

Manual transaction creation permits only AdjustmentIn and AdjustmentOut and requires a reason. Purchase, Sale, TransferIn, TransferOut and OpeningBalance are owned by their corresponding application workflows. Purchase and sale aggregates reject duplicate product lines.

Flow: controller → `ProductStockService` or `StockTransactionService` → UnitOfWork → repositories → balance/movement tables → views. Permissions: ProductStock.View/Create/Edit/Delete and StockTransactions.View/Create.

Sources: `ProductStock.cs`, `StockTransaction.cs`, `StockTransactionType.cs`; ProductStocks DTOs; product-stock/transaction services and repositories; configurations; `ProductStocksController`, `StockTransactionsController`; their views. Changes impact purchases, sales, transfers and accounting docs.

## Storage locations
Warehouses have structured locations with a unique code per warehouse, zone, aisle, rack, level, bin, operational type, status and optional capacity. The Warehouse Locations screen searches these fields and existing product balances by product name/barcode and warehouse. Unassigned Stock calculates warehouse quantity minus allocated location quantities, supports warehouse/name/barcode filters and sorting, and lets authorized staff allocate all or part of the balance to an active location without changing the warehouse total.

When a product's warehouse balance is zero, its previous rack/bin allocations are no longer physically valid. The next purchase receipt for that product and warehouse removes those stale allocations inside the purchase transaction, so the entire newly received quantity returns to Unassigned Stock for a fresh putaway.

Inter-warehouse transfer locations are optional. When a location is provided, it must be active and belong to the matching warehouse; posting deducts/adds the location balance. Without a source location, posting can use only the unassigned source quantity. Without a destination location, the received quantity enters the Unassigned Stock queue. Cancellation reverses the same allocation behavior.
