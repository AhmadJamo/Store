# FRD-I Inventory Putaway Filters V4 completed (2026-11-27): FilterBar, three FieldCues and explicit apply/reset actions preserve warehouse/query and the supported product/barcode/warehouse server sort contract plus loading/error behavior. 358/358 tests, build and Playwright 7/7 pass; Unassigned Stock typed DataTable is next.
# FRD-I Inventory Putaway Rule Form V4 completed (2026-11-27): one described FormSection, four default FieldCues plus a conditional named product/category cue and WorkflowFooter preserve ProductStock.Edit, default/product/category targeting, warehouse/destination choices, priority, validation, dirty state and exact createRule payload. 357/357 tests, build and Playwright 7/7 pass; Filters V4 is next.
# FRD-I Inventory Putaway Assignment Form V4 completed (2026-11-27): localized Inventory context, one described FormSection, four required FieldCues and WorkflowFooter preserve named product/warehouse/destination choices, suggested prefill, quantity, ProductStock.Edit, idempotency, validation/reset, dirty state and exact assignment payload. 356/356 tests, build and Playwright 7/7 pass; Rule Form V4 is next.
# FRD-I Inventory Putaway safety audit completed (2026-11-27): five focused tests protect server-authored quantities/suggestions, warehouse/query/sort requests, named options, ProductStock.Edit gating, exact assignment/rule/toggle payloads, idempotency, validation, server errors and dirty navigation. 355/355 tests, build and Playwright 7/7 pass; runtime is unchanged and Assignment Form V4 is next.
# FRD-I Inventory Replenishment Workspace V4 completed (2026-11-27): typed 11-column Suggestions DataTable preserves grouped availability/projection/source evidence, server order, empty state, status and all permission-gated links/actions. 350/350 tests, build and Playwright 7/7 pass; Putaway safety audit is next.
# FRD-I Inventory Replenishment Filters V4 completed (2026-11-27): FilterBar, three FieldCues and explicit apply/reset actions preserve warehouse/search/exceptions query params and loading behavior. 350/350 tests, build and Playwright 7/7 pass; Suggestions DataTable is next.
# FRD-I Inventory Replenishment Rule Form V4 completed (2026-11-27): localized Inventory context, one described FormSection, ten FieldCues and a WorkflowFooter preserve named options, planning fields, validation, reset, Manage permission, dirty guard and exact payload. 350/350 tests, build and Playwright 7/7 pass; Filters V4 is next.
# FRD-I Inventory Replenishment safety audit completed (2026-11-27): five focused tests protect named evidence, filters, options, rule validation, dirty state, independent Manage/transfer/purchase permissions, exact save/toggle/draft commands, navigation and server errors. 350/350 tests pass; runtime is unchanged and Rule Form V4 is next.
# FRD-I Inventory Tracking Workspace V4 completed (2026-11-27): tracking history now uses a typed six-column DataTable with localized empty state while preserving server date/product/identifier/type/quantity/reference evidence and order. 347/347 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; Replenishment safety audit is next.
# FRD-I Inventory Tracking Balances/Quarantine V4 completed (2026-11-27): typed permission-aware DataTable and required reason cue preserve all balance evidence, ManageQuarantine, quantity/status gates, validation and exact quarantine/release payloads. 347/347 tests, build and Playwright 7/7 pass.
# FRD-I Inventory Tracking Recall Workspace V4 completed (2026-11-27): DetailSection preserves server recall identity/status/impact/communication evidence; two action FormSections, six FieldCues and two WorkflowFooters preserve active ManageRecall, validation and exact communication/close payloads. 347/347 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass.
# FRD-I Inventory Tracking Recall Create V4 completed (2026-11-26): described FormSection, four required FieldCues and WorkflowFooter preserve ManageRecall, named tracked products, validation, reset and exact create command. 347/347 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass.
# FRD-I Inventory Tracking Expiration Alerts V4 completed (2026-11-26): a two-item SummaryStrip and typed nine-column DataTable preserve server expired/expiring counts, severity, product/identifier/warehouse/location, quantity/date/days/status and true-empty evidence. 347/347 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass.
# FRD-I Inventory Tracking Opening Allocation V4 completed (2026-11-26): localized Inventory context, two described FormSections, nine required/optional FieldCues for the first line and WorkflowFooter preserve named product/warehouse/location choices, lot/serial policy, add/remove/reset, dates, validation, dirty guard and exact open payload. 347/347 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass.
# FRD-I Inventory Tracking safety audit completed (2026-11-26): five focused tests protect server balance/expiration/recall/communication/transaction evidence, lot/serial mappings, complete opening allocation, four independent permissions, all management commands, validation, errors and dirty navigation. 347/347 Angular tests across 140 files pass; runtime is unchanged.
# FRD-I Inventory Scanning Workspace V4 completed (2026-11-26): conditional Putaway/Relocate/Count workspaces now use described FormSections, required quantity FieldCues and WorkflowFooters while preserving independent permission/state gates, validation, idempotency keys, exact payloads, messages and refresh. 344/344 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; Tracking safety audit is next.
# FRD-I Inventory Scanning result evidence V4 completed (2026-11-26): a localized DetailSection groups the four adjustment/product/source/destination identities while preserving server statuses 0/1/2/3, display names and secondary evidence. 343/343 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; FRD-ISS-012 remains independent.
# FRD-I Inventory Scanning resolve workspace V4 completed (2026-11-26): localized Inventory context, a described FormSection, four optional FieldCues and WorkflowFooter preserve autofocus, all four query values, GET normalization and loading/recovery. 343/343 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; FRD-ISS-012 remains independent.
# Project TODO
# FRD-I Inventory Scanning safety audit completed (2026-11-26): five focused tests protect four resolver states, server scan/balance evidence, compound permissions, exact Putaway/Relocate/Count commands, idempotency keys, quantity validation, blind count and server errors. 343/343 Angular tests across 140 files pass; runtime is unchanged.
# FRD-I Stock Transfer Receipt V4 completed (2026-11-26): described receipt evidence, three received/short/damaged FieldCues and dedicated WorkflowFooter preserve states 7/8/9, remaining limits, destination, permission, empty blocking and exact receive payload. Stock Transfer Workspace is complete. 338 tests, build, 142 templates, 122 pages and Playwright 7/7 pass; FRD-ISS-012 remains independent.
# FRD-I Stock Transfer Record Evidence V4 completed (2026-11-26): typed seven-column item and five-column history tables preserve product, quantities, destination, transitions, user, reason and timestamp. 338 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; FRD-ISS-012 remains independent.
# FRD-I Stock Transfer Form V4 completed (2026-11-26): localized Inventory context, described transfer and line-item FormSections, five FieldCues per line and conditional location cues preserve named options, barcode, add/remove, resets, draft recovery, validation, create/update and dirty guard. 338 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; FRD-ISS-012 remains independent.
# FRD-I Stock Transfer safety audit completed (2026-11-26): six focused component tests protect server list evidence, all ten statuses, exact action/receipt matrix, independent permissions, named warehouse/location requirements, distinct warehouses, create and workflow commands, reasons, discrepancy inputs and dirty navigation. 338/338 Angular tests across 139 files pass; runtime is unchanged.
# FRD-I Product Stock manual-adjustment Form V4 completed (2026-11-26): localized FormSection, five required FieldCues and WorkflowFooter preserve StockTransactions.Create, named selectors, types 6/7/8, positive quantity, required reason, validation/focus, dirty guard and createAdjustment. Product Stock Workspace is complete. 332 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; FRD-ISS-012 remains independent.
# FRD-I Product Stock opening/edit Form V4 completed (2026-11-26): localized FormSection, three required FieldCues and WorkflowFooter preserve named selectors, locked edit identity, non-negative quantity, separate create/update commands, validation/focus and dirty guard. 332 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; FRD-ISS-012 remains independent.
# FRD-I Product Stock List V4 completed (2026-11-26): localized Inventory context, two-item SummaryStrip and typed seven-column DataTable preserve server named rows, quantities, AVCO/value, order, empty state and Edit/Delete permissions while leaving both operation forms unchanged. 332 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; FRD-ISS-012 remains independent.
# FRD-I Product Stock safety net completed (2026-11-26): seven focused tests protect server named rows and quantity/cost/value evidence, independent permissions, deep edit identity, separate opening/create and current-quantity update commands, manual types 6/7/8 with reason, validation/focus, dirty guard and server delete/reduction failures. Targeted 7/7 tests pass; runtime is unchanged.
# FRD-I Inventory Adjustment Record V4 completed (2026-11-26): localized WorkflowProgress, DetailSection summary, draft FormSection, typed four-column results and WorkflowFooter preserve all five states, blind-count evidence, independent permissions, commands, validation and dirty guard. 328 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; FRD-ISS-012 remains independent.
# FRD-I Inventory Adjustment Record safety net completed (2026-11-26): five focused tests protect server record evidence, Draft/Counted/Approved/Posted/Cancelled action matrix, blind expected-quantity hiding, independent permissions, completed states, exact count/approve/post/cancel commands, invalid counts and dirty leave confirmation. Targeted 5/5 tests pass; runtime is unchanged.
# FRD-I Inventory Adjustment Create V4 completed (2026-11-26): localized Inventory context, three FormSections, FieldCues and WorkflowFooter preserve named warehouse/countable-location/stocked-product choices, dependency reset, blind count, validation/focus, dirty guard, exact POST and record navigation. 324 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; FRD-ISS-012 remains independent.
# FRD-I Inventory Adjustment Create safety net completed (2026-11-26): five focused tests protect server-returned named warehouse/countable-location/stocked-product choices, warehouse/location dependency, blind-count/reason/product payload, validation focus, dirty leave confirmation and record navigation after success. Targeted 5/5 tests pass; runtime and workflow authority are unchanged.
# FRD-I Inventory Adjustment List V4 completed (2026-11-26): localized Inventory context, three-item SummaryStrip and typed eight-column DataTable preserve the complete newest-first server response, document/warehouse/location/count/date evidence, all five workflow states, detail links and independent Create permission without inventing filters or paging. 319 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; FRD-ISS-012 remains independent.
# FRD-I Inventory Adjustment List safety net completed (2026-11-26): five focused tests protect server-returned document/warehouse/location/count/date evidence, all five workflow states, newest-first service order, independent Create permission and loading/error/empty behavior. The GET contract has no filter or paging inputs, and the UI will not invent them. Targeted 5/5 tests pass; runtime behavior is unchanged.
# FRD-I Inventory Reconciliation slice 5 completed (2026-11-26): localized Inventory context, FilterBar, two-item SummaryStrip, typed nine-column DataTable and shared Pager preserve server-authored counters, named product/warehouse evidence, quantities/value/latest movement, all seven issue mappings, consistency, filters/paging and independent permission navigation while distinguishing true-empty/filtered-empty. 314 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; FRD-ISS-012 remains the sole independent gate failure.
# FRD-I Inventory Reconciliation safety net completed (2026-11-26): five focused tests protect server-authored counters, named rows, warehouse/assigned/unassigned/latest quantities, value/latest movement, all seven issue mappings, consistent state, filters/paging, permission-aware Insights navigation and empty behavior. Targeted 5/5 tests pass; runtime behavior is unchanged.
# FRD-I Inventory Insights slice 4 completed (2026-11-26): localized Inventory context, FilterBar, five-item SummaryStrip and typed seven-column DataTable preserve server-authored KPIs, named product/warehouse rows, quantities/values/activity classifications, thresholds, filters and permission-aware Reconciliation navigation while distinguishing true-empty/filtered-empty. 309 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; the unchanged 84.55 kB core-localization lazy-budget issue remains tracked.
# FRD-I Inventory Insights safety net completed (2026-11-26): five focused tests protect server-authored KPIs, named product/warehouse rows, quantities/values/activity classification, filters and threshold validation, the permission-aware Reconciliation link and empty behavior before presentation changes. Targeted 5/5 tests pass; runtime behavior is unchanged.
# FRD-I Stock Transactions slice 3 completed (2026-11-26): localized Inventory context, FilterBar, SummaryStrip and typed sortable ten-column DataTable preserve immutable server identity/name/quantity/cost/type/reference/date evidence, all type/movement/date/search/sort/page contracts and GET-only authority while distinguishing true-empty/filtered-empty. 304 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; the unchanged 84.55 kB core-localization lazy-budget issue remains tracked.
# FRD-I Stock Transactions safety net completed (2026-11-26): five focused tests protect the ten displayed server-authored identity/name/quantity/cost/type/reference/date fields, type/movement/date filters, debounced search, sorting, bounded paging/reset and empty behavior before presentation changes. Targeted 5/5 tests pass; runtime behavior is unchanged.
# FRD-I Inventory Reservations slice 2 completed (2026-11-26): localized Inventory context, FilterBar, SummaryStrip and typed seven-column DataTable preserve server-authored reference/source/status/quantity/lifecycle evidence, server search/status/paging/reset and GET-only authority while distinguishing true-empty/filtered-empty. 299 tests, production build, 142 templates, 122 pages and Playwright 7/7 pass; the independent 84.55 kB core-localization lazy-budget issue is tracked.
# FRD-I Inventory Reservations safety net completed (2026-11-26): five focused tests protect server-authored reference/source/status/quantity/lifecycle evidence, debounced search, status/paging query, bounded reset and empty behavior before presentation changes. Targeted 5/5 tests pass; runtime behavior is unchanged.
# FRD-I Inventory Availability slice 1 completed (2026-11-26): localized Inventory context, FilterBar, SummaryStrip and typed DataTable preserve server-authored product/warehouse/location names, OnHand/Reserved/Available, debounced server search, warehouse/paging/reset and internal IDs while distinguishing true-empty/filtered-empty. 295 tests, build, 142 templates, 122 pages and Playwright 7/7 pass.
# FRD-I Inventory Availability safety net completed (2026-11-25): five focused tests protect server-authored named product/warehouse/location rows, displayed quantities, debounced search, warehouse/paging query, bounded reset, existing hierarchy and empty behavior before presentation changes. Targeted 5/5 tests pass; runtime behavior is unchanged.
# FRD-I Inventory audit completed (2026-11-25): 19 routes across 15 templates are classified by read/master/workflow risk, API/Application authority, permissions, branch/warehouse context and test coverage. Inventory Availability is the first bounded GET-only slice; its safety net must be committed before UI changes. No runtime behavior changed.
# FRD-H closed (2026-11-25): Branch Workspace, Warehouse Master and Warehouse Locations are migrated V4 surfaces with branch/context/name/permission parity. 291 tests, build, 142 templates, 122 pages, Playwright 7/7 and 1,668 SecurityRegression checks pass; live authenticated API remains an explicit external gate. Evidence: frontend-redesign/acceptance/branches-warehouses-pilot-2026-11-25/.
# FRD-H Branch Workspace slice 3 completed (2026-11-25): localized Administration context, SummaryStrip, FormSection/FieldCue/WorkflowFooter and separate company, branch-directory and access DetailSections replace the mixed legacy hierarchy while preserving server-returned scope, named linked records, current-branch links, create/edit/status/grant commands and all permission gates. 291 tests, build, 142 templates and 122-page consistency pass.
# FRD-H Branch Workspace safety net completed (2026-11-25): extracted the inline template into statically scanned HTML and added component coverage for server-returned scope, current operations/links, create/status commands and user-grant replacement. Focused 3/3 tests and 142-template accessibility pass.
# FRD-H Warehouse Locations slice 2 completed (2026-11-25): localized Inventory context, three FormSections, required FieldCues, WorkflowFooter, FilterBar, SummaryStrip and typed hierarchical DataTable preserve all fields, hierarchy, capabilities, status, named warehouse IDs and View/Create/Edit behavior. 287 tests, build, 141 templates, 121 pages and Playwright 7/7 pass.
# FRD-H Warehouse Locations safety net completed (2026-11-25): new component coverage protects warehouse-name rendering, hierarchical rows, the full create payload and parent-preserving edit updates before presentation changes. Focused 3/3 tests pass.
# FRD-H Warehouse Master slice 1 completed (2026-11-25): localized Inventory context, three FormSections, required FieldCues, WorkflowFooter, typed DataTable and warehouse-specific Delete label preserve every setting, named branch/account ID choice, dirty/recovery behavior and View/Create/Edit/Delete gate. 283 tests, build, 141 templates, 121 pages and Playwright 7/7 pass.
# FRD-H Branches/Warehouses audit completed (2026-11-25): Branch Workspace, Warehouse Master, Warehouse Locations, Shell Branch Switcher and 45 warehouse-picker references are scoped with permission/context/name-selection invariants. Warehouse Master List/Form is the first bounded slice; no runtime behavior changed.
# FRD-G closed (2026-11-25): Customers/Suppliers List/Form/Record migrated with contract and permission parity. 282 tests, build, 141 templates, 121 pages, Playwright 7/7 and 1,668 SecurityRegression checks pass; live authenticated API remains an explicit external gate. Evidence: frontend-redesign/acceptance/contacts-pilot-2026-11-25/.
# FRD-G Supplier Record slice 6 completed (2026-11-25): localized PageHeader context and two DetailSections expose status/product count while preserving profile/commercial and Suppliers.Edit; no nonexistent supplier code was invented. Recovery and focused coverage pass. 282 tests, build, 141 templates, 121 pages and Playwright 7/7 pass.
# FRD-G Customer Record slice 5 completed (2026-11-25): localized PageHeader context and four DetailSections expose status/type/code while preserving profile/commercial/contacts/addresses and Customers.Edit; recoverable retry and focused coverage pass. 280 tests, build, 141 templates, 121 pages and Playwright 7/7 pass.
# FRD-G Supplier Form slice 4 completed (2026-11-25): shared FormSection/FieldCue/WorkflowFooter and localized Contacts context preserve payable-account/product name selection with internal IDs, active state, row version, dirty guard and recovery. 278 tests, build, 140 templates, 121 pages and Playwright 7/7 pass.
# FRD-G Customer Form slice 3 completed (2026-11-25): shared FormSection/FieldCue/FeedbackState/WorkflowFooter and localized Contacts context preserve all fields, option sources, generated code, dirty/concurrency recovery and commands. 277 tests, build, 140 templates, 121 pages and Playwright 7/7 pass.
# FRD-G Supplier List slice 2 completed (2026-11-25): shared FilterBar/SummaryStrip/typed DataTable, localized Contacts context, filtered-empty/reset and compact accessible actions preserve inactive filtering, paging, preferences and permissions. 276 tests, build, 140 templates, 121 pages and Playwright 7/7 pass.
# FRD-G Customer List slice 1 completed (2026-11-25): shared FilterBar/SummaryStrip/typed DataTable, localized Contacts context, filtered-empty/reset and compact accessible actions preserve server sort, saved preferences and permissions. 275 tests, build, 140 templates, 121 pages and Playwright 7/7 pass.
# FRD-G Contacts audit completed (2026-11-25): six Customer/Supplier List/Form/Record surfaces and their API/permission/dirty/concurrency invariants are scoped. Customer List is the first bounded slice; no runtime behavior changed.
# FRD-F completed (2026-11-25): Products List/Form/Record is the first migrated V4 module with documented search/barcode/CRUD/permission parity. 274 tests, build, 140 templates, 121 pages, Playwright 7/7 and 1,668 security checks pass; live authenticated API checks remain explicitly external.
# FRD-F Product Form slice 2 completed (2026-11-25): bilingual section descriptions and field-addressable cross-field validation replace generic failures for weight, dimensions, tracking and expense account rules. 274 tests, build, 140 templates, 121 pages and Playwright 7/7 pass.
# FRD-F Product Record slice 1 completed (2026-11-25): localized record context, a two-column detail hierarchy, and an explained compact delete zone preserve Edit/Attributes/Delete permissions and behavior. 273 tests, build, 140 templates, 121 pages and Playwright 7/7 pass.
# FRD-F Product Form slice 1 completed (2026-11-25): identity and commercial data are separate sections, duplicate top Cancel is removed, and the localized workflow footer remains authoritative. 272 tests, build, static gates and Playwright 7/7 pass.
# FRD-F Products List slice 3 completed (2026-11-25): stable column widths and compact accessible row actions preserve View/Edit/Attributes and permission behavior. 271 tests, build, 140 templates, 121 pages and Playwright 7/7 pass. Product Form is next.
# FRD-F Products List slice 2 completed (2026-11-25): true empty and filtered-empty states are distinct, and Clear filters restores the allowlisted PageContext defaults without changing server-side search. 271 tests, build, 140 templates, 121 pages and Playwright 7/7 pass.
# FRD-F Products List slice 1 completed (2026-11-25): six secondary catalog destinations are consolidated in one localized menu and Add Product remains the sole primary action. 271 tests, build, 140 templates, 121 pages and Playwright 7/7 pass.
# FRD-E completed (2026-11-25): branch, company, user and sign-out now share one contextual account menu while search remains directly available. The planned Application Shell layers are complete; 270 tests, build, 140 templates, 121 pages and Playwright 7/7 pass. Products pilot is next.
# FRD-E fifth slice completed (2026-11-25): the topbar now shows the real destination title and the breadcrumb lives with history/Open Work in one context toolbar. 269 tests, build, 140 templates, 121 pages and Playwright 7/7 pass.
# FRD-E fourth slice completed (2026-11-25): every registered destination now resolves App/Section/Archetype/Mobile priority metadata, mobile navigation consumes the priority contract, and registry coverage prevents unclassified destinations. 268 tests, build, 140 templates, 121 pages and Playwright 7/7 pass.
# FRD-E third slice completed (2026-11-24): current-app navigation is a focused component and Open Work styles are globally scoped under app-shell; app-shell.scss fell from 8.47 kB to 4.79 kB and its build warning is closed. 265 tests, build, 140 templates, 121 pages and Playwright 7/7 pass.
# FRD-E second slice completed (2026-11-24): a permission-aware App Switcher and current-app-only desktop/mobile navigation are active; 264 tests, build, 139-template accessibility, 121-page consistency and Playwright 7/7 pass. CSS budget extraction and Section/Archetype metadata remain.
# FRD-E first slice completed (2026-11-24): the ten permission-aware Apps now have centralized metadata and safe entry destinations consumed by Launcher and Shell; 262 tests, build, 138-template accessibility, 121-page consistency and Playwright 6/6 pass. Current-app navigation composition and shell CSS extraction remain.
# FRD-D completed (2026-11-24): 260 tests, build, static gates and Playwright 6/6 verify RTL/LTR, theme/density, keyboard and desktop/mobile; FRD-E Application Shell is active.
# FRD-D first slice completed (2026-11-24): Plum/Teal semantic tokens, localized List/Record/state previews and focused tests are in place; 259 tests, build, 138-template accessibility and UI consistency pass. Visual acceptance and reusable primitive extraction remain.
# UX-V4-001 started (2026-11-22): first shell/app-launcher slice passed 258 Angular tests, production build,
# 138-template accessibility and UI/UX consistency. Contextual work/history controls are complete; visual acceptance remains.
# ERP-003 completed (2026-11-16): granular branch permissions, tenant-owned user grants and verified optional request context are implemented. ERP-004 switcher/workspace is proposed. ERP plan: 10 total, 3 complete, 7 remaining.
# ERP-002 completed (2026-11-15): branch context policy and read-only workspace are implemented. ERP-003 user branch grants and server request context are proposed; no branch switcher is authorized yet.
# UX-017D completed (2026-11-15): close readiness and controlled fee/interest/transfer adjustments are implemented; no further UX-017 slice is pending.
# ERP-001 implemented (2026-11-15): enterprise gap audit plus named active-Asset selection for bank setup.
> Source of truth: Current repository scan  
> Last reviewed: 2026-11-07

