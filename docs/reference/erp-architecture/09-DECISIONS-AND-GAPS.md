# 09 — Architecture Decisions and Open Questions

## ADR log (proposed; confirm against repo)
| ID | Decision | Status | Consequence |
|---|---|---|---|
| ADR-001 | Modular monolith, DDD layering | Proposed | explicit bounded context interfaces |
| ADR-002 | EF Core + SQL Server | Proposed/current-project expectation | SQL migrations and concurrency tokens |
| ADR-003 | Manual DTO mapping, no AutoMapper | Proposed preference | more explicit code |
| ADR-004 | Outbox/inbox for async integrations | Proposed | reliable delivery + dedup |
| ADR-005 | Ledger-based financial and stock truth | Proposed | reconciled projections |
| ADR-006 | Immutable postings, reversals only | Proposed | auditable correction flows |
| ADR-007 | Multi-company scope in every transaction | Proposed | isolation tests and indexes |
| ADR-008 | Document numbering configurable by scope | Proposed | concurrency-safe allocation |

## Decisions required before production design
1. Single business vs SaaS multi-tenant? Shared DB or DB-per-tenant?
2. Exact target countries and tax/e-invoicing/local payroll rules?
3. Inventory costing: FIFO, average, standard; perpetual vs periodic accounting?
4. Sales invoice trigger: order, delivery, milestone or per-customer contract?
5. Negative stock policy and reservations/backorders per warehouse/product?
6. Purchase matching: 2-way/3-way, price/quantity tolerances, GRNI?
7. Numbering: when assigned, scope, reset and legally permissible gaps?
8. Currency exchange rate provider and rounding precision?
9. Is an existing Sales/StockTransfer status workflow already deployed and used by customers?
10. Should cross-company product/party masters be shared or isolated?
11. Payroll, payment gateways, carrier, e-invoicing and POS offline integrations?
12. Target deployment, load, backup RPO/RTO and compliance retention?

## Repository gap analysis template
| Module | Current files/tables | Implemented behaviors | Proposed changes | Data migration | Regression tests | Owner | Status |
|---|---|---|---|---|---|---|---|
| Inventory | TODO inspect | TODO | TODO | TODO | TODO | TBD | Unknown |
| Sales | TODO inspect | TODO | TODO | TODO | TODO | TBD | Unknown |
| Purchases | TODO inspect | TODO | TODO | TODO | TODO | TBD | Unknown |
| Accounting | TODO inspect | TODO | TODO | TODO | TODO | TBD | Unknown |

## Change-control procedure
Every accepted design change gets ADR with date, alternatives, rationale, affected modules, migration/rollback, tests and explicit owner approval. Superseded decisions remain in history.
