# Fiscal-period posting control

Date: 2026-09-29  
Status: Accepted

## Context
All purchase, sale and sales-return journals now pass through one posting gateway. The system needs company-specific period close controls without breaking existing companies that have never configured an accounting calendar.

## Decision
Store non-overlapping tenant fiscal periods with Open, Soft Closed and Closed states. The central `JournalPostingService` asks `FiscalPeriodService` to validate every posting date before generating or persisting a journal. If no period exists for a tenant, posting remains available for compatibility. Once the first period exists, the date must be covered by an Open period. Soft Closed and Closed both block current operational postings. Status changes require a reason, record UTC time and user, and use SQL Server rowversion concurrency.

## Consequences
All current and future workflows using the central gateway inherit the same date rule. Administrators must cover every allowed accounting date after activation. A future manual-adjustment workflow may introduce a separate permissioned override for Soft Closed without weakening Closed periods.
