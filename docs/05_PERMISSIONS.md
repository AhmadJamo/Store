# Permissions and authorization
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-09-29

`InventoryReservations.View` protects the read-only inventory reservation history screen.

Inventory adjustments split `View`, `Create`, `Count`, `Approve`, `Post` and `Cancel` so operational counting and supervisory approval can be assigned separately.

Inventory tracking uses `InventoryTracking.View` for traceability, `InventoryTracking.Open` for the high-impact opening allocation/policy activation and `InventoryTracking.ManageQuarantine` for reasoned quarantine/release transitions.
`InventoryTracking.ManageRecall` controls recall creation and closure. Recall visibility follows `InventoryTracking.View`; closing a recall never grants or implies quarantine release authority.
`Inventory.Replenishment.View` protects reviewed replenishment suggestions; `Inventory.Replenishment.Manage` separately protects rule create/update and activation changes.
`InventoryInsights.View` protects the read-only slow/dead inventory activity report; it grants no stock mutation capability.
`InventoryScanning.View` protects the scan console. Scanned putaway additionally requires `ProductStock.Edit`; both policies are enforced on the POST action and the form is hidden without edit authority.
Scanned internal relocation additionally requires `LocationMovements.Create`; its POST action enforces both permissions and antiforgery, and uses a relocation-specific retry key.
Scanned Draft line counting additionally requires `InventoryAdjustments.Count`; it records an absolute line value only and grants no approval or posting authority.

Authentication is ASP.NET Core Identity cookie authentication. `PermissionAuthorizeAttribute` creates policies with `Permission:` prefix. Identity owns credentials, while `TenantRole`, `TenantRolePermission` and `TenantUserRole` own authorization inside each company. Both evaluators require an active tenant; Admin receives full access only from that company's protected Admin assignment. Non-Admin permissions resolve only through the active company's role definition.

## Defined permission groups
Products, Warehouses, ProductStock, StockTransactions, StockMovements, LocationMovements, InventoryReconciliation, Suppliers, Purchases, PurchaseReturns, Sales, SalesReturns, StockTransfers, Users, Roles, and Settings are defined in `MiniStore.Application/Permissions/PermissionDefinitions.cs`. CRUD permissions exist for several modules even where matching actions do not exist (for example purchase/sale edit/delete). Purchase and sales returns define View/Create. Stock transfer also defines Submit/Approve/Reject/Post/Cancel. Location movements define View/Create. Inventory reconciliation and the physical StockMovement pilot define View only.

## Enforcement map
| Module | Backend enforcement |
|---|---|
| Products, Warehouses, Suppliers, Product Stocks | View/Create/Edit/Delete controller actions have matching `PermissionAuthorize`. |
| Purchases | View/Create enforced; Edit/Delete definitions have no controller actions. |
| Purchase returns | View/Create enforced; Create also requires a posted original purchase and sufficient current stock. |
| Goods receipt returns | Separate View/Create permissions; Create accepts posted new-flow receipts only and revalidates remaining receipt quantity, current stock, tracking and accounting configuration. |
| Sales | View/Create enforced; Edit/Delete definitions have no controller actions. |
| Sales returns | View/Create enforced; the original-sale return action is also hidden without Create. |
| Stock transactions | View/Create enforced. |
| Location movements | View/Create enforced for history and internal relocation; putaway continues to require ProductStock.Edit. |
| Inventory reconciliation | Dedicated View permission protects the read-only exception report; the controller exposes no mutation action. |
| Inventory insights | Dedicated View permission protects the read-only activity report; thresholds and filters do not mutate stock. |
| Inventory scanning | View protects resolution; putaway adds ProductStock.Edit, relocation adds LocationMovements.Create and Draft line counting adds InventoryAdjustments.Count. Every mutation uses antiforgery; movement commands use scoped keys and counts use absolute state assignment. |
| Stock transfers | View/Create/Edit/Submit/Approve/Reject/Post/Cancel enforced. |
| Users | View/Create enforced; Edit/Delete definitions have no actions. |
| Roles | View/Create/Edit/Delete enforced; controller uses `TenantRoleService`, system Admin cannot be edited/deleted and all IDs are resolved inside the active tenant. |
| Settings | Requires `Administration.Access`, which only an Admin assignment in the active tenant can satisfy. |
| Accounting master data | Accounts, branches, customers, tax rates and payment methods require the same tenant-Admin boundary; navigation visibility remains separately gated. |

## Known inconsistencies
`Settings.View` and `Settings.Edit` remain mainly navigation capabilities. `Administration.Access` is an internal protected policy key and is absent from the delegable permission catalogue. Identity mutations (`Users.Create/Edit/Delete`, `Roles.Create/Edit/Delete`) require tenant Admin membership and are filtered from custom-role editing, regardless of stored data. Assignment validates an active company membership and a role owned by the same tenant.

## Update rules
Permission change → `PermissionDefinitions`, seeder, controller attributes, navigation/views, `05_PERMISSIONS.md`, related module/controller/screen docs and tests.

`InventoryBalances.View` protects the read-only Inventory Availability projection. Balance mutation remains owned by inventory workflows; no direct edit permission exists.

# PUR-010 supplier purchasing permissions

- `Purchases.SupplierTerms.View` allows viewing supplier/product purchasing data.
- `Purchases.SupplierTerms.Manage` allows create, edit, prefer, activate and deactivate commands.

The Index and mutation actions enforce these policies server-side. Permission seeding adds both definitions and grants them to protected tenant Admin roles through the existing idempotent seeder.

# PUR-025 purchase approval permissions

- `PurchaseApprovalRules.View` and `PurchaseApprovalRules.Manage` separate rule visibility from configuration changes.
- `PurchaseRequests.Approve` and `PurchaseRequests.Reject` separately protect decision commands.
- A decision requires both the matching command permission and membership in the tenant role frozen on the current approval step. Having only one of these authorities is insufficient.

# PUR-030 purchase sourcing permissions

- `PurchaseSourcing.View` protects sourcing-event listing and detail visibility.
- `PurchaseSourcing.Create` protects converting an approved request into exactly one sent supplier-invitation event.
- The command remains commercial intent only; it has no inventory or accounting permission because it cannot mutate either area.

# PUR-040 supplier quotation permissions

- `SupplierQuotations.View` protects quotation comparison visibility.
- `SupplierQuotations.Create` protects capture and submission of a supplier's invited response.
- `SupplierQuotations.Award` protects the one-time, reason-required supplier selection. It does not grant Purchase Order, receipt, stock or accounting authority.
- `PurchaseOrders.View` protects order list/detail visibility.
- `PurchaseOrders.Create` permits creating one order from the recorded quotation award only.
- `PurchaseOrders.Approve`, `PurchaseOrders.Confirm` and `PurchaseOrders.Cancel` separately protect the corresponding state transitions; confirmation still has no inventory or accounting effect.
- `GoodsReceipts.Create` protects both capture and posting of a Goods Receipt; it is limited to confirmed Purchase Orders and invokes the purchasing Application service. `GoodsReceipts.View` reserves separate receipt visibility for the upcoming history screen.
- `GoodsReceiptReturns.Create` protects capture and immediate posting of a receipt-based supplier return; `GoodsReceiptReturns.View` protects its list/detail history. These permissions are distinct from legacy `PurchaseReturns.*`.

