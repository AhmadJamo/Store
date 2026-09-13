# ADR: Protect inventory writes with rowversion and serializable transactions
> Date: 2026-09-13 | Status: Accepted

Concurrent sales, purchases, adjustments and transfers can read the same ProductStock quantity and overwrite one another. A document and its stock movements must also commit or roll back together.

Decision: ProductStock and StockTransfer carry SQL Server rowversion concurrency tokens. Existing UnitOfWork operations execute with Serializable isolation and translate stale-row updates into a retry message. The transfer token prevents concurrent duplicate workflow transitions and posting. Document-owned movement types cannot be created through the manual movement service. Purchase and sale aggregates reject repeated product lines.

This adds blocking under contention and callers may need to retry after another stock write commits. Apply migration `AddInventoryConcurrency` before deploying the new application version. A later reconciliation job should compare balances with movement totals.
