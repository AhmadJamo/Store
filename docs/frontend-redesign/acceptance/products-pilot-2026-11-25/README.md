# FRD-F Products List/Form/Record acceptance

> Date: 2026-11-25  
> Result: Passed for the presentation and existing contract scope  
> Scope: `/products`, `/products/new`, `/products/:id/edit`, `/products/:id`

## Parity evidence

| Requirement | Evidence | Result |
|---|---|---|
| Name, product-code and barcode search | `ProductListPageComponent` sends the debounced `search` query; `ProductApiService` preserves it; `ProductRepository.SearchAsync` matches `Name`, `ProductCode` and nullable `Barcode`. | Passed by source/contract inspection and Angular API spec. |
| Paging and active-state filter | The page-context allowlist owns `page`, `search` and `isActive`; the API client sends bounded page size 25 and the server DTO keeps paging/sort/filter inputs. | Passed by component and API specs. |
| Create/Edit/Delete | Dedicated form GETs and POST/PUT/DELETE contracts remain unchanged. Form dirty guard, save lock, server recovery and delete confirmation remain active. | Passed by focused component/API specs. |
| Permissions | Reads use `Products.View`; create form/write use `Products.Create`; edit form/write use `Products.Edit`; delete uses `Products.Delete`; every mutation keeps antiforgery. UI actions remain permission-aware. | Passed by source inspection and 1,668 SecurityRegression checks. |
| Loading/empty/filtered-empty/error | List has distinct loading, empty, filtered-empty, clear-filter and API recovery states; Form and Record keep loading, validation and recovery states. | Passed by focused component coverage and static acceptance. |
| Form validation | Existing validators and business rules are unchanged. Cross-field failures for weight, dimensions, tracking and expense account now resolve to bilingual focusable fields. | Passed by six Product Form component tests. |
| Record hierarchy | Localized catalog/status/type/category context, one primary Edit action, secondary navigation and a responsive two-column detail sheet preserve existing data and routes. | Passed by Product Details component coverage. |
| EN/AR, RTL/LTR and responsive layout | Shared localization contains both cultures; logical CSS is used; browser acceptance passed direction switching and Desktop/390px responsive behavior. | Playwright 7 passed, 0 failed. |
| API/schema/business invariants | No API, DTO, schema, domain, Application or repository behavior changed in FRD-F. SalePrice authority and name-based selections remain intact. | Passed by diff and architecture review. |

## Verification snapshot

- Angular: 127 files, 274/274 tests passed.
- Production build: passed; the pre-existing 204.88 kB initial shared-chunk warning remains tracked as `FRD-ISS-002`.
- Static accessibility: 140 templates passed.
- UI consistency: 121 feature pages accounted for.
- Browser presentation: 7 passed, 0 failed, 3 authenticated-API checks skipped.
- Security and inventory regression: 1,668 checks passed in Release configuration.

`browser-acceptance.json` is the machine-readable browser result. Its three skipped checks require an externally supplied live API URL and authenticated storage state. This acceptance therefore proves presentation, frontend contract and server permission parity, but does not claim a new live authenticated E2E run.

## Cutover and rollback

The Product Angular routes and APIs predated FRD-F; this slice changed their presentation without changing URLs or contracts. Legacy Product Razor presentation was already retired under `MOD-031C`, so rollback is deployment/Git rollback to the preceding Angular version, not a hidden MVC switch.

## Decision

FRD-F is closed. Products is the first migrated V4 module surface. The next migration wave is Contacts, starting with a bounded Customers/Suppliers audit before any UI change.
