# 17 — Purchases migration audit

> Status: FRD-J in progress — Reconciliation workspace V4 complete; Purchase Analytics safety net next
> Date: 2026-11-27
> Source of truth: Angular routes/components and clients, Internal API controllers, Application services/DTOs, repositories and purchasing module documentation

## Scope inventory

The authenticated Purchasing workspace exposes 59 Angular routes, including the `/purchases` module hub, through 21 purchasing page templates. A separate anonymous, capability-limited `/supplier-portal/:token` route reuses the same feature directory but is an external trust boundary and must not be migrated with ordinary authenticated pages.

| Surface | Route count | Archetype | Authority and independent permissions | Migration risk |
|---|---:|---|---|---|
| Module hub | 1 | navigation hub | permission-filtered module metadata | Low; already shared shell presentation |
| Purchase Requests + approval rules | 4 | list/create/record/configuration | request service and approval service own validation, frozen approval steps, rowversion and Submit/Cancel/Approve/Reject/Manage permissions | High workflow risk; current page is the only broadly V4-composed purchasing surface |
| Sourcing + quotations + comparison | 6 | list/create/record/print/quotation/award | sourcing and quotation services own policy, attachments, allocations, award completeness and Create/View/Award permissions | High commercial and document risk |
| Purchase Orders | 2 | list/record workflow | lifecycle service owns approval, confirmation, cancellation, closure, reopening, dropship, direct delivery and supplier-confirmation commands with separate permissions | High workflow/accounting/inventory risk |
| Goods Receipts | 4 | list/create/record/scan workflow | receipt service owns staged receipt, inspection, storage, tracking, inventory, GRNI and command permissions | Critical stock and accounting risk |
| Vendor Bills | 3 | list/create/record workflow | bill service owns matching, override, approval, posting, cancellation, extraction and attachments under separate permissions | Critical payable/accounting risk |
| Supplier Credit Notes | 3 | list/create/record posting | service owns quantity caps, stock removal, valuation and journal posting | Critical stock/accounting risk |
| Document Reversals | 3 | list/create/record posting | reversal service owns dependency order, fiscal period, exact opposite journals and idempotency | Critical accounting/inventory risk |
| Landed Costs | 3 | list/create/record posting | service owns allocation, stock revaluation and accounting posting | Critical valuation risk |
| Service Acceptances | 3 | list/create/record workflow | service owns completion evidence and ordered/service billing availability | High workflow/payables risk |
| Agreements | 3 | list/create/record workflow | service owns activation, cancellation, releases and immutable commercial evidence | High commercial workflow risk |
| Purchase Templates | 2 | list/create/action workspace | template service owns creation from orders, generation and activation | Medium command risk |
| Supplier Terms | 4 | master/list/import/history | supplier purchasing services own assignments, tiers, imports, concurrency, exceptions and exports | High bulk-write/file boundary risk |
| Replenishment Automation | 1 | settings/run workspace | automation service owns settings, demand calculation and document creation | High cross-module command risk |
| Purchase Analytics | 1 | read-only dashboard | `PurchaseAnalyticsService` owns KPIs, grouping, options and acceptance journey; `PurchaseAnalytics.View` | Low mutation risk, medium presentation complexity |
| Accounting Reconciliation | 1 | read-only evidence report | reconciliation service/repository own inventory/ledger/GRNI values, tolerance, ordering and exception filtering; `PurchaseAccountingReconciliation.View` | Lowest bounded risk; selected first |
| Supplier Communications | 2 | prepare/record workspace | communication service owns prepared/sent/delivered/replied evidence and attachments | Medium external-communication risk |
| Supplier Integrations | 2 | dashboard/submission record | integration service owns credentials, tokens, outbox, retries, UBL and downloads with separate permissions | Critical security/external boundary risk |
| Supplier Payments | 3 | list/create/record posting | payment service owns availability, currency, realized gain/loss and journal posting | Critical cash/accounting risk |
| Goods Receipt Returns | 3 | list/create/record posting | return service owns receipt caps, stock/tracking removal and accounting reversal | Critical stock/accounting risk |
| Legacy Purchases + returns | 5 | historical list/record/return | creation is retired; existing services retain historical posting and controlled return authority | High legacy/accounting risk |
| Supplier Portal | 1 external | anonymous token portal | hash-only, expiring and revocable capability boundary; supplier-scoped views and idempotent submissions | Separate security wave; never combine with authenticated restyling |

