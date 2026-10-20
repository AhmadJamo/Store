# Sales order
> Status: SAL-040 IMPLEMENTED | Last reviewed: 2026-10-20

`SalesOrder` is a tenant-owned commercial commitment created once from an Accepted `SalesQuotation`. It stores its independent `SO-` number and a restrictive source-quotation link. Tenant-scoped unique indexes on the quotation and every source quotation line prevent duplicate conversion even under direct database writes.

The order copies the quotation's customer/address, currency/exchange-rate, payment-term, note and total snapshots. Every `SalesOrderLine` copies the source quotation-line product/unit conversion, quantity, price-list/rule evidence, transaction/company prices, tax evidence and totals. Confirmation never recalculates these values, so later master-data or pricing changes cannot rewrite the order.

The lifecycle is Draft → Confirmed, or Draft/Confirmed → Cancelled with a required reason. Every transition writes `SalesOrderHistory`, uses SQL rowversion and has a dedicated permission and antiforgery-protected endpoint. A converted or cancelled source cannot create another order; the quotation screen replaces the conversion button with the generated order link.

Creation, confirmation and cancellation have no inventory or accounting authority. They do not reserve or reduce stock, create movements, invoices, journals or payments. `SAL-050` owns inventory reservation and `SAL-060` owns delivery-time physical stock and cost effects.

