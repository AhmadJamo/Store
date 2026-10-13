# ADR: Immutable line-level split supplier awards

> Status: Accepted  
> Date: 2026-10-13  
> Scope: PUR-210 quotation award and Purchase Order creation

## Context

A single winning quotation cannot represent choosing different suppliers by product or dividing one approved quantity between suppliers. Replacing the historical quotation itself would lose the commercial evidence used to justify and reproduce the purchasing decision.

## Decision

One `PurchaseQuotationAward` remains the one-time decision header for a sourcing event. Its tenant-owned `PurchaseQuotationAwardAllocation` children are the authoritative decision evidence. Each child identifies one submitted quotation line and freezes the sourcing line, supplier, awarded quantity, unit price, discount, tax, currency, lead time, payment terms, product/unit labels and required reason.

The Application service resolves posted quotation-line IDs inside the current sourcing event, rejects duplicate or foreign lines and requires the allocation sum for every sourcing line to equal the requested quantity exactly. Award creation and sourcing-event closure commit in one Serializable transaction. The retained header `SupplierQuotationId` is compatibility metadata for existing records; new downstream behavior reads allocations.

Purchase Order creation groups allocations by supplier quotation. Because a sourcing event already fixes one warehouse and each supplier has one quotation per event, this also fixes supplier, currency and warehouse per generated order. Each order copies only its allocated quantities and frozen commercial terms. A split allocation cannot create a Purchase Agreement; agreements require one complete allocation per sourcing line from one quotation.

## Consequences

- One sourcing event can create multiple Draft Purchase Orders without duplicating or dropping approved demand.
- Historical awards do not change when supplier defaults, quotations or currency settings change later.
- The award button disappears after completion; generated orders remain separately subject to create, approve and confirm permissions.
- The migration backfills old whole-quotation awards as one allocation per quotation line and closes their sourcing events without creating orders, inventory effects or journals.
- A SQL integration scenario with three products and two suppliers verifies exact totals, two orders and preserved allocation traceability.
