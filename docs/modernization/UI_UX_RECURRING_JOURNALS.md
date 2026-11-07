# UX-016 — Recurring Journal Templates and Controlled Runs

> Completed: 2026-11-07

## Outcome

The accounting workspace now supports tenant-owned balanced recurring-journal templates at
`/accounting/recurring-journals`. A permitted operator can create or update a named template, pause or reactivate it,
and explicitly generate the next dated journal Draft. A run never posts automatically and never edits an earlier Draft.

## Control boundary

- `RecurringJournals.View`, `RecurringJournals.Manage` and `RecurringJournals.Run` are independent permissions.
- All mutations are authenticated, antiforgery-protected and use rowversion for template changes and runs.
- A stable run key makes a retry return the already-created Draft instead of producing another entry or advancing twice.
- `ManualJournalService` owns account/dimension/amount validation and Draft creation inside the caller transaction.
- Generated entries use `RecurringJournal` source identity and remain ordinary Draft evidence. Posting still requires the
  existing manual-journal posting permission and `JournalPostingService`; the fiscal period is checked only at posting.
- Templates and lines use the shared tenant query/write boundary and tenant-composite relationships.
- Create/update/state and run operations emit allowlisted semantic audit events.

## Workspace behavior

- The page shows status, frequency, next run, last run and the generated journal link.
- Account, branch and warehouse choices use names; identifiers remain internal.
- Create-form content is recoverable only in the current page-context memory. Update concurrency always comes from the
  server rowversion.
- English and Arabic copy, RTL-safe logical layout, persistent ERP tabs and unsaved-change protection are retained.

## Verification

- Release rebuild: 0 warnings, 0 errors.
- Security/accounting regression: 1,883 passed.
- Angular: 242/242 tests across 116 files.
- Production bundle: 337,191 / 358,400 initial bytes; 81,877 / 81,920 largest lazy bytes.
- Static accessibility: 137 templates passed.
- UI/UX inventory: 120 feature pages, 109 shared headers, 78 registered destinations, 47/47 operational routes.
- Authenticated Microsoft Edge acceptance: 15 passed, 0 failed, 0 skipped.

## Explicit exclusions and risks

No background scheduler, automatic posting, year-end closing entry, automated accrual, budget, consolidation, bank
integration or posted-entry mutation was added. An authorized human remains responsible for each run and review. A user
needs journal-read access to open the generated Draft through the general journal evidence screen.
