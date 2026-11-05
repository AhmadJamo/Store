# MOD-006 — Proven shared UI primitives

> Status: COMPLETE  
> Completed: 2026-10-24  
> Database changes: None

## Outcome

Angular now has four small presentation-only primitives proven by two business consumers: Customers and Suppliers. The shared layer owns layout and accessibility behavior only; feature components retain permissions, columns, queries, actions and business decisions.

- `PageHeaderComponent`: title, description, eyebrow and projected feature actions.
- `FeedbackStateComponent`: accessible loading, empty and error states with optional retry output.
- `DataTableComponent`: responsive, keyboard-focusable and named overflow region around feature-owned table markup.
- `PagerComponent`: displays bounded server paging and emits requested page numbers without fetching or authorizing.

All templates and SCSS are separate files. No inline CSS, schema-driven form, generic CRUD engine, client paging authority or client permission authority was introduced. Customer form validation remains feature-local because a second form consumer has not proven its reusable shape.

## Second business consumer

Suppliers now have a read-only Angular list and authenticated `/api/v1/suppliers` endpoint using `Suppliers.View`. The Application service normalizes search and 10–100 page bounds; the tenant-filtered repository performs search/count/order/page. This avoids an unbounded browser list and avoids loading product assignments solely to render directory rows. Existing MVC supplier management remains unchanged.

## Verification

| Verification | Result |
|---|---|
| Web Release build | Passed, 0 warnings/errors |
| Angular production build | Passed; 274.39 kB initial raw bundle |
| Angular Vitest | Passed 9/9 files and 22/22 tests |
| Backend SecurityRegression | Passed 1079 checks |

Tests cover shared metadata rendering, accessible error/retry behavior, emitted page decisions, named table regions, supplier API parameters and backend authentication/permission/layer boundaries.

## UX-004 evolution — 2026-11-05

`DataTableComponent` now retains the original projected-table mode and adds an opt-in typed mode. Column metadata
describes presentation semantics only; feature-owned `DataTableCellDirective` templates retain business links, badges
and actions. The shared component renders bounded loading/error/empty states, sticky headers, logical type alignment and
horizontal overflow. Sortable headers emit a key/direction request and never reorder data locally. Products, Purchase
Requests and Stock Transfers are the first typed consumers. Verification passed 227/227 Angular tests, 126 static
templates, 5/5 hosted layout checks and the custom production budgets.
