# MOD-030X three-controller purchasing retirement

> Completed: 2026-11-14

## Cohort

- `GoodsReceiptReturnsController` and three views.
- `PurchaseServiceAcceptancesController` and three views.
- `SupplierCreditNotesController` and three views.

Each flow already had an exact Angular list/create/details feature and authenticated API over the same Application service.
The APIs preserve View/Create separation, antiforgery and server-side validation. No domain, schema or operational data changed.

## Verification

- Release: zero warnings and zero errors.
- Security/inventory regression: 1,803 passed.
- Angular: 242/242 passed across 116 files.
- Static templates: 137; feature pages: 120.
- Bundle: 337,191/358,400 initial; 81,877/81,920 largest lazy chunk.
- Generic responsive/RTL/LTR/keyboard acceptance: passed.

## Outcome

The inventory is 65 controllers, 373 actions, 191 Razor files, 173 user-facing views and 183 Razor control rows. Legacy
GET/HEAD routes preserve authorization and exact identifiers; old MVC Create POSTs return 405 and are never replayed.
