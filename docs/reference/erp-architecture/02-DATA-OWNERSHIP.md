# 02 — Data Ownership / ملكية البيانات والعلاقات

## Master-data relationship map
```mermaid
erDiagram
 COMPANY ||--o{ BRANCH : has
 COMPANY ||--o{ WAREHOUSE : owns
 WAREHOUSE ||--o{ LOCATION : contains
 PARTY ||--o{ PARTY_ADDRESS : has
 PARTY ||--o{ PARTY_ROLE : acts_as
 PRODUCT ||--o{ PRODUCT_VARIANT : has
 PRODUCT_VARIANT ||--o{ PRODUCT_UOM_CONVERSION : converts
 PRODUCT_VARIANT ||--o{ PRICE_LIST_LINE : priced_by
 COMPANY ||--o{ DOCUMENT_SEQUENCE : configures
 COMPANY ||--o{ ACCOUNT : has
 ACCOUNT ||--o{ JOURNAL_LINE : posted_to
 JOURNAL_ENTRY ||--|{ JOURNAL_LINE : contains
 PRODUCT_VARIANT ||--o{ STOCK_MOVE_LINE : tracked_in
 LOCATION ||--o{ STOCK_MOVE_LINE : source_or_target
 LOT ||--o{ STOCK_MOVE_LINE : optionally_tracks
}
```

## Transaction relationship map
```mermaid
erDiagram
 SALES_ORDER ||--|{ SALES_ORDER_LINE : contains
 SALES_ORDER ||--o{ SHIPMENT : fulfills
 SHIPMENT ||--|{ SHIPMENT_LINE : contains
 SHIPMENT_LINE ||--o{ STOCK_MOVE_LINE : produces
 SALES_ORDER_LINE ||--o{ INVOICE_LINE_SOURCE : invoiced_from
 INVOICE ||--|{ INVOICE_LINE : contains
 INVOICE_LINE ||--o{ INVOICE_LINE_SOURCE : maps_to
 PURCHASE_ORDER ||--|{ PURCHASE_ORDER_LINE : contains
 PURCHASE_ORDER_LINE ||--o{ GOODS_RECEIPT_LINE : received_by
 GOODS_RECEIPT ||--|{ GOODS_RECEIPT_LINE : contains
 GOODS_RECEIPT_LINE ||--o{ STOCK_MOVE_LINE : produces
 PURCHASE_ORDER_LINE ||--o{ BILL_LINE_SOURCE : billed_from
 VENDOR_BILL ||--|{ VENDOR_BILL_LINE : contains
 VENDOR_BILL_LINE ||--o{ BILL_LINE_SOURCE : maps_to
 INVOICE ||--o{ PAYMENT_ALLOCATION : settled_by
 PAYMENT ||--o{ PAYMENT_ALLOCATION : applies
 JOURNAL_ENTRY ||--|{ JOURNAL_LINE : contains
}
```

## Entity ownership and authoritative source
| Data | Owner | Authoritative measure | External references |
|---|---|---|---|
| Party / customer / vendor | Parties | canonical party ID, roles, contacts | PartyId |
| Product / variant / unit | Catalog | SKU, base UOM, attributes | ProductVariantId |
| Warehouse / location | Inventory | storage hierarchy | LocationId |
| Stock on hand | Inventory | posted stock moves; optimized balance projection | ProductVariantId, LocationId, LotId |
| Stock reserved | Inventory | active reservations | SalesOrderLineId / DemandRef |
| Available-to-promise | Inventory | on-hand minus reserved + policy-based incoming | read contract |
| Sales quote/order | Sales | quantities, agreed prices, status | PartyId, ProductVariantId |
| Purchase RFQ/order | Procurement | ordered/received/billed quantities | PartyId, ProductVariantId |
| Invoice and credit note | Billing | legal invoice amounts and settlement status | Source document IDs |
| Journal entry and balance | Accounting | posted journal lines | SourceDocumentRef |
| Payments | Treasury | executed/received payments and allocations | InvoiceId/BillId |
| Inventory valuation | Inventory Costing | cost layers and valuation movements | StockMoveId |
| Taxes | Tax | rule/version and applied tax snapshot | TaxRuleId |
| Number series | Platform | scope, prefix, next allocation policy | DocumentType, CompanyId |
| User permission | Identity | effective access decision | UserId, RoleId |
| Audit trail | Platform | append-only actor/action/change record | EntityRef, CorrelationId |

## Data modeling rules
1. Use `decimal(18,6)` or an explicitly reviewed precision for quantities and rates; use `decimal(19,4)` or currency-aware precision for monetary amounts. These are **suggested defaults**, not universal rules.
2. `RowVersion` concurrency token on mutable aggregates; use optimistic concurrency with a user-friendly retry/conflict response.
3. `CompanyId` required on operational transactions. Unique indexes scoped by company and document type as appropriate.
4. Store document snapshots: item description, UOM, unit price, tax rate/code, exchange rate, counterparty legal identity and address at confirmation/posting; master-data changes must not rewrite history.
5. `SourceDocumentType + SourceDocumentId + SourceLineId` provenance for every generated document/line. Prefer typed source link table if multiple sources can contribute to one line.
6. No `ON DELETE CASCADE` across posted financial/inventory records; use restricted deletes, soft archive for masters and controlled retention.
7. `QuantityOrdered`, `QuantityReceived`, `QuantityBilled`, `QuantityReturned` are distinct; never infer from one another without state filters.
8. UOM conversions explicit and versioned; fractional conversion/rounding tested. Duplicate product lines are allowed unless an actual domain constraint forbids them.
9. A product may appear multiple times in BOM/recipe with different role, operation, lot or UOM; do not enforce uniqueness on ProductId alone.
10. Preserve original IDs and document numbers during migrations; add mapping tables for legacy identifiers.

## Suggested indexes / constraints
- `(CompanyId, DocumentType, DocumentNumber)` unique for finalized numbers.
- `(CompanyId, ExternalSystem, ExternalId)` unique when external ID is present.
- `(CompanyId, ProductVariantId, LocationId, LotId, OwnerId)` inventory projection key (nullable dimensions normalized consistently).
- `(ConsumerName, MessageId)` unique inbox deduplication.
- `(SourceDocumentType, SourceDocumentId, PostingKind, Revision)` unique posting key.
- `(CompanyId, FiscalYear, AccountCode)` unique chart-of-accounts code as policy allows.
- Foreign keys inside bounded context; cross-context validation by contract and consistency jobs.

## Data lifecycle
Draft -> validated -> approved (if required) -> posted/fulfilled -> reversed/closed -> retained/archived. Delete only unreferenced drafts under authorization; posted records retained per jurisdictional retention policy.
