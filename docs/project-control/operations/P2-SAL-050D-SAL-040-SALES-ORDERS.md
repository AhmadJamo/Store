# P2-SAL-050D / SAL-040 — Sales orders
> Completed: 2026-10-20

## Delivered

- One-time conversion from an Accepted Sales Quotation to an independently numbered Sales Order.
- Immutable copied header/line commercial snapshots with explicit source quotation and source-line links.
- Draft, Confirmed and reasoned Cancelled states with semantic history and rowversion concurrency.
- Four dedicated permissions and antiforgery protection on conversion, confirmation and cancellation.
- Responsive bilingual order list/detail screens with no horizontal scrolling.
- Completed-action behavior: after conversion the quotation hides the button and shows the generated order link.
- Permission-aware actionable errors and next-step guidance.
- Migration `20261019224051_AddSalesOrders` (migration 121).

## Safety boundary

Creating or confirming a Sales Order records commercial intent only. No inventory reservation, stock movement, accounting journal, invoice or payment is created. Reservation remains the separately approved `SAL-050` slice.

## Verification

- Final Release build passed with zero warnings/errors after code, localization and documentation updates.
- Security/inventory regression passed 856 checks.
- EF Core reported no pending model changes after migration 121.
- The SQL integration project builds and includes same-source uniqueness, cross-tenant reuse/isolation and zero stock/journal assertions. Its execution remains blocked before database creation by the local Windows SSPI authentication context.
- Local Demo remains at migration 119 because its SQL Server encryption connection also blocks applying migrations 120–121.

