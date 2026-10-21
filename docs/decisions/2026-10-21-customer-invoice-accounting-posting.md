# ADR — Customer invoice accounting posting and reversal
> Status: Accepted and implemented | Date: 2026-10-21

## Context

SAL-070 created independently approved customer invoices but deliberately produced no ledger or payment effect. SAL-080 must recognize receivable, revenue and output tax once, preserve source dimensions and allow correction without deleting history or duplicating delivery-time inventory/COGS.

## Decision

1. Only Approved invoices may post; only Posted invoices may reverse. Dedicated Post and Reverse permissions remain separate from commercial approval.
2. `CustomerInvoiceService` resolves tenant-filtered customer, account, tax, delivery, warehouse and branch evidence and calls the existing `JournalPostingService` inside the same Serializable UnitOfWork.
3. Posting freezes company-currency line/header amounts, the customer receivable account, default revenue account and each taxed line's output-tax account. The source-unique pair is `CustomerInvoice + InvoiceNumber`.
4. Delivered-basis amounts are distributed proportionally across exact source deliveries using their warehouse/branch dimensions; the last slice receives each rounding residual. A branch revenue override supersedes the company default. Ordered/service lines remain company-level because the current order model has no branch dimension.
5. The accounting entry debits receivable and credits net revenue/output tax. Delivery already owns stock and COGS, so invoice posting never mutates inventory.
6. Reversal creates an exact inverse journal through the central gateway with source `CustomerInvoiceReversal`, preserves the original entry as Reversed, stores date/reason/actor and releases invoice eligibility. It never deletes history.
7. Journal evidence is exposed through a dedicated read-only `JournalEntries.View` page; it is not an editing surface.
8. Posting does not create payment or mark the invoice paid. SAL-090 owns payments and allocations.

## Consequences

- Receivable and ledger evidence are balanced, tenant-safe, source-idempotent and explainable by invoice, account and dimension.
- Existing Draft/Approved/Cancelled invoices are preserved by an additive migration with nullable posting evidence.
- Ordered/service invoices currently have no branch dimension; introducing a required order branch is a separate model decision, not inferred during posting.
