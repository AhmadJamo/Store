# MiniStore purchase management evolution plan
> Status: APPROVED ARCHITECTURAL PLAN — PUR-000 THROUGH PUR-030 IMPLEMENTED
> Source of truth: current code and migrations  
> Last reviewed: 2026-10-08

## 1. Executive decision

MiniStore will evolve from its current direct-purchase workflow into an ERP-grade procure-to-pay module through additive vertical slices. The existing `Purchase` aggregate is not a purchase order: creating it records a supplier invoice and receives inventory immediately, while accounting is posted separately. It will therefore remain a compatible **Legacy Direct Purchase** workflow and will not be renamed or silently reinterpreted as a purchase order.

The target workflow is:

```text
Purchase Request
    -> approval
    -> sourcing event / RFQ
    -> supplier quotations and comparison
    -> Purchase Order
    -> one or more Goods Receipts
    -> Vendor Bill and three-way matching
    -> Supplier Payment
```

Commercial intent, physical receipt, supplier liability and payment are separate responsibilities. A purchase order never changes stock. Only a posted goods receipt changes inventory. A vendor bill creates the supplier payable, and payment settles that payable.

### Current database data classification

All records currently stored in the development database are classified as **demo/test data**, including suppliers, purchases, inventory, users and accounting history. Demo rows may be deliberately reset or recreated when a clean baseline is required. This does not make the schema disposable: every migration, tenant boundary, accounting rule and workflow must remain production-safe. No destructive reset is an automatic migration side effect.

### Execution boundary

This document authorizes the ordered purchase-management roadmap, not unrelated scope. Each slice must include its domain rules, Application services, persistence, permissions, bilingual UI, tests, migration when required and documentation. Platform-enforced approval prompts, production credentials, external publication and destructive operations remain outside this standing project authorization.

## 2. Existing system review

### Implemented strengths to preserve

- Domain/Application/Infrastructure/Web dependency direction, DTO mapping, repositories and the existing UnitOfWork.
- Shared-database tenant isolation, tenant-aware relationships and write guards.
- Suppliers, products, categories, measurement units and managed conversion factors.
- Warehouses, exact locations, Unassigned Stock, putaway, movements, balances and reservation-aware removal.
- Lot, serial, manufacture-date and expiration tracking.
- `ProductStock` warehouse quantity and moving weighted-average valuation.
- Immutable stock transactions and physical movement facts.
- Tax rates with purchase input accounts.
- Chart of accounts, journal posting gateway, fiscal periods and centralized document numbering.
- Tenant-owned roles, permissions, localization and general audit interception.
- Immutable supplier returns against legacy direct purchases.
- Reviewed replenishment suggestions that can later create purchase demand.

### Current direct-purchase behavior

`PurchaseService` validates the supplier, product, warehouse, quantity, price and tracking data, then immediately updates inventory and AVCO inside a Serializable UnitOfWork. `PurchasePostingService` later posts inventory, input tax, discount, payable and provisional-negative settlement entries. The same record therefore combines a supplier invoice, goods receipt and inventory valuation event.

This workflow remains useful for compatibility and potentially for a controlled quick-purchase mode. It must not become the authority for the new purchase-order lifecycle.

## 3. Capability gap analysis

| Capability | Current state | Target decision |
|---|---|---|
| Supplier purchasing data | Basic supplier identity/account | Add supplier-product terms, lead time, price validity and priority |
| Purchase request | Missing | New approval-aware aggregate |
| RFQ/sourcing | Missing | New sourcing event with multiple supplier invitations |
| Supplier quotations | Missing | Separate immutable supplier quotation aggregate and comparison |
| Purchase order | Current `Purchase` is not a PO | Add a new `PurchaseOrder` aggregate |
| Receipt control | Receipt happens during Purchase creation | Add partial/multiple posted Goods Receipts |
| Billing | Purchase row acts as supplier invoice | Add separate Vendor Bill and allocations |
| Three-way matching | Missing | PO/receipt/bill matching with configured tolerances |
| Payment | No AP payment lifecycle | Integrate with Accounting after bill posting exists |
| Approval | Missing | Configurable rules, instances, steps and history |
| Currency | Company display currency only | Freeze document currency/rate; add full exchange-rate policy separately |
| Tax history | Mutable TaxRate reference | Freeze tax code/rate/inclusion/account snapshots |
| Agreements/templates | Missing | Later vertical slices |
| Landed cost | Missing | Later receipt-linked valuation adjustment |
| Receiving scan | Deferred | Add after Goods Receipt is authoritative |
| Notifications | No purchase notification subsystem | Add reliable post-commit/outbox notifications later |

