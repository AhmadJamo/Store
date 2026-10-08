# Additive procure-to-pay evolution with legacy direct-purchase compatibility

Date: 2026-10-08  
Status: Accepted as implementation direction

## Context

MiniStore's current `Purchase` workflow validates a supplier invoice and immediately receives inventory, updates moving weighted-average cost and writes inventory history. General-ledger posting is a later action. The aggregate has no request, sourcing, approval, purchase-order, partial-receipt, vendor-bill or payment lifecycle. Reinterpreting it as a purchase order would change the meaning of existing rows and break purchase returns, inventory references and accounting idempotency.

## Decision

Keep the current aggregate and history as the compatible Legacy Direct Purchase workflow. Introduce new procure-to-pay aggregates additively: Purchase Request, sourcing event, Supplier Quotation, Purchase Order, Goods Receipt, Vendor Bill and their approval/matching histories.

A Purchase Order records commercial commitment and never changes stock. Only a posted Goods Receipt changes inventory through existing Application-layer inventory services. Supplier liability belongs to Vendor Bill. Receipt accounting will use GRNI so inventory and the general ledger remain aligned when receipt and invoice occur at different times.

Existing purchases will not be backfilled into fabricated orders, receipts or bills. New and legacy workflows may operate side by side until reconciliation and business acceptance justify disabling new legacy creation. Historical legacy reads and returns remain supported.

## Alternatives rejected

1. Convert `Purchase` into `PurchaseOrder`: rejected because current creation has already received stock and the historical meaning cannot safely change.
2. Add statuses and nullable receipt/bill fields to the same aggregate: rejected because it would preserve mixed responsibilities, make partial workflows ambiguous and create conditional invariants throughout the model.
3. Replace the existing purchase subsystem in one migration: rejected because it risks inventory valuation, return history, accounting references and tenant data.
4. Post supplier payable at PO confirmation: rejected because an approved order is a commitment, not an invoice liability.
5. Delay all receipt accounting until the vendor bill: rejected as the target because inventory subledger value could diverge from the general ledger; GRNI provides a controlled interim liability.

## Consequences

The transition temporarily exposes two purchase entry modes and requires clear Legacy labels. New document tables, numbering types, permissions and accounting settings are needed. Reporting must deliberately combine or distinguish legacy and new documents. The additive design costs more than renaming the current aggregate but preserves historical truth and provides clean boundaries for partial receipts, billing, matching, approvals and future integrations.
