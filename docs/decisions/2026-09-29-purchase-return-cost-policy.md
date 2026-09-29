# Purchase-return cost and posting policy

Date: 2026-09-29  
Status: Accepted

## Context
A supplier return must reverse the original commercial amounts while inventory may now carry a different moving-average cost because later receipts and issues changed the pool.

## Decision
Purchase returns are immutable, immediately posted documents linked to a posted original purchase. Commercial amounts—supplier payable, input tax and purchase discount—are reversed proportionally from the original line. Physical inventory is issued at the current moving weighted-average cost. The journal uses its currency-rounded value, and the difference between the original proportional inventory amount and that value is posted to the configured cost-of-sales account as purchase-return cost variance. Quantity cannot exceed either the unreturned original quantity or currently available stock.

## Consequences
The inventory ledger and general-ledger inventory reduction use the same moving-average amount at their respective precision, while supplier and tax balances retain the original invoice basis. Returns require the original purchase mappings and an Open fiscal period; a non-zero cost difference also requires the configured cost-of-sales account.