## 4. Authoritative domain model

### 4.1 Supplier purchasing information

`SupplierProductPurchasingInfo` owns supplier-specific commercial terms for one product:

- supplier and product;
- supplier product code and description;
- purchase measurement unit;
- minimum order quantity and order multiple;
- normal lead time;
- unit price, currency and validity dates;
- preferred flag and priority;
- active state and rowversion.

These values do not belong on `Product`, because they vary by supplier.

### 4.2 Purchase Request

`PurchaseRequest` and its lines express internal demand. The header records requester, department/cost context when available, priority, needed date, destination warehouse, justification and status. Lines record product, requested unit/quantity, converted stock quantity, suggested supplier and notes.

The initial lifecycle is Draft -> Submitted -> Approved/Rejected -> Sourcing/Ordered -> Closed, with reasoned cancellation. Approved quantities cannot be silently changed.

### 4.3 Approval model

Approval thresholds must not be hard-coded. Purchase-focused approval rules initially evaluate document type, amount, warehouse, requester/role and priority. Approval instances and steps freeze the rule used, assigned approver, decision, actor, timestamp and reason. The general audit interceptor supplements but does not replace explicit business history.

### 4.4 Sourcing event and supplier quotation

A `PurchaseSourcingEvent` groups one commercial need and requirement lines. Invitations target multiple suppliers. Each `SupplierQuotation` belongs to one supplier and freezes quoted price, currency, tax, validity, lead time, quantity and commercial terms. Comparison is computed from quotation snapshots; it never reads the supplier's current default price as historical truth.

### 4.5 Purchase Order

`PurchaseOrder` is a new aggregate. It owns supplier, internal order number, source request/quotation/agreement, order date, expected date, default destination warehouse, currency/rate, payment terms, commercial totals, rowversion and action history.

Status is intentionally multidimensional:

- order/approval status;
- receipt status;
- billing status;
- payment status.

Lines freeze product, unit, conversion factor, ordered/stock quantities, price, discounts, tax and destination. Receipt and billing progress are derived from posted child allocations, not accepted as freely editable counters.

### 4.6 Goods Receipt

`GoodsReceipt` is the inventory receipt document and references a confirmed PO. Each receipt targets one warehouse; orders spanning warehouses produce separate receipts. Lines reference PO lines and may contain exact location and lot/serial/expiry allocations.

Posting is atomic and idempotent. It validates remaining quantity, tracking policy, warehouse access and location capability, then updates ProductStock/AVCO, InventoryBalance, tracking balances, StockTransaction and StockMovement through the existing Application services. Posted receipts are immutable; corrections use a controlled return or reversal.

### 4.7 Vendor Bill

`VendorBill` freezes the supplier's invoice number/date, currency/rate, terms, tax snapshots and totals. Lines allocate quantities and values to PO and receipt lines according to the configured billing policy. Active supplier invoice uniqueness is tenant + supplier + normalized supplier invoice number.

### 4.8 Three-way matching

Matching compares:

| Source | Authority |
|---|---|
| Purchase Order | agreed quantity, price, discount and tax |
| Goods Receipt | quantity and identity actually received |
| Vendor Bill | quantity and amount requested by the supplier |

Quantity and price tolerances are tenant settings. An override requires a dedicated permission, reason, actor and timestamp. Match runs and exceptions are persisted when a bill is submitted/approved so the decision remains auditable.

