# UX-012 — Controlled manual journal foundation

> Completed: 2026-11-06

## Delivered

- `/accounting/manual-journals` provides a permission-aware directory and balanced multi-line draft composer.
- `/accounting/manual-journals/{id}` exposes immutable evidence plus the eligible Post or Reverse action.
- Account, branch and warehouse selectors are tenant-filtered and name-based; IDs remain internal.
- Unsaved form content and its request key are retained only in `PageContextService` memory and cleared after success.
- A stable request key survives retry. Reuse with different content or another reversal is rejected.

## Authority and lifecycle

- `ManualJournals.View`, `.Create`, `.Post` and `.Reverse` are separate permissions. Create-only users are not forced
  through the View endpoint.
- Create persists a balanced `ManualJournal` in Draft status. A Draft has no financial-report effect.
- Post passes the tracked Draft through `JournalPostingService.PostDraftAsync`, which enforces the existing fiscal-period
  policy before the domain entity becomes Posted.
- Reverse passes through `JournalPostingService.ReverseAsync`. The original is retained as Reversed and a separate balanced
  `ManualJournalReversal` is Posted in the selected open period.
- Posted/Reversed entries have no edit or delete endpoint. There is no browser-side posting calculation.

## Validation and security

- Server validation requires two or more lines, exactly one positive debit/credit per line, equal totals, two-decimal
  company-currency amounts, active tenant accounts and valid tenant dimensions.
- Every mutation is authenticated, permission-gated and antiforgery protected.
- Create, Post and Reverse run inside the existing Serializable UnitOfWork. Source/reference uniqueness and stable request
  keys prevent duplicate drafts and reversals.
- Four new permission definitions are seeded through the existing catalog; no schema migration is required.

## Verification

- .NET Web Release: zero warnings/errors.
- Security/accounting regression: 1,855 checks, including golden service coverage for permission boundaries, CSRF,
  idempotent draft creation, tenant account rejection, closed-period blocking, balanced posting and exact reversal.
- Angular: 238/238 tests across 113 files.
- Static accessibility: 134 templates.
- UI consistency: 117 feature pages, 106 shared headers, 11 documented exceptions, 75 destinations and 44/44 operational
  routes.
- Production budgets: 336,483/358,400 initial bytes and 81,880/81,920 largest lazy bytes.
- Hosted browser acceptance: 6 passed, zero failed for states, RTL/LTR, keyboard and 390px layout; anonymous API access
  returned 401 and two authenticated reconciliation checks were skipped without external storage state.

## Remaining risks

- No authenticated browser storage-state fixture is stored, so live mutation smoke must remain non-destructive until an
  authorized test session is supplied.
- Bulk journals, imports, recurring templates, approvals and edits/deletes of posted evidence remain deliberately absent.