The approved numbered Arabic/English delivery sequence is maintained in `MASTER_IMPLEMENTATION_ROADMAP_AR_EN.md`. Detailed working status, business rules and the rolling suggestion queue remain in `ARABIC_SHARED_ROADMAP.md`. This file remains the concise technical backlog.

The PUR-000 through PUR-280 purchase evolution is implemented. The approved completion sequence is maintained in `PURCHASE_COMPLETION_ROADMAP_AR_EN.md`. Incoming customer-management and sales requirements must be reconciled with master-roadmap steps 7 through 11 before implementation.

The ERP SaaS Foundation Blueprint is imported as a non-authoritative planning reference. `FND-000` reconciles it with current code; later `FND` slices are not implemented until explicitly approved and evidenced.

- [x] UX-001 audit the Angular UI and establish the continuously maintained modernization roadmap, grouped
  permission-aware navigation, bilingual breadcrumbs and safe navigation preference persistence. Completed 2026-11-04.
- [x] UX-002 implement a typed page-context contract for filters, paging, selected tabs and approved drafts, then pilot
  Customers, Suppliers, Products, Purchase Requests and Stock Transfers. Completed 2026-11-05.
- [x] UX-003 standardize page headers, filters, action zones, summary strips and workflow footers on Products,
  Purchase Requests and Stock Transfers. Completed 2026-11-05.
- [x] UX-004 add typed enterprise table infrastructure and pilot it on Products, Purchase Requests and Stock Transfers
  without moving paging, filtering or sorting from the server. Completed 2026-11-05.
