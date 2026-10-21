# P2-SAL-080 — Customer Invoice Accounting
> Status: COMPLETE | Completed: 2026-10-21

## Delivered

- Approved → Posted → Reversed lifecycle with actor/time/date/reason history and completed-action replacement.
- One source-unique company-currency journal: debit customer receivable; credit branch/default revenue and output tax.
- Frozen receivable/default-revenue/output-tax accounts and company net/tax/total evidence.
- Exact source delivery warehouse/branch dimensions with residual-safe allocation and branch revenue override.
- Controlled exact inverse journal, released invoice eligibility and no delivery, inventory, movement, COGS or payment mutation.
- Six invoice permissions plus dedicated read-only journal-evidence permission and responsive bilingual screens.
- Permission-aware setup links for customer, tax, warehouse, branch, accounting and fiscal-period prerequisites; next-step guidance preserves the payment boundary.

## Verification

- Release Web build: zero warnings/errors.
- SecurityRegression: 919 checks passed.
- Disposable SQL integration: partial invoice foundation, balanced post, frozen accounts/company amounts, branch/warehouse journal dimensions, exact linked reversal, eligibility release, tenant isolation and unchanged stock/movements all passed.
- Migration `20261020205959_AddCustomerInvoiceAccountingPosting` (125) applied to local Demo.
- EF reports no pending model changes.

## Boundary

No customer payment, allocation, credit note, refund or credit-limit workflow was added. These remain gated by SAL-090+.
