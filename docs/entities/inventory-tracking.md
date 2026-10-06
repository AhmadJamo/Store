# Inventory tracking

`InventoryTrackingBalance` stores lot/serial quantity by product, warehouse and optional location, including manufacture/expiration dates, status, source and rowversion. `InventoryTrackingTransaction` is immutable trace history. Serial balances are constrained to zero or one and serial identifiers are tenant-product unique.

Opening allocation must exactly reconcile to every non-zero InventoryBalance position before changing Product.TrackingPolicy.

Available balances are filtered for quarantine and expiration before `InventoryRemovalAllocator` orders them. FIFO uses receipt time, FEFO prioritizes dated stock by earliest expiration, location priority uses `StorageLocation.Sequence`, and minimize-locations takes the largest balance first. Stable receipt/identifier tie-breakers keep repeated allocations deterministic.

`InventoryTrackingTransaction.PickingStrategy` records the effective automatic strategy or `Manual` for explicit issue allocations. It is nullable so pre-migration history is not misrepresented. Migration `AddTrackedRemovalStrategyAudit` adds the field without rewriting existing trace rows.
