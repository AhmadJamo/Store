# Sales and inventory services

`InventoryRemovalAllocator` is the shared tracked-stock ordering policy used by `InventoryTrackingService` for issues and transfers. It converts the source warehouse's configured FIFO, FEFO, location-priority or minimize-locations strategy into a deterministic candidate order; Manual currently falls back to FIFO until the audited override workflow is implemented.
> Status: IMPLEMENTED WITH REMAINING CONTROLS | Last reviewed: 2026-10-13

`SaleService` owns pricing, numbering, discounts, stock decrement and immutable direct/recipe-derived UnitCost/COGS snapshots. `PurchaseService` creates purchases and recalculates product/warehouse moving averages from net line cost; `PurchasePostingService` posts the supplier document separately. `ProductStockService` creates valued opening balances and adjustments. `StockTransactionService` records auditable quantity/average/value snapshots and permits only reasoned manual adjustments. `StockTransferService` carries source cost into the destination average during posting/cancellation. UnitOfWork uses Serializable transactions and converts EF concurrency conflicts to a retryable data-change message.

`CustomerService` owns the CUS-010 Customer master Application boundary. It returns DTO-only paged/searchable/sortable lists and form/detail models, generates tenant-local Customer numbers inside the existing UnitOfWork, validates receivable-account, configured currency/tax/default-price-list and uniqueness rules, and performs update/archive/restore with rowversion conflict protection. `SaleService` accepts only an active tenant customer for new sales; archived customers remain readable in historical flows.

`SalesPriceListService` owns SAL-020B price-list administration and read-only evaluation. It resolves list/customer defaults, managed-unit conversion, the dated company-currency-per-unit exchange rate, deterministic domain rule selection, final increment rounding and explicit/customer tax policy, then returns DTO evidence for every decision. Create/archive/rule mutations use the shared UnitOfWork; evaluation never changes a sale, stock or ledger.

`SalesQuotationService` owns SAL-030 quotation lists, details, creation and lifecycle commands. Creation resolves only active tenant customer/address/product/unit masters, evaluates each line through `SalesPriceListService`, requires common currency/exchange-rate evidence, generates the tenant quotation number and saves all immutable snapshots in one UnitOfWork transaction. The request key makes retries idempotent. Send/Accept/Reject/Expire/Cancel re-read the tenant document, validate rowversion and domain state, and save semantic history; none of these commands can mutate stock or accounting.

`SalesOrderService` owns SAL-040 read models, one-time Accepted-quotation conversion, confirmation and cancellation. It rechecks the tenant source and rowversion inside UnitOfWork, returns the existing order for a repeated conversion request, generates the `SO-` number and delegates exact snapshot copying to the domain aggregate. Confirmation/cancellation write history only; the service does not depend on inventory or accounting mutation services.

CUS-020 extends the same service/repository boundary for atomic contact/address create/update/default switching and archive/restore. It clears the previous active primary/default inside the UnitOfWork, validates submitted rowversion and exposes address snapshot DTOs for future sales documents without yet changing a sale.

`RecipeService` creates a new immutable recipe version, deactivates the old version and validates ingredient/unit compatibility. SaleService reads active recipes and writes aggregated RecipeConsumption movements. StockTransactionService additionally permits a reason-required KitchenVariance movement; it may cross zero only when the ingredient explicitly enables controlled negative recipe consumption.

`JournalPostingService` is the shared posting gateway for purchases, sales and sales returns. It accepts resolved journal lines from the feature workflow and centrally performs source duplicate detection, journal-number generation, balance/post validation and persistence inside the caller's UnitOfWork transaction. Fiscal-period and approval controls will attach here.

`SalePostingService` creates one source-unique posted journal for a sale. It resolves payment settlement, branch/default revenue, net invoice discount, frozen output tax, COGS and warehouse inventory accounts, generates the central journal number inside UnitOfWork and rejects missing mappings or duplicate posting. Inclusive prices are split into net revenue/output tax without increasing settlement; exclusive tax is added to settlement.

Purchase receipt costing excludes recoverable tax when a selected tax rate is price-inclusive. `PurchasePostingService` also reads receipt movements by source reference and settles provisional negative-stock cost variance between COGS and the affected warehouse inventory account.

`SalesReturnService` creates an immutable partial/full return only for a posted sale. Inside one Serializable transaction it enforces cumulative remaining quantity, generates the return and journal numbers, restores eligible direct inventory at historical sale cost and posts the proportional revenue/tax/discount/refund plus inventory/COGS reversal. Prepared items are refunded without recreating ingredients.

`UnassignedStockService` assigns unallocated warehouse quantity to an active exact location and records a Putaway movement. It uses `PutawayRuleService` to preselect the highest-precedence product, category or warehouse-default rule whose active receivable destination has enough remaining capacity; authorized users manage rule active state from the same screen. `LocationMovementService` moves product quantity between two active locations in the same warehouse, validates source quantity and total destination capacity, preserves the warehouse total and writes a Relocation history row in the same transaction.

`UntrackedInventoryRemovalService` prevents warehouse issues from leaving stale rack/bin quantities. It plans against current location rows and reserved InventoryBalance quantities, then removes from active pickable exact locations or Unassigned according to the warehouse policy in the owning Serializable transaction. Tracked products bypass it and remain under `InventoryTrackingService`.

`ReplenishmentService` manages one Product + destination Warehouse rule and projects reviewed suggestions from aggregate OnHand, Reserved and Available. It adds Approved-transfer inbound/outbound forecasting, avoids subtracting already-reserved outbound twice and exposes preferred-source availability. Reading a suggestion never changes stock or documents.

