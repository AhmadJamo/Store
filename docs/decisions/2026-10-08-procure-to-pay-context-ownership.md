# Procure-to-pay context ownership and integration boundaries

Date: 2026-10-08  
Status: Accepted as implementation direction

## Context

MiniStore already has separate purchasing, inventory, accounting, settings and security code, but the legacy `PurchaseService` combines supplier invoice capture with immediate inventory receipt. The new procure-to-pay flow introduces more documents and could create duplicate sources of truth or direct cross-module table writes if ownership is not fixed first. The imported ERP reference also proposes broader Party, Billing/AP and integration contexts that are not currently implemented.

## Decision

Purchasing owns Purchase Request, sourcing, Supplier Quotation and Purchase Order. The existing Supplier remains the purchasing counterparty until a separately approved Party migration. Inventory owns physical quantities, tracking identities, balances, movements and valuation mutations. Purchasing orchestrates Goods Receipt against a PO through Inventory Application contracts but never writes inventory tables directly. Accounting/AP owns Vendor Bill, payable, payment allocation and settlement; Accounting alone owns journals and periods.

Ordered, received, returned, billed and paid quantities remain distinct. Progress values are derived from posted source records or transactionally maintained, reconcilable projections. Cross-module work uses Application contracts and the existing UnitOfWork when synchronous atomicity is required. Future integration events are reserved facts, not an instruction to add an event bus before a concrete consumer exists.

## Alternatives rejected

1. Let the Purchase aggregate own stock and AP state: rejected because it repeats the legacy mixed-responsibility problem and prevents partial receipt/billing authority.
2. Create a new generic Party immediately: rejected because Supplier and Customer are already implemented and a safe unification requires broader migration evidence.
3. Allow services to update other modules' repositories directly: rejected because it bypasses domain rules and creates competing transaction ownership.
4. Add a generic event bus/outbox now: rejected because there is no implemented asynchronous consumer requirement in PUR-010; the contract is reserved and infrastructure will be added with an owning slice.

## Consequences

Later slices require explicit application ports between Purchasing, Inventory and Accounting. Goods Receipt is coordinated by Purchasing but delegates all physical mutation to Inventory. Vendor Bill is not placed inside the Purchase Order aggregate. Some future reference-architecture names will differ from current folders until broader contexts are justified. The boundary costs additional DTOs and orchestration but keeps each source of truth auditable and replaceable.
