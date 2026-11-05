# ADR: Server-authored financial reports over journal evidence

> Date: 2026-11-06  
> Status: Accepted

## Context

The Angular accounting workspace needed reconciliable financial statements without duplicating accounting logic in the
browser or introducing a second ledger/read-store authority.

## Decision

Aggregate tenant-filtered journal lines in the repository, calculate report semantics in `FinancialReportService`, and send
read-only DTOs to Angular. Exclude Draft entries. Include both Posted and Reversed originals because reversal is represented
by a separate posted opposite entry; excluding the original would incorrectly leave the opposite entry unbalanced.

Hierarchy rollups are presentation rows. Grand totals use direct ledger movements to prevent double counting. The existing
`JournalEntries.View` permission protects both reports and evidence drill-down.

## Consequences

- The Application layer remains the single report-definition authority and Angular performs no financial calculation.
- Existing tenant filters and company-currency ledger boundaries are reused; no schema or transaction mechanism is added.
- Query performance must be measured on production-like journal volume before production readiness.
- Cash flow, consolidation, budgets, close processing and manual journals require separate decisions.
