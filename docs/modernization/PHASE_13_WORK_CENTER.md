# MOD-013 — Action and exception center

## UX-006 workflow evolution (2026-11-05)

The later UI/UX program adds a shared display-only workflow progress region and localized next-step guidance to Stock
Transfers, Purchase Requests, Purchase Orders and Sales Orders. Features continue to own server-status mapping, permissions,
reasons and protected commands; the shared component neither calculates nor executes a transition.

> Status: COMPLETED  
> Completed: 2026-10-24  
> Database changes: none

## Outcome

Angular now provides a bilingual personal work center over existing Sales Order and Customer Invoice decisions. The
Application service emits only actions the current user can both view and perform: confirm Draft orders, approve Draft
invoices, post Approved invoices and review overdue Posted balances before collection. Each item carries contextual
help and opens the authoritative document-evidence route; Angular does not execute or invent the business action.

## Boundaries

- The backend filters every item with the existing View plus action permission.
- Queries are bounded and reuse existing Application services; the controller does not access EF or repositories.
- Empty, loading, recovery, critical and attention states are localized and use external SCSS.
- MVC source screens remain the mutation and rollback path.

## Verification

Web Release build passed without warnings/errors. The final bundle passed 1092 backend regression checks, 55 Angular
tests and the Angular production build.
