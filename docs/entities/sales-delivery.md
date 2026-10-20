# Sales delivery
> Status: SAL-060 IMPLEMENTED | Last reviewed: 2026-10-20

`SalesDelivery` is the tenant-owned physical-fulfillment aggregate for a confirmed `SalesOrder`. It has its own centrally generated `DEL-` number, one source reservation, one warehouse, delivery date, rowversion and Draft → Posted or Draft → Cancelled lifecycle with immutable semantic history.

Every line links the exact order line and reservation line and freezes product code/name, stock-unit quantity, moving-average unit cost, COGS and optional lot/serial allocation evidence. Draft quantity cannot exceed either the order's undelivered quantity or the reservation line's remaining quantity.

Posting is atomic and one-time. It removes stock from the reservation's exact warehouse/location, reduces `OnHand` and `Reserved`, records a Sale `StockTransaction`, a `SalesDeliveryOut` physical `StockMovement`, partial/final reservation consumption and debit COGS / credit Inventory. Source and idempotency keys reject repeated financial or physical effects.

Only a Draft may be cancelled. A Posted delivery is immutable and any later reversal belongs to a controlled return workflow. The aggregate has no authority to create revenue, customer receivable, invoice or payment.
