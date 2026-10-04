# Inventory tracking

`InventoryTrackingBalance` stores lot/serial quantity by product, warehouse and optional location, including manufacture/expiration dates, status, source and rowversion. `InventoryTrackingTransaction` is immutable trace history. Serial balances are constrained to zero or one and serial identifiers are tenant-product unique.

Opening allocation must exactly reconcile to every non-zero InventoryBalance position before changing Product.TrackingPolicy.
