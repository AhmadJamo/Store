# Permissions and authorization
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-09-29

`InventoryReservations.View` protects the read-only inventory reservation history screen.

Inventory adjustments split `View`, `Create`, `Count`, `Approve`, `Post` and `Cancel` so operational counting and supervisory approval can be assigned separately.

Inventory tracking uses `InventoryTracking.View` for traceability and `InventoryTracking.Open` for the high-impact opening allocation/policy activation.

Authentication is ASP.NET Core Identity cookie authentication. `PermissionAuthorizeAttribute` creates policies with `Permission:` prefix. Identity owns credentials, while `TenantRole`, `TenantRolePermission` and `TenantUserRole` own authorization inside each company. Both evaluators require an active tenant; Admin receives full access only from that company's protected Admin assignment. Non-Admin permissions resolve only through the active company's role definition.

## Defined permission groups
Products, Warehouses, ProductStock, StockTransactions, StockMovements, LocationMovements, InventoryReconciliation, Suppliers, Purchases, PurchaseReturns, Sales, SalesReturns, StockTransfers, Users, Roles, and Settings are defined in `MiniStore.Application/Permissions/PermissionDefinitions.cs`. CRUD permissions exist for several modules even where matching actions do not exist (for example purchase/sale edit/delete). Purchase and sales returns define View/Create. Stock transfer also defines Submit/Approve/Reject/Post/Cancel. Location movements define View/Create. Inventory reconciliation and the physical StockMovement pilot define View only.

## Enforcement map
| Module | Backend enforcement |
|---|---|
| Products, Warehouses, Suppliers, Product Stocks | View/Create/Edit/Delete controller actions have matching `PermissionAuthorize`. |
| Purchases | View/Create enforced; Edit/Delete definitions have no controller actions. |
| Purchase returns | View/Create enforced; Create also requires a posted original purchase and sufficient current stock. |
| Sales | View/Create enforced; Edit/Delete definitions have no controller actions. |
| Sales returns | View/Create enforced; the original-sale return action is also hidden without Create. |
| Stock transactions | View/Create enforced. |
| Location movements | View/Create enforced for history and internal relocation; putaway continues to require ProductStock.Edit. |
| Inventory reconciliation | Dedicated View permission protects the read-only exception report; the controller exposes no mutation action. |
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

