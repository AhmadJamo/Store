# Phase 23 — Purchasing Angular migration

> Status: IN PROGRESS — `MOD-023A..R` COMPLETE on 2026-10-28
>
> Presentation default: MVC until a separate cutover approval.

## Scope

Move the existing procure-to-pay workflows to Angular in bounded slices. Document numbering, managed units, approval snapshots,
supplier sourcing, inventory/accounting posting, concurrency and tenant authority remain in Application/Domain and the server.

## `MOD-023A` delivered — Purchase Requests

- Added authenticated, versioned list/details/create-form/create/submit/cancel APIs over `PurchaseRequestService` only.
- Preserved independent `PurchaseRequests.View/Create/Submit/Cancel` permissions, antiforgery on every command and row-version checks.
- Added permission-aware Angular `/purchases/requests`, `/new` and `/:id` views with search/status filter, named warehouse/product/unit/
  supplier options, dynamic lines, details, approval evidence, history, submit and reasoned cancel.
- Rejected input remains editable and create forms are dirty-route protected. All presentation is Arabic/English, RTL-safe, responsive
  and uses external SCSS. Numbering, purchasability, unit-dimension conversion and lifecycle transitions remain server-owned.
- No schema, inventory, sourcing or accounting behavior changed; MVC remains default.

## Evidence

- Web Release build: zero warnings and zero errors.
- Security regression: 1227 checks passed.
- Angular: 166/166 tests across 63 files passed.
- Static accessibility: 72 templates passed; external-SCSS policy passed.
- Production bundles: 304,833/358,400 initial bytes and 81,742/81,920 largest lazy bytes.
- Live checks: list/create/details Angular routes returned 200; anonymous API read/write returned 401.

## `MOD-023B` delivered — approval rules and decisions

- Added an authenticated API for approval-rule list/create/activate/deactivate plus separately permissioned Purchase Request
  approve/reject commands. Every command validates antiforgery; rule activation retains row-version concurrency.
- Extended the Angular purchasing workspace with a permission-linked rules route, ordered role steps, warehouse/priority scope,
  active-state control, current-role evidence and eligible approve/reject controls with bounded notes.
- `PurchaseApprovalService` still resolves and freezes the matching rule, enforces the current ordered role and advances/rejects the
  instance and request atomically. Angular cannot skip steps or decide from UI permission state alone.
- Arabic/English, RTL, dirty-form recovery, external SCSS and accessibility are preserved. No schema or business rule changed.

## Current evidence

- Web Release build: zero warnings and zero errors.
- Security regression: 1230 checks passed.
- Angular: 167/167 tests across 63 files passed.
- Static accessibility: 72 templates passed; external-SCSS policy passed.
- Production bundles: 305,003/358,400 initial bytes and 81,742/81,920 largest lazy bytes.
- Live checks: approval-rules Angular route returned 200; anonymous approval/request reads returned 401.

## `MOD-023C` delivered — sourcing, quotations and award

- Added authenticated versioned APIs and Angular routes for sourcing list/details/create-from-approved-request, supplier invitation,
  quotation capture/comparison and one-time reasoned line allocation.
- Every command uses the existing Application services, independent `PurchaseSourcing.*` / `SupplierQuotations.*` permissions and
  antiforgery. Request eligibility, invited-supplier checks, commercial snapshots, exchange rates, exact demand allocation and state
  transitions remain server-owned.
- The bilingual RTL-safe workspace exposes names and commercial evidence rather than user-facing IDs, retains rejected input, hides the
  award action after completion and displays the frozen allocation evidence. No schema or business rule changed; MVC remains default.

## Current evidence

- Web Release: zero warnings/errors; SecurityRegression: 1233 checks.
- Angular: 168/168 tests across 64 files; 73 templates passed accessibility; external SCSS passed.
- Production bundles: 305,736/358,400 initial bytes and 81,742/81,920 largest lazy bytes.

## `MOD-023D` delivered — Purchase Orders

- Added authenticated list/details and create-from-award APIs plus approve, confirm, cancel, close/reopen remaining quantity,
  direct-delivery configuration/evidence and supplier delivery-date confirmation.
- Every command has its own existing `PurchaseOrders.*` permission, antiforgery and validated concurrency evidence. The Angular screen
  hides completed actions and exposes supplier, warehouse, currency, fulfillment progress, commercial lines and lifecycle history.
- `PurchaseOrderService` and Domain remain authoritative for award materialization, snapshots, status, direct delivery and quantities.
  Approval/confirmation still changes neither stock nor accounting. No schema/business-rule change; MVC remains default.

