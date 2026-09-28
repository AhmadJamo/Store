# Moving weighted-average inventory cost

## Status
Accepted and implemented on 2026-09-28.

## Decision
Inventory is valued per Product + Warehouse using a moving weighted average. ProductStock stores quantity, current average unit cost, inventory value and a last reference unit cost. Receipts recalculate the average from existing value plus net incoming line cost. Issues preserve the current average and snapshot their value. Warehouse transfers carry the source unit cost into the destination average.

Every new StockTransaction freezes quantity, average and inventory value before/after, movement unit cost/value and any provisional-negative settlement variance. SaleItem freezes unit cost and COGS; prepared-product cost is derived from the active recipe and current warehouse ingredient costs.

## Controlled negative stock
Authorized recipe/kitchen consumption may cross zero using the current or last reference cost. A later receipt keeps remaining positive stock at the receipt cost and records the difference between provisional and received cost separately as CostVariance. Accounting journal settlement of that variance is a following posting phase.

## Compatibility
Existing ProductStock rows are initialized from the product purchase price because historic cost snapshots do not exist. Historical StockTransactions and SaleItems remain zero-valued rather than fabricating unsupported history. All new movements after migration contain full valuation snapshots.

## Consequences
- Valuation remains warehouse-level; rack/bin balances do not own separate averages.
- Purchase line discount reduces incoming cost. Current recoverable purchase tax remains outside inventory cost.
- Zero quantity has zero inventory value while the last reference cost remains available operationally.
- Posted sales costs are immutable snapshots and are not recalculated from future averages.
- COGS and variance journal creation, returns and non-recoverable tax/landed-cost allocation remain separate work.
