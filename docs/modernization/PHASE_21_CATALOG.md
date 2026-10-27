# MOD-021 — Catalog migration

> Status: COMPLETE — `MOD-021A..G` completed 2026-10-27
> Presentation default: MVC until an explicit cutover gate

## Scope and sequence

| Slice | Surface | Status |
|---|---|---|
| `MOD-021A` | Product list, details, create, edit and delete | COMPLETE |
| `MOD-021B` | Product-category create/list/activate/deactivate | COMPLETE |
| `MOD-021C` | Product attribute definitions, options and per-product values | COMPLETE |
| `MOD-021D` | Product templates and controlled variant creation | COMPLETE |
| `MOD-021E` | Prepared-product recipes and immutable recipe versions | COMPLETE |
| `MOD-021F` | Tax rates, payment methods and branch master data | COMPLETE |
| `MOD-021G` | Warehouse master data | COMPLETE |

Measurement-unit operations were already completed in `MOD-020L`; this wave reuses that Angular surface instead of
creating a duplicate unit editor.

## Non-negotiable rules

- Angular never accesses persistence and never receives EF/Domain entities. Every request crosses an explicit API DTO
  and the existing Application service.
- Product names, category names, supplier names and unit names are user-facing; numeric IDs remain transport values.
- Product code is database generated. Optional barcode is tenant-unique and search covers name, product code and barcode.
- Product type continues to own inventory behavior, purchasing rules, direct-sale channels and prepared-product cost
  behavior. Angular may guide the form, but the Domain/Application layer revalidates every rule.
- Unsafe operations require their independent permission and antiforgery validation. Tenant query/write guards remain
  authoritative.
- New presentation text is bilingual, RTL-safe and stored in localization resources. Styles remain external SCSS and
  use logical layout properties.
- Rejected mutations preserve user input and render normalized Problem Details. Delete never exposes database details.

## `MOD-021A` delivered behavior

- Existing server-paged product list retains name/code/barcode search and status filtering.
- Permission-aware Angular routes add product details, create and edit forms, unsaved-change protection and confirmed
  delete.
- The complete current MVC product contract is represented: type, managed stock unit, category, three prices, purchase
  billing/expense account, suppliers, channels, active state, tracking/shelf life, dimensions, weights, handling flags
  and controlled recipe-negative behavior.
- `ProductsApiController` delegates list/details/form/create/update/delete to `ProductService`; mutations use separate
  `Products.Create`, `Products.Edit` and `Products.Delete` permissions plus antiforgery validation.
- The Application-owned form-page DTO supplies only explicit unit, category, supplier and expense-account options.
  Details expose names rather than presenting internal IDs.
- MVC remains the rollback path and the committed presentation mode remains `Mvc`.

## Verification evidence

- Web Release: passed, zero warnings and zero errors.
- Security/inventory regression: 1204 checks passed.
- Angular: 150/150 tests across 51 files passed.
- Static accessibility: 60 templates passed; external-SCSS policy passed.
- Production bundle: 299,259 / 358,400 initial bytes; largest lazy chunk 80,866 / 81,920 bytes.

## `MOD-021B` delivered behavior

- Angular `/products/categories` provides the complete existing list, create and activate/deactivate operations.
- The page is read-only under `Products.View`; creation and status controls appear only with `Products.Edit`, while the
  API repeats both policies and validates antiforgery on every mutation.
- Category code is normalized server-side and constrained to language-neutral letters, numbers, hyphens and underscores.
  Duplicate name/code validation returns localized Problem Details and preserves the failed form.
- Deactivation retains historical rows and product references; no delete operation was invented because MVC/domain do
  not provide one.

## `MOD-021C` delivered behavior

- Angular `/products/attributes` provides definition list/create/activate/deactivate, typed Text/Number/Boolean/Selection
  rules, ordered selection options, required/variant flags and optional category applicability.
- Angular `/products/:id/attributes` renders the server-authoritative applicable fields and saves typed values while
  preserving rejected input. The Application service continues to validate required values, type parsing, selection
  ownership, category applicability and assigned-template variant refresh inside its transaction.
- Reads require `Products.View`; definition and per-product value operations require `Products.Edit`. Every mutation
  validates antiforgery, and the API returns explicit DTOs and localized Problem Details rather than domain entities.
- Product list/details expose permission-aware value links. MVC remains the rollback/default presentation.

## `MOD-021D` delivered behavior

- Angular `/products/templates` provides the complete template create, existing-SKU assignment/removal, grouped variant
  list, bounded combination preview and explicit generation operations.
- The browser submits only template/source/option identifiers. `ProductTemplateService` rechecks category equality,
  complete variant-defining values, neutral SHA-256 signatures, duplicate combinations, source membership and the
  50-combination bound, then recomputes the plan during generation.
- Generated products copy only the established safe commercial/unit/logistics fields and non-variant values. Barcode,
  stock, recipe and history are not copied; prepared-to-order and non-Selection automatic generation remain rejected.
- Reads require `Products.View`; create/assign/preview/generate require `Products.Edit` plus antiforgery. MVC remains
  the rollback/default presentation.

## `MOD-021E` delivered behavior

- Angular `/products/recipes` lists every prepared product and active recipe summary; `/products/recipes/:productId`
  provides dynamic ingredient rows, compatible unit filtering, yield, negative-policy guidance and dirty-form recovery.
- Saving creates a new immutable version through `RecipeService`; the server revalidates stocked ingredients, active
  units, matching dimensions, positive invariant quantities, duplicate/domain rules and transaction ownership.
- The browser displays product/unit names and never calculates converted stock quantity or recipe cost. Reads require
  `Products.View`; editor/save require `Products.Edit`, and save validates antiforgery.

## `MOD-021F` delivered behavior

- Angular `/products/catalog-masters` provides all current tax-rate, payment-method and branch list/create operations in
  one administration-protected surface without inventing edit/delete/status operations absent from MVC.
- Explicit Application DTOs replace entity exposure and project account code/name for tax and settlement choices.
  Tax percentage uses invariant decimal validation; branch codes are normalized language-neutral values.
- The API requires `Administration.Access` and antiforgery on each of the three mutations. Existing accounting/account
  existence, duplicate-name/code and domain rules remain server-owned.

## `MOD-021G` delivered behavior

- Angular `/warehouses` provides list/create/edit/delete with named branch and inventory-account options, all current
  operational type/control/picking/receiving/POS/capacity/location fields and dirty/rejected-form recovery.
- `WarehouseService` remains authoritative for SaaS limits, duplicate names, accounting assignments, unsafe Simple-mode
  transitions, POS-access dependencies and deletion constraints. Display DTOs now include names rather than exposing IDs.
- API reads/forms/mutations preserve independent `Warehouses.View/Create/Edit/Delete` permissions; every mutation uses
  antiforgery. No schema, stock balance or history behavior changed.

## Completion

Every approved `MOD-021A..G` slice reached current MVC functional parity with its own verification evidence. MVC remains
the presentation default until an explicit cutover gate; the modernization program proceeds to `MOD-022`.