## Current `MOD-023D` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1236 checks.
- Angular: 169/169 tests across 65 files; 74 templates passed accessibility; external SCSS passed.
- Bundles: 306,260/358,400 initial bytes and 81,742/81,920 largest lazy bytes.

## `MOD-023E` delivered — Goods Receipts

- Added authenticated Goods Receipt list/details/create-form APIs plus direct create/post, scanning draft, idempotent product
  scan, final post, staged quality inspection and accepted-stock put-away operations.
- Every mutation retains antiforgery and its existing `GoodsReceipts.Create/Inspect/PutAway` permission. Confirmed-order
  eligibility, remaining quantities, managed units, lot/serial/date requirements, inventory valuation, physical movements,
  GRNI journal posting, staged status transitions and exact accepted/rejected/damaged/short totals remain server-owned.
- Added bilingual RTL-safe Angular `/purchases/receipts`, `/new`, `/:id` and `/:id/scan` routes with rejected-input
  preservation, dirty navigation protection and a permission-aware Receive Goods link from eligible confirmed orders.
- No schema or business-rule change; MVC remains default.

## Current `MOD-023E` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1239 checks.
- Angular: 170/170 tests across 66 files; 75 templates passed accessibility; external SCSS passed.
- Bundles: 306,587/358,400 initial bytes and 81,742/81,920 largest lazy bytes.

## `MOD-023F` delivered — Vendor Bills

- Added authenticated list/details/create-form and multipart draft-create APIs plus approve, reasoned match override, post,
  reasoned cancel, attachment download/upload, extraction generation and explicit extraction review operations.
- Independent `VendorBills.*` permissions, antiforgery, row-version concurrency and file count/size limits remain enforced.
  Receipt/service allocation, available quantities, duplicate supplier invoice protection, tax, matching tolerances, PPV,
  GRNI, payable and balanced journal posting remain server-owned.
- Added bilingual RTL-safe Angular `/purchases/bills`, `/new` and `/:id` routes with dynamic source lines, editable
  extraction review, matching evidence, dirty navigation protection and an eligible-order link.
- No schema or business-rule change; MVC remains default.

## Current `MOD-023F` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1242 checks.
- Angular: 171/171 tests across 67 files; 76 templates passed accessibility; external SCSS passed.
- Bundles: 307,621/358,400 initial bytes and 81,743/81,920 largest lazy bytes.

## `MOD-023G` delivered — Supplier Payments

- Added authenticated list/details/create-form and atomic create-and-post APIs over `SupplierPaymentService`.
- Angular provides bilingual RTL-safe `/purchases/payments`, `/new` and `/:id` routes, named payment methods,
  multi-bill allocation bounded by server-projected open balances and company-currency/carrying evidence.
- `SupplierPayments.View/Create`, antiforgery and validated IDs/amounts remain enforced. Same-supplier/currency eligibility,
  paid/credited recalculation, overpayment prevention, exchange differences, payable/cash-bank accounts and balanced journals
  remain server-owned. No schema/business-rule change; MVC remains default.

## Current `MOD-023G` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1245 checks.
- Angular: 172/172 tests across 68 files; 77 templates passed accessibility; external SCSS passed.
- Bundles: 308,019/358,400 initial bytes and 81,743/81,920 largest lazy bytes.

## `MOD-023H` delivered — Goods Receipt Returns

- Added authenticated list/details/create-form and create-and-post APIs over `GoodsReceiptReturnService`.
- Angular provides bilingual RTL-safe `/purchases/receipt-returns`, `/new` and `/:id` routes, receipt-line remaining quantities,
  received tracking evidence, explicit removal allocations and inventory/accounting evidence.
- `GoodsReceiptReturns.View/Create`, antiforgery and validated IDs/quantities remain enforced. Posted-receipt eligibility,
  unbilled return limits, current availability, location/tracking removal, AVCO, GRNI/PPV and balanced journals remain server-owned.
  No schema/business-rule change; MVC remains default.

## Current `MOD-023H` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1248 checks.
- Angular: 173/173 tests across 69 files; 78 templates passed accessibility; external SCSS passed.
- Bundles: 308,271/358,400 initial bytes and 81,743/81,920 largest lazy bytes.
- Live checks: Angular return list returned 200; anonymous API read returned 401.

## `MOD-023I` delivered — Supplier Credit Notes

- Added authenticated list/details/create-form and atomic create-and-post APIs over `SupplierCreditNoteService`.
- Angular provides bilingual RTL-safe `/purchases/credit-notes`, `/new` and `/:id` routes with billed/credited/remaining quantities,
  received tracking, explicit removal allocations, payable/tax totals and removed inventory-cost evidence.
