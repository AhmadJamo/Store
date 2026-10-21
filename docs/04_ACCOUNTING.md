# Accounting implementation assessment
## Customer return disposition posting

`SAL-100D` posts an independent financial credit for eligible quantities already present in a Posted customer-return receipt. It debits the invoice's frozen sales-revenue and output-tax accounts and credits its frozen receivable account in company currency, carrying the return warehouse branch/warehouse dimensions. Posted credits reduce the invoice's derived outstanding balance and payment eligibility. The credit does not repeat inventory/COGS, refund cash or silently apply the balance elsewhere.

`SAL-100C` posts only inspected `Restock` lines. It restores inventory at the frozen historical delivery cost and creates one source-unique balanced journal per return receipt: debit the affected warehouse inventory account and credit Cost of Sales, carrying branch and warehouse dimensions. Quarantine, Reject and Destroy produce no inventory or journal value. The posting does not reduce receivables, reverse revenue/output tax, create a credit note or refund cash; those remain separate controlled actions.

## Customer return authorization boundary

`SAL-100A` produces no journal, stock movement, credit note, refund or customer-balance change. Accounting begins only in later receipt/disposition and credit-note slices.

`SAL-100B` is also accounting-neutral: inspection freezes custody/disposition evidence but posts no inventory, COGS, receivable, tax, credit or cash.

> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-10-21

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
| Configurable customer receivable policy | IMPLEMENTED FOUNDATION | Tenant may choose shared control, automatic individual subaccounts or manual selection prospectively. Existing links/history remain unchanged; later mode changes with dependent sales require a controlled transition. |
| Tax account mapping | IMPLEMENTED | `TaxRate` stores required input/output tax account IDs. |
| Discount/revenue/COGS account mapping | IMPLEMENTED | Singleton settings configure company defaults; each branch can override sales revenue with a required subaccount. |
| Purchase journal posting | IMPLEMENTED | Explicit posting creates a balanced inventory/input-tax/purchase-discount/supplier-payable entry. |
| Central journal numbering | IMPLEMENTED | Tenant-specific JournalEntry sequence is generated inside posting; supplier invoice remains the external source reference. |
| Payment settlement and customer selection | IMPLEMENTED | Payment method maps to a chart account; customer must use a chart subaccount; sales allow unknown customer. |
| Moving weighted-average inventory valuation | IMPLEMENTED | Product+warehouse average/value, immutable movement snapshots, source-cost transfers and provisional-negative variance are persisted. |
| COGS snapshot | IMPLEMENTED | New direct and prepared sale lines freeze unit cost and COGS; prepared cost is derived from current ingredient averages. |
| Sale revenue, output tax and COGS posting | IMPLEMENTED | Explicit idempotent posting debits settlement and net invoice discount, credits net revenue and frozen output tax, debits COGS and credits warehouse inventory. Inclusive and exclusive invoice tax are supported. |
| Customer Invoice receivable posting | IMPLEMENTED | SAL-080 separately posts an Approved invoice once: debit the customer asset receivable account, credit branch/default sales revenue and output-tax liability in company currency. Delivered amounts preserve exact source branch/warehouse dimensions; posting does not create a payment or inventory/COGS effect. |
| Customer payment settlement | IMPLEMENTED | SAL-090 debits the active payment-method asset account, credits each invoice's frozen receivable carrying amount, records explicit realized exchange gain/loss where required and supports exact reversal. Posted allocations derive Unpaid/Partially Paid/Paid and never mutate inventory. |
| Customer Invoice accounting reversal | IMPLEMENTED | A dated reasoned reversal creates one exact inverse journal, preserves both entries and source evidence, marks the invoice Reversed and releases billing eligibility without deleting history. |
| Provisional negative-stock cost settlement | IMPLEMENTED | Purchase posting reads receipt cost variances and adjusts COGS against the affected warehouse inventory account in either direction. |
| Sales returns | IMPLEMENTED | Immutable partial/full return documents cap cumulative quantity, reverse revenue/output tax/discount, and restock direct items with historical-cost COGS reversal. |
| Fiscal periods | IMPLEMENTED | Tenant periods cannot overlap; once configured, the central journal gateway accepts postings only inside an Open period. Soft Closed and Closed block operational posting. |
| Purchase returns | IMPLEMENTED | Immutable partial/full supplier returns reverse payable, input tax and discount, remove stock at current moving average and post the cost difference to COGS. |
| Goods Receipt GRNI posting | IMPLEMENTED | Posted new-flow GRNs debit the warehouse inventory account and credit the configured Goods Received Not Invoiced account through the central journal gateway. |
| Staged receipt quality hold | IMPLEMENTED | PUR-250A Receive/Inspect/Store creates no stock or journal during receipt or inspection. Storage posts only accepted quantity to Inventory/GRNI; rejected, damaged and short quantities remain non-financial evidence. |
| Dropship supplier delivery | IMPLEMENTED | PUR-250B creates no Goods Receipt, inventory valuation or GRNI. Supplier delivery proof makes only the evidenced quantity billable; posting debits the product's frozen purchase Expense account and credits supplier payable (plus normal input-tax handling). |
| Receipt-based supplier returns | IMPLEMENTED | Posted new-flow returns debit GRNI at original receipt value, credit Inventory at actual removed AVCO cost and post any difference to the configured Purchase Price Variance account. |
| Vendor Bill posting | IMPLEMENTED | Entry creates a Draft with no journal; matching approval also has no journal. Only the separate Post action on an Approved bill posts accounting. Receipt-policy lines clear GRNI and isolate Purchase Price Variance/exchange variance. Ordered-quantity and service-completion lines debit the frozen line Expense account. Recoverable input tax is debited and supplier payable is credited for the complete mixed bill. |
| Vendor Bill attachment extraction | IMPLEMENTED | PUR-260 extraction creates a review-only suggestion and no journal. Explicit Apply may update a Draft header and its dated company-currency values, but normal matching approval and separate posting remain mandatory. Suggested totals never replace authoritative bill lines. |
| Supplier portal/API/UBL intake | IMPLEMENTED | PUR-270 quotations enter the normal commercial workflow, order confirmations update operational dates, and delivery notices/invoice UBL remain review evidence. No external endpoint creates inventory, approves a Vendor Bill or posts a journal. |
| Three-way match gate | IMPLEMENTED | Quantity and net-price differences above tenant tolerances block Vendor Bill posting unless a separately authorized, reasoned override is persisted with the exceptions. |
| Supplier Payment posting | IMPLEMENTED | Each payment debits the supplier payable account and credits the cash/bank account mapped by the selected active payment method; partial allocations are capped by cumulative Vendor Bill outstanding balances. |
| Purchase Inventory/GRNI reconciliation | IMPLEMENTED | PUR-160 provides an as-of-date, read-only comparison of warehouse operational inventory to assigned Inventory accounts and receipt-derived GRNI to the configured ledger account, with document evidence and SQL coverage. |
| Derived Purchase Order closure | IMPLEMENTED | PUR-165 closes a fully received order only after every received quantity is billed or returned; reasoned remaining-quantity closure also requires settled receipts. Closure itself creates no journal and preserves downstream accounting documents as authority. |
| Service completion evidence | IMPLEMENTED | Service completion is operational evidence only and deliberately posts no stock or journal. Its approved Vendor Bill recognizes the cost by debiting the frozen purchase Expense account, without GRNI or inventory movement. |
| Supplier Credit Notes | IMPLEMENTED | PUR-170 returns billed receipt stock, debits supplier payable, credits snapshotted input tax and current inventory cost, and balances net-versus-current-cost difference through Purchase Price Variance without changing the original bill. |
| Controlled Purchase Reversal | IMPLEMENTED | PUR-180 preserves the original receipt/bill/payment/landed-cost journal as Reversed and posts a separate exact debit-credit opposite in an open fiscal period; ledger reconciliation includes both entries so they net to zero. |

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
Generalized reversals outside the supported purchase types and broader mixed-rate tax policy remain planned decisions. Supplier Credit Notes provide the billed-return path and preserve the original bill's company-currency carrying values.
## Planned procure-to-pay accounting boundary

