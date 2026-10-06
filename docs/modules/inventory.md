# Inventory and stock movements
> Status: IMPLEMENTED WITH APPROVED WMS EVOLUTION | Last reviewed: 2026-10-04

The approved evolution is documented in `../WMS_EVOLUTION_PLAN.md`. Existing ProductStock, ProductLocationStock, StockTransaction, LocationMovement, warehouse policies and StockTransfer workflows remain authoritative until each WMS vertical slice is implemented and reconciled. WMS-020A now adds the StockMovement kernel for new Putaway and Relocation operations only.

Balances and moving weighted-average valuation are held by product/warehouse in `ProductStock`; immutable-by-convention quantity/cost movement rows are `StockTransaction`. Every new movement freezes quantity, average and inventory value before/after plus unit cost, movement value and cost variance. ProductStock and StockTransfer have SQL Server rowversion concurrency tokens. Inventory UnitOfWork operations use Serializable transactions, and a stale update rolls back the document, balance and movement together with a retry message.

Prepared-to-order products consume active recipe ingredients instead of a finished-item balance. Requirements are converted to each ingredient's stock unit, aggregated and recorded as RecipeConsumption. Controlled negative balances are restricted to authorized recipe/kitchen use and retain a provisional reference cost. A later receipt isolates the provisional-versus-actual CostVariance and values any remaining positive quantity at receipt cost; purchase posting settles that variance between COGS and the affected warehouse inventory account.

Manual transaction creation permits only AdjustmentIn and AdjustmentOut and requires a reason. Purchase, Sale, TransferIn, TransferOut and OpeningBalance are owned by their corresponding application workflows. Purchase and sale aggregates reject duplicate product lines.

Sales returns create positive `SalesReturn` stock movements for direct stocked products at the original sale-line unit cost. Prepared-to-order returns do not recreate recipe ingredients automatically.

Flow: controller → `ProductStockService` or `StockTransactionService` → UnitOfWork → repositories → balance/movement tables → views. Permissions: ProductStock.View/Create/Edit/Delete and StockTransactions.View/Create.

Sources: `ProductStock.cs`, `StockTransaction.cs`, `StockTransactionType.cs`; ProductStocks DTOs; product-stock/transaction services and repositories; configurations; `ProductStocksController`, `StockTransactionsController`; their views. Changes impact purchases, sales, transfers and accounting docs.

## Storage locations
Warehouses have hierarchical structured locations with a unique code, optional unique barcode, display name, parent, sequence, zone, aisle, rack, level, bin, operational type, status, optional capacity and explicit receive/pick/reserve/ship/count capabilities. Existing IDs and codes remain stable. The Warehouse Locations screen searches these fields, creates roots/children and edits hierarchy/capabilities. The Application layer rejects self-parenting, descendant cycles and parents from another warehouse. Unassigned Stock calculates warehouse quantity minus allocated location quantities, supports warehouse/name/barcode filters and sorting, and lets authorized staff allocate all or part of the balance to an active location without changing the warehouse total. Capacity validation uses the total quantity of all products in the destination location.

The location list is ordered as a visual parent/child tree. Creation filters parent choices to the selected warehouse. Putaway and transfer destinations must be receivable; relocation and transfer sources must be pickable. Reservable, shippable and countable are persisted for their later owning workflows and are not silently treated as equivalent to receive/pick permission.

When a product's warehouse balance is zero, its previous rack/bin allocations are no longer physically valid. The next purchase receipt for that product and warehouse removes those stale allocations inside the purchase transaction, so the entire newly received quantity returns to Unassigned Stock for a fresh putaway.

Inter-warehouse transfer locations are optional. When a location is provided, it must be active and belong to the matching warehouse; posting deducts/adds the location balance. Without a source location, posting can use only the unassigned source quantity. Without a destination location, the received quantity enters the Unassigned Stock queue. Cancellation reverses the same allocation behavior.

## Internal location movements and audit
The Location Movements screen moves a selected product between two active locations in the same warehouse. It shows only source locations holding a positive balance for the selected product, displays available quantity, filters destination locations by warehouse and rejects identical locations, insufficient source quantity or destination-capacity overflow. The Serializable UnitOfWork transaction updates both location balances and writes one immutable `LocationMovement` history row while leaving `ProductStock` unchanged.

Every assignment from Unassigned Stock also writes a `Putaway` history row. History can be filtered by warehouse and searched by product name, barcode or reference, and records quantity, source, destination, UTC creation time, user, reference and notes. Permissions are `LocationMovements.View` and `LocationMovements.Create`.

## Physical movement kernel pilot
New Putaway and Relocation requests carry a server-rendered idempotency key. Inside the existing Serializable transaction, the services first reject a repeated key, update location quantities, preserve the legacy LocationMovement row and write one linked Posted StockMovement. Tenant-aware foreign keys and unique indexes enforce the source linkage and idempotency boundary. Historic LocationMovement rows are not backfilled. The read-only Physical Stock Movements screen uses `StockMovements.View` and reports linked pilot rows alongside the legacy count.