- [x] UX-005 standardize form sections, localized required/optional cues and detail sections on Product, Purchase Request
  and Stock Transfer create/detail pilots without changing validation or workflow authority. Completed 2026-11-05.
- [x] UX-006 add display-only workflow progress and bilingual next-step guidance to Stock Transfers, Purchase Requests,
  Purchase Orders and Sales Orders while preserving server transition authority. Completed 2026-11-05.
- [x] UX-007 activate permission-filtered shell page/command search, Ctrl+K and `/` shortcuts, and safe memory-only recent
  destinations through the existing Router/workspace authority. Completed 2026-11-05.
- [x] UX-008 complete module-wide consistency rollout, executable exception inventory and final validation. Completed
  2026-11-05 with all 113 pages accounted for (102 shared headers + 11 named exceptions), 232/232 Angular tests,
  1,843 security/inventory checks, clean isolated Release build and 5/5 hosted browser checks.

- [x] MOD-000 complete the read-only MVC-to-API/Angular repository audit, target architecture, dependency graph, workspace design, MVP, feature matrix and risk assessment. Completed 2026-10-24.
- [x] MOD-001 create the isolated modernization branch and Angular 22 foundation in `MiniStore Front-End`, with strict standalone architecture, lazy routing, RTL/LTR shell, design tokens, restricted dev proxy, Signals workspace boundary, documentation, rollback and verified frontend/backend builds. Completed 2026-10-24.
- [x] MOD-002 add the versioned internal API foundation: same-origin session/CSRF contracts, permission/tenant enforcement, standardized ProblemDetails, authenticated internal-only OpenAPI, bounded paging conventions, Angular clients and focused security/live HTTP checks. Completed 2026-10-24.
- [x] MOD-003 establish the reusable visual-system foundation: semantic tokens, system/light/dark themes, comfortable/compact density, persisted RTL/LTR preferences, focus/motion/forced-color rules and a bilingual state preview. Completed 2026-10-24.
- [x] MOD-004 build the reusable workspace engine: permission-aware sidebar metadata, tab identity/deduplication, per-tab history, close/close-others, refresh restoration and dirty-state navigation guard. Completed 2026-10-24 with 12 Angular tests and unchanged backend behavior.
- [x] MOD-005 deliver the first Customers Angular vertical slice with explicit API DTOs, server-side paging/search, validation, concurrency and MVC parity. Completed 2026-10-24 with 17 Angular tests and 1076 backend checks.
- [x] MOD-006 extract only proven shared form/table primitives and validate them against a second real feature consumer. Completed 2026-10-24 with Customers/Suppliers, 22 Angular tests and 1079 backend checks.
- [x] MOD-007 complete Suppliers as the next Angular migration slice: details/create/edit, validation, concurrency and MVC parity. Completed 2026-10-24 with 26 Angular tests and 1081 backend checks.
- [x] MOD-008 add a permission-aware Command Center foundation with reusable quick preview, favorites and bounded user snapshots. Completed 2026-10-24 with 30 Angular tests.
- [x] MOD-009 add reusable smart-form productivity: explicit draft policy, saved non-sensitive filters and keyboard-first entry without automatic posting. Completed 2026-10-24 with 36 Angular tests.
- [x] MOD-010 complete responsive dashboard, density, focus and layout preferences with safe reset and accessibility evidence. Completed 2026-10-24 with visual review and 39 Angular tests.
- [x] MOD-011 add reusable duplicate/conflict/data-quality and recovery presentation over authoritative backend evidence. Completed 2026-10-24 with stable API codes, typed duplicate outcomes, reusable Customer/Supplier recovery, 52 Angular tests and 1084 backend checks.
- [x] MOD-012 add reusable document journey, related-record evidence and Explain This Number presentation over authoritative backend evidence. Completed 2026-10-24 on Sales Orders and Customer Invoices without database changes.
- [x] MOD-013 add a permission-aware action/exception center and contextual help over existing server decisions. Completed 2026-10-24.
- [x] MOD-014 add allowlisted Focused/Operator/Analyst workspace profiles and a responsive split context panel without changing permissions. Completed 2026-10-24.
- [x] MOD-015 expose the reusable Analytics Center through Angular with server grouping/comparison, allowlisted drill-down and bounded export preview. Completed 2026-10-24.
- [x] MOD-016 add permission-filtered runtime monitoring for real hosted workers, SLA state and truthful analytics refresh plans without pretending report delivery exists. Completed 2026-10-24.
- [x] MOD-017 governed Angular workspace builder/save, backend-authored financial preview and tenant-safe user/role delegation with optional start/expiry, revocation evidence and source-permission rechecks. Completed 2026-10-25.
- [x] MOD-018A reusable acceptance gate: Release/regression, Angular tests/build, external SCSS, explicit initial/lazy budgets, static accessibility and real Edge RTL/LTR/keyboard/state/mobile checks with one passed/failed/skipped report. Completed 2026-10-25.
- [x] MOD-018B prove the real authenticated session and three permission-allowed read endpoints with an external short-lived storage state; the final gate passed 11/11 with zero skips and deleted the temporary state. Completed 2026-10-25.
- [x] MOD-019 generate the complete 89-controller/189-view/344-MVC-action/198-Razor-file registry and add reversible, GET-only, allowlisted per-surface cutover controls with MVC committed as the default. Completed 2026-10-25.
- [x] MOD-020 complete all current Settings/Document Design, account/session, tenant-user/role, public registration and
  guided onboarding operations in Angular. Identity, tenant membership, provisioning, lockout/audit, CSRF and HttpOnly
  cookies remain server-owned; MVC remains the default until a later cutover gate. Completed 2026-10-27.
- [x] MOD-021 complete Angular product master, categories, attributes/values, templates/variants, recipes,
  tax/payment/branch masters and warehouse CRUD with explicit DTOs, separate permissions, CSRF and tests. Completed
  `MOD-021A..G` on 2026-10-27; MVC remains default pending cutover.
- [x] MOD-022A migrate the read-only inventory transaction ledger with server paging/search/type/direction/date filters,
  sorting, cost/value evidence, permission-aware navigation, bilingual RTL UI and full verification. Completed 2026-10-27.
- [x] MOD-022B migrate all current ProductStock operations and reason-required manual stock adjustments with explicit API
  DTOs, permissions, CSRF, retained server rules and bilingual Angular UI. Completed 2026-10-27.
- [x] MOD-022C migrate dimensional availability and reservation history with Application-owned paging, independent permissions,
  named dimensions, bilingual read-only Angular routes and full verification. Completed 2026-10-27.
- [x] MOD-022D migrate the complete inventory-adjustment lifecycle: create, count, approve, post and cancel with independent
  permissions, CSRF, completed-action states and server-owned dimensional posting. Completed 2026-10-27.
- [x] MOD-022E migrate all current lot/serial visibility, opening allocation, expiration, quarantine/release, recall lifecycle and
  manual recall communication operations with independent permissions and server-owned rules. Completed 2026-10-27.
- [x] MOD-022F migrate warehouse locations, Unassigned Stock putaway/rules, relocation and Location/Stock Movement history with all current operations and server-owned invariants. Completed 2026-10-27.
- [x] MOD-023A migrate Purchase Request list/create/details/submit/cancel with named dynamic lines, concurrency, independent permissions and server-owned rules. Completed 2026-10-27.
- [x] MOD-023B migrate approval-rule list/create/activation and Purchase Request approve/reject decisions with frozen ordered roles and independent permissions. Completed 2026-10-27.
- [x] MOD-023C migrate sourcing list/details/create, invited-supplier quotation capture/comparison and reason-required exact-quantity award with independent permissions and server-owned commercial rules. Completed 2026-10-28.
- [x] MOD-023D migrate Purchase Order list/details/create-from-award and approve/confirm/cancel/close/reopen/direct-delivery/supplier-confirmation operations with independent permissions and server-owned lifecycle rules. Completed 2026-10-28.
- [x] MOD-023E migrate Goods Receipt list/details/direct create, scanning draft/post, staged inspection and put-away with independent permissions and server-owned quantity/tracking/inventory/accounting rules. Completed 2026-10-28.
- [x] MOD-023F migrate Vendor Bill list/details/create/approve/override/post/cancel plus bounded attachments and reviewed extraction with server-owned allocation, matching, tax and accounting. Completed 2026-10-28.
- [x] MOD-023G migrate Supplier Payment list/details/create-and-post with named methods, bounded multi-bill allocation and server-owned balances, exchange differences and journals. Completed 2026-10-28.
- [x] MOD-023H migrate Goods Receipt Return list/details/create-and-post with explicit tracking allocations and server-owned unbilled limits, availability, valuation, movements and journals. Completed 2026-10-28.
- [x] MOD-023I migrate Supplier Credit Note list/details/create-and-post with explicit tracking allocations and server-owned bill limits, stock availability, tax, valuation, payable and journals. Completed 2026-10-28.
- [x] MOD-023J migrate controlled Purchase Document Reversal list/details/create for receipts, bills, payments and landed costs with server-owned dependencies, valuation, corrections, opposite journals and idempotency. Completed 2026-10-28.
- [x] MOD-023K migrate Landed Cost list/details/create-and-post with receipt/reversal links and server-owned allocation, capitalization/expense split, AVCO, movements and journals. Completed 2026-10-28.
- [x] MOD-023L migrate Purchase Service Acceptance list/details/create with eligible-order link, remaining service quantities, immutable evidence and no inventory effect. Completed 2026-10-28.
- [x] MOD-023M migrate Purchase Agreements and repeat Purchase Templates with full lifecycle/release/generation operations and server-owned award, concurrency and quantity rules. Completed 2026-10-28.
- [x] MOD-023N migrate supplier purchasing terms, bulk save, price breaks/history/exceptions and bounded CSV/XLSX import/export. Completed 2026-10-28.
- [x] MOD-023O migrate automated replenishment settings, manual execution and run/suggestion history. Completed 2026-10-28.
- [x] MOD-023P migrate purchasing analytics filters, KPIs, trends, supplier/buyer performance, matching exceptions and acceptance journey. Completed 2026-10-28.
- [x] MOD-023Q migrate Purchase Accounting Reconciliation as a read-only Angular evidence surface with server-owned reconciliation calculations. Completed 2026-10-28.
- [x] MOD-023R migrate supplier communication preparation, attachments and recorded activity evidence with independent permissions. Completed 2026-10-28.
- [x] MOD-023S migrate supplier credentials, portal access and external submission queue/retry/download operations. Completed 2026-10-28.
- [x] MOD-023T migrate the token-scoped supplier portal quotation, order confirmation and delivery notice operations. Completed 2026-10-28.
- [x] MOD-023U migrate the remaining Legacy Direct Purchase history/details/post evidence and posted Purchase Return list/details/create-and-post operations; keep new legacy purchase creation retired. Completed 2026-10-29.
- [x] MOD-024A migrate Sales Price Lists, pricing rules, lifecycle operations and server-authored pricing evaluation as the first sales slice. Completed 2026-10-29.
- [x] MOD-024B migrate Sales Quotation list/details/create, lifecycle transitions and PDF preview/download with server-owned pricing and snapshots. Completed 2026-10-29.
- [x] MOD-024C migrate Sales Order list/details, accepted-quotation conversion, confirmation/cancellation and control approvals with server-owned source snapshots. Completed 2026-10-29.
- [x] MOD-024D migrate Sales Order inventory reservation page, reserve and release operations with named warehouses/locations and server-owned remaining quantities. Completed 2026-10-29.
- [x] MOD-024E migrate Sales Delivery list/details/create, posting and cancellation with server-owned reservation, tracking, valuation, stock and COGS authority. Completed 2026-10-29.
- [x] MOD-024F migrate Customer Invoice list/details/create, approval/cancellation, posting and reversal with server-owned cumulative eligibility, frozen commercial evidence and accounting. Completed 2026-10-29.
- [x] MOD-024G migrate Customer Payment list/details/create, allocation, posting and reversal with server-owned balances, FX and accounting. Completed 2026-10-29.
- [x] MOD-024H migrate Customer Return Request list/details/create and authorization lifecycle with server-owned returnable quantities. Completed 2026-10-29.
- [x] MOD-024I migrate Customer Return Receipt list/details/create, inspection, cancellation and disposition posting with server-owned quantities, tracking, inventory and accounting. Completed 2026-10-29.
- [x] MOD-024J migrate Customer Credit Note list/details/create-and-post with server-owned eligibility, cumulative credits and accounting. Completed 2026-10-29.
- [x] MOD-024K migrate Sales Approval Rule list/create/activation management with server-owned scope, threshold and ordered-step validation. Completed 2026-10-29.
- [x] MOD-024L migrate Sales Quotation Template list/details/create, version cloning and lifecycle management with server-owned template validation. Completed 2026-10-29.
- [x] MOD-025A migrate legacy immediate-sale history/details, wholesale creation and one-time posting while preserving server-owned pricing, stock, tracking and accounting. Completed 2026-10-29.
- [x] MOD-025B migrate the retail POS composer with terminal warehouse access, experience settings, order context and server-owned sale execution. Completed 2026-10-29.
- [x] MOD-025C migrate immediate-sale return list/details/create-and-post with server-owned remaining quantities, tracking, stock and accounting. Completed 2026-10-29.
- [x] MOD-026A migrate the chart-of-accounts list/create surface through an Application-owned API with server-owned hierarchy and validation. Completed 2026-10-29.
- [x] MOD-026B migrate Journal Entry details with named accounts, branch/warehouse dimensions and immutable source evidence. Completed 2026-10-29.
- [x] MOD-027A migrate tenant subscription summary and checkout with server-owned plan, promotion and payment-state validation. Completed 2026-10-29.
- [x] MOD-027B migrate the public landing and pricing surfaces with server-authored bilingual plan cards and registration destinations. Completed 2026-10-29.
- [x] MOD-027C migrate Platform sign-in/session and the read-only control-center overview with its separate operator boundary. Completed 2026-10-30.
- [x] MOD-027D migrate Platform companies, plans, promotions and pending-payment operations with role-specific policies. Completed 2026-10-30.
- [x] MOD-027E migrate Platform audit-event reading/export and retention/legal-hold operations. Completed 2026-10-30.
- [x] MOD-028A reconcile the generated migration registry with every actual shared and remaining MVC surface, classify parity and approve the first bounded implementation slice. Completed 2026-10-30.
- [x] MOD-028B migrate Stock Transfer list/details/create/edit and its full submit/approve/reject/return/post/cancel lifecycle through an Application-owned API. Completed 2026-10-30.
- [x] MOD-028C migrate Inventory Scanning resolution, putaway, relocation and absolute draft-count operations with independent permissions. Completed 2026-10-30.
- [x] MOD-028D migrate Inventory Insights and Inventory Reconciliation as independent read-only Angular evidence surfaces. Completed 2026-10-30.
- [x] MOD-028E migrate tenant Audit Event reading, sensitive-field policy and purpose-bound bounded CSV export. Completed 2026-10-30.
- [x] MOD-028F close Purchase Sourcing attachment upload/download and printable RFQ parity. Completed 2026-10-30.
- [x] MOD-028G synchronize Angular locale with server culture and complete the final registry reconciliation. Completed 2026-10-30.
- [x] MOD-023 through MOD-028 migrate purchasing, sales, POS, accounting, tenant/public,
  platform and shared MVC workflows to Angular, including every operation, permission, localization, file/PDF and edge state.
