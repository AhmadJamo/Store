# Accounting implementation assessment
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-10-08

For the Arabic accounting-cycle, journal examples, international-standard map, mandatory software controls and ordered implementation guide, see [`ACCOUNTING_REFERENCE_AR.md`](ACCOUNTING_REFERENCE_AR.md). That reference describes the target policy and must not be read as an implemented-feature list; the status table below remains the implementation assessment.

## Accounting foundation (2026-09-13)

The selected valuation method is moving weighted average. The initial accounting schema adds a single company-wide hierarchical chart of accounts, branches and multi-line journal entries. Each journal line can carry an optional branch and warehouse dimension. A journal entry begins as Draft and can post only with at least two lines whose total debit equals total credit. Tax rates link to input/output tax accounts, and accounting settings hold discount, revenue and COGS account mappings. Payment methods link a cash or bank account to sales; sales support an optional registered customer or an explicit unknown walk-in customer.

Warehouses now have optional BranchId and InventoryAccountId fields. Completing the warehouse screen/service is required before these are assigned in production.

## What exists
| Area | Status | Evidence |
|---|---|---|
| Sales invoice totals/discounts | IMPLEMENTED | `Sale`, `SaleItem`, `SaleService`. |
| Purchase document totals | IMPLEMENTED | `Purchase`, `PurchaseItem`, `PurchaseService`. |
| Per-warehouse quantity balances and movement log | IMPLEMENTED | `ProductStock`, `StockTransaction`. |
| Prepared-item recipes and operational ingredient consumption | IMPLEMENTED | Immutable recipe versions, SaleItem snapshot, RecipeConsumption and controlled negative ingredient exceptions; migration applied locally. |
| Stock transfers / reversal by cancellation | IMPLEMENTED | `StockTransferService`. |
| Product prices | IMPLEMENTED | purchase/wholesale/sale price fields. |
| Chart of accounts and balanced journal-entry foundation | IMPLEMENTED | `Account`, `JournalEntry`, `JournalEntryLine` and administration UI. |
| Central journal-posting gateway | IMPLEMENTED | `JournalPostingService` owns source idempotency, journal numbering, construction, balance validation, posting and persistence for purchase, sale and sales-return workflows. |
| Customers and suppliers account linkage | IMPLEMENTED | Customer account is required; supplier payable account is optional. |
| Tax account mapping | IMPLEMENTED | `TaxRate` stores required input/output tax account IDs. |
| Discount/revenue/COGS account mapping | IMPLEMENTED | Singleton settings configure company defaults; each branch can override sales revenue with a required subaccount. |
| Purchase journal posting | IMPLEMENTED | Explicit posting creates a balanced inventory/input-tax/purchase-discount/supplier-payable entry. |
| Central journal numbering | IMPLEMENTED | Tenant-specific JournalEntry sequence is generated inside posting; supplier invoice remains the external source reference. |
| Payment settlement and customer selection | IMPLEMENTED | Payment method maps to a chart account; customer must use a chart subaccount; sales allow unknown customer. |
| Moving weighted-average inventory valuation | IMPLEMENTED | Product+warehouse average/value, immutable movement snapshots, source-cost transfers and provisional-negative variance are persisted. |
| COGS snapshot | IMPLEMENTED | New direct and prepared sale lines freeze unit cost and COGS; prepared cost is derived from current ingredient averages. |
| Sale revenue, output tax and COGS posting | IMPLEMENTED | Explicit idempotent posting debits settlement and net invoice discount, credits net revenue and frozen output tax, debits COGS and credits warehouse inventory. Inclusive and exclusive invoice tax are supported. |
| Provisional negative-stock cost settlement | IMPLEMENTED | Purchase posting reads receipt cost variances and adjusts COGS against the affected warehouse inventory account in either direction. |
| Sales returns | IMPLEMENTED | Immutable partial/full return documents cap cumulative quantity, reverse revenue/output tax/discount, and restock direct items with historical-cost COGS reversal. |
| Fiscal periods | IMPLEMENTED | Tenant periods cannot overlap; once configured, the central journal gateway accepts postings only inside an Open period. Soft Closed and Closed block operational posting. |
| Purchase returns | IMPLEMENTED | Immutable partial/full supplier returns reverse payable, input tax and discount, remove stock at current moving average and post the cost difference to COGS. |
| Goods Receipt GRNI posting | IMPLEMENTED | Posted new-flow GRNs debit the warehouse inventory account and credit the configured Goods Received Not Invoiced account through the central journal gateway. |
| Receipt-based supplier returns | IMPLEMENTED | Posted new-flow returns debit GRNI at original receipt value, credit Inventory at actual removed AVCO cost and post any difference to the configured Purchase Price Variance account. |
| Vendor Bill posting | IMPLEMENTED | Posted receipt allocations debit GRNI at receipt value, debit/credit Purchase Price Variance for the bill-net difference, debit snapshotted input tax and credit the supplier's payable account. |
| Three-way match gate | IMPLEMENTED | Quantity and net-price differences above tenant tolerances block Vendor Bill posting unless a separately authorized, reasoned override is persisted with the exceptions. |
| Supplier Payment posting | IMPLEMENTED | Each payment debits the supplier payable account and credits the cash/bank account mapped by the selected active payment method; partial allocations are capped by cumulative Vendor Bill outstanding balances. |