WMS-030 extends the same ledger to stock transfers. Approval creates Planned outbound, transit and inbound stages per transfer line. Posting retains the current atomic warehouse/location and AVCO movement behavior, then marks all stages Posted in the same transaction. Cancellation retains the current compensating balance/valuation behavior and marks linked stages Reversed. Historic transfers are not backfilled; pre-WMS Approved transfers are planned only when subsequently posted, while old Posted transfers remain cancellable without invented history.

## Explicit balances and availability
WMS-040 adds InventoryBalance per Product, Warehouse and optional exact location. A null location is the protected virtual Unassigned position. OnHand is physical quantity, Reserved starts at zero pending WMS-050, and Available is always OnHand minus Reserved. Filtered tenant-safe indexes guarantee one Unassigned row and one row per exact location. RowVersion supports later atomic reservation updates.

ProductStock remains the warehouse quantity/AVCO authority and ProductLocationStock remains the exact-location compatibility source. The persistence boundary refreshes affected projection rows before the same transaction commits, covering all current purchase, sale, return, recipe, adjustment, transfer and location workflows without duplicated service calls. Migration backfill stops on unexplained over-allocation, changes no legacy quantity and can be reconciled by summing InventoryBalance OnHand back to ProductStock.

The disposable SQL fixture verifies tenant isolation, opening-balance projection creation, projection update when two contexts compete for the last unit, and final zero OnHand consistency.

## Reservations and allocation

WMS-050 makes `InventoryReservation` the owner of Reserved. Transfer approval allocates every line against its exact reservable source location or the protected Unassigned position. Posting consumes the reservation inside the same transaction as the physical issue. Source uniqueness makes retries idempotent, while balance and reservation rowversions prevent two concurrent documents from claiming the final available quantity. Immediate sales validate aggregate Available before issuing stock atomically, so they cannot consume a transfer hold, but do not create artificial zero-duration reservations.

## Inventory adjustments and cycle counting

WMS-060 adds a controlled Draft → Counted → Approved → Posted workflow, with cancellation before posting and optional blind counts. Expected OnHand is frozen per product and exact location/Unassigned position. Posting refuses stale counts or a result below Reserved, then atomically updates warehouse/location balances and writes both the valued AdjustmentIn/AdjustmentOut StockTransaction and linked physical StockMovement. General-ledger gain/loss posting is deferred until explicit accounts are configured.

## Lot, serial and expiration tracking

WMS-070A adds dimensional lot/serial balances and immutable trace transactions. Serial quantities are limited to zero/one and identifiers are unique per tenant product; lot identifiers may occupy multiple locations. Opening activation rejects negative or partially allocated stock and requires submitted allocations to equal every current InventoryBalance position before Product.TrackingPolicy changes.

WMS-070B requires lot identity or one serial per received unit on tracked purchase lines. WMS-080A orders non-expired tracked candidates using the source warehouse policy: FIFO by receipt time, FEFO by earliest expiration then receipt time, location priority by configured location sequence, or minimize-locations by largest balance first. The same allocator governs direct sales, tracked recipe ingredients and transfer allocation. Manual policy currently retains deterministic FIFO compatibility until the auditable identity-override command is added. Transfer posting and cancellation preserve the exact lot/serial identity between source and destination positions and write balanced trace entries inside the owning transaction. Sales and purchase returns require allocations whose total exactly matches the return quantity: lots use `LOT:QUANTITY`, while serials use one identifier per line. The service validates each identity against the original sale or purchase and prevents duplicate returns.

WMS-080B1 lets wholesale-sale lines explicitly select tracked identities using `LOT:QUANTITY` or one serial per line. The request must exactly cover the sold quantity and only available, non-expired identities are accepted. Every resulting issue trace stores `PickingStrategy = Manual`; automatic issues and transfers store the effective warehouse strategy. Existing history remains null rather than receiving fabricated provenance. POS and transfer override screens remain follow-up work.

WMS-080B2 carries the same explicit selection on stock-transfer draft lines. Posting validates the selected identities in the chosen source position. Cancellation derives its selection from the original destination trace so the same lots/serials return to the source; it does not silently substitute another identity.

WMS-080C adds tenant-owned `PutawayRule` rows scoped to one warehouse and destination location. A rule may target one product, one category or act as the warehouse default. Authorized users administer and activate/deactivate rules from Unassigned Stock. Suggestions prefer product over category over default, then priority, and only return active receivable locations with enough remaining capacity for the proposed quantity; the first eligible receivable location is a deterministic fallback. The suggested location is preselected but remains operator-confirmed before posting.

Untracked issues now use one central exact-location removal service for sales, recipe consumption, supplier returns and legacy aggregate adjustment paths. It allocates only current unreserved quantity from active pickable locations, keeps Unassigned as a fallback position and updates `ProductLocationStock` in the same Serializable transaction as the warehouse balance. LocationPriority consumes the lowest configured sequence first; MinimizeLocations consumes the largest available positions first. FIFO/FEFO cannot be derived for untracked stock because no receipt layer exists, so they use deterministic location sequence. Controlled recipe-negative shortages remain explicitly in Unassigned rather than manufacturing a negative rack balance. Lot/serial products remain owned by `InventoryTrackingService`.

