# MOD-007 — Complete Suppliers Angular slice

> Status: COMPLETED  
> Completed: 2026-10-24  
> Database change: none

## Outcome

Suppliers are the second complete Angular business slice. The Angular workspace now provides a permission-aware,
server-paged list plus details, create and edit routes while the existing MVC screens remain available as the
rollback path. All component styling remains in separate SCSS files.

## Application and API boundary

`SupplierService` remains the use-case authority. Its form-page contract supplies the current supplier, active
accounts and eligible active non-prepared products. Browser-provided identifiers are never trusted as authority:
create/update reload and validate the account and apply product assignments through the existing
`SupplierProductAssignmentService` inside the existing transaction boundary.

`SuppliersApiController` exposes operation-specific contracts under `/api/v1/suppliers`:

- `GET /` and `GET /{id}` require `Suppliers.View`;
- `GET /form` and `POST /` require `Suppliers.Create`;
- `GET /{id}/form` and `PUT /{id}` require `Suppliers.Edit`.

Unsafe requests retain the global CSRF filter. Validation is returned as Problem Details. A stale row version is
represented by `SupplierConcurrencyException` and returned as HTTP 409, without discarding the Angular form.
Tenant isolation continues through the existing repositories and `AppDbContext` filters.

## Angular behavior

The feature directory owns typed API models/client and isolated list, details and form pages. Only supplier name
and the existing payable-account business requirement are mandatory; profile, contact, tax, website, notes and
product assignment fields remain optional. The form supports multi-product assignment, server field messages,
dirty-workspace state and a navigation guard. Routes and actions are hidden according to the bootstrapped backend
permission set, but the API permission remains authoritative.

Shared page-header, feedback, table and pager components remain presentation-only. No generic CRUD or form engine
was introduced, and no business payload or permission grant is persisted in browser storage.

## Verification

- Web Release build: succeeded, 0 warnings and 0 errors.
- Angular production build: succeeded, 276.30 kB initial raw bundle.
- Angular unit tests: 10 files, 26 tests passed, including create success, validation failure and 409 recovery.
- Security/inventory regression: 1081 checks passed.
- Production dependency audit: no production vulnerability (verified for this phase).
- MVC, database schema and supplier business rules: unchanged.

## Rollback

Remove the Angular supplier routes/navigation entry and the v1 supplier controller endpoints. Existing MVC
supplier screens and Application services remain usable; no data or schema rollback is required.
