# P2-SAL-050E / SAL-050 — Sales-order inventory reservations

Date: 2026-10-20  
Status: COMPLETED

## Delivered

- Activated `InventoryReservationSourceType.SaleOrder` for confirmed, non-cancelled SalesOrders.
- Added full and partial allocation from a named warehouse and optional active reservable location or Unassigned position.
- Added explicit block-entire-request and reserve-available shortage policies without implicit negative stock.
- Added exact SalesOrderLine source evidence and cumulative active-reservation validation.
- Added multiple immutable allocation records per order, reasoned release, reallocation history and automatic release when the order is cancelled.
- Added separate Reserve and ReleaseReservation permissions, antiforgery-protected endpoints, Arabic localization, actionable availability/location links and permission-aware next-step guidance.
- Added responsive cards for line progress, dimensional OnHand/Reserved/Available and reservation history; completed quantities no longer show a repeat action.

## Database

Migration `20261020105011_AddSalesOrderReservations` is migration 122. It adds nullable SourceLineId, preserves tenant-unique StockTransfer source identity, permits multiple SaleOrder records and adds separate dimension uniqueness filters for generic versus source-line allocations. Migrations 120–122 were applied to local Demo; code and Demo both report 122, and EF reports no pending model changes.

## Verification

- Release solution build: passed with zero warnings and zero errors.
- Security/inventory regression: 862 checks passed.
- Disposable SQL integration: passed. It proves partial reservation, source-line evidence, manual release/reallocation, automatic release on order cancellation, tenant isolation, unchanged OnHand/StockTransaction/JournalEntry counts and exactly one winner for the final available unit.
- Existing transfer reservation uniqueness and line duplicate protection remain database-enforced.

## Boundary

Reservation changes only Reserved/Available. It does not post delivery, reduce OnHand, select tracked identities for shipment, create COGS, invoice receivables or record payment. Those responsibilities begin with the separately approved SAL-060 delivery slice.