- [ ] MOD-029 prove full-system parity through dual-run reconciliation, performance/security/accessibility evidence and cutover/rollback rehearsals.
  - [x] MOD-029A1 generate and gate an 82-counterpart static acceptance matrix; correct compact-action/API classification. Completed 2026-10-30.
  - [x] MOD-029A2 run public/high-risk boundary browser/API samples, bounded performance and route-level rollback rehearsal. Completed 2026-10-30; authenticated data evidence moves to MOD-029B.
  - [x] MOD-029B reconcile authenticated same-data identity, financial and inventory reads with an operator-supplied Playwright session. Completed 2026-10-30.
    - [x] Add a secret-safe runner, git-ignore enforcement and identity/financial/inventory value comparisons. Completed 2026-10-30.
    - [x] Capture an operator-authenticated state and execute the runner against Demo: 9/9 authenticated checks, zero skips; financial empty-state and real inventory same-data evidence passed. Completed 2026-10-30.
  - [x] MOD-029C run permission-driven read-only acceptance across the high-risk sales, purchasing, inventory, accounting and administration cohorts. Completed 2026-10-30 with 15/15 browser checks and zero skips.
  - [x] MOD-029D rehearse live process-level AngularDefault cutover and MVC rollback for allowlisted GET routes without committing a default change. Completed 2026-10-30 on 10 authorized routes; applied additive migration 136 after the rehearsal exposed Demo drift.
