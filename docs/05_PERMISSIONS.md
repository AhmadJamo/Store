# Permissions and authorization
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-09-13

Authentication is ASP.NET Core Identity cookie authentication. `PermissionAuthorizeAttribute` creates policies with `Permission:` prefix; `PermissionAuthorizationHandler` grants Admin all permissions, otherwise maps Identity role names → Identity role IDs → `RolePermissions` → `Permissions`.

## Defined permission groups
Products, Warehouses, ProductStock, StockTransactions, Suppliers, Purchases, Sales, StockTransfers, Users, Roles, and Settings are defined in `MiniStore.Application/Permissions/PermissionDefinitions.cs`. CRUD permissions exist for several modules even where matching actions do not exist (for example purchase/sale edit/delete). Stock transfer also defines Submit/Approve/Reject/Post/Cancel.

## Enforcement map
| Module | Backend enforcement |
|---|---|
| Products, Warehouses, Suppliers, Product Stocks | View/Create/Edit/Delete controller actions have matching `PermissionAuthorize`. |
| Purchases | View/Create enforced; Edit/Delete definitions have no controller actions. |
| Sales | View/Create enforced; Edit/Delete definitions have no controller actions. |
| Stock transactions | View/Create enforced. |
| Stock transfers | View/Create/Edit/Submit/Approve/Reject/Post/Cancel enforced. |
| Users | View/Create enforced; Edit/Delete definitions have no actions. |
| Roles | View/Create/Edit/Delete enforced. |
| Settings | Controller requires Identity role `Admin`, not Settings.* permissions. |

## Known inconsistencies
`Settings.View` and `Settings.Edit` are defined but unused. UI navigation visibility is defined separately in `NavigationDefinitions`; inspect it with each permission change. Identity mutations (`Users.Create/Edit/Delete`, `Roles.Create/Edit/Delete`) now require current database Admin membership in both permission evaluators, regardless of stored role mappings. Read permissions remain delegable. Navigation and buttons use the same evaluator.

## Update rules
Permission change → `PermissionDefinitions`, seeder, controller attributes, navigation/views, `05_PERMISSIONS.md`, related module/controller/screen docs and tests.

