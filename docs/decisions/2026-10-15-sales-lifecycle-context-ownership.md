# Sales lifecycle context ownership and posting boundaries

Date: 2026-10-15
Status: Accepted as `SAL-005` implementation direction

## Context

MiniStore's current `Sale` flow is an immediate sale, invoice, settlement and inventory issue. It is correct for its existing wholesale/POS history but cannot represent quotations, independently confirmed orders, reservations, partial deliveries, receivable invoices or allocated payments. Adding those capabilities inside `SaleService` would duplicate inventory/accounting authority and destroy the distinction between commercial, physical and financial facts.

## Decision

The existing `Sale`/`SalesReturn` flow remains compatible and is not reinterpreted or destructively migrated. New Sales development is additive. Sales owns quotations and Sales Orders; Inventory owns availability, reservation, delivery, tracking, quantity and valuation; Invoicing owns customer invoices and credit notes; Accounting owns journals, periods and receivables; Payments owns customer payments and allocations. Cross-context mutation is coordinated through Application contracts and the existing UnitOfWork, never direct table access from controllers.

Sales Order confirmation reserves but does not reduce OnHand. The first policy requires full reservation and forbids negative Sales stock; partial backorders are a configured later extension. A posted Delivery consumes the reservation, changes physical stock, freezes delivery cost and posts COGS/Inventory. Stocked goods invoice delivered quantity by default; services may invoice ordered quantity through a frozen line policy. A posted customer Invoice records Accounts Receivable, revenue and output tax. A posted customer Payment allocates and clears receivables. These states and quantities remain independent.

New commercial/financial documents freeze transaction currency, company currency, dated rate/source and original/company values. Inventory and COGS remain company-currency facts. The current POS remains an immediate cash-sale path initially and may later reuse common pricing/tax services without forcing the full visible lifecycle on cashiers.

## Alternatives rejected

1. Expand the current `Sale` into a Sales Order: rejected because it changes historical meaning and preserves the existing mixed-responsibility boundary.
2. Decrease stock on order confirmation: rejected because confirmation is commercial intent, not physical delivery.
3. Post receivables at order confirmation or debit cash on an unpaid invoice: rejected because neither reflects the finalized financial fact.
4. Force POS through quotation/order/delivery/invoice/payment documents: rejected for the first release because it adds operational cost without a proven cashier requirement.
5. Introduce a generic event bus, Party model or approval engine immediately: rejected until a concrete slice proves the need and migration boundary.

## Consequences

The new lifecycle requires separate aggregates, source allocations, status projections, permissions, sequences, tenant-safe relationships, concurrency tokens and SQL integration tests. Delivery and invoice posting become separate accounting facts, so COGS may precede revenue when delivery and invoicing occur in different periods; reconciliation reports must make that timing visible. The approach adds documents and orchestration but preserves historical truth, supports partial workflows and keeps Inventory and Accounting authoritative.
