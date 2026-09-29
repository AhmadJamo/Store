# Inventory reconciliation and future movement identity contract

Date: 2026-09-30  
Status: Accepted

## Context

The current inventory model stores warehouse quantity/value in ProductStock, exact assigned quantity in ProductLocationStock and the latest warehouse quantity/value snapshot in StockTransaction. Before introducing StockMovement or InventoryBalance, MiniStore needs a tenant-safe read-only baseline that exposes disagreement without silently correcting it. Future dual-write also needs a stable source identity and controlled cutover states.

## Decision

Inventory reconciliation is keyed by Tenant + Product + Warehouse. It compares ProductStock quantity/value, the database aggregate of ProductLocationStock, derived Unassigned quantity and the latest StockTransaction by monotonically increasing Id. It also reports orphan location balances, negative location quantities and location balances inside Simple warehouses. Quantity tolerance is 0.000001 and value tolerance is 0.00000001, matching current persisted precision. The report is read-only and no migration or automatic correction is allowed.

Future physical movements will identify their business source with language-neutral structured fields: SourceType, SourceId, optional SourceLineId, Action and Ordinal. The idempotency key is tenant-scoped and deterministically derived from those fields; display document numbers and translated labels are never idempotency keys. Any user-entered reference remains descriptive only.

Movement rollout uses an independently configurable state per tenant and producer workflow: Off, DualWrite, Compare, ReadNew and Enforced. Moving forward requires zero unexplained reconciliation differences and the phase's SQL integration checks. Moving backward changes the read/producer state; it never deletes or rewrites posted facts.

## Consequences

Administrators can distinguish implicit Unassigned quantity from invalid over-allocation and stale movement snapshots before cutover. Repository queries aggregate inside SQL Server and tenant query filters remain effective. Reconciliation currently pages after the filtered snapshot is evaluated in the Application layer; if the balance set becomes operationally large, server-side exception classification/paging must be introduced before production scale.

WMS-000 does not add StockMovement fields or a feature-flag table. Those structures are implemented with the producer they control in WMS-020, using this contract and a separate schema migration.
