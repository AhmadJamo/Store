# AI Work Log

- 2026-10-30: Completed approved `MOD-028G` and therefore the full `MOD-028A..G` remaining-surface wave. Added an antiforgery-protected, allowlisted culture API sharing the MVC essential HttpOnly SameSite=Lax cookie; Angular now changes locale only after server success and treats session culture as authoritative. A neutral shared culture signal avoids the HTTP interceptor/session dependency cycle and keeps document direction plus feature session consumers aligned. The generated registry now reports 164 controllers, 199 views, 358 MVC actions, 209 Razor controls, 180 Angular routes, 82 counterparts and zero remaining surfaces. Release passed with zero warnings/errors, 1,323 backend checks, 214/214 Angular tests across 106 files, 121 accessibility templates, and bundle measurements of 326,764/358,400 initial and 81,880/81,920 largest lazy bytes. MVC remains the default; execution advances to `MOD-029A` acceptance baseline and dual-run reconciliation.

- 2026-10-24: Completed continuously approved `MOD-006`. Extracted four presentation-only Angular primitives—page header, accessible feedback state, named responsive table region and server-driven pager—only after Customers supplied real usage, then proved them with a second business consumer. Added an authenticated `Suppliers.View` read-only API and Angular supplier directory with debounced search and bounded server paging; repository/Application layers retain tenant query and paging authority, and the row projection avoids product-assignment N+1 work. All CSS remains in separate SCSS files. Web/Angular builds passed without warnings, 9/9 files and 22/22 Angular tests passed, and backend regression passed 1079 checks. No database, supplier mutation, MVC or business-rule change.

- 2026-10-24: Corrected the Angular development proxy target from stale port 7097 to the committed MiniStore.Web HTTPS launch port 54052. Documented the exact two-terminal backend/frontend startup, Angular URL, backend login URL and one-time development-certificate trust command. No production, API, database or business behavior changed.

- 2026-10-24: Completed approved `MOD-005`, the first Angular business vertical slice. Added authenticated, versioned and permission-separated Customer list/details/form/create/update API endpoints over the existing `CustomerService`, localized validation Problem Details and explicit `409` row-version concurrency without changing numbering, receivable/tax policies, transactions, MVC or the database. Added a permission-aware Customers workspace entry, typed API client, responsive bilingual server-paged and debounced-search list, details and complete create/edit form with external SCSS, field/server errors and dirty-route protection. Web Release build passed with zero warnings/errors, Angular production build passed at 273.76 kB raw initial, 7/7 files and 17/17 tests passed, backend regression passed 1076 checks and production npm audit found zero vulnerabilities.

- 2026-10-24: Completed approved `MOD-004`. Replaced the placeholder Angular tab state with a reusable metadata-driven, session-permission-aware workspace engine: stable navigation/context identity and deduplication, configurable tab/history/recent limits, independent bounded Back/Forward history, close/close-others/reopen, allowlisted versioned refresh restoration and dirty-state decisions including browser unload protection. Added localized ARIA tabs, RTL/LTR keyboard movement and focused state/restoration/guard tests. Angular production build passed without warnings at 256.32 kB raw initial, 5/5 files and 12/12 tests passed, production dependency audit remained zero, and backend 1072 regression checks passed. No backend, database, MVC or business-rule change.

- 2026-10-24: Completed approved `MOD-003`. Added versioned non-sensitive UI preferences, system/light/dark theme resolution, comfortable/compact density, persisted locale direction, expanded semantic tokens and accessible focus/reduced-motion/forced-color behavior. Added a lazy bilingual design-system preview for actions, statuses, fields and empty/loading/error states. Angular production build passed at 256.08 kB raw initial, 4/4 files and 6/6 tests passed, production dependency audit remained zero, backend 1072 regression checks passed, and a live Arabic RTL visual/accessibility-tree review succeeded. No backend, database or MVC behavior changed.

- 2026-10-24: Completed approved `MOD-002`. Added the versioned same-origin internal API foundation in the existing Web host: tenant-aware session and granted-permission DTOs through Application/repository boundaries, CSRF bootstrap and side-effect-free protected probe, localized RFC Problem Details with correlation IDs, JSON 401/403 behavior, authenticated v1-only OpenAPI and bounded paging/sort contracts. Angular now bootstraps session/CSRF, centralizes API errors and attaches CSRF headers to unsafe internal requests. No migration or MVC/business-rule change. Web/SecurityRegression build passed with zero warnings/errors, 1072 checks passed, Angular build (232.18 kB initial raw) and 2/2 files with 3/3 tests passed, and live anonymous session/CSRF/OpenAPI probes returned `401 application/problem+json`.

# 2026-10-15 — Purchasing work procedure catalog

- Added a bilingual Settings catalog containing the 19-step purchasing sequence from prerequisite setup through management review.
- Identified the recommended step owner and reviewer/approver beside every step according to accounting segregation and administrative control principles.
- Added practical instructions, completion evidence, conditional return/credit steps and permission-aware buttons to the actual work screens.
- Kept the catalog as read-only guidance through the Application layer; transactional services, permissions and domain rules remain authoritative.
- Added the catalog as the first Settings card and documented future expansion to customer management, sales, inventory and accounting.
- Release build completed with zero warnings/errors, all 748 focused checks passed and no database migration was required.

# 2026-10-15 — PUR-280 advanced analytics and legacy purchase retirement

- Expanded purchasing analytics with date, supplier, buyer and order-status filters plus month/supplier/buyer grouping.
- Added spend, cost, seasonality, lead-time, unconfirmed/overdue order, price-variance, buyer-workload and open-match-exception evidence with source-document drill-through.
- Added explicit KPI source/period/calculation definitions and a permission-aware procure-to-pay acceptance journey from request through matching, payment, return and closure.
- Added legacy-versus-modern usage comparison and accepted the decision to retire new Legacy Direct Purchase creation while preserving historical records.
- Enforced retirement in the Application service and controller, relabeled legacy navigation and replaced the creation action with a Purchase Request destination.
- Updated Arabic localization, responsive card layouts, regression coverage, roadmap status and architectural documentation.
- Debug/Release builds completed with zero warnings/errors, all 744 focused checks passed, SQL integration including advanced analytics evidence passed and the EF model has no pending changes.

# 2026-10-15 — PUR-270 supplier portal and external integration

- Added expiring/revocable private supplier portal links for invited RFQs and supplier Purchase Orders, including quotation submission, delivery-date confirmation and review-only delivery notices.
- Added supplier-bound API keys stored only as hashes, exact operation scopes, separate rate limits and idempotent write commands that reject key reuse with changed content.
- Added safe UBL 2.1 Purchase Order export plus Order Response and Invoice intake; invoice documents remain immutable review evidence and cannot post stock or accounting.
- Added a tenant-owned reliable outbox with atomic leases, stable idempotency keys, bounded retry, delivery/error status and manual retry. HTTPS callbacks reject redirects, proxies and private/local network targets.
- Added external submission payload/fingerprint review, security audit evidence, responsive bilingual screens, dedicated internal permissions and navigation.
- Replaced completed portal-link and Purchase Order queue actions with active/queued state, and linked missing callback errors directly to integration settings.
- Generated and locally applied migration `20261014205756_AddSupplierPortalAndIntegration`; Debug and Release builds completed with zero warnings/errors, all 741 focused checks passed, SQL integration completed successfully and the EF model has no pending changes.

# 2026-10-14 — PUR-260 supplier documents and communication

- Added bilingual RFQ and Purchase Order email preparation with tenant-owned message snapshots, downloadable attachments and document-level communication history.
- Kept Prepared distinct from Sent, Delivered and Replied; users record actual evidence manually, with delivery references and reply notes enforced by the domain.
- Added Purchase Order supplier delivery-date confirmations that preserve previous/new dates, reference and audit actor/time.
- Added Draft Vendor Bill attachment management and local review-first invoice extraction suggestions. Applying a reviewed suggestion never approves or posts the bill.
- Added dedicated permissions, Arabic localization, actionable error and next-step guidance, responsive cards, tenant-safe mappings and migration `20261014202715_AddSupplierCommunicationAndInvoiceExtraction`, applied locally.
- Release build completed with zero warnings/errors, all 721 focused checks passed, SQL integration completed successfully and the EF model has no pending changes.

# 2026-10-14 — PUR-250B demand allocation and direct delivery

- Added tenant-owned Purchase Order demand allocations and automatically retain the originating Purchase Request line through sourcing and split award creation.
- Added Draft-only Dropship configuration with recipient/address capture, product purchase-Expense-account validation and separate fulfillment permission.
- Added immutable partial supplier-to-customer delivery evidence, cumulative quantity caps, delivery progress, permission-aware next steps and completed-state UI.
- Blocked Goods Receipts and every internal stock movement for Dropship; Vendor Bills become eligible only up to delivered quantity and post to the frozen Expense account without GRNI.
- Added and locally applied migration `20261014161000_AddDemandAllocationAndDirectDelivery`, bilingual UI/resources, actionable product-account guidance, security/domain/SQL regression checks and ADR `2026-10-14-dropship-no-stock-boundary.md`.

# 2026-10-14 — PUR-250A staged receiving and quality hold

- Added warehouse-configurable One Step or Receive/Inspect/Store purchase receiving and froze the route on every Goods Receipt.
- Added separate receipt, quality and storage states with delivered, accepted, rejected, damaged and short evidence plus mandatory discrepancy reasons.
- Staged receipt and inspection create no inventory, tracking, movement or accounting effect; storage posts only accepted quantity through the existing atomic Inventory/GRNI path. Fully rejected receipts finish with zero stock.
- Added dedicated inspection/storage permissions, state-aware responsive receipt cards without horizontal scrolling, Arabic localization and permission-aware next-step links. Completed actions disappear from the screen.
- Added and locally applied migration `20261014153921_AddStagedReceivingQualityHold` with safe one-step defaults, existing-line quantity backfill and active-stage uniqueness. The restricted environment could not negotiate required SQL encryption, while the approved host Windows environment applied it successfully.
- The solution builds with zero warnings/errors, 695 focused regression checks pass and the fresh Debug EF design-time model reports no pending changes.

# 2026-10-14 — PUR-240 automated replenishment and demand forecasting

- Added tenant-configurable scheduled and manual planning with review-only, Draft Purchase Request and policy-bounded direct RFQ modes.
- Extended purchase-sourced replenishment rules with historical-demand coverage and optional seasonality while preserving min/max planning.
- Calculations store reservation-aware availability, confirmed incoming supply, history, factor, target, rounded supplier quantity, reason and generated-document links.
- Added grouping by supplier/warehouse/currency/date, active-document duplicate suppression, scheduled-run uniqueness and a responsive bilingual history/settings page.
- Generated migration `20261014110940_AddAutomatedReplenishmentPlanning`; local application was blocked by the SQL Server Windows-authentication SSPI context.
- The full solution builds without warnings or errors, 687 focused regression checks pass and EF reports no pending model changes.

# 2026-10-14 — PUR-230 native Excel completion

- Completed PUR-230 with supplier-scoped native XLSX export/import beside the existing CSV workflow. The formatted `SupplierPrices` sheet freezes and filters headers and preserves typed quantities, prices, dates and booleans.
- Excel uploads are capped at 1 MB/500 rows, reject formulas, extra columns, invalid templates and unsafe compressed structures, then reuse the same per-row preview, assignment/domain/concurrency checks and atomic bulk-save authority as CSV.
- Added separate permission-protected Excel export, preview and confirmation actions, localized responsive controls and a distinct immutable `ExcelImport` history source while retaining existing post-import next-step guidance.
- Pinned the .NET package family to the repository's existing 10.0.12 baseline and overrode the transitive PKCS package with patched 9.0.21 after the current package audit flagged the older version.
- The Release solution build passes with zero warnings/errors and all 672 focused security/inventory checks pass. No database migration is required.

# 2026-10-14 — PUR-230 supplier price exception report

- Added a deterministic active-row analyzer for expired prices, prices expiring within 30 days, duplicate case-insensitive supplier product codes within one supplier and multiple preferred purchasing rows for one product.
- Added global exception counts to Supplier Purchasing and a permission-protected, filterable responsive card report with direct supplier-data and immutable-history links.
- Duplicate findings are advisory: the report never merges, deactivates or reprices records automatically, and exact supplier/product/unit duplicates remain blocked by the database.
- The Web project builds with zero warnings and all 669 focused security/inventory checks pass.

# 2026-10-14 — PUR-230 immutable supplier price history

- Added append-only supplier-price revisions that freeze the actor, timestamp, change source, supplier/product/purchase-unit identity, minimum quantity, base price, currency, validity and complete quantity-break set.
- New supplier-product assignments record their initial price. Existing records automatically preserve their current baseline immediately before the first actual price or tier change; non-price and unchanged saves do not create noise.
- Manual, bulk, CSV and price-break workflows share the same snapshot authority and remain inside their existing atomic/concurrency-protected save boundary.
- Added a permission-protected read-only responsive history page with no edit/delete action and an accepted ADR for full-snapshot history ownership.
- Added migration `AddSupplierPriceHistory`; the Web and SQL integration projects build with zero warnings, all 667 focused checks pass and EF reports no pending model changes.

# 2026-10-14 — PUR-230 supplier quantity price breaks

- Added tenant-owned supplier/product/unit price-break rows with unique minimum-quantity thresholds, optional discounts and a 20-tier aggregate limit.
- The domain resolves the highest eligible purchase-unit tier, applies its discount and falls back to the supplier base price when no threshold qualifies.
- Added a localized responsive editor with explanation tooltips, permission-separated view/manage actions, no horizontal table and explicit return guidance after saving.
- Replacing tiers advances the parent revision and rowversion so concurrent edits cannot silently overwrite another user's changes; inactive terms cannot be edited.
- Added migration `AddSupplierQuantityPriceBreaks`; the Web project builds with zero warnings and all 659 security/inventory regression checks pass.

# 2026-10-14 — PUR-230 supplier-price CSV foundation

- Added supplier-scoped UTF-8 CSV export using the current active purchasing rows as a reusable template; exported cells neutralize spreadsheet-formula prefixes and remain round-trip importable.
- Added a permission-protected upload preview capped at 1 MB and 500 rows. Responsive cards show Create/Update status and localized row-specific errors without horizontal scrolling.
- Preview and confirmation resolve product/unit codes, require an active supplier-product assignment, validate managed-unit compatibility and every commercial field, detect duplicate/preferred conflicts and preserve rowversion concurrency.
- Confirmation reparses the original CSV server-side and uses the existing bulk transaction, so one invalid or concurrently changed row prevents all writes.
- Added explicit post-import next-step guidance back to the same supplier and regression coverage for permissions, CSV quoting, formula protection, template rejection and guidance.

# 2026-10-13 — PUR-220 mixed purchase billing completion

- Completed policy-aware Vendor Bill sourcing: a single Draft can allocate posted Goods Receipt lines, ordered-quantity Purchase Order lines and immutable Service Completion lines while active Draft/Approved/Posted bills reserve the appropriate source quantity.
- Posting now clears GRNI only for receipt-policy lines, isolates their purchase-price/currency variance, and debits each ordered/service line's frozen purchase Expense account; tax and supplier payable cover the complete mixed bill.
- Purchase Order settlement/automatic closure now derives billed quantity per order line for all three policies. Receipt returns, reversals and stock-return credit notes remain safely scoped to receipt-based allocations.
- Replaced Vendor Bill line tables with responsive source cards, exposed localized billing-policy/source context, and removed the create action when no billable quantity remains.
- Agreement releases now freeze the product's current billing policy and required Expense account on the generated Purchase Order line.
- Added migration `AddMixedPurchaseBillingSources`, safe existing-line backfill, filtered source uniqueness and tenant-composite service/account relationships.
- Verified the Web and regression projects build with zero warnings; all 645 security/inventory regression checks pass. The SQL integration executable could not connect because the local SQL Server Windows SSPI context failed before database setup.

## 2026-10-13 — PUR-220 staged Vendor Bill review

- Replaced controller-level immediate posting with Draft entry, matching approval and Approved-only posting, each protected by a separate permission and rowversion.
- Drafts reserve receipt quantities; normalized supplier invoice identity is rechecked at entry, approval and posting.
- Added up to ten 5 MB tenant-owned attachments, audited download, responsive no-horizontal-scroll entry cards and localized next-step guidance.
- Added migration `AddVendorBillReviewWorkflow`. Mixed Ordered Quantity and Service Completion bill sources remain the next PUR-220 increment.

## 2026-10-13 — PUR-220 service procurement foundation

- Added Service products plus Ordered Quantity, Received Quantity and Service Completion purchase-billing policies, with a required Expense account for policies that bypass Goods Receipt.
- Froze policy/account authority on new Purchase Order lines and extended lifecycle progress to distinguish physical receipt from completed service.
- Added tenant-safe, separately numbered Purchase Service Acceptance evidence, permissions, navigation, bilingual responsive screens and next-step guidance. The operation records no stock, tracking, movement, valuation or journal effect.
- Restricted manual and scanned Goods Receipts to receipt-policy lines backed by active Stocked products.
- Added migration `20261013162045_AddPurchaseServiceBillingPolicies`; existing products and order lines default safely to Received Quantity. Draft/review/post Vendor Bills and attachments remain the next PUR-220 slice.

## 2026-10-13 — PUR-210 line-level split supplier awards

- Replaced whole-quotation winner selection with exact line-level quantity allocations that freeze supplier, quoted price, discount, tax, currency, lead time, payment terms, product/unit snapshots and a required selection reason.
- Rebuilt quotation comparison as responsive supplier cards, enforced complete requested quantities without over-award, closed completed sourcing events and replaced repeated action buttons with completed allocation evidence.
- Created one Draft Purchase Order per awarded supplier quotation and exposed every generated order; single-supplier Purchase Agreements now reject split or incomplete awards.
- Added and applied migration `20261013104238_AddLineLevelSplitAwards`, including safe backfill of existing awards, Arabic localization, next-step guidance and the architectural decision record.
- Release builds passed with zero warnings/errors; 630 focused regression checks passed, and SQL integration proved the 3-product/2-supplier scenario creates two orders with exact quantities and no duplication.

## 2026-10-13 — PUR-200 Flexible RFQ Workspace and Fast Path

- Added a separately authorized direct RFQ path alongside approved Purchase Request sourcing. A tenant policy gates it by enable state, estimated company-currency amount, optional tenant role and supplier active state; the Application service rechecks permission and policy before saving.
- Direct events freeze the applied policy amount/role and commercial context without fabricating approval history. They still require the existing quotation, award and Purchase Order approval/confirmation steps, so the fast path cannot authorize an order.
- Both sourcing paths now capture supplier-response deadline, expected delivery, buyer reference, estimate, notes and bounded tenant-owned attachments. Details include protected downloads and a bilingual print view that can be printed or saved as PDF.
- Added responsive card-based sourcing listing, the bilingual policy Settings screen, actionable policy/limit links, exact next-step guidance, the dedicated `PurchaseSourcing.CreateDirect` permission and ADR `2026-10-13-direct-rfq-policy.md`.
- Added and applied migration `20261013101055_AddFlexibleRfqWorkspace`; it safely backfills existing events as approved-request sourcing and preserves optional-source tenant constraints. Entity Framework reports no pending model changes.
- Release builds passed with zero warnings, 623 focused regression checks passed and the disposable SQL Server integration suite passed.

## 2026-10-13 — PUR-190 Multi-Currency Purchasing

- Added tenant-owned dated exchange rates with source/audit evidence and a bilingual Settings screen. Rates use company-currency units per one supplier-currency unit and resolve latest-on-or-before each document date. Supplier quotation entry now selects from configured currencies and comparison ranks normalized company-currency totals rather than incomparable raw amounts.
- Confirmed Purchase Orders and posted Goods Receipts, Vendor Bills and Supplier Payments now freeze transaction/company currencies, rate, rate date and source; detail screens show both document value and company-currency equivalent.
- Inventory and GRNI use the receipt-date value. Vendor Bills separate commercial Purchase Price Variance from receipt-to-bill exchange variance, while Supplier Payments preserve payable carrying value and post realized exchange gain/loss against payment-date cash value.
- Supplier Credit Notes preserve proportional frozen bill company values, receipt returns/reversals use receipt-rate inventory value, and missing-rate/account errors link to the correct Settings page.
- Added realized exchange gain/loss account settings, ADR `2026-10-13-purchase-multi-currency-policy.md` and migration `20261013030745_AddPurchaseMultiCurrency`, including safe rate-one backfill for legacy documents. The migration was applied to local `MiniStoreDb`.
- Release build passed with zero warnings, 610 focused checks passed, the SQL integration suite passed, and Entity Framework reported no pending model changes.

## 2026-10-13 — PUR-180 Controlled Purchase Document Reversal

- Added immutable tenant-owned `PRV-` reversal records for posted Goods Receipts, Vendor Bills, Supplier Payments and Landed Costs, with mandatory reason, actor/time, original identity, opposite journal number and request-key idempotency.
- Preserved originals as Reversed and posted exact debit-credit opposite journals in an open fiscal period; reconciliation now includes both the original and opposite entries so their ledger effect nets to zero.
- Enforced reverse-order dependencies: payments before bills, and bills/returns/landed costs before receipts. Receipt and landed-cost reversals also require the original quantity and valuation to remain safely reversible.
- Added physical receipt-reversal movement/transaction evidence, landed-cost value reversal, restored Vendor Bill availability after payment reversal and automatic Purchase Order reopening when reversing a completing receipt or bill.
- Added separate View/Create permissions, bilingual responsive history/create/detail screens, original-document completion links, localized errors, workflow guidance and ADR `2026-10-13-controlled-purchase-document-reversal.md`.
- Added and applied migration `20261013024801_AddPurchaseDocumentReversals`; Entity Framework reports no pending model changes. Release build has zero warnings, 604 focused regression checks passed and the disposable SQL Server fixture proved idempotent payment reversal, opposite journal and restored bill payment availability.

## 2026-10-13 — PUR-170 Supplier Credit Notes and Billed Returns

- Added tenant-owned immutable Supplier Credit Notes linked to the original Vendor Bill, Purchase Order, Goods Receipt lines, supplier and warehouse, with separate View/Create permissions and centralized `SCN-` numbering.
- Cumulative credited quantity cannot exceed each billed line. Partial credits freeze proportional net/tax/gross values; the final allocation consumes exact remaining values to avoid rounding drift.
- Posting removes current available stock through existing location and lot/serial controls, records a dedicated stock transaction and outbound physical movement, debits supplier payable, reverses frozen input tax, credits current inventory cost and balances the difference through Purchase Price Variance.
- Vendor Bill and Supplier Payment balances now subtract posted credit totals. Completed credit actions disappear and bill details show paid, credited and remaining amounts with exact next-step guidance.
- Added responsive bilingual list/create/detail screens, actionable setup links, Arabic resources, migration `20261012125000_AddSupplierCreditNotes`, and applied it to the local `MiniStoreDb` without historical backfill.
- 596 focused regression checks passed. The disposable SQL Server fixture proved a fully paid billed return creates the expected supplier credit balance, removes the final stock, records movement evidence and posts a balanced payable/inventory/PPV journal.

## 2026-10-12 — PUR-165 Derived Purchase Order Closure

- Added per-line derived Purchase Order progress from posted Goods Receipts, receipt returns and Vendor Bill receipt-clearing quantities.
- Fully received and fully settled confirmed orders now close automatically inside the final bill/return transaction. Partial orders can close their unreceived remainder only after received quantities are settled and a mandatory reason is supplied.
- Added a separately authorized, reason-required reopen command that applies only to remaining-quantity closures; automatically completed orders remain closed.
- Added tenant-owned immutable lifecycle history plus current closure metadata, migration `20261012122525_AddPurchaseOrderDerivedClosure`, and applied it to the local `MiniStoreDb` without historical backfill.
- Reworked Purchase Order details into responsive progress cards without horizontal scrolling. Completed receipt/bill actions disappear and are replaced by completion states; close/reopen success messages provide permission-aware next-step links.
- The Web project built with zero warnings, 586 focused regression checks passed, Entity Framework reports no pending model changes, and the disposable SQL Server integration fixture proved final return settlement creates exactly one automatic closure event.

## 2026-10-11 — PUR-150 Barcode-Assisted Goods Receiving

- Added empty Draft Goods Receipt creation and a bilingual scanner-focused screen that resolves exact tenant product barcode/code matches only within the selected confirmed Purchase Order.
- Scans accumulate received quantity without changing stock, enforce open-order caps and lot/serial input rules, and reject ambiguous or unrelated products.
- Added immutable tenant-owned scan evidence with workflow-scoped, tenant-unique retry keys. Replaying the same receipt/token/quantity request is a no-op; reusing its key for a different receipt, product token or quantity is rejected.
- Final posting revalidates the draft and delegates to the existing atomic Goods Receipt inventory, tracking, AVCO, physical movement and Inventory/GRNI journal path.
- Added migration `20261011113457_AddGoodsReceiptBarcodeScanning` and applied it to `AHMAD/MiniStoreDb` without backfill or historical receipt changes.
- Added domain, permission and retry-key checks plus a disposable SQL Server scenario proving duplicate-scan idempotency, zero draft stock effect and one atomic stock/movement/journal effect after posting.
- Release build passed without warnings, 545 focused checks passed, the migration model is current, Arabic resources have no duplicate or missing receipt-screen keys, and the disposable SQL Server integration fixture passed with the scanned-draft scenario.

## 2026-10-11 — PUR-140 Purchasing Analytics and Supplier Performance

- Added a permission-protected, bilingual purchasing analytics screen with a bounded date filter and cards for current open/overdue orders, receipt value, landed costs, billed spend, current payables and matching exceptions/overrides.
- Added supplier projections for order count/value, fulfilment, on-time delivery, average delivery days, returns, billed spend, price variance, current outstanding balance and matching exceptions.
- Defined supplier operational rates from the selected-period commercial order cohort plus all posted receipt/return evidence for that cohort; financial period measures follow bill, receipt or landed-cost dates as appropriate.
- Reused authoritative tenant-filtered repositories with no reporting tables, migration, inventory mutation or accounting side effect; added a dedicated read permission and purchasing navigation entry.
- Added focused rate/permission checks and extended the disposable SQL Server integration fixture to verify analytics against real receipt, return, bill, payment, matching and landed-cost records.
- Release build passed without warnings, 540 focused checks passed, Arabic resources had no duplicate or missing analytics keys, and the disposable SQL Server integration fixture passed including the supplier analytics projection.

## 2026-10-11 — PUR-130 Landed Costs

- Added immutable, centrally numbered Landed Cost documents against posted Goods Receipts with freight, customs, insurance and other component snapshots.
- Costs allocate automatically by base-stock quantity or frozen receipt value. The current-stock share raises AVCO through a zero-quantity StockTransaction; the consumed share is frozen as Cost of Sales expense.
- Posting atomically debits Inventory and/or Cost of Sales and credits the configured Landed Cost Clearing account through the fiscal-period and idempotency gateway.
- Added normalized tenant-unique external references, tenant-safe relationships, separate view/create permissions, bilingual list/create/detail screens and a Goods Receipt entry point.
- Created and applied migration `20261011105547_AddLandedCosts` to `AHMAD/MiniStoreDb`; no historical receipt was revalued or backfilled.
- Release build passed without warnings, 538 focused checks passed, and the disposable SQL Server integration fixture verified the value-only AVCO adjustment, capitalization/expense split and balanced journal posting.