- [ ] MOD-030 make Angular the only presentation and retire MVC incrementally only after explicit approval and zero registry gaps. Reversible cutover is complete; physical MVC retirement remains approval-gated.
  - [x] MOD-030A host the production Angular build under `/app`, prove deep-link/assets/API separation and cut over Customers as the first reversible route cohort. Completed 2026-10-30 with 15/15 hosted checks and a 10-route mixed pilot.
  - [x] MOD-030B cut over Suppliers and Products as the second reversible cohort. Completed 2026-10-31 with 15/15 hosted checks, a 12-route mixed pilot and a 12-route MVC rollback.
  - [x] MOD-030C cut over Analytics Center and prepare GET-only Settings mappings. Completed 2026-10-31 with 15/15 hosted checks, a 12-route mixed pilot and a 12-route MVC rollback; Settings stayed MVC because the captured operator lacked `Administration.Access`.
  - [x] MOD-030D cut over Settings with an `Administration.Access` operator. Completed 2026-10-31 with 15/15 hosted checks, 29-route Angular cutover and 29-route MVC rollback; Release clean, 1,385 backend checks, 214/214 Angular tests, 121 templates and bundle budgets passed.
  - [x] MOD-030E complete runtime AngularDefault coverage for the 77 mapped surfaces that remain MVC by default; do not delete MVC during this phase. Completed 2026-11-03 with all 81 presentation surfaces AngularDefault, zero presentation gaps and Language explicitly retained as the sole POST-only compatibility boundary.
    - [x] MOD-030E1 cut over InventoryBalances, InventoryReservations, InventoryInsights and InventoryReconciliation. Completed 2026-10-31 with hosted 15/15, 33-route cutover/rollback, 1,394 backend checks and 214/214 Angular tests.
    - [x] MOD-030E2A cut over StockTransactions and ProductStocks GET presentation while all mutations remain independently protected. Completed 2026-10-31 with hosted 15/15, Angular/API stock reconciliation, 38-route cutover/rollback, 1,406 backend checks and 215/215 Angular tests.
    - [x] MOD-030E2B cut over InventoryAdjustments list/create/details GET presentation while lifecycle mutations remain independently protected. Completed 2026-11-01 with hosted 15/15, 40-route cutover/rollback, 1,413 backend checks and 215/215 Angular tests.
    - [x] MOD-030E2C cut over StockTransfers list/create/edit/details GET presentation while lifecycle mutations remain independently protected. Completed 2026-11-01 with hosted 15/15, 42-route cutover/rollback, 1,423 backend checks and 215/215 Angular tests; Demo had no transfer fixture, so no data was created for live Details/Edit probes.
    - [x] MOD-030E2D cut over the InventoryTracking Index GET presentation while open/quarantine/release/recall mutations remain independently protected. Completed 2026-11-01 with hosted 15/15, 43-route cutover/rollback, 1,427 backend checks and 215/215 Angular tests.
    - [x] MOD-030E2E cut over the read-only StockMovements Index GET presentation with exact query preservation and MVC rollback. Completed 2026-11-01 with hosted 15/15, 44-route cutover/rollback, 1,430 backend checks and 215/215 Angular tests.
    - [x] MOD-030E2F cut over the bounded warehouse-operations cohort: WarehouseLocations, UnassignedStock, LocationMovements and InventoryScanning Index GET presentation while all writes and unsupported WarehouseLocations Create/Edit deep links remain on MVC. Completed 2026-11-01 with hosted 15/15, 48-route cutover/rollback, 1,440 backend checks and 215/215 Angular tests.
    - [x] MOD-030E2G build the dedicated Inventory Replenishment API/Angular counterpart and cut over Index GET while keeping rule, transfer and purchase-demand permissions independent. Completed 2026-11-01 with hosted 15/15, 49-route cutover/rollback, 1,447 backend checks and 218/218 Angular tests.
    - [x] MOD-030E3A cut over the shared Branches, PaymentMethods and TaxRates Index GET surfaces to `/products/catalog-masters`; keep create/update/delete commands independently authorized and outside the presentation catalog. Completed 2026-11-01 with hosted 15/15, 52-route cutover/rollback and 1,455 backend checks.
    - [x] MOD-030E3B cut over ProductCategories Index and ProductAttributes Index/positive-product Values GET presentation to their exact Angular routes; keep create/status/value-save commands independently authorized and outside the presentation catalog. Completed 2026-11-01 with hosted 15/15, 55-route cutover/rollback and 1,465 backend checks.
    - [x] MOD-030E3C cut over Warehouses Index GET to `/warehouses`; keep unsupported Create/Edit deep links and every mutation on MVC/API until Angular consumes explicit URL intent. Completed 2026-11-01 with hosted 15/15, 56-route cutover/rollback and 1,469 backend checks.
    - [x] MOD-030E3D cut over AuditEvents Index GET to `/audit-events`; keep export POST independently authorized and outside presentation cutover. Completed 2026-11-01 with hosted 15/15, 57-route cutover/rollback and 1,473 backend checks.
    - [x] MOD-030E3E cut over Users Index/Create and Roles Index/Create/Edit GET presentation to exact Angular routes; keep identity/role mutations independently authorized and outside presentation cutover. Completed 2026-11-01 with hosted 15/15, 62-route cutover/rollback and 1,485 backend checks.
    - [x] MOD-030E3F cut over ProductTemplates Index and Recipes Index/positive-product Edit GET presentation to exact Angular routes; keep template/recipe mutations outside presentation cutover. Completed 2026-11-01 with hosted 15/15, 65-route cutover/rollback and 1,493 backend checks.
    - [x] MOD-030E4A cut over the read-only PurchaseAnalytics Index GET presentation to `/purchases/analytics`; preserve filters and keep every purchasing mutation outside presentation cutover. Completed 2026-11-01 with hosted 15/15, 66-route cutover/rollback and 1,496 backend checks.
    - [x] MOD-030E4B cut over the read-only PurchaseAccountingReconciliation Index GET presentation to `/purchases/accounting-reconciliation`; preserve as-of/exception filters and keep all writes outside presentation cutover. Completed 2026-11-01 with hosted 15/15, 67-route cutover/rollback and 1,499 backend checks.
    - [x] MOD-030E4C cut over PurchaseReplenishment Index GET presentation to `/purchases/replenishment`; keep settings-save and manual-run mutations independently authorized and outside presentation cutover. Completed 2026-11-01 with hosted 15/15, 68-route cutover/rollback and 1,502 backend checks.
    - [x] MOD-030E4D cut over PurchaseApprovalRules Index GET presentation to `/purchases/approval-rules`; keep create/status mutations independently authorized and outside presentation cutover. Completed 2026-11-01 with hosted 15/15, 69-route cutover/rollback and 1,505 backend checks.
    - [x] MOD-030E4E cut over PurchaseTemplates Index GET presentation to `/purchases/templates`; keep create/generate/status mutations independently authorized and outside presentation cutover. Completed 2026-11-01 with hosted 15/15, 70-route cutover/rollback and 1,508 backend checks.
    - [x] MOD-030E4F cut over PurchaseRequests Index/Create/positive-ID Details GET presentation to exact Angular routes; keep lifecycle/approval mutations independently authorized and outside presentation cutover. Completed 2026-11-01 with hosted 15/15, 73-route cutover/rollback and 1,516 backend checks without creating data.
    - [x] MOD-030E4G cut over PurchaseSourcing Index and positive-ID Details/Print GET presentation to exact Angular routes; keep create/direct forms, attachments and all mutations outside presentation cutover. Completed 2026-11-02 with hosted 15/15, 76-route cutover/rollback and 1,524 backend checks without creating data.
    - [x] MOD-030E4H cut over SupplierQuotations Create/Comparison GET presentation for positive sourcing-event IDs to exact Angular routes; keep Create/Award POST and every mutation outside presentation cutover. Completed 2026-11-02 with hosted 15/15, 78-route cutover/rollback and 1,531 backend checks without creating data.
    - [x] MOD-030E4I cut over PurchaseOrders Index and positive-ID Details GET presentation to exact Angular routes; keep CreateFromAward and every lifecycle/fulfillment mutation outside presentation cutover. Completed 2026-11-02 with hosted 15/15, 80-route cutover/rollback and 1,537 backend checks without creating data.
    - [x] MOD-030E4J cut over GoodsReceipts Index, positive-purchase-order Create and positive-ID Details/Scan GET presentation to exact Angular routes; keep all receipt, scan, inspection, posting and reversal mutations outside presentation cutover. Completed 2026-11-02 with hosted 15/15, 84-route cutover/rollback and 1,549 backend checks without creating data.
    - [x] MOD-030E4K cut over GoodsReceiptReturns Index, positive-receipt Create and positive-ID Details GET presentation to exact Angular routes; keep Create POST and every return mutation outside presentation cutover. Completed 2026-11-02 with hosted 15/15, 86-route cutover/rollback and 1,558 backend checks without creating data; no saved return existed for a live Details probe.
    - [x] MOD-030E4L cut over VendorBills Index, positive-order Create and positive-ID Details GET presentation to exact Angular routes; keep attachments and all bill lifecycle/accounting mutations outside presentation cutover. Completed 2026-11-02 with hosted 15/15, 89-route cutover/rollback and 1,568 backend checks without creating data.
    - [x] MOD-030E4M cut over SupplierCreditNotes Index, positive-bill Create and positive-ID Details GET presentation to exact Angular routes; keep Create POST outside cutover. Completed 2026-11-02 with hosted 15/15, 91-route cutover/rollback and 1,577 backend checks; no saved credit note existed for live Details.
    - [x] MOD-030E4N cut over PurchaseDocumentReversals Index and positive-ID Details GET presentation; keep compound-query Create and reversal mutation outside cutover. Completed 2026-11-02 with hosted 15/15, 92-route cutover/rollback and 1,583 backend checks; no saved reversal existed for live Details.
    - [x] MOD-030E4O cut over LandedCosts Index, positive-receipt Create and positive-ID Details GET presentation; keep Create POST outside cutover. Completed 2026-11-02 with hosted 15/15, 95-route cutover/rollback and 1,592 backend checks without creating data.
    - [x] MOD-030E4P cut over PurchaseServiceAcceptances Index, positive-order Create and positive-ID Details GET presentation; keep Create POST outside cutover. Completed 2026-11-02 with hosted 15/15, 97-route cutover/rollback and 1,601 backend checks without creating data; no saved acceptance existed for live Details.
    - [x] MOD-030E4Q cut over PurchaseAgreements Index, positive-sourcing-event Create and positive-ID Details GET presentation; keep lifecycle and release POST actions outside cutover. Completed 2026-11-02 with hosted 15/15, 99-route cutover/rollback and 1,610 backend checks; eligible sourcing data covered Create and no saved agreement existed for live Details.
    - [x] MOD-030E4R cut over SupplierPurchasing Index, PriceExceptions and positive-ID PriceBreaks/PriceHistory GET presentation; keep exports, imports and mutations outside cutover. Completed 2026-11-02 with hosted 15/15, 101-route cutover/rollback and 1,622 backend checks; no purchasing-info row existed for live price-route probes.
    - [x] MOD-030E4S cut over SupplierCommunications positive-ID Details GET presentation; keep Prepare/RecordActivity POST and Attachment file download outside cutover. Completed 2026-11-02 with hosted 15/15, 101-route cutover/rollback and 1,626 backend checks; no saved communication existed for a live Details probe.
    - [x] MOD-030E4T cut over SupplierIntegrations Index and positive-ID Submission GET presentation; keep UBL/submission downloads and every mutation outside cutover. Completed 2026-11-02 with hosted 15/15, 102-route cutover/rollback and 1,632 backend checks; no saved submission existed for a live Details probe.
    - [x] MOD-030E4U cut over token-scoped public SupplierPortal Index GET presentation; preserve rate limiting, private response headers and denied-access audit, and keep all POST submissions outside cutover. Completed 2026-11-02 with hosted 15/15, 102-route cutover/rollback, explicit redirect-header evidence and 1,637 backend checks; no portal access or submission was created.
    - [x] MOD-030E4V cut over legacy direct-purchase and purchase-return GET presentation; keep retired purchase creation, posting and return creation mutations outside cutover. Completed 2026-11-02 with hosted 15/15, 104-route cutover/rollback and 1,650 backend checks; no legacy purchase/return sample existed for live detail/form probes and no document was created.
    - [x] MOD-030E4W cut over SalesPriceLists Index GET presentation; preserve the selected-list query and keep create/status/rule/evaluation POST actions outside cutover. Completed 2026-11-02 with hosted 15/15, 104 authorized-route cutover/rollback and 1,654 backend checks; the current session lacked SalesPricing.View for a live route probe and no pricing data changed.
    - [x] MOD-030E4X cut over SalesQuotations Index, query-bound Create and positive-ID Details GET presentation; keep PDF download and lifecycle mutations outside cutover. Completed 2026-11-02 with hosted 15/15, 104 authorized-route cutover/rollback and 1,663 backend checks; the current session lacked quotation permissions for live route probes and no quotation changed.
    - [x] MOD-030E4Y cut over SalesOrders Index, positive-ID Details and Reserve GET presentation; keep conversion, lifecycle, approvals and reservation mutations outside cutover. Completed 2026-11-02 with hosted 15/15, 104 authorized-route cutover/rollback and 1,671 backend checks; the current session lacked SalesOrders.View for live route probes and no order/reservation changed.
    - [x] MOD-030E4Z cut over SalesDeliveries Index, positive-order Create and positive-ID Details GET presentation; keep Create/Post/Cancel mutations outside cutover. Completed 2026-11-02 with hosted 15/15, 104 authorized-route cutover/rollback and 1,680 backend checks; the current session lacked delivery permissions for live route probes and no delivery changed.
    - [x] MOD-030E5A cut over CustomerInvoices Index, positive-order Create and positive-ID Details GET presentation; keep Create/Approve/Cancel/Post/Reverse mutations outside cutover. Completed 2026-11-02 with hosted 15/15, 104 authorized-route cutover/rollback and 1,689 backend checks; the session lacked invoice permissions for live probes and no invoice/accounting changed.
    - [x] MOD-030E5B cut over CustomerPayments Index, positive-invoice Create and positive-ID Details GET presentation; keep Create/Reverse mutations outside cutover. Completed 2026-11-02 with hosted 15/15, 104 authorized-route cutover/rollback and 1,698 backend checks; no payment or journal changed.
    - [x] MOD-030E5C cut over CustomerReturnRequests Index, positive-delivery Create and positive-ID Details GET; keep lifecycle mutations outside cutover. Completed 2026-11-02 with hosted 15/15, cutover/rollback 104/104 and 1,707 backend checks; no return request changed.
    - [x] MOD-030E5D cut over CustomerReturnReceipts Index, positive-request Create and positive-ID Details GET; keep create/inspect/post/cancel outside cutover. Completed 2026-11-02 with hosted 15/15, cutover/rollback 104/104 and 1,716 backend checks; no stock/accounting mutation.
    - [x] MOD-030E5E cut over CustomerCreditNotes Index, positive-invoice Create and positive-ID Details GET; keep create-and-post outside cutover. Completed 2026-11-02 with hosted 15/15, cutover/MVC rollback 117/117 and 1,725 backend checks; no credit note, receivable or journal changed.
    - [x] MOD-030E5F cut over SalesApprovalRules Index GET; keep rule creation and activation changes outside cutover. Completed 2026-11-02 with hosted 15/15, cutover/MVC rollback 118/118 and 1,725 backend checks; no rule or order approval changed.
    - [x] MOD-030E5G cut over SalesQuotationTemplates Index, optional-source Create and positive-ID Details GET; keep create/lifecycle changes outside cutover. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 119/119 and 1,729 backend checks; no template changed.
    - [x] MOD-030E5H cut over legacy Sales Index/Create/Details and POS GET presentation; keep sale creation and GL posting outside cutover. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 123/123 and 1,738 backend checks; no sale, stock or journal changed.
    - [x] MOD-030E5I cut over legacy SalesReturns Index, positive-sale Create and positive-ID Details GET; keep create-and-post outside cutover. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 124/124 and 1,748 backend checks; no return, stock or journal changed.
    - [x] MOD-030E5J cut over Accounts Index GET; keep account creation outside cutover. Completed 2026-11-03 with hosted 15/15, rendered Angular/API financial reconciliation, cutover/MVC rollback 125/125 and 1,760 backend checks; no account or journal changed.
    - [x] MOD-030E5K cut over positive-ID JournalEntries Details GET; keep all journal posting/reversal authority outside cutover. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 125/125 and 1,763 backend checks; Demo had no payment-backed JournalEntryId for a live detail probe and no accounting data changed.
    - [x] MOD-030E5L cut over SupplierPayments Index, positive-bill Create and positive-ID Details GET; keep create-and-post outside cutover. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 128/128 and 1,773 backend checks; no payment or journal changed.
    - [x] MOD-030E5M cut over tenant Subscription Index GET; keep checkout POST and all pricing/activation authority outside cutover. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 129/129 and 1,776 backend checks; no checkout or subscription state changed.
    - [x] MOD-030E5N cut over anonymous Public landing and pricing GET presentation; keep registration and every SaaS mutation outside cutover. Completed 2026-11-03 with hosted 15/15, anonymous cutover/MVC rollback 131/131 and 1,782 backend checks; no registration, plan or subscription changed.
    - [x] MOD-030E5O cut over isolated PlatformAccount Login GET; keep credential submission, rate limiting, cookie issue and logout outside cutover. Completed 2026-11-03 with hosted 15/15, anonymous cutover/MVC rollback 132/132 and 1,786 backend checks; no credential attempt or Platform session mutation.
    - [x] MOD-030E5P cut over Platform overview and management GET presentation; keep company/plan/promotion/payment mutations outside cutover. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 132/132, live tenant-to-Platform isolation and 1,802 backend checks; no Platform session was available for an authorized live route probe and no company/plan/promotion/payment changed.
    - [x] MOD-030E5Q cut over PlatformAuditEvents Index GET; keep export, sensitive-field authority and audit semantics outside cutover. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 132/132, live tenant-to-audit isolation and 1,806 backend checks; no Platform session fixture for an authorized live probe and no export/audit mutation.
    - [x] MOD-030E5R cut over PlatformAuditRetention Index GET; keep holds, checkpoints and retention execution outside cutover. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 132/132, live tenant-to-owner isolation and 1,810 backend checks; no Platform session fixture for an authorized live probe and no hold/checkpoint/retention mutation.
    - [x] MOD-030E5S cut over Account Login/Register/AccessDenied GET presentation; keep authentication, registration and logout mutations outside cutover. Completed 2026-11-03 with hosted 15/15, anonymous cutover/MVC rollback 135/135 and 1,820 backend checks; no credential attempt, registration or session mutation.
    - [x] MOD-030E5T cut over CompanyOnboarding Index GET; keep complete/skip and starter-template provisioning outside cutover. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 136/136 and 1,824 backend checks; no completion, skip or setup-data mutation.
    - [x] MOD-030E5U cut over Dashboard Index GET; keep server-authored work guidance and every downstream mutation outside cutover. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 137/137 and 1,827 backend checks; no business mutation.
    - [x] MOD-030E5V cut over DocumentDesigns Index/Create/Details GET; keep create/activate/default/archive mutations outside cutover. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 139/139 and 1,836 backend checks; no saved design existed for a live Details probe and no design/lifecycle mutation occurred.
    - [x] MOD-030E5W cut over the authenticated Home compatibility redirect GET; retain its onboarding/Dashboard decision and MVC rollback. Completed 2026-11-03 with hosted 15/15, cutover/MVC rollback 140/140, warning-free Release and 1,840 backend checks; no onboarding state or business data changed.
    - [x] MOD-030E5X classify Language Set POST as a permanent mutation-only MVC compatibility boundary; do not add it to the GET/HEAD presentation catalog. Completed 2026-11-03 with 81 presentation counterparts, zero presentation gaps, one explicit mutation-only boundary, warning-free Release and 1,843 backend checks.
  - [x] MOD-030F inventory physical MVC retirement dependencies and define bounded reversible cohorts; completed 2026-11-12 with all 411 MVC actions and 217 Razor files classified, eight R1 surfaces and no deletion.
  - [x] MOD-030G retire a separately approved R1 pilot limited to InventoryBalances, InventoryReservations, InventoryInsights, InventoryReconciliation and StockMovements. Completed 2026-11-12 with five controllers/views removed, protected query-preserving legacy redirects and Edge acceptance across 145 routes.
  - [x] MOD-030H retire the separately approved financial R1 JournalEntries Details and PurchaseAccountingReconciliation Index surfaces. Completed 2026-11-12 with two controllers/views removed, positive-ID/query-preserving protected redirects and Edge acceptance across 146 routes.
  - [ ] MOD-030I review the remaining anonymous Public R1 surface as an independent entry/authentication boundary; no deletion without separate approval.

