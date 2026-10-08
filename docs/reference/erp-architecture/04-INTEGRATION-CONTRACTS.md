# 04 — Integration Contracts / عقود التكامل

## Commands vs events
- Command: `ReserveStock`, `PostGoodsReceipt`, `IssueInvoice`, `PostJournalEntry`, `AllocatePayment`. Owner validates permission, company, state and concurrency.
- Event: `SalesOrderConfirmed`, `StockReservationCreated`, `StockMovePosted`, `GoodsReceived`, `InvoicePosted`, `PaymentAllocated`, `JournalEntryPosted`.
- No consumer assumes exactly-once delivery. Delivery may be **at least once**, out of order, delayed or repeated.

## Required envelope
```json
{
  "messageId":"uuid", "eventType":"InvoicePosted", "schemaVersion":1,
  "occurredAtUtc":"ISO-8601", "companyId":"uuid",
  "aggregateId":"uuid", "aggregateVersion":7,
  "correlationId":"uuid", "causationId":"uuid",
  "actorId":"uuid-or-service", "payload":{}
}
```

## Contract matrix
| Producer | Message | Consumer | Consistency | Dedup key |
|---|---|---|---|---|
| Sales | SalesOrderConfirmed | Fulfillment/Inventory | async command orchestration | OrderId + revision + action |
| Inventory | StockMovePosted | Costing, Sales delivery projection, BI | async | MoveId + posting version |
| Procurement | PurchaseOrderApproved | Inventory receiving planning | async | POId + revision |
| Inventory | GoodsReceived | Procurement receipt projection, AP matching | async | ReceiptId + version |
| Billing | InvoicePosted | Accounting | reliable local posting orchestration | InvoiceId + posting type |
| Costing | InventoryValuationPosted | Accounting | reliable local posting orchestration | ValuationId |
| Treasury | PaymentCaptured/Executed | AR/AP, Accounting | async / reliable orchestration | PaymentId + action |
| HR | PayrollApproved | Accounting | reliable orchestration | PayrollRunId + posting type |
| POS | POSSessionClosed | Treasury/Accounting | async | SessionId + closure revision |
| Parties | PartyChanged | CRM, Sales read models, BI | async | PartyId + version |

## Transactional outbox + inbox
1. In one DB transaction, save aggregate changes **and** outbox message.
2. Dispatcher publishes pending messages, retries with exponential backoff and dead-letter visibility.
3. Consumer inserts inbox dedup record and performs its owned update atomically when feasible.
4. Preserve event ordering per aggregate where required; detect version gaps and defer/rebuild.
5. Correlate every generated document to its source; add reconciliation job for orphan/incomplete postings.

## Contract compatibility
- Version schemas; additive fields preferred. Breaking changes require new event version, dual-read period, migration and consumer readiness.
- Do not publish EF entities, DbContext, internal navigation properties or mutable ORM graphs as integration contracts.
- DTOs expose stable IDs, snapshots and explicit units/currency.
- All money messages include amount, currency, exchange rate/date and rounding rule; quantity messages include UOM.
- Validate all inbound external data, signatures, replay protection, company mapping and per-source permissions.

## API behavior
- POST side-effecting commands accept `Idempotency-Key`; persist response/result for repeat requests.
- Conflict (`409`) for invalid state or row-version collision; validation (`400/422`) for invalid fields; forbidden (`403`) for denied authorization.
- Cursor/page all large lists; use query-specific read models rather than loading aggregate graphs.
- Audit security-sensitive actions and background service identity.