## Existing rules
Sales select the product `SalePrice` for retail POS or `WholesalePrice` for wholesale; client-submitted line price is ignored by `SaleService`. Normal stock removal cannot exceed balance. Prepared-product recipe use and kitchen variance may cross zero only for an ingredient explicitly configured for controlled negative consumption, and the negative row remains visible for reconciliation. A transfer creator cannot approve their own submitted transfer. Discount settings can disable types/limits and an override permission is checked.

Feature services calculate business amounts and resolve accounts, then send prepared lines through the central posting gateway inside their existing Serializable UnitOfWork. The gateway validates the fiscal period, rejects duplicate sources, generates the journal number, invokes domain balancing/posting and persists the entry. Until a company creates its first fiscal period, the gateway preserves legacy all-dates-open behavior.

## Risks and controls missing
- Sales post explicitly and once to a balanced revenue/output-tax/settlement/discount and COGS/inventory journal. The current tax model supports one optional frozen tax rate per invoice; mixed-rate product taxes remain future scope.
- Controlled negative ingredient quantities retain provisional cost; later purchase receipts calculate the variance and purchase posting settles it between COGS and warehouse inventory. Inclusive input tax is excluded from receipt inventory cost.
- Purchase posting has no cancellation/reversal workflow yet; posted purchase documents must remain immutable until reversals are added.
- Manual stock transactions are limited to adjustment-in, adjustment-out and reason-required kitchen variance; financial posting for those variances is not implemented.
- ProductStock has rowversion and inventory transactions are Serializable, but database-backed simultaneous recipe-sale/reconciliation tests remain pending.
- Purchase invoices immediately affect stock; returns now require prior posting and sufficient remaining quantity/current stock, while general cancellation remains unavailable.
- Soft Closed currently blocks the same operational posting sources as Closed; a future manual-adjustment workflow may add a narrowly permissioned soft-close override.

## Before accounting expansion
Generalized reversals, currency policy and mixed-rate product tax remain planned decisions before broader accounting expansion.
## Planned procure-to-pay accounting boundary

PUR-005 accepts GRNI as the accounting bridge for the new, separate Goods Receipt and Vendor Bill flow. PO confirmation has no GL effect. A posted GRN debits the warehouse Inventory account and credits the configured Goods Received Not Invoiced account through the central fiscal-period/idempotency gateway. A posted receipt-based return may reverse only the unbilled portion: it reverses GRNI at original receipt value, removes inventory at current AVCO and records the difference in Purchase Price Variance. A posted Vendor Bill clears allocated GRNI, records the difference between bill net and receipt value in Purchase Price Variance, recognizes snapshotted recoverable input tax and credits the supplier's Accounts Payable account. A Supplier Payment then debits that payable account and credits the selected payment method's cash/bank account. SQL integration verifies receipt, bill, payment and unbilled-return journal atomicity plus concurrency protection. Inventory-to-GL reconciliation reporting remains later work. See `PURCHASE_ARCHITECTURE_ALIGNMENT.md` and `decisions/2026-10-08-grni-and-purchase-currency-boundary.md`.
