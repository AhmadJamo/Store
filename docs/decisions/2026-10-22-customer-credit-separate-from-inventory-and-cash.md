# Customer credit is separate from inventory and cash

- Date: 2026-10-22
- Status: Accepted for SAL-100D

## Context

A physical customer return, the commercial reduction of an invoice and a cash refund are different facts. Combining them would repeat inventory when disposition was already posted, hide customer-credit balances and make partial/multiple invoice and receipt allocations difficult to reconcile.

## Decision

`CustomerCreditNote` is an independent Posted financial document. Each line must reference one Posted customer-invoice line and one Posted customer-return receipt line from the same sales-order source. Cumulative validation caps credit by invoice quantity, exact delivery allocation and receipt quantity. A request key provides retry idempotency.

Posting reverses the invoice's frozen receivable, revenue and output-tax evidence. It does not call inventory/tracking services and does not create a payment/refund. Invoice and payment outstanding projections subtract Posted credit notes. A posted invoice with credit evidence cannot use the ordinary invoice reversal path.

## Consequences

- Inventory disposition and COGS remain owned by SAL-100C.
- Financial credit is auditable and can differ in timing from physical receipt.
- Cash refund or applying credit to another balance requires a later explicit controlled document/action.
- Reversal of a posted credit note is not invented implicitly; it needs a separately designed forward-control workflow.