The shelf-life control slice adds per-product default shelf-life days, mandatory-expiration policy and warning horizon. Receipts derive missing expiration when a default exists, reject missing mandatory dates and reject already-expired stock. Sales and transfers cannot allocate expired balances. The tracking screen highlights expired and expiring-soon balances and reports their active counts.

Authorized users can quarantine an available positive lot/serial balance or release a quarantined balance with an obligatory reason. Both state-only events write immutable zero-quantity trace entries and preserve physical quantity. Allocation queries accept only Available balances, so quarantine immediately blocks sales and transfers. Receiving or returning more units to a quarantined lot does not silently release it. Scheduled notifications and full recall orchestration remain follow-up work.

Inventory recall records one tracked Product and normalized lot/serial identity, unique active case and reason. Starting a recall atomically quarantines every currently available positive balance for that identity across positions. The recall list derives affected purchase, sale, transfer and return references from immutable tracking history. Closing requires notes and never releases stock; quality release remains a separate permission and trace event. Automated supplier/customer communication remains outside this slice.

The expiration alert center is a live tenant-safe projection over positive tracked balances. It uses each Product's warning horizon, orders the most urgent balances first and shows severity, identity, warehouse/location, quantity, expiration date, remaining days and quarantine state. It creates no duplicate persisted notifications and cannot become stale; scheduled delivery channels may consume this same projection later.

Recall impact resolves immutable tracking references into purchase/sale/transfer/return document types. Purchase receipts show the supplier and stored phone when available; sale issues show the customer name or an explicit walk-in customer label. Quantity is aggregated per document and transfer source/destination legs are counted once. The report is operational evidence, not an automated outbound message.

While a recall is Active, authorized users may append immutable communication entries containing party, phone/email/address, channel, outcome, notes, actor and UTC time. Communication history remains visible after closure and cannot be edited. Automated sending is not implied; entries record actual manual or externally completed contact.

## Warehouse operating policies

## Replenishment planning

WMS-090A stores one active/inactive replenishment rule per tenant Product + destination Warehouse with minimum, maximum, safety stock, lead days and an optional preferred source warehouse. The suggestion screen uses reservation-aware Available across all positions. At or below Minimum it suggests replenishing to Maximum and shows preferred-source availability when configured. Suggestions are advisory only and never mutate inventory or create a purchase/transfer document automatically.
Warehouse operational use is independent from inventory control. Types cover general, central, branch backroom, sales floor, outlet, production, transit, returns and quarantine uses. Control modes are Simple, LocationManaged and Hybrid. An organized sales-floor or central warehouse may explicitly allow direct POS sales.

Each warehouse stores its picking strategy, POS eligibility, capacity enforcement and whether exact source/destination locations are required on transfers. Simple warehouses do not use exact locations. Location-managed and Hybrid warehouses support a product in multiple locations; the warehouse balance remains the total and `ProductLocationStock` rows represent physical distribution. Hybrid is the safe default because receipts may enter Unassigned Stock before putaway.

The POS warehouse list contains only warehouses with POS sales enabled, and SaleService validates the same rule before saving so a crafted request cannot bypass it. Existing warehouses were enabled during migration/backfill to preserve established POS operation; administrators can disable them individually.

The Inventory Settings singleton supplies defaults for new warehouses. Effective rules remain warehouse-specific. Existing warehouses are migrated as Hybrid with capacity enforcement so established location data and workflows remain available.

## Inventory reconciliation baseline

The `InventoryReconciliation.View` report is a read-only WMS control. For each tenant Product + Warehouse it compares ProductStock quantity/value, the SQL-aggregated ProductLocationStock quantity, derived Unassigned quantity and the latest StockTransaction quantity/value snapshot. It also finds location balances without a warehouse balance, negative location rows and location balances attached to Simple warehouses. Search accepts product name, generated code or barcode; warehouse, exception-only and page-size filters are available. The report never adjusts stock.

Quantity differences use a 0.000001 tolerance and value differences use 0.00000001. Exceptions must be resolved through the owning inventory document or a future approved adjustment workflow. The SQL integration fixture creates a disposable database, verifies query translation and tenant isolation for two companies, and proves that ProductStock rowversion allows exactly one of two concurrent last-unit issues to succeed.

The 2026-09-30 read-only run against the demo database found five explained legacy exceptions: three positive balances whose latest pre-valuation transactions contain zero snapshot fields, and two controlled-negative recipe ingredient balances with the same legacy zero snapshots. No location allocation rows were present for those balances. They are demo-data baseline exceptions, not silent corrections; the report retains them until the demo inventory is deliberately reseeded or an approved baseline workflow exists.

## Branch and POS access
`BranchWarehouseAccess` assigns warehouses to a branch with priority and separate permissions for POS sales, purchases, transfer-in, transfer-out and replenishment. One access row may be the branch POS default. `PosTerminalWarehouse` narrows the branch list for each terminal and orders its default and alternatives.

POS sale validation requires warehouse, branch-access and terminal-access approval. Installations with no configured terminal retain transitional legacy POS behavior; after the first terminal is created, terminal selection is mandatory.