## Current presentation and test baseline

- All 21 templates use the shared `PageHeader`; 11 currently use the shared typed `DataTable`.
- Purchase Requests already uses FilterBar, SummaryStrip, FormSection, FieldCue, DetailSection, WorkflowProgress and WorkflowFooter. Purchase Orders uses WorkflowProgress and WorkflowFooter. Other pages remain mixed card/form/table layouts and must be migrated in bounded slices rather than by a feature-wide rewrite.
- English and Arabic localization services exist for every surface; Purchase Templates intentionally reuses Purchase Agreement localization. Any changed copy must remain bilingual and RTL-safe.
- At audit time, the purchasing feature had 20 API-client spec files but zero `*page.spec.ts` component files. API request coverage alone does not protect rendered server evidence, permission-gated actions, empty/error states, form payloads or dirty recovery.
- The completed Reconciliation pilot has nine component tests. The current baseline is 379 Angular tests across 143 files, Backend Release with zero warnings/errors, 1,668 SecurityRegression checks, production build and Playwright 7/7. Static gates report 44 DataTable pages, 14 FilterBars, 16 SummaryStrips and 10 DetailSection pages. FRD-ISS-012 remains independently open at 84.55 kB versus 80 kB; live API/performance checks remain skipped without an external URL and authenticated state.

## Cross-layer contracts that must not change

- Angular must not calculate trusted prices, taxes, currency values, matching decisions, inventory value, GRNI, payable balances, payment availability, allocation completeness, replenishment demand or journal effects.
- All writes continue through existing Application services, repositories and UnitOfWork. Controllers must not access EF directly and DTOs remain the API boundary.
- Purchase Request, Sourcing, Order, Receipt, Bill, Payment, Return, Credit, Reversal, Landed Cost and Agreement transitions retain independent permissions, antiforgery, rowversion/idempotency and current-state validation.
- Products, suppliers, warehouses, accounts and users remain name/code-first in the UI while IDs remain internal command values.
- Goods Receipt, return, credit, reversal and landed-cost presentation must preserve server-owned inventory, tracking, movement, valuation, fiscal-period and journal authority.
- Vendor Bill, Supplier Payment and reconciliation presentation must preserve server-owned matching, payable, currency, rounding and accounting evidence.
- Supplier portal, integration credentials, UBL, external submissions, outbox delivery, downloads and attachments retain their current trust, rate-limit, file-validation and network boundaries.
- Historical Legacy Purchases stay visible but new legacy creation remains retired.
- No API, DTO, schema, permission, posting rule or purchase/inventory/accounting behavior is authorized by FRD-J presentation work.

## First slice decision

Purchase Accounting Reconciliation is the first bounded FRD-J slice because it is one authenticated GET-only route with one independent view permission, one typed API client call and no mutation command. The server returns and orders every inventory, ledger, GRNI, tolerance, settlement and status value. The page only filters by as-of date and exception visibility and exposes permission-gated links to source records.

Before any presentation change, add a focused component safety net that proves:

1. server-authored inventory and GRNI totals, differences, status and historical-snapshot notice render unchanged;
2. warehouse/account, receipt/order/supplier and settlement evidence remains named and server-authored;
3. as-of date and `exceptionsOnly` are the exact GET query inputs;
4. Settings, stock, warehouse, receipt, order, bill and return links remain independently permission-gated;
5. empty/error states do not invent evidence and the report remains explicitly read-only.

Purchase Accounting Reconciliation V4 is complete. Localized context, FilterBar, SummaryStrip and two DetailSections now present Inventory-to-Ledger in a seven-column typed table and GRNI in an eleven-column typed table. Server order, values, status, account evidence, settlements, empty states and independent stock/warehouse/receipt/order/bill/return permissions remain unchanged. The next bounded task is a component safety net for the single-route GET-only Purchase Analytics page before any restyle. Do not change the API, Application service, repository, DTOs, permissions or accounting calculations.

## Deferred ordering

After the reconciliation pilot is accepted, prefer other read-only/reporting surfaces before transactional workflows: Purchase Analytics, supplier-term history/exceptions and historical Legacy directories. Migrate requests/sourcing/orders next with workflow safety nets, then receipts/bills/payments/returns/credits/reversals/landed costs. Handle integrations and the anonymous Supplier Portal only in dedicated security-boundary waves.
