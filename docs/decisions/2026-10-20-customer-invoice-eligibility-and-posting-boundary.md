# ADR: Customer invoice eligibility and posting boundary
> Date: 2026-10-20 | Status: Accepted

## Context

The professional sales flow already separates quotation, order, inventory reservation and physical delivery. Customer invoicing must support partial and multiple documents without reusing the legacy immediate Sale/POS model or implying that approval is payment.

## Decision

1. `CustomerInvoice` is an additive tenant aggregate with Draft, Approved and Cancelled states, `INV-` numbering, idempotent request key, rowversion, immutable snapshots and semantic history.
2. Each company chooses Delivered or Ordered eligibility. Delivered is the stocked-product default; services always use ordered quantity.
3. Delivered-basis lines store exact Posted delivery-line allocations. Draft and Approved quantities are both committed; cancellation releases them.
4. Partial money is proportional, while the last active quantity takes the rounding residual.
5. SAL-070 approval creates no inventory, journal, receivable or payment effect. A Posted state is not exposed until SAL-080 can atomically create the source-unique revenue/tax/receivable journal and reversal evidence.
6. Changing the eligibility policy after dependent customer invoices exist is blocked pending a controlled transition workflow.

## Consequences

The UI can safely create and approve auditable partial invoices and clearly guide the next step without presenting a financially false Posted state. SAL-080 must reuse the same aggregate rather than create a second invoice model, and must keep accounting posting separate from settlement in SAL-090.

## Verification

Migration 124 is applied to Demo with no pending model changes. 908 focused regression checks pass. The disposable SQL suite proves 0.4/0.6 partial invoices, exact delivery allocations, over-invoicing rejection, cancellation release, request idempotency, tenant isolation and zero new stock, movement or journal effects.