## 2026-10-11 — PUR-120 Purchase Agreements and Repeat Templates

- Added centrally numbered, tenant-owned Purchase Agreements created only from a recorded winning quotation, with frozen terms, validity, activation/cancellation audit and rowversion protection.
- Agreement releases recalculate cumulative non-cancelled quantities inside a Serializable transaction, reject quantities above the contracted remainder and create linked Draft Purchase Orders that still require normal approval and confirmation.
- Added repeat Purchase Templates saved from an existing Purchase Order. Every generation increments a protected counter, revalidates current purchasability and creates a distinctly traced Draft Purchase Request without bypassing submission, approval, sourcing or award.
- Added separate agreement/template permissions, purchasing navigation, bilingual list/create/detail actions and source links from quotation comparison and Purchase Order details.
- Created migration `20261010190513_AddPurchaseAgreementsAndTemplates`; it was verified against the current model and is present on `AHMAD/MiniStoreDb` with no historical backfill.
- Release build passed with zero warnings, 503 focused checks passed, and the disposable SQL Server integration fixture passed its full tenant, concurrency, inventory and purchasing-accounting suite.

## 2026-10-10 — PUR-110 Replenishment Purchase Demand

- Added an explicit reviewed-suggestion action that creates a Draft Purchase Request only for active purchase-sourced replenishment rules that are still below their minimum.
- Valid preferred supplier terms select the purchase unit and suggested supplier, round demand up to the configured minimum and order multiple, and use the longer rule/supplier lead time.
- Purchase Requests now preserve optional source type/reference provenance. A filtered tenant-unique index and Serializable source check prevent concurrent duplicate active requests while allowing replacement after cancellation.
- The action requires both replenishment-management and purchase-request-create permissions; the replenishment screen links to the open request and the request detail identifies its reviewed source.
- Created and applied migration `20261010184007_AddReplenishmentPurchaseDemand` to `AHMAD/MiniStoreDb`; existing Purchase Requests remain unchanged.
- Release build passed without warnings, 503 focused checks passed and the disposable SQL Server fixture accepted exactly one of two concurrent requests for the same replenishment source.

## 2026-10-10 — PUR-100 Supplier Payments

- Added tenant-owned, centrally numbered Supplier Payments with immutable partial allocations across posted Vendor Bills for one supplier and currency.
- Outstanding balances are recomputed inside the Serializable transaction; allocations above the remaining bill balance are rejected, including competing concurrent full-balance attempts.
- Posting atomically debits the supplier payable account and credits the cash/bank account mapped by the selected active payment method through the central fiscal-period and idempotency gateway.
- Added separate view/create permissions, purchasing navigation, a Vendor Bill entry point and bilingual list/create/detail screens.
- Created and applied migration `20261010182545_AddSupplierPayments` to `AHMAD/MiniStoreDb`; no historical payments were invented.
- Release build passed with zero warnings, 500 focused checks passed and the disposable SQL Server fixture proved exactly one of two concurrent full-balance payments succeeds with one balanced settlement journal.

## 2026-10-10 — PUR-090 three-way matching

- Added tenant-configurable quantity and net-price tolerance percentages with a bilingual Settings screen and rowversion-protected singleton persistence.
- Every posted Vendor Bill now owns an immutable match run. Quantity over-allocation and tax-exclusive price differences above tolerance become line exceptions and block normal posting.
- Added the distinct `VendorBills.OverrideMatch` permission. Authorized overrides require and persist a reason, actor and timestamp together with expected/actual values, variance and the tolerance snapshot used.
- Extended receipt clearing to freeze the quantity actually backed by the receipt; permitted or overridden excess quantity is isolated from GRNI clearing and flows through the existing variance posting.
- Created and applied migration `20261010180946_AddPurchaseThreeWayMatching` to `AHMAD/MiniStoreDb`; existing bill lines are safely initialized from their billed quantity, while historical bills are not assigned invented match decisions.
- Release build passed without warnings, 492 focused checks passed and the disposable SQL Server fixture persisted a price exception and reasoned override atomically with the single winning concurrent Vendor Bill.

## 2026-10-10 — PUR-080 Vendor Bills

- Added tenant-owned Vendor Bills with partial posted-receipt allocations, frozen price/tax/receipt-clearing snapshots and centralized `VB-` document numbering.
- Active supplier invoice identity is normalized and protected by a database unique index scoped to tenant and supplier. The Application service also checks cumulative returned and billed quantities inside the Serializable transaction.
- Posting atomically debits GRNI at receipt value, records positive or negative Purchase Price Variance, debits snapshotted input tax and credits the supplier's payable account. Receipt returns now reject quantities already billed so the unbilled GRNI reversal cannot corrupt Accounts Payable; billed returns await a supplier credit-note slice.
- Added separate view/create permissions, navigation, bilingual list/create/detail screens and an order-detail entry point.
- Created and applied migration `20261010125824_AddVendorBills` to `AHMAD/MiniStoreDb`; no historical bills were invented or backfilled.
- Release build passed with zero warnings, 483 focused checks passed and the disposable SQL Server fixture proved exactly one of two concurrent duplicate invoices succeeds with one balanced bill journal; concurrent returns remain limited to the unbilled balance.

## 2026-10-10 — PUR-070 receipt-based supplier returns

- Added tenant-owned, GRR-numbered immutable returns against posted new-flow Goods Receipts while preserving the legacy Purchase Return model and routes.
- Return posting caps cumulative unbilled receipt-line quantity, converts to base stock quantity, removes available untracked or lot/serial inventory, writes StockTransaction and `ReceiptReturnOut`, then debits GRNI at original receipt value, credits Inventory at actual AVCO removal cost and posts any difference to the new tenant-safe Purchase Price Variance account.
- Added separate permissions, bilingual list/create/detail UI, navigation, domain/metadata/controller checks and SQL integration proving concurrent attempts cannot exceed the remaining unbilled receipt quantity and keep linked journals balanced.
- Created and applied migration `20261010124312_AddGoodsReceiptReturns` to `AHMAD/MiniStoreDb`; the migration is additive and does not backfill or mutate historical quantities.

## 2026-10-10 — PUR-060C SQL concurrency completion

- Extended the disposable SQL Server fixture with a complete tenant-owned request/sourcing/quotation/confirmed-PO setup and two concurrent full-quantity Goods Receipt attempts.
- Proved exactly one attempt succeeds and atomically leaves one posted GRN, one net-discount-cost ProductStock/StockTransaction update, one physical `ReceiptIn` movement and one balanced Inventory/GRNI journal; the losing attempt leaves no partial facts.
- The Release integration build passed without warnings and the full disposable SQL fixture passed. PUR-060 is complete; controlled receipt reversal and inventory-to-GL reconciliation reporting remain in PUR-065/PUR-070.

## 2026-10-10 — PUR-065A GRNI receipt posting

- Added the tenant-safe GRNI account mapping to Accounting Settings and applied `AddGoodsReceiptGrniAccount` to `AHMAD/MiniStoreDb`.
- Positive-value Goods Receipts now create one idempotent, fiscal-period-validated journal in the receipt transaction: debit warehouse Inventory and credit GRNI. Receipt valuation uses the frozen PO unit price after line discount; zero-value receipts do not create invalid zero-amount journal lines.
- Added regression coverage for the tenant-safe GRNI mapping, balanced receipt journal invariant and central journal-gateway dependency. Vendor-bill clearing, receipt reversal and inventory-to-GL reconciliation remain later work.

## 2026-10-10 — PUR-060A goods receipt posting foundation

- Added tenant-safe Goods Receipt/line records, GRN numbering, a partial-receipt screen and permissions. Receipt lines freeze PO evidence and accept lot/serial/manufacture/expiry values.
- Posting occurs in one Serializable transaction, recomputes the open quantity inside that transaction, then updates ProductStock moving average, tracked identity balances and immutable StockTransactions. No GRNI journal is created.
- Applied `AddGoodsReceiptFoundation` to `AHMAD/MiniStoreDb`; it is additive and creates no demo receipt data.

## 2026-10-10 — PUR-060B receipt history and physical movement

- Added bilingual Goods Receipt list/detail visibility and a `ReceiptIn` StockMovement for every posted GRN line, linked to the persisted document and line with a retry-safe idempotency key.
- Extended the existing UnitOfWork with an in-transaction flush only so the receipt identity can be assigned before the physical movement is written; the outer transaction still commits or rolls back every effect together.

## 2026-10-10 — PUR-050B purchase order operation

- Completed the Purchase Order list/detail operation with status/search filtering, supplier and warehouse context, navigation and permission-aware actions.
- Moved order creation to a single action after quotation selection, preventing the misleading appearance that a different unawarded quote could create an order.
- Confirmed orders remain commercial intent only; receipt/billing-derived closure begins with PUR-060.

## 2026-10-10 — PUR-050A purchase order foundation

- Added a distinct, tenant-safe PurchaseOrder aggregate sourced only from the reasoned awarded supplier quotation, with frozen commercial lines and independent `PO-` numbering.
- Added Draft/Approved/Confirmed/Cancelled state transitions, rowversion protection, permissions and a detail screen. Confirmation deliberately changes neither stock nor accounting.
- Applied `AddPurchaseOrderFoundation` to `AHMAD/MiniStoreDb`; its only legacy-schema addition is the tenant composite key required for quotation-line references.

## 2026-10-10 — PUR-040B supplier quotation capture and comparison

- Added a bilingual capture screen limited to suppliers invited by the sent sourcing event. It requires one quoted line per frozen sourcing line and records the tenant base currency from general settings.
- Submitted quotations are immutable; comparison derives net, tax and gross totals only from quote snapshots rather than mutable supplier master data.
- Added a one-time PurchaseQuotationAward record with mandatory business reason, actor and timestamp, plus independent view/create/award permissions.
- Applied `AddPurchaseQuotationAward` to `AHMAD/MiniStoreDb`; the award remains commercial intent and does not create an order, receipt, stock movement or journal entry.

## 2026-10-08 — PUR-040A supplier quotation foundation

- Added tenant-owned supplier quotation headers and lines, one quote per sourcing event/supplier, and rowversion protection for the mutable Draft document.
- Quote lines freeze quoted quantity, unit price, discount, tax and product/unit identifiers. Submitted quotes are immutable and their net/tax/gross comparison totals are calculated only from their stored snapshots.
- Added and applied `AddSupplierQuotationFoundation` to `AHMAD/MiniStoreDb`; the additive migration does not create quotes, orders, stock movements or accounting entries.
- The focused regression suite now covers quotation normalization, calculated totals, submitted-document immutability and tenant ownership.

## 2026-10-08 — PUR-030 sourcing event and supplier invitations

- Added an RFX-numbered sourcing event created from exactly one approved Purchase Request, with frozen request-line product, unit and quantity snapshots.
- Added deduplicated supplier invitations and a send transition that requires both demand lines and at least one selected supplier.
- Added a tenant-safe repository, Application service, separate sourcing view/create permissions, bilingual list/create/details UI and a Request Details entry point.
- Kept PUR-030 outside inventory and accounting: it creates no quotation price, Purchase Order, receipt, stock movement or journal entry.
- Added and applied the additive `AddPurchaseSourcingFoundation` migration to `AHMAD/MiniStoreDb`; Release build passed without warnings, and the focused regression suite passed.

## 2026-10-08 — PUR-025B configurable approval workflow

- Added bilingual purchase approval rule administration with warehouse/global scope, priority ranges and up to three ordered tenant-role steps.
- Purchase Request submission now deterministically selects an active rule (warehouse-specific before global, then narrowest priority range), freezes its steps and rejects submission when no rule matches.
- Added separate rule view/manage and request approve/reject permissions. Each decision also verifies that the actor belongs to the role frozen on the current step.
- Final approval or rejection updates Purchase Request status/history atomically; cancellation closes a pending approval without stock or accounting effects.
- Release build passed without warnings and all 428 focused regression checks passed. No additional migration was required beyond PUR-025A.

## 2026-10-08 — PUR-025A purchase approval foundation

- Added tenant-owned, warehouse/priority-scoped Purchase Approval Rules with ordered role steps and rowversion concurrency.
- Added per-request approval instances that freeze the selected rule name and ordered roles so later configuration changes cannot rewrite historical decisions.
- Added ordered approve/reject domain behavior with actor, timestamp, note/reason and terminal completion state, plus Approved/Rejected Purchase Request transitions and history.
- Added and applied the additive `AddPurchaseApprovalFoundation` migration to `AHMAD/MiniStoreDb`; no default rules, inventory movements or accounting entries were created.
- Release build passed with zero warnings and all 425 focused regression checks passed.

## 2026-10-07 — WMS-100B3 scanned inventory counting

- Added exact Draft adjustment-number scanning and absolute per-product count entry through the existing inventory-adjustment application boundary.
- Kept scanned documents Draft until the ordinary all-line completion action, leaving approval, posting and inventory variance mutation under their separate permissions.
- Preserved blind-count secrecy by hiding current balances in the scan console and prefilled saved scanned values on the completion form.
- Added matching-position and included-product validation plus dual scan/count authorization. No migration was required; Release build passed without warnings, 378 focused checks and the updated disposable SQL suite passed repeated absolute scan-count persistence, and EF reports no pending model changes.

## 2026-10-07 — WMS-100B2 idempotent scanned relocation

- Added exact source-location scanning and permission-gated relocation to the inventory scan console.
- Delegated mutation to `LocationMovementService`, retaining same-warehouse, pick/receive capability, source stock, capacity, Serializable transaction and movement dual-write controls.
- Added a relocation-specific retry-key namespace that cannot be substituted with a putaway key, plus dual-permission and antiforgery enforcement.
- No migration was required; Release build passed without warnings, 376 focused checks and the disposable SQL integration suite passed, and EF reports no pending model changes.

## 2026-10-07 — WMS-100B1 idempotent scanned putaway

- Added the first scan mutation: permission-gated putaway from Unassigned into one exactly scanned destination location.
- Reused `UnassignedStockService` so warehouse mode, receivability, capacity, unassigned quantity, Serializable transaction and movement dual-write controls remain authoritative.
- Added workflow-scoped retry keys; replaying the same facts is harmless while cross-workflow or changed-fact key reuse is rejected.
- No migration was required; Release build passed without warnings, 373 focused checks and the disposable SQL integration suite passed, and EF reports no pending model changes. The focused test output was isolated from a separately running process that held its normal output files.

## 2026-10-07 — WMS-100A read-only inventory scanning

- Added a bilingual, permission-protected scan console for exact product barcode/code and storage-location barcode/code lookup.
- Normalized scanner input, rejected ambiguous identifiers and prevented invalid location scans from widening results to all locations.
- Displayed the existing OnHand, Reserved and Available projection without creating a stock movement, reservation or document.
- No migration was required; Release build passed without warnings, 370 focused checks and the disposable SQL integration suite passed, and EF reports no pending model changes.

## 2026-10-07 — WMS-090D inventory activity insights

- Added a bilingual, permission-protected, read-only report for slow and dead positive inventory with configurable inactivity thresholds.
- Classified activity from actual outbound StockTransaction history and separated received-but-never-issued stock from positive balances with no movement history.
- Added current and attention-value summaries plus warehouse, product and status filters without changing stock or accounting values.
- No migration was required; Release build passed without warnings, 367 focused checks and the disposable SQL integration suite passed, and EF reports no pending model changes.

## 2026-10-07 — WMS-090B2 reviewed transfer draft

- Added explicit conversion of a still-needed preferred-source replenishment suggestion into a Draft StockTransfer.
- Required both replenishment-management and transfer-create permissions and revalidated projected shortage, source availability, branch access and exact-location policies.
- Added capacity-aware destination suggestion, single-position source resolution and duplicate open-intent protection through the normal transfer service.
- No submit, approval, reservation, posting or stock change is automatic. No migration was required; Release build passed without warnings, 364 focused checks and SQL integration passed, and EF reports no pending model changes.

## 2026-10-07 — WMS-090B1 confirmed transfer forecast

- Added confirmed incoming, confirmed outgoing, projected OnHand and projected Available to replenishment suggestions using Approved transfers only.
- Avoided double-counting outbound: Approved transfer quantity is displayed as outgoing but is already removed from Available through its reservation.
- Excluded drafts and terminal transfer states, and documented why the immediate-receipt Purchase aggregate is not future incoming.
- No migration was required; Release build passed without warnings, 363 focused checks and SQL integration passed, and EF reports no pending model changes.

## 2026-10-06 — WMS-090A replenishment planning

- Added tenant-safe per-product/destination min-max, safety-stock, lead-time and optional preferred-source rules.
- Added bilingual permission-separated administration and reservation-aware suggestions that replenish Available to Maximum when it reaches Minimum.
- Suggestions remain advisory and create no stock movement or business document automatically.
- Created and applied migration `AddReplenishmentRules` to `AHMAD/MiniStoreDb`; Release build passed without warnings, 362 focused checks and SQL integration passed, and EF reports no pending model changes.

## 2026-10-06 — WMS-080C3 untracked location removal

- Added a central reservation-aware allocator for non-tracked inventory issues across active pickable exact locations and Unassigned.
- Connected sales, recipe consumption, supplier returns and legacy aggregate adjustment-out paths so warehouse deductions no longer leave stale rack/bin quantities.
- Applied warehouse LocationPriority and MinimizeLocations policies; deterministic location order is the documented fallback where untracked inventory has no FIFO/FEFO receipt layer.
- Preserved controlled recipe-negative shortages in Unassigned and left lot/serial removal under the existing tracking engine.
- No migration was required; Release build passed without warnings, 357 focused checks and the disposable SQL integration suite passed, and EF reports no pending model changes.

## 2026-10-06 — Web composition-root cleanup

- Reduced `Program.cs` to a readable composition entry point and moved presentation, persistence, security, feature DI, startup seeding and middleware/routes into focused Web configuration extensions.
- Kept registrations explicit and grouped by business responsibility, preserving service lifetimes and middleware order.
- Removed the duplicate `ISaleRepository` and `ISaleService` registrations; no database or business behavior changed.
- No migration was required; Release build passed without warnings and 353 focused checks passed.

## 2026-10-06 — WMS-080C2 managed putaway suggestions

- Added ProductStock.Edit-protected putaway-rule creation and active-state management to the Unassigned Stock screen.
- Added product/category/default suggestion display and preselection while excluding inactive, non-receivable, cross-warehouse and insufficient-capacity destinations.
- Added bilingual UI and validation messages plus controller authorization and rule lifecycle regression checks.
- No schema migration was required; Release build passed without warnings, 353 focused checks passed and EF reports no pending model changes.

## 2026-10-06 — WMS-080C1 putaway-rule foundation

- Added tenant-safe warehouse putaway rules targeting a product, category or warehouse default location with explicit priority and active state.
- Added deterministic suggestion resolution and active/receivable-location fallback without changing stock authority or automatically moving quantities.
- Created and applied migration `AddPutawayRules` to `AHMAD/MiniStoreDb`; Release build passed without warnings and 351 focused checks passed.

## 2026-10-06 — WMS-080B3 POS tracked selection

- Added a compact optional lot/serial field to POS cart rows only when the selected product uses tracking.
- Reused the shared Sale DTO and InventoryTracking issue validation, so POS and wholesale have identical quantity, availability, expiration and audit rules.
- Rejected crafted manual selections for untracked or prepared-to-order sale lines and added Arabic validation/UI resources.
- No schema migration was required; Release build passed without warnings and 347 focused checks passed.

## 2026-10-06 — WMS-080B2 transfer identity override

- Persisted optional lot/serial allocation text on transfer draft lines and exposed it on create, edit and details screens.
- Posting applies exact-quantity, source-position, expiration and availability validation and marks resulting trace rows Manual.
- Cancellation reconstructs the original delivered identities from immutable positive destination trace rows, preventing substitution by a new automatic allocation.
- Created and applied migration `AddTransferTrackingAllocations` to `AHMAD/MiniStoreDb`; Release build, 346 focused checks and SQL integration passed.

## 2026-10-06 — WMS-080B1 audited manual tracked removal

- Added optional exact lot/serial selection to wholesale sale lines with exact-quantity, availability, expiration and serial uniqueness validation.
- Persisted Manual or effective warehouse removal strategy on new tracking transactions and exposed it in tracking history.
- Added bilingual wholesale UI and Arabic resources; existing rows remain null rather than receiving invented audit values.
- Created and applied migration `AddTrackedRemovalStrategyAudit` to `AHMAD/MiniStoreDb`.
- Release build passed without warnings, 345 focused checks and the disposable SQL integration workflow passed, and EF reports no pending model changes.

## 2026-10-06 — WMS-080A warehouse removal strategies

- Added one deterministic allocator for tracked FIFO, FEFO, location-priority and minimize-locations removal.
- Sales, tracked recipe consumption and stock transfers now use the source warehouse policy while continuing to exclude expired/quarantined stock.
- Kept Manual warehouses on deterministic FIFO compatibility until an explicit audited identity override is available.
- No schema migration was required; the Release build passed without warnings, 344 focused checks passed and the disposable SQL integration workflow passed.

## 2026-10-06 — Recall communication evidence

- Added append-only recall communication entries for party, contact address, Phone/Email/SMS/In-person/Other channel, Attempted/Reached/Confirmed/Failed outcome, notes, actor and time.
- Restricted new entries to Active recalls and retained the log after closure.
- Added bilingual recall UI and tenant-safe child mapping. Migration `AddInventoryRecallCommunicationLog` is applied to `AHMAD/MiniStoreDb`; 341 focused checks and the disposable SQL workflow passed.

## 2026-10-06 — Recall affected-party report

- Resolved recall trace references into purchase, sale, transfer and return impact rows.
- Purchase impacts show supplier/phone; sale impacts show the named customer or walk-in classification.
- Aggregated affected quantity per document and avoided double-counting balanced transfer legs.
- Added bilingual UI and focused/SQL coverage without changing the database schema.

## 2026-10-05 — Live expiration alert center

- Added an urgency-ordered, tenant-safe alert projection for positive expired and expiring-soon tracked balances.
- Alerts use each product's warning horizon and expose product, lot/serial, warehouse/location, quantity, expiration, remaining days and quarantine state.
- Kept alerts live rather than duplicating persisted notification rows; scheduled delivery channels can consume this source later.
- Added bilingual UI plus focused and SQL integration coverage.

## 2026-10-05 — Inventory recall workflow

- Added tenant-owned active/closed recall cases for one Product plus lot/serial identity, with reason, closure notes, user/time audit and concurrency token.
- Recall initiation atomically quarantines every available positive balance for the identity across positions and reports affected receipt, issue, transfer and return references from immutable tracking history.
- Recall closure deliberately preserves quarantine; release remains a separate quality decision.
- Added dedicated recall permission, bilingual UI and ADR. Migration `AddInventoryRecallWorkflow` is applied to `AHMAD/MiniStoreDb`; 335 focused checks and the disposable SQL workflow passed.

## 2026-10-04 — Lot and serial quarantine workflow

- Added reasoned Available → Quarantined → Available transitions without changing physical quantity.
- Quarantine/release writes immutable zero-quantity trace events and uses the dedicated `InventoryTracking.ManageQuarantine` permission.
- Quarantined balances remain excluded from sales/transfers, and additional receipts/returns cannot silently release a quarantined lot.
- Added and applied `AddInventoryTrackingQuarantineEvents` to `AHMAD/MiniStoreDb`; controller authorization, domain and SQL integration coverage passed. The normal startup seeder grants the new permission to protected tenant Admin roles.

## 2026-10-04 — Product shelf-life controls and expiry alerts

- Added product-level default shelf-life days, mandatory expiration and warning-horizon policy with database constraints and safe defaults.
- Tracked receipts derive expiration when configured and reject missing mandatory or already-expired dates; sales and transfers exclude expired balances.
- Added expired/expiring-soon tracking counters and row highlighting using each product's warning horizon.
- Added and applied `AddProductShelfLifeControls` to `AHMAD/MiniStoreDb`; domain and SQL integration coverage passed. Existing database rows keep neutral demo-safe defaults.

## 2026-10-04 — WMS-070B tracked receipts, issues and transfers

- Added lot and serial input to purchase lines and atomically creates tracked receipt balances/history with the existing purchase transaction.
- Direct sales and recipe consumption now allocate non-expired tracked stock using FEFO; exact location quantities are reduced when applicable.
- Transfer posting and cancellation preserve lot/serial identity across warehouse/location positions and write balanced trace transactions.
- Added explicit lot-quantity/serial allocation to sales and purchase returns, original-document identity validation and duplicate-return prevention.
- Added regression coverage for serial relocation/restoration and SQL coverage for receipt, FEFO issue, lot transfer and both tracked return directions. WMS-070B is complete.

## 2026-10-04 — WMS-070A opening lot/serial tracking

- Added tenant-owned tracked balances and immutable trace history with lot/serial identity, manufacture/expiration dates and rowversion.
- Added serial-one constraints, serial global uniqueness and location-aware lot uniqueness.
- Added atomic opening allocation that reconciles every current dimensional balance before activating Product.TrackingPolicy.
- Added bilingual trace/expiry UI, dedicated permissions, ADR, migration and SQL integration coverage.
- Applied `AddInventoryLotAndSerialTracking` to `AHMAD/MiniStoreDb`; operational receipt/issue/transfer/return integration remains WMS-070B.

## 2026-10-04 — WMS-060 inventory adjustments and cycle counting

- Added tenant-owned adjustment header/lines, centralized ADJ numbering and Draft/Counted/Approved/Posted/Cancelled lifecycle.
- Added blind counts, counter/approver separation, stale-snapshot and reservation guards.
- Posting atomically updates warehouse/optional location stock and emits valued StockTransaction plus linked physical StockMovement.
- Added bilingual screens, six dedicated permissions, ADR, tests and documentation.
- Applied `AddInventoryAdjustments` to `AHMAD/MiniStoreDb`; release build and SQL integration passed.

## 2026-10-04 — WMS-050 reservations and allocation

- Added tenant-owned reservation header/lines, lifecycle, source idempotency and exact dimensional allocation.
- Transfer approval now reserves availability and transfer posting consumes it atomically; legacy approved transfers use a safe compatibility path.
- Immediate sales now validate aggregate Available and cannot consume quantities held by an active reservation.
- Added bilingual read-only history and `InventoryReservations.View`.
- Applied both additive migrations to `AHMAD/MiniStoreDb`.
- Release build passed with zero warnings, 306 regression checks passed, and SQL concurrency integration proved only one claimant can reserve the final available unit.

## 2026-09-30 — WMS evolution analysis and roadmap
Completed an analysis-only review of the existing inventory/warehouse architecture and documented its implemented strengths, gaps and migration hazards. Added `WMS_EVOLUTION_PLAN.md` with a capability matrix, target separation of business documents/physical movements/balance projections/valuation, authoritative-data rules, structured proposals, concurrency/reporting/permission requirements, safe additive migration and rollback strategy, phased vertical slices and an exact file change map. The plan also preserves Product as the concrete inventory SKU while adding staged logistics metadata, typed business-specific attributes and optional template/variant grouping. Added an ADR choosing gradual evolution: preserve StockTransfer workflow, ProductStock AVCO and legacy ledgers; extend StorageLocation; introduce StockMovement and InventoryBalance only through dual-write reconciliation. No runtime code, schema or migration was added for WMS planning.

