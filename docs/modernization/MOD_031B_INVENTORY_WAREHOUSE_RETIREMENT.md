# MOD-031B inventory and warehouse ten-controller retirement

> Completed: 2026-11-14

Retired LocationMovements, StockTransactions, UnassignedStock, InventoryAdjustments, InventoryTracking,
WarehouseLocations, Warehouses, StockTransfers, GoodsReceipts and PurchaseDocumentReversals as one bounded cohort.
Ten MVC controllers, 25 user-facing Razor views and ten Razor page-model companions were removed.

The retired-route catalog now owns 25 protected GET/HEAD entry points. Positive receipt/order identifiers are validated,
query strings are preserved and no legacy mutation is redirected or replayed. Authenticated Angular APIs continue through
the same Application services, permissions, antiforgery, UnitOfWork, inventory, valuation and accounting rules.

Verification: Release passed with zero warnings/errors; 1,656 security/inventory checks and 242/242 Angular tests passed;
137 templates and 120 feature pages passed consistency/accessibility checks; bundle budgets remained 337,191/358,400
initial and 81,877/81,920 largest lazy; generic RTL/LTR, keyboard and responsive acceptance passed. Live probes confirmed
25 protected legacy reads return 302, ten Angular workspaces return 200, ten anonymous APIs return 401 and one representative
legacy mutation per controller returns 405.

Active inventory is 45 MVC controllers, 263 actions, 143 Razor files and 127 user-facing views. Seventy-two protected
presentation actions across 37 surfaces are physically retired.
