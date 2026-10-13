# Mixed purchase billing source and posting policy

> Status: Accepted  
> Date: 2026-10-13

## Context

PUR-220 requires one controlled Vendor Bill workflow for stocked goods, purchases billed from ordered quantity and services billed from completion evidence. A Goods Receipt cannot be invented for non-stock costs, and GRNI must not carry service or order-based expense amounts.

## Decision

Each Vendor Bill line freezes its Purchase Order billing policy, matching-basis quantity and optional purchase Expense account. Exactly one source shape is valid:

- Received Quantity references one posted Goods Receipt line.
- Service Completion references one immutable Purchase Service Acceptance line.
- Ordered Quantity references the Purchase Order line without receipt evidence.

Draft, Approved and Posted bills reserve source quantities; Cancelled bills do not. Matching compares invoice quantity with the frozen eligible basis and net invoice price with the frozen order/receipt net unit cost.

Posting treats the bill as one payable document but applies each line's accounting policy. Receipt lines debit GRNI at frozen receipt value and route commercial and exchange differences through the configured variance accounts. Ordered and service lines debit their frozen purchase Expense account at bill-date company value. Recoverable input tax and supplier payable use the mixed bill totals.

Supplier Credit Notes remain the physical stock-return path and therefore expose only receipt-based bill lines. A future non-stock commercial credit flow must not create inventory movement.

## Consequences

- One supplier invoice can represent realistic mixed goods and services without synthetic receipts.
- Purchase Order completion works consistently across all three billing policies.
- Historical posting remains reproducible if product configuration changes later.
- Reversal and reconciliation logic can distinguish GRNI-bearing lines from direct Expense lines.
- Existing Vendor Bill lines are backfilled as Received Quantity using their prior receipt-clearing quantity.
