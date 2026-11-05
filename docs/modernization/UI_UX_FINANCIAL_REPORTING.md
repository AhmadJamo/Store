# UX-011 — Read-only financial reporting foundation

> Completed: 2026-11-06

## Delivered

- `/accounting/financial-reports` provides a bilingual, RTL-safe trial balance, balance sheet and income statement.
- `FinancialReportService` calculates every balance on the server; Angular only formats values and navigation.
- Period and selected report tab are allowlisted through `PageContextService` and survive safe navigation.
- Every account row can drill into `/accounting/journal-entries` with exact dates and `accountId`; the repository applies
  the account filter, so the browser does not infer journal membership.
- The accounting hub exposes the report only to `JournalEntries.View` users.

## Accounting definitions

- Draft entries are excluded. Posted entries and entries marked Reversed are included because the latter preserve the
  original evidence and are neutralized by a separate posted opposite entry.
- Trial-balance movement is debit and credit activity inside the selected inclusive period. Closing balance contains all
  non-draft activity through the selected end date.
- Assets and expenses use debit-normal presentation. Liabilities, equity and revenue use credit-normal presentation.
- The balance sheet is as of the end date. Current earnings equal cumulative revenue less cumulative expense balances and
  are included with liabilities and equity.
- The income statement uses only selected-period revenue and expense movements.
- Account hierarchy rollups are server-authored. Statement and trial totals use direct account movements so parent rollups
  cannot double count the ledger.
- Values remain in the current tenant/company ledger currency; this phase adds no conversion or consolidation model.

## Boundaries

- No database schema, journal posting, fiscal-period, reversal or permission definition changed.
- No manual journal create/edit/delete/post endpoint was added.
- Tenant isolation remains enforced by the existing EF query filters on entries, lines and accounts.
- Cash flow, budgets, consolidation and multi-company reporting remain outside this phase.

## Verification

- .NET Web Release: zero warnings and zero errors.
- Security/accounting regression: 1,847 checks, including a balanced golden ledger proving trial balance, balance sheet,
  current earnings and period net income.
- Angular: 237/237 tests across 112 files.
- Static accessibility: 133 templates.
- UI consistency: 116 feature pages, 105 shared headers, 11 documented exceptions, 74 destinations and 43/43 operational
  routes.
- Production budgets: 336,179/358,400 initial bytes and 81,880/81,920 largest lazy bytes.
- Hosted browser acceptance: 6 passed, zero failed, covering states, RTL/LTR, keyboard and 390px responsive layout; two
  authenticated reconciliation checks remained skipped because no external browser storage state was supplied.

## Recorded risks

- A ledger with historical unclosed revenue/expense accounts relies on the documented current-earnings calculation. Formal
  year-close journals remain a future accounting workflow.
- Financial performance at production ledger scale has not been load-tested; the repository aggregation is server-side and
  tenant-filtered, but indexes/query plans must be measured against production-like volume before production sign-off.
