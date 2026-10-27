# MOD-021 — Catalog migration

> Status: IN PROGRESS — `MOD-021A..C` completed 2026-10-27
> Presentation default: MVC until an explicit cutover gate

## Scope and sequence

| Slice | Surface | Status |
|---|---|---|
| `MOD-021A` | Product list, details, create, edit and delete | COMPLETE |
| `MOD-021B` | Product-category create/list/activate/deactivate | COMPLETE |
| `MOD-021C` | Product attribute definitions, options and per-product values | COMPLETE |
| `MOD-021D` | Product templates and controlled variant creation | NEXT |
| `MOD-021E` | Prepared-product recipes and immutable recipe versions | PLANNED |
| `MOD-021F` | Tax rates, payment methods and branch master data | PLANNED |
| `MOD-021G` | Warehouse master data | PLANNED |

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
- Security/inventory regression: 1192 checks passed.
- Angular: 137/137 tests across 43 files passed.
- Static accessibility: 55 templates passed; external-SCSS policy passed.
- Production bundle: 298,342 / 358,400 initial bytes; largest lazy chunk 80,866 / 81,920 bytes.

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

## Remaining work

Proceed with `MOD-021D` product templates and controlled variants. Do not declare the catalog wave complete until every listed slice reaches
functional parity and its own verification evidence is recorded.