## 2026-09-29 — Posted purchase returns
Added immutable tenant purchase-return documents, separate numbering and permissions, cumulative original-quantity and current-stock controls, moving-average stock issues and atomic central journal posting. Supplier payable, input tax and purchase discount reverse proportionally; the original-versus-current inventory-cost difference posts to COGS. Added bilingual navigation/screens, regression coverage and an ADR documenting the valuation policy. Migration `20260929155442_AddPurchaseReturns` was applied to `AHMAD/MiniStoreDb`; Release build passed without warnings, 251 focused checks passed and EF reports no pending model changes.

## 2026-09-29 — Fiscal-period posting control
Added tenant-owned, non-overlapping fiscal periods with Open, Soft Closed and Closed states, required status-change reasons, actor/time metadata and rowversion concurrency. The central journal gateway now validates every posting date; companies with no periods retain legacy behavior, while configured companies may post only inside Open periods. Added an Admin-only bilingual Settings screen, tenant-safe persistence and ADR. Migration `20260929153537_AddFiscalPeriods` was applied to `AHMAD/MiniStoreDb`; Release build passed without warnings, 245 focused checks passed and EF reports no pending model changes.

## 2026-09-29 — Central journal-posting gateway
Added `JournalPostingService` as the shared Application-layer gateway for source duplicate detection, central journal-number generation, journal construction, domain balance validation, posting and persistence. Refactored purchase posting, sale posting and sales-return reversal to submit their resolved lines through the gateway inside their existing Serializable UnitOfWork transactions. This creates one insertion point for fiscal-period, approval and generalized reversal controls without changing the accounting calculations owned by each feature. No schema migration was required.

## 2026-09-29 — Immutable posted sales returns
Added tenant-isolated SalesReturn/SalesReturnItem documents, a separate centralized sequence, View/Create permissions, repository/service/controller and bilingual Index/Create/Details screens. A return requires a posted original sale, caps cumulative partial returns inside the Serializable transaction, freezes proportional revenue/discount/output-tax/refund amounts and posts the reversal journal immediately. Direct stocked items return to the original warehouse at historical sale cost with COGS reversal; prepared-to-order returns do not recreate consumed ingredients. Migration `20260929151051_AddSalesReturns` adds the protected schema and was applied to `AHMAD/MiniStoreDb`. Release build passed without warnings, 240 focused checks passed and EF reports no pending model changes.

## 2026-09-29 — Sales tax snapshots and output-tax posting
Added one optional invoice-level sales tax to wholesale and POS sales. Sale creation freezes the selected rate, output account and inclusive/exclusive policy, calculates tax after discounts and persists the tax amount. Sale posting now separates net revenue, the net-of-tax invoice discount and output tax while preserving balanced settlement for both inclusive and exclusive prices. Updated bilingual UI/resources, domain/configuration/DTO/service layers, accounting documentation and ADR; migration `20260929145026_AddSalesTaxSnapshotAndPosting` adds the tenant-safe snapshot columns and TaxRate relationship. Release build passed without warnings, 234 focused checks passed, migration SQL generation succeeded and EF reports no pending model changes. The initial migration attempt failed because the isolated automation identity could not establish Windows Integrated Security; rerunning with the host Windows identity applied it successfully to `AHMAD/MiniStoreDb`, and a repeat update confirmed the database is current.

## 2026-09-29 — Negative-stock cost-variance ledger settlement
Completed the accounting settlement for provisional recipe-negative inventory. Purchase posting now reads receipt cost movements by source reference and warehouse, then debits COGS/credits inventory when actual cost is higher or reverses the direction when it is lower. Purchase receipt valuation also excludes recoverable input tax from price-inclusive lines so the moving average agrees with the purchase journal. Added repository support, Arabic errors, balanced-journal regression coverage and documentation. Release build passed without warnings, all 231 focused checks passed, EF reports no pending model changes and no schema migration was required.

## 2026-09-28 — Sale revenue and COGS posting
Added `SalePostingService` and a localized posting action/status on sale details. A sale can post once to a source-unique balanced journal: debit payment settlement, debit configured invoice discount when applicable, credit branch/default revenue, debit COGS and credit warehouse inventory using the immutable sale-line cost snapshots. Central journal numbering and duplicate detection run inside the existing UnitOfWork. Release build passed without warnings and 230 focused checks passed. Sales tax and provisional-negative variance settlement remain follow-up accounting work.

## 2026-09-28 — Moving weighted-average inventory valuation
Implemented moving weighted-average valuation per product/warehouse. Purchases use net line cost after discount; sales freeze direct or recipe-derived UnitCost/COGS; transfers carry source cost into the destination average; stock movements freeze quantity/average/value before and after. Controlled recipe negatives use a provisional reference cost and later receipts isolate CostVariance while valuing remaining positive stock at receipt cost. Migration `20260928153229_AddMovingWeightedAverageInventoryCost` initialized current balances from product purchase prices and was applied to `MiniStoreDb` on `AHMAD`. Release build passed with zero warnings, 228 checks passed, SQL generation succeeded and EF reported no pending model changes. Ledger COGS/variance posting remains next.

## 2026-09-28 — GitHub repository documentation
Added a comprehensive root README describing MiniStore's current ERP/SaaS scope, café recipe model, managed units, architecture, local setup, security guidance, verification commands, roadmap and production-readiness limits. Added a documentation index and refreshed the project context and codebase map so GitHub readers can navigate authoritative module, database, accounting, security and decision records.

## 2026-09-28 — Arabic decimal range validation fix
Fixed the Measurement Units page crash under Arabic culture by making decimal `RangeAttribute` limits parse and convert with invariant culture. Applied the same correction to recipe and stock-transaction decimal DTOs to prevent the identical failure elsewhere. Added an Arabic-culture regression check; Release build passed with zero warnings and all 225 focused checks passed.

## 2026-09-28 — Managed units connected to products and recipes
Replaced fixed product/recipe unit selection with the tenant-managed unit catalogue. Products now reference an active stock unit; recipes validate same-dimension units and freeze authored/stock unit IDs, codes, factors and converted stock quantity so historical consumption remains stable. Added safe built-in seeding and legacy backfill plus compatibility repair for the older MeasurementUnits tenant alternate key in migration `20260928145232_ConnectManagedUnitsToProductsAndRecipes`, applied successfully to `MiniStoreDb` on `AHMAD`. Release build passed without warnings and 224 focused checks passed.

## 2026-09-28 — Managed measurement-unit engine foundation
Added a tenant-owned measurement-unit catalogue for Count, Mass and Volume with unique codes, symbols, decimal(24,12) factors to a defined base, target precision and protected system/active state. Settings now provides a bilingual management screen; built-ins cover piece, milligram, gram, kilogram, ounce, pound, milliliter and liter, while custom units can be added and safely deactivated without deletion. Added a central dimension-safe, precision-aware conversion engine, repository/service/DI, tenant isolation, tests and migration `20260928140113_AddManagedMeasurementUnits`, applied to local `MiniStoreDb` on `AHMAD`. This foundation was subsequently connected to products and recipes in the compatibility migration above.

## 2026-09-28 — Dashboard and product catalogue controls
Made `/Dashboard` the authenticated post-login landing page, redirected already-authenticated login requests and retained Home as a compatibility redirect. Added raw-material, direct-sale and prepared-to-order product classification; SQL-generated internal product codes; optional tenant-unique barcodes; active and POS/sales channel flags; prepared purchase-price clearing; purchase/stock/sale workflow enforcement; and a localized searchable, filterable, sortable and paged product catalogue. Product forms now expose business intent and return localized validation errors instead of masking them with the generic failure message. Migration `20260928133432_AddProductCatalogControls` safely backfilled existing rows and was applied to local `MiniStoreDb` on `AHMAD`. Release build passed with zero warnings/errors, 217 focused checks passed, migration SQL generation succeeded and EF reports no pending model changes. Managed units, moving-average recipe cost, per-terminal/user POS scope and central Excel/CSV import/export remain follow-up phases.

## 2026-09-28 — Versioned cafe recipes and controlled negative ingredients
Added stocked versus prepared-to-order product behavior, piece/mass/volume stock units, immutable active recipe versions and SaleItem recipe snapshots. Each recipe line freezes its authored unit plus the converted stock quantity/unit so later master-data changes cannot reinterpret history. POS/wholesale sale creation now aggregates and consumes recipe ingredients in the existing Serializable transaction; normal stock stays strict while explicitly configured ingredients may become negative through RecipeConsumption or reason-required KitchenVariance. Added bilingual recipe management, negative-balance warnings, tenant-safe composite relationships, six-decimal recipe quantities, ADR and migration `20260927221520_AddCafeRecipesAndControlledNegativeStock`. Release build passed without warnings, all 212 focused checks passed, EF reports no pending model changes and migration SQL generation succeeded. The migration was applied successfully to local `MiniStoreDb` on server `AHMAD`, and a repeated EF update confirmed the database is current. An authenticated HTTP/SQL recipe-sale journey remains pending. Weighted-average/provisional cost and variance settlement are not implemented yet.

## 2026-09-27 — Arabic accounting and software-design reference
Added `docs/ACCOUNTING_REFERENCE_AR.md` as a practical reference before implementing moving weighted-average valuation. It explains double entry and the accounting cycle, example postings, perpetual inventory and valuation, sales, purchases, tax, cash, receivables, foreign currency, fixed assets, leases, provisions, payroll, close and financial statements. It maps the relevant IFRS/IAS topics without claiming product compliance, records 2027 effective-date changes for IFRS 18 and the third edition of IFRS for SMEs, defines mandatory accounting-software invariants, and turns the remaining MiniStore accounting work into an ordered implementation plan and decision register. Documentation links were updated; no application code or database schema changed.

## 2026-09-15 — Database-enforced tenant relationship isolation
Added immutable central classification for all mapped Domain entities and an EF model convention that rebuilds every relationship between tenant-owned records with TenantId on both sides. Added the same explicit composite boundary between promotion redemptions and tenant subscriptions. Migrations `20260915121458_HardenTenantIsolationRelationships` and `20260915121859_HardenTenantSubscriptionRedemption` were applied to the local two-company database. SQL metadata reports 59 composite tenant FKs and zero unsafe tenant-to-tenant FKs. A transactional attempt to move ProductStock row 1 from tenant 1 to tenant 2 was rejected by SQL Server with error 547 and left the original row unchanged. Regression checks now reject unclassified entities and unsafe relationships; Release build passed and all 191 checks passed with no pending EF model changes.

## 2026-09-15 — Registration rules popup
Added an accessible bilingual Bootstrap popup to public company registration. It explains the actual company-name limit, unique 3–63 character company address format, supported unique username characters, unique valid email, Identity password complexity, plan selection and 14-day card-free trial. The form now also exposes matching company-address and password length hints to browser validation. Release build passed without warnings/errors; live English and Arabic requests returned HTTP 200 and contained the rules trigger, modal and RTL Arabic content.

## 2026-09-15 — Shared Arabic implementation roadmap
Added `docs/ARABIC_SHARED_ROADMAP.md` as the owner/Codex working reference. It orders the next work by dependency: document numbering reliability, tenant-owned roles, moving weighted-average valuation, complete sales posting, reversals/fiscal periods, SaaS plan versioning/provider billing, and advanced inventory/POS workflows. It defines task states, Definition of Done, completion evidence, business rules, decision points and a required rolling queue of five new suggestions after every completed task.

## 2026-09-15 — Friendly registration rate limiting
Fixed public company signup exposing a raw HTTP 429 page after five failed or repeated submissions. Registration now defaults to a configurable 20 submissions per 60-minute IP window, returns a `Retry-After` value and redirects rejected users to the bilingual signup page with a clear wait message. Tenant and platform login rejections use the same friendly flow, and the registration button prevents accidental duplicate submissions. A live local check confirmed request 21 redirects to the retry page and that page returns HTTP 200; Release build passed with zero warnings/errors and all 189 focused checks passed.

## 2026-09-15 — Platform login HTTP 400 fix
Fixed the platform login form returning HTTP 400 before credential validation by adding its missing antiforgery token. Added explicit tokens to the platform plan and promotion creation forms for the same reason. A live cookie-preserving GET/POST check confirmed the login page returns 200, emits antiforgery tokens and an invalid credential submission now reaches the action and returns 200 instead of 400.

## 2026-09-15 — SaaS onboarding, subscriptions and tenant authorization
Added rate-limited bilingual company registration that atomically creates an Identity owner, tenant, membership, tenant-scoped Admin assignment and 14-day trial. Added subscription self-service, persisted monthly/annual checkout quotes, promotion validation and one-use-per-company redemption, plus protected platform confirmation that activates the subscription in a serializable transaction. Expanded the separate bilingual RTL/LTR Platform area with company/subscription and pending-payment administration, role-based Owner/Admin/Billing policies, named company/plan choices instead of raw IDs, and an explicit initial-owner migration. Added subscription lifecycle middleware and replaced global Identity Admin checks on tenant administration screens with active-company authorization. Checkout validation now rejects unknown billing cycles/currencies and duplicate confirmation. Applied all SaaS migrations locally; Release build passed with zero warnings/errors and 167 focused security/inventory checks passed. External online payment webhooks remain provider-dependent.

## 2026-09-14 — English/Arabic localization foundation
Added ASP.NET localization for `en-US` and `ar-JO`, a rowversion-protected company default language in General Settings and an antiforgery-protected per-user culture-cookie switch. Arabic requests render `lang=ar-JO`, `dir=rtl` and Bootstrap RTL. Central Arabic resources now drive shared navigation, login/access-denied, notifications, deletion confirmation, Settings hub and General Settings. The database default provider is cached and invalidated after updates. Disabled Windows Event Log output after a restricted test process showed that missing Event Log write permission could mask an earlier failure with a .NET process-error dialog. Added and applied migration `20260914125430_AddLocalizationSettings`; Release build passed without errors and all 67 focused checks passed. A live HTTP test confirmed status 200, Arabic culture, RTL and no leaked Markdown fences.

## 2026-09-14 — POS order workflow foundation
Extended each POS profile with enabled/default Walk-in, Dine-in, Takeaway and Delivery modes plus optional dine-in service-reference requirements, guest count, per-item preparation notes and fast barcode entry. The POS adapts controls to the selected terminal; SaleService revalidates terminal policy and persists order context on Sale/SaleItem. Rebuilt Sale Details to remove visible legacy Markdown markers, show product names and display operational context. Added and applied migration `20260914123417_AddPosOrderWorkflow` with profile-aware settings backfill. Release build passed with zero warnings/errors, all 61 focused checks passed, EF reports no pending model changes and `git diff --check` found no whitespace errors.

## 2026-09-14 — Configurable POS experience
Added per-terminal POS presentation settings with Retail, Grocery, Cafe, Restaurant and Quick Service presets. Administrators can apply a preset, customize product layout, theme, cart position, brand accent/header, grid columns, card density, touch sizing, search focus and visible barcode/price/stock fields, and review the result in a live preview. The sales POS loads and reapplies the selected terminal's persisted experience without changing pricing or inventory rules. Added and applied migration `20260914120932_AddPosExperienceSettings` to the local MiniStoreDb with Retail backfill for existing terminals. The Release and Debug builds passed with zero warnings/errors, all 57 focused checks passed, EF reports no pending model changes and `git diff --check` found no whitespace errors. Specialty restaurant, cafe and grocery workflows remain separate follow-up phases.

## 2026-09-13
### Completed
Initial documentation scan and creation of the documentation system.

### Changed Files
Markdown documentation under `docs/` and root `AGENTS.md` documentation instructions only.

### Documentation Updated
Initial set created.

### Tests
No test project found. No tests executed.

### Build
Earlier repository review recorded a successful `dotnet build MiniStore.sln --no-restore` on 2026-09-12; this documentation task does not change application source.

### Problems Found
See `TODO.md`, security and accounting documentation.

### Next Steps
Address security/inventory integrity priorities before SaaS expansion.

## 2026-09-13 — Tax and discount account mapping
Added a singleton `AccountingSettings` aggregate, repository, application service and Settings screen that select purchase-discount, sales-discount, sales-revenue and cost-of-sales accounts from the chart. Tax rates already require input/output tax accounts; the Settings hub now opens both accounting posting setup and tax administration. Added and applied migration `AddAccountingPostingSettings` to the local MiniStoreDb (along with the pending purchase tax-rate migration). These links are configuration-only: the current immediate sale/purchase inventory workflows still do not create journal entries. Release build passed with zero warnings or errors and EF reports no pending model changes.

## 2026-09-13 — Purchase ledger posting
Added explicit purchase posting from the purchase details screen. It validates the supplier payable account, warehouse inventory account and any required purchase-discount account, then writes one balanced posted entry with inventory, input-tax, purchase-discount and supplier-payable lines. Journal source type/reference fields and a filtered unique index prevent the same invoice posting twice. Added and applied migration `AddJournalEntrySource` to the local MiniStoreDb; sales posting remains pending because sales have no customer or payment-account dimension.

## 2026-09-13 — Payment methods and sale settlement data
Added payment methods under Settings, each linked to a selected cash/bank/card settlement account in the chart. Sales and POS now require one payment method and accept either a registered customer or no customer for an unknown walk-in sale. Customer creation now requires a chart subaccount. Added and applied migration `AddPaymentMethodsAndSaleSettlement` to the local MiniStoreDb; sales journal posting and sale tax remain follow-up work.

## 2026-09-13 — Branch sales revenue accounts
Added a Settings → Accounting screen for assigning each branch a dedicated sales-revenue subaccount. The application validates that the selected account is a chart subaccount and persists the mapping on Branch. Added and applied migration `AddBranchSalesRevenueAccount` to the local MiniStoreDb. Future sale posting will select this branch account in preference to the company default revenue account.

## 2026-09-13 — Navigation update
Added navigation entries for customers, chart of accounts, branches, tax rates and payment methods. Accounting connection screens remain under Settings. These master-data controllers retain their existing Admin role authorization.

## 2026-09-13 — Warehouse location foundation
Added storage-location master data with warehouse-scoped unique codes, zone, aisle, rack, level, bin, type, status and optional capacity. Added a Warehouse Locations screen with warehouse filtering and combined location/product-name/barcode search. Existing warehouse-level stock is shown as Unassigned pending location allocation and putaway, avoiding duplicate quantities during transition. Added navigation and applied migration `AddWarehouseStorageLocations` to the local MiniStoreDb. Release build passed with no warnings or errors and EF reports no pending model changes.

## 2026-09-13 — Exact locations on warehouse transfers
Added source and destination StorageLocation references to every stock-transfer line. New transfers require both, filter location choices by selected source/destination warehouse and validate warehouse ownership and active status in the Application layer. Details display both location codes. Historical rows remain nullable. Added and applied migration `AddStockTransferLocations` to the local MiniStoreDb; location quantity enforcement awaits the allocation ledger.

## 2026-09-13 — Unassigned stock and per-line purchase warehouses
Made exact transfer locations optional. Added concurrent location-level balances and an Unassigned Stock work queue with warehouse/name/barcode search, sorting and partial allocation to active locations with capacity and aggregate-balance validation. Transfer posting/cancellation now moves location balances when selected and otherwise uses unassigned quantities. Purchase invoices now select a warehouse per line and may distribute one invoice across warehouses; receipt remains unassigned for later putaway. Added and applied migration `AddLocationAllocationAndPurchaseItemWarehouses` to the local MiniStoreDb with legacy purchase-item warehouse backfill. The Release build passed with zero warnings or errors and EF reports no pending model changes.

## 2026-09-13 — Reset putaway after stock depletion
Purchase creation now removes obsolete rack/bin allocations when the product's pre-receipt warehouse balance is zero. The new receipt is therefore shown in full in Unassigned Stock and must be assigned to a current physical location. The cleanup shares the Serializable purchase transaction, so the purchase, warehouse balance, movement and allocation reset commit or roll back together.

## 2026-09-13 — Feature-based source organization
Reorganized the Domain entities/contracts/commands, Application services and catalog/inventory DTOs, Infrastructure repositories/configurations, and Web controllers into consistent business-feature folders. Kept existing namespaces, MVC view locations and chronological migration locations stable so the move does not change runtime behavior or require a database migration. Reformatted the new warehouse-location entities, services, repositories, controllers, EF configurations and Razor pages into readable blocks, extracted small controller helpers and replaced repeated list scans with dictionaries/grouping. Added the feature-folder ADR and updated architecture, development and codebase documentation. The Release solution build passed with zero warnings/errors, all 41 security and inventory regression checks passed, `git diff --check` passed and EF reports no pending model changes.

## 2026-09-14 — Internal warehouse location movements
Added same-warehouse product relocation between active exact locations with source-balance and total destination-capacity validation. The operation updates both location balances without changing warehouse stock and records an immutable movement containing source, destination, quantity, type, reference, notes, user and time. Unassigned Stock putaway now writes the same audit history as a Putaway movement. Added dedicated View/Create permissions, navigation, a filtered relocation/history screen and migration `20260914070859_AddLocationMovementHistory`, which was applied successfully to the local MiniStoreDb. Release build passed with zero warnings/errors, all 45 focused security/inventory checks passed and EF reports no pending model changes.

## 2026-09-13 — Security remediation
Inspected authentication, permission evaluators, role/user management, seeders, MVC mutations, dynamic views and error handling. Added Admin-only identity mutation policy in Application and enforced it in both evaluators; fixed role-delete GET CSRF and all delete controls; enabled lockout/rate limiting; removed default bootstrap password and blocked existing-account promotion; sanitized broad exception responses while logging details. Added focused executable regression checks. No deployment/database credentials changed. Remaining credential rotation, hosting configuration, inventory concurrency and audit limitations are recorded in security/TODO docs.

Validation completed: Web build succeeded with zero warnings/errors. `dotnet run --project tests/SecurityRegression/SecurityRegression.csproj --no-restore` passed 35 checks covering both permission evaluators for Admin/non-Admin, all five delete HTTP methods and login limiter metadata. These are focused checks, not deployed HTTP or SQL integration tests.

## 2026-09-13 — Inventory integrity phase 1
Added ProductStock and StockTransfer rowversions with migration `AddInventoryConcurrency`, changed UnitOfWork inventory transactions to Serializable isolation and mapped stale writes to a retry message. The transfer token prevents concurrent duplicate posting/status changes. Restricted manual stock movements to adjustment-in/out with a required reason and transactional balance/movement saving. Purchase and sale aggregates now reject duplicate product lines. The Web Release build passed with zero warnings/errors; focused security/inventory checks passed from an isolated output directory. Migration `20260913112156_AddInventoryConcurrency` was applied successfully to the local MiniStoreDb, adding both RowVersion columns. Database-backed concurrent-write/reconciliation tests remain follow-up work.

## 2026-09-14 — Inventory operating policies
Separated warehouse operational use from inventory control with General/Central/Backroom/Sales Floor/Outlet/Production/Transit/Returns/Quarantine types and Simple/LocationManaged/Hybrid modes. Warehouses now persist POS eligibility, picking strategy, capacity enforcement and optional transfer source/destination requirements. Added a rowversion-protected Inventory Settings screen whose values initialize new warehouse forms. Simple warehouses are excluded from putaway and internal-location workflows, structured warehouses may hold one product across multiple locations, and transfer validation applies the effective warehouse policy at submission and posting. POS selection and SaleService enforce POS warehouse eligibility; the two existing warehouses were enabled to preserve current operation. Existing warehouses were migrated safely to Hybrid with capacity enforcement. Added and applied migration `20260914075257_AddInventoryOperatingPolicies` to local MiniStoreDb. Release build passed without warnings/errors, all 48 focused checks passed and EF reports no pending model changes.

## 2026-09-14 — Branch and POS warehouse access
Added branch warehouse permissions for POS, purchases, transfer directions and replenishment with priority and a branch default. Added POS terminal warehouse allow-lists with per-terminal priority and default validation. Settings now manages both layers; the POS screen selects a terminal, limits warehouses to its allow-list and chooses its default. SaleService revalidates warehouse, branch and terminal access, and POS sales retain the terminal ID for audit. Installations without terminals retain legacy POS until the first terminal is configured. Added and applied migrations `20260914083444_AddBranchAndPosWarehouseAccess` and `20260914084504_AddSalePosTerminalAudit` with backfill for existing data. Release build passed without warnings/errors and all 53 focused checks passed.

## 2026-09-13 — Accounting foundation
Added the initial domain/schema foundation for one company chart of accounts, branches and balanced multi-line journal entries. Warehouses can now be linked to a branch and inventory account. Created migration `AddAccountingFoundation`; it has not been applied to the database. The Release build passed with zero warnings/errors. Tax, POS terminal/product assortment, account administration screens and automatic moving-weighted-average posting remain the next implementation phase.

## 2026-09-14 — SaaS tenant-isolation foundation
Converted the shared ERP data model from one company to a shared-database tenant boundary. Added Tenant and TenantMembership, active-company claims, login and per-request membership validation, tenant-aware language caching, company-scoped user administration, global EF query filters, automatic TenantId stamping and cross-tenant mutation rejection for 34 business entities. Business unique indexes now include TenantId. Migration `20260914133020_AddTenantIsolation` created Demo Company, attached all existing test rows and three existing users, removed temporary TenantId defaults and was applied successfully to local MiniStoreDb. Added graceful startup database-initialization failure handling so connection errors exit cleanly instead of escaping as Windows application errors. Web build completed with zero warnings/errors, all 140 focused checks passed, the database has tenant foreign keys with zero TenantId default constraints, and the live login page returned HTTP 200.

## 2026-09-14 — SaaS control plane, public pricing and promotions
Added a bilingual public landing page and database-backed pricing page; a separately authenticated Platform area and explicit operator allow-list; plans with features and user/warehouse/branch/POS/product limits; lifecycle-aware tenant subscriptions; and actual user/warehouse limit enforcement. Added platform-managed percentage promo codes with validity windows, optional plan restriction, redemption caps, per-tenant redemption uniqueness and rowversion. Applied `20260914173013_AddSaasControlPlane`, seeding Starter/Professional/Enterprise plans and a 30-day Professional trial. Live HTTP checks returned 200 for root, pricing and platform login.
## 2026-09-15 — Central document numbering
Replaced the separate sale-invoice and stock-transfer settings with one tenant-scoped `DocumentSequence` aggregate and a bilingual Settings screen. Administrators can configure prefix, suffix, tokenized layout, 1–18 digit padding, forward-only next number, reset start and Never/Yearly/Monthly/Daily reset with live preview. Tenant/type uniqueness, rowversion and generation inside each document's Serializable transaction protect concurrency; Gregorian date tokens remain stable across UI cultures and a reset sequence cannot move back to an older period. Wholesale sales, POS sales, transfers and purchase journal entries now use the central service; supplier invoice numbers remain external references. Migration `20260915070028_UnifyDocumentNumbering` preserved old counters, created all missing defaults and was applied to MiniStoreDb. Release build passed with zero warnings/errors, all 178 focused checks passed and EF reported no pending model changes.
## 2026-09-15 — Tenant-owned roles and permissions
Separated ERP authorization from global Identity roles by adding tenant-owned role definitions, permission mappings and user assignments. Added a protected rowversion-backed Admin role, tenant-local role-name uniqueness, active-membership/same-company assignment checks and filtering of non-delegable identity mutation permissions. RolesController now uses an Application service instead of EF; bilingual role/user screens show only the active company. Onboarding and startup create independent default roles and seed Admin permissions. `20260915074008_AddTenantOwnedRoleDefinitions` copied legacy definitions, permissions and assignments, and `20260915083946_EnforceTenantRoleAssignmentBoundary` added a composite FK that rejects cross-company role assignment; both were applied locally. Release build passed without warnings and all 187 focused checks passed. EF reported no pending model changes. Database verification found zero orphan assignments and zero tenants without protected Admin; live HTTP startup/login checks passed.

