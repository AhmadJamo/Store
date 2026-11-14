# UX-017 bank-statement reconciliation readiness audit

> Completed: 2026-11-14  
> Scope: non-destructive source audit; no schema, runtime, permission or accounting mutation

## Outcome

MiniStore has a reusable ledger and settlement-account foundation, but it does not yet have a bank-statement subledger.
Bank reconciliation must not be modeled as a flag on `PaymentMethod` or `JournalEntry`: one statement line may match several
ledger lines, one ledger line may clear through several statement lines, and either side may remain partially unmatched.

## Existing authority that can be reused

- `PaymentMethod.AccountId` maps active cash/card methods to an asset settlement account.
- `CashFlowCloseService` already derives cash accounts from active payment methods and reads posted/reversed ledger movement.
- Journal entries have immutable posted lines, date, source type/reference, separate reversals and account dimensions.
- Customer and supplier payments freeze settlement/payment evidence and accept bounded external references.
- Fiscal periods, controlled manual journals, UnitOfWork, tenant filters, composite tenant foreign keys, rowversion patterns,
  antiforgery and semantic audit infrastructure already exist.
- Angular already has the accounting hub, persistent tabs, safe filters, journal drill-down and close-readiness workspace.

## Missing capabilities

- No explicit tenant-owned bank-account master with account name, masked identifier, currency and linked GL account.
- No statement/import batch, immutable statement line, source fingerprint or duplicate-file/line protection.
- No statement balance evidence, import format contract or bounded CSV parser.
- No many-to-many reconciliation allocation, partial match amount, cleared balance or stale-write protection.
- No permissions for statement view/import/match/unmatch and no API/Angular route.
- No close-readiness check for unreconciled bank lines.
- No controlled bank-fee, interest, transfer or correction workflow. Those must use journal services and open periods rather
  than directly changing ledger rows.

## Required accounting rules

1. A bank account belongs to one tenant, one active asset GL account and one currency.
2. Imported lines are immutable source evidence; corrections are replacement imports or explicit exclusions with reason.
3. File and line fingerprints are tenant/bank scoped and idempotent.
4. Reconciliation links only posted/reversed-effective journal lines on the same tenant, GL account and company-currency
   direction; Draft entries are never eligible.
5. Matching is allocation-based. Neither a statement line nor a journal line may be overallocated, and partial balances
   remain visible until exactly cleared within the currency precision.
6. Matching does not create, edit, post or reverse a journal entry.
7. Unmatch requires a reason, rowversion and audit evidence; it cannot erase the imported line.
8. A closed fiscal period blocks new accounting adjustments. Reconciliation evidence may be recorded only under an explicit
   policy decided before implementation; the first slice will not create adjustments.

## Delivery sequence

### UX-017B — statement evidence foundation

- Add tenant-owned bank-account, import-batch and immutable statement-line models.
- Support bounded CSV only; preserve normalized evidence and SHA-256 fingerprints, not the original unrestricted file.
- Add independent View and Import permissions, antiforgery, API DTOs and a bilingual Angular list/import/details workspace.
- Show server-authored candidate counts and journal links, but do not match, post, adjust or change close readiness yet.

### UX-017C — controlled partial matching

- Add allocation rows and Match/Unmatch permissions with rowversion, reasoned unmatch and semantic audit.
- Enforce account/currency/direction/remaining-amount invariants inside one UnitOfWork.
- Add exact/manual matching first; suggestions remain display-only and never auto-commit.

### UX-017D — close and adjustment integration

- Add unreconciled-line close-readiness evidence.
- Add separately approved fee/interest/transfer adjustment commands through controlled journals and fiscal-period checks.
- Do not infer reconciliation from external references alone.

## Risks

- Reusing `PaymentMethod` as a bank account would lose statement identity, currency and balance authority.
- A one-to-one matched flag would make partial deposits, card batches and bank fees incorrect.
- Fuzzy auto-match could silently clear the wrong transaction; only explicit allocations may become authority.
- Storing raw unrestricted uploads would add privacy and malware exposure; the first parser must be bounded and text-only.
- Foreign-currency reconciliation requires an explicit carrying/statement currency decision and is not hidden inside UX-017B.

## Verification decision

UX-017A authorizes no runtime change. UX-017B is the smallest safe implementation slice because it creates immutable source
evidence and read-only candidate visibility without changing journals or claiming reconciliation.
