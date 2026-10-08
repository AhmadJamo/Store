# 06 — Implementation Roadmap / خارطة التنفيذ

## Phase 0 — Inventory the existing code (mandatory)
- Enumerate solutions, projects, entities, migrations, controllers, services, repositories, tests, routes and screens.
- Build `EXISTING-FEATURES.md`: implemented / partial / proposed / unknown with file paths.
- Capture current database schema, baseline tests and public API contracts; create backups.
- Compare existing flows (Products, Warehouses, ProductStock, StockTransactions, Suppliers, Purchases, Sales, StockTransfers, Identity, RolePermissions, numbering) **against actual repository**; do not assume memory is the source of truth.
- Create gap tickets, preserving all working custom features.
**Gate:** verified inventory and passing baseline or documented existing failures.

## Phase 1 — Foundations
Organization/companies/branches; Identity and policy authorization; Parties; Catalog/UOM; currency; numbering; audit; outbox/inbox; shared contracts; migration conventions.
**Gate:** cross-company isolation, numbering concurrency, identity permission tests.

## Phase 2 — Inventory core
Warehouses, locations, stock moves, reservations, lots/serials, transfers, inventory count, balances and stock policy. Adapt existing stock-transfer workflow rather than replacing it.
**Gate:** simultaneous reservation and transfer tests; ledger reconciliation.

## Phase 3 — Sales + CRM + pricing
Customer lifecycle, lead/quote/order, taxes/discounts, price lists, returns, fulfillment contracts. Keep accounting effects behind posting contracts.
**Gate:** partial delivery, multiple lines of same product, credit limit and cancel cases.

## Phase 4 — Procurement
RFQ/PO, vendor price rules, receipts, partials, returns, vendor bills and matching. Keep purchasing independent of direct stock-table writes.
**Gate:** PO/receipt/bill mismatch and duplicate receipt prevention.

## Phase 5 — Finance
Chart of accounts, journals, AR/AP, invoice posting, payment allocation, treasury, tax, inventory valuation and reconciliation; define period close.
**Gate:** trial balance always balanced, no duplicate journal, corrections linked to originals.

## Phase 6 — POS + Commerce + Subscriptions
Sessions/cash, refunds, eCommerce order bridge, online payment reconciliation, recurring billing and rental if required.
**Gate:** offline replay, duplicate webhook, session reclose and renewal rerun.

## Phase 7 — Manufacturing + Quality + Maintenance
BOM revisions, routings, material issue, WIP, finished goods, scrap, QC holds and equipment maintenance.
**Gate:** double consumption prevention, traceability and repeated BOM component acceptance.

## Phase 8 — HR + Projects + Services
Employees, attendance, leave, payroll posting, projects, timesheets, helpdesk and field service.
**Gate:** payroll rerun and timesheet billing deduplication.

## Phase 9 — Reporting, BI, integrations and hardening
Projections, dashboards, connectors, backups, recovery, load/security testing and compliance review.
**Gate:** reconciled reports and disaster recovery rehearsal.

## Feature delivery template
For each ticket record: scope; owner; existing behavior; affected entities; dependency graph; command/event contracts; state machine; invariants; permission keys; company scope; posting effects; migrations; UI/API compatibility; tests; rollback/compensation; documentation updates.

## PR merge gates
1. All old tests pass; changed behavior approved.
2. Unit, integration, authorization, concurrency, retry and reversal tests for affected paths.
3. Migration reviewed and data-preserving; seeded legacy data validated.
4. No new direct cross-context writes or circular project references.
5. Financial/inventory totals reconciled; documentation and ADR updated.
