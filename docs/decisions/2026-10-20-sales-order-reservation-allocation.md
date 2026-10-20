# ADR: Sales-order reservation allocation records

Date: 2026-10-20  
Status: Accepted

## Context

The existing WMS reservation aggregate allowed exactly one record per source. That is correct for a StockTransfer, whose approval owns one idempotent hold, but it cannot preserve the history of a partially reserved SalesOrder that is released and reallocated across warehouses or locations.

## Decision

- Keep one tenant-unique reservation source for `StockTransfer` through a filtered database index.
- Allow multiple `SaleOrder` reservation records for the same order. Each allocation request creates a new record; release closes that record instead of rewriting or deleting it.
- Add nullable `SourceLineId` to reservation lines. Sales-order allocations require it and cumulative active quantities are validated against the exact SalesOrderLine inside a serializable UnitOfWork transaction.
- Use one named warehouse and optional exact reservable location per allocation command. Repeating the command can allocate remaining lines from another position.
- Offer two explicit shortage policies: block the complete request or reserve only currently available quantity and leave the rest waiting.
- Keep `InventoryReservationService` as the single balance-mutation authority. SalesOrderReservationService owns sales-order rules and delegates Reserved changes to it.
- Releasing or cancelling restores Reserved. Reservation never changes OnHand and never writes stock movements or accounting entries.

## Consequences

- Partial fulfillment and reallocation remain auditable without mutable allocation history.
- Existing transfer behavior and idempotency are preserved.
- `Available = OnHand - Reserved` remains the concurrency-protected invariant.
- SAL-060 can consume exact active allocations when delivery is implemented; invoicing and payment remain separate.

