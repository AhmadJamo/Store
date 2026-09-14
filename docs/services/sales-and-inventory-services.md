# Sales and inventory services
> Status: IMPLEMENTED WITH REMAINING CONTROLS | Last reviewed: 2026-09-14

`SaleService` depends on sale/product/stock/movement/invoice/discount repositories, UnitOfWork and permission service. It owns pricing, discount and stock-decrement flow. `PurchaseService` creates purchases plus stock/movement. Purchase and sale aggregates reject duplicate product lines. `ProductStockService` creates opening balances and direct target-quantity adjustments. `StockTransactionService` uses UnitOfWork and permits only manual AdjustmentIn/AdjustmentOut with a required reason. `StockTransferService` owns workflow and transfer posting/cancellation. UnitOfWork uses Serializable database transactions and converts EF concurrency conflicts to a retryable user-facing inventory conflict.

`UnassignedStockService` assigns unallocated warehouse quantity to an active exact location and records a Putaway movement. `LocationMovementService` moves product quantity between two active locations in the same warehouse, validates source quantity and total destination capacity, preserves the warehouse total and writes a Relocation history row in the same transaction.

Storage-location, unassigned-stock, location-movement and transfer services enforce each warehouse's control mode. Simple warehouses bypass location allocation; structured warehouses can require transfer locations. Capacity checks follow the destination warehouse's enforcement policy.

SaleService rejects POS sales against a warehouse whose `AllowPosSales` policy is disabled. SalesController filters the POS warehouse selector to the same eligible set.

InventoryAccessService manages branch warehouse permissions, branch priority/defaults, POS terminals and terminal warehouse priorities. SaleService requires the selected terminal to be active and authorized by both branch and terminal mappings once terminals exist.

The POS reads terminal-specific presentation and order-workflow preferences from `PosExperienceSettingsService`. SaleService independently validates the submitted order type, required dine-in service reference, guest-count permission and item-note permission before persisting the context; these preferences do not alter pricing, inventory validation or accounting behavior.

Controllers: Sales, Purchases, ProductStocks, StockTransactions, StockTransfers, UnassignedStock and LocationMovements. Change any service → inspect its domain entities/DTOs/repositories/configurations, relevant controller/views, permissions, stock concurrency, accounting and module docs.
