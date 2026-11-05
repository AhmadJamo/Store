# Accounting module

> Last reviewed: 2026-11-06

The current Angular accounting presentation includes the chart-of-accounts list/create surface, posted journal-entry detail
routes reached from business documents, and administration-owned accounting setup for fiscal periods, exchange rates and
branch sales-account mappings. The `/accounting` module hub exposes only permitted existing workspaces.

UX-010 projects the chart as a server-authored expandable hierarchy with safe search and adds a read-only journal
directory under `/accounting/journal-entries`. The directory is filtered and paged in the repository, exposes balanced
debit/credit evidence and links to immutable details under the existing `JournalEntries.View` permission. Tenant query
filters apply to accounts, entries and lines. Account creation can attach a new account only to an existing tenant parent;
no re-parent operation exists, so the management surface cannot introduce a cycle.

UX-011 adds `/accounting/financial-reports`: a server-calculated trial balance, balance sheet and income statement with
safe period/tab context and exact journal-evidence drill-down. Drafts are excluded; Posted and Reversed evidence remains
included so separately posted reversals net correctly. Existing tenant filters and `JournalEntries.View` remain authoritative.

UX-012 adds controlled manual Draft creation, explicit fiscal-period-protected posting and exact separate-entry reversal.
The workspace uses named tenant accounts/dimensions, stable retry keys, memory-only unsaved drafts and independent
View/Create/Post/Reverse permissions. Posted evidence remains immutable.

Current limitations remain explicit: account re-parenting/status management, AR/AP aging, cash flow, close processing,
recurring/bulk journals, budgets and consolidation are not implemented. Operational posting and fiscal-period rules remain
unchanged; manual journals reuse them.