## 2026-09-15 — Guided SaaS company onboarding
Implemented CompanyOnboarding state, business presets and a bilingual setup screen after registration. The versioned service applies the starter chart, linked branch/warehouse, settings, payment methods, optional tax and POS in one Serializable transaction and records Completed or Skipped once. Added company-code membership selection to login, pending-setup resumption, migration 20260915140248_AddCompanyGuidedOnboarding, ADR and regression coverage. Applied the migration locally. Release build, 197 regression checks, EF model check and HTTP/SQL end-to-end verification passed.
## 2026-09-30 — WMS demo-data classification
Recorded that all rows currently held in the development database are demo/test data rather than production records. The WMS plan may therefore choose a controlled reset/reseed when a clean inventory baseline is safer than preserving inconsistent demo history. Schema, migrations, domain rules and tenant/security boundaries remain production-grade, future migrations must remain safe for real data, and no destructive reset is an automatic migration side effect.

## 2026-09-30 — WMS standing execution authorization
Recorded the project owner's standing authorization to execute routine work within the approved WMS roadmap without requesting a separate confirmation for each code edit, build, test, development migration or dedicated temporary test database. The authorization remains scoped to inventory/warehouse delivery and does not override platform-required prompts or authorize production-data, external-publication or unclear-target operations.

## 2026-09-30 — WMS-000 inventory reconciliation baseline
Implemented a tenant-safe, read-only bilingual inventory reconciliation report with a dedicated View permission and navigation entry. SQL-side aggregates compare ProductStock quantity/value, ProductLocationStock allocation, derived Unassigned and latest StockTransaction snapshots, and flag orphan, over-allocated, negative-location, Simple-warehouse and stale/missing movement conditions. Added the movement source/idempotency and feature-state ADR. Added a disposable SQL Server fixture that seeds two tenants, verifies reconciliation translation/isolation and confirms only one concurrent last-unit issue succeeds. Release builds completed without warnings, 254 focused regression checks passed and the live SQL fixture passed. A read-only scan of MiniStoreDb found five explained demo exceptions from legacy zero snapshot fields, including two controlled-negative recipe balances; no business data or schema was changed.

## 2026-09-30 — WMS-005 product logistics foundation
Implemented tenant-owned product categories, managed Length units, nullable net/gross weight and dimensions, derived-volume semantics, handling flags and None/Lot/Serial policy while preserving Product as the concrete SKU. Product create/edit and catalogue screens now manage and filter logistics/category/tracking data; a dedicated bilingual category screen reuses Products permissions. ProductService validates unit dimensions and blocks enabling Lot/Serial on any non-zero warehouse balance. Added ADR and migration `20260929220416_AddProductLogisticsFoundation`; generated SQL is valid, EF reports no pending model changes, and the migration was applied to `MiniStoreDb`. Release builds passed without warnings, 260 focused checks passed and the disposable SQL fixture passed including the tracking-policy guard.

## 2026-09-30 — WMS-010 hierarchical storage locations
Extended StorageLocation in place without changing IDs or codes. Added display name, optional tenant/warehouse-unique barcode, parent hierarchy, stable sequence and explicit receive/pick/reserve/ship/count capabilities. Added create/edit UI, name/barcode search, capability display and Application-layer validation preventing self-parenting, descendant cycles and cross-warehouse parents. Migration `20260930010144_AddHierarchicalStorageLocations` backfills existing names from code, sequence from ID and capabilities from location type. Release build passed without warnings, 260 focused checks passed and EF reports no pending model changes. The migration was applied successfully to `AHMAD/MiniStoreDb` using the host Windows identity after the restricted sandbox identity produced an SSPI authentication failure.

Completed the WMS-010 operational pass by ordering the location list as a parent-indented tree, filtering create-form parent choices by selected warehouse and enforcing capabilities in owning workflows: putaway and transfer/relocation destinations require receive permission, while transfer/relocation sources require pick permission. The live application reached the protected login route successfully; authenticated form smoke remains intentionally pending because the repository stores no login password. Release build passed without warnings, 262 focused checks passed, EF reports no pending model changes and the diff whitespace check passed.

## 2026-09-30 — WMS-015A typed product-attribute dictionary
Added tenant-owned Text/Number/Boolean/Selection attribute definitions, ordered selection options and many-category applicability. Definitions distinguish descriptive from variant-defining metadata, validate neutral codes and are deactivated instead of deleted. Added a bilingual Products-linked administration screen and transactional creation through the Application layer. Migration `20260930022611_AddProductAttributeDefinitions` was applied to `AHMAD/MiniStoreDb`. Release and Debug builds passed without warnings, 271 focused checks passed, EF reports no pending model changes and the whitespace check passed. Product remains the inventory SKU; value assignment, ProductTemplate grouping and controlled variant generation remain the next WMS-015 slice.

## 2026-09-30 — WMS-015B typed product attribute values
Added one typed value per Product/attribute definition with Text, decimal Number, Boolean or selected Option storage. Application validation limits fields to active globally/category-applicable definitions, enforces required values and option ownership, and replaces a product's values transactionally. A database check requires exactly one typed column and tenant-composite relationships protect Product, definition and option boundaries. Added a bilingual value editor linked from the product catalogue. Migration `20260930023553_AddProductAttributeValues` was applied to `AHMAD/MiniStoreDb`. Release/Debug builds passed without warnings, 276 focused checks passed, EF reports no pending model changes and the whitespace check passed. Product remains the inventory SKU and attributes do not carry stock independently.

## 2026-10-03 — WMS-015C ProductTemplate grouping
Added optional category-owned ProductTemplate grouping while retaining Product as the concrete inventory SKU. Existing products can be assigned only when category matches and every active variant-defining attribute has a value. Canonical signatures use neutral codes and SHA-256; a tenant-safe filtered unique index prevents duplicate combinations and a readable label supports UI display. Attribute value changes refresh assigned signatures transactionally, category changes are blocked until unassignment, and removing a template never changes ProductId or stock history. Added a bilingual template/variant screen and ADR. Migration `20261003203924_AddProductTemplatesAndVariantIdentity` was applied to `AHMAD/MiniStoreDb`. Release and Debug builds passed without warnings, 281 focused checks passed, EF reports no pending model changes and the whitespace check passed. Automatic Cartesian generation remains intentionally unavailable until bounded preview is implemented.
## 2026-10-03 — WMS-015D controlled variant generation
- Added a bilingual preview-and-confirm workflow for selection-based ProductTemplate combinations.
- Enforced a server-side maximum of 50 combinations, recomputation at creation and duplicate-signature skipping.
- New SKUs clone safe commercial, unit, logistics and non-variant attribute configuration only; barcode, stock, recipes and history are never cloned.
- Kept prepared-to-order and non-selection variants manual, preserving recipe and identity safety.
## 2026-10-04 — WMS-020A physical movement kernel
- Added tenant-isolated StockMovement posted facts and idempotency enforcement.
- Putaway and Relocation now update balances, preserve LocationMovement and write the linked new fact atomically.
- Added a bilingual, permission-protected, read-only pilot ledger; no historic rows were backfilled.
- Applied migration `20261003211115_AddPhysicalStockMovementKernel` to `AHMAD/MiniStoreDb` without changing inventory quantities.
## 2026-10-04 — WMS-030 transfer and transit stages
- Approved transfers now create deterministic Planned outbound, transit and inbound StockMovement stages per line.
- Existing Post atomically transfers balances/AVCO and marks stages Posted; cancellation retains compensating behavior and marks linked stages Reversed.
- Added just-in-time compatibility for older Approved transfers and preserved cancellation of historic Posted transfers without fabricating history.
- Applied migration `20261003212429_AddTransferMovementTransitStages` to `AHMAD/MiniStoreDb`; it preserves existing kernel timestamps and changes no quantities.
## 2026-10-04 — WMS-040 explicit balances and availability
- Added transactionally synchronized InventoryBalance rows with OnHand, Reserved, Available and rowversion.
- Added a protected virtual Unassigned dimension and filtered uniqueness for null/exact locations.
- Guarded migration backfill changes no legacy quantities and stops on unexplained over-allocation.
- Applied migration `20261003214320_AddInventoryBalancesAndDefaultLocations` to `AHMAD/MiniStoreDb`; direct SQL found 8 rows, zero ProductStock mismatches and zero invalid availability rows.
- Added a bilingual permission-protected availability screen, tests, ADR and documentation.
- Extended the disposable SQL fixture to prove transactional projection synchronization before and after the concurrent last-unit test.

## 2026-10-08 — PUR-000 purchase-management architecture plan
- Reviewed the implemented direct-purchase, inventory receipt, supplier-return, accounting, permission, numbering and tenant-isolation boundaries.
- Added the approved procure-to-pay plan covering supplier purchasing data, requests, approvals, sourcing, quotations, purchase orders, partial receipts, vendor bills, three-way matching, payments, agreements, landed costs and receiving scans.
- Recorded the decision to preserve current Purchase records as Legacy Direct Purchases and add the new lifecycle without fabricated history or destructive migration.
- Classified current development-database records as demo/test data while requiring production-safe schema and migrations.
- Changed documentation only; no runtime code, schema or database data changed.

## 2026-10-08 — Cross-ERP architecture reference import
- Added the supplied eleven-document ERP architecture package under `docs/reference/erp-architecture/` without treating proposed capabilities as implemented behavior.
- Preserved its system charter, module/data ownership maps, workflows, integration contracts, cross-cutting guidance, delivery roadmap, test matrix and decisions/gaps register.
- Added an explicit precedence note: repository source, `AGENTS.md`, accepted ADRs and MiniStore's documentation workflow remain authoritative; the imported Codex file is reference guidance only.
- Updated the documentation index and codebase map. No application code, schema or database data changed.

## 2026-10-08 — PUR-005 purchase architecture alignment
- Fixed ownership before runtime work: Purchasing owns requests/sourcing/orders, Inventory owns physical stock effects, and Accounting/AP owns vendor bills, payables, payments and journals.
- Retained Supplier as the current purchasing counterparty and deferred a generic Party migration until repository-wide evidence justifies it.
- Defined distinct ordered/received/returned/billed/paid authorities, purchase-first approval history, a staged currency boundary and future event facts without prematurely adding an event bus.
- Accepted GRNI as the target bridge between Goods Receipt and Vendor Bill while leaving Legacy Direct Purchase accounting unchanged.
- Added two ADRs and entry gates for later purchase slices. Documentation only; no runtime code, migration or database data changed.

## 2026-10-08 — PUR-010 supplier purchasing data and lead time
- Added tenant-owned, rowversion-protected supplier/product purchasing information with compatible purchase unit, supplier code/description, minimum/order-multiple quantities, lead time, base-currency price, validity, priority, preferred and active state.
- Added Application validation that rejects prepared-to-order/inactive products, inactive or dimension-incompatible units, foreign currency and duplicate supplier/product/unit identities; no inventory or accounting state is changed.
- Added repository/configuration/DI, separate view/manage permissions, Purchasing navigation and a bilingual RTL-safe administration screen.
- Added domain, EF metadata and controller-permission regression coverage; 212 focused checks passed and the solution built with zero warnings.
- Created migration `AddSupplierPurchasingData`; EF reports no pending model changes. Sandbox Windows authentication could not reach SQL, so the authorized host identity applied `20261008161855_AddSupplierPurchasingData` successfully to `AHMAD/MiniStoreDb`.

## 2026-10-08 — PUR-020A purchase-request foundation
- Added tenant-owned PurchaseRequest, PurchaseRequestLine and PurchaseRequestHistory with Draft -> Submitted and reasoned Draft/Submitted -> Cancelled transitions.
- Added frozen requested-unit conversion and stock quantity, optional suggested supplier, rowversion and explicit Created/Submitted/Cancelled actor/time history.
- Extended centralized numbering with PurchaseRequest and the `PRQ-` default without creating stock or accounting effects.
- Added domain/metadata regression checks; the focused executable passed 404 checks and the solution built with zero warnings.
- Created and applied migration `20261008180612_AddPurchaseRequestFoundation` to `AHMAD/MiniStoreDb`. PUR-020 remains in progress until PUR-020B adds the Application service, permissions and bilingual UI.

## 2026-10-08 — PUR-020B purchase-request application and UI
- Added repository and Application orchestration for Draft creation, managed-unit conversion, Submit and reasoned Cancel with rowversion checks.
- Added distinct View/Create/Submit/Cancel permissions, navigation, list/create/details screens and Arabic translations.
- Product, warehouse, unit and optional suggested-supplier identities are validated by name-backed selectors; prepared-to-order products are excluded.
- Purchase requests remain internal demand only and create no inventory or accounting effect.

## 2026-10-11 — Arabic/English master implementation roadmap
- Added `MASTER_IMPLEMENTATION_ROADMAP_AR_EN.md` with an approved 18-step delivery sequence from current-state stabilization through production launch.
- Defined an intake workflow for future customer-management, pricing, sales, receivables and return prompts so new requirements are assigned to their dependency-correct phase before implementation.
- Updated the documentation index, codebase map, concise TODO and shared Arabic roadmap to reference the master sequence and replace the stale immediate-priority text.
- Documentation only; no runtime code, schema, configuration or database data changed.

## 2026-10-11 — Purchase-sourcing submission binding fix
- Bound the Purchase Sourcing POST action to the create page's nested `Form.*` field prefix, preserving the Purchase Request ID, selected suppliers, invitation message and notes instead of falling through to an HTTP 404 with request ID zero.
- Hid Start Sourcing on approved Purchase Requests when the current user lacks `PurchaseSourcing.Create`; server-side authorization remains authoritative.
- Added focused regression coverage for the binding contract and updated the purchase module, controller and screen documentation.
- Release build passed with zero warnings/errors; the final combined binding-audit verification is recorded below.

## 2026-10-11 — Nested purchase-form binding audit
- Fixed Supplier Quotation submission to bind the page's nested `Form.*` values, preventing the sourcing-event ID from becoming zero and falling through to an HTTP 404 after Submit Quotation.
- Applied the same binding contract to manual Goods Receipt submission after auditing the remaining purchase forms; other wrapper pages already override their field names to direct command properties.
- Made Enter Supplier Quotation and Quotation Comparison links respect their separate permissions and added regression checks for both corrected binding contracts.
- Release build passed with zero warnings/errors and a freshly rebuilt test executable passed all 548 focused security and inventory regression checks.

## 2026-10-11 — Actionable purchase-error guidance
- Added a shared resolver that maps recognized purchase prerequisites to accounting, warehouse, approval-rule, payment-method, matching, tax, supplier, product, measurement-unit or document-number screens.
- Extended the shared notification experience to show a localized resolution button for both modal notifications and page validation errors. Buttons appear only when the user has the target management permission; otherwise the user is directed to an administrator.
- Kept ordinary validation errors message-only, documented actionable guidance as the default rule for future features and added Arabic labels for every new action.
- Release build passed with zero warnings/errors and all 555 focused security and inventory regression checks passed.

## 2026-10-11 — Clear purchase number formatting
- Added one shared formatter for purchase quantities, money, percentages and numeric form values. It respects the existing General Settings maximum decimal places while suppressing insignificant trailing zeros, so `5.0000` displays as `5` and real fractions remain visible.
- Applied the formatter across requests, sourcing, quotations, orders, agreements, receipts and receipt returns, vendor bills, supplier payments, landed costs, purchase returns, direct purchases, supplier purchasing data and purchase analytics. Updated the direct-purchase browser calculations to follow the same rule.
- Clarified the bilingual General Settings labels and help text so administrators understand that the configured values are maximum precision, not forced zeros. No schema or stored numeric precision changed.
- The Web project built with Razor compilation and zero warnings/errors; all 558 focused security and inventory regression checks passed.

## 2026-10-11 — Permission-aware workflow next steps
- Extended the shared success notification with a localized "What is the next step?" card, short explanation and direct action button. The button is shown only when the current user holds the destination permission; the destination controller still enforces access.
- Added explicit purchase guidance from request creation through approval, sourcing, quotations, orders, receiving, vendor billing/payment and agreements, plus warehouse-location, unassigned-stock, stock-balance and replenishment guidance.
- Added a scoped completion filter so successful purchase, inventory and warehouse POST redirects without an existing notification still receive safe generic guidance. Added missing success messages to purchase request/order/agreement/template and approval-rule transitions.
- Documented next-step guidance as a required principle for future workflow work and added complete Arabic localization.
- The Web project built with Razor compilation and zero warnings/errors; all 563 focused security and inventory regression checks passed.

## 2026-10-11 — Completed workflow actions replace duplicate buttons
- Made one-time purchase and inventory actions state-aware: submitted, sourced, approved, confirmed, fully received, counted, activated and posted steps now show a localized completion label after success instead of leaving the old action available.
- Quotation comparison now loads the recorded award and any resulting Purchase Order or Purchase Agreement. Award/create buttons disappear after completion and the created document is linked for viewing.
- Vendor Bill details now calculate posted supplier-payment allocations, show paid/outstanding amounts and replace Pay with Paid in full when the balance reaches zero. Fixed direct Purchase details so a posted purchase never shows Post again, including users without return permission.
- Fully released agreements now show a finished state instead of another release button. Repeatable partial receipt, release and payment actions remain available while a real quantity or balance remains.
- Documented this as the required UI rule for future workflows. The Web project built with Razor compilation and zero warnings/errors; the focused regression executable passed 565 checks.

## 2026-10-12 — Supplier-first bulk purchasing data
- Reworked Supplier Purchasing Data around selecting one supplier first, then editing several eligible product rows in one searchable table. Users can select visible rows, clear selection, or change a field to select its row automatically.
- Added a nested bulk command with explicit model binding, compatible purchase-unit choices per product, existing rowversion protection and server-side supplier/product identity checks.
- Bulk saves run in one Serializable UnitOfWork transaction: all selected rows succeed together or the full operation rolls back. Existing single-row and active/inactive commands remain supported.
- Added full Arabic localization, updated purchase/screen/controller/codebase documentation and extended permission/binding regression coverage.
- The Web project built with Razor compilation and zero warnings/errors; all 566 focused security and inventory regression checks passed.
- Replaced the wide bulk table and saved-row table with responsive product cards and a wrapping summary list. Field labels now follow a consistent business grouping and the screen no longer requires horizontal scrolling.
- Reworded price validity, selection order, order multiple and supply lead-time fields in plain business language and added short examples/help under each field in both supported languages.
- Moved those inline explanations into accessible question-mark tooltips that open on hover or keyboard focus and disappear on leave/blur, reducing vertical clutter in every product card.

## 2026-10-12 — Professional supplier profiles and synchronized product assignment
- Expanded Supplier master data with legal, tax, registration, contact, website, payment-term, notes, active-state and rowversion fields, presented in responsive bilingual profile sections and summary cards.
- Added searchable multi-product selection on Supplier create/edit and searchable multi-supplier selection on Product create/edit. Both screens write the same `SupplierProductPurchasingInfo` relationship in one Serializable transaction, so assignments synchronize automatically in both directions.
- Preserved commercial history by deactivating removed relationships instead of deleting them. Supplier Purchasing Data now shows only active products assigned to the selected supplier and directs empty suppliers back to product assignment.
- Added and applied the schema migration to local `MiniStoreDb`, plus Arabic localization, supplier module documentation and regression checks for concurrency and bidirectional form contracts.
- Supplier save completion now guides authorized users directly to the same supplier's purchasing terms to finish prices, quantities and lead times.
- The Web project built with zero warnings/errors; the focused regression executable passed 569 checks.

## 2026-10-12 — Permission-aware Dashboard application launcher
- Replaced the empty authenticated Dashboard with an Odoo-inspired launcher for Point of Sale, Sales, Purchasing, Inventory, Products, Accounting and Administration.
- Added responsive application cards, distinct visual themes, plain-language descriptions, common-screen shortcuts and instant localized application search in English and Arabic.
- Filtered every application and shortcut through the existing permission service; each card opens its first authorized destination while controller policies remain authoritative.
- Added centralized Dashboard application definitions, module/screen/controller/codebase documentation and regression checks ensuring distinct apps and registered shortcut permissions.
- The Web project built with zero warnings/errors; all 571 focused security and inventory regression checks passed.

## 2026-10-12 — Post-PUR-150 purchase completion roadmap
- Studied the implemented purchase lifecycle and organized the remaining accounting, commercial, automation, logistics, supplier-collaboration and integration gaps into fourteen dependency-ordered slices from PUR-160 through PUR-280.
- Added a bilingual Arabic/English roadmap with scope, dependencies and exit gates for reconciliation, order closure, Supplier Credit Notes, controlled reversals, multi-currency, flexible RFQs, split awards, service procurement, bulk vendor prices, replenishment, advanced receiving, supplier communication, integrations, analytics and Legacy Direct Purchase retirement.
- Set PUR-160 inventory-to-GL and GRNI reconciliation as the single starting slice and linked the plan from the documentation index, codebase map, master roadmap, purchase evolution plan and concise TODO.
- Documentation only; no runtime code, schema, configuration or database data changed.

## 2026-10-12 — PUR-160 purchase accounting reconciliation
- Added a dedicated read-only repository and Application service that compare as-of-date warehouse operational Inventory values with each assigned GL account and receipt-derived GRNI with the configured ledger account.
- Added responsive bilingual evidence cards for warehouse differences and Goods Receipt value less unbilled returns and Vendor Bill clearing, including direct Purchase Order, receipt, bill, return, stock and permission-aware setup links.
- Added the `PurchaseAccountingReconciliation.View` permission, Purchasing navigation and Dashboard shortcut; the report performs no posting or adjustment and explains historical timing differences caused by movement-recording time versus document date.
- Extended focused regression coverage and the disposable two-tenant SQL fixture. Release builds passed with zero warnings; 566 focused checks passed, and SQL verified exact Inventory/GL and GRNI/GL agreement after receipt, partial billing, unbilled return and landed-cost capitalization.
- No schema migration was required; no production or development business data was changed.
## 2026-10-15 — SAL-000 Sales architecture audit and implementation plan

- Audited the implemented Sales, Customer, Catalog, Inventory, Invoicing-like Sale, Accounting, Payment Method, permission, numbering, currency, audit and test foundations against the supplied ERP Sales architecture requirements.
- Classified the current `Sale` aggregate as a compatible immediate sale/invoice/payment/stock workflow rather than a Sales Order, and prohibited destructive reinterpretation of its history.
- Added `SALES_ARCHITECTURE_AUDIT_AND_IMPLEMENTATION_PLAN.md` with current strengths, gaps, preserved components, bounded contexts, aggregate and transaction boundaries, internal facts, permissions, migration implications, risks, test requirements, frontend impact, expected file map and `SAL-005` through `SAL-130` vertical slices.
- Updated the project context, architecture, Sales module, documentation index, codebase map, master roadmap and TODO. No runtime code, schema, permission or UI behavior changed.

## 2026-10-15 — SAL-005 Sales architecture alignment gate

- Accepted explicit ownership boundaries between Sales, Inventory, Invoicing, Accounting, Payments, Pricing, Customers and shared Foundation services.
- Preserved the current `Sale` and POS as compatible immediate-sale flows while prohibiting destructive reinterpretation of their history.
- Fixed the safe default of full reservation with no negative Sales stock, stock/COGS effects only at posted Delivery, delivered-quantity invoicing for stocked goods, ordered-quantity eligibility for configured services and separate receivable/payment authority.
- Fixed dated transaction/company-currency snapshots for new commercial documents and retained company-currency Inventory/COGS authority.
- Added the alignment document and accepted ADR, marked `SAL-005` complete and advanced the next runtime step to `SAL-010` Customer foundation.
- Documentation only; no runtime code, schema, permission, UI or database data changed.

## 2026-10-15 — CUS-000 Customer and Business Partner audit

- Audited the current minimal Customer entity, service, repository, screens, permissions, tenant boundaries, accounting linkage, Sale references, migrations, audit trail and focused test infrastructure against the supplied enterprise Customer and Business Partner requirements.
- Classified the professional customer profile, contacts, addresses, searching, customer-specific permissions, receivables, credit, collections, CRM, helpdesk, portal, duplicate merge, Customer 360 and reports by implementation status and dependency.
- Preserved current Customer and Supplier identifiers and documented the proposed BusinessPartner unification as an independent decision gate with an additive expand-and-contract migration rather than an immediate replacement.
- Added `CUSTOMER_AND_BUSINESS_PARTNER_AUDIT_AND_IMPLEMENTATION_PLAN.md` with compatibility matrix, conflicts, database risks, target entities, ownership map, permissions, tests, acceptance criteria and `CUS-005` through `CUS-110` phases.
- Documentation only; no runtime code, schema, permission, UI, configuration or database data changed. Implementation remains paused pending explicit approval.

## 2026-10-15 — FND-000 ERP SaaS Foundation Blueprint integration

- Studied the supplied ERP SaaS Foundation package and separated its embedded agent prompt/rules from the user's request and the repository's authoritative instructions.
- Installed the architecture, tenant security, audit, finance/inventory integrity, disaster recovery, approvals, configuration, provisioning/billing, exception, work/guidance, import/export, delivery, test and operations references under `docs/reference/erp-saas-foundation/`.
- Added source provenance with SHA-256 hashes and preserved the source instruction files under non-authoritative archive names so they cannot be mistaken for active repository instructions.
- Added an evidence-based MiniStore compatibility matrix: tenant isolation is implemented with targeted review gaps; audit, financial/inventory integrity, approvals, configuration, provisioning and guidance are partial; exception center and general import/export are missing; backup/restore and production capacity remain unknown until evidenced.
- Defined `FND-010` through `FND-080`, with operational backup/restore evidence as the first proposed gate and explicit ADR triggers preventing premature microservices, broker, cache, RLS or stamp adoption.
- Documentation only; no runtime code, schema, permission, UI, configuration, dependency, deployment or database data changed.

## 2026-10-15 — CTRL-000 ERP project recovery and execution control

- Recursively inventoried the four-layer solution, source groups, 115 Domain entities, 87 Application services, 66 repositories, 64 controllers, 163 Razor views, 112 migrations, tests, documentation, duplicate export tree and recent Git history.
- Executed the Release security/inventory regression program successfully with 748 checks and the disposable SQL integration program successfully across tenant isolation, inventory/accounting reconciliation and concurrent purchase workflows.
- Added `docs/project-control/` with current status, a module-by-module feature inventory, architecture/database/security/business-integrity audits, documentation conflict register, dependency map, one P0–P4 master roadmap, decision register, technical debt, test evidence, quick context and exactly one proposed next task.
- Identified historically published administrator credentials as the only recommended next task because source-code removal cannot revoke access on an existing deployment; no credential was read or recorded during this audit.
- Marked backup/restore, production migrations, CI/CD, observability, performance and penetration testing as unknown or missing evidence rather than claiming production readiness.
- Documentation only; no application code, schema, migration, permission, runtime configuration, secret or database business data changed. `P0-SEC-001` remains unimplemented pending explicit approval.

