# Customer Return Request
> Last reviewed: 2026-10-21

`CustomerReturnRequest` is the professional order-to-cash return authorization aggregate introduced by `SAL-100A`. It is separate from the legacy immediate-sale `SalesReturn`.

- Source authority: one Posted `SalesDelivery` and its exact `SalesDeliveryLine` rows.
- States: Draft, Submitted, Approved, Rejected and Cancelled, with immutable semantic history and rowversion concurrency.
- Active Draft/Submitted/Approved lines reserve return eligibility; Rejected/Cancelled lines release it.
- The sum of active requested quantity cannot exceed the source delivered quantity. The check and insert execute in the Serializable UnitOfWork transaction.
- Lines freeze product, warehouse/location, delivered cost and tracking evidence for later receipt/inspection.
- `SAL-100A` creates no stock movement, journal, credit note, refund or customer-balance change.

Owned rows: `CustomerReturnRequestLine`, `CustomerReturnRequestHistory`. All three types are tenant-owned and all relationships use tenant-composite foreign keys.
