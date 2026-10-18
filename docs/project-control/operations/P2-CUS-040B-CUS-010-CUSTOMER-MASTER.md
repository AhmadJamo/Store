# P2-CUS-040B / CUS-010 — Customer master evidence
> Completed: 2026-10-18 | Environment: owner-declared local Demo

## Outcome

- Expanded Customer additively while preserving every CustomerId, AccountId and historical Sale relationship.
- Added tenant-generated CustomerCode, company/individual profile, contact/tax/language/time-zone fields, reversible archive and SQL rowversion.
- Replaced entity-bound pages with DTO-only server search, sort and pagination plus responsive Create/Edit/Details views.
- Added Customers.View/Create/Edit/Archive permissions, antiforgery, localized Arabic messages and permission-aware next-step guidance.
- New sales reject archived customers; historical inventory/sale uses may still resolve archived rows.

## Database evidence

- Migration `20261018144824_AddProfessionalCustomerMaster` applied as 115/115.
- Existing Demo customer count stayed 1; missing codes 0; tenant/code distinct count 1; active count 1.
- The owning tenant sequence is 2 after its preserved customer; four empty tenants start at 1.
- `dotnet ef migrations has-pending-model-changes`: no pending model change.

## Verification

- Release solution build: succeeded, 0 warnings and 0 errors.
- SecurityRegression: 787 checks passed.
- Disposable SQL integration: passed, including customer tenant isolation, tenant-local code uniqueness, archive visibility and rowversion stale-update rejection.

## Rollback posture

Operational rollback should restore the previous UI/service while retaining the additive columns. Do not run the destructive migration Down path against shared data. No Git commit was created because the owner does not use Git for this workspace.
