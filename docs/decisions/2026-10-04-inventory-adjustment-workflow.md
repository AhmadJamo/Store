# ADR: Controlled inventory adjustment workflow

Date: 2026-10-04  
Status: Accepted

## Decision

Inventory differences use a tenant-owned `InventoryAdjustment` document with Draft, Counted, Approved, Posted and Cancelled states. Creation freezes the expected dimensional OnHand quantity. Blind counts hide this snapshot until counting is completed. The counter cannot approve their own document.

Posting rechecks that OnHand still equals the frozen snapshot and that the counted quantity is not below an active reservation. Each non-zero variance updates ProductStock and optional ProductLocationStock atomically, writes an AVCO-valued AdjustmentIn/AdjustmentOut StockTransaction and writes a linked posted StockMovement.

No general-ledger entry is invented in this phase. Accounting posting waits for explicit inventory gain/loss account configuration.

## Consequences

- Posted documents are immutable audit evidence.
- Stale counts must be cancelled and restarted rather than silently rebased.
- Exact locations must be active and countable.
- Cycle schedules and assignments remain a later extension of this stable posting workflow.