## 2026-10-15 — P0-SEC-001 Demo credential closure

- Recorded the owner's classification that all current users and data belong to the local Demo environment; no staging, production or external deployment artifact was found.
- Verified without printing the value that the historically published administrator credential still authenticated before remediation, confirming a real local exposure.
- Disabled password access for the affected `admin` account, permanently locked it, and rotated security/concurrency stamps for all nine pre-existing Demo users to invalidate prior sessions.
- Created `demo-admin-rotated` through the existing one-time bootstrap using a generated strong password kept only in the local `User Secrets` development store; committed bootstrap configuration remains disabled.
- Re-tested the historical value and confirmed rejection, then positively authenticated the replacement administrator and observed an authorized redirect.
- Removed the separate stale `BootstrapAdmin:Password` key identified by the owner from its local User Secrets store without recording its value or changing adjacent JWT keys.
- Added secret-free operational evidence, closed `P0-SEC-001` for the declared scope and advanced the sole proposed task to `P0-OPS-002` backup/restore verification. No application code, schema, migration or committed runtime configuration changed.

## 2026-10-15 — P0-OPS-002 local Demo backup and restore drill

- Adopted Demo recovery objectives of 24-hour RPO and 4-hour RTO; production values remain an owner/hosting decision.
- Created a SQL Server master key and dedicated backup certificate, exporting the public certificate and password-encrypted private key while keeping their generated protection secrets in local User Secrets outside the repository.
- Created a compressed, checksum-protected, `COPY_ONLY` backup of `MiniStoreDb` encrypted with AES-256; `RESTORE VERIFYONLY` passed.
- Restored the backup under a unique isolated database name, made it read-only and completed `DBCC CHECKDB` successfully.
- Compared all 147 table row counts and 21 company/user/inventory/ledger/purchase/outbox totals with zero differences; source and restored journal lines were balanced.
- Safely removed only the restore-drill database after strict name validation. The original database, encrypted backup, certificate and encrypted private key remain.
- Recorded that same-host backup is proof of recoverability, not protection from host/disk loss. Off-host retention, automation, failure alerts and cross-host restore remain production work.
- Added secret-free evidence, closed `P0-OPS-002` for the local Demo drill and advanced the sole proposed task to read-only `P0-DB-003` migration/schema reconciliation. No application code, schema migration or committed runtime configuration changed.

## 2026-10-15 — P0-DB-003 migration and schema reconciliation

- Compared 112 migration source files, the EF Core migration list and 112 `__EFMigrationsHistory` rows: first/last IDs and every intermediate ID matched with no missing, extra or duplicate migration.
- Confirmed the runtime model has no changes since the last migration and generated the complete 563,829-character idempotent migration script in memory without storing or executing it.
- Inspected the live Demo schema: 147 user tables, 420 trusted enabled foreign keys, no disabled/untrusted check constraints, no disabled indexes, no table without a primary key and no unsafe tenant-to-tenant relationship.
- Completed `DBCC CHECKCONSTRAINTS` without violations. The two recorded EF versions are a normal patch transition: 7 migrations on 10.0.11 and 105 on 10.0.12.
- Corrected three stale database-document claims and closed `DOC-002`/`DB-002`; no forward-fix is required.
- The normal Release output was locked by the user's running MiniStore process, so it was left running and the solution was built to an isolated artifacts path instead; all four projects passed with zero warnings/errors.
- Added reconciliation evidence, closed `P0-DB-003` for the local Demo database and advanced the sole proposed task to the design-only `P1-AUD-010`. No migration, schema or business data changed.

## 2026-10-15 — P1-AUD-010 audit-security design

- Inventoried the generic `AuditLog`/SaveChanges interceptor, its lack of a read surface and the authoritative accounting, inventory, purchase-lifecycle, supplier-integration and document-status evidence that a general audit stream must not duplicate.
- Recorded the previously observed local Demo baseline of 707 generic audit rows across four tenants, including 32 System rows and no Unknown identifiers, without exposing personal or secret values.
- Approved an additive semantic `AuditEvent` boundary with Tenant/Platform/System scope, outcome, reason, correlation, bounded redacted changes, idempotency and staged integrity evidence; legacy rows remain unchanged and are not backfilled or reinterpreted.
- Defined allowlist redaction, separate successful-transaction versus rejected/failed-security recording paths, permission-separated reading/export, proposed retention classes subject to legal/operational approval and external checkpoints needed before claiming tamper resistance.
- Added the architecture/implementation plan and ADR with migration, feature-flag rollback, test gates and `P1-AUD-011` through `P1-AUD-016` slices. Advanced the sole proposed task to `P1-AUD-011`.
- Documentation only; no runtime code, schema, migration, permission, UI, configuration or Demo data changed.

## 2026-10-16 — P1-AUD-011 audit-event foundation

- Added the mixed-scope `AuditEvent` domain model and additive table for Tenant, Platform and System events without changing or backfilling the 707 legacy `AuditLogs` rows.
- Added an Application-owned writer/redactor/integrity contract and Infrastructure implementation. Writing is disabled by default; enabled use requires a versioned external Base64 key of at least 256 bits.
- Enforced active-tenant writes, explicit platform/system scope, fail-closed normal reads, append-only DbContext behavior, reason-code-first allowlists, bounded scalar JSON, secret-like field rejection, operation idempotency and versioned HMAC-SHA256 verification.
- Added migration `20261015173055_AddAuditEventFoundation` with three trusted checks and six enabled indexes, applied it to local Demo as migration 113 and confirmed 0 new events, 707 legacy rows and no pending model change.
- Extended focused checks to 756 and SQL integration to prove disabled no-op, language-neutral codes, successful atomic commit, transaction rollback, two-tenant isolation, explicit platform bypass, append-only rejection, duplicate-operation rejection, integrity verification after database round-trip and prior-key verification after rotation.
- No audit reader/export UI or permissions were added, no semantic business service was enrolled and no secret was committed. Advanced the only proposed task to `P1-AUD-012`.
- Recorded secret-free execution evidence in `project-control/operations/P1-AUD-011-AUDIT-FOUNDATION.md`.

## 2026-10-16 — P1-AUD-012 first semantic audit coverage

- Added fixed Application event codes and a semantic event service backed by an Application-owned request-context contract; Web supplies the trusted correlation identifier and route template without introducing an HTTP dependency into Application.
- Enrolled tenant-role creation, ordinary update, permission-set change and deletion. Refactored role persistence into the shared `UnitOfWork` so the role, permission links and event commit or roll back together.
- Enrolled Accounting posting accounts, non-personal General operating values, Discount controls, Inventory defaults, Purchase Matching tolerances and Purchase Sourcing policy. Each event shares the existing repository save with its setting change.
- Added deny-by-default field allowlists. General settings omit company names, phone and address; no password, token, integrity key, attachment or raw request is stored.
- Generated a random 32-byte Demo integrity key in local User Secrets, selected version `v1` and enabled Demo locally. Committed configuration remains disabled with no key value.
- Release Web and test-project builds passed with zero warnings/errors; SecurityRegression remained 756/756; SQL integration passed semantic event count/order, tenant/actor/correlation, redaction, database-round-trip integrity and rejected-write rollback together with the existing suite.
- No schema change or migration was needed; EF reported no pending model changes. Demo inspection after activation showed 113 migrations, 707 legacy rows and 0 semantic rows before the next enrolled user operation. No audit reader/export UI or permission was added, and legacy `AuditLogs` remains unchanged.
- Recorded secret-free evidence in `project-control/operations/P1-AUD-012-SEMANTIC-AUDIT-COVERAGE.md` and advanced the only proposed task to `P1-AUD-013`.

## 2026-10-16 — P1-AUD-013 authentication, denial, platform and integration audit coverage

- Added fixed security/administration/integration event codes for rejected company and Platform login, lockout, actual endpoint authorization denial, Platform subscription/payment/plan/promotion administration and supplier API-credential/portal-access lifecycle.
- Added a bounded independent security writer that creates a fresh persistence scope for denied/failed events, enforces explicit Tenant versus Platform/System scope, respects the feature flag, logs persistence failure as Critical and rethrows it.
- Extended the HTTP audit context to classify Web/Platform/API/Portal channels, mask IPv4 to `/24` and IPv6 to `/64`, and store only a SHA-256 user-agent fingerprint.
- Kept successful Platform and supplier-access administration events in the same save/transaction as their business operation. No username input, password, cookie, API key, portal token, callback secret/URL or raw request body is copied into `AuditEvent`.
- Web and SQL-test builds passed with zero warnings/errors. SecurityRegression passed 761 checks. SQL integration passed with two exactly counted independent denied events and strict System/Tenant scope after rerunning under the normal local Windows identity because the sandbox identity could not establish SSPI.
- No schema change or migration was needed; legacy `AuditLogs` remains unchanged. Recorded evidence in `project-control/operations/P1-AUD-013-SECURITY-AUDIT-COVERAGE.md` and advanced the only proposed task to `P1-AUD-014`.

## 2026-10-16 — P1-AUD-014 permission-aware audit reader and export

- Added an Application query/export service and an Infrastructure repository that enforce either the active tenant or the separate Platform/System boundary; controllers do not query EF directly and entities are projected to DTOs.
- Added tenant and Platform readers with fixed date/page limits, deterministic ordering, category/outcome/resource/correlation filters, responsive Arabic/English cards and no horizontal-scroll table.
- Added separate tenant View, Export and ViewSensitive permissions plus separate Platform policies. Export requires view and export authority, antiforgery and one of three fixed review purposes; sensitive masked request evidence is projected only when explicitly authorized.
- Added a bounded 5,000-row CSV export with spreadsheet-formula protection. The snapshot is produced before its own event, then exactly one `DataAccess.AuditEventsExported` event records purpose, scope, row count and SHA-256 file fingerprint without copying file content.
- Web and SQL-test builds passed with zero warnings/errors. SecurityRegression passed 765 checks. SQL integration passed tenant/platform isolation, sensitive suppression/projection and exactly one export event together with the existing suite.
- No schema change or migration was needed; migration count remains 113 and legacy `AuditLogs` remains unchanged. Recorded evidence in `project-control/operations/P1-AUD-014-AUDIT-READER-EXPORT.md` and advanced the only proposed task to `P1-AUD-015`, which requires owner/legal/hosting decisions before approval.

## 2026-10-16 — P1-AUD-015 retention, legal hold and external checkpoint

- Recorded owner approval for seven-year protected evidence, three-year ordinary operations, ninety-day detailed failed-login evidence, Owner-only legal holds, daily Demo file checkpoints and separate database roles.
- Added mixed-scope `AuditLegalHold`, Owner-only bilingual Platform controls and semantic events for hold creation/release, checkpoints and retention execution.
- Added a daily/manual fail-closed retention pipeline. It writes signed external checkpoint and minimal destruction-manifest files before deletion, then rechecks every ID/integrity fingerprint and legal hold inside the maintenance transaction.
- Added separate read/maintenance connection hooks and migration-created SQL roles: insert-only writer, select-only reader and controlled maintainer. Production identity assignment remains environment-owned and secret.
- Applied migration `20261016191833_AddAuditRetentionControls` to local Demo as migration 114. User Secrets holds the enabled flag, archive directory and independent 256-bit checkpoint key; committed configuration is disabled and secret-free.
- Web and SQL-test builds passed with zero warnings/errors; SecurityRegression passed 768 checks; SQL integration passed migration, role impersonation, legal hold, checkpoint, manifest-before-delete and evidence-minimization cases. EF reported no pending model changes.
- Started Demo once to produce the first signed checkpoint. Demo then reported 707 unchanged legacy rows, one new maintenance event, 114 migrations and zero eligible deletions. Recorded evidence in `project-control/operations/P1-AUD-015-RETENTION-CHECKPOINT.md` and advanced the proposed task to `P1-AUD-016`.

## 2026-10-17 — Local Demo Platform Owner activation

- Received explicit owner authorization to promote the rotated local Demo administrator to Platform Owner.
- Used the existing opt-in `PlatformOperatorSeeder`; startup inserted one active `PlatformOperatorRole.Owner` record for the existing Identity account.
- Reset `PlatformOwner:Enabled` to false and removed the temporary `PlatformOwner:Username` project secret immediately after activation.
- Verified a real antiforgery-protected platform login returned HTTP 200 at `/platform` with no invalid-credential response.
- No application code, schema, migration or committed secret changed. The next-task gate remains `P1-AUD-016` awaiting separate approval.

## 2026-10-18 — P1-AUD-016 coverage reconciliation and legacy-audit freeze

- Added a fixed Application coverage catalog for 37 critical controls: 28 semantic event types and nine named authoritative domain-history sources. Reflection checks require exact parity with every declared semantic event constant.
- Added independent `LegacyAudit:WriteEnabled`; code defaults to compatibility-on when absent, while committed cutover configuration is false. `AuditLogs` remains mapped and readable.
- Extended the repeatable read-only `--inspect-audit` report with catalog counts, expected scope/outcome, observed semantic counts, authoritative references, gaps and unexpected types.
- SQL integration proved disabled generic writes with continuing semantic events, then enabled rollback adding exactly one future legacy row without changing history.
- Created signed Demo checkpoints before and after cutover. Legacy rows remained 710; semantic rows advanced 9→11; catalog reconciliation reported 37 expected, zero gaps and zero unexpected types.
- Web build passed with zero warnings/errors, SecurityRegression passed 771 checks, SQL integration passed and EF reported no pending model changes. No migration or historical backfill was introduced.
- Closed `P1-AUD-016`; advanced the sole proposed task to `P1-OBS-020`, awaiting explicit approval and environment decisions.

## 2026-10-18 — P1-OBS-020 local observability and performance gate

- Recorded the owner's decision not to use Git as an execution gate; no hosted workflow was added. GitHub may be used through its connector only if a future remote operation actually requires it.
- Added secret-free `/health/live` and SQL-aware `/health/ready`, plus vendor-neutral request count/failure/duration/active-request and database-probe metrics.
- Added structured HTTP 500, slow-request and database-readiness logs. Regression proves that an HTTP 500 emits one failure measurement and an alert log.
- Added dependency-free `PerformanceSmoke` and the one-command `scripts/verify-local.ps1` gate covering Release build, focused checks, disposable SQL integration, readiness and bounded p95/p99 workload evidence.
- Full local Windows run passed: zero build warnings/errors, 777 focused checks, complete SQL integration, and 500/500 requests with zero failures, p95 16.85 ms and p99 28.35 ms against limits 250/500 ms.
- No schema, migration, business state or secret changed. Added a runbook and ADR; closed `P1-OBS-020` for local Demo and advanced the sole proposed task to `P1-ARC-030`, awaiting explicit approval and slice selection.

## 2026-10-18 — P1-ARC-030 Warehouse Locations application boundary

- Selected the smallest useful legacy boundary: WarehouseLocations used `IWarehouseRepository` directly and passed Warehouse/StorageLocation domain entities to Razor.
- Added Application DTO projections for location rows, warehouse options and parent options. `StorageLocationService` now owns these page reads, and the controller depends only on that service plus localization.
- Preserved all create/update rules and the existing repository save boundary; no entity, mapping, transaction, schema, migration or user-facing text changed.
- Release build passed with zero warnings/errors. SecurityRegression passed 780 checks, including three new architecture guards. Runtime readiness and a 500-request smoke passed with zero failures; SQL was intentionally not repeated for this projection-only slice after the immediately preceding full green SQL gate.
- Recorded closure evidence in `project-control/operations/P1-ARC-030-WAREHOUSE-LOCATIONS.md` and advanced the sole proposed task to the documentation-only `CUS-005` customer identity decision gate.

## 2026-10-18 — CUS-005 customer and business-partner identity decision

