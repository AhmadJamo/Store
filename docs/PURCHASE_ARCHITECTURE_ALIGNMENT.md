# Purchase architecture alignment gate
> Slice: PUR-005  
> Status: ACCEPTED — DOCUMENTATION-ONLY FOUNDATION  
> Source of truth: current code, accepted ADRs and migrations  
> Last reviewed: 2026-10-08

## 1. Purpose

PUR-005 fixes the ownership and integration boundaries that later procure-to-pay slices must follow. It prevents Purchase, Inventory and Accounting from becoming competing sources of truth while preserving MiniStore's implemented Supplier, inventory and journal foundations.

This document is a target contract. It does not assert that Purchase Request, Purchase Order, Goods Receipt, Vendor Bill, approvals, multi-currency or an outbox are currently implemented.

## 2. Decisions fixed by this gate

### 2.1 Supplier remains the purchasing counterparty

The existing `Supplier` entity remains authoritative for purchasing during the current roadmap. PUR-010 may reference `SupplierId`; it must not introduce a duplicate Vendor or Party table.

The imported ERP reference proposes a future Party model that could allow one legal party to be both customer and supplier. That is not part of PUR-010 and requires its own repository-wide ADR and migration. New purchase documents must use stable supplier references and frozen supplier legal/contact snapshots where historical evidence requires them, so a later Party migration remains possible.

### 2.2 Context ownership

| Concern/document | Owner | Other modules may do |
|---|---|---|
| Supplier master and supplier-product terms | Suppliers/Purchasing | Read through Application contracts |
| Purchase Request, sourcing event, quotation and Purchase Order | Purchasing | Inventory/Accounting may read stable references/projections |
| Warehouse, location, tracking, physical receipt effect and stock ledgers | Inventory | Purchasing requests receipt posting; it never writes balances directly |
| Goods Receipt commercial orchestration | Purchasing Application workflow | Inventory validates/posts physical effects and returns immutable result references |
| Vendor Bill, payable, payment allocation and settlement | Accounting/AP | Purchasing supplies PO/receipt evidence for matching |
| General ledger entries and fiscal periods | Accounting | Other workflows request posting through the central journal gateway |
| Document numbers | Settings/Platform numbering | Owning workflow allocates through `DocumentNumberService` |
| Permissions and actors | Security/Tenancy | Every command enforces tenant membership and explicit permission |

Goods Receipt is intentionally a boundary document: Purchasing owns the PO relationship and receipt lifecycle orchestration; Inventory owns every physical quantity, identity, movement and valuation mutation. The Application layer coordinates both inside the established UnitOfWork where synchronous atomicity is required.

### 2.3 No direct cross-module persistence

- Purchase controllers call Purchase Application services only.
- Purchase domain entities do not mutate `ProductStock`, `InventoryBalance`, tracking balances or journals.
- Purchase Application services call published Inventory and Accounting application contracts.
- Controllers and Razor views never access EF Core directly.
- Repository interfaces remain feature-scoped and do not become a generic data-access escape hatch.

### 2.4 Quantity authorities

The following measures have distinct authorities:

| Measure | Authority |
|---|---|
| Ordered | confirmed Purchase Order lines |
| Received | posted, non-reversed Goods Receipt line allocations |
| Returned | posted receipt-based supplier-return allocations |
| Billed | posted/approved Vendor Bill allocations according to billing policy |
| Paid | posted supplier payment allocations |
| Remaining to receive/bill/pay | query projection derived from the authoritative posted records |

Cached totals may be added only as transactionally maintained projections with reconciliation tests. They are never unrestricted user-editable fields.

### 2.5 Currency boundary

PUR-010 does not build a full exchange-rate engine. It introduces only a forward-compatible commercial boundary:

- currency is stored as an uppercase, language-neutral `CurrencyCode`;
- the tenant/company base currency comes from the existing general settings until a dedicated Currency module is accepted;
- supplier prices may initially use base currency only, or carry a non-base code as non-posting commercial information when the slice explicitly supports it;
- every confirmed/posted monetary document later freezes transaction currency, base currency, exchange rate, rate date/source and rounding result;
- no inventory or journal posting in non-base currency is enabled until the accounting currency policy, gain/loss accounts and tests exist;
- money and rates use reviewed decimal precision, never binary floating point.

This boundary avoids pretending that a currency code alone is multi-currency accounting.

### 2.6 Approval boundary

Approval is designed as a reusable capability but implemented purchase-first in PUR-025. It must not become a god service or be coupled to MVC.

- `PurchaseApprovalRule` owns purchase-specific rule inputs initially.
- An approval instance freezes the selected rule/version and document amount/currency basis.
- Approval steps freeze sequence, assigned role/user, decision, actor, timestamp and reason.
- The owning purchase document controls which commands are legal after approval.
- The general AuditLog supplements but does not replace explicit approval history.
- Self-approval and separation-of-duty policies are configuration decisions enforced server-side.
- Generalization to sales, payments or accounting close requires a later ADR based on proven common behavior.

