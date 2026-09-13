# Inventory and stock movements
> Status: IMPLEMENTED WITH REMAINING CONTROLS | Last reviewed: 2026-09-13

Balances are held by product/warehouse in `ProductStock`; immutable-by-convention movement rows are `StockTransaction`. ProductStock and StockTransfer have SQL Server rowversion concurrency tokens. Inventory UnitOfWork operations use Serializable transactions, and a stale update rolls back the document, balance and movement together with a retry message.

Manual transaction creation permits only AdjustmentIn and AdjustmentOut and requires a reason. Purchase, Sale, TransferIn, TransferOut and OpeningBalance are owned by their corresponding application workflows. Purchase and sale aggregates reject duplicate product lines.

Flow: controller → `ProductStockService` or `StockTransactionService` → UnitOfWork → repositories → balance/movement tables → views. Permissions: ProductStock.View/Create/Edit/Delete and StockTransactions.View/Create.

Sources: `ProductStock.cs`, `StockTransaction.cs`, `StockTransactionType.cs`; ProductStocks DTOs; product-stock/transaction services and repositories; configurations; `ProductStocksController`, `StockTransactionsController`; their views. Changes impact purchases, sales, transfers and accounting docs.
