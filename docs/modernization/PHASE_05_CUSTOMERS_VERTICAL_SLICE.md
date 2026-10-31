# MOD-005 — Customers Angular vertical slice

> Status: COMPLETE  
> Completed: 2026-10-24  
> Database changes: None  
> MVC rollback path: Preserved

## Outcome

Customers are the first complete business slice exposed through the internal API and Angular workspace. The existing MVC screens remain operational. Both presentations call the same `CustomerService`; customer numbering, receivable policy, commercial-default validation, tenant isolation, transactions and row-version authority remain server-owned.

## Internal API

`/api/v1/customers` provides permission-separated list, details, create-form options, edit-form options, create and update operations. It returns Application DTOs only and never exposes EF entities.

- `Customers.View`: paged/searchable/sortable list and details.
- `Customers.Create`: create options and mutation.
- `Customers.Edit`: edit options and mutation.
- Global antiforgery validation protects POST/PUT requests.
- Invalid model or business input returns localized Validation Problem Details.
- A stale row version returns `409 Conflict` through the explicit `CustomerConcurrencyException` while remaining compatible with the MVC error boundary.
- Generated `CustomerCode` and policy-resolved `AccountId` remain optional at the request boundary.

## Angular experience

- Permission-aware Customers navigation entry.
- Responsive server-paged list with search, archive inclusion and safe supported sorting.
- Details page for profile, commercial defaults, contacts and addresses.
- One create/edit form for the complete customer profile and future-document defaults.
- Client validation is convenience only; server validation remains authoritative and maps Problem Details to the form.
- Successful save clears workspace dirty state and opens details; failed save keeps the form and displays the problem.
- Router and workspace guards require an explicit decision before abandoning dirty input.
- All CSS is in the separate `customers.scss` stylesheet; templates contain no inline CSS.

## Verification evidence

| Verification | Result |
|---|---|
| Web Release build | Passed, 0 warnings/errors |
| Angular production build | Passed; 273.76 kB initial raw bundle |
| Angular Vitest | Passed 7/7 files and 17/17 tests |
| Backend SecurityRegression | Passed 1076 checks |
| Production dependency audit | 0 vulnerabilities |

Regression covers API authentication/versioning, permission separation, antiforgery and explicit concurrency. Angular tests cover generated-code omission, query/update contracts, successful save navigation, failed-save preservation and the existing workspace/design foundations.

## Rollback

Remove the Customers Angular routes/components/client and the internal Customers API controller, then remove the specialized concurrency subtype and restore the two original throws. The existing MVC route, Application behavior and database need no rollback.
