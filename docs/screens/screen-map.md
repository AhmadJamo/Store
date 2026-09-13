# Razor screen map
> Status: IMPLEMENTED | Last reviewed: 2026-09-13

| Screens | Controller flow / notable UI |
|---|---|
| `Views/Account/{Login,AccessDenied}.cshtml` | Account login form/logout navigation; Identity flow. |
| `Views/Home/Index.cshtml` | authenticated home page. |
| `Views/Products/{Index,Create,Edit}.cshtml` | product search/list and CRUD forms. |
| `Views/Warehouses/*`, `Views/Suppliers/*` | list/create/edit master data and delete posts. |
| `Views/ProductStocks/{Index,Create,Edit}.cshtml` | balance list/opening balance/direct quantity adjustment. |
| `Views/StockTransactions/{Index,Create}.cshtml` | paged/filterable movement list and manual movement form. |
| `Views/Purchases/{Index,Create,Details}.cshtml` | purchase list/detail and client-side dynamic item rows. |
| `Views/Sales/{Index,Create,Details,Pos}.cshtml` | wholesale sale, detail/list and POS cart; client JavaScript constructs item hidden inputs. |
| `Views/StockTransfers/{Index,Create,Edit,Details}.cshtml` | transfer list/edit/detail/status actions; client-side product rows/filtering. |
| `Views/Settings/{Index,Invoices,General,Discounts}.cshtml` | Admin-only setting forms. |
| `Views/Users/{Index,Create}.cshtml`, `Views/Roles/{Index,Create,Edit}.cshtml` | Identity user/role administration. |
| `Views/Shared/{_Layout,_Notification,_ErrorPopup,_ConfirmDelete,_ValidationScriptsPartial}.cshtml` | global navigation, feedback, confirmation/validation partials. |

The `.cshtml.cs` files adjacent to views were included in the repository scan; custom role is Unknown / Not determined from code review and they should be inspected before editing/removed only after verifying compilation behaviour. Views use model binding; client validation is usability only and service-side rules remain authoritative.

## UI change rules
Button/form → inspect its JavaScript/hiddens → action → permission → DTO/service → database effect. Update screen map, relevant module/controller docs, navigation and permissions where applicable.

## Delete forms (2026-09-13)
Products, Warehouses, Suppliers, ProductStocks and Roles index delete controls use POST forms with generated antiforgery tokens and confirmation. Identity mutation buttons use the Admin-only permission rule through the existing PermissionService.
