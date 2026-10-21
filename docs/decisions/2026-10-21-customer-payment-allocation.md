# ADR: Independent customer receipts and derived invoice balances

Date: 2026-10-21  
Status: Accepted

## Decision

Represent collection as an independent `CustomerPayment` aggregate with immutable allocation lines. Do not store a manually editable paid flag or balance on `CustomerInvoice`. Derive paid and outstanding amounts from Posted payment allocations; Reversed payments remain historical but are excluded.

The payment snapshots its settlement account and dated exchange rate. Each allocation snapshots the invoice receivable account and carrying amount. Accounting debits settlement, credits receivable and posts explicit realized exchange gain/loss. Invoice and payment reversal are ordered: allocated payments must be reversed first.

## Consequences

- Partial and multi-invoice receipts are supported without rewriting invoice posting evidence.
- Serializable allocation checks plus rowversion and source-unique journals prevent over-allocation and duplicate posting.
- Historical account/rate changes do not rewrite posted receipts.
- Customer credit balances, refunds and credit notes remain outside SAL-090.
