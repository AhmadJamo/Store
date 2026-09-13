# Accounting implementation assessment
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-09-13

## Accounting foundation (2026-09-13)

The selected valuation method is moving weighted average. The initial accounting schema adds a single company-wide hierarchical chart of accounts, branches and multi-line journal entries. Each journal line can carry an optional branch and warehouse dimension. A journal entry begins as Draft and can post only with at least two lines whose total debit equals total credit. Tax rates link to input/output tax accounts, and accounting settings hold discount, revenue and COGS account mappings. Payment methods link a cash or bank account to sales; sales support an optional registered customer or an explicit unknown walk-in customer.

Warehouses now have optional BranchId and InventoryAccountId fields. Completing the warehouse screen/service is required before these are assigned in production.

## What exists
| Area | Status | Evidence |
|---|---|---|
| Sales invoice totals/discounts | IMPLEMENTED | `Sale`, `SaleItem`, `SaleService`. |
| Purchase document totals | IMPLEMENTED | `Purchase`, `PurchaseItem`, `PurchaseService`. |
| Per-warehouse quantity balances and movement log | IMPLEMENTED | `ProductStock`, `StockTransaction`. |
| Stock transfers / reversal by cancellation | IMPLEMENTED | `StockTransferService`. |
| Product prices | IMPLEMENTED | purchase/wholesale/sale price fields. |
| Chart of accounts and balanced journal-entry foundation | IMPLEMENTED | `Account`, `JournalEntry`, `JournalEntryLine` and administration UI. |
| Customers and suppliers account linkage | IMPLEMENTED | Customer account is required; supplier payable account is optional. |
| Tax account mapping | IMPLEMENTED | `TaxRate` stores required input/output tax account IDs. |
| Discount/revenue/COGS account mapping | IMPLEMENTED | Singleton settings configure company defaults; each branch can override sales revenue with a required subaccount. |
| Purchase journal posting | IMPLEMENTED | Explicit posting creates a balanced inventory/input-tax/purchase-discount/supplier-payable entry. |
| Payment settlement and customer selection | IMPLEMENTED | Payment method maps to a chart account; customer must use a chart subaccount; sales allow unknown customer. |
| Inventory valuation and COGS | NOT IMPLEMENTED | Quantity moves only; no cost layers/valuation. |
| Sales/purchase returns | NOT IMPLEMENTED | No return document entities/services found. |

## Existing rules
Sales select the product `SalePrice` for retail POS or `WholesalePrice` for wholesale; client-submitted line price is ignored by `SaleService`. Stock cannot be removed beyond current balance in one tracked operation. A transfer creator cannot approve their own submitted transfer. Discount settings can disable types/limits and an override permission is checked.

## Risks and controls missing
- Sales do not yet create accounting journal entries; a sale now captures payment method and optional customer, but tax and weighted-average COGS posting remain to be completed.
- Purchase posting has no cancellation/reversal workflow yet; posted purchase documents must remain immutable until reversals are added.
- Manual stock transaction UI can create Purchase/Sale/Transfer types without their source document.
- No stock concurrency token can make balance data diverge from movement records under concurrent operations.
- Purchase invoices immediately affect stock and have no cancellation/return path.
- Date input is not subject to fiscal-period controls.

## Before accounting expansion
Decide cost method, posting rules, document finality, tax model, currency model and tenant boundary. These are PLANNED decisions, not present code.
