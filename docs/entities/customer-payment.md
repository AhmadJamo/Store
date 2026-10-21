# Customer Payment

> Status: IMPLEMENTED (SAL-090)  
> Last reviewed: 2026-10-21

`CustomerPayment` is an independent tenant-owned receipt document. It is not a flag on `CustomerInvoice` and it never changes inventory.

## Lifecycle

- Draft exists only inside the atomic creation transaction.
- Posted allocations reduce invoice outstanding balances.
- Reversed preserves the receipt and allocation rows, posts the exact inverse journal and removes those allocations from derived balances.

## Frozen evidence

The header stores payment number/date, customer, payment method, settlement-account snapshot, transaction/company currencies, dated exchange rate/source, external reference, actors/timestamps and rowversion. Each line stores invoice identity/number, transaction amount, receipt company amount, frozen receivable account and receivable carrying amount.

## Invariants

- Lines must reference Posted, non-reversed invoices for one customer and currency.
- One invoice may appear once per payment; allocation cannot exceed the current outstanding amount.
- Only Posted allocations count toward Unpaid/Partially Paid/Paid.
- The journal debits settlement, credits receivable carrying amount and records realized exchange gain/loss when required.
- An allocated invoice cannot be reversed until its posted payments are reversed.
