# MOD-030S — ProductCategories MVC retirement

> Status: COMPLETED — 2026-11-13

## Approved scope

- Removed `MiniStore.Web/Controllers/Catalog/ProductCategoriesController.cs`.
- Removed `MiniStore.Web/Views/ProductCategories/Index.cshtml`.
- Moved `ProductCategories:Index` to the protected retired-route catalog at `/products/categories`.
- Removed the obsolete active cutover setting and migration-registry mapping.

`ProductCategoriesApiController`, `ProductCategoryService`, DTOs, repository, entity, Angular page, localization,
`Products.View/Edit`, antiforgery and category validation remain unchanged.

## Runtime contract

- legacy GET/HEAD list URLs redirect to `/app/products/categories` under `Products.View` with query preservation;
- retired MVC Create and SetActive POST routes return 405 and are not replayed;
- API POST/PUT remain the only mutation paths and retain `Products.Edit` plus antiforgery.

## Verification

- Release: 0 warnings, 0 errors.
- Security/inventory regression: 1,840 checks.
- Angular: 242/242 tests across 116 files.
- Static accessibility: 137 templates; UI consistency: 120 feature pages.
- Bundle gates: 337,191 / 358,400 initial; 81,877 / 81,920 largest lazy.
- Edge: 147 authorized GET routes matched exact targets.
- Explicit live boundary: both retired ProductCategories POST routes returned 405 without mutation.

## Inventory

- 70 MVC controllers and 391 MVC actions.
- 202 total Razor files, including 184 user-facing views and 194 generated control rows.
- 15 retired presentation GET actions across 12 surfaces and 15 retired views.
