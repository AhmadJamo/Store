# Accounting implementation assessment
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-09-28

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
| Customers and suppliers account linkage | IMPLEMENTED | Customer account is required; supplier payable account is optional. |
| Tax account mapping | IMPLEMENTED | `TaxRate` stores required input/output tax account IDs. |
| Discount/revenue/COGS account mapping | IMPLEMENTED | Singleton settings configure company defaults; each branch can override sales revenue with a required subaccount. |
| Purchase journal posting | IMPLEMENTED | Explicit posting creates a balanced inventory/input-tax/purchase-discount/supplier-payable entry. |
| Central journal numbering | IMPLEMENTED | Tenant-specific JournalEntry sequence is generated inside posting; supplier invoice remains the external source reference. |
| Payment settlement and customer selection | IMPLEMENTED | Payment method maps to a chart account; customer must use a chart subaccount; sales allow unknown customer. |
| Moving weighted-average inventory valuation | IMPLEMENTED | Product+warehouse average/value, immutable movement snapshots, source-cost transfers and provisional-negative variance are persisted. |
| COGS snapshot | IMPLEMENTED | New direct and prepared sale lines freeze unit cost and COGS; prepared cost is derived from current ingredient averages. |
| COGS/variance journal posting | NOT IMPLEMENTED | Cost snapshots exist, but automatic debit COGS / credit Inventory and variance settlement entries remain pending. |
| Sales/purchase returns | NOT IMPLEMENTED | No return document entities/services found. |

## Existing rules
Sales select the product `SalePrice` for retail POS or `WholesalePrice` for wholesale; client-submitted line price is ignored by `SaleService`. Normal stock removal cannot exceed balance. Prepared-product recipe use and kitchen variance may cross zero only for an ingredient explicitly configured for controlled negative consumption, and the negative row remains visible for reconciliation. A transfer creator cannot approve their own submitted transfer. Discount settings can disable types/limits and an override permission is checked.

## Risks and controls missing
- Sales do not yet create accounting journal entries; a sale captures payment method, optional customer and immutable COGS snapshots, but tax and accounting posting remain to be completed.
- Controlled negative ingredient quantities now retain provisional cost and later receipts calculate a separate cost variance. Posting that variance to the ledger remains pending.
- Purchase posting has no cancellation/reversal workflow yet; posted purchase documents must remain immutable until reversals are added.
- Manual stock transactions are limited to adjustment-in, adjustment-out and reason-required kitchen variance; financial posting for those variances is not implemented.
- ProductStock has rowversion and inventory transactions are Serializable, but database-backed simultaneous recipe-sale/reconciliation tests remain pending.
- Purchase invoices immediately affect stock and have no cancellation/return path.
- Date input is not subject to fiscal-period controls.

## Before accounting expansion
Decide cost method, posting rules, document finality, tax model, currency model and tenant boundary. These are PLANNED decisions, not present code.
