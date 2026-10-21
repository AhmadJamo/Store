# Customer Invoice
> Status: SAL-080 implemented | Last reviewed: 2026-10-21

`CustomerInvoice` is the tenant-owned billing document created from one confirmed `SalesOrder`. It has independent `INV-` numbering, an idempotent request key, rowversion and semantic Draft → Approved → Posted → Reversed history; Draft/Approved may instead become Cancelled. Approval is commercial review, while only Posted creates the receivable/revenue/output-tax journal. Posting never records payment.

The header freezes customer/address, invoice and due dates, payment days, currency and exchange-rate evidence. Every line freezes the order's product, unit conversion, price source, tax source and proportional net/tax/total values. The last active partial invoice receives the rounding residual so active invoices reconcile to the order line.

Eligibility is company-configurable in Accounting Settings. `DeliveredQuantity` is the safe default for stocked products and requires exact allocations to Posted `SalesDeliveryLine` rows. `OrderedQuantity` permits invoicing up to the order quantity before delivery. Service products always use ordered quantity. Draft, Approved and Posted documents reserve eligibility; cancellation or controlled reversal releases it. Cumulative active quantity cannot exceed the applicable source.

SAL-080 freezes company-currency line amounts, customer receivable, default revenue and output-tax account evidence. The posting service resolves branch revenue overrides and distributes delivered lines across their exact source warehouse/branch dimensions with residual-safe rounding. One source-unique journal debits receivables and credits revenue/output tax. The journal number and its actual account/dimension lines are visible through a permission-protected evidence screen.

Controlled reversal requires a date not earlier than the invoice and a reason. It creates one exact inverse journal through `JournalPostingService`, retains both journal records, marks the invoice Reversed and makes the source quantity eligible again. Posting/reversal never changes delivery, stock, stock movement or COGS.

SAL-090 derives Paid and Outstanding amounts plus Unpaid/Partially Paid/Paid from Posted `CustomerPaymentLine` allocations. The invoice exposes collection only while an amount remains, replaces the action with completion state at zero, and cannot reverse while any posted payment allocation remains.