When explicitly requested by an authorized operator, the same service revalidates a suggestion and delegates creation of a Draft intent to `StockTransferService`; branch access, exact-location requirements and duplicate open references are enforced before the draft is returned for the ordinary transfer workflow.

`InventoryInsightsService` is a read-only projection of positive ProductStock and StockTransaction history. It classifies recent, slow and dead stock from the last actual negative movement, distinguishes stock that was received but never issued, and exposes missing movement history as a separate control state. Configurable thresholds affect reporting only; they never write inventory or accounting values.

`InventoryScanningService` is the WMS-100A read-only scanner boundary. It normalizes exact product and location codes/barcodes, rejects zero or multiple matches explicitly and reads the existing InventoryBalance projection. It never creates a movement, reservation or document; later scan mutations must delegate to the owning receipt, movement or count application service with an idempotency key.

WMS-100B1 extends that boundary only for scanned putaway. It validates a strongly scoped scan retry key and exact product/location results, then delegates to `UnassignedStockService`; it does not duplicate capacity, balance, transaction or movement rules inside the scanner service.

WMS-100B2 adds an independently scoped scanned-relocation retry key. After exact product/source/destination resolution, `InventoryScanningService` delegates to `LocationMovementService`, preserving the existing same-warehouse, source-availability, location-capability, capacity and Serializable movement controls.

WMS-100B3 delegates one absolute scanned quantity to `InventoryAdjustmentService.RecordScannedLineAsync`. The service validates the Draft document, included product and exact count position inside the UnitOfWork; it does not complete, approve, post or mutate inventory. Existing scanned values are prefilled when the operator later completes all lines.

`InventoryReconciliationService` is read-only. It classifies tenant-filtered SQL snapshots from `InventoryReconciliationRepository`, comparing ProductStock quantity/value, aggregated ProductLocationStock, derived Unassigned and the latest StockTransaction snapshot. It applies fixed quantity/value tolerances, exposes exceptions through a paged bilingual report and never performs a correction.

`PurchaseAccountingReconciliationService` is the PUR-160 read-only financial comparison. Its dedicated repository projects warehouse operational Inventory values, posted ledger activity, Goods Receipt values, unbilled return reversals and Vendor Bill GRNI clearing under tenant query filters. The service applies two-decimal accounting tolerance, classifies warehouse exceptions and open receipts, and never creates an adjustment or journal.

`PurchaseOrderLifecycleService` is the PUR-165 completion authority. It derives each order line's received, returned, Vendor-Bill-cleared, remaining-to-receive and unsettled quantities from posted downstream documents. A fully received and fully settled confirmed order closes automatically inside the final bill/return transaction. A partially received order can close its remainder only when all receipts are settled and a reason is supplied; only that closure type can be reopened through its separately authorized, reasoned command.

`SupplierCreditNoteService` is the PUR-170 billed-return authority. It calculates remaining credited quantity per Vendor Bill line, freezes proportional bill values with exact final residual rounding, delegates location and lot/serial removal to existing inventory services, records current-cost stock evidence and posts payable/input-tax/inventory/PPV reversal in the same Serializable transaction. `VendorBillService` and `SupplierPaymentService` subtract posted credit totals before exposing or accepting payment allocation.

`PurchaseDocumentReversalService` is the PUR-180 correction authority for posted Goods Receipts, Vendor Bills, Supplier Payments and Landed Costs. It resolves the typed original, enforces reverse-order dependencies and unchanged stock valuation, creates one retry-safe audit document, coordinates physical/value reversal when required, posts the exact opposite journal through `JournalPostingService`, updates the original status and reopens automatically completed Purchase Orders when necessary.

`ProductService` also validates product logistics against managed units: weights require Mass, dimensions require Length and all three dimensions, and category selection remains tenant-scoped. It refuses a transition from untracked to Lot/Serial while any ProductStock row has a non-zero balance. `ProductCategoryService` creates and activates/deactivates preserved category rows; it never deletes referenced categories.

Storage-location, unassigned-stock, location-movement and transfer services enforce each warehouse's control mode. Simple warehouses bypass location allocation; structured warehouses can require transfer locations. Capacity checks follow the destination warehouse's enforcement policy.

`StorageLocationService` also owns hierarchy mutation rules and the Warehouse Locations page projection. It returns DTO-only warehouse selectors, parent selectors and location rows, so the controller does not inject repositories or expose persistence entities. It normalizes optional barcodes, enforces warehouse-local barcode uniqueness, requires parents in the same warehouse and prevents self/descendant cycles before saving hierarchy or operational capabilities.

Location search returns parents before children with a calculated display depth. Putaway requires a receivable destination; relocation and transfer validation require pickable sources and receivable destinations in addition to active status, warehouse ownership and capacity rules.

SaleService rejects POS sales against a warehouse whose `AllowPosSales` policy is disabled. SalesController filters the POS warehouse selector to the same eligible set.

InventoryAccessService manages branch warehouse permissions, branch priority/defaults, POS terminals and terminal warehouse priorities. SaleService requires the selected terminal to be active and authorized by both branch and terminal mappings once terminals exist.

The POS reads terminal-specific presentation and order-workflow preferences from `PosExperienceSettingsService`. SaleService independently validates the submitted order type, required dine-in service reference, guest-count permission and item-note permission before persisting the context; these preferences do not alter pricing, inventory validation or accounting behavior.

Controllers: Sales, Purchases, ProductStocks, StockTransactions, StockTransfers, UnassignedStock, LocationMovements, InventoryReconciliation and PurchaseAccountingReconciliation. Change any service → inspect its domain entities/DTOs/repositories/configurations, relevant controller/views, permissions, stock concurrency, accounting and module docs.