- Approved the documentation-only identity boundary after reconciling the current Customer, Supplier, Sales and Accounting references with the existing SAL-005 decision.
- Preserved CustomerId and SupplierId as separate compatibility identities. CUS-010 will expand Customer additively and will not create a BusinessPartner table or rewrite historical keys.
- Reserved an optional future dual-role link for proven need only; actual unification remains a separately approved CUS-110 expand-and-contract migration with reconciliation and rollback evidence.
- Rejected immediate table replacement, a third competing source of truth, mutable natural-key identity and an editable customer balance.
- No source code, schema, migration, configuration, permission, user interface, business data or secret changed. Recorded the decision in `ADR-20261018-CUSTOMER-PARTNER-IDENTITY.md` and advanced the sole proposed task to CUS-010.
- 2026-10-18: Completed `CUS-010` professional Customer master. Preserved CustomerId/Sale history, added centrally numbered tenant-local codes, profile/archive/rowversion, DTO-only paged UI, dedicated permissions, Arabic localization and next-step guidance. Applied migration 115 to local Demo; one existing customer was preserved and assigned a valid code, and all five tenant sequences reconciled. Release build had zero warnings/errors, SecurityRegression passed 787 checks, SQL integration passed customer isolation/uniqueness/archive/concurrency plus the full existing suite, and EF reported no pending model change. Advanced the approval gate to `CUS-020` contacts and addresses.
- 2026-10-18: Completed `CUS-020` contacts and addresses. Added tenant-safe multi-contact and General/Billing/Shipping address entities, one active primary/default rules, archive/restore, rowversion, DTO snapshots, responsive bilingual UI and permission/antiforgery controls. Applied migration 116 to Demo without inventing data: one Customer remained, contacts/addresses both started at zero. Release build passed with zero warnings/errors, 798 regression checks and the full disposable SQL suite passed, and EF had no pending model change. Advanced the gate to accounting-decision-first `CUS-030`.
- 2026-10-19: Approved the platform-wide configurable-policy/safe-history principle and completed `CUS-030A`. Added three prospective customer receivable modes, bilingual help, control/parent account validation, automatic individual account creation, shared-account reuse and a dependent-sales transition guard. Applied migration 117 to Demo without selecting a policy or changing the existing customer/account. Build succeeded without warnings/errors, 802 checks and the full disposable SQL suite passed, and EF had no pending model change. Proposed `CUS-030B` commercial defaults next.
- 2026-10-19: Completed `CUS-030B` commercial defaults. Customer now stores validated prospective payment days, a preferred company/configured currency, and either a tenant-safe default TaxRate or a required tax-exemption reason. Added bilingual responsive create/edit/detail fields, reused existing currency/tax masters and kept legacy Sale/history untouched; price lists and sales-team assignment remain owned by SAL-020+. Applied additive migration 118 to Demo, built with zero warnings/errors, passed 805 regression checks and the full SQL suite, and confirmed no pending EF model changes. Advanced the sole proposed task to `SAL-020`.
- 2026-10-19: Completed the documentation-only `SAL-020A` design gate. Accepted deterministic price-rule precedence, managed-unit normalization, dated currency direction, final-only monetary rounding, tax/exemption line snapshots and a language-neutral explainable-result contract. Preserved legacy Sale/POS and purchase pricing ownership; no code, schema, configuration, permission, data or secret changed. Advanced the sole proposed task to `SAL-020B` runtime price-list storage and read-only evaluation.
- 2026-10-20: Completed `SAL-020B` sales pricing foundation. Added tenant-safe price lists/rules, deterministic domain selection, customer default list, dated-currency/managed-unit/tax evaluation, responsive bilingual administration/evaluator, separate view/manage permissions and actionable setup links without changing Legacy Sale/POS or creating stock/accounting effects. Applied additive migration 119 to Demo and previously passed SQL isolation/uniqueness/cross-tenant-reference coverage. Final Release build passed with zero warnings/errors, 805 regression checks passed and EF reported no pending model changes; the same-day disposable SQL rerun was blocked before database creation by the local Windows SSPI authentication context and was recorded rather than treated as passed. Advanced the sole proposed task to `SAL-030` sales quotations.
- 2026-10-20: Completed `SAL-030` Sales Quotations. Added tenant-owned quotation/line/history aggregates, immutable customer/address/product/unit/price/currency/exchange/tax snapshots, request-key idempotency, rowversion lifecycle, `SQT-` numbering, six permissions, bilingual responsive list/create/detail screens, actionable errors and next-step guidance. Corrected inclusive-tax net storage and kept every quotation action free of stock/accounting/order effects. Added migration 120 and SQL isolation/idempotency/zero-effect assertions; EF has no pending model change, Release build passed and 836 regression checks passed. The disposable SQL run remained blocked by local SSPI and Demo migration application by its encryption connection, so Demo is honestly recorded at 119/120. Advanced the sole proposed task to `SAL-040` Sales Orders.
- 2026-10-20: Completed `SAL-040` Sales Orders. Added one-time Accepted-quotation conversion, `SO-` numbering, copied immutable header/line commercial snapshots, Draft/Confirmed/reasoned-Cancelled history, rowversion, four permissions, bilingual responsive list/detail screens, generated-order link replacement and workflow guidance. Database uniqueness protects one order/source line per quotation, and confirmation has no reservation, stock or accounting effect. Added migration 121 and compiled SQL isolation/uniqueness/zero-effect assertions; EF has no pending model change, Release build passed and 856 regression checks passed. Disposable SQL remained blocked by local SSPI and Demo remained 119/121 because its encryption connection blocks migrations 120–121. Advanced the sole proposed task to `SAL-050` Inventory Reservation.
- 2026-10-20: Completed `SAL-050` Sales Order Inventory Reservation. Activated the existing SaleOrder reservation source with exact SalesOrderLine evidence, multiple auditable full/partial allocation records, named warehouse/location selection, explicit block-or-partial shortage policy, reasoned release/reallocation and automatic release on order cancellation. Added separate Reserve/ReleaseReservation permissions, Arabic responsive no-horizontal-scroll UI, completed-action replacement, actionable availability/location errors and next-step guidance. Migration 122 preserves tenant-unique StockTransfer sources while permitting multiple SaleOrder records. Applied migrations 120–122 to Demo; Release build passed with zero warnings/errors, 862 regression checks passed, disposable SQL integration passed partial/manual-release/reallocation/automatic-cancellation-release/tenant/concurrent-last-unit/no-OnHand/no-journal assertions, and EF reported no pending model changes. Advanced the sole proposed task to `SAL-060` Delivery, awaiting explicit approval.
- 2026-10-20: Completed `SAL-060` Sales Deliveries. Added tenant-owned Draft/Posted/Cancelled delivery documents with `DEL-` numbering, exact order/reservation/warehouse/location lines, frozen tracking and moving-average cost evidence, four permissions and responsive bilingual list/create/detail screens. Posting atomically issues the exact reserved position, decreases OnHand and Reserved, partially/finally consumes the reservation, writes Sale stock transaction plus SalesDeliveryOut movement and posts debit COGS / credit Inventory once; posted delivery/order cancellation is blocked and no invoice/payment is created. Added actionable setup/source links, next-step guidance and completed-action replacement. Applied migration 123 to Demo; Release build passed with zero warnings/errors, 884 regression checks passed, the rebuilt disposable SQL suite passed partial 0.4/final 0.6 delivery, tenant, physical, balance and journal assertions, and EF reported no pending model changes. Advanced the sole proposed task to `SAL-070` Customer Invoicing, awaiting explicit approval.
- 2026-10-20: Completed `SAL-070` Customer Invoicing. Added tenant-owned Draft/Approved/Cancelled invoices with `INV-` numbering, request idempotency, configurable Delivered/Ordered eligibility, service-product ordered basis, exact posted-delivery allocations, immutable customer/commercial/tax snapshots, proportional partial amounts and cancellation release. Added four permissions, responsive bilingual screens, Sales Order/Delivery progress actions, navigation, actionable source errors and next-step guidance; approval is explicitly not accounting posting or payment. Applied migration 124 to Demo; build passed with zero warnings/errors, 908 regression checks passed, the disposable SQL suite passed 0.4/0.6 invoices, over-invoice rejection, cancellation replacement, tenant isolation and zero premature stock/journal effects, and EF reported no pending model changes. Advanced the sole proposed task to `SAL-080` receivable/revenue/tax posting, awaiting explicit approval.
- 2026-10-21: Completed `SAL-080` Customer Invoice Accounting. Added separate Posted/Reversed states, dedicated Post/Reverse permissions, frozen company-currency and receivable/default-revenue/output-tax account evidence, source-unique balanced receivable/revenue/tax posting, delivery-derived branch/warehouse dimensions with branch revenue override, and dated reasoned exact inverse journals that release invoice eligibility without changing delivery, stock, movement, COGS or payment. Added a dedicated `JournalEntries.View` read-only linked evidence screen, bilingual responsive UI, completed-action replacement, actionable customer/tax/warehouse/branch/accounting/fiscal links and collection-boundary next steps. Applied additive migration 125 to Demo; Release Web build passed with zero warnings/errors, 919 regression checks and the full disposable SQL suite passed, and EF reported no pending model changes. Advanced the sole proposed task to `SAL-090` Customer Payments, awaiting explicit approval.
- 2026-10-21: Completed `SAL-090` Customer Payments. Added tenant-owned receipt/allocation aggregates, `CPY-` numbering, partial and multi-invoice same-customer/currency allocation, derived Unpaid/Partially Paid/Paid balances, frozen settlement/receivable/rate evidence, explicit realized exchange gain/loss, source-unique journal links and controlled exact reversal that reopens balances. Added three permissions, bilingual responsive list/create/detail screens, disappearing completed actions, actionable setup links and next-step guidance; allocated invoices must reverse payments first and collection never mutates inventory. Applied migration 126 to Demo; Release build passed, 932 regression checks and full disposable SQL receipt/reversal coverage passed, and EF reported no pending model change. Advanced the sole proposed task to `SAL-100` Returns and Credit Notes, awaiting explicit approval.
- 2026-10-21: Completed `SAL-100A` Customer Return Authorization. Added delivery-backed tenant-owned request/line/history, `CRQ-` numbering, exact source snapshots, Draft/Submitted/Approved/Rejected/Cancelled lifecycle, six permissions, bilingual responsive screens, completed-action replacement and receipt/inspection next-step guidance. Active cumulative quantities are capped in Serializable creation; authorization has no stock/accounting/credit-note effect and legacy Sale/POS returns remain unchanged. Applied migration 127 to Demo; Debug build passed with zero warnings/errors and 947 regression checks passed. Advanced the approved SAL-100 work to `SAL-100B` receipt, inspection and disposition.
- 2026-10-21: Completed `SAL-100B` Customer Return Receipt and Inspection. Added tenant-owned partial/multiple receipt/line/history, `CRR-` numbering, Restock/Quarantine/Reject/Destroy evidence, exact source-delivery lot/serial validation, four permissions, bilingual responsive screens and completed-action replacement. Inspection freezes custody/disposition without inventory, tracking balance, journal, credit-note or refund mutation. Applied migration 128 to Demo; build passed with zero warnings/errors and 962 regression checks passed. Advanced the approved work to `SAL-100C` inventory/COGS disposition posting.
- 2026-10-21: Completed `SAL-100C` in source. Added inspected-to-posted return disposition workflow, dedicated permission, inventory/location/lot-serial restoration at frozen delivery cost, immutable stock transaction/movement evidence and balanced Inventory/COGS reversal, while Quarantine/Reject/Destroy and credit-note/refund boundaries remain stock/accounting neutral as designed. Added bilingual UI evidence, setup/next-step guidance, migration 129 and a disposable SQL Restock scenario. Release build passed with zero warnings/errors, 962 regression checks passed and EF reported no pending model change. SQL integration and Demo migration were blocked before execution by host SQL SSPI/encryption connectivity, so Demo remains evidenced at 128 and operational proof is explicitly unverified.
- 2026-10-22: Completed `SAL-100D` in source. Added independent tenant-owned customer credit notes allocated to Posted invoice and return-receipt quantities, exact delivery caps, tenant-unique request idempotency, proportional residual-safe amounts and balanced receivable/revenue/output-tax reversal without inventory or cash mutation. Invoice/payment balances now subtract posted credits and invoice reversal is blocked after credit. Added two permissions, bilingual screens/navigation/guidance, migration 130 and a disposable SQL assertion for balance/journal/no-stock behavior. Release builds passed without warnings/errors, 980 regression checks passed and EF reported no pending model changes. SQL integration and Demo migrations 129-130 remain blocked before execution by host SSPI/encryption connectivity; Demo remains evidenced at 128. Advanced the proposed next gate to `SAL-110`, requiring explicit approval.
- 2026-10-22: Completed `SAL-110` in source after explicit approval for high customizability. Added company/customer-scoped amount, credit-exposure, overdue-day, discount and margin rules with configurable comparisons, priorities, effective dates and Warn/RequireApproval/Block actions. Added arbitrary sequential role steps, requester self-approval and distinct-approver policies, frozen findings/decisions, separate permissions, bilingual rule/order UI and migration 131. Release build passed without warnings/errors, 1004 regression checks passed and EF reported no pending model changes. The disposable SQL scenario and Demo migrations 129-131 remain blocked before execution by host SQL encryption/SSPI connectivity. Advanced the proposed next gate to `SAL-120`, requiring explicit approval and a selected advanced-sales capability.
- 2026-10-22: Completed `SAL-120A` in source after explicit approval for an advanced, highly customizable quotation-template capability and competitor research. Added versioned Draft/Active/Archived templates, category/tags/search/default, reusable introduction/terms/footer, required/optional/alternative products, bounded option groups, quantity controls and additional-line policy. Template application reloads authoritative policy, reprices for customer/date and freezes template/selection snapshots without inventory or accounting effects. Added two permissions, bilingual responsive administration/application screens, migration 132, ADR, staged competitor-informed plan and compiled SQL evidence. Release builds passed with zero warnings/errors, 1017 regression checks passed and EF reported no pending model changes. SQL execution and Demo migrations 129-132 remain blocked before execution by host SSPI. Advanced the next approval gate to `SAL-120B` branded proposal/PDF presentation.
- 2026-10-22: Began explicitly approved `SAL-120B` with the reusable organization-identity foundation requested by the owner. Split regional preferences from a dedicated bilingual identity editor; added trade/legal name policy, registration/contact/address data, separately loaded signature-validated 2 MB PNG/JPEG logo and up to 20 ordered bilingual custom fields with sales/purchase/receipt/footer visibility. Added tenant isolation/configuration, migration 133, PII-safe audit evidence, antiforgery, localization, ADR and operation evidence. Release Web build passed without warnings/errors, 1026 regression checks passed and EF reported no pending model changes. Demo migration/SQL remain blocked by the known host encryption/SSPI failure; versioned document layout and frozen RTL PDF rendering remain active SAL-120B work.
- 2026-10-22: Continued approved `SAL-120B` by generalizing layout work into a central Settings Document Designer rather than a quotation-only screen. Added tenant-isolated Draft/Active/Archived design versions for multiple document types, per-type defaults, paper/language/logical margins, colors/fonts, header/logo/name placement, identity/line/total/QR/barcode visibility, bilingual cover/header/footer/watermark content, 30 ordered standard/custom sections, drag ordering and live preview. Quotation templates and quotations now select active quotation designs, and quotations freeze design code/version/name/language evidence. Consolidated the unapplied identity/designer schema into migration 133 (`20261022120233_AddOrganizationIdentityAndDocumentDesigner`), preserved the existing-row name-style default, added ADR/operation evidence and Arabic resources. Debug/Release builds passed without warnings/errors, 1037 regression checks passed and EF reported no pending changes. Demo remains at 128 due the known SQL connection failure; encoded PDF rendering and effective identity snapshot freezing remain next.
- 2026-10-22: Completed approved `SAL-120B` with a reusable Application-layer `IHtmlToPdfEngine` contract and singleton Playwright/Chromium implementation rather than per-page PDF code. Added encoded bilingual RTL/LTR quotation composition, configurable paper/orientation/logical margins/colors/fonts/header/logo/sections/watermark/page numbers, Draft preview and protected download. Sending now atomically creates a tenant-owned snapshot of effective identity, design, sections, customer/content/lines/totals and optional logo; a SHA-256 hash is verified before every historical render. Added migration 134, ADR and operation evidence. Release build passed with zero warnings/errors, 1039 regression checks passed, EF has no pending model changes, and an A4 bilingual PDF was rendered through Edge, rasterized with Poppler and visually inspected. Demo remains at 128 due the known SQL connection failure.
- 2026-10-22: Completed `SAL-130A` as a reusable Analytics Center instead of a Sales-only dashboard. Added a generic provider/query/dataset contract, tenant-owned personal/shared workspaces, advanced per-widget bilingual titles, visualization metadata, ordering, spans, Top-N, colors, targets, legends and period comparison, plus central previous-period/year execution. Sales quote-to-cash and Purchasing procure-to-pay are the first independent providers. Added three permissions, responsive RTL/LTR center/builder, migration 135, module/ADR/operation documentation and localization. Release regression build passed with zero warnings/errors, 1049 checks passed and EF reported no pending model changes. Demo remains at 128 due the known SQL connection failure; advanced workspace lifecycle/export/scheduling is proposed as SAL-130B.
- 2026-10-22: Audited the reported non-working Analytics Center, Organization Identity, Document Designs and Customer creation paths. Route/action, permission and antiforgery contracts for the first three are valid, but their runtime schema begins in migrations 133-135 while Demo is still at 128; both the normal database and an isolated LocalDB attempt were blocked by the host SQL encryption/SSPI/runtime condition before UI execution. Found and fixed an independent Customer create/edit defect: the shared view posts `Form.*` fields while both actions previously bound an unprefixed DTO. Added explicit `Form` binding, focused controller/form protections and a global regression that verifies every sidebar/dashboard button targets an existing controller action. Release build passed without warnings/errors and all 1049 checks passed.
- 2026-10-22: Completed the UI-failure remediation operationally. Applied migrations 129-135 to local Demo under the interactive Windows identity, verified EF reports no pending migrations and started the application successfully against the reconciled schema. This removes the missing-schema cause for Organization Identity, Document Designs and Analytics Center; authenticated browser journeys remain part of the general UI/E2E gap because no login secret is stored in the repository.
- 2026-10-23: Continued the repository-wide UI action audit beyond sidebar navigation. Added compiled-controller validation for every static Razor `asp-action`/`asp-controller` link and form, confirmed client-only buttons have handlers or intentional disabled/bootstrap behavior, and found 22 legacy POST actions across 13 controllers without server-side antiforgery enforcement. Added controller-level automatic validation, a global regression that prevents any MVC POST gap, and retained the customer nested-form regression. Release build passed with zero warnings/errors, all 1052 checks passed, the application started against Demo migration 135 and the protected Analytics Center route redirected normally to login.
- 2026-10-23: Completed the bilingual UI and project-wide workflow-guide pass. Replaced legacy direct English Razor labels, buttons, placeholders, JavaScript alerts and controller notifications with the shared localizer, supplied Arabic entries, removed case-variant resource duplicates, and added permanent checks for missing Arabic keys, common direct-English UI text, hard-coded action messages and case-insensitive duplicate keys. Expanded the Settings catalog from the preserved 19-step purchasing sequence to 62 real-action, permission-aware steps across seven ERP modules. Release build passed with zero warnings/errors and all 1060 regression checks passed.
- 2026-10-24: Fixed the still-silent Customer create/edit submission. Replaced prefix-only parameter binding with an explicit nested `CustomerFormPostDto` matching the Razor field shape, made form targets and submit buttons explicit, displayed the full validation summary and missing field-level errors, and localized invalid/success feedback. Added focused regression coverage for the post contract and rendered validation/submit markup. Release build passed with zero warnings/errors and all 1062 checks passed.
- 2026-10-24: Corrected customer create validation after real UI feedback. Made generated `CustomerCode` nullable at the form boundary so MVC no longer requires a value before the numbering service runs, and made `AccountId` nullable so shared-control/automatic-account policy can resolve an empty selection. Manual-selection policy retains Application-layer validation with a localized specific message. Other optional customer profile/commercial fields remain optional. Release build passed with zero warnings/errors and all 1064 checks passed.
- 2026-10-24: Fixed the Arabic-culture `RangeAttribute` crash reported through the MVC stack trace. Every Application DTO decimal range now parses declared limits and submitted values with invariant culture, covering recipe/unit conversion, document design, inventory and the professional sales lifecycle. Added assembly-wide regression that inspects every decimal range and executes it under `ar-JO`; Release build passed with zero warnings/errors and all 1066 checks passed.
- 2026-10-24: Completed explicitly approved modernization `MOD-000..001`. Audited the synchronized clean `master`/GitHub state at `fd0deb7`, created `codex/angular-modernization-foundation`, recorded the phased API/Angular architecture and same-origin coexistence ADR, and created the Angular 22.2.4 workspace at `MiniStore Front-End` inside the main repository. Added strict standalone/lazy structure, SCSS design tokens, responsive Arabic RTL/English LTR shell, a small Signals workspace-state boundary and restricted `/api`/`/Account` dev proxy without changing MVC, database or business rules. Angular production build passed at 229.81 kB raw initial bundle, Vitest passed 1/1, backend Release build passed without warnings/errors and all 1066 regression checks passed.
- 2026-10-24: Completed continuously approved `MOD-007`. Expanded Suppliers from the shared-primitives pilot into the second full Angular vertical slice with permission-separated details/create/edit routes, Application-owned active account and eligible product options, optional profile fields, existing payable-account/product-assignment rules, Problem Details, explicit HTTP 409 row-version conflicts and dirty-form recovery. MVC remains the rollback path, all component CSS remains in separate SCSS files, and no schema or business rule changed. Web Release and Angular production builds passed; 10/10 Angular files and 26/26 tests passed, backend regression passed 1081 checks and the production dependency audit reported zero vulnerabilities. Advanced the continuous gate to `MOD-008` Command Center foundation.
- 2026-10-24: Completed continuously approved `MOD-008`. Replaced the Angular home placeholder with a reusable bilingual Command Center backed by an explicit typed quick-preview provider registry. Customers and Suppliers provide permission-filtered, server-paged previews capped at 10 rows; direct record links still use their protected APIs. Added allowlisted personal favorites and versioned view snapshots that persist only navigation/provider identifiers and comfortable/compact mode—never records, searches, URLs, credentials or permissions—and reconstruct safe objects when restoring hostile/obsolete storage. Added responsive RTL/LTR external SCSS and loading/empty/error/retry states. Angular production build passed at 276.63 kB initial; 12/12 files and 30/30 tests passed, backend evidence remains 1081 checks and npm production audit remains zero. Advanced the continuous gate to `MOD-009` smart-form productivity.
- 2026-10-24: Completed continuously approved `MOD-009`. Added reusable Customer/Supplier form productivity without a generic CRUD engine: discoverable Ctrl/Command+S routed through existing validation, duplicate-save suppression, accessible centralized error links, first-invalid/server-field focus with selector allowlisting and preserved failed inputs. Added versioned Customer/Supplier saved-list preferences restricted to non-sensitive booleans, sort enum and bounded page size; searches, current pages and record payloads are never persisted and hostile storage normalizes to defaults. Documented the deliberate no-local-business-draft policy pending classified server-side design. All CSS remains external. Angular production build passed at 276.63 kB initial and 15/15 files with 36/36 tests passed; backend evidence remains 1081 checks. Advanced the continuous gate to `MOD-010` responsive layout/preferences/accessibility.
- 2026-10-24: Completed continuously approved `MOD-010`. Upgraded non-sensitive UI preferences to version 2 with allowlisted sidebar and content-width choices while safely restoring v1 theme/density and resetting all four defaults. Preserved navigation on phones as a horizontal keyboard-accessible strip, added responsive 390/680 breakpoints, semantic density coverage, skip-to-content and route focus, plus unsaved-dialog initial/trapped/Escape/restored focus. Extended reduced-motion and forced-colors behavior. A real Arabic browser preview exposed an oversized permanent settings toolbar; replaced it with a compact accessible settings panel and visually rechecked it, then stopped the temporary server. Angular production build passed without warnings at 278.13 kB initial, 16/16 files and 39/39 tests passed, Web Release passed with zero warnings/errors, backend regression remained 1081 and npm production audit remained zero. Advanced the continuous gate to `MOD-011` data quality and recovery presentation.
- 2026-10-24: Completed continuously approved `MOD-011`. Added stable language-neutral Problem Details codes, explicit Application duplicate outcomes and a centralized correlation-bearing factory/writer for validation, duplicate, concurrency, not-found, unauthorized, forbidden and transient failures. Angular now classifies the contract centrally and uses one bilingual external-SCSS recovery component across Customer/Supplier lists and forms. Retry is limited to transient reads; failed mutations preserve values, conflicts open the latest record only when View is permitted, and support references are copyable without stack traces. Web Release and Angular production builds passed, 18/18 Angular files with 52/52 tests passed and backend regression passed 1084 checks. No schema or business rule changed. Advanced the continuous gate to `MOD-012` document journey, related records and Explain This Number.
- 2026-10-24: Completed bundled `MOD-012`. Added an Application-owned document-evidence contract and four explicit View-protected API routes for Sales Orders and Customer Invoices. The Angular Documents experience reuses one external-SCSS component for journey, permission-filtered related records and Explain This Number; totals and outstanding balances remain server-authored. Web Release passed without warnings/errors and backend regression passed 1087 checks. No schema or mutation behavior changed. Continued the approved five-phase bundle with `MOD-013` action/exception center.
- 2026-10-24: Completed the approved `MOD-013..016` bundle. MOD-013 added a server-filtered Sales Order/Customer Invoice action and exception center with contextual help and read-only evidence links. MOD-014 added versioned allowlisted Focused/Operator/Analyst profiles and a responsive split context panel that can only reorder visible navigation. MOD-015 exposed the existing provider-based Analytics Center through a protected Angular route with server comparison/grouping, allowlisted Customer/Supplier drill-down and bounded on-screen export preview. MOD-016 added a reusable runtime monitor to the actual replenishment, supplier-outbox and audit-retention hosted workers, permission-filtered SLA presentation and analytics refresh plans explicitly marked as non-delivery. All CSS remains external. Web Release and Angular production builds passed without warnings, 55/55 Angular tests and 1092 backend regression checks passed, live unauthenticated API probes returned 401 and the Angular operations route returned 200. No migration, financial calculation or business mutation rule changed. Advanced the gate to `MOD-017` governed report-builder/delegation/financial-preview planning.
- 2026-10-24: Expanded the approved Angular roadmap from a foundation ending at an MVC retirement assessment into a complete presentation migration program through `MOD-030`. Added a mandatory migration registry and parity definition covering every MVC view, route, action, control, JavaScript behavior, permission, localization, file/PDF/download and edge state. The program now sequences identity/settings, catalog, WMS, purchasing, professional sales, POS/legacy sales, accounting, SaaS/platform and cross-cutting remainder waves, followed by full dual-run acceptance and an explicit Angular-only retirement gate. No runtime code, schema or current next-task scope changed.
- 2026-10-24: Clarified the full migration design rule: MVC supplies the functional inventory and rollback reference, not the target visual design. Every migrated screen must be rebuilt with the Angular semantic design system, external SCSS, responsive mobile/desktop composition, RTL/LTR, themes, density and accessibility, while a separate functional-parity gate prevents any action or evidence from being lost.
- 2026-10-25: Started the approved five-phase `MOD-017..021` bundle without overstating completion. Added a permission-protected Angular report-workspace builder that saves through the existing provider-based Application service, a reusable modernization verification script, and a source-generated registry covering 89 MVC controllers and 189 user-facing views. Added the first real `MOD-020` settings overview and `MOD-021` server-paged product catalog through explicit DTO APIs; all new UI is bilingual/RTL and uses external SCSS. Web Release built with zero warnings/errors, Angular production build passed, 59/59 Angular tests and 1095 backend checks passed, and the external-style policy passed. No schema or business rule changed; delegation/financial preview, full acceptance automation, cutover controls and complete settings/catalog parity remain open.
- 2026-10-25: Completed `MOD-017B` financial preview without a schema or accounting-rule change. `AnalyticsCenterService` now emits only authorized money widgets with company currency, selected period, provider provenance, organization rounding precision, bilingual definitions and generation time; Angular renders the server-rounded values without financial calculation. Web Release and Angular production builds passed, 59/59 Angular tests and 1096 backend regression checks passed. Advanced the next gate to isolated user/role delegation with expiry, revocation and source-permission rechecks.
- 2026-10-25: Completed `MOD-017C` governed analytics delegation in source. Added tenant-owned user/role grants with optional start/expiry, immutable creator/revoker evidence, duplicate/current-count bounds, rowversion concurrency, active role-aware workspace discovery and tenant-composite workspace/role relationships. Owner or analytics-manager administration uses protected API endpoints and server-supplied active tenant principals; provider permission remains mandatory on every delegated execution. Added a bilingual RTL/LTR Angular access screen with external SCSS, source migration `20261025025522_AddAnalyticsWorkspaceDelegation`, ADR and evidence docs. Web Release and Angular production builds passed, 61/61 Angular tests and 1106 backend checks passed. Demo migration remains unapplied; advanced the next gate to `MOD-018A` acceptance automation.
- 2026-10-25: Completed `MOD-018A` acceptance automation. Expanded the single modernization command to Release/backend regression, Angular tests/build, external-SCSS, static accessibility, explicit initial/lazy bundle budgets and a real Edge/Playwright pass for RTL/LTR, keyboard skip navigation, representative loading/empty/error states and 390px layout. Added one passed/failed/skipped JSON report and optional real API/performance/authenticated read probes using only an external storage-state file. The browser gate exposed and fixed skip-link focus. Final local evidence passed ten results, 1106 backend checks, 61/61 Angular tests, anonymous session 401 and 100/100 availability requests with zero failures (p95 10.32ms, p99 10.61ms); bundle evidence was 290,987/358,400 initial and 52,866/81,920 largest lazy. Only authenticated-session evidence was skipped; advanced to `MOD-018B`.
- 2026-10-25: Completed `MOD-019` without changing the active presentation. Expanded the generated migration registry from view counts to 89 controllers (79 MVC/10 API), 189 user-facing views, 344 MVC actions and control summaries for 198 Razor files. Added validated per-surface `Mvc`/`AngularPilot`/`AngularDefault` configuration and a code-owned allowlist that can redirect only GET/HEAD actions under a local `/app` base; POST, downloads, missing values and unknown actions remain MVC. All committed surface modes are `Mvc`. Added an ADR and regression assertions; Release built with zero warnings/errors and all 1113 focused checks passed. `MOD-018B` remains open because no external authenticated storage state was supplied.
- 2026-10-25: Completed `MOD-018B` with a real authorized local session without storing credentials. Added an interactive Edge capture mode and wrapper that writes Playwright state only to a unique Windows temporary directory, runs the established gate and deletes the directory in `finally`. The final report passed 11/11 with zero failures/skips: Release, 1113 backend checks, 61/61 Angular tests, SCSS/accessibility/bundle/browser checks, anonymous 401, authenticated session, three permission-allowed read endpoints and 100/100 availability requests (p95 40.37 ms, p99 61.85 ms). Confirmed the temporary session directory was deleted; advanced to `MOD-020`.
- 2026-10-25: Completed `MOD-020A`, the first writable Angular settings slice. Added an authenticated `Administration.Access` PUT for operating preferences through the existing `GeneralSettingsService`; rowversion validation, semantic audit and tenant isolation stay authoritative, and the API invalidates the company-culture cache just like MVC. The bilingual/RTL Angular form covers currency, decimal precision, date format, time zone and default language with Ctrl+S, duplicate-submit prevention, success/error states and dirty navigation protection. Web Release passed with zero warnings/errors, SecurityRegression passed 1113 checks, Angular passed 64/64 tests across 24 files, production build/accessibility/bundle budgets passed, and Settings remains `Mvc` by default.
- 2026-10-25: Completed five additional `MOD-020B..F` settings slices under the rule that Angular migration means
  every applicable operation, not GET-only. Added bilingual responsive forms and explicit protected APIs for organization
  identity, up to 20 custom identity fields, PNG/JPEG logo upload/removal, discount controls and purchase-matching
  tolerances. All mutations delegate to existing Application services, preserve semantic audit and antiforgery, return
  refreshed DTOs, retain failed edits and use rowversion concurrency; purchase matching now carries its existing database
  rowversion and all stale settings writes return coded HTTP 409 instead of 500. Settings remains `Mvc` by default and
  no schema/business rule changed. Web Release passed with zero warnings/errors, SecurityRegression passed 1124 checks,
  Angular passed 69/69 tests across 25 files, 28 templates passed static accessibility, and production bundle evidence
  was 291,518/358,400 initial with a 54,678/81,920 largest lazy chunk. Advanced to `MOD-020G` complete inventory settings.
- 2026-10-25: Completed `MOD-020G` inventory settings as a full Angular GET/PUT cycle. The bilingual form covers all
  warehouse type, control mode, picking and operational defaults, clears incompatible exact-location flags in Simple
  mode, preserves semantic audit and rowversion, and maps stale writes to HTTP 409. MVC remains the rollback path and
  cutover remains disabled. Web Release passed, Angular passed 72/72 tests and production build passed; security
  regression passed 1127 checks. Advanced to `MOD-020H` purchase-sourcing policy.
- 2026-10-25: Completed `MOD-020H` purchase-sourcing policy with protected Angular GET/PUT operations. The form uses
  server-provided tenant roles and company currency, preserves the direct-RFQ amount/role/supplier rules, semantic audit
  and rowversion, and maps stale writes to 409. Web Release passed, Angular passed 73/73 tests and SecurityRegression
  passed 1130 checks. MVC remains default; advanced to `MOD-020I` document numbering.
- 2026-10-25: Completed `MOD-020I` document numbering with protected GET and atomic PUT for all 30 supported document
  types. Angular edits prefix, suffix, template, length, forward-only next number, reset start/period and displays the
  server preview; every row retains its rowversion and refreshed evidence. Stale writes return 409 and MVC remains the
  default. SecurityRegression passed 1133 checks, Angular passed 75/75 tests, 31 templates passed accessibility, and
  bundle evidence was 292,010/358,400 initial with a 54,678/81,920 largest lazy chunk.
- 2026-10-26: Completed `MOD-020J` fiscal periods with protected Angular GET, POST create and PUT status operations.
  The bilingual responsive screen preserves non-overlap and Open/Soft Closed/Closed domain rules, requires a reason for
  status changes, sends the current rowversion, keeps failed drafts and maps stale writes to coded HTTP 409. MVC remains
  the default. Web Release passed with zero warnings/errors, SecurityRegression passed 1137 checks, Angular passed
  78/78 tests across 25 files, 32 templates passed accessibility, and bundle evidence was 292,786/358,400 initial with
  a 54,668/81,920 largest lazy chunk. Advanced to `MOD-020K` currency exchange rates.
- 2026-10-26: Completed `MOD-020K` currency exchange rates with protected Angular GET and POST upsert operations.
  The bilingual responsive screen shows the company currency and records a normalized three-letter foreign currency,
  effective date, positive company-currency rate and bounded source through the existing Application/domain rules.
  Failed validation preserves the form and MVC remains default. Web Release passed with zero warnings/errors,
  SecurityRegression passed 1140 checks, Angular passed 81/81 tests across 25 files, 33 templates passed accessibility,
  and bundle evidence was 292,959/358,400 initial with a 54,668/81,920 largest lazy chunk. Advanced to
  `MOD-020L` measurement units.
- 2026-10-26: Completed `MOD-020L` measurement units with protected Angular GET, create POST and deactivate POST.
  The bilingual responsive screen covers code/name/symbol, Count/Mass/Volume/Length, positive base factor and zero-to-six
  decimals; built-in units cannot be deactivated and custom deactivation requires confirmation plus a domain recheck.
  Failed creates preserve input and MVC remains default. Web Release passed with zero warnings/errors,
  SecurityRegression passed 1144 checks, Angular passed 85/85 tests across 25 files, 34 templates passed accessibility,
  and bundle evidence was 293,121/358,400 initial with a 54,668/81,920 largest lazy chunk. Advanced to
  `MOD-020M` accounting settings.
- 2026-10-26: Completed `MOD-020N` branch sales accounts with an Application-owned DTO projection, protected Angular
  GET and per-branch PUT assignment. The UI uses server-provided account options, preserves unsaved row selections and
  never exposes Branch entities. Web Release passed with zero warnings/errors, SecurityRegression passed 1150 checks,
  Angular passed 89/89 tests, 36 templates passed accessibility and bundle evidence was 293,447/358,400 initial with
  a 54,668/81,920 largest lazy chunk. MVC remains default; advanced to `MOD-020O` inventory access and terminals.
- 2026-10-26: Completed `MOD-020M` accounting settings with protected Angular GET/PUT over the existing Application
  service. The complete posting-account, customer-receivable and invoice-quantity policy contract is server-authored;
  the service now consistently applies posting accounts even when receivable mode is supplied. Failed controlled
  transitions preserve input and MVC remains default.
- 2026-10-26: Completed `MOD-020O` inventory access and terminals with protected Angular GET, branch-access PUT,
  terminal-create POST and terminal-warehouse PUT. The bilingual RTL-ready screen covers every operation flag,
  priority/default selection and filters POS warehouses from server-provided branch access while the Application service
  revalidates every boundary. Rejected writes preserve the submitted form. Web Release passed with zero warnings/errors,
  SecurityRegression passed 1155 checks, Angular passed 94/94 tests across 25 files, 37 templates passed accessibility,
  and bundle evidence was 293,607/358,400 initial with a 57,784/81,920 largest lazy chunk. MVC remains default;
  advanced to `MOD-020P` POS experience settings.
- 2026-10-26: Completed `MOD-020P` POS experience with protected Angular GET/PUT, terminal selection, full appearance
  and order-workflow editing, explicit server-side profile-default application and a responsive live preview. Corrected
  stale rowversion handling to the shared settings concurrency exception so the API returns coded HTTP 409. Web Release
  passed without warnings/errors, SecurityRegression passed 1158 checks, Angular passed 98/98 tests, 38 templates
  passed accessibility and bundles stayed within budget.
