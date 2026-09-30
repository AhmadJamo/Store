# Products and recipes
> Status: IMPLEMENTED | Last reviewed: 2026-09-30

## Purpose and flow
Manages database-generated product code, optional barcode, raw/direct/prepared classification, product category, logistics measurements, handling requirements, future tracking policy, active state, POS/sales channel availability, prices and stock unit. `RecipesController` and `RecipeService` create immutable recipe versions for prepared products; saving a recipe automatically marks the product prepared-to-order and clears direct purchase price.

WMS-015A adds a tenant-owned attribute dictionary. Definitions have stable neutral codes, Text/Number/Boolean/Selection types, required and variant-defining flags, display order, optional category applicability and ordered selection options. Definitions are deactivated rather than deleted. Product value assignment and ProductTemplate variant grouping are deliberately deferred to the next WMS-015 slice, so this foundation does not change stock identity or generate SKUs.

Settings → Measurement Units manages a tenant-local catalogue of count, mass, volume and length units with base conversion factors and precision. Products reference an active managed stock unit. Recipe lines select a compatible managed unit and freeze both authored/stock unit codes and factors with the converted stock quantity, so later configuration changes cannot rewrite historical consumption. Legacy enum columns remain only as a compatibility bridge.

## Rules and dependencies
Name is required; barcode is optional and unique per company when supplied. Category is optional. Weight requires a Mass unit; gross weight cannot be lower than net weight. Length, width and height must be positive and entered together with a Length unit. Refrigerated and Frozen are mutually exclusive. Lot/Serial cannot be enabled for a product with any non-zero warehouse balance, and WMS-005 does not yet execute tracked receipts. Raw materials cannot use sales channels. Direct-sale products retain purchase/wholesale/retail validation, while prepared products have no direct purchase price. Product search covers name, code and barcode with type/category/tracking/status/channel filters, sorting and paging. Recipe lines require distinct stocked ingredients and compatible managed units from the same count/mass/volume dimension. Each product can explicitly allow controlled negative use only through recipe/kitchen workflows. Recipes reuse Products.View/Edit permissions. Product and recipe deletion remains restricted by operational history.

## Source files
`Domain/Entities/Catalog/{Product,ProductRecipe,RecipeIngredient,UnitOfMeasure,UnitConversion}.cs`; Product/Recipe DTOs and services; product/recipe repositories and configurations; Products/Recipes controllers and views.

## If this changes
Review product entity doc, inventory/purchase/sales/transfers docs, configuration/migration, controller/screens, permissions and map.
