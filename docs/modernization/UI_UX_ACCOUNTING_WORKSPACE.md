# UX-010 — Accounting workspace foundation

> Completed: 2026-11-05

## Outcome

The Angular accounting workspace now presents the server-authored chart as an expandable hierarchy and provides a
permission-protected, searchable journal-entry directory with inclusive date/status filters, bounded paging and links to
the existing immutable journal evidence page.

## Architecture and controls

- `AccountService` builds the account tree from tenant-filtered repository data. Angular expands, collapses and searches
  that returned hierarchy but does not infer balances or posting rules.
- New accounts can select only an existing tenant account as parent. Because the new account does not exist before insert
  and no re-parent operation exists, the management surface cannot introduce a hierarchy cycle; defensive tree projection
  also terminates malformed historical cycles safely.
- `IJournalEntryRepository.GetPagedAsync` owns database filtering, ordering and paging. `JournalEntryQueryService` normalizes
  bounds and maps explicit list DTOs with line-derived debit/credit totals.
- `/api/v1/journal-entries` and `/accounting/journal-entries` retain `JournalEntries.View`; tenant query filters and composite
  relationships remain authoritative. No journal mutation was added.
- Search, dates, status, page and page size use the existing tenant/user-scoped URL/memory page-context boundary.

## Verification

- .NET Release build: zero warnings and zero errors.
- Security/inventory regression: 1,843/1,843 checks passed.
- Angular: 236/236 tests across 111 files.
- Static accessibility: 132 templates passed.
- UI inventory: 115 pages, 104 shared headers and 11 named exceptions.
- Navigation: 73 registered destinations and 42/42 covered operational routes.
- Production: 336,053/358,400 initial bytes and 81,880/81,920 largest lazy bytes.
- Hosted browser: 5/5 state, RTL/LTR, keyboard and 390px responsive checks passed.
- Live readiness and `/app/`: HTTP 200; anonymous journal-directory access returned 401.

No schema, migration, posting rule, fiscal-period rule or manual-journal capability changed.
