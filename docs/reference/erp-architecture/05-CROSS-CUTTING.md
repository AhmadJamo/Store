# 05 — Cross-Cutting Rules / قواعد مشتركة

## Numbering
- Sequence scope = company + document type + optional branch + fiscal period; prefix/suffix templates and reset rules configurable.
- Separate internal immutable ID from human-readable document number.
- Define whether gaps are permitted by document type and jurisdiction. Never promise gapless sequences with naive DB identity.
- Reserve/assign at explicit lifecycle transition (e.g., invoice posting), under concurrency-safe lock/transaction.
- Preview is non-consuming; retries must reuse previously allocated number. Voided numbers remain auditable.

## Authorization
- Policy-based permissions e.g. `Sales.View`, `Sales.Create`, `Sales.Confirm`, `Sales.Cancel`, `Inventory.Transfer.Post`, `Accounting.Period.Close`.
- Enforce company, branch, warehouse, ownership and amount thresholds; use separation of duties for approvals.
- Backend checks on every command and filtered query. Record approver ID/time, reason and delegated authority.
- Permissions default deny; background jobs use dedicated service principals.

## Accounting and tax
- Journal entry: posting date, period, currency, source, balanced debit/credit, immutable after posting.
- Define mapping by transaction type and product/category/company: AR/AP, revenue, tax, stock asset, COGS, GRNI, WIP, variance, cash/bank.
- Tax calculation uses effective-dated tax rules, included/excluded tax policy, exemptions and invoice snapshots; country-specific compliance requires local review.
- Multi-currency: transaction currency vs company base currency, rate source/date, realized/unrealized FX gain/loss and rounding differences.
- Period closing blocks backdated posting except authorized reopening workflow.

## Inventory and costing
- Location types: internal, vendor, customer, transit, production, scrap, adjustment.
- Track on-hand, reserved, forecast, lot/serial, expiry, ownership/consignment and optional packages.
- Costing method configurable by product/category/company: FIFO, average, standard (subject to chosen accounting policy). Do not mix costing methods mid-period without migration plan.
- Backdated moves, negative stock, landed cost, unit conversions and serial uniqueness require explicit policies.

## Audit and observability
- Append-only audit: actor, action, entity, old/new nonsecret fields, UTC timestamp, correlation, IP/device if legally appropriate, approval reason.
- Structured logs + traces + metrics; alerts for outbox backlog, posting failures, unreconciled balances, negative stock exceptions and job failures.
- Never log passwords, access tokens, payment secrets or unnecessary personal data.

## Multi-company / branch / tenancy
- Determine tenant model before schema migration: shared DB with scoped rows vs database per tenant.
- Enforce scope on read, write, background job and reports; do not trust client-provided CompanyId without membership check.
- Intercompany transactions require paired documents, clearing accounts and explicit elimination/consolidation rules.

## Configuration and localization
- Effective-dated fiscal calendars, taxes, currencies, time zones, languages, UOM and pricing rules.
- Feature flags scoped by company and rollout; defaults safe and recorded.
- Retention, privacy, e-invoicing and payroll legislation vary by jurisdiction and require country-specific adapters.

## Nonfunctional baselines (to agree)
- Define SLA, max users, peak transactions, data retention, backup RPO/RTO, performance budgets, accessibility and localization test criteria before production.
- Use SQL migrations with tested rollback/forward-fix and production-like backup restore drills.