### 4.9 Later aggregates

- `PurchaseAgreement` and releases for blanket/contract purchasing.
- Purchase templates for repeat orders.
- Receipt-based supplier returns for the new flow; legacy Purchase Returns remain intact.
- `LandedCost` and allocation records for freight, customs, insurance and other receipt costs.
- Supplier performance projections for delivery, quality, price variance and rejection metrics.

## 5. Integration contracts

### Inventory

- Request, RFQ, quotation and PO confirmation do not change on-hand inventory.
- A posted Goods Receipt is the only positive stock event in the new flow.
- Existing inventory services remain responsible for tracking, AVCO, balances, physical movement and putaway.
- WMS-090C may convert reviewed replenishment into a Draft Purchase Request after that lifecycle exists.
- WMS-100B4 receiving scans will target Draft Goods Receipts and reuse idempotent scan conventions.

### Accounting

The target accounting boundary uses Goods Received Not Invoiced (GRNI):

```text
Goods receipt:  Dr Inventory / Cr GRNI
Vendor bill:    Dr GRNI + Dr Input Tax +/- Price Variance / Cr Accounts Payable
Payment:        Dr Accounts Payable / Cr Cash or Bank
```

GRNI, purchase-price variance, landed-cost clearing and currency gain/loss accounts require explicit configuration before the related posting slice is enabled. The existing central journal gateway and fiscal-period validation remain mandatory.

### Sales and other modules

Purchasing shares products, units, taxes and inventory with Sales but does not depend on sales documents. Dropship, manufacturing and external integrations remain later consumers of the same purchase contracts; they must not be embedded prematurely in the core aggregates.

### Permissions, localization and notifications

Separate permissions are required for view, create, edit draft, submit, approve, reject, cancel, send RFQ, compare quotes, confirm PO, receive, bill, override match, pay and close. Every screen/message requires English and Arabic resources and RTL-safe layout. Notifications must be emitted after a successful commit, preferably through a durable outbox, so delivery failure cannot roll back a valid business transaction.

## 6. Proposed database evolution

Expected table families, introduced only with their owning vertical slice:

- `SupplierProductPurchasingInfos`
- `PurchaseRequests`, `PurchaseRequestLines`
- `PurchaseApprovalRules`, `PurchaseApprovalInstances`, `PurchaseApprovalSteps`, `PurchaseActionHistories`
- `PurchaseSourcingEvents`, `PurchaseSourcingLines`, `PurchaseSupplierInvitations`
- `SupplierQuotations`, `SupplierQuotationLines`
- `PurchaseOrders`, `PurchaseOrderLines`
- `GoodsReceipts`, `GoodsReceiptLines`, `GoodsReceiptTrackingAllocations`
- `VendorBills`, `VendorBillLines`, `VendorBillAllocations`
- `PurchaseMatchRuns`, `PurchaseMatchExceptions`
- `PurchaseAgreements`, `PurchaseAgreementLines`, `PurchaseAgreementReleases`
- `LandedCosts`, `LandedCostAllocations`

Every tenant-owned table must participate in the established tenant-isolation model and composite relationship checks. Financial and posted operational documents require immutable snapshots, stable source IDs, rowversion where edits are allowed, idempotency keys for commands that may be retried and indexes matching real lookup paths.

## 7. Compatibility and migration strategy

1. Keep existing `Purchase`, `PurchaseItem` and `PurchaseReturn` tables and routes operational during transition.
2. Do not backfill existing purchases into artificial requests, POs, receipts or bills; that would invent approvals and document identities.
3. Label old records and screens as Legacy Direct Purchases when the new flow becomes visible.
4. Add new tables and nullable supplier fields only through additive migrations.
5. Extend centralized document types and seed tenant defaults idempotently per slice.
6. Reconcile inventory and journals before enabling new receipt/accounting posting.
7. Pilot the new workflow beside the legacy workflow, then disable legacy creation only after business acceptance; historical reads and returns remain supported.
8. Never reuse `Purchase.InvoiceNumber` as the new PO number. It remains the external reference for legacy history.