- [x] FND-000 import the ERP SaaS Foundation Blueprint with provenance and classify its ten capabilities against the actual MiniStore repository.
- [ ] FND-010 inventory production environments and operational ownership, approve RPO/RTO and backup retention, run an isolated restore drill, reconcile restored financial/inventory/tenant/outbox state and record evidence.
- [ ] FND-020 through FND-080 incrementally harden audit/security, integrity/idempotency, observability/performance, configuration/approval reuse, provisioning/billing, exception/work guidance and safe import/export after their individual approvals.
- [x] CTRL-000 complete the repository recovery audit and create the consolidated project-control status, inventory, audits, conflict register, dependency map, single roadmap, decision/debt/test registers and handoff context.
- [x] P0-SEC-001 close the historically published administrator credential on the owner-declared local Demo environment, invalidate sessions, prove old-secret rejection and verify a replacement administrator.
- [x] P0-OPS-002 create an AES-256 encrypted local Demo backup, verify and restore it in isolation, reconcile 147 tables plus business/accounting totals, and clean up only the restore database.
- [x] P0-DB-003 prove all 112 EF Core migrations match the local Demo database, no model change is pending and no disabled/untrusted constraint or tenant relationship drift exists.
- [x] P1-AUD-010 approve the audit-security design for correlation, outcomes, redaction, access, retention classes, staged tamper evidence, additive migration, rollback and tests without runtime changes.
- [x] P1-AUD-011 add and apply the AuditEvent schema, core Application writer, allowlist redactor and keyed integrity hash behind a disabled-by-default feature flag; legacy audit history remained unchanged.
- [x] P1-AUD-012 enabled first semantic audit coverage for tenant role/permission and six sensitive-setting groups with local Demo key material held only in User Secrets; SQL proves one valid event per successful operation and no success event on rejection/rollback.
- [x] P1-AUD-013 add authentication/lockout, actual denied privileged action, platform administration and external integration semantic events with an independent failure-visible security writer and masked request evidence.
- [x] P1-AUD-014 add permission-aware tenant/platform audit readers, deterministic pagination, sensitive-access controls and a bounded purpose-required audited export.
- [x] P1-AUD-015 add SQL writer/reader/maintainer roles and separate connection hooks, approved 7/3-year and 90-day retention, Owner-only legal holds, signed external checkpoints and manifest-before-delete execution.
- [x] P1-AUD-016 reconcile 37 critical controls, preserve 710 historical `AuditLogs` rows, freeze new generic writes behind a reversible independent flag and prove semantic continuity plus rollback in SQL.
- [x] P1-OBS-020 add the owner-approved local build/SQL gate, secret-free health checks, vendor-neutral metrics, testable log alerts and a measured p95/p99 availability workload.
- [x] P1-ARC-030 complete the first bounded architecture slice: WarehouseLocations now receives projected Application DTOs and no longer injects warehouse/location repositories; continue the pattern incrementally when other slices are touched.
- [x] P2-CUS-040A / CUS-005 preserve CustomerId/SupplierId, expand Customer first and defer BusinessPartner unification to a separately approved CUS-110 gate.
- [x] CUS-010 implement the additive professional customer master with stable ID, code/profile/archive/concurrency, DTO-only paged queries, permissions, localization and regression coverage.
- [x] CUS-020 add tenant-safe customer contacts and billing/shipping addresses with DTO snapshot contracts, defaults, archive and concurrency.
- [x] CUS-030A establish the configurable customer-receivable policy: shared control, automatic individual account or manual selection, with prospective effect and transition guard.
- [x] CUS-030B add prospective customer payment-term, configured-currency and tax/exemption defaults; defer price list and salesperson/team to their owning SAL-020+ capabilities.

- [x] PUR-000 complete purchase architecture review, legacy compatibility boundary, migration strategy and vertical-slice roadmap.
- [x] PUR-005 fix context ownership, quantity authority, staged currency, approval and GRNI boundaries before runtime implementation.
- [x] PUR-010 add supplier-product purchasing information and lead time; migration applied to `AHMAD/MiniStoreDb`.
- [x] PUR-020 add the Purchase Request lifecycle with Draft/Submit/Cancel, frozen units, history, permissions and bilingual UI.
- [x] PUR-025 add configurable purchase approval rules, frozen instances, ordered decisions, permissions and bilingual UI.
- [x] PUR-030 add sourcing events and supplier invitations from approved Purchase Requests without stock changes.
- [x] PUR-040 add supplier quotations, comparison and a reasoned award decision.
- [x] PUR-050 add the Purchase Order lifecycle without stock mutation. Receipt/bill-based closure is deferred to later slices.
- [x] PUR-060 complete partial Goods Receipts, history, physical movement facts and SQL concurrency coverage.
- [x] PUR-065 complete Inventory/GRNI posting, Vendor Bill clearing, unbilled receipt-return reversal and PUR-160 reconciliation reporting with SQL coverage.
- [x] PUR-070 add receipt-based supplier returns with unbilled-quantity caps, inventory/tracking removal and GRNI/PPV reversal while preserving legacy returns.
- [x] PUR-080 add receipt-allocated Vendor Bills with normalized duplicate protection and GRNI/Input Tax/PPV/Accounts Payable posting.
- [x] PUR-090 add three-way matching tolerances, persisted exceptions and controlled override.
- [x] PUR-100 add partial supplier payments and Accounts Payable settlement.
- [x] PUR-110 convert reviewed purchase-sourced replenishment into a traceable, duplicate-safe Draft Purchase Request.
- [x] PUR-120 add awarded-quotation Purchase Agreements with capped Draft order releases and repeat templates that generate traced Draft Purchase Requests.
- [x] PUR-130 allocate receipt-linked landed costs with auditable AVCO valuation, consumed-cost expense and balanced clearing journals.
- [x] PUR-140 add bounded purchasing dashboards and supplier-performance projections over authoritative tenant-owned operational and financial records.
- [x] PUR-150 add idempotent barcode-assisted Draft Goods Receipt capture with explicit posting through the existing inventory/accounting path.
- [x] Complete the approved PUR-000 through PUR-150 purchase-management evolution plan.
- [x] PUR-160 add inventory-to-GL and GRNI reconciliation and close the remaining PUR-065 reporting gap.
- [x] PUR-165 add derived Purchase Order closure, controlled remaining-quantity closure and permission-gated reopen.
- [x] PUR-170 add Supplier Credit Notes for already billed receipt returns.
- [x] PUR-180 add controlled reversal for posted Goods Receipts, Vendor Bills, Supplier Payments and Landed Costs.
- [x] PUR-190 add multi-currency purchase, receipt, bill and payment accounting.
- [x] PUR-200 add a policy-controlled direct RFQ workspace and fast path.
- [x] PUR-210 add line-level and split supplier awards.
- [x] PUR-220 add service procurement, per-product billing policy, stock-neutral service completion and reviewed mixed-source Draft Vendor Bills with policy-aware GRNI/Expense posting.
- [x] PUR-230 complete bulk vendor price management with controlled CSV/native Excel templates, 500-row preview, atomic import, quantity price breaks/discounts, immutable full-snapshot history and expired/duplicate exception reporting.
- [x] PUR-240 add scheduled tenant-safe replenishment, historical-demand/seasonality suggestions, grouped Draft Purchase Requests or policy-bounded direct RFQs, idempotency and traceability.
- [x] PUR-250 add configurable receiving/quality routes, demand allocation and dropshipping contracts.
  - [x] PUR-250A configure one-step or receive/inspect/store routes, quality hold, accepted/rejected/damaged/short evidence and delayed stock/accounting posting.
  - [x] PUR-250B allocate incoming demand and define the dropshipping contract without internal stock.
- [x] PUR-260 add supplier email, attachments, confirmation tracking and reviewed OCR capture.
- [x] PUR-270 add a supplier portal plus secure API/EDI/UBL exchange and reliable outbox delivery.
- [x] PUR-280 add advanced purchase analytics and make a documented Legacy Direct Purchase retirement decision.
- [x] Add the bilingual Settings work procedure catalog with the complete purchasing sequence, role ownership, review responsibility, control principles, completion evidence and permission-aware links.
- [x] Extend the work procedure catalog into a complete 62-step ERP guide covering setup/administration, customers, sales/receivables, purchasing/payables, inventory/warehouses, accounting/close and analytics/management review. Completed 2026-10-23 with permission-aware real-action links and full Arabic resource coverage.
- [x] SAL-000 audit the actual Sales, Inventory, Invoicing, Accounting and Payment foundations; document the legacy immediate-sale compatibility boundary and an additive phased implementation plan.
- [x] SAL-005 approve the Sales context-ownership, invoicing-policy, COGS-timing, currency, reservation and POS boundaries before runtime implementation.
- [x] CUS-000 audit the Customer and Business Partner requirements against the actual repository and document compatibility, migration safeguards, ownership, tests and a phased implementation plan.
- [x] CUS-005 approve the Business Partner compatibility decision, customer-code policy, receivable-account model and archive rules before runtime implementation.
- [x] SAL-010 extend the Customer foundation additively while preserving current Customer IDs and Legacy Immediate Sale compatibility (delivered through CUS-010..030B).
- [x] SAL-020A approve deterministic price-rule priority, unit conversion, dated currency, rounding, tax/exemption snapshots and explainable result contract.
- [x] SAL-020B implement tenant-safe price-list/rule storage, customer default selection and read-only explainable evaluation without changing Legacy Sale/POS.
- [x] SAL-030 implement independent tenant-safe sales quotations with immutable commercial snapshots and no stock/accounting effect.
- [x] SAL-040 implement one-time accepted-quotation conversion to an independent sales order with no stock/accounting effect.
- [x] SAL-050 implement full/partial sales-order inventory reservation, release/reallocation, tenant-safe source-line audit and concurrent last-unit protection without changing OnHand or accounting.
- [x] SAL-060 implement partial/full reservation-backed delivery, exact-position stock/tracking issue, physical movement and one-time delivery COGS posting without creating an invoice or payment.
- [x] SAL-070 implement policy-based partial/multiple customer invoice documents with controlled source quantities, Draft/Approved/Cancelled lifecycle, exact delivery allocations and no premature accounting/payment effect. Completed 2026-10-20.
- [x] SAL-080 post Approved customer invoices to receivables/revenue/output tax with idempotent Posted/Reversed lifecycle, branch/warehouse dimensions, frozen journal evidence and no payment/inventory effect. Completed 2026-10-21.
- [x] SAL-090 add independent customer payments with capped partial/multi-invoice allocation, derived balances, foreign-exchange posting, journal evidence and controlled reversal. Completed 2026-10-21.
- [x] SAL-100A add delivery-backed customer return request Draft/Submit/Approve/Reject/Cancel control with cumulative quantity protection and no stock/accounting effect. Completed 2026-10-21.
- [x] SAL-100B receive and inspect Approved customer returns, recording explicit Restock/Quarantine/Reject/Destroy disposition and exact tracked identities without stock/accounting/credit-note effect. Completed 2026-10-21.
- [x] SAL-100C post Inspected return dispositions to inventory/tracking and reverse COGS where appropriate, while keeping credit notes separate. Completed 2026-10-21; migration applied to Demo, with the disposable SQL scenario still pending.
- [x] SAL-100D add independent retry-safe customer credit notes allocated to posted invoice/return quantities, reversing receivable/revenue/output tax and reducing balances without automatic cash refund or duplicate inventory mutation. Completed 2026-10-22; migrations applied to Demo, with the disposable SQL scenario still pending.
- [x] SAL-110 add configurable company/customer sales-credit, overdue, price-below-cost, exceptional-discount and margin controls with Warn/RequireApproval/Block actions, arbitrary role steps, frozen evidence and configurable self/distinct-approver safeguards. Completed 2026-10-22; migration applied to Demo.
- [x] SAL-120A add advanced versioned quotation templates with searchable categories/tags, reusable content, required/optional/alternative products, bounded option groups, quantity policy, optional extra lines, customer/date repricing and frozen applied-template evidence. Completed 2026-10-22; migration applied to Demo.
- [x] SAL-120B central organization identity, highly configurable versioned Document Designs, reusable central HTML-to-PDF engine, bilingual RTL/LTR quotation PDF and hash-protected identity/design/content/logo snapshot frozen at Send. Completed 2026-10-22 with migrations 133-134, Demo application and visual PDF verification. Customer portal, signature and payment remain later provider-dependent gates.
- [x] SAL-130A reusable provider-based Analytics Center with tenant-owned personal/shared workspaces, advanced widget settings, central previous-period/year comparison, and Sales/Purchasing providers. Completed 2026-10-22 with migration 135 applied to Demo and 1049 passing regression checks.
- [x] UI-AUDIT-001 verify every navigation, dashboard and static Razor link/form target, enforce antiforgery on every public MVC POST action, complete shared Arabic coverage, and protect customer create/edit binding plus visible validation feedback. Audit completed 2026-10-23; customer save, optional generated/policy fields and culture-safe decimal validation were verified 2026-10-24 with 1066 passing checks.
- [ ] SAL-130B add safe workspace edit/clone/archive, presets, central export and optional scheduled distribution after explicit approval.

