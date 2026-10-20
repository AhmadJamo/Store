# P2-SAL-060 — Sales deliveries

> Status: COMPLETED 2026-10-20

## Delivered outcome

- Added tenant-owned `SalesDelivery`, line and history records with Draft/Posted/Cancelled lifecycle, rowversion and `DEL-` numbering.
- Added permission-separated bilingual list, create and detail pages without horizontal scrolling.
- Draft creation selects one active Sales Order reservation and supports partial/full remaining quantities.
- Posting issues exact reserved warehouse/location stock, including lot/serial evidence, and decreases OnHand and Reserved inside one Serializable transaction.
- Each line freezes moving-average unit cost and COGS; posting writes Sale stock transaction, SalesDeliveryOut physical movement and debit COGS / credit Inventory once.
- Partial posting preserves the reservation remainder; final posting closes it. Posted delivery/order cancellation and duplicate posting are blocked.
- Added permission-aware setup/recovery links, next-step guidance and completed-action replacement.
- Customer invoice, receivable, revenue and payment remain outside this slice.

## Schema and Demo

- Migration: `20261020114141_AddSalesDeliveries` (123).
- Adds `ConsumedQuantity` to reservation lines and creates the three delivery tables with tenant-composite relationships and uniqueness guards.
- Applied successfully to local `MiniStoreDb`; source, model and Demo are aligned at 123.

## Verification evidence

- Release build: passed, zero warnings and zero errors.
- SecurityRegression: 884 checks passed.
- SQL integration: passed on a disposable database after rebuilding the test project. It posted 0.4 then 0.6 units, proved Active then Consumed reservation state, zero final ProductStock/OnHand/Reserved/Available, two source delivery movements/transactions/journals, tenant isolation and blocked order cancellation after delivery.
- EF model check: no pending model changes.

## Remaining boundary

Posted delivery reversal is intentionally owned by a controlled sales-return workflow. The only next proposed task is SAL-070 Customer Invoicing and requires explicit approval.
