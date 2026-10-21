# Inspect customer returns before inventory posting
> Status: Accepted — 2026-10-21

## Decision

Return custody and inspection are persisted before any inventory posting. An Inspected receipt freezes Restock, Quarantine, Reject or Destroy decisions and exact tracked identities. It does not make goods available or post accounting.

This boundary supports untracked and tracked products consistently and avoids treating quarantined or rejected goods as available stock. `SAL-100C` will apply accepted dispositions through the authoritative inventory/tracking/accounting services.

## Consequences

- Partial/multiple receipts are supported with cumulative caps.
- A request cannot be cancelled while it has an active receipt.
- Completed inspection is immutable; its one-time action disappears.
- Credit notes and refunds remain separate from physical custody.
