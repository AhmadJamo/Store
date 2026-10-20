# InventoryReservation

Tenant-owned aggregate that records which document owns available inventory before fulfillment.

- Stock-transfer sources remain unique by `SourceType + SourceId` per tenant. A sales order may own multiple immutable reservation records so released quantities can be reallocated without erasing history.
- Lifecycle: `Active`, `Consumed`, `Released`.
- Lines identify product, warehouse, optional exact storage location and positive quantity. Sales-order lines also retain `SourceLineId` so cumulative committed reservations cannot exceed that exact order line. `ConsumedQuantity` records partial delivery consumption and `RemainingQuantity` is derived rather than separately mutable.
- A null location means the protected system Unassigned position.
- Active remaining lines are reflected in `InventoryBalance.Reserved`; Available remains `OnHand - Reserved`. A partial delivery reduces only the delivered amount and keeps the reservation Active; final consumption changes it to Consumed.
- Header rowversion and balance rowversion protect concurrent changes.

Transfer approval reserves and transfer posting consumes. SAL-050 uses the same aggregate for confirmed sales orders: each allocation operation can reserve all or part of the remaining stock from one named warehouse and exact reservable location or Unassigned position. A whole allocation record can be released and replaced from another position; cancellation releases every active order reservation. Immediate sales do not require a reservation because stock is issued in the creation transaction.
