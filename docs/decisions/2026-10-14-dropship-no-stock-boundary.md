# ADR: Direct delivery is purchasing evidence, not internal stock

Date: 2026-10-14  
Status: Accepted

## Decision

A Purchase Order has one frozen fulfillment route: Warehouse or Dropship. Dropship records the recipient, address and immutable partial supplier-to-customer delivery evidence on the purchase line. It never creates a Goods Receipt or internal inventory movement. Only delivered quantity is eligible for a Vendor Bill, which uses the product's frozen purchase Expense account rather than Inventory/GRNI.

Demand allocation is stored separately from fulfillment. Current award-derived orders automatically link to their Purchase Request line. The source contract reserves Inventory Reservation, Sales Order and Production Order without inventing records for modules that do not exist yet. The current immediate Sale is not a future-demand source.

## Consequences

- Direct delivery cannot inflate warehouse stock or GRNI.
- Destination setup is Draft-only; delivery evidence is confirmed-order-only and cumulatively capped.
- A purchase Expense account is required before selecting Dropship.
- Returns/credits follow supplier and future customer-return workflows; they must not manufacture an internal receipt.
