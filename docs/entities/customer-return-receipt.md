# Customer Return Receipt
> Last reviewed: 2026-10-21

`CustomerReturnReceipt` is the `SAL-100B` custody and inspection aggregate. It consumes an Approved `CustomerReturnRequest` partially or fully and is separate from inventory posting and customer credit.

- States: Draft, Inspected, Cancelled, with rowversion and immutable history.
- Lines freeze exact request/delivery line, product, original warehouse/location, received quantity, original unit cost, inspection notes, tracking identities and one disposition: Restock, Quarantine, Reject or Destroy.
- Active Draft/Inspected receipts reserve request quantities; Cancelled drafts release them.
- Tracked identities must have been issued by the source delivery and cannot be assigned to another active receipt.
- Inspection creates no ProductStock, tracking balance, stock movement, journal, credit note, refund or customer-balance mutation.

Owned rows: `CustomerReturnReceiptLine` and `CustomerReturnReceiptHistory`.
