# Inventory and stock movements
> Status: IMPLEMENTED WITH REMAINING CONTROLS | Last reviewed: 2026-09-14

Balances are held by product/warehouse in `ProductStock`; immutable-by-convention movement rows are `StockTransaction`. ProductStock and StockTransfer have SQL Server rowversion concurrency tokens. Inventory UnitOfWork operations use Serializable transactions, and a stale update rolls back the document, balance and movement together with a retry message.

Manual transaction creation permits only AdjustmentIn and AdjustmentOut and requires a reason. Purchase, Sale, TransferIn, TransferOut and OpeningBalance are owned by their corresponding application workflows. Purchase and sale aggregates reject duplicate product lines.

Flow: controller → `ProductStockService` or `StockTransactionService` → UnitOfWork → repositories → balance/movement tables → views. Permissions: ProductStock.View/Create/Edit/Delete and StockTransactions.View/Create.

Sources: `ProductStock.cs`, `StockTransaction.cs`, `StockTransactionType.cs`; ProductStocks DTOs; product-stock/transaction services and repositories; configurations; `ProductStocksController`, `StockTransactionsController`; their views. Changes impact purchases, sales, transfers and accounting docs.

## Storage locations
Warehouses have structured locations with a unique code per warehouse, zone, aisle, rack, level, bin, operational type, status and optional capacity. The Warehouse Locations screen searches these fields and existing product balances by product name/barcode and warehouse. Unassigned Stock calculates warehouse quantity minus allocated location quantities, supports warehouse/name/barcode filters and sorting, and lets authorized staff allocate all or part of the balance to an active location without changing the warehouse total. Capacity validation uses the total quantity of all products in the destination location.

When a product's warehouse balance is zero, its previous rack/bin allocations are no longer physically valid. The next purchase receipt for that product and warehouse removes those stale allocations inside the purchase transaction, so the entire newly received quantity returns to Unassigned Stock for a fresh putaway.

Inter-warehouse transfer locations are optional. When a location is provided, it must be active and belong to the matching warehouse; posting deducts/adds the location balance. Without a source location, posting can use only the unassigned source quantity. Without a destination location, the received quantity enters the Unassigned Stock queue. Cancellation reverses the same allocation behavior.

## Internal location movements and audit
The Location Movements screen moves a selected product between two active locations in the same warehouse. It shows only source locations holding a positive balance for the selected product, displays available quantity, filters destination locations by warehouse and rejects identical locations, insufficient source quantity or destination-capacity overflow. The Serializable UnitOfWork transaction updates both location balances and writes one immutable `LocationMovement` history row while leaving `ProductStock` unchanged.

Every assignment from Unassigned Stock also writes a `Putaway` history row. History can be filtered by warehouse and searched by product name, barcode or reference, and records quantity, source, destination, UTC creation time, user, reference and notes. Permissions are `LocationMovements.View` and `LocationMovements.Create`.

## Warehouse operating policies
Warehouse operational use is independent from inventory control. Types cover general, central, branch backroom, sales floor, outlet, production, transit, returns and quarantine uses. Control modes are Simple, LocationManaged and Hybrid. An organized sales-floor or central warehouse may explicitly allow direct POS sales.

Each warehouse stores its picking strategy, POS eligibility, capacity enforcement and whether exact source/destination locations are required on transfers. Simple warehouses do not use exact locations. Location-managed and Hybrid warehouses support a product in multiple locations; the warehouse balance remains the total and `ProductLocationStock` rows represent physical distribution. Hybrid is the safe default because receipts may enter Unassigned Stock before putaway.

The POS warehouse list contains only warehouses with POS sales enabled, and SaleService validates the same rule before saving so a crafted request cannot bypass it. Existing warehouses were enabled during migration/backfill to preserve established POS operation; administrators can disable them individually.

The Inventory Settings singleton supplies defaults for new warehouses. Effective rules remain warehouse-specific. Existing warehouses are migrated as Hybrid with capacity enforcement so established location data and workflows remain available.

## Branch and POS access
`BranchWarehouseAccess` assigns warehouses to a branch with priority and separate permissions for POS sales, purchases, transfer-in, transfer-out and replenishment. One access row may be the branch POS default. `PosTerminalWarehouse` narrows the branch list for each terminal and orders its default and alternatives.

POS sale validation requires warehouse, branch-access and terminal-access approval. Installations with no configured terminal retain transitional legacy POS behavior; after the first terminal is created, terminal selection is mandatory.
