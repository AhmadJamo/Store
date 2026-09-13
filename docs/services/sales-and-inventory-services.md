# Sales and inventory services
> Status: IMPLEMENTED WITH REMAINING CONTROLS | Last reviewed: 2026-09-13

`SaleService` depends on sale/product/stock/movement/invoice/discount repositories, UnitOfWork and permission service. It owns pricing, discount and stock-decrement flow. `PurchaseService` creates purchases plus stock/movement. Purchase and sale aggregates reject duplicate product lines. `ProductStockService` creates opening balances and direct target-quantity adjustments. `StockTransactionService` uses UnitOfWork and permits only manual AdjustmentIn/AdjustmentOut with a required reason. `StockTransferService` owns workflow and transfer posting/cancellation. UnitOfWork uses Serializable database transactions and converts EF concurrency conflicts to a retryable user-facing inventory conflict.

Controllers: Sales, Purchases, ProductStocks, StockTransactions, StockTransfers. Change any service → inspect its domain entities/DTOs/repositories/configurations, relevant controller/views, permissions, stock concurrency, accounting and module docs.
