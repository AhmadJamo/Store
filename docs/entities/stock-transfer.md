# StockTransfer aggregate
> Status: IMPLEMENTED | Last reviewed: 2026-09-13

Paths: `StockTransfer.cs`, `StockTransferItem.cs`, `StockTransferHistory.cs`, status/action enums. The aggregate owns header status, source/destination warehouses, actor/timestamp/reason fields, item list, history and a database-generated RowVersion. Its behaviours enforce Draft-only edits, non-duplicate products, state transitions, required rejection/cancellation reason and no self-approval. RowVersion prevents concurrent duplicate workflow transitions. EF table relationships and unique transfer number are in three transfer configuration files.

Change impact: commands/DTOs, service/repository/controller/screens, document numbering, stock balance/movement logic, transfer module/database docs.
