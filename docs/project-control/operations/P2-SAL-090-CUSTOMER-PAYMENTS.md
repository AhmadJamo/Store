# P2-SAL-090 — Customer payments

Completed: 2026-10-21

## Delivered

- Independent tenant-owned customer payment and allocation entities.
- Partial or multi-invoice allocation for the same customer/currency with outstanding caps.
- Derived Unpaid/Partially Paid/Paid state and disappearing completed action.
- Settlement/receivable/realized-FX journal with frozen account and exchange evidence.
- Controlled exact reversal that reopens balances; invoices with posted allocations cannot be reversed first.
- Bilingual responsive screens, dedicated permissions, actionable setup links and next-step guidance.
- Migration 126 applied to local Demo.

## Evidence

- Release Web build: zero warnings/errors.
- Security/inventory regression: 932 checks passed.
- Disposable SQL integration: partial receipt, balanced journal, reversal and reopened invoice passed.
- EF source/model/Demo: migration 126, no pending model changes.
