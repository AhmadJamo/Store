# ADR: controlled bank adjustments and close readiness

Date: 2026-11-15  
Status: Accepted

## Decision

Unreconciled bank-statement amounts through a close date are blocking readiness evidence. Fees, interest and transfers are created only by an explicit `BankStatements.Adjust` command. The Application service validates tenant evidence, rowversion, remaining amount, statement date and counterpart rules, then calls `JournalPostingService` and creates the reconciliation allocation in the same UnitOfWork transaction.

## Consequences

- Fiscal-period, balanced-entry and number authority stays in the official journal gateway.
- Fees require Expense/outflow; interest uses Revenue/inflow or Expense/outflow; transfers require another configured bank ledger.
- Account options are permission-protected and named while IDs remain internal.
- Imported evidence and posted entries remain immutable; reasoned Unmatch remains the allocation correction path.
- Candidates and references never create a journal automatically.
