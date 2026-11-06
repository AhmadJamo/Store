# UX-015 — Controlled period-close execution

> Completed: 2026-11-07

## Outcome

Fiscal-period status is now an explicit controlled workflow. The only forward path is Open → SoftClosed → Closed and
reopen moves one step backward. Every transition requires a reason and current rowversion. The period entity retains actor,
UTC time and reason, while the tamper-evident audit stream records the before/after status.

## Server authority

- `FiscalPeriod` rejects direct jumps and repeated states.
- `FiscalPeriodService` reruns the UX-014 report inside the UnitOfWork before soft/final close.
- Any `Blocked` readiness control rejects the operation before mutation. The report's fiscal-status row is excluded because
  it describes the current target state rather than an unresolved accounting exception.
- Closing before the period end is rejected. Reopen does not bypass posting rules: only an Open period permits posting.
- No automatic closing journal, posted-entry change or source-document mutation exists in this phase.

## Security and UI

`FiscalPeriods.View`, `FiscalPeriods.SoftClose`, `FiscalPeriods.Close` and `FiscalPeriods.Reopen` are independent. The three
commands use explicit antiforgery-protected POST endpoints. Angular shows only actions valid for both current status and
session permission, requires a reason, prevents repeat actions after refresh and links the exact dates to readiness evidence.
Arabic/English copy is feature-lazy to retain the fixed global localization budget.

## Verification

- Web Release rebuild: passed, zero warnings/errors.
- Security/accounting regression: 1,872 checks passed, including stepwise transitions, blocker preservation, permission,
  antiforgery and semantic-audit coverage.
- Angular: 240/240 tests across 115 files.
- Static accessibility: 136 templates passed.
- Bundles: 337,026/358,400 initial bytes and 81,877/81,920 largest lazy bytes.
- Authenticated Microsoft Edge acceptance: 15 passed, 0 failed, 0 skipped, including RTL/LTR, keyboard and 390px viewport.

## Remaining decisions and risks

- Year-end revenue/expense closing entries require a separate approved accounting policy; the system does not infer one.
- A warning-only unclassified cash movement remains visible for controller review; authoritative `Blocked` checks stop close.
- Reopening is permissioned and audited but may allow new posting once the period reaches Open; operational policy must
  restrict the permission to an appropriate controller role.
