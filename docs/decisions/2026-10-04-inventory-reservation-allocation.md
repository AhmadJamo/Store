# ADR: Inventory reservation and exact allocation

Date: 2026-10-04  
Status: Accepted

## Context

InventoryBalance exposes OnHand, Reserved and Available, but no business document owned Reserved. Approved transfers could therefore compete with sales or other transfers before posting.

## Decision

Introduce a tenant-owned `InventoryReservation` aggregate with immutable dimensional lines, a unique source identity and Active/Consumed/Released lifecycle. A line allocates a product quantity at one warehouse and either one exact reservable location or the protected Unassigned position.

Transfer approval creates the reservation and increases `InventoryBalance.Reserved` in the same serializable transaction. Transfer posting consumes it before stock movement. Posting an older approved transfer safely creates and immediately consumes its missing reservation for compatibility. Source uniqueness makes retries idempotent, rowversions reject concurrent claims, and the balance invariant prohibits over-reservation.

Immediate POS and invoice sales continue to issue stock atomically, but now validate aggregate Available so they cannot consume quantities held by active reservations. They do not create zero-duration reservations. The generic SaleOrder source type is reserved for a future staged sales-order workflow.

## Consequences

- Available stock now has auditable ownership.
- Exact locations must be marked reservable; Unassigned remains supported.
- Reservation close operations release Reserved exactly once.
- Posted/cancelled transfer quantity and AVCO behavior remain unchanged.
- Existing approved transfers need no destructive backfill.
