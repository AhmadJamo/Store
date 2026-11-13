# MOD-030R — Configuration-candidate MVC audit

> Status: COMPLETED — NON-DESTRUCTIVE — 2026-11-13

## Scope

Compared ProductCategories, PurchaseApprovalRules and SalesApprovalRules across MVC, internal API, Angular, Application
services, permissions, validation, antiforgery, concurrency and workflow impact. No controller, view, route, configuration
or business data changed.

| Candidate | Parity evidence | Risk boundary | Decision |
|---|---|---|---|
| ProductCategories | Angular/API and MVC all use `ProductCategoryService`; list/create/status preserve `Products.View/Edit`, antiforgery, localized validation and neutral-code rules | Tenant catalog configuration only; no stock, accounting, identity or document transition | Recommend as the next bounded cohort |
| PurchaseApprovalRules | Angular/API use `PurchaseApprovalService`, preserve View/Manage, antiforgery and rowversion | Active rules directly govern purchase-request submission and approval routing | Defer behind rule-resolution workflow acceptance |
| SalesApprovalRules | Angular/API use `SalesOrderApprovalService`, preserve View/Manage, antiforgery and rowversion | Active rules directly govern sales-order approval requirements and approver separation | Defer behind sales approval workflow acceptance |

Authenticated read-only probes returned 200 from all three APIs. The immediately preceding Edge run covered all three
legacy Index redirects among 147 permission-eligible GET routes. No POST, PUT or DELETE ran.

## Proposed MOD-030S scope

The following destructive scope is documented but **not approved by this audit**:

- delete `MiniStore.Web/Controllers/Catalog/ProductCategoriesController.cs`;
- delete `MiniStore.Web/Views/ProductCategories/Index.cshtml`;
- move `ProductCategories:Index` to the retired catalog at `/products/categories` with `Products.View`;
- remove the obsolete active cutover setting and registry mapping;
- retain `ProductCategoriesApiController`, `ProductCategoryService`, DTOs, repository, entity, Angular page,
  `Products.View/Edit`, antiforgery and localization;
- prove old Create/SetActive POST routes return 405 without mutation while API POST/PUT remain authoritative.

No physical deletion may start until this exact controller/one-view scope receives separate explicit approval.
