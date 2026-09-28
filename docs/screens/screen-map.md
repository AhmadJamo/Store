# Razor screen map
> Status: IMPLEMENTED | Last reviewed: 2026-09-28

| Screens | Controller flow / notable UI |
|---|---|
| `Views/Account/{Login,Register,AccessDenied}.cshtml` | tenant login and bilingual company/owner registration with plan selection, duplicate-submit protection and friendly rate-limit retry feedback. Register includes an accessible popup explaining the actual company name/address, username, email, password, plan and 14-day trial rules. |
| `Views/Public/*`, `_PublicLayout.cshtml` | anonymous bilingual landing and database-backed pricing experience. |
| `Views/Subscription/{Index,Checkout}.cshtml` | current subscription, plan selection, promo entry and immutable checkout quote. |
| `Areas/Platform/Views/*` | separately signed-in operator dashboard for plans, promotions, companies/subscriptions and pending payment confirmation; login displays friendly retry feedback when throttled. |
| `Views/Dashboard/Index.cshtml` | authenticated landing page after login; authenticated requests to Login redirect here. Home remains a compatibility redirect. |
| `Views/Products/{Index,Create,Edit}.cshtml` | localized product catalogue with name/code/barcode search, type/status/channel filters, sorting and paging. Forms configure raw/direct/prepared type, optional barcode, managed stock unit, active state, sale channels and controlled-negative ingredient policy; prepared purchase price is hidden and cleared. |
| `Views/Recipes/{Index,Edit}.cshtml` | bilingual prepared-product recipe list and dynamic immutable-version editor using active managed units, immutable conversion snapshots and visible negative policy. |
| `Views/Warehouses/*`, `Views/Suppliers/*` | list/create/edit master data and delete posts; warehouses configure operating use, location control, picking, POS and transfer policies. |
| `Views/ProductStocks/{Index,Create,Edit}.cshtml` | balance list/opening balance/direct quantity adjustment; negative recipe exceptions are highlighted for reconciliation. |
| `Views/StockTransactions/{Index,Create}.cshtml` | paged/filterable movement list and manual movement form, including reason-required kitchen variance. |
| `Views/WarehouseLocations/{Index,Create}.cshtml`, `Views/UnassignedStock/Index.cshtml` | location master data, warehouse/product search and putaway assignment. |
| `Views/LocationMovements/Index.cshtml` | same-warehouse rack/bin relocation form with filtered choices, available quantity and searchable movement history. |
| `Views/Purchases/{Index,Create,Details}.cshtml` | purchase list/detail and client-side dynamic item rows. |
| `Views/Sales/{Index,Create,Details,Pos}.cshtml` | wholesale sale, detail/list and POS cart; POS terminal selection filters warehouses, applies saved appearance and exposes only configured order types/context, guest count and item notes. Details shows product names and the persisted POS order context. |
| `Views/StockTransfers/{Index,Create,Edit,Details}.cshtml` | transfer list/edit/detail/status actions; client-side product rows/filtering. |
| `Views/Settings/{Index,DocumentNumbers,General,Discounts,Inventory}.cshtml` | Admin-only forms; DocumentNumbers centrally configures four tenant sequences with format tokens, padding, counters, reset policies and live preview; General selects the English/Arabic company default. |
| `Views/Settings/InventoryAccess.cshtml` | branch warehouse operation flags/priorities plus POS terminal creation and prioritized warehouse assignment. |
| `Views/Settings/Pos.cshtml` | per-terminal business profile, POS appearance and order workflow settings with preset application and live preview. |
| `Views/Settings/Units.cshtml` | tenant measurement-unit catalogue grouped by dimension; creates custom units and safely deactivates non-system units while showing factors and precision. |
| `Views/Users/{Index,Create}.cshtml`, `Views/Roles/{Index,Create,Edit,_Form}.cshtml` | bilingual company-local users and role/permission administration; protected roles are read-only. |
| `Views/Shared/{_Layout,_Notification,_ErrorPopup,_ConfirmDelete,_ValidationScriptsPartial}.cshtml` | localized LTR/RTL navigation and language switch, feedback, confirmation/validation partials. |

The `.cshtml.cs` files adjacent to views were included in the repository scan; custom role is Unknown / Not determined from code review and they should be inspected before editing/removed only after verifying compilation behaviour. Views use model binding; client validation is usability only and service-side rules remain authoritative.

## UI change rules
Button/form → inspect its JavaScript/hiddens → action → permission → DTO/service → database effect. Update screen map, relevant module/controller docs, navigation and permissions where applicable.

## Delete forms (2026-09-13)
Products, Warehouses, Suppliers, ProductStocks and Roles index delete controls use POST forms with generated antiforgery tokens and confirmation. Identity mutation buttons use the Admin-only permission rule through the existing PermissionService.

## Company setup
/onboarding is a bilingual, responsive post-registration screen with business-type cards, country/currency/language/fiscal choices, tax and POS switches, inventory control level, transactional Apply and explicit manual-setup Skip.
