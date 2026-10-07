# Reviewed replenishment suggestions

## Decision
Replenishment policy is tenant-owned per Product + destination Warehouse. It stores minimum, maximum, safety stock, lead days and an optional preferred source warehouse. A suggestion appears when Available is at or below Minimum and proposes `Maximum - Available`.

Suggestions are read-only planning output. They never change stock or automatically create purchase, transfer or manufacturing documents. A later reviewed workflow may convert one into a draft business document while retaining its normal approval rules.

Approved transfers are confirmed forecast movements. Incoming is added to Available; outbound is shown but not deducted from Available a second time because approval already created its reservation. Posted transfers are already reflected in OnHand. The current Purchase entity is an immediate receipt, not an order, and therefore is not treated as future incoming.

## Rationale
This separates planning from physical and financial posting, uses the reservation-aware InventoryBalance projection, and avoids creating orders from incomplete demand forecasts.