- 2026-10-26: Completed `MOD-020Q` workflow catalog as its truthful read-only Angular surface. The server localizes all
  code-defined steps and emits destination URLs only after the existing permission policy succeeds; Angular groups the
  62 steps by module and phase without inventing mutation operations. SecurityRegression passed 1161 checks, Angular
  passed 100/100 tests across 26 files, 39 templates passed accessibility and bundle evidence was 293,933/358,400
  initial with a 63,998/81,920 largest lazy chunk. All `SettingsController` flows now have Angular parity; advanced to
  `MOD-020R` document-design administration while MVC remains default.
- 2026-10-26: Completed `MOD-020R` document-design administration with protected list/filter, create-form defaults,
  details, full create/clone, activate, archive and set-default API operations over `DocumentDesignService`. Angular now
  provides the complete bilingual RTL-ready editor for page/language/margins/brand/header/identity visibility/written
  content and up to 30 ordered sections with a live preview. Stale lifecycle commands return coded HTTP 409 and rejected
  creates retain the complete draft. Web Release passed with zero warnings/errors, SecurityRegression passed 1163 checks,
  Angular passed 104/104 tests across 28 files, 42 templates passed accessibility and bundle evidence was
  294,996/358,400 initial with a 75,003/81,920 largest lazy chunk. MVC remains default; advanced to `MOD-020S` account
  and session flows.
- 2026-10-26: Completed `MOD-020S` account and session flows without changing ASP.NET Core Identity or the database.
  Extracted one audited tenant authentication service shared by MVC and API, added rate-limited and CSRF-protected
  login plus authenticated logout, made only antiforgery-token issuance anonymous, and returned localized Problem
  Details for API rate limits. Angular now provides bilingual RTL/LTR login and access-denied pages, authenticated route
  guards, current user/company evidence, protected logout and expired-session return routing. Passwords and tokens are
  never persisted in browser storage; the HttpOnly cookie and server session remain authoritative. Web Release passed
  with zero warnings/errors, SecurityRegression passed 1168 checks, Angular passed 111/111 tests across 32 files,
  43 templates passed accessibility, and production bundles passed at 296,570/358,400 initial with a
  77,410/81,920 largest lazy chunk. A live proxy probe issued an anonymous CSRF token and returned HTTP 400 Problem
  Details for an invalid protected login request. MVC remains default; advanced to `MOD-020T` users, roles and permissions.
- 2026-10-26: Completed `MOD-020T` users, roles and permissions for every operation present in the MVC source: tenant-user
  list/create and tenant-role list/create/edit/delete. Added explicit Application/API DTOs, a shared MVC/API user
  administration service with subscription/tenant-role validation and compensating Identity cleanup, code-only permission
  projection, antiforgery-protected mutations, system-role guards and HTTP 409 role concurrency. Angular adds bilingual
  RTL-ready lists/forms and permission-aware navigation/settings destinations; rejected user creation clears the password
  while retaining non-secret fields. Web Release passed with zero warnings/errors, SecurityRegression passed 1175 checks,
  Angular passed 115/115 tests across 34 files, 47 templates passed accessibility and production bundles passed at
  297.21 kB initial with an 80.87 kB largest lazy chunk. MVC remains default; advanced to `MOD-020U` registration/onboarding.
- 2026-10-27: Completed `MOD-020U` registration and guided company onboarding. Added public plan/read and CSRF/rate-limited
  registration API contracts, one MVC/API registration-and-cookie-sign-in service, and tenant-Admin protected GET,
  Complete and Skip onboarding operations over the existing serializable setup service. Angular now owns bilingual
  RTL/LTR registration, secret-safe rejection recovery, selected-plan trial entry, business preset choices, tax/POS/
  inventory/fiscal inputs, explicit skip and pending-login resume. Feature copy is lazy so the bundle gate remains within
  budget; access-denied presentation was also moved to external HTML/SCSS. Web Release passed without warnings/errors,
  SecurityRegression passed 1182 checks, Angular passed 122/122 tests across 37 files, 50 templates passed accessibility,
  external SCSS passed, and bundles measured 297,465/358,400 initial and 80,866/81,920 largest lazy bytes. `MOD-020`
  is functionally complete with MVC still default; advanced to `MOD-021A` product master operations.
- 2026-10-27: Completed `MOD-021A` product-master functional parity. `ProductsApiController` now exposes authenticated,
  permission-separated details/form/create/update/delete contracts with explicit antiforgery and localized Problem
  Details, while `ProductService` remains the owner of barcode, unit, category, price, channel, purchasing, supplier,
  tracking, shelf-life and logistics rules. Angular adds bilingual RTL-ready list/details/create/edit/delete routes,
  all current MVC fields, dirty-form protection, confirmed deletion and feature-local lazy copy; display names replace
  internal IDs in user-facing evidence. Web Release passed with zero warnings/errors, SecurityRegression passed 1186
  checks, Angular passed 127/127 tests across 38 files, 52 templates passed accessibility, external SCSS passed, and
  bundles measured 297,865/358,400 initial and 80,866/81,920 largest lazy bytes. MVC remains default; advanced to
  `MOD-021B` product-category operations.
- 2026-10-27: Completed `MOD-021B` product-category functional parity. Added an authenticated versioned API and
  bilingual RTL-ready Angular `/products/categories` surface for list, create and activate/deactivate operations over
  `ProductCategoryService`. Reads require `Products.View`; mutations require `Products.Edit` plus antiforgery. Category
  codes are language-neutral and server-normalized, duplicate failures retain the form, and inactive history is
  preserved without inventing delete. Web Release passed without warnings/errors, SecurityRegression passed 1189
  checks, Angular passed 131/131 tests across 40 files, 53 templates passed accessibility, external SCSS passed, and
  bundles measured 298,021/358,400 initial and 80,866/81,920 largest lazy bytes. MVC remains default; advanced to
  `MOD-021C` product attributes and values.
- 2026-10-27: Completed `MOD-021C` product-attribute functional parity. Added authenticated versioned Application-DTO
  endpoints and bilingual RTL-ready Angular `/products/attributes` and `/products/:id/attributes` surfaces for
  definition list/create/status, ordered Selection options, category applicability and typed per-product values.
  `ProductAttributeService` remains authoritative for neutral-code uniqueness, required/type/option/category rules,
  transactions and assigned-template variant refresh; every mutation requires `Products.Edit` and antiforgery, and
  rejected forms/values remain editable. Web Release passed without warnings/errors, SecurityRegression passed 1192
  checks, Angular passed 137/137 tests across 43 files, 55 templates passed accessibility, external SCSS passed, and
  bundles measured 298,342/358,400 initial and 80,866/81,920 largest lazy bytes. Live Angular routes returned 200 and
  the anonymous API returned 401. MVC remains default; advanced to `MOD-021D` product templates and variants.
- 2026-10-27: Completed `MOD-021D` product-template and controlled-variant parity. Added authenticated versioned DTO
  endpoints and bilingual RTL-ready Angular `/products/templates` for template creation, existing-SKU assignment/removal,
  grouped variants, bounded preview and explicit generation. `ProductTemplateService` remains authoritative for category
  matching, required variant values, neutral SHA-256 identity, duplicate prevention, source membership, the 50-row cap
  and safe cloning exclusions; Angular submits no trusted signature or calculated product. Web Release passed with zero
  warnings/errors, SecurityRegression passed 1195 checks, Angular passed 141/141 tests across 45 files, 56 templates
  passed accessibility, external SCSS passed, and bundles measured 298,693/358,400 initial and 80,866/81,920 largest
  lazy bytes. MVC remains default; advanced to `MOD-021E` prepared-product recipes.
- 2026-10-27: Completed `MOD-021E` prepared-product recipe parity. Added authenticated versioned DTO endpoints and
  bilingual RTL-ready Angular `/products/recipes` list plus dynamic editor for yield, ingredient products, compatible
  managed units and negative-policy guidance. Saving still creates a new immutable version through `RecipeService`,
  which owns stocked eligibility, unit-dimension conversion snapshots, positive quantities, domain duplication rules,
  product classification and the transaction; rejected rows remain editable. Web Release passed with zero warnings/
  errors, SecurityRegression passed 1198 checks, Angular passed 144/144 tests across 47 files, 58 templates passed
  accessibility, external SCSS passed, and bundles measured 298,961/358,400 initial and 80,866/81,920 largest lazy
  bytes. MVC remains default; advanced to `MOD-021F` tax rates, payment methods and branches.
- 2026-10-27: Completed `MOD-021F` tax-rate, payment-method and branch master parity. Added an authenticated,
  `Administration.Access`-protected DTO API and bilingual RTL-ready Angular `/products/catalog-masters` surface for all
  three current list/create workflows. Named accounts replace exposed account IDs, tax input uses invariant 0–100
  validation, branch code is language-neutral and normalized, and every mutation validates antiforgery while existing
  account/duplicate/domain rules remain server-owned. Web Release passed with zero warnings/errors,
  SecurityRegression passed 1201 checks, Angular passed 147/147 tests across 49 files, 59 templates passed
  accessibility, external SCSS passed, and bundles measured 299,119/358,400 initial and 80,866/81,920 largest lazy
  bytes. MVC remains default; advanced to `MOD-021G` warehouse master data.
- 2026-10-27: Completed `MOD-021G` and closed the full `MOD-021A..G` catalog wave. Added authenticated versioned DTO
  endpoints and bilingual RTL-ready Angular `/warehouses` list/create/edit/delete with named branch/account options and
  every current operational type, control mode, picking, receiving, POS, capacity and transfer-location field.
  `WarehouseService` retains SaaS limits, duplicate/accounting checks, unsafe control-mode and POS transitions and
  deletion constraints; API operations preserve independent View/Create/Edit/Delete permissions and antiforgery.
  Web Release passed with zero warnings/errors, SecurityRegression passed 1204 checks, Angular passed 150/150 tests
  across 51 files, 60 templates passed accessibility, external SCSS passed, and bundles measured 299,259/358,400
  initial and 80,866/81,920 largest lazy bytes. MVC remains default; advanced to `MOD-022A` inventory ledger.
- 2026-10-27: Completed `MOD-022A`, the first inventory Angular slice. Added authenticated, versioned, GET-only
  `/api/v1/stock-transactions` over `StockTransactionService`, protected by `StockTransactions.View`, plus a
  permission-aware bilingual RTL Angular `/inventory/transactions` route. The screen supports debounced product,
  warehouse, reference and type search; all 18 current transaction types; incoming/outgoing and inclusive date filters;
  allowlisted sorting, page sizes and paging; and server-authored quantity, unit-cost, movement-value and cost-variance
  evidence. Angular never recomputes balance or valuation and the API exposes no mutation path. Web Release passed
  with zero warnings/errors, SecurityRegression passed 1206 checks, Angular passed 151/151 tests across 52 files,
  61 templates passed accessibility, external SCSS passed, and bundles measured 302,834/358,400 initial and
  80,941/81,920 largest lazy bytes. Live route returned 200 and anonymous API access returned 401. MVC remains default;
  advanced to `MOD-022B` product stock balances and manual adjustments.
- 2026-10-27: Completed `MOD-022B`. Added versioned, permission-separated and CSRF-protected ProductStock form/create/update/delete
  contracts plus manual Adjustment In/Out and Kitchen Variance form/command contracts over the existing Application services. Added
  bilingual RTL-ready `/inventory/stocks` with named product/warehouse choices, opening balance, correction, preserved delete refusal,
  reason-required adjustment, dirty protection and rejected-input recovery. No schema, balance, location, tracking, AVCO or accounting
  rule changed and MVC remains default. Web Release passed with zero warnings/errors, SecurityRegression passed 1210 checks, Angular
  passed 155/155 tests across 54 files, accessibility passed 62 templates, external SCSS and production budgets passed at
  302,983/358,400 initial and 81,017/81,920 largest lazy bytes, and live 200/401 checks passed. Advanced to `MOD-022C` availability
  and reservation history.
- 2026-10-27: Completed `MOD-022C`. Added authenticated, versioned and GET-only availability and reservation APIs with independent
  View permissions and bounded Application-service paging. Added bilingual RTL-ready Angular `/inventory/availability` and
  `/inventory/reservations` with product/reference search, named warehouse/status filters, paging, server-authored dimensional
  quantities and reservation lifecycle evidence. Angular cannot mutate balances or reservations; no schema, tracking, valuation or
  accounting rule changed and MVC remains default. Web Release passed with zero warnings/errors, isolated SecurityRegression passed
  1213 checks, Angular passed 159/159 tests across 57 files, accessibility passed 64 templates, external SCSS passed, bundles measured
  303,235/358,400 initial and 81,211/81,920 largest lazy bytes, and live 200/401 checks passed. Advanced to `MOD-022D` inventory
  adjustments and cycle counting.
- 2026-10-27: Completed `MOD-022D`. Added an authenticated versioned inventory-adjustment API and bilingual RTL-ready Angular list,
  create and detail/count routes for every current operation: Draft creation, full-line counting, approval, posting and cancellation.
  Every command retains its independent permission and CSRF; completed actions become status evidence and rejected/dirty forms retain
  input. `InventoryAdjustmentService` and Domain remain authoritative for blind expected values, separation of duties, unchanged-stock
  and reservation validation, dimensional balance/location/tracking, AVCO and atomic movement posting. No schema or business rule
  changed and MVC remains default. Web Release passed with zero warnings/errors, isolated SecurityRegression passed 1216 checks,
  Angular passed 161/161 tests across 59 files, accessibility passed 67 templates, external SCSS passed, bundles measured
  303,702/358,400 initial and 81,296/81,920 largest lazy bytes, and live 200/401 checks passed. Advanced to `MOD-022E` inventory
  tracking and quality controls.
- 2026-10-27: Completed `MOD-022E`. Added authenticated, versioned and permission-separated inventory-tracking read/open,
  quarantine/release and recall create/close/communication endpoints, with antiforgery on every mutation and explicit named/validated
  DTOs. Added bilingual RTL-ready Angular `/inventory/tracking` for dynamic full-balance opening allocation, live expiration alerts,
  current balances and trace, reasoned quarantine/release, recall impact, manual communication evidence and closure that deliberately
  does not release quarantine. `InventoryTrackingService` remains authoritative and no schema/domain/accounting rule changed; MVC
  remains default. Web Release passed with zero warnings/errors, SecurityRegression passed 1219 checks, Angular passed 164/164 tests
  across 61 files, accessibility passed 68 templates, external SCSS passed, bundles measured 303,857/358,400 initial and
  81,401/81,920 largest lazy bytes, and live Angular 200 plus anonymous API read/write 401 checks passed. Advanced to `MOD-022F`
  warehouse locations, Unassigned putaway and exact-location relocation/history.
- 2026-10-27: Completed `MOD-022F` and the current inventory/WMS Angular wave. Added explicit validated Application DTO/page contracts
  and authenticated versioned APIs for location search/create/edit, Unassigned Stock assignment and putaway-rule create/activation,
  exact-location relocation/history and the read-only physical ledger. Added bilingual permission-aware Angular location, putaway and
  movement workspaces with hierarchy/barcode/capabilities, suggestions, idempotency, dirty recovery and external RTL-safe SCSS.
  Capacity, receive/pick eligibility, dimensional balances and movement authority remain server-owned; MVC remains default and no
  schema changed. Web Release passed with zero warnings/errors, SecurityRegression passed 1224 checks, Angular passed 165/165 tests
  across 62 files, accessibility passed 71 templates, external SCSS passed, and bundles measured 304,437/358,400 initial and
  81,662/81,920 largest lazy bytes. Advanced to `MOD-023A` Purchase Requests.
- 2026-10-27: Completed `MOD-023A`, the first purchasing Angular slice. Added authenticated, versioned Purchase Request list,
  details, named create options, create, submit and cancel contracts over `PurchaseRequestService`, preserving independent permissions,
  antiforgery, row-version concurrency, numbering, purchasability and managed-unit conversion. Added one bilingual responsive Angular
  workspace for searchable list, dynamic multi-line Draft creation, details, approval/history evidence and eligible Submit/Cancel
  commands with dirty/rejected-input recovery and external RTL-safe SCSS. MVC remains default and no schema/inventory/accounting rule
  changed. Web Release passed without warnings/errors, SecurityRegression passed 1227 checks, Angular passed 166/166 tests across
  63 files, accessibility passed 72 templates, external SCSS passed, bundles measured 304,833/358,400 initial and 81,742/81,920
  largest lazy bytes, and live Angular 200 plus anonymous API read/write 401 checks passed. Advanced to `MOD-023B` approvals.
- 2026-10-27: Completed `MOD-023B`. Added authenticated APIs for Purchase Approval Rule list/create/activate/deactivate and separately
  permissioned Purchase Request approve/reject commands, all with antiforgery and rule concurrency evidence. Extended the bilingual
  Angular purchasing workspace with warehouse/priority-scoped ordered-role rules, active-state control, current-role/step evidence and
  eligible decisions. `PurchaseApprovalService` still freezes rule instances, verifies the current tenant role and advances/rejects
  request and approval atomically; no schema or business rule changed and MVC remains default. Web Release passed without warnings/
  errors, SecurityRegression passed 1230 checks, Angular passed 167/167 tests across 63 files, accessibility passed 72 templates,
  external SCSS passed, bundles measured 305,003/358,400 initial and 81,742/81,920 largest lazy bytes, and live route 200 plus
  anonymous read 401 checks passed. Advanced to `MOD-023C` sourcing and supplier quotations.
- 2026-10-28: Completed approved `MOD-023C`. Added authenticated Angular sourcing list/details/create-from-approved-request,
  invited supplier quotation entry, comparable company-currency evidence and reason-required exact-quantity award through the existing
  `PurchaseSourcingService` and `SupplierQuotationService`. Independent permissions and antiforgery protect every command; request and
  supplier eligibility, commercial snapshots, currency/tax calculations and state transitions remain server-owned. Web Release passed
  with zero warnings/errors, SecurityRegression passed 1233 checks, Angular passed 168/168 tests across 64 files, 73 templates passed
  accessibility, external SCSS passed and bundles measured 305,736/358,400 initial and 81,742/81,920 largest lazy bytes. No schema or
  business-rule change; MVC remains default. Advanced the gate to `MOD-023D` Purchase Orders.
- 2026-10-28: Completed approved `MOD-023D`. Added authenticated Angular Purchase Order list/details, create-from-award, independent
  approve/confirm/cancel/close/reopen, direct-delivery configuration/evidence and supplier delivery confirmation over the existing
  Application service. CSRF, rowversion, permission and server lifecycle/quantity rules remain authoritative; confirmation has no early
  inventory or accounting effect. Web Release passed with zero warnings/errors, SecurityRegression passed 1236 checks, Angular passed
  169/169 tests across 65 files, 74 templates passed accessibility, external SCSS passed and bundles measured 306,260/358,400 initial and
  81,742/81,920 largest lazy bytes. No schema/business-rule change; MVC remains default. Advanced to `MOD-023E` Goods Receipts.
- 2026-10-28: Completed approved `MOD-023E`. Added authenticated Goods Receipt list/details/create-form APIs and Angular routes for
  direct create/post, idempotent barcode or product-code scanning drafts, final posting, staged quality inspection and accepted-stock
  put-away. Independent View/Create/Inspect/PutAway permissions and antiforgery are preserved; `GoodsReceiptService`, Inventory and
  Accounting remain authoritative for eligible confirmed orders, remaining quantities, tracking/date evidence, valuation, movements,
  GRNI journals and staged state transitions. Web Release passed with zero warnings/errors, SecurityRegression passed 1239 checks,
  Angular passed 170/170 tests across 66 files, 75 templates passed accessibility, external SCSS passed, and bundles measured
  306,587/358,400 initial and 81,742/81,920 largest lazy bytes. No schema/business-rule change; MVC remains default. Advanced to
  `MOD-023F` Vendor Bills.
- 2026-10-28: Completed approved `MOD-023F`. Added authenticated Vendor Bill list/details/create-form and multipart draft APIs,
  independent approve/match-override/post/cancel commands, bounded attachment download/upload, extraction generation and explicit
  editable review. Angular adds bilingual RTL-safe list/create/details routes and matching/settlement evidence. `VendorBillService`
  remains authoritative for allocations, duplicate supplier invoice protection, matching tolerances, tax, PPV, GRNI, payable and
  balanced journals. Web Release passed with zero warnings/errors, SecurityRegression passed 1242 checks, Angular passed 171/171
  tests across 67 files, 76 templates passed accessibility, external SCSS passed, and bundles measured 307,621/358,400 initial and
  81,743/81,920 largest lazy bytes. No schema/business-rule change; MVC remains default. Advanced to `MOD-023G` Supplier Payments.
- 2026-10-28: Completed approved `MOD-023G`. Added authenticated Supplier Payment list/details/create-form and atomic create/post API
  plus bilingual RTL-safe Angular routes with named active payment methods, multi-bill allocation and company-currency/carrying evidence.
  `SupplierPaymentService` remains authoritative for same-supplier/currency eligibility, paid/credited recalculation, overpayment,
  exchange gains/losses, payable and cash/bank journals. Web Release passed with zero warnings/errors, SecurityRegression passed 1245
  checks, Angular passed 172/172 tests across 68 files, 77 templates passed accessibility, external SCSS passed, and bundles measured
  308,019/358,400 initial and 81,743/81,920 largest lazy bytes. MVC remains default. Advanced to `MOD-023H` Goods Receipt Returns.
- 2026-10-28: Completed approved `MOD-023H`. Added authenticated Goods Receipt Return list/details/create-form and create-and-post API
  plus bilingual RTL-safe Angular routes with receipt-line remaining quantities, received tracking evidence and explicit removal
  allocations. `GoodsReceiptReturnService` remains authoritative for posted-receipt eligibility, unbilled return limits, current
  availability, tracking/location removal, AVCO, GRNI/PPV and balanced journals. Web Release passed with zero warnings/errors,
  SecurityRegression passed 1248 checks, Angular passed 173/173 tests across 69 files, 78 templates passed accessibility, external
  SCSS passed, and bundles measured 308,271/358,400 initial and 81,743/81,920 largest lazy bytes. Live Angular returned 200 and the
  anonymous API returned 401. MVC remains default. Advanced to `MOD-023I` Supplier Credit Notes.
- 2026-10-28: Completed approved `MOD-023I`. Added authenticated Supplier Credit Note list/details/create-form and atomic post API,
  bilingual RTL-safe Angular list/create/details routes and a permission-aware link from eligible posted Vendor Bills. Billed,
  credited and remaining quantities, received/removal tracking, tax, totals and inventory-cost evidence are visible, while
  `SupplierCreditNoteService` remains authoritative for bill eligibility, stock/tracking availability, proration, AVCO, payable,
  input-tax and PPV journals. Web Release passed with zero warnings/errors, SecurityRegression passed 1251 checks, Angular passed
  174/174 tests across 70 files, 79 templates passed accessibility, external SCSS passed and bundles measured 308,688/358,400
  initial and 81,743/81,920 largest lazy bytes. Live Angular returned 200 and anonymous API returned 401. MVC remains default.
  Advanced to `MOD-023J` controlled Purchase Document Reversals.
- 2026-10-28: Completed approved `MOD-023J`. Added authenticated Purchase Document Reversal list/details/create-form and idempotent
  post API plus bilingual RTL-safe Angular routes and permission-aware reversal links from posted Goods Receipts, Vendor Bills and
  Supplier Payments. `PurchaseDocumentReversalService` remains authoritative for downstream dependency order, fiscal periods,
  valuation and availability gates, physical/value corrections, opposite journals, original-state transitions and retry safety.
  Web Release passed with zero warnings/errors, SecurityRegression passed 1254 checks, Angular passed 175/175 tests across 71 files,
  80 templates passed accessibility, external SCSS passed and bundles measured 309,114/358,400 initial and 81,743/81,920 largest
  lazy bytes. Live Angular returned 200 and anonymous API returned 401. MVC remains default. Advanced to `MOD-023K` Landed Costs.
- 2026-10-28: Completed approved `MOD-023K`. Added authenticated Landed Cost list/details/create-form and atomic post API plus
  bilingual RTL-safe Angular routes, a permission-aware receipt link and controlled-reversal link. `LandedCostService` remains
  authoritative for quantity/value allocation, current-stock capitalization versus consumed expense, AVCO, value-only movements,
  clearing/expense accounts and journals. Web Release passed with zero warnings/errors, SecurityRegression passed 1257 checks,
  Angular passed 176/176 tests across 72 files, 81 templates passed accessibility, external SCSS passed and bundles measured
  309,507/358,400 initial and 81,743/81,920 largest lazy bytes. Live Angular returned 200 and anonymous API returned 401. MVC remains
  default. Advanced to `MOD-023L` Purchase Service Acceptances.
- 2026-10-28: Completed approved `MOD-023L`. Added authenticated Purchase Service Acceptance list/details/create-form and create API,
  bilingual RTL-safe Angular routes and a permission-aware link from eligible confirmed Purchase Orders. The UI shows ordered,
  previously completed and remaining service quantities plus immutable completion evidence; `PurchaseServiceAcceptanceService`
  remains authoritative for confirmed-order eligibility, service billing policy and cumulative limits, and no stock movement is
  created. Web Release passed with zero warnings/errors, SecurityRegression passed 1260 checks, Angular passed 177/177 tests across
  73 files, 82 templates passed accessibility, external SCSS passed and bundles measured 309,966/358,400 initial and
  81,743/81,920 largest lazy bytes. Live Angular returned 200 and anonymous API returned 401. MVC remains default. Advanced to
  `MOD-023M` Purchase Agreements and repeat templates.
- 2026-10-28: Completed approved `MOD-023M`. Added authenticated Purchase Agreement list/details/create-form/create, activate,
  cancel and capped release APIs plus Purchase Template list, create-from-order, generate-request and active-state APIs. Added
  bilingual RTL-safe Angular workspaces and permission-aware links from single-supplier awards and Purchase Orders. Existing
  Application services remain authoritative for award exclusivity, row-version lifecycle, validity, contracted remaining quantity,
  traced request generation and current product/unit eligibility; no direct inventory/accounting effect was introduced. Web Release
  passed with zero warnings/errors, SecurityRegression passed 1266 checks, Angular passed 178/178 tests across 74 files, 84 templates
  passed accessibility, external SCSS passed and bundles measured 310,690/358,400 initial and 81,743/81,920 largest lazy bytes.
  Both Angular routes returned 200 and both anonymous APIs returned 401. MVC remains default. Advanced to `MOD-023N` supplier terms.
- 2026-10-28: Completed approved `MOD-023N`. Added authenticated supplier purchasing page, exceptions, price-break and history APIs
  plus atomic bulk save, active state, bounded CSV/XLSX preview/import and export. Added a bilingual RTL-safe Angular bulk editor,
  named supplier/unit choices, import validation evidence, exception filters and immutable price history. Server services remain
  authoritative for unit compatibility, duplicates/preferred rules, concurrency, validity, base currency, atomic imports and price
  snapshots. Web Release passed with zero warnings/errors, SecurityRegression passed 1269 checks, Angular passed 179/179 tests across
  75 files, 85 templates passed accessibility, external SCSS passed and bundles measured 311,304/358,400 initial and
  81,743/81,920 largest lazy bytes. Live Angular returned 200 and anonymous API returned 401. MVC remains default. Advanced to
  `MOD-023O` automated replenishment.