- A posted eligible Vendor Bill now exposes a permission-aware credit-note link. `SupplierCreditNotes.View/Create`, antiforgery and
  DTO validation remain enforced. Bill eligibility, current stock/tracking availability, prorated tax, AVCO removal, payable,
  input-tax and PPV journal lines remain server-owned. No schema/business-rule change; MVC remains default.

## Current `MOD-023I` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1251 checks.
- Angular: 174/174 tests across 70 files; 79 templates passed accessibility; external SCSS passed.
- Bundles: 308,688/358,400 initial bytes and 81,743/81,920 largest lazy bytes.
- Live checks: Angular credit-note list returned 200; anonymous API read returned 401.

## `MOD-023J` delivered — Purchase Document Reversals

- Added authenticated list/details/create-form and idempotent reversal APIs over `PurchaseDocumentReversalService`.
- Angular provides bilingual RTL-safe `/purchases/reversals`, `/new` and `/:id` routes with original-document, reason, actor and
  opposite-journal evidence. Posted Goods Receipt, Vendor Bill and Supplier Payment details expose permission-aware reversal links.
- `PurchaseDocumentReversals.View/Create`, antiforgery, bounded reason and idempotency validation remain enforced. Dependency order,
  fiscal period, unchanged valuation, stock availability, physical/value corrections, opposite journals and original-state changes
  remain server-owned. No schema/business-rule change; MVC remains default.

## Current `MOD-023J` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1254 checks.
- Angular: 175/175 tests across 71 files; 80 templates passed accessibility; external SCSS passed.
- Bundles: 309,114/358,400 initial bytes and 81,743/81,920 largest lazy bytes.
- Live checks: Angular reversal list returned 200; anonymous API read returned 401.

## `MOD-023K` delivered — Landed Costs

- Added authenticated list/details/create-form and atomic create-and-post APIs over `LandedCostService`.
- Angular provides bilingual RTL-safe `/purchases/landed-costs`, `/new` and `/:id` routes with receipt evidence, typed components,
  quantity/value allocation choice, capitalization/expense split and before/after AVCO evidence.
- Posted receipts link to landed-cost allocation, and posted landed costs link to controlled reversal. Independent permissions,
  antiforgery and DTO validation remain enforced. Allocation, current-stock split, AVCO, value-only movements, clearing/expense
  accounts and balanced journals remain server-owned. No schema/business-rule change; MVC remains default.

## Current `MOD-023K` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1257 checks.
- Angular: 176/176 tests across 72 files; 81 templates passed accessibility; external SCSS passed.
- Bundles: 309,507/358,400 initial bytes and 81,743/81,920 largest lazy bytes.
- Live checks: Angular landed-cost list returned 200; anonymous API read returned 401.

## `MOD-023L` delivered — Purchase Service Acceptances

- Added authenticated list/details/create-form and create APIs over `PurchaseServiceAcceptanceService`.
- Angular provides bilingual RTL-safe `/purchases/service-acceptances`, `/new` and `/:id` routes with ordered, previously completed and
  remaining service quantities, immutable completion evidence and a permission-aware link from eligible confirmed Purchase Orders.
- `PurchaseServiceAcceptances.View/Create`, antiforgery and DTO validation remain enforced. Confirmed-order eligibility,
  service-completion billing policy, cumulative quantity limits and the no-inventory-effect boundary remain server-owned.
  No schema/business-rule change; MVC remains default.

## Current `MOD-023L` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1260 checks.
- Angular: 177/177 tests across 73 files; 82 templates passed accessibility; external SCSS passed.
- Bundles: 309,966/358,400 initial bytes and 81,743/81,920 largest lazy bytes.
- Live checks: Angular service-acceptance list returned 200; anonymous API read returned 401.

## `MOD-023M` delivered — Purchase Agreements and repeat templates

- Added authenticated Agreement list/details/create-form/create, activate, cancel and capped release APIs plus Template list,
  create-from-order, generate-request and active-state APIs over the existing Application services.
- Angular provides bilingual RTL-safe agreement and template workspaces, links a single-supplier award to agreement creation and links
  Purchase Orders to repeat-template capture. Every command retains its independent permission and antiforgery.
- Award exclusivity, row-version lifecycle control, contracted remaining quantity, validity, current product/unit eligibility,
  monotonic generation references and the normal request/approval/sourcing path remain server-owned. No inventory/accounting effect
  or schema/business-rule change was introduced; MVC remains default.

