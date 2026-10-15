# Financial & Inventory Integrity
## Accounting
JournalHeader + JournalLines, balanced debits/credits in functional currency per posting unit, validated currency precision and rounding, chart of accounts, fiscal periods and lock controls. Posted journal is immutable; correction uses reversal/adjustment with references. Unique (TenantId, SourceType, SourceId, PostingKind) for idempotency; verify ledger-to-subledger reconciliation.

## Inventory
StockMovement ledger immutable after posting; StockBalance derived/materialized for fast availability. Unique (TenantId, WarehouseId, ProductId, LotId/VariantId as applicable); atomically decrement balance with Quantity>=requested and rows affected check; enforce negative-stock policy per tenant. Transfers and returns must be consistent with movement ledger. Reservations separate on-hand from available; account for expiry, batches, units, costing method and valuation.

## Transactions
Single SQL transaction for locally coupled financial posting and stock changes where feasible; outbox in same transaction for downstream async consumers. No blind retries on non-idempotent operations. Document numbering allocated by safe sequence strategy; gaps may occur and must be handled according to jurisdiction/policy rather than unsafe global locks.

## Tests
Concurrent oversell (100+ actors), double-click/retry, deadlocks, crash between posting and event publish, period lock, rounding, duplicate posting, partial receipt, cancellation/reversal, stock-ledger reconciliation, balanced journal, tenant separation.