### 2.7 Accounting and GRNI boundary

The accepted target uses perpetual inventory accounting with GRNI for the new flow:

```text
Posted receipt
  Dr Inventory
  Cr Goods Received Not Invoiced (GRNI)

Posted vendor bill
  Dr GRNI
  Dr Recoverable Input Tax
  Dr/Cr Purchase Price Variance
  Cr Accounts Payable

Posted supplier payment
  Dr Accounts Payable
  Cr Cash/Bank
```

PUR-060 may post physical inventory only after its atomic and idempotent receipt contract is proven. PUR-065 enables GRNI journal posting only after account mappings, fiscal-period behavior, reversal policy and reconciliation tests are implemented. PUR-080 owns Vendor Bill/AP posting. PO confirmation itself has no stock or GL entry.

Legacy Direct Purchase accounting remains unchanged until a separate controlled cutover; legacy and new flows must have distinct source types and idempotency keys.

### 2.8 Integration style

MiniStore remains a modular monolith. Current in-process Application contracts and one SQL UnitOfWork are preferred where the operation must be atomic. The design reserves stable business facts:

- `PurchaseOrderConfirmed`
- `GoodsReceiptPosted`
- `VendorBillPosted`
- `SupplierPaymentPosted`

These names describe future integration facts, not implemented messages. No event bus or outbox is added in PUR-005/PUR-010. When asynchronous consumers become necessary, a dedicated slice must add a transactional outbox, schema versioning, correlation/causation identifiers, retries, deduplication and operational visibility.

## 3. State and immutability rules

- Draft documents may be edited under rowversion and permission checks.
- Submitted/approved documents change only through explicit commands and action history.
- Confirmed PO commercial snapshots cannot be silently rewritten; amendments require a controlled revision policy.
- Posted receipt, bill, payment and journal effects are immutable.
- Corrections use cancellation before posting or linked reversal/return/credit documents after posting.
- A retry must return the original effective result or a safe conflict; it must not post twice.
- Document number is a business key, never the primary key.

## 4. Scope and tenant rules

- Every new operational row is tenant scoped through the existing shared-database isolation model.
- Foreign keys between tenant-owned records include/enforce the tenant boundary according to existing conventions.
- Warehouse/branch access is checked on commands and filtered queries; client-provided IDs are not trusted.
- Unique business keys are tenant scoped; supplier invoice uniqueness additionally includes Supplier and normalized external invoice number.
- Background processing, when introduced, must restore and validate tenant context explicitly.

## 5. Compatibility rules

1. Existing `Purchase`, `PurchaseItem` and `PurchaseReturn` identities and behavior are preserved.
2. They remain Legacy Direct Purchase records and are not backfilled into invented POs, receipts or bills.
3. New source/document types must not reuse legacy purchase source keys.
4. New reporting must distinguish or intentionally combine legacy and new flows.
5. No new feature may require destructive cleanup of current demo rows as a migration side effect.
6. Current development data is Demo/Test, but every schema change remains production-safe.

## 6. Slice entry gates

### PUR-010 may start when

- this alignment and its ADRs are accepted;
- Supplier remains the referenced master;
- currency behavior is explicitly limited to the boundary above;
- no inventory/accounting side effect is included.

### PUR-020/PUR-025 may start when

- numbering and permission keys are defined;
- request status transitions and approval separation-of-duty rules are testable;
- requester/warehouse scope is explicit.

### PUR-050 may start when

- snapshots, revision/concurrency and independent receipt/billing statuses are defined;
- confirmation is proven to cause no stock or GL mutation.

### PUR-060/PUR-065 may start when

- receipt idempotency, remaining-quantity concurrency and WMS ownership are tested;
- GRNI mappings and reversal/reconciliation design are complete before accounting activation.

### PUR-080/PUR-090 may start when

- AP ownership, invoice normalization, billing policy and matching tolerances are approved;
- duplicate and concurrent bill scenarios have SQL-backed tests.

## 7. Deferred decisions

The following are deliberately not solved by PUR-005:

- unified Party/Customer/Supplier master;
- full currency and exchange-rate provider;
- generic cross-module approval engine;
- external event bus and background dispatcher;
- jurisdiction-specific tax/e-invoicing;
- intercompany purchasing;
- supplier portal and external quotation submission;
- complete AP subledger and treasury design.

Each requires evidence from the implementing slice and a separate accepted decision when it becomes necessary.

## 8. Outcome

PUR-005 changes documentation only. The next implementation slice is PUR-010 Supplier Purchasing Data and Lead Time. PUR-010 must remain additive, tenant-safe and free of inventory/accounting posting.
