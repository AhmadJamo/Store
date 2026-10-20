# Sales order
> Status: SAL-040 THROUGH SAL-060 IMPLEMENTED | Last reviewed: 2026-10-20

`SalesOrder` is a tenant-owned commercial commitment created once from an Accepted `SalesQuotation`. It stores its independent `SO-` number and a restrictive source-quotation link. Tenant-scoped unique indexes on the quotation and every source quotation line prevent duplicate conversion even under direct database writes.

The order copies the quotation's customer/address, currency/exchange-rate, payment-term, note and total snapshots. Every `SalesOrderLine` copies the source quotation-line product/unit conversion, quantity, price-list/rule evidence, transaction/company prices, tax evidence and totals. Confirmation never recalculates these values, so later master-data or pricing changes cannot rewrite the order.

The lifecycle is Draft → Confirmed, or Draft/Confirmed → Cancelled with a required reason. Every transition writes `SalesOrderHistory`, uses SQL rowversion and has a dedicated permission and antiforgery-protected endpoint. A converted or cancelled source cannot create another order; the quotation screen replaces the conversion button with the generated order link.

Creation and confirmation have no inventory or accounting mutation authority. After confirmation, `SalesOrderReservationService` may create multiple full or partial `InventoryReservation` records linked to exact order lines and warehouse/location dimensions. Active totals are derived from reservation rows, never copied into mutable order fields. Releasing a record restores Available, and cancelling the order releases every active record inside the cancellation transaction.

Reservation changes `Reserved` and therefore Available only; it never changes OnHand, creates a stock movement or posts a journal. A block-entire-operation policy and an explicit reserve-available policy are supported without implicit negative stock.

SAL-060 derives delivered/remaining progress from posted `SalesDelivery` lines. Each Draft uses one active reservation and warehouse. Posting consumes the exact reserved location quantity, records tracking/physical/cost evidence and may leave a partial reservation remainder. Once any delivery is posted the order cannot be cancelled; after full delivery the prepare-delivery action is replaced by completed state and document links. Delivery does not create a customer invoice, receivable or payment.

