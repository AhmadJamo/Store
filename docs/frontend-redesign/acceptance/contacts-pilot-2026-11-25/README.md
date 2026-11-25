# FRD-G Contacts Customers/Suppliers acceptance

> Date: 2026-11-25  
> Result: Passed for the presentation and existing contract scope  
> Scope: Customer and Supplier List/Form/Record routes

## Parity evidence

| Requirement | Evidence | Result |
|---|---|---|
| Customer list | Shared FilterBar, SummaryStrip and typed DataTable preserve server search, archived filtering, supported sorting, paging and safe view preferences. | Passed by focused component coverage and source inspection. |
| Supplier list | The same list hierarchy preserves server search, inactive filtering, paging and safe view preferences. | Passed by focused component coverage and source inspection. |
| Forms | Shared FormSection, FieldCue and WorkflowFooter preserve every existing field, option source, internal ID, row version, dirty guard, command and recovery path. | Passed by Customer and Supplier form component coverage. |
| Records | Localized Contacts context and DetailSection preserve Customer profile/commercial/contacts/addresses and Supplier profile/commercial facts. Status, customer type/code and assigned-product count are visible without inventing actions or data. | Passed by Customer and Supplier details coverage, including retry. |
| Supplier identifier boundary | `SupplierDetails` exposes no supplier code. The redesigned record therefore does not present an internal ID or invent a business code. | Passed by API-model and template inspection. |
| Permissions | Customer reads/create/edit remain separated by `Customers.View`, `Customers.Create` and `Customers.Edit`; Supplier operations remain separated by `Suppliers.View`, `Suppliers.Create` and `Suppliers.Edit`. UI actions remain permission-aware and server authorization stays authoritative. | Passed by source inspection and 1,668 SecurityRegression checks. |
| Loading/empty/filtered-empty/error | Lists distinguish true empty and filtered empty; forms and records retain loading, validation, concurrency and normalized recovery states. | Passed by focused component coverage and static acceptance. |
| EN/AR, RTL/LTR and responsive layout | Shared localization contains both cultures and logical CSS is used. Browser acceptance passed direction switching and Desktop/390px responsive behavior. | Playwright 7 passed, 0 failed. |
| API/schema/business invariants | No API, DTO, schema, domain, Application or repository behavior changed in FRD-G. Account/product selections remain name-based with internal IDs. | Passed by diff and architecture review. |

## Verification snapshot

- Angular: 131 files, 282/282 tests passed.
- Production build: passed; the pre-existing 204.88 kB initial shared-chunk warning remains tracked as `FRD-ISS-002`.
- Static accessibility: 141 templates passed.
- UI consistency: 121 feature pages accounted for.
- Browser presentation: 7 passed, 0 failed, 3 authenticated-API checks skipped.
- Security and inventory regression: 1,668 checks passed in Release configuration.

`browser-acceptance.json` is the machine-readable browser result. Its three skipped checks require an externally supplied live API URL and authenticated storage state. This acceptance proves presentation, frontend contract and server permission parity, but does not claim a new live authenticated E2E run.

## Cutover and rollback

Customer and Supplier Angular routes and APIs predated FRD-G. This wave changed presentation only, without changing URLs or contracts. Rollback is deployment/Git rollback to the preceding Angular version.

## Decision

FRD-G is closed. Customers and Suppliers are migrated V4 Contacts surfaces. The next migration wave starts with a bounded Branches/Warehouses audit before any UI change.
