# Customer Invoice
> Status: SAL-070 implemented | Last reviewed: 2026-10-20

`CustomerInvoice` is the tenant-owned billing document created from one confirmed `SalesOrder`. It has independent `INV-` numbering, an idempotent request key, rowversion, Draft → Approved or Draft/Approved → Cancelled lifecycle and semantic history. Approval is a commercial review state, not accounting posting and not payment.

The header freezes customer/address, invoice and due dates, payment days, currency and exchange-rate evidence. Every line freezes the order's product, unit conversion, price source, tax source and proportional net/tax/total values. The last active partial invoice receives the rounding residual so active invoices reconcile to the order line.

Eligibility is company-configurable in Accounting Settings. `DeliveredQuantity` is the safe default for stocked products and requires exact allocations to Posted `SalesDeliveryLine` rows. `OrderedQuantity` permits invoicing up to the order quantity before delivery. Service products always use ordered quantity. Draft and Approved documents reserve eligibility; cancellation releases it. Cumulative active quantity cannot exceed the applicable source.

SAL-070 deliberately has no Posted state. It creates no stock transaction, movement, COGS, revenue, tax, receivable or payment. SAL-080 owns fiscal-period validation, customer account resolution, idempotent posting/reversal and receivable balance evidence.
