# Sales delivery reservation and cost boundary

- Status: Accepted
- Date: 2026-10-20
- Scope: SAL-060 professional Sales Order delivery

## Context

A confirmed Sales Order and its reservation are commercial intent and stock protection; neither is evidence that goods physically left the warehouse. The delivery boundary must support partial fulfillment, exact location and tracking evidence, moving-average cost, tenant isolation and retry safety without creating an invoice or payment.

## Decision

1. `SalesDelivery` is an independent tenant aggregate with Draft, Posted and Cancelled states, its own `DEL-` number, rowversion, lines and semantic history.
2. One Draft is sourced from one active reservation and one warehouse. Each line references the exact SalesOrderLine and InventoryReservationLine.
3. Posting consumes only the selected reservation quantity. Partial consumption keeps the reservation Active with a derived remainder; final consumption closes it as Consumed.
4. Posting issues stock only from the reservation's exact warehouse/location. Tracked products preserve the inventory allocator's lot/serial evidence; untracked products use the centralized exact-position removal path.
5. The posted line freezes current moving-average unit cost and COGS. One transaction writes the ProductStock change, InventoryBalance change, Sale StockTransaction, SalesDeliveryOut StockMovement, reservation consumption and debit COGS / credit Inventory journal.
6. Source uniqueness and movement idempotency prevent a repeated posting effect. The shared fiscal-period and journal gateway remain authoritative.
7. Only Draft delivery cancellation is allowed. Posted reversal is not an edit or cancellation; it belongs to a separately controlled sales-return workflow. An order with a posted delivery cannot be cancelled.
8. Delivery creates no customer invoice, accounts receivable, revenue or payment. SAL-070/080 own those boundaries.

## Consequences

- OnHand and Reserved decrease together at posting, so Available remains consistent.
- Delivered quantities remain auditable and prevent re-reservation, while the reservation UI shows only the active remaining hold as Reserved.
- Multiple delivery documents can fulfill one order without mutable delivered counters on the order.
- A zero-cost item may post without a monetary journal amount, but physical and delivery evidence still remains; nonzero cost is journaled once.
- A later return/reversal feature must reference the posted delivery and historical cost/tracking evidence rather than editing it.

## Verification

Migration 123 is applied to Demo. Release build passes with zero warnings/errors, 884 focused regression checks pass, the complete disposable SQL suite proves partial then final delivery and one-time physical/COGS effects, and EF reports no pending model change.
