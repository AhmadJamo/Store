# InventoryReservation

Tenant-owned aggregate that records which document owns available inventory before fulfillment.

- Unique source: `SourceType + SourceId` per tenant.
- Lifecycle: `Active`, `Consumed`, `Released`.
- Lines identify product, warehouse, optional exact storage location and positive quantity.
- A null location means the protected system Unassigned position.
- Active lines are reflected in `InventoryBalance.Reserved`; Available remains `OnHand - Reserved`.
- Header rowversion and balance rowversion protect concurrent changes.

Transfer approval reserves, transfer posting consumes, and a future staged sales-order flow may use the same aggregate. Immediate sales do not require a reservation because stock is issued in the creation transaction.
