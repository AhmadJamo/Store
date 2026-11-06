# UX-014 — Cash Flow and Period-Close Readiness

> Completed: 2026-11-06

## Outcome

`/accounting/cash-flow-close` is a bilingual, responsive, GET-only accounting workspace. It derives cash evidence from
posted/reversed journal history, keeps all financial calculations on the server and preserves only allowlisted date/view
context. It does not close periods, create journals, collect money or mutate source documents.

## Financial boundary

- Active `PaymentMethod.AccountId` values define cash and cash equivalents; duplicates are collapsed.
- Opening cash is the debit-minus-credit balance through the day before the selected range.
- Net movement is the sum of every cash-account line during the range; closing equals opening plus net movement.
- Commercial source prefixes and revenue/expense counterparts are Operating; asset-only counterparts are Investing;
  liability/equity-only counterparts are Financing; mixed/unknown evidence is Unclassified.
- Unclassified movement remains in net movement and is a visible readiness warning, so the report always reconciles.

## Close-readiness boundary

The checklist reads existing authoritative services/repositories for cash configuration, unclassified cash, trial-balance
and balance-sheet differences, AR/AP control-account differences, inventory reconciliation exceptions, Draft journals and
fiscal-period coverage. Each result links to the owning evidence surface. It is advisory and cannot change period status.

## Verification

- Web Release rebuild: 0 warnings, 0 errors.
- Security/accounting regression: 1,867 passed.
- Angular: 240/240 across 115 files; production build passed.
- Static accessibility: 136 templates; UI consistency: 119 pages, 108 shared headers, 11 documented exceptions,
  77 destinations and 46 operational routes.
- Bundle budgets: 337,026/358,400 initial and 81,877/81,920 largest lazy.
- Authenticated Microsoft Edge acceptance: 15 passed, 0 failed, 0 skipped; accounting cohort includes the new endpoint.

## Risks and next gate

Classification is intentionally conservative; mixed counterpart evidence requires review instead of silent guessing.
Settlement-account configuration quality remains operationally important. UX-015 must receive explicit approval because
closing/reopening a fiscal period or creating closing journals is a materially different mutation boundary.
