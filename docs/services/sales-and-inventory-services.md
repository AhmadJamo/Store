# Sales and inventory services
> Status: IMPLEMENTED WITH REMAINING CONTROLS | Last reviewed: 2026-09-30

`SaleService` owns pricing, numbering, discounts, stock decrement and immutable direct/recipe-derived UnitCost/COGS snapshots. `PurchaseService` creates purchases and recalculates product/warehouse moving averages from net line cost; `PurchasePostingService` posts the supplier document separately. `ProductStockService` creates valued opening balances and adjustments. `StockTransactionService` records auditable quantity/average/value snapshots and permits only reasoned manual adjustments. `StockTransferService` carries source cost into the destination average during posting/cancellation. UnitOfWork uses Serializable transactions and converts EF concurrency conflicts to a retryable data-change message.

`RecipeService` creates a new immutable recipe version, deactivates the old version and validates ingredient/unit compatibility. SaleService reads active recipes and writes aggregated RecipeConsumption movements. StockTransactionService additionally permits a reason-required KitchenVariance movement; it may cross zero only when the ingredient explicitly enables controlled negative recipe consumption.

`JournalPostingService` is the shared posting gateway for purchases, sales and sales returns. It accepts resolved journal lines from the feature workflow and centrally performs source duplicate detection, journal-number generation, balance/post validation and persistence inside the caller's UnitOfWork transaction. Fiscal-period and approval controls will attach here.

`SalePostingService` creates one source-unique posted journal for a sale. It resolves payment settlement, branch/default revenue, net invoice discount, frozen output tax, COGS and warehouse inventory accounts, generates the central journal number inside UnitOfWork and rejects missing mappings or duplicate posting. Inclusive prices are split into net revenue/output tax without increasing settlement; exclusive tax is added to settlement.

Purchase receipt costing excludes recoverable tax when a selected tax rate is price-inclusive. `PurchasePostingService` also reads receipt movements by source reference and settles provisional negative-stock cost variance between COGS and the affected warehouse inventory account.

`SalesReturnService` creates an immutable partial/full return only for a posted sale. Inside one Serializable transaction it enforces cumulative remaining quantity, generates the return and journal numbers, restores eligible direct inventory at historical sale cost and posts the proportional revenue/tax/discount/refund plus inventory/COGS reversal. Prepared items are refunded without recreating ingredients.

`UnassignedStockService` assigns unallocated warehouse quantity to an active exact location and records a Putaway movement. `LocationMovementService` moves product quantity between two active locations in the same warehouse, validates source quantity and total destination capacity, preserves the warehouse total and writes a Relocation history row in the same transaction.

`InventoryReconciliationService` is read-only. It classifies tenant-filtered SQL snapshots from `InventoryReconciliationRepository`, comparing ProductStock quantity/value, aggregated ProductLocationStock, derived Unassigned and the latest StockTransaction snapshot. It applies fixed quantity/value tolerances, exposes exceptions through a paged bilingual report and never performs a correction.

`ProductService` also validates product logistics against managed units: weights require Mass, dimensions require Length and all three dimensions, and category selection remains tenant-scoped. It refuses a transition from untracked to Lot/Serial while any ProductStock row has a non-zero balance. `ProductCategoryService` creates and activates/deactivates preserved category rows; it never deletes referenced categories.

Storage-location, unassigned-stock, location-movement and transfer services enforce each warehouse's control mode. Simple warehouses bypass location allocation; structured warehouses can require transfer locations. Capacity checks follow the destination warehouse's enforcement policy.

`StorageLocationService` also owns hierarchy mutation rules. It normalizes optional barcodes, enforces warehouse-local barcode uniqueness, requires parents in the same warehouse and prevents self/descendant cycles before saving hierarchy or operational capabilities.

SaleService rejects POS sales against a warehouse whose `AllowPosSales` policy is disabled. SalesController filters the POS warehouse selector to the same eligible set.

InventoryAccessService manages branch warehouse permissions, branch priority/defaults, POS terminals and terminal warehouse priorities. SaleService requires the selected terminal to be active and authorized by both branch and terminal mappings once terminals exist.

The POS reads terminal-specific presentation and order-workflow preferences from `PosExperienceSettingsService`. SaleService independently validates the submitted order type, required dine-in service reference, guest-count permission and item-note permission before persisting the context; these preferences do not alter pricing, inventory validation or accounting behavior.

Controllers: Sales, Purchases, ProductStocks, StockTransactions, StockTransfers, UnassignedStock, LocationMovements and InventoryReconciliation. Change any service → inspect its domain entities/DTOs/repositories/configurations, relevant controller/views, permissions, stock concurrency, accounting and module docs.
