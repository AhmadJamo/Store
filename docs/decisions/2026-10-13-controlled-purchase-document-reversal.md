# ADR: Controlled purchase-document reversal

## Decision

Posted Goods Receipts, Vendor Bills, Supplier Payments and Landed Costs are corrected through a separate immutable `PurchaseDocumentReversal` record. The original business document is retained and moved to `Reversed`; its original journal is retained as `Reversed`, while a new posted journal swaps every debit and credit. Ledger queries include both Posted and Reversed originals so the original and opposite entry net to zero.

The reversal record uses a polymorphic document type/id pair rather than four nullable foreign keys. Tenant-scoped unique indexes on original type/id and the client request key prevent duplicate reversal and make safe retries return the existing result.

## Dependency policy

Reversal proceeds from the latest dependent document backwards. A Vendor Bill cannot be reversed while a posted Supplier Payment or Supplier Credit Note depends on it. A Goods Receipt cannot be reversed while a posted return, Vendor Bill or Landed Cost depends on it. Supplier Credit Notes remain the correct path for billed stock returns and are not replaced by general reversal.

Physical reversal is allowed only when the original stock remains available and its valuation has not been changed by later activity. Landed Cost capitalization is reversible only while its related quantity and average cost remain intact. Otherwise the operation is rejected with an explanatory message.

## Consequences

- Audit history remains immutable and the correcting actor, date, reason, original document and opposite journal are explicit.
- Repeated requests cannot duplicate stock or accounting effects.
- Some corrections require reversing downstream documents first; consumed or revalued inventory requires the normal return/correction workflow rather than destructive history edits.
- Automatically closed Purchase Orders reopen when reversing their completing receipt or bill.
