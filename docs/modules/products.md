# Products and recipes
> Status: IMPLEMENTED | Last reviewed: 2026-10-27

## Angular migration foundation (2026-10-25)

`MOD-021A` completes product-master parity in Angular: server-paged list/search, details, create, edit and confirmed
delete. The API delegates to `ProductService`, separates View/Create/Edit/Delete permissions and validates antiforgery
on every mutation. The form includes all current MVC fields and server-provided category, managed-unit, supplier and
expense-account options; user-facing details show names while IDs remain transport-only. `MOD-021B` completes category
list/create/activate/deactivate with neutral-code validation and preserved inactive rows. `MOD-021C` completes attribute
definition/options/status and typed per-product values with category applicability and variant refresh remaining
server-authoritative. `MOD-021D` completes template creation, SKU assignment, preview and bounded controlled variant
generation without copying stock/history/barcode/recipes. `MOD-021E` completes prepared-product recipe list/edit and
immutable-version save while conversion remains server-owned. Measurement-unit operations are already Angular-owned
from `MOD-020L`. `MOD-021F` also completes tax-rate, payment-method and branch list/create operations through explicit
DTOs and named account choices. MVC remains the committed default. See `modernization/PHASE_21_CATALOG.md`.

`MOD-021G` completes warehouse list/create/edit/delete in Angular with branch/account names and all current operational,
control, picking, receiving, POS, capacity and transfer-location settings. Server-side entitlement and inventory guards
remain authoritative. The complete `MOD-021A..G` catalog wave is now functionally complete.

Recipe yields, ingredient quantities and measurement-unit conversion factors use culture-invariant decimal validation, so Arabic UI culture cannot turn valid dot-based HTML values into a validation exception.

## Purpose and flow
Manages database-generated product code, optional barcode, raw/direct/prepared classification, product category, logistics measurements, handling requirements, future tracking policy, active state, POS/sales channel availability, prices and stock unit. `RecipesController` and `RecipeService` create immutable recipe versions for prepared products; saving a recipe automatically marks the product prepared-to-order and clears direct purchase price.

WMS-015 adds a tenant-owned attribute dictionary. Definitions have stable neutral codes, Text/Number/Boolean/Selection types, required and variant-defining flags, display order, optional category applicability and ordered selection options. Definitions are deactivated rather than deleted. Each Product can store one typed value per applicable definition; required, option ownership and category applicability are revalidated on every save. Product remains the stock identity and templates never carry balances.

WMS-015C/D add optional ProductTemplate grouping for existing concrete Product SKUs and controlled variant creation. A template has one category; assignment requires the same category and values for every active applicable variant-defining attribute. The service builds a stable SHA-256 signature from neutral definition/option codes and prevents duplicate combinations inside a template. For selection-based attributes, users choose an assigned source SKU, preview at most 50 combinations, then explicitly create only missing variants. The server recomputes the plan during creation. New products copy safe commercial, unit and logistics configuration plus non-variant attribute values, but never copy barcode, stock, recipe or history. Prepared-to-order products and non-selection automatic variants remain manual.

Settings → Measurement Units manages a tenant-local catalogue of count, mass, volume and length units with base conversion factors and precision. Products reference an active managed stock unit. Recipe lines select a compatible managed unit and freeze both authored/stock unit codes and factors with the converted stock quantity, so later configuration changes cannot rewrite historical consumption. Legacy enum columns remain only as a compatibility bridge.

## Rules and dependencies
Direct-sale and raw-material forms can select multiple active suppliers. These selections use the same `SupplierProductPurchasingInfo` relationship as the Supplier form, so both screens stay synchronized without duplicate association data. Removing a supplier deactivates and preserves prior purchasing terms. Prepared-to-order products cannot be supplier-assigned because their ingredients are purchased instead.

Name is required; barcode is optional and unique per company when supplied. Category is optional. Weight requires a Mass unit; gross weight cannot be lower than net weight. Length, width and height must be positive and entered together with a Length unit. Refrigerated and Frozen are mutually exclusive. Lot/Serial cannot be enabled for a product with any non-zero warehouse balance, and WMS-005 does not yet execute tracked receipts. Raw materials cannot use sales channels. Direct-sale products retain purchase/wholesale/retail validation, while prepared products have no direct purchase price. Product search covers name, code and barcode with type/category/tracking/status/channel filters, sorting and paging. Recipe lines require distinct stocked ingredients and compatible managed units from the same count/mass/volume dimension. Each product can explicitly allow controlled negative use only through recipe/kitchen workflows. Recipes reuse Products.View/Edit permissions. Product and recipe deletion remains restricted by operational history.

## Shelf-life policy

Tracked products may define an optional default shelf life in days, require an expiration date on receipt and set their own expiration-warning horizon. Shelf-life controls are rejected when lot/serial tracking is disabled. When a receipt omits expiration and a default duration exists, expiration is derived from manufacture date, or from the receipt day when manufacture date is absent.

## Source files
`Domain/Entities/Catalog/{Product,ProductRecipe,RecipeIngredient,UnitOfMeasure,UnitConversion}.cs`; Product/Recipe DTOs and services; product/recipe repositories and configurations; Products/Recipes controllers and views.

## If this changes
Review product entity doc, inventory/purchase/sales/transfers docs, configuration/migration, controller/screens, permissions and map.

## Angular cutover update (2026-10-31)

MOD-030B makes the allowlisted Product list/create/edit GET routes AngularDefault after hosted acceptance and a successful
MVC rollback rehearsal. Query strings and positive IDs are preserved; mutations and unknown MVC actions are never redirected.
The Razor controller/views remain available as the rollback path.
