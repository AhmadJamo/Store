# ADR: Transfer movement stages and compatibility

## Decision
An approved StockTransfer creates three Planned StockMovement facts per line: TransferOutbound, TransferTransit and TransferInbound. Posting the existing transfer remains the only event that changes warehouse/location balances and valuation; it atomically changes all three facts to Posted. Cancelling a posted transfer reverses balances through the existing workflow and marks its linked movement stages Reversed.

## Compatibility
- Existing transfer status transitions, permissions, StockTransactions, location rules and cancellation behavior remain unchanged.
- Historic transfers are not backfilled.
- An Approved transfer created before WMS-030 receives its plan just-in-time if it is later posted.
- A historic Posted transfer with no movement plan can still be cancelled without fabricating historical movement facts.
- Tenant-safe unique document/line/stage and idempotency indexes prevent duplicate plans.

## Consequences
Transit is visible as an explicit workflow stage, but this slice does not introduce a separate transit balance or partial shipment/receipt. Those require later balance and fulfillment slices.
