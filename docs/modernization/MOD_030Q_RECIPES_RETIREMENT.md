# MOD-030Q — Recipes MVC retirement

> Status: COMPLETED — 2026-11-13

## Approved scope

After explicit approval, the bounded Recipes presentation cohort was retired:

- removed `MiniStore.Web/Controllers/Catalog/RecipesController.cs`;
- removed `MiniStore.Web/Views/Recipes/Index.cshtml`;
- removed `MiniStore.Web/Views/Recipes/Edit.cshtml`;
- moved `Recipes:Index` and `Recipes:Edit` from reversible cutover to the protected retired-route catalog;
- removed the obsolete `Recipes=AngularDefault` setting and active registry mapping.

`RecipesApiController`, `RecipeService`, DTOs, repositories, domain entities, Angular pages, localization, permissions and
immutable-version behavior remain authoritative and unchanged.

## Compatibility contract

- `GET/HEAD /Recipes` and `/Recipes/Index` redirect to `/app/products/recipes` under `Products.View`.
- `GET/HEAD /Recipes/Edit?productId={positiveId}` moves the identifier to
  `/app/products/recipes/{positiveId}` under `Products.Edit`, removes `productId` from the query and preserves unrelated
  query values.
- missing, zero or negative recipe identifiers return 404 instead of producing an unsafe redirect.
- the retired MVC save POST is not redirected and returns 405; the antiforgery-protected API PUT remains the only save path.

## Verification

- Release build: 0 warnings, 0 errors.
- Security/inventory regression: 1,842 checks passed.
- Angular tests: 242/242 across 116 files.
- Static accessibility: 137 templates.
- UI consistency: 120 feature pages.
- Bundle gates: 337,191 / 358,400 initial bytes; 81,877 / 81,920 largest lazy bytes.
- Microsoft Edge: 147 permission-eligible GET routes matched exact Angular targets.
- Live retirement boundary: unsafe recipe ID returned 404 and legacy POST returned 405 without mutation.

## Inventory after retirement

- 71 MVC controllers.
- 394 MVC actions.
- 203 total Razor files, including 185 user-facing views and 195 generated Razor control rows.
- 14 mapped retired presentation GET actions across 11 surfaces and 14 retired views.
