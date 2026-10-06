# Razor screen map
> Status: IMPLEMENTED | Last reviewed: 2026-09-30

| Screens | Controller flow / notable UI |
|---|---|
| `Views/Account/{Login,Register,AccessDenied}.cshtml` | tenant login and bilingual company/owner registration with plan selection, duplicate-submit protection and friendly rate-limit retry feedback. Register includes an accessible popup explaining the actual company name/address, username, email, password, plan and 14-day trial rules. |
| `Views/Public/*`, `_PublicLayout.cshtml` | anonymous bilingual landing and database-backed pricing experience. |
| `Views/Subscription/{Index,Checkout}.cshtml` | current subscription, plan selection, promo entry and immutable checkout quote. |
| `Areas/Platform/Views/*` | separately signed-in operator dashboard for plans, promotions, companies/subscriptions and pending payment confirmation; login displays friendly retry feedback when throttled. |
| `Views/Dashboard/Index.cshtml` | authenticated landing page after login; authenticated requests to Login redirect here. Home remains a compatibility redirect. |
| `Views/Products/{Index,Create,Edit,_LogisticsFields}.cshtml` | localized catalogue with category/tracking/type/status/channel filters. Forms configure product type, category, weight, dimensions, handling, future tracking policy, optional barcode, managed units, active state, channels and controlled-negative ingredient policy. |
| `Views/ProductCategories/Index.cshtml` | category create/list/activate screen with language-neutral codes and preserved inactive categories. |
| `Views/Recipes/{Index,Edit}.cshtml` | bilingual prepared-product recipe list and dynamic immutable-version editor using active managed units, immutable conversion snapshots and visible negative policy. |
| `Views/Warehouses/*`, `Views/Suppliers/*` | list/create/edit master data and delete posts; warehouses configure operating use, location control, picking, POS and transfer policies. |
| `Views/ProductStocks/{Index,Create,Edit}.cshtml` | balance, moving average and inventory value list; opening balance/direct adjustment; negative recipe exceptions are highlighted. |
| `Views/StockTransactions/{Index,Create}.cshtml` | paged/filterable movement list with unit cost, movement value and variance; manual adjustments include reason-required kitchen variance. |
| `Views/WarehouseLocations/{Index,Create,Edit}.cshtml`, `Views/UnassignedStock/Index.cshtml` | parent-indented location master data plus warehouse/product search, putaway-rule administration and capacity-aware destination suggestions before capability-validated putaway. |
| `Views/LocationMovements/Index.cshtml` | same-warehouse rack/bin relocation form with filtered choices, available quantity and searchable movement history. |
| `Views/InventoryReconciliation/Index.cshtml` | bilingual read-only WMS exception report comparing warehouse quantity/value, assigned/unassigned location quantity and the latest movement snapshot, with warehouse/search/exception/page filters. |
| `Views/ProductAttributes/{Index,Values}.cshtml` | bilingual definition administration plus per-product typed value assignment with required/category/option validation. |
| `Views/ProductTemplates/Index.cshtml` | bilingual template creation, existing-SKU assignment and grouped variant listing. |
| `Views/Purchases/{Index,Create,Details}.cshtml` | purchase list/detail and client-side dynamic item rows. |
| `Views/Sales/{Index,Create,Details,Pos}.cshtml` | wholesale sale, detail/list and POS cart; optional inclusive/exclusive invoice tax is selected and previewed, while terminal selection filters warehouses and applies saved order experience. Wholesale/POS tracked direct lines can submit an optional audited lot/serial selection. Details shows tax, product names, POS context, unit cost/COGS and an idempotent general-ledger posting action/status. |
| `Views/SalesReturns/{Index,Create,Details}.cshtml` | return history, remaining-quantity entry from a posted sale, and posted refund/restock details. |
| `Views/StockTransfers/{Index,Create,Edit,Details}.cshtml` | transfer list/edit/detail/status actions; client-side product rows/filtering. |
| `Views/Settings/{Index,DocumentNumbers,General,Discounts,Inventory}.cshtml` | Admin-only forms; DocumentNumbers centrally configures four tenant sequences with format tokens, padding, counters, reset policies and live preview; General selects the English/Arabic company default. |
| `Views/Settings/InventoryAccess.cshtml` | branch warehouse operation flags/priorities plus POS terminal creation and prioritized warehouse assignment. |
| `Views/Settings/Pos.cshtml` | per-terminal business profile, POS appearance and order workflow settings with preset application and live preview. |
| `Views/Settings/Units.cshtml` | tenant measurement-unit catalogue grouped by dimension; creates custom units and safely deactivates non-system units while showing factors and precision. |
| `Views/Settings/FiscalPeriods.cshtml` | Admin-only period creation and status management with non-overlap validation, reason capture and concurrency protection. |
| `Views/PurchaseReturns/{Index,Create,Details}.cshtml` | bilingual supplier-return history, original-line remaining quantity entry and immutable posted return details. |
| `Views/Users/{Index,Create}.cshtml`, `Views/Roles/{Index,Create,Edit,_Form}.cshtml` | bilingual company-local users and role/permission administration; protected roles are read-only. |
| `Views/Shared/{_Layout,_Notification,_ErrorPopup,_ConfirmDelete,_ValidationScriptsPartial}.cshtml` | localized LTR/RTL navigation and language switch, feedback, confirmation/validation partials. |

The `.cshtml.cs` files adjacent to views were included in the repository scan; custom role is Unknown / Not determined from code review and they should be inspected before editing/removed only after verifying compilation behaviour. Views use model binding; client validation is usability only and service-side rules remain authoritative.

## UI change rules
Button/form → inspect its JavaScript/hiddens → action → permission → DTO/service → database effect. Update screen map, relevant module/controller docs, navigation and permissions where applicable.

## Delete forms (2026-09-13)
Products, Warehouses, Suppliers, ProductStocks and Roles index delete controls use POST forms with generated antiforgery tokens and confirmation. Identity mutation buttons use the Admin-only permission rule through the existing PermissionService.

## Company setup
/onboarding is a bilingual, responsive post-registration screen with business-type cards, country/currency/language/fiscal choices, tax and POS switches, inventory control level, transactional Apply and explicit manual-setup Skip.
## WMS-015D Product Templates
The Product Templates screen supports a two-step selection-variant workflow: choose an assigned source SKU and options, inspect the bounded preview, then explicitly create missing products. Existing combinations are labelled and skipped.
## Physical Stock Movements
Bilingual read-only ledger showing Putaway/Relocation plus Planned, Posted or Reversed transfer outbound/transit/inbound stages, product, warehouse, locations, quantity and reference. It intentionally reports historic legacy rows without backfilling them.
## Inventory Availability
Bilingual read-only Product/Warehouse/Location projection showing OnHand, Reserved and Available. The system-owned Unassigned row is visibly distinguished and cannot be managed as a normal storage location.
## Inventory reservations

`InventoryReservations/Index` is a bilingual, permission-protected audit list of source reference, lifecycle status, line count, total quantity and create/close times.

## Inventory adjustments
`InventoryAdjustments/Index`, `Create` and `Details` implement the bilingual count workflow. Blind Draft documents hide expected quantities; later states expose expected, counted and variance values plus allowed workflow actions.

## Lot and serial tracking
`InventoryTracking/Index` shows current lot/serial positions, expiration highlighting and immutable history. Its opening form captures all dimensional allocations required to activate tracking on legacy stock.
