# Sales quotation
> Status: SAL-030 IMPLEMENTED | Last reviewed: 2026-10-20

`SalesQuotation` is a tenant-owned commercial proposal. It starts as Draft and may move to Sent, then Accepted, Rejected or Expired; Draft and Sent quotations may also be Cancelled. Every transition is state-checked, recorded in `SalesQuotationHistory` with the acting user and UTC time, and protected at the web boundary by a dedicated permission, antiforgery validation and SQL rowversion concurrency.

The header freezes the generated quotation number, idempotent request key, customer identity and tax snapshots, optional address snapshot, quotation and validity dates, currency, dated exchange rate/source, payment-term days, notes and totals. `SalesQuotationLine` freezes product and managed sales-unit identity, stock conversion factor and quantity, selected price list/rule and decision source, transaction/company prices, tax source and totals. Later customer, product, price-list, exchange-rate or tax changes cannot rewrite the quotation.

Creation evaluates every line through `SalesPriceListService`; all lines must resolve to the same transaction currency and exchange-rate evidence. Product selection supports name, code and barcode while IDs remain internal. The request key and quotation number are unique per tenant, and one product/unit combination can appear only once per quotation.

A quotation has no inventory or accounting authority. Sending, accepting, rejecting, expiring or cancelling it creates no reservation, stock transaction, journal entry, invoice or payment. Accepted quotations remain completed evidence; `SAL-040` will add the separately authorized Sales Order conversion and downstream document link.