### Primary migration risks

- confusing legacy Purchase identity with Purchase Order identity;
- cross-tenant foreign keys or uniqueness gaps;
- duplicate supplier invoices caused by normalization differences;
- mutable unit/tax/currency data changing historical documents;
- duplicate receipts or bills after retry/concurrency;
- accounting cutover that leaves inventory subledger and GL out of balance;
- returns crossing legacy and new receipt models;
- oversized migrations or speculative backfills.

Each slice must include rollback/retry analysis and SQL-backed tenant/concurrency coverage proportional to risk.

## 8. Delivery roadmap

| Slice | Scope | Completion evidence |
|---|---|---|
| PUR-000 | Architecture, legacy boundary and roadmap | This plan + ADR, no runtime change |
| PUR-005 | Context ownership, currency, approval and GRNI alignment | Alignment gate + ownership/accounting ADRs, no runtime change |
| PUR-010 | Supplier purchasing data and lead time | Implemented through bilingual UI with tests; additive migration applied to `AHMAD/MiniStoreDb` |
| PUR-020 | Purchase Request lifecycle | Implemented: domain/schema/numbering/history, Application service, permissions and bilingual UI |
| PUR-025 | Configurable purchase approvals | Implemented: tenant-safe rule administration, deterministic submit-time resolution, frozen ordered role steps, role/permission-gated decisions and bilingual UI |
| PUR-030 | Sourcing event and supplier invitations | Implemented: RFX numbering, frozen approved-demand snapshots, deduplicated supplier invitations, permissions and bilingual UI; no stock or accounting mutation |
| PUR-040 | Supplier quotations and comparison | Frozen terms and auditable award decision |
| PUR-050 | Purchase Order lifecycle | Create/approve/confirm/cancel/close; no stock mutation |
| PUR-060 | Partial Goods Receipt | Idempotent receipt integrated with WMS and tracking |
| PUR-065 | Receipt accounting | GRNI posting and reconciliation |
| PUR-070 | Receipt-based supplier returns | New-flow returns without breaking legacy returns |
| PUR-080 | Vendor Bills | Partial billing, duplicate protection and AP posting |
| PUR-090 | Three-way matching | Tolerances, exceptions and controlled override |
| PUR-100 | Supplier payments | AP settlement through Accounting |
| PUR-110 | Replenishment purchase demand | WMS-090C -> Draft Purchase Request |
| PUR-120 | Agreements and templates | Contract releases and repeat ordering |
| PUR-130 | Landed costs | Auditable receipt valuation adjustments |
| PUR-140 | Dashboards and supplier performance | Operational KPIs and drill-down reports |
| PUR-150 | Barcode receiving | WMS-100B4 against Goods Receipts |

No slice may claim completion from entity creation alone. It must cross every affected layer, preserve tenant and accounting boundaries, include translations and update documentation.

## 9. Anticipated file impact

New code belongs in the existing feature folders:

- `MiniStore.Domain/Entities/Purchases`, `Enums/Purchases`, `Interfaces/Purchases`
- `MiniStore.Application/Dtos/Purchases`, `Services/Purchases`, `Permissions`
- `MiniStore.Infrastructure/Persistence/Configurations/Purchases`, `Repositories/Purchases`, `Migrations`
- `MiniStore.Web/Controllers/Purchases`, `Views/Purchases`, navigation and shared localization resources
- focused regression checks in the existing executable test suites, plus SQL integration scenarios where persistence/concurrency is material

Shared accounting, settings, inventory and document-number files are changed only by the slice that needs them. `Program.cs` remains a concise composition root; registrations belong in the existing feature DI extensions.

## 10. Immediate next slice

PUR-010 is implemented and `AddSupplierPurchasingData` is applied to `AHMAD/MiniStoreDb`. Smoke-test the bilingual screen, then begin **PUR-020 Purchase Request lifecycle**. PUR-010 remains additive and has no inventory/accounting side effect or foreign-currency posting claim.
