# Permissions and authorization
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-09-14

Authentication is ASP.NET Core Identity cookie authentication. `PermissionAuthorizeAttribute` creates policies with `Permission:` prefix. Both permission evaluators first require an active tenant and then query `TenantUserRoles`; Admin therefore receives full access only inside the active company. Non-Admin assignments resolve through the existing global role-permission templates.

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
| Roles | View/Create/Edit/Delete enforced. |
| Settings | Requires `Administration.Access`, which only an Admin assignment in the active tenant can satisfy. |
| Accounting master data | Accounts, branches, customers, tax rates and payment methods require the same tenant-Admin boundary; navigation visibility remains separately gated. |

## Known inconsistencies
`Settings.View` and `Settings.Edit` remain mainly navigation capabilities. `Administration.Access` is an internal protected policy key and is deliberately absent from the delegable permission catalogue. Identity mutations (`Users.Create/Edit/Delete`, `Roles.Create/Edit/Delete`) also require tenant Admin membership regardless of stored templates. Role definitions and non-Admin permission templates are still global, although assignment and Admin evaluation are tenant-scoped.

## Update rules
Permission change → `PermissionDefinitions`, seeder, controller attributes, navigation/views, `05_PERMISSIONS.md`, related module/controller/screen docs and tests.

