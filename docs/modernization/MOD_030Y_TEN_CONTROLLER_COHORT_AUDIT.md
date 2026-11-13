# MOD-030Y ten-controller cohort audit

> Completed: 2026-11-14  
> Runtime change: none

## Selected cohort

| MVC surface | Angular route | API | Primary risk retained server-side |
|---|---|---|---|
| SupplierPayments | `/purchases/payments` | `/api/v1/supplier-payments` | financial posting |
| CustomerPayments | `/sales/payments` | `/api/v1/customer-payments` | posting and reversal |
| PurchaseReplenishment | `/purchases/replenishment` | `/api/v1/purchase-replenishment` | settings and planning run |
| ProductAttributes | `/products/attributes` | `/api/v1/product-attributes` | definitions and product values |
| ProductStocks | `/inventory/stocks` | `/api/v1/product-stocks` | stock master and manual adjustment boundary |
| SalesDeliveries | `/sales/deliveries` | `/api/v1/sales-deliveries` | reserve/post/cancel workflow |
| ProductTemplates | `/products/templates` | `/api/v1/product-templates` | assignment and variant generation |
| Replenishment | `/inventory/replenishment` | `/api/v1/replenishment` | transfer/request draft creation |
| Suppliers | `/suppliers` | `/api/v1/suppliers` | concurrency and referenced deletion |
| InventoryScanning | `/inventory/scanning` | `/api/v1/inventory-scanning` | putaway/relocation/count |

## Decision

The ten surfaces map their reads and commands to authenticated APIs using the same Application services as MVC. API
mutations preserve their existing permissions and antiforgery. Execution must retire MVC POSTs with 405 rather than
redirecting or replaying them, preserve positive identifiers in GET/HEAD redirects and run the complete Release,
regression, Angular, bundle and hosted-boundary gates.

Account, Subscription, Users and Platform surfaces are deliberately excluded because they cross authentication,
subscription or platform-session boundaries and require separate evidence.