## Current `MOD-023M` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1266 checks.
- Angular: 178/178 tests across 74 files; 84 templates passed accessibility; external SCSS passed.
- Bundles: 310,690/358,400 initial bytes and 81,743/81,920 largest lazy bytes.
- Live checks: both Angular routes returned 200; both anonymous API reads returned 401.

## `MOD-023N` delivered — supplier purchasing terms and prices

- Added authenticated page, exception, price-break and price-history APIs plus atomic bulk save, active state, bounded CSV/XLSX
  preview/import and export operations over `SupplierPurchasingInfoService`.
- Angular provides a bilingual RTL-safe bulk product editor, named supplier/unit choices, import validation evidence, exception filters,
  quantity-price breaks and immutable price history. View/Manage permissions, antiforgery and 1 MB preview limits remain enforced.
- Product/unit compatibility, duplicate/preferred rules, concurrency, validity, base currency, atomic import and price snapshots remain
  server-owned. No schema/business-rule change; MVC remains default.

## Current `MOD-023N` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1269 checks.
- Angular: 179/179 tests across 75 files; 85 templates passed accessibility; external SCSS passed.
- Bundles: 311,304/358,400 initial bytes and 81,743/81,920 largest lazy bytes.
- Live checks: Angular route returned 200; anonymous API read returned 401.

## `MOD-023O` delivered — automated replenishment

- Added authenticated settings/read and separately permitted save/run APIs over `ReplenishmentAutomationService`.
- Added bilingual RTL-safe Angular settings, manual run, history, suggestion evidence and generated request/RFQ links.
- Scheduling, demand calculation, duplicate prevention and document generation remain server-owned; MVC remains default.
- Evidence: Release zero warnings/errors; 1271 regression checks; Angular 180/180 across 76 files; 86 templates; bundles
  311,468/358,400 initial and 81,743/81,920 largest lazy bytes.

## `MOD-023P` delivered — purchasing analytics

- Added an authenticated `PurchaseAnalytics.View` API over the existing `PurchaseAnalyticsService`; filters, KPI calculations,
  supplier and buyer grouping, matching exceptions and acceptance-stage counts remain server-owned.
- Added a bilingual RTL-safe Angular analytics workspace with date/supplier/buyer/status/group filters, operational cards, trends,
  buyer workload, supplier performance, source-document links and localized KPI/acceptance evidence.
- Buyer identities are resolved to display names server-side, permission-aware journey links remain protected and validation uses
  localized Problem Details. This is read-only and introduces no schema, inventory or accounting change; MVC remains default.

## Current `MOD-023P` evidence

- Web Release: zero warnings/errors; SecurityRegression: 1272 checks.
- Angular: 181/181 tests across 77 files; 87 templates passed accessibility; external SCSS passed.
- Bundles: 311,587/358,400 initial bytes and 81,743/81,920 largest lazy bytes.
- Live checks: Angular route returned 200; anonymous API read returned 401.

## `MOD-023Q` delivered — Purchase Accounting Reconciliation

- Added an authenticated `PurchaseAccountingReconciliation.View` API over the existing Application service and repository projection.
- Added a bilingual RTL-safe Angular workspace for as-of/exceptions filters, inventory-to-ledger evidence, GRNI receipt balances,
  settlement documents and permission-aware source/setup links.
- Historical timing notes, missing-account guidance and every reconciliation total/difference remain server-owned. The screen is
  read-only and introduces no schema, inventory or accounting change; MVC remains default.
- Evidence: Release zero warnings/errors; SecurityRegression 1273; Angular 182/182 across 78 files; 88 templates; bundles
  311,737/358,400 initial and 81,743/81,920 largest lazy bytes; live Angular 200 and anonymous API 401.

## `MOD-023R` delivered — supplier communications

- Added authenticated APIs for communication detail/document history, preparation, activity recording and attachment download.
- Added bilingual RTL-safe Angular preparation/history/detail workspaces linked by supplier name from RFQ and Purchase Order details.
- Preparation and activity commands retain independent permissions and antiforgery. Eligibility, immutable message/attachment snapshots,
  status ordering and evidence requirements remain server-owned; opening the mail client never claims delivery. MVC remains default.
- Evidence: Release zero warnings/errors; SecurityRegression 1275; Angular 183/183 across 79 files; 89 templates; bundles
  312,007/358,400 initial and 81,743/81,920 largest lazy bytes; live Angular 200 and anonymous API 401.

## Next slice

`MOD-023S` migrates supplier credentials, private portal access, external submission queue/retry and immutable payload downloads.
