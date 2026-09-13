# Accounting implementation assessment
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-09-13

## What exists
| Area | Status | Evidence |
|---|---|---|
| Sales invoice totals/discounts | IMPLEMENTED | `Sale`, `SaleItem`, `SaleService`. |
| Purchase document totals | IMPLEMENTED | `Purchase`, `PurchaseItem`, `PurchaseService`. |
| Per-warehouse quantity balances and movement log | IMPLEMENTED | `ProductStock`, `StockTransaction`. |
| Stock transfers / reversal by cancellation | IMPLEMENTED | `StockTransferService`. |
| Product prices | IMPLEMENTED | purchase/wholesale/sale price fields. |
| General ledger, chart of accounts, debit/credit | NOT IMPLEMENTED | No entities/services found. |
| Receivables, payables, customers, payments/cash/bank | NOT IMPLEMENTED | No entities/services found. |
| Tax, fiscal periods, financial statements | NOT IMPLEMENTED | No entities/services found. |
| Inventory valuation and COGS | NOT IMPLEMENTED | Quantity moves only; no cost layers/valuation. |
| Sales/purchase returns | NOT IMPLEMENTED | No return document entities/services found. |

## Existing rules
Sales select the product `SalePrice` for retail POS or `WholesalePrice` for wholesale; client-submitted line price is ignored by `SaleService`. Stock cannot be removed beyond current balance in one tracked operation. A transfer creator cannot approve their own submitted transfer. Discount settings can disable types/limits and an override permission is checked.

## Risks and controls missing
- Posted sales and purchases have no accounting journal entry and no reversal/return workflow.
- Manual stock transaction UI can create Purchase/Sale/Transfer types without their source document.
- No stock concurrency token can make balance data diverge from movement records under concurrent operations.
- Purchase invoices immediately affect stock and have no cancellation/return path.
- Date input is not subject to fiscal-period controls.

## Before accounting expansion
Decide cost method, posting rules, document finality, tax model, currency model and tenant boundary. These are PLANNED decisions, not present code.