- 2026-10-28: Completed `MOD-023O` Angular automated replenishment settings, manual execution and suggestion history. Release passed with zero warnings/errors, SecurityRegression 1271, Angular 180/180 across 76 files and 86 templates. MVC remains default.
- 2026-10-28: Completed `MOD-023P` purchasing analytics. Added an authenticated read-only API and bilingual RTL-safe Angular workspace
  for date/supplier/buyer/status/group filters, operational KPIs, trends, buyer workload, supplier performance, open match exceptions
  and permission-aware acceptance-stage drill-through. `PurchaseAnalyticsService` remains authoritative for all calculations; the API
  resolves buyer names and localizes KPI/source/journey evidence. Web Release passed with zero warnings/errors, SecurityRegression
  passed 1272 checks, Angular passed 181/181 tests across 77 files, 87 templates passed accessibility, external SCSS passed and bundles
  measured 311,587/358,400 initial and 81,743/81,920 largest lazy bytes. Live Angular returned 200 and anonymous API returned 401.
  MVC remains default. Advanced to `MOD-023Q` Purchase Accounting Reconciliation.
- 2026-10-28: Completed `MOD-023Q` Purchase Accounting Reconciliation. Added an authenticated read-only API and bilingual RTL-safe
  Angular workspace for as-of/exceptions filtering, warehouse inventory-to-ledger evidence, GRNI receipt balances, settlement documents
  and permission-aware source/setup links. Existing Application/repository calculations remain authoritative; Angular derives no
  trusted balance and the feature has no mutation. Release passed with zero warnings/errors, SecurityRegression passed 1273 checks,
  Angular passed 182/182 tests across 78 files, 88 templates passed accessibility, external SCSS passed and bundles measured
  311,737/358,400 initial and 81,743/81,920 largest lazy bytes. Live Angular returned 200 and anonymous API returned 401. MVC remains
  default. Advanced to `MOD-023R` Supplier Communications.
- 2026-10-28: Completed `MOD-023R` Supplier Communications. Added authenticated APIs and bilingual RTL-safe Angular preparation,
  document-history and detail workspaces, immutable RFQ attachment downloads and explicit activity evidence. Named supplier links were
  added to RFQ and Purchase Order details. Server rules still own eligibility, snapshots, activity ordering and required evidence;
  opening the mail client never claims sending. Release passed with zero warnings/errors, SecurityRegression passed 1275 checks,
  Angular passed 183/183 tests across 79 files, 89 templates passed accessibility, external SCSS passed and bundles measured
  312,007/358,400 initial and 81,743/81,920 largest lazy bytes. Live Angular returned 200 and anonymous API returned 401. MVC remains
  default. Advanced to `MOD-023S` Supplier Integrations.
- 2026-10-28: Completed `MOD-023S` Supplier Integrations management. Added authenticated management APIs and a bilingual RTL-safe
  Angular dashboard for credentials, one-time API keys, deactivation, document-scoped portal access/revocation, Purchase Order queueing,
  retries, UBL/payload downloads, submissions and external security audit. RFQ/PO pages link named supplier contexts. Server services
  retain secret protection, eligibility, idempotency and delivery rules. Release passed with zero warnings/errors, SecurityRegression
  passed 1277 checks, Angular passed 184/184 tests across 80 files, 90 templates passed accessibility, external SCSS passed and bundles
  measured 312,273/358,400 initial and 81,743/81,920 largest lazy bytes. Live Angular returned 200 and anonymous API returned 401.
  MVC remains default. Advanced to `MOD-023T` Supplier Portal.
- 2026-10-28: Completed `MOD-023T` Supplier Portal. Added an anonymous token-scoped and rate-limited API plus a bilingual RTL-safe
  public Angular route outside the employee authentication guard for RFQ/PO viewing, quotation submission, order confirmation and
  delivery notices. Mutations retain antiforgery and idempotency; server services retain token, tenant, supplier, document and
  eligibility authority. Invalid tokens produce localized Problem Details, denied security audit and no-store/no-referrer/noindex
  headers. Release passed with zero warnings/errors, SecurityRegression passed 1279 checks, Angular passed 185/185 tests across 81
  files, 91 templates passed accessibility, external SCSS passed and bundles measured 312,392/358,400 initial and 81,743/81,920
  largest lazy bytes. Live public Angular returned 200; an invalid anonymous API token returned 404 with privacy headers. MVC remains
  default. Advanced to `MOD-023U` legacy direct purchase history and returns.
- 2026-10-29: Completed `MOD-023U` and the purchasing Angular wave. Added authenticated permission-separated APIs and a bilingual
  RTL-safe lazy Angular workspace for historical direct-purchase list/details, one-time ledger posting, Purchase Return list/details
  and create-and-post with remaining quantities and bounded tracking evidence. New legacy purchase creation remains retired.
  Application services retain posting, cumulative quantity, stock, tracked identity, valuation, tax, payable, variance and journal
  authority. Release passed with zero warnings/errors, SecurityRegression passed 1282 checks, Angular passed 186/186 tests across 82
  files, 92 templates passed accessibility, external SCSS passed and bundles measured 313,034/358,400 initial and 81,743/81,920
  largest lazy bytes. Five Angular routes returned 200 and both anonymous APIs returned 401. MVC remains default. Advanced to
  `MOD-024A` Sales Price Lists and pricing evaluation.
- 2026-10-29: Completed `MOD-024A` Sales Price Lists and pricing evaluation. Added an authenticated permission-separated API and a
  bilingual RTL-safe lazy Angular workspace covering list creation/activation/archive, rule creation/activation/archive and complete
  read-only evaluation evidence. `SalesPriceListService` remains authoritative for precedence, units, quantities, dates, customer
  defaults/exemption, currency, rounding and tax; evaluation has no document, stock, accounting or payment effect. Release passed
  with zero warnings/errors, SecurityRegression passed 1284 checks, Angular passed 187/187 tests across 83 files, 93 templates passed
  accessibility, external SCSS passed and bundles measured 313,179/358,400 initial and 81,743/81,920 largest lazy bytes. Live Angular
  returned 200 and anonymous API returned 401. MVC remains default. Advanced to `MOD-024B` Sales Quotations.
- 2026-10-29: Completed `MOD-024B` Sales Quotations. Added authenticated, permission-separated list/details/create-form/create,
  PDF and Send/Accept/Reject/Expire/Cancel endpoints plus bilingual RTL-safe Angular list, composer and details/history workspaces.
  `SalesQuotationService` remains authoritative for idempotency, numbering, templates/designs, repricing, unit/currency/tax evidence,
  immutable snapshots, concurrency, transition eligibility and frozen PDF generation; no inventory, accounting or payment effect was
  added. Web Release passed with zero warnings/errors, SecurityRegression passed 1287 checks, Angular passed 188/188 tests across
  84 files, 96 templates passed accessibility, external SCSS passed and bundles measured 313,579/358,400 initial and
  81,743/81,920 largest lazy bytes. MVC remains default. Advanced to `MOD-024C` Sales Orders.
- 2026-10-29: Completed `MOD-024C` Sales Orders. Added authenticated list/details, one-time accepted-quotation conversion,
  confirmation/cancellation and approval decision APIs plus a bilingual RTL-safe Angular workspace with source links, immutable
  commercial evidence, control findings/steps, history and downstream reservation/delivery/invoice progress. Application services
  remain authoritative for numbering, source snapshots, concurrency, control evaluation, approval order and transition eligibility;
  conversion/confirmation create no reservation, stock movement or accounting entry. Web Release passed with zero warnings/errors,
  SecurityRegression passed 1290 checks, Angular passed 189/189 tests across 85 files, 97 templates passed accessibility, external
  SCSS passed and bundles measured 313,941/358,400 initial and 81,743/81,920 largest lazy bytes. MVC remains default. Advanced to
  `MOD-024D` Sales Order Reservations.
- 2026-10-29: Completed `MOD-024D` Sales Order Reservations. Added authenticated reservation-page, Reserve and Release APIs plus a
  bilingual RTL-safe Angular workspace for named warehouses/locations, shortage policy, per-line remaining quantities, stock-position
  evidence and reservation history. `SalesOrderReservationService` remains authoritative for confirmed-order eligibility,
  rowversion, cumulative committed quantities, locked balance availability and release ownership; reservations change Reserved only,
  not OnHand or accounting. Web Release passed with zero warnings/errors, SecurityRegression passed 1293 checks, Angular passed
  190/190 tests across 86 files, 98 templates passed accessibility, external SCSS passed and bundles measured 314,140/358,400 initial
  and 81,743/81,920 largest lazy bytes. MVC remains default. Advanced to `MOD-024E` Sales Deliveries.
- 2026-10-29: Completed `MOD-024E` Sales Deliveries. Added authenticated list/details/create-form/create, Post and Cancel APIs plus a
  bilingual RTL-safe Angular workspace for reservation selection, partial quantities, lot/serial allocation, immutable cost evidence,
  history and remaining-delivery continuation. `SalesDeliveryService` remains authoritative for reservation consumption, quantity
  limits, physical/tracked stock issue, valuation, movement and COGS posting; no invoice or payment is created. Web Release passed
  with zero warnings/errors, SecurityRegression passed 1296 checks, Angular passed 191/191 tests across 87 files, 99 templates passed
  accessibility, external SCSS passed and bundles measured 314,527/358,400 initial and 81,743/81,920 largest lazy bytes. MVC remains
  default. Advanced to `MOD-024F` Customer Invoices.
- 2026-10-29: Completed `MOD-024F` Customer Invoices. Added authenticated list/details/create-form/create and independent
  Approve/Cancel/Post/Reverse APIs plus a bilingual RTL-safe Angular workspace for eligible quantities, frozen customer/commercial
  evidence, source deliveries, settlement balances, lifecycle history and journal evidence. `CustomerInvoiceService` remains
  authoritative for cumulative eligibility, numbering, snapshots, accounting, reversal and outstanding balances. Web Release passed
  with zero warnings/errors, SecurityRegression passed 1299 checks, Angular passed 192/192 tests across 88 files, 100 templates
  passed accessibility, external SCSS passed and bundles measured 315,181/358,400 initial and 81,743/81,920 largest lazy bytes.
  MVC remains default. Advanced to `MOD-024G` Customer Payments.
- 2026-10-29: Completed `MOD-024G` Customer Payments. Added authenticated list/details/create-form/create-and-post and Reverse APIs
  plus a bilingual RTL-safe Angular workspace for open-invoice allocation, named payment methods, exchange-rate and receivable
  carrying evidence, journals and reversal. `CustomerPaymentService` remains authoritative for balances, currency rates, realized
  exchange differences, numbering and accounting; collection never changes inventory. Web Release passed with zero warnings/errors,
  SecurityRegression passed 1302 checks, Angular passed 193/193 tests across 89 files, 101 templates passed accessibility, external
  SCSS passed and bundles measured 315,565/358,400 initial and 81,743/81,920 largest lazy bytes. MVC remains default. Advanced to
  `MOD-024H` Customer Return Requests.
- 2026-10-29: Completed `MOD-024H` Customer Return Requests. Added authenticated list/details/create-form/create and independent
  Submit/Approve/Reject/Cancel APIs plus a bilingual RTL-safe Angular workspace for posted-delivery eligibility, cumulative
  returnable quantities, authorization history and downstream receipt evidence. `CustomerReturnRequestService` remains authoritative
  for delivery ownership, numbering, cumulative active requests, lifecycle and concurrency; authorization changes no inventory,
  accounting, invoice balance, credit note or cash. Web Release passed with zero warnings/errors, SecurityRegression passed 1302
  checks, Angular passed 194/194 tests across 90 files, 102 templates passed accessibility, external SCSS passed and bundles measured
  315,842/358,400 initial and 81,743/81,920 largest lazy bytes. MVC remains default. Advanced to `MOD-024I` Customer Return Receipts.
- 2026-10-29: Completed `MOD-024I` Customer Return Receipts. Added authenticated list/details/create-form/create and independent
  Inspect/Post Disposition/Cancel APIs plus a bilingual RTL-safe Angular workspace for remaining approved quantities, inspection
  decisions, exact tracked identities, restock/cost evidence and journal reference. `CustomerReturnReceiptService` remains
  authoritative for cumulative limits, tracking ownership, stock/location/tracking updates, historical valuation, movements and COGS
  reversal; inspection alone has no inventory or accounting effect. Web Release passed with zero warnings/errors,
  SecurityRegression passed 1302 checks, Angular passed 195/195 tests across 91 files, 103 templates passed accessibility, external
  SCSS passed and bundles measured 316,265/358,400 initial and 81,743/81,920 largest lazy bytes. MVC remains default. Advanced to
  `MOD-024J` Customer Credit Notes.
- 2026-10-29: Completed `MOD-024J` Customer Credit Notes. Added authenticated list/details/create-form/create-and-post APIs plus a
  bilingual RTL-safe Angular workspace for posted invoice/return eligibility, remaining quantities, immutable financial reversal
  evidence and journal reference. `CustomerCreditNoteService` remains authoritative for invoice/delivery/receipt ownership,
  cumulative limits, proportional currency/tax snapshots, request-key idempotency, numbering and accounting; the operation never
  repeats inventory or creates cash refund. Web Release passed with zero warnings/errors, SecurityRegression passed 1302 checks,
  Angular passed 196/196 tests across 92 files, 104 templates passed accessibility, external SCSS passed and bundles measured
  316,670/358,400 initial and 81,743/81,920 largest lazy bytes. MVC remains default. Advanced to `MOD-024K` Sales Approval Rules.
- 2026-10-29: Completed `MOD-024K` Sales Approval Rules. Added authenticated rule-page/create/active-state APIs plus a bilingual
  RTL-safe Angular workspace for customer scope, metrics, comparisons, thresholds, effects, effective dates, priority and ordered
  approver roles. `SalesOrderApprovalService` remains authoritative for customer/role validity, action-step compatibility,
  concurrency and order evaluation; Angular never evaluates or confirms orders. Web Release passed with zero warnings/errors,
  SecurityRegression passed 1302 checks, Angular passed 197/197 tests across 93 files, 105 templates passed accessibility, external
  SCSS passed and bundles measured 316,828/358,400 initial and 81,743/81,920 largest lazy bytes. MVC remains default. Advanced to
  `MOD-024L` Sales Quotation Templates.
- 2026-10-29: Completed `MOD-024L` Sales Quotation Templates. Added authenticated list/details/create-form/create and independent
  activate/archive/default APIs plus a bilingual RTL-safe Angular workspace for reusable content, required/optional/alternative
  items, option groups, price-list/document-design selection and source-version cloning. `SalesQuotationTemplateService` remains
  authoritative for code/version lineage, product/unit/tax/design validity, lifecycle concurrency and default exclusivity; Angular
  never reprices quotations. Web Release passed with zero warnings/errors, SecurityRegression passed 1302 checks, Angular passed
  198/198 tests across 94 files, 106 templates passed accessibility, external SCSS passed and bundles measured 317,266/358,400
  initial and 81,743/81,920 largest lazy bytes. MVC remains default. Completed `MOD-024` and advanced to `MOD-025A` legacy
  immediate-sale history/details, wholesale creation and one-time posting.
- 2026-10-29: Completed `MOD-025A` Legacy Wholesale Immediate Sale. Added an Application-owned workspace projection and authenticated
  list/details/create-form/create/post APIs plus a bilingual RTL-safe Angular workspace with named warehouse/customer/payment/tax
  choices, product name/code/barcode search, tracked-identity evidence and immutable accounting status. `SaleService` remains
  authoritative for server-side wholesale price, discounts, tax, numbering, inventory, tracking, valuation and atomic creation;
  `SalePostingService` retains idempotent balanced GL posting. Web Release passed with zero warnings/errors, SecurityRegression
  passed 1302 checks, Angular passed 199/199 tests across 95 files, 107 templates passed accessibility, external SCSS passed and
  bundles measured 317,629/358,400 initial and 81,743/81,920 largest lazy bytes. MVC remains default. Advanced to `MOD-025B` POS.
- 2026-10-29: Completed `MOD-025B` Retail POS. Extended the bounded sale workspace/API with active terminals, allowed warehouses,
  runtime experience settings, stock evidence and an explicit RetailPos command. Added a bilingual touch-friendly Angular POS with
  configured product layout/density/visibility, name/code/barcode search, quick barcode add, cart discounts/tracking and enabled
  order context. `SaleService` remains authoritative for access, retail price, recipes, stock, tracking, cost and numbering. Web
  Release passed with zero warnings/errors, SecurityRegression passed 1302 checks, Angular passed 200/200 tests across 95 files,
  107 templates passed accessibility, external SCSS passed and bundles measured 317,769/358,400 initial and 81,743/81,920 largest
  lazy bytes. MVC remains default. Advanced to `MOD-025C` immediate-sale returns.
- 2026-10-29: Completed `MOD-025C` Immediate-Sale Returns. Added an Application-authored create projection and authenticated
  list/details/create-form/create-and-post API plus a bilingual Angular workspace for sold/returned/remaining quantities, tracking
  capture and immutable refund/tax/restock/cost evidence. `SalesReturnService` retains cumulative limits, tracking provenance,
  inventory restoration, valuation and balanced accounting. Web Release passed with zero warnings/errors, SecurityRegression passed
  1302 checks, Angular passed 201/201 tests across 96 files, 108 templates passed accessibility, external SCSS passed and bundles
  measured 318,180/358,400 initial and 81,743/81,920 largest lazy bytes. No independent POS close/print surface exists in source;
  `MOD-025` completed and execution advanced to `MOD-026A` chart of accounts.
- 2026-10-29: Completed `MOD-026A` Chart of Accounts. Added an Application-authored named-hierarchy projection and authenticated
  list/create API plus a bilingual RTL-safe Angular workspace for codes, names, localized account types and parent selection.
  `AccountService` retains duplicate-code, parent and entity validation. Web Release passed with zero warnings/errors,
  SecurityRegression passed 1302 checks, Angular passed 202/202 tests across 97 files, 109 templates passed accessibility, external
  SCSS passed and bundles measured 318,327/358,400 initial and 81,743/81,920 largest lazy bytes. MVC remains default. Advanced to
  `MOD-026B` Journal Entry details.
- 2026-10-29: Completed `MOD-026B` Journal Entry Details. Added a read-only permission-protected API and bilingual RTL-safe Angular
  evidence screen for entry status/date/source, named accounts, branch/warehouse dimensions, line descriptions and debit/credit
  totals. No mutation was introduced; accounting services retain posting, uniqueness and reversal authority. Web Release passed
  with zero warnings/errors, SecurityRegression passed 1302 checks, Angular passed 203/203 tests across 98 files, 110 templates
  passed accessibility, external SCSS passed and bundles measured 318,598/358,400 initial and 81,743/81,920 largest lazy bytes.
  MVC remains default. Completed `MOD-026` and advanced to `MOD-027A` tenant subscription/checkout.
- 2026-10-29: Completed `MOD-027A` Tenant Subscription and Checkout. Added an Application-authored current-subscription summary,
  an authenticated read API and antiforgery-protected checkout command, plus a bilingual RTL-safe Angular workspace for lifecycle
  evidence, public plan selection, promotion entry and the resulting 30-minute quote. The client submits no price, discount,
  currency, expiry or payment state; `BillingCheckoutService` reloads and validates all commercial inputs and persists Pending.
  Entitlement recovery now permits only session bootstrap, subscription API and the subscription Angular route while continuing to
  block the rest of the ERP. Web Release passed with zero warnings/errors, SecurityRegression passed 1304 checks, Angular passed
  204/204 tests across 99 files, 111 templates passed accessibility, external SCSS passed and bundles measured
  318,742/358,400 initial and 81,743/81,920 largest lazy bytes. MVC remains default. Advanced to `MOD-027B` public landing/pricing.
- 2026-10-29: Completed `MOD-027B` Public Landing and Pricing. Added an anonymous versioned read-only plan API over
  `PlatformSaasService` plus bilingual RTL-safe Angular welcome and pricing routes outside the authenticated shell. Plan names,
  descriptions, limits and monthly/annual prices remain server-authored; registration receives only a plan ID and validates it
  again. Web Release passed with zero warnings/errors, SecurityRegression passed 1305 checks, Angular passed 205/205 tests across
  100 files, 112 templates passed accessibility, external SCSS passed and bundles measured 318,986/358,400 initial and
  81,743/81,920 largest lazy bytes. MVC remains default. Advanced to `MOD-027C` Platform sign-in/session and read-only shell.
- 2026-10-30: Completed `MOD-027C` Platform Sign-in and Session. Shared MVC/API lockout and denied-login auditing through one
  authentication service; added isolated Platform-cookie login/session/overview/logout APIs and bilingual Angular screens.
  Release passed cleanly, SecurityRegression passed 1307 checks, Angular passed 207/207 across 101 files, 114 templates passed
  accessibility and bundles measured 323,961/358,400 initial and 81,744/81,920 largest lazy bytes. Advanced to `MOD-027D`.
- 2026-10-30: Completed `MOD-027D` Platform Management. Added Platform-cookie APIs and a bilingual role-aware Angular workspace
  for companies/subscriptions, plans, promotions and pending payments. Management and Billing policies remain separate; payment
  confirmation sends only the provider reference and reuses the serializable server transaction. Release passed cleanly,
  SecurityRegression passed 1309 checks, Angular passed 207/207 across 101 files, 115 templates passed accessibility and bundles
  measured 324,362/358,400 initial and 81,744/81,920 largest lazy bytes. Advanced to `MOD-027E`.
- 2026-10-30: Completed `MOD-027E` Platform Audit and Retention. Added isolated Platform-cookie APIs and bilingual RTL-safe
  Angular workspaces for filtered/paged audit evidence, purpose-bound CSV export, retention-policy evidence, legal-hold
  creation/release, external checkpoints and retention runs. AuditView/Export/Sensitive/Retention remain independent;
  sensitive inclusion is server-derived and every mutation retains antiforgery and the existing Application audit/retention
  authority. Release passed with zero warnings/errors, SecurityRegression passed 1311 checks, Angular passed 207/207 across
  101 files, 117 templates passed accessibility and bundles measured 324,732/358,400 initial and 81,744/81,920 largest lazy
  bytes. MVC remains default. Completed `MOD-027` and advanced to `MOD-028A` source/registry reconciliation.
- 2026-10-30: Completed `MOD-028A` source/registry reconciliation. Rebuilt the generator to include MVC Areas, verify
  declared counterparts against 171 actual Angular routes and separate pilot coverage from the 11-action cutover allowlist.
  The reproducible inventory now records 160 controllers, 199 views, 358 MVC actions and 209 Razor control rows. It found
  six remaining surfaces: Stock Transfers, Inventory Scanning, Inventory Insights, Inventory Reconciliation, tenant Audit
  Events and server Language synchronization. It also exposed Purchase Sourcing attachment upload/download and print as a
  cross-surface parity gap. No business code, schema, permission or presentation default changed. Approved `MOD-028B`
  Stock Transfers as the largest and highest-risk operational gap.
- 2026-10-30: Completed `MOD-028B` Stock Transfers. Added Application-owned list/details/form projections and removed
  repository and Identity-user composition from the MVC controller. Added the authenticated, permission-separated and
  antiforgery-protected API for list/details/create/edit plus submit/approve/reject/return-to-draft/post/cancel. Angular now
  provides bilingual RTL-safe list/create/edit/details routes with named warehouses/products/locations, tracking allocations,
  dirty-form protection and state-aware actions. Existing numbering, reservations, movements, tracking, valuation, posting and
  reversal rules remain in `StockTransferService`; no schema or business transition changed. Release passed with zero
  warnings/errors, SecurityRegression passed 1,315 checks, Angular passed 208/208 tests across 102 files, 118 templates passed
  accessibility and bundles measured 325,287/358,400 initial and 81,824/81,920 largest lazy bytes. The generated registry now
  reports 175 Angular routes and five remaining MVC surfaces. MVC remains default; advanced to `MOD-028C` Inventory Scanning.
- 2026-10-30: Completed `MOD-028C` Inventory Scanning. Added a versioned authenticated API over the existing exact resolver
  and Application service for lookup, putaway, relocation and absolute Draft-count entry. The three commands retain both the
  scan permission and their independent stock/movement/count permission plus antiforgery. Angular adds a bilingual RTL-safe,
  scanner/keyboard-first route with explicit empty/resolved/not-found/ambiguous states, named balance evidence, blind-count
  hiding, server-issued workflow-scoped idempotency keys and duplicate-submit suppression. No schema or inventory rule changed.
  Release passed with zero warnings/errors, SecurityRegression passed 1,317 checks, Angular passed 209/209 tests across 103
  files, 119 templates passed accessibility and bundles measured 325,405/358,400 initial and 81,911/81,920 largest lazy bytes.
  The generated registry now reports 176 routes and four remaining surfaces. MVC remains default; advanced to `MOD-028D`.
- 2026-10-30: Completed `MOD-028D` Inventory Insights and Reconciliation. Added two independently permission-protected,
  GET-only APIs over the existing Application services and bilingual RTL-safe Angular routes for warehouse/search/activity
  thresholds, server-authored KPI values, issue evidence, exception filtering and bounded paging. Replaced the Insights
  warehouse tuple with an explicit API DTO; no classification, tolerance, value or paging rule moved to Angular and no mutation
  was introduced. Removed four proven-unused foundation localization strings to restore lazy-budget headroom without raising
  limits. Release passed with zero warnings/errors, SecurityRegression passed 1,319 checks, Angular passed 210/210 tests across
  104 files, 120 templates passed accessibility and bundles measured 325,647/358,400 initial and 81,537/81,920 largest lazy
  bytes. The registry now reports 178 routes and two remaining surfaces. MVC remains default; advanced to `MOD-028E`.
- 2026-10-30: Completed `MOD-028E` Tenant Audit Events. Added authenticated tenant audit search/paging and purpose-bound
  CSV export over the existing Application service. View/export/sensitive permissions remain independent, export retains
  antiforgery, sensitive inclusion is derived server-side and the client request has no sensitive toggle. Angular adds a
  bilingual RTL-safe route with filters, paging, redacted evidence and permission-aware safe download. Existing date bounds,
  5,000-row limit, approved purposes, CSV formula protection and semantic export evidence remain unchanged. Release passed
  with zero warnings/errors, SecurityRegression passed 1,321 checks, Angular passed 211/211 tests across 105 files, 121
  templates passed accessibility and bundles measured 325,733/358,400 initial and 81,600/81,920 largest lazy bytes. The
  registry now reports 179 routes and only Language remains unmapped. MVC remains default; advanced to `MOD-028F` sourcing
  attachments and print parity.
- 2026-10-30: Completed `MOD-028F` Purchase Sourcing attachment/print parity. Replaced the API's attachment clearing with
  bounded multipart binding under the existing Create permission and antiforgery, added tenant-filtered View-protected
  downloads with safe filename/media-type handling, and kept the Application/domain ten-file and 5 MB limits authoritative.
  Angular now selects/submits files, lists immutable metadata, downloads evidence and renders a bilingual RTL-safe RFQ
  print/save-PDF route without a mutation or new document. Release passed with zero warnings/errors, SecurityRegression
  passed 1,322 checks, Angular passed 211/211 tests across 105 files, 121 templates passed accessibility and bundles measured
  326,124/358,400 initial and 81,600/81,920 largest lazy bytes. The registry reports 180 routes; only Language remains
  unmapped and no inspected cross-surface file/print gap remains. MVC remains default; advanced to `MOD-028G`.
