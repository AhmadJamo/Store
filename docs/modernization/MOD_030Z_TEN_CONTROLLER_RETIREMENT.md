# MOD-030Z ten-controller retirement

> Completed: 2026-11-14

Retired SupplierPayments, CustomerPayments, PurchaseReplenishment, ProductAttributes, ProductStocks, SalesDeliveries,
ProductTemplates, Replenishment, Suppliers and InventoryScanning as one cohort. Ten controllers, 21 user-facing views and
their MVC-only partial/page-model files were removed. Exact protected GET/HEAD routes resolve to Angular. MVC mutations are
not redirected or replayed; authenticated APIs retain permissions, antiforgery and the same Application services.

Verification: Release zero warnings/errors; 1,725 regression checks; 242/242 Angular tests; 137 templates; bundle budgets
337,191/358,400 initial and 81,877/81,920 largest lazy; generic RTL/LTR, keyboard and responsive acceptance passed.
Live local probes confirmed all ten anonymous legacy GETs return 302 to sign-in, all ten Angular workspaces return 200,
all ten unauthenticated APIs return 401 and representative legacy mutations for every retired controller return 405.