- [x] WMS-050 reservations and exact transfer allocation with concurrent last-unit protection.
- [x] WMS-060 blind cycle counts and controlled inventory adjustment posting; scheduling remains deferred.
- [x] WMS-070A opening lot/serial allocation and traceability foundation.
- [x] WMS-070B tracked purchases, FEFO sales/recipe issues, transfer/cancellation and identifier-aware sales/purchase returns.
- [x] Product shelf-life defaults, mandatory receipt expiration, expiry blocking and warning dashboard.
- [x] Add permission-protected, reasoned lot/serial quarantine and release with immutable trace events.
- [x] Add tracked-identity recall creation/closure, automatic quarantine and affected-document trace.
- [x] Add a live expiration alert center sourced from current tracked balances and per-product warning horizons.
- [x] Resolve recall history into affected document types, supplier/customer identities, contacts and quantities.
- [x] Add immutable manual recall communication history with channel and outcome.
- [x] WMS-080A apply warehouse FIFO/FEFO/location-priority/minimize-locations policy to tracked issues and transfers.
- [x] WMS-080B1 add auditable manual tracked-identity override to wholesale sales.
- [x] WMS-080B2 extend audited identity override to transfers and preserve exact identities on cancellation.
- [x] WMS-080B3 extend audited identity override to POS tracked products.
- [x] WMS-080C1 add the tenant-safe putaway-rule model and deterministic suggestion service.
- [x] WMS-080C2 add putaway-rule administration UI and capacity-aware suggestions.
- [x] WMS-080C3 add policy-driven untracked exact-location removal.
- [x] WMS-090A add reviewed min/max replenishment rules and suggestions.
- [x] WMS-090B1 add Approved-transfer incoming/outgoing forecasting.
- [x] WMS-090B2 add reviewed replenishment conversion into a Draft stock transfer.
- [x] WMS-090D add read-only slow/dead inventory insights and no-movement-history visibility.
- [ ] WMS-090C add purchase-order intent after a purchase-order lifecycle exists.
- [x] WMS-100A add safe read-only product/location barcode resolution and availability lookup.
- [x] WMS-100B1 add idempotent scanned putaway through the controlled Unassigned Stock workflow.
- [x] WMS-100B2 add idempotent scanned relocation between exact locations.
- [x] WMS-100B3 add retry-safe absolute scanned counts to Draft inventory adjustments.
- [ ] WMS-100B4 add receiving scans after a real purchase-order/receipt workflow exists.
- [ ] Add scheduled/automated delivery channels and delivery-provider callbacks.

## Completed
- Refactored the Web composition root into focused presentation, persistence, security, business-service and application-pipeline extensions; removed duplicate sale registrations without changing runtime behavior.
- Core catalog, warehouse, supplier, inventory, purchase, sale and stock-transfer code exists.
- Shared-database tenant isolation is implemented for all current business entities, including active membership validation, tenant-scoped reads/writes/unique indexes and demo-data backfill.
- Added separately authenticated platform control center, bilingual public landing/pricing pages, plans/features/limits, tenant subscription states, user/warehouse limit enforcement and bounded percentage promotion codes.
- Added atomic self-service company registration with a selected-plan trial, tenant subscription self-service, persisted monthly/annual checkout quotes, promotion calculation/redemption and protected manual payment confirmation.
- Added tenant-scoped user-role assignments and removed cross-company global Admin checks from tenant administration controllers.
- Replaced separate invoice/transfer counters with centralized, tenant-scoped, rowversion-protected document sequences for wholesale, POS, transfers and journals.
- Added tenant-owned role definitions and permission sets, protected company Admin roles and tenant-local user assignments; removed direct EF access from RolesController.
- Reworked registration throttling to a configurable 20 requests/hour/IP default and added bilingual retry feedback instead of exposing a raw HTTP 429 page.
- Added a bilingual registration-rules popup covering company name/address, owner username/email, password complexity, selected plan and the 14-day trial.
- Added centralized immutable entity classification and database-enforced composite TenantId foreign keys for every tenant-to-tenant ERP/SaaS relationship; 59 protected relationships and no unsafe relationship remain in the local schema.
- Added and locally migrated the cafe recipe foundation: immutable recipes, unit conversion, automatic ingredient consumption, explicit kitchen variance and per-ingredient controlled negative stock.
- Added the authenticated Dashboard landing flow and locally migrated product classification, generated internal codes, optional barcodes, POS/sales channel controls and the advanced product catalogue.
- Replaced product and recipe unit entry with the tenant-managed measurement catalogue, including safe legacy backfill and immutable unit-code/factor snapshots on recipe versions.
- Added moving weighted-average valuation per product/warehouse, immutable cost snapshots, recipe-derived sale cost and provisional-negative settlement variance.
- Added idempotent sale posting for settlement, invoice discount, revenue, COGS and warehouse inventory.
- Added provisional-negative cost-variance settlement to purchase posting and excluded recoverable inclusive input tax from moving-average inventory cost.
- Added immutable invoice-level sales-tax snapshots, inclusive/exclusive calculation and output-tax journal posting.
- Added immutable partial/full sales returns with cumulative quantity control, proportional refund/tax reversal, direct-item restocking at historical cost and an atomic posted reversal journal.
- Added a central journal-posting gateway used by purchase, sale and sales-return workflows for source idempotency, numbering, balancing, posting and persistence.
- Added tenant fiscal periods with overlap prevention, Open/Soft Closed/Closed states, status audit metadata, concurrency protection and central posting-date enforcement.
- Added immutable partial/full purchase returns with cumulative and available-stock controls, moving-average issues, proportional supplier/tax/discount reversal and COGS cost-variance posting.
- Completed the repository-wide WMS gap analysis, target authority model, safe migration strategy, ADR, phased vertical-slice roadmap and file change map without changing runtime inventory code.
- Added WMS-000 read-only bilingual inventory reconciliation, dedicated permission/navigation, structured movement identity/cutover contract and a disposable two-tenant SQL fixture for query translation and concurrent last-unit protection.
- Implemented WMS-005 product categories, Mass/Length logistics metadata, handling requirements, tracking-policy guard, catalogue filters and additive migration; the migration is applied to the development database.
- Implemented WMS-010 hierarchical location metadata, barcode, sequence, tree ordering, warehouse-filtered parent selection, operational capabilities, edit workflow and cycle/same-warehouse validation. Putaway/transfers/relocations enforce receive/pick capabilities. Its additive migration is applied to `AHMAD/MiniStoreDb`; authenticated UI smoke remains because no login secret is stored in the repository.
- Completed UX-016 balanced tenant-owned recurring-journal templates and explicit idempotent Draft generation with separate
  View/Manage/Run permissions, antiforgery, rowversion, semantic audit evidence and bilingual Angular management. Migration
  138 is applied to local Demo; Release, 1,883 checks, 242/242 Angular tests, 137 templates, 78 destinations, 47/47 routes,
  production budgets and authenticated Edge 15/15 passed.

## In Progress
- MOD-030L completed the separately approved AuditEvents MVC retirement. Protected GET/HEAD compatibility routes remain,
  the legacy MVC export is retired without redirect, and Angular/API permissions and audit semantics are unchanged.
- MOD-030M completed the non-destructive PlatformAuditEvents audit. Static parity passed and tenant/anonymous isolation was
  proven live, but no authorized Platform cookie fixture exists; physical retirement remains blocked and unapproved.
- MOD-030N completed the non-destructive LocationMovements/AnalyticsCenter comparison. LocationMovements remains deferred
  for stock-mutation evidence; AnalyticsCenter is the proposed MOD-030O cohort and requires separate deletion approval.
- MOD-030O completed the separately approved AnalyticsCenter MVC retirement. Protected Index/builder GETs remain, the
  legacy workspace POST is retired without redirect, and services/API/Angular/providers are unchanged.
- MOD-030P completed the non-destructive Recipes/StockTransactions/Subscription/Users comparison. Recipes is the proposed
  MOD-030Q cohort; inventory, billing and identity mutations remain deferred and no deletion is yet approved.
- MOD-030Q completed the separately approved Recipes MVC retirement. Protected list/editor GETs now resolve to Angular,
  invalid product IDs return 404 and the retired MVC save returns 405; API and immutable-version behavior are unchanged.
- MOD-030R completed the non-destructive ProductCategories/PurchaseApprovalRules/SalesApprovalRules comparison.
  ProductCategories is proposed for MOD-030S; approval-rule cohorts remain deferred and no deletion is approved.
- MOD-030S completed the approved ProductCategories MVC retirement. Protected reads redirect to Angular and both legacy
  mutation routes return 405; API/service/category behavior remains unchanged.
- MOD-030T completed the non-destructive approval-rule lifecycle audit. Both paths retain server authority; SalesApprovalRules
  has the stronger isolated Angular/API/SQL evidence and is proposed for a separately approved MOD-030U retirement.
- PurchaseApprovalRules remained active at this audit point pending more isolated service-level SQL lifecycle evidence.
- MOD-030U completed the separately approved SalesApprovalRules MVC retirement. Protected reads redirect to Angular and
  legacy Create/SetActive POSTs return 405; API and sales-order approval behavior remain unchanged.
- MOD-030V completed isolated disposable-SQL lifecycle evidence for PurchaseApprovalRules, including rule resolution,
  frozen ordered steps, role-gated atomic decisions, rollback, uniqueness and rowversion.
- MOD-030W completed the approved PurchaseApprovalRules MVC retirement. Protected reads resolve to Angular; legacy
  Create/SetActive POSTs return 405 and API/service/workflow authority remains unchanged.
- The next retirement gate is a non-destructive audit that must name exactly three controllers as one cohort before deletion.
- MOD-030X completed the first three-controller cohort: GoodsReceiptReturns, PurchaseServiceAcceptances and
  SupplierCreditNotes. Nine protected reads now resolve to Angular and the three retired Create POSTs return 405.
- MOD-030Y completed the non-destructive audit and named the first ten-controller cohort. MOD-030Z is approved to execute
  exactly those ten surfaces together; identity, subscription and Platform boundaries remain excluded.
- MOD-030Z completed the first ten-controller retirement as one cohort; APIs/services and accounting/inventory authority remain.
- MOD-031A selected the next inventory/warehouse cohort and MOD-031B completed its ten-controller retirement. Twenty-five
  protected reads remain as Angular navigation compatibility; old mutations return 405. Inventory is 45/263/143.
- UX-016 recurring-journal templates and controlled runs is complete with Draft-only manual execution. UX-017 bank-statement
  reconciliation is proposed and requires owner approval before adding statement import and matching records.
- UX-009 navigation completeness is complete: 72 registered destinations and 41/41 operational routes are covered through
  permission-aware Purchasing, Sales and Accounting hubs. UX-010 Accounting Workspace Foundation is next.
- PUR-280 is implemented. The purchase completion roadmap is complete; continue from the master roadmap after reconciling customer-management and sales requirements.
- WMS delivery is paused by product-owner direction. Its remaining items stay documented in `WMS_EVOLUTION_PLAN.md` and resume after the current purchase-management priority.
- WMS-015A/B/C/D typed definitions, values, ProductTemplate grouping and controlled selection-based variant generation are implemented. Product remains the SKU; proceed to the next WMS slice.
- WMS-020A physical StockMovement kernel is implemented for idempotent Putaway/Relocation dual-write. Next: authenticated pilot smoke test and WMS-030 transfer/transit integration; do not cut over legacy reads yet.
- WMS-030 transfer movement stages are implemented without historic backfill. Next: authenticated transfer lifecycle smoke test, then WMS-040 explicit balances and availability; partial transit receipt remains deferred.
- WMS-040 InventoryBalance projection is implemented and reconciled (8 rows, zero ProductStock mismatches in the demo database). Next: authenticated workflow smoke tests, then WMS-050 reservations; keep ProductStock/ProductLocationStock authoritative during observation.
- Run an authenticated HTTP/SQL cafe recipe sale and reconciliation journey. Batch production, actual yields and negative-cost settlement remain follow-up work.
- Add per-terminal product assortment UI/enforcement and per-user branch/POS data scope.
- Build the central preview/validation/template/audit engine for Excel and CSV import/export, then onboard modules incrementally.
- Build generalized reversal metadata and controlled cancellation workflows on the central posting and fiscal-period foundation.
- Migrate remaining legacy feature screens and existing validation messages to the shared English/Arabic resources before making further functional changes to those screens.