PUR-005 accepts GRNI as the accounting bridge for the separate Goods Receipt and Vendor Bill flow. PUR-190 defines one rate as company-currency units per one document-currency unit and resolves the latest dated rate on or before the document date. Supplier quotations may select any configured currency and comparison normalizes gross values to company currency. PO confirmation freezes a rate but has no GL effect. A posted GRN values Inventory and GRNI at its receipt-date rate. A Vendor Bill clears that frozen GRNI, sends commercial price difference to Purchase Price Variance, sends the remaining receipt-to-bill exchange difference to exchange gain/loss, recognizes input tax at the bill rate and credits payable at the bill-rate company value. A Supplier Payment debits payable at its proportional bill carrying value, credits cash/bank at the payment-date value and posts the difference as realized exchange gain/loss. Supplier Credit Notes preserve proportional bill-rate company values while removing stock at current AVCO. All document and company-currency values remain separately visible and reproducible. See `decisions/2026-10-13-purchase-multi-currency-policy.md`.

## PUR-130 landed-cost posting

Posted Landed Costs use the company base currency and one configured clearing account. The system allocates freight, customs, insurance and other components to a posted Goods Receipt by stock quantity or frozen receipt value. The share associated with current positive warehouse stock debits Inventory and increases AVCO without changing quantity; the consumed share debits Cost of Sales. The total credits Landed Cost Clearing. The document, value-only StockTransaction and balanced journal commit atomically, and fiscal-period rules apply through the central journal gateway.

## PUR-150 barcode receipt boundary

Creating a Draft Goods Receipt and recording barcode scans have no stock, valuation or journal effect. The explicit final post revalidates the order and accumulated quantities, then uses the existing atomic Goods Receipt path: Inventory is debited and Goods Received Not Invoiced is credited only when the draft becomes Posted.
## SAL-050 sales-order reservation boundary

SAL-050 inventory reservation has no journal authority. Creating or releasing a sales-order reservation changes only `InventoryBalance.Reserved`; `OnHand`, `StockTransaction`, `StockMovement`, revenue, receivable, inventory and COGS accounts remain unchanged. SAL-060 Delivery owns the later physical issue and delivery-time COGS decision.

## SAL-060 sales-delivery posting

Posting a `SalesDelivery` is the physical and cost-recognition boundary for the professional sales workflow. The transaction removes the exact reserved warehouse/location quantity, decreases `OnHand` and `Reserved` together, freezes the current moving-average unit cost on every delivery line and records one source-unique journal:

```text
Debit:  Cost of Goods Sold
Credit: Warehouse Inventory
```

The shared fiscal-period and journal-posting controls apply. Draft creation and Draft cancellation have no stock or accounting effect. A retry cannot post a second journal, stock transaction or physical movement. SAL-060 does not recognize revenue, create accounts receivable or record payment; SAL-070/080 own those later boundaries.

SAL-070 creates and approves customer invoice documents but deliberately does not post them. Its company setting chooses Delivered or Ordered quantity eligibility, not an accounting method. Approval freezes billing evidence and reserves source quantity only; it does not debit receivables, credit revenue/output tax or record payment. This prevents a document from appearing financially Posted before a balanced source-unique journal exists.

SAL-080 owns invoice recognition and SAL-090 separately owns receipt allocation and receivable settlement. Neither repeats delivery-time COGS or inventory effects. An allocated invoice cannot be reversed until its posted customer payments are reversed.
