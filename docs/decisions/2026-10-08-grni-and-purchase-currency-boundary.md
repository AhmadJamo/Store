# GRNI accounting and staged purchase-currency boundary

Date: 2026-10-08  
Status: Accepted as target; activation deferred to owning slices

## Context

The new purchase flow separates physical receipt from the supplier invoice. Posting Accounts Payable at PO confirmation would recognize a liability too early. Waiting until the Vendor Bill to recognize received inventory in the general ledger would allow the inventory subledger and GL to diverge. MiniStore also stores a company currency setting but does not yet have a complete multi-currency accounting, rate or foreign-exchange revaluation engine.

## Decision

Use GRNI for the new procure-to-pay flow. A posted Goods Receipt will debit Inventory and credit GRNI. A posted Vendor Bill will clear GRNI for matched receipt value, recognize recoverable input tax, record purchase-price variance as required and credit Accounts Payable. Supplier payment clears Accounts Payable through Accounting/Treasury behavior. Each posting uses the central journal gateway, fiscal-period validation, stable source type/id and retry-safe posting key.

Currency support is staged. Commercial records carry an uppercase currency code, while confirmed/posted monetary documents eventually freeze transaction currency, base currency, exchange rate, rate date/source and rounding. Non-base-currency inventory or journal posting remains disabled until a dedicated accounting slice implements exchange-rate policy, precision, realized/unrealized gain/loss accounts and reconciliation tests. PUR-010 does not claim full multi-currency support.

## Alternatives rejected

1. Credit Accounts Payable when the PO is confirmed: rejected because a purchase commitment is not the supplier invoice liability.
2. Record no GL effect at receipt: rejected as the target because physically received valued inventory would not reconcile to the general ledger before billing.
3. Treat a currency code as complete multi-currency support: rejected because rates, rounding, base values and FX differences are necessary for correct posting.
4. Change Legacy Direct Purchase accounting immediately: rejected because its history and current posting behavior must remain compatible until a separate cutover.

## Consequences

PUR-065 requires configured GRNI and purchase-price-variance accounts, reversal rules and inventory-to-GL reconciliation. PUR-080 requires AP posting and matching allocations. Legacy and new purchase postings use distinct source identities. Supplier prices can be designed for future currencies without enabling unsupported foreign-currency accounting early.