## High Priority
- Do not begin movement/balance cutover before reconciling ProductStock, ProductLocationStock, implicit Unassigned Stock and latest StockTransaction snapshots per tenant/product/warehouse.
- [x] Disabled the historically exposed local Demo administrator, invalidated existing Demo sessions and verified a replacement administrator; reopen this action if an external deployment is discovered.



## Bugs / Technical Debt
- Missing database unique constraints for barcode and master-data names.
- Duplicate `PermissionsCodeExport` tree can drift.

## Accounting Gaps
- Sales/purchase posting, immutable returns, fiscal-period enforcement, financial statements, AR/AP aging and read-only
  cash-flow/close readiness and controlled period close are implemented. Mixed-rate product tax, wider reversal coverage,
  budgets and consolidation remain pending.

## Testing Gaps
- Security regression executable checks complete entity classification and composite tenant relationships. A live two-company SQL mutation was rejected by the database; comprehensive repository HTTP/database integration and penetration tests remain outstanding.

## Future ERP/SaaS Features
- Add company switching/invitations, editing existing user assignments, role templates/data scopes, plan versioning/overrides, external provider checkout/webhooks/refunds, 2FA for platform operators and controlled support impersonation.

## Security fixes completed (2026-09-13)
- Admin-only identity mutations enforced in both permission evaluators.
- Role deletion changed to antiforgery-protected POST; delete links replaced with forms.
- Login lockout and per-IP rate limiting enabled.
- Default admin password removed; bootstrap opt-in and existing-account promotion blocked.
- Broad exception handlers log details server-side and show generic errors.

## Inventory integrity completed (2026-09-13)
- Added ProductStock rowversion and Serializable UnitOfWork transactions for concurrent writes.
- Restricted manual movements to adjustments with a required reason.
- Rejected duplicate product lines in purchase and sale aggregates.
- Added focused metadata/domain regression checks and a SQL-backed two-tenant reconciliation/concurrent-last-unit fixture. Reservation and transfer retry concurrency follow their WMS phases.

## Warehouse location workflows completed (2026-09-14)
- Added unassigned-stock putaway, exact location balances, internal location-to-location movement and immutable putaway/relocation history.
- Enforced active same-warehouse locations, source availability and total destination-location capacity inside Serializable transactions.
- Added configurable warehouse operational types, Simple/LocationManaged/Hybrid control, POS eligibility, picking strategy, capacity enforcement and optional transfer-location requirements.
- Added rowversion-protected Inventory Settings defaults for new warehouses. Automatic FIFO/FEFO allocation and replenishment tasks remain follow-up work.
- Added branch warehouse operation permissions/priorities and per-POS default/alternative warehouse allow-lists. Automatic picking and replenishment tasks remain follow-up work.

## POS experience completed (2026-09-14)
- Added rowversion-protected per-terminal POS profiles and custom layout settings with a live administration preview.
- Added Retail, Grocery, Cafe, Restaurant and Quick Service presentation presets and runtime application on terminal selection.
- Added configurable Walk-in/Dine-in/Takeaway/Delivery modes, persisted service references, guest counts and per-item preparation notes with server-side terminal-policy validation.
- Restaurant table occupancy, kitchen tickets/routing, modifiers, combo meals, delivery dispatch and scale integration remain future POS workflow phases.

## Localization foundation completed (2026-09-14)
- Added company-default and per-user English/Arabic culture selection, centralized Arabic resources, RTL/LTR layout switching and Bootstrap RTL.
- Localized shared navigation, login/access-denied, notifications, delete confirmation, Settings hub and General Settings; all future UI changes must include both languages under `AGENTS.md`.


## Completed 2026-09-15: guided company onboarding
- [x] Continue registration into signed-in business setup.
- [x] Add presets for retail, grocery, cafe, restaurant, quick service, wholesale and services.
- [x] Create linked accounting, branch, warehouse, payment, tax, numbering, discount and POS foundations transactionally.
- [x] Support explicit Skip and resumption of Pending setup.
- [x] Accept and membership-validate the company slug during login.

## UX-013 completed (2026-11-06)

- [x] Server-authored AR/AP aging buckets and company-currency open-item balances.
- [x] Partial payment, credit-note and dated-reversal treatment.
- [x] Complete subledger-to-control-account reconciliation and source drill-down.
- [x] Frozen due-date evidence for new vendor bills without rewriting historical rows.

## UX-014 completed (2026-11-06)

- [x] Ledger-derived opening, movement and closing cash using configured settlement accounts.
- [x] Transparent operating/investing/financing classification with explicit unclassified review.
- [x] Read-only close checks for financial equality, AR/AP, inventory, draft journals and fiscal-period coverage.
- [x] Safe date/tab context, direct evidence links and authenticated Edge acceptance.

## UX-015 completed (2026-11-07)

- [x] Stepwise Open/SoftClosed/Closed and one-step reopen domain transitions.
- [x] Transactional readiness rerun that blocks on authoritative UX-014 exceptions.
- [x] Independent permissions, explicit antiforgery, required reason/rowversion and semantic audit evidence.
- [x] Bilingual state-aware UI that removes unavailable/repeated commands and links to readiness evidence.
- [x] No automatic closing journal and no modification of posted or source documents.

## MOD-031C completed (2026-11-14)

- [x] Retire the approved 15-controller MVC/Razor cohort.
- [x] Preserve 34 protected reads and reject retired writes with 405.
- [x] Pass Release, 1,638 checks, 242 Angular tests and bundle/browser gates.

## MOD-031D completed (2026-11-14)

- [x] Retire the approved 15 operational MVC/Razor surfaces.
- [x] Preserve 47 protected reads; keep writes at 405 and downloads API-only.
- [x] Pass Release, 1,605 checks, 242 Angular tests and bundle/browser gates.

## MOD-031C completed (2026-11-14)

- [x] Retire the approved 15-controller commercial MVC/Razor cohort.
- [x] Preserve 34 protected reads and reject retired writes with 405.
- [x] Pass Release, 1,638 regression checks, 242 Angular tests and bundle/browser gates.

## MOD-031E completed (2026-11-14)

- [x] Retire Users and Roles MVC controllers and Razor artifacts after Angular/API parity proof.
- [x] Preserve exact permission-protected legacy reads and reject retired writes.
- [x] Reach zero remaining ordinary presentation surfaces in the generated inventory.
- [ ] Verify the 13 retained operational boundaries independently; do not batch-delete identity/session/public/Platform routes.
- [x] MOD-031F verified and documented all 13 retained operational boundaries without deletion or mutation.
- [x] MOD-031G makes zero ordinary MVC presentation surfaces and the exact retained-boundary allowlist executable invariants.
- [x] MOD-031H reconciled final project/security/testing wording and closed the MOD-031 retirement stream.
- [ ] UX-017A audit bank-statement reconciliation readiness without schema or runtime mutation.
- [x] UX-017A audited bank reconciliation and defined an allocation-based staged design without runtime mutation.
- [ ] UX-017B implement immutable bank-account/statement CSV evidence and read-only candidates only.

## ERP enterprise expansion (2026-11-15)

- [x] ERP-001 complete source-backed enterprise gap audit and replace manual bank ledger IDs with named choices.
- [x] ERP-002 define branch context policy and add a read-only current-configuration workspace.
- [ ] ERP-003 add tenant-owned user branch grants and server-authoritative optional branch request context.
- [ ] After ERP-003, add a permitted-branch switcher without using client state as authorization.
- [ ] Keep business activities, inter-branch dispatch/receipt and POS cash sessions as separate vertical slices.
ERP-004 completed 2026-11-16: permitted branch selector, scoped persistence, dirty-tab protection and active-branch operational links. ERP-005 is the next proposed slice.
ERP-005 and ERP-006 completed together 2026-11-16: branch-authorized dispatch, repeated partial receipt and explicit open shortage/damage evidence. ERP-007 is next.

ERP-007 completed 2026-11-16: accepted POS cash-session ADR and tenant/branch/terminal domain foundation with blind counts and independent variance approval. ERP-008 is next for authorized open/movements/blind close/approval and explicit accounting integration.

## ERP-008 completed 2026-11-16

- [x] Add permissioned, CSRF-protected setup/open/move/close/approve commands.
- [x] Require verified open sessions and replay-safe cash sale/return evidence.
- [x] Route accounting through the shared journal gateway and expose a cohesive bilingual Angular workflow.
- [ ] ERP-009: hosted touch-POS/profile presentation audit and bounded usability improvements.

## ERP-009 implementation 2026-11-16

- [x] Apply terminal layout, theme/accent, title, cart-side, touch and autofocus settings.
- [x] Add explicit exact-match Enter scanning, Escape clear, quantity buttons and cart summary.
- [x] Pass Release, 1,649 regression checks, 250 Angular tests and production build.
- [ ] Close authenticated Edge desktop/mobile/RTL acceptance after the Windows automation ACL failure is resolved.
- [ ] Only then mark ERP-009 complete and advance the gate to ERP-010.

## ERP-009 completed 2026-11-16

- [x] Close authenticated Edge desktop/mobile/RTL/keyboard acceptance: 15/15 passed.
- [x] Harden captured-session validation and replace the stale retired-MVC culture assertion.
- [ ] ERP-010: branch/company dashboards and reconciliation reports.

## ERP-010 completed 2026-11-16

- [x] Add server-owned permitted-branch/company operational projections.
- [x] Add independent `Branches.ViewOperations` authorization without weakening `Branches.View`.
- [x] Render cohesive bilingual sales, stock, transfer and cash reconciliation indicators with existing drill-down links.
- [x] Close the planned ERP expansion roadmap at 10/10 after Release, 1,649 checks, 250 Angular tests and production build.

## V3 Recovery

- [x] REC-001: complete branch create/edit/activate/deactivate lifecycle in `/branches` with granular permissions, antiforgery, semantic audit, bilingual UX and regression verification (2026-11-17).
- [x] REC-002: linked-warehouse delete safety verified; superseded by the detailed completion entry below.
- [x] REC-002: prove linked-warehouse delete safety on migrated SQL, return localized HTTP 409, preserve Angular row/context, and confirm existing create/edit/branch-account UX (2026-11-17).
- [ ] REC-003: classify all Angular routes by controller/API, Application service, permission and verification evidence.
- [x] REC-003: generate cross-layer evidence for all 196 Angular screen routes and enforce zero unresolved P1 wiring for Accounting, Sales and Inventory (2026-11-17).
- [ ] REC-003 follow-up: manually review the seven authenticated/API-specific P2 authorization boundaries recorded in the route matrix.
- [ ] REC-004: normalize Internal API ProblemDetails and business-conflict classification without changing domain rules.
- [x] REC-004A: add bounded global toasts for mutation ProblemDetails while preserving inline field errors, page recovery and form values (2026-11-17).
- [ ] REC-004B: inventory and normalize server-side status/code classification for validation, duplicate, not-found, conflict and transient errors.
- [x] REC-004B1: reserve conflict for business-state 409 responses and keep explicit row-version concurrency semantics (2026-11-17).
- [x] REC-004B2: add typed Application problem semantics and migrate manual journals plus sales orders without message matching (2026-11-17).

- [x] REC-004B3: migrate stock transfers and purchase requests to typed 400/404/409 semantics (2026-11-17).
- [ ] REC-004B4: select the next bounded functional cohort from the measured 70-controller/959-throw backlog; do not bulk-rewrite by message text.

- [x] REC-004B4: make customer/supplier duplicates real 409 responses and remove product message matching (2026-11-17).

- [x] REC-004B5: migrate product categories, attributes and templates and remove duplicate message matching (2026-11-17).

- [x] REC-004B6: migrate measurement units, tax rates, payment methods and branches to typed Application problems (2026-11-17).

- [x] REC-004B7: migrate general, sourcing, POS, exchange-rate and accounting settings to typed Application problems (2026-11-17).

- [x] REC-004B8: migrate document-design lifecycle and API mapping to typed Application problems (2026-11-17).

- [x] REC-004B9: migrate fiscal-period lifecycle and posting-date enforcement to typed Application problems (2026-11-17).

- [x] REC-004B10: migrate chart-of-accounts creation and the central posting/reversal gateway to typed Application problems (2026-11-17).

- [x] ERP-ROADMAP-000: create the checkable ERP capability registry with 180 capabilities and 1,440 execution tasks (2026-11-18).
- [ ] ERP-ROADMAP-001: verify P0 partial capability gaps against current code and select the first bounded vertical implementation slice.
