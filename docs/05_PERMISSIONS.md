# Permissions and authorization
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-09-15

Authentication is ASP.NET Core Identity cookie authentication. `PermissionAuthorizeAttribute` creates policies with `Permission:` prefix. Identity owns credentials, while `TenantRole`, `TenantRolePermission` and `TenantUserRole` own authorization inside each company. Both evaluators require an active tenant; Admin receives full access only from that company's protected Admin assignment. Non-Admin permissions resolve only through the active company's role definition.

## Defined permission groups
Products, Warehouses, ProductStock, StockTransactions, LocationMovements, Suppliers, Purchases, Sales, StockTransfers, Users, Roles, and Settings are defined in `MiniStore.Application/Permissions/PermissionDefinitions.cs`. CRUD permissions exist for several modules even where matching actions do not exist (for example purchase/sale edit/delete). Stock transfer also defines Submit/Approve/Reject/Post/Cancel. Location movements define View/Create.

## Enforcement map
| Module | Backend enforcement |
|---|---|
| Products, Warehouses, Suppliers, Product Stocks | View/Create/Edit/Delete controller actions have matching `PermissionAuthorize`. |
| Purchases | View/Create enforced; Edit/Delete definitions have no controller actions. |
| Sales | View/Create enforced; Edit/Delete definitions have no controller actions. |
| Stock transactions | View/Create enforced. |
| Location movements | View/Create enforced for history and internal relocation; putaway continues to require ProductStock.Edit. |
| Stock transfers | View/Create/Edit/Submit/Approve/Reject/Post/Cancel enforced. |
| Users | View/Create enforced; Edit/Delete definitions have no actions. |
| Roles | View/Create/Edit/Delete enforced; controller uses `TenantRoleService`, system Admin cannot be edited/deleted and all IDs are resolved inside the active tenant. |
| Settings | Requires `Administration.Access`, which only an Admin assignment in the active tenant can satisfy. |
| Accounting master data | Accounts, branches, customers, tax rates and payment methods require the same tenant-Admin boundary; navigation visibility remains separately gated. |

## Known inconsistencies
`Settings.View` and `Settings.Edit` remain mainly navigation capabilities. `Administration.Access` is an internal protected policy key and is absent from the delegable permission catalogue. Identity mutations (`Users.Create/Edit/Delete`, `Roles.Create/Edit/Delete`) require tenant Admin membership and are filtered from custom-role editing, regardless of stored data. Assignment validates an active company membership and a role owned by the same tenant.

## Update rules
Permission change → `PermissionDefinitions`, seeder, controller attributes, navigation/views, `05_PERMISSIONS.md`, related module/controller/screen docs and tests.

