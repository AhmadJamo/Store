# UX-013 — AR/AP aging and subledger reconciliation

> Completed: 2026-11-06

## Outcome

/accounting/aging is a bilingual, RTL-safe, read-only workspace for receivables and payables aging as of a selected
date. Angular sends only the allowlisted date, search and view context. AgingReportService owns all eligibility,
company-currency carrying values, partial-payment and credit-note application, reversal timing, bucket assignment and
control-account reconciliation.

## Accounting definitions

- Buckets use document due date: current, 1–30, 31–60, 61–90 and more than 90 days.
- Customer invoices use their frozen DueDate and ReceivableAccountIdSnapshot.
- New vendor bills freeze PaymentTermDaysSnapshot and DueDate. Existing historical bills are not rewritten; a null
  historical due date falls back visibly and deterministically to BillDate.
- Only posted documents effective on or before the as-of date participate. A currently reversed invoice, bill or payment
  remains effective before its dated reversal and disappears on/after the reversal date.
- Customer payments use ReceivableCarryingAmount; supplier payments use PayableCarryingAmount; credit notes use their
  frozen company-currency gross values. No current exchange rate is applied.
- Reconciliation groups the complete, unfiltered subledger by control account and compares it to non-draft journal lines
  through the same as-of date. Search filters display rows and bucket cards only; it never changes reconciliation totals.
- Missing control-account assignment is shown as an explicit unreconciled row instead of disappearing.

## Navigation and safety

The page reuses JournalEntries.View, remains GET-only and links to source invoices, payments, credit notes and filtered
journal evidence. Tenant query filters remain authoritative in every repository. No collection, payment, posting, edit or
revaluation command was added.

## Verification

- Web Release rebuild: zero warnings and errors.
- Security/accounting regression: 1,864 checks.
- Angular: 239 tests across 114 files.
- Static accessibility: 135 templates.
- UI consistency: 118 feature pages, 107 shared headers, 11 documented exceptions, 76 destinations and 45 operational routes.
- Production bundle: 336,904/358,400 initial bytes and 81,877/81,920 largest lazy bytes.
- Hosted browser presentation checks: 6 passed, 0 failed, 2 skipped without external authenticated storage state.
- Live readiness and /app/accounting/aging: HTTP 200; anonymous API: HTTP 401.

## Residual risk

Historical vendor bills predate the due-date snapshot. They intentionally use bill date rather than silently deriving a
new historical date from mutable supplier terms. A separate owner-approved data-cleanup process may backfill verified
historical due dates if source evidence becomes available.
