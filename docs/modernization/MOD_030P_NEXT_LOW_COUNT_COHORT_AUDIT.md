# MOD-030P — Next low-count MVC cohort audit

> Status: COMPLETED — NON-DESTRUCTIVE — 2026-11-13

## Scope and outcome

Compared the remaining low-count mixed MVC candidates against their actual MVC controllers and views, internal APIs,
Angular routes/components, Application services, permissions and browser acceptance coverage. No controller, view, route,
configuration, data or business state was changed.

| Candidate | Replacement evidence | Risk boundary | Decision |
|---|---|---|---|
| Recipes | Angular list/editor use `RecipesApiController` and the same `RecipeService`; GET, editor load and immutable-version save preserve `Products.View/Edit`, antiforgery and server validation | Configuration mutation creates a new immutable recipe version but does not post stock or accounting entries | Recommend as the next bounded retirement cohort |
| StockTransactions | Angular list and manual-adjustment APIs exist | POST changes on-hand stock and valuation evidence | Defer until mutation replay/rejection evidence is captured |
| Subscription | Angular page and checkout quote API exist | Session/plan/billing boundary; checkout changes commercial state | Defer behind subscription-specific acceptance |
| Users | Angular list/create flow and API exist | Identity creation, password policy, role assignment and plan limits | Defer behind identity-specific acceptance |

The existing authenticated Edge run covered the legacy Recipes list and positive-product editor redirect probes as part of
the 147 permission-eligible GET routes. The full modernization verification completed immediately before this audit with
a clean Release build, 1,845 backend checks, 242/242 Angular tests, 137 templates and the 120-page consistency inventory.

## Proposed next cohort — MOD-030Q

The following destructive scope is **not approved by this audit**:

- delete `MiniStore.Web/Controllers/Catalog/RecipesController.cs`;
- delete `MiniStore.Web/Views/Recipes/Index.cshtml`;
- delete `MiniStore.Web/Views/Recipes/Edit.cshtml`;
- move protected legacy GET/HEAD routes to the retired-route catalog:
  - `Recipes:Index` → `/products/recipes` with `Products.View`;
  - `Recipes:Edit` → `/products/recipes/{productId}` with `Products.Edit`;
- remove the obsolete active cutover entries/configuration while retaining `RecipesApiController`, `RecipeService`, DTOs,
  repositories, Angular pages and all domain behavior;
- verify that the retired MVC POST is no longer reachable while the antiforgery-protected API PUT remains authoritative.

No deletion may begin until this exact controller/two-view scope receives separate explicit approval.
