# ADR: Immutable posted sales returns
> Date: 2026-09-29 | Status: Accepted

A sales return is a new immutable document linked to one posted original sale; it never edits or deletes that sale. The service aggregates earlier return quantities and rejects any quantity above the remaining sold amount inside the Serializable transaction.

The refund reverses the proportional frozen revenue, invoice discount and output tax from the original sale. Direct stocked products return to the original warehouse at the sale line's historical unit cost and reverse COGS. Prepared-to-order products do not recreate consumed ingredients automatically, so their refund does not restock inventory or reverse COGS. A future production/waste workflow may record physical recovery separately.

Creation, document numbering, stock receipt, immutable movement, return rows and the posted reversal journal commit atomically. Returns use their own source-unique journal reference and `SalesReturn` document sequence.
