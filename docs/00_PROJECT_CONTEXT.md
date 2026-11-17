# MiniStore project context
> ERP-005 and ERP-006 completed together 2026-11-16: approved inter-branch transfers now dispatch into an explicit in-transit lifecycle and support repeated partial destination receipts with open shortage/damage evidence. Source/destination branch context, warehouse policy, independent permissions, reservation consumption, valuation, tracking and locations remain server-authoritative. ERP plan has 10 slices; 6 are complete and 4 remain.
> ERP-004 completed 2026-11-16: the Angular shell restores only a server-permitted active branch in tenant/user-scoped storage, sends it only to internal APIs, protects dirty tabs during switching and exposes permission-aware operational links for the active branch. ERP plan has 10 slices; 4 are complete and 6 remain.
> ERP-003 completed 2026-11-16: user branch grants are tenant-owned, Branches.View and Branches.ManageAccess are independent, ordinary branch reads are grant-scoped, and X-MiniStore-Branch-Id is accepted only after server validation. ERP plan has 10 slices; 3 are complete and 7 remain.
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-11-14

UX-017 is complete through UX-017D: immutable bank-statement evidence, controlled partial matching, unreconciled close-readiness blocking and explicit fee/interest/transfer posting operate through tenant-scoped services, the official journal/fiscal-period gateway and one cohesive Angular workspace. Suggestions never auto-post.

ERP-001 records a source-backed enterprise gap analysis. Current branch functionality is an accounting/access dimension rather than a complete branch workspace or user scope; inter-branch receipt and POS cash sessions are not implemented. The first safe Phase-1 slice replaces manual bank-ledger ID entry with permission-scoped named active-Asset options and server revalidation.

The Angular UI/UX modernization program under `UI_UX_MODERNIZATION_PLAN.md` is complete for its approved presentation
scope through UX-015:
the permission-aware shell uses persistent collapsible business groups, active-route expansion and bilingual
breadcrumbs while retaining Router authority and the existing persistent workspace tabs. Five pilot pages now preserve
allowlisted list context through URL state and tenant/user-scoped memory; Purchase Request and Stock Transfer create
forms keep only safe structural drafts in memory. Products, Purchase Requests and Stock Transfers now use shared page
headers, filter bars, summary strips, section headers and workflow footers. Their list tables now use typed column
configuration, feature-owned cell templates and shared table states without client-side data authority. Shared form
sections, localized required/optional cues and detail sections now pilot Products, Purchase Requests and Stock Transfers
without moving validation or workflow authority to the client. Stock Transfers, Purchase Requests, Purchase Orders and
Sales Orders now render shared display-only progress and localized next-step guidance from their server-returned state;
permissions and transition commands remain feature/server-owned. The shell now provides permission-filtered page/command
search with Ctrl+K or `/`, workspace-tab navigation and memory-only safe recents; it never indexes business entities or
persists search text. The final executable inventory accounts for all 113 feature pages: 102 ERP workspace pages use the
shared header and 11 authentication/public/onboarding/platform/design-reference layouts are named exceptions. Angular,
Release, security, accessibility, bundle and hosted browser gates passed. `MOD-030F` completed the non-destructive
physical-retirement inventory: all 411 discovered MVC actions across 82 surfaces are classified, 217 Razor files are
accounted for and mixed controllers are blocked from early view deletion. No MVC file was removed; every physical
retirement cohort still requires separate owner approval.
`MOD-030G` then completed the separately approved first retirement pilot: five read-only inventory controllers and their
five Razor views were removed. Permission-protected legacy GET/HEAD redirects preserve both historical URL forms and
query strings while the Angular APIs remain authoritative. `MOD-030H` subsequently retired the separately approved
JournalEntries Details and PurchaseAccountingReconciliation Index MVC presentation after confirming the existing API and
Angular replacements. The active inventory is now 75 MVC controllers, 404 actions and 210 Razor files; seven surfaces
are physically retired and anonymous Public is the only remaining R1 surface. Public and mixed surfaces remain approval-gated.
`MOD-030I` audited that final surface without deletion. Public has Angular/API parity, but its exact anonymous `/` and
`/pricing` paths are incompatible with the current permission-protected conventional retired-route catalog. It is now the
sole `R1-A` boundary; any physical retirement requires a separately approved exact anonymous route contract. Active counts
remain 75 MVC controllers, 404 actions and 210 Razor files.
`MOD-030J` then completed that separately approved retirement: `PublicController` and its two views were removed, while
exact anonymous GET/HEAD redirects preserve `/`, `/pricing` and their query strings. Registration, authentication,
rate limiting, plan validation, API and shared layout boundaries remain unchanged. Active inventory is now 74 MVC
controllers, 402 actions and 208 Razor files; nine presentation actions across eight surfaces are physically retired and
no R1/R1-A surface remains.
`MOD-030K` then completed a non-destructive prioritization audit of all 54 R2 mixed controllers. The generated inventory now
orders dependency counts without treating the score as deletion authority. Tenant `AuditEvents` is the next proposed bounded
cohort because its read and permission-separated CSRF export have complete Angular/API replacements; Platform audit and
stock-moving candidates were deferred for higher session/mutation risk. No runtime file changed and counts remain 74/402/208.
`MOD-030L` then completed the separately approved tenant AuditEvents retirement. Its controller and Razor view are removed;
permission-protected legacy GET/HEAD routes preserve query strings into Angular, while the legacy MVC export POST is retired
without redirect. The Application read service, versioned API, Angular page, permission separation and audit semantics remain
unchanged. Active inventory is now 73 MVC controllers, 400 actions and 207 Razor files.
`MOD-030M` then audited PlatformAuditEvents without runtime changes. Static MVC/API/Angular and policy parity is complete,
but all available browser states are tenant-only. Live tenant/anonymous probes correctly failed at the isolated Platform
cookie boundary. Physical retirement is deferred until an authorized Platform fixture proves read and role behavior.
`MOD-030N` then compared LocationMovements and AnalyticsCenter without mutation. LocationMovements is deferred behind
database-backed stock replay/rejection evidence. AnalyticsCenter is the next proposed bounded cohort because its read,
builder and configuration-only creation flow have complete Angular/API replacements. No deletion is approved.
`MOD-030O` then completed the separately approved AnalyticsCenter retirement. Its controller and two Razor views are gone;
protected legacy GET/HEAD routes preserve Index and builder navigation, while the legacy workspace POST is retired without
redirect. Services, providers, API, Angular, permissions and delegation remain unchanged. Active inventory is now 72 MVC
controllers, 397 actions and 205 Razor files.
`MOD-030P` then compared Recipes, StockTransactions, Subscription and Users without mutation. Recipes is the next bounded
candidate because its Angular/API flow reuses `RecipeService` and preserves authorization, antiforgery and server validation
without posting stock or accounting entries. Stock movement, billing and identity cohorts remain deferred behind dedicated
acceptance evidence; physical Recipes retirement remains separately approval-gated as MOD-030Q.
`MOD-030Q` then completed the explicitly approved Recipes retirement. Its controller and two Razor views are gone;
protected legacy list/editor GET/HEAD routes preserve navigation into Angular, including positive product-ID conversion.
The retired MVC save returns 405; API, RecipeService and immutable recipe behavior remain unchanged. Active inventory is
now 71 MVC controllers, 394 actions and 203 Razor files.
`MOD-030R` then compared ProductCategories, PurchaseApprovalRules and SalesApprovalRules without mutation.
ProductCategories is the next bounded configuration candidate; purchase and sales rules remain deferred because their
activation changes document approval routing. Physical ProductCategories retirement remains separately approval-gated.
`MOD-030S` then completed the explicitly approved ProductCategories retirement. Its controller and Razor view are gone;
protected legacy reads redirect to Angular, while old create/status POSTs return 405. API, ProductCategoryService and
catalog rules remain unchanged. Active inventory is now 70 MVC controllers, 391 actions and 202 Razor files.
`MOD-030T` then audited purchase and sales approval lifecycles without mutation, and `MOD-030U` completed the separately
approved SalesApprovalRules presentation retirement. Its controller and Razor view are gone; protected reads redirect to
Angular and old Create/SetActive POSTs return 405. The API, SalesOrderApprovalService, rule evaluation and approval workflow
remain unchanged. Active inventory is now 69 MVC controllers, 388 actions and 201 Razor files.
`MOD-030V` then closed the remaining PurchaseApprovalRules evidence gap with a disposable SQL lifecycle scenario over the
real Application service, repositories and UnitOfWork. It changed test code only and authorizes no presentation deletion.
`MOD-030W` then completed the approved PurchaseApprovalRules presentation retirement. The controller and single Razor view
are gone; protected reads resolve to Angular and the retired Create/SetActive POSTs return 405. API, service, authorization
and purchase-request approval behavior remain unchanged. Active inventory is now 68 MVC controllers, 385 actions and 200
Razor files. Future retirement planning uses explicit three-controller cohorts.
`MOD-030X` completed the first such cohort: Goods Receipt Returns, Purchase Service Acceptances and Supplier Credit Notes.
Their three controllers and nine views are gone; exact protected legacy reads resolve to Angular and old Create POSTs return
405. APIs, services, permissions and financial/inventory posting rules remain unchanged. Active inventory is 65 MVC
controllers, 373 actions and 191 Razor files.
`MOD-030Y` then audited and named the first ten-controller cohort without runtime mutation. It deliberately excludes
authentication, subscription, user and Platform session boundaries. Exact execution scope is recorded in NEXT_TASK as
MOD-030Z; active inventory remains 65 controllers, 373 actions and 191 Razor files until that cohort passes its gates.
`MOD-030Z` then completed that exact ten-controller cohort. Twenty-one presentation GET actions and 21 user-facing views
were retired while APIs and Application authority remained unchanged. Active inventory is now 55 MVC controllers, 324
actions and 170 Razor files.
MOD-031A selected the next connected inventory/warehouse cohort, and the owner authorized immediate execution without
another approval pause. MOD-031B then retired its ten MVC controllers and 25 user-facing views while preserving the
authenticated Angular APIs and Application/domain authority. Twenty-five protected legacy GET/HEAD routes remain as
navigation compatibility only; old mutations return 405. Active inventory is now 45 controllers, 263 actions, 143 Razor
files and 127 user-facing views.
MOD-031C retired an approved fifteen-controller commercial cohort. Thirty-four protected GET/HEAD routes preserve
navigation; writes are never redirected. Active generated inventory is 30 MVC surfaces, 214 actions, 100 Razor rows
and 93 views.
MOD-031C retired an approved fifteen-controller commercial cohort. Thirty-four protected GET/HEAD routes preserve
navigation; writes are never redirected, and supplier attachment download remains API-only. Active generated inventory
is now 30 MVC surfaces, 214 actions, 100 Razor rows and 93 views.
MOD-031C then retired an explicitly approved fifteen-controller commercial cohort. Thirty-four protected GET/HEAD
routes preserve navigation; writes are never redirected, and supplier attachment download remains API-only. The active
generated inventory is now 30 MVC surfaces, 214 actions, 100 Razor control rows and 93 views.
MOD-031D retired the next approved fifteen operational MVC/Razor surfaces after confirming Angular/API, permissions and
download boundaries. Forty-seven protected reads preserve navigation; writes return 405 and downloads remain API-only.
The active inventory is now 15 MVC surfaces, 91 actions, 50 Razor control rows and 45 views.

`MOD-031E..H` supersede every earlier active-inventory snapshot above. Users/Roles were the final ordinary MVC
presentation cohort. The generated source inventory now reports zero ordinary MVC presentation surfaces, 13 intentional
runtime boundaries and one explicit mutation-only Language endpoint. The retained controllers own identity/session,
onboarding, settings/download rollback, subscription, supplier-token or isolated Platform responsibilities; they are not
unfinished Angular pages. `Test-MvcPresentationInvariant.ps1` is part of the standard modernization gate and fails if an
ordinary MVC business page returns or the reviewed boundary allowlist drifts. The MOD-031 retirement stream is closed.

UX-009 adds permission-aware Purchasing, Sales and Accounting hubs with complete operational-route discovery. UX-010
projects the tenant chart of accounts as an expandable hierarchy and adds a permission-protected, repository-paged journal
directory. UX-011 adds server-authored financial reports. UX-012 adds independently permissioned balanced manual Drafts,
fiscal-period-protected posting and exact separate reversal through the shared journal gateway.
UX-013 adds server-authored AR/AP aging and full control-account reconciliation with frozen company-currency carrying
values and new-vendor-bill due-date snapshots.
UX-014 adds ledger-derived cash flow and read-only period-close readiness. UX-015 adds controlled stepwise fiscal-period
close/reopen execution with server-side readiness blocking, separate permissions, antiforgery and semantic audit evidence;
it deliberately does not create automatic closing journals or modify posted entries.

## Project identity
MiniStore is an Angular-presented ERP workspace hosted by an ASP.NET Core application. It targets .NET 10 and Angular 22,
uses EF Core 10 with SQL Server and ASP.NET Core Identity, and follows a Domain/Application/Infrastructure/Web layered
solution. `MiniStore.Web` remains the same-origin composition, API, authentication and compatibility host; Razor is limited
to the 13 reviewed runtime boundaries rather than ordinary business presentation. No separate public API project exists.

## Current development stage
The repository implements an operational slice for catalog, warehouses, stock, suppliers, purchases, sales (wholesale and POS), configurable discounts, stock transfers, users/roles, settings and shared-database SaaS tenancy. Public signup, pricing, plans, trials, subscription access, promotion codes and a separate platform-owner control center are implemented. Product classification, generated internal codes, advanced catalog search, managed measurement units, immutable prepared-product recipes, automatic ingredient consumption and controlled kitchen-negative stock are also implemented. It is not yet a complete accounting ERP and does not yet integrate an external payment provider.

The WMS track is temporarily paused after implementing reconciliation, logistics, movements, balances, reservations, counts, lot/serial execution, removal/putaway policies, replenishment transfer/purchase-demand planning, inventory insights and safe scan workflows. The additive procure-to-pay evolution is implemented through PUR-280. Advanced purchasing analytics expose filterable spend, delay, confirmation, price-variance, supplier, seasonal, buyer-workload and open-exception evidence with source definitions and document links. Creation of new Legacy Direct Purchases is retired; historical records remain visible and users are directed to Purchase Requests for new activity.

The imported ERP SaaS Foundation Blueprint remains a proposed reference. The project-control audit provides current control evidence. CUS-010/020/030A/030B provide the professional Customer master and receivable policy. SAL-020A/B through SAL-100D provide pricing through collection and controlled returns/credits. SAL-110 adds tenant-configurable order controls and sequential approvals. SAL-120A/B add versioned quotation templates, central organization identity, a reusable document designer/PDF engine and frozen sent-document snapshots. SAL-130A adds a reusable provider-based Analytics Center with personal/shared workspaces and Sales/Purchasing providers. Angular functional migration, dual-run acceptance, runtime cutover and physical presentation retirement are complete through `MOD-031H`. Ordinary business presentation is Angular under `/app`; server-owned runtime boundaries remain explicit and guarded. Migration source/model and the local Demo database are reconciled through migration 138. Legacy Sale/POS business behavior remains unchanged.

## Status classification
- IMPLEMENTED: catalog, warehouses, suppliers, stock balances/movements, purchases, sales, transfer workflow, Identity login, role-permission checks, audit-log rows and Angular business presentation.
- PARTIALLY IMPLEMENTED: authorization administration, auditability and inventory controls.
- IMPLEMENTED: tenant-scoped centralized numbering for wholesale sales, POS sales, stock transfers and journal entries, including safe defaults and concurrent-edit protection.
- IMPLEMENTED: database-enforced tenant relationship boundaries across all current ERP entities and tenant subscription redemptions, with automatic entity classification and relationship regression checks.
- IMPLEMENTED: posted sales and purchase returns plus tenant fiscal-period posting controls.
- PLANNED: reversal coverage outside the four supported purchase document types, external payment-provider/webhook integration, API/integrations.
- UNKNOWN: production deployment and migration process, off-host scheduled backups, hosted monitoring/alerts, external integrations, and production operational ownership. Local encrypted restore and observability drills are proven.

## Main modules
| Module | Status | Core code | Notes |
|---|---|---|---|
| Products | IMPLEMENTED | `Product`, `ProductService`, `ProductsController` | Generated code, optional barcode, raw/direct/prepared type, managed stock unit, channel controls and advanced catalog search. |
| Warehouses | IMPLEMENTED | `Warehouse`, `WarehouseService` | Named warehouse master data. |
| Suppliers | IMPLEMENTED | `Supplier`, `SupplierService` | Supplier master data. |
| Inventory | PARTIALLY IMPLEMENTED | `ProductStock`, `StockTransaction`, recipes, measurement units | Balances, moving-average valuation, auditable cost movements, managed-unit conversion, prepared recipes, controlled negative variance and purchase-time variance settlement; manual adjustment posting remains pending. |
| Purchases | IMPLEMENTED | `Purchase`, `PurchaseService`, `PurchaseReturnService` | Receipts increase stock; posted partial/full supplier returns remove available stock at moving-average cost and reverse accounting. |
| Sales | PARTIALLY IMPLEMENTED | `Sale`, pricing/order/delivery/invoice/payment, return, approval and quotation-template aggregates | Legacy Sale/POS remains operational; the professional lifecycle includes configurable controls, advanced quotation templates, branded PDF and an Analytics Center provider. Customer collaboration and cutover remain staged. |
| Analytics | PARTIALLY IMPLEMENTED | `AnalyticsWorkspace`, `AnalyticsCenterService`, module providers | Reusable personal/shared workspaces, advanced widget settings and central time comparison; Sales and Purchasing are the first providers. Editing, scheduled delivery and governed self-service fields remain staged. |
| Stock transfers | IMPLEMENTED | `StockTransfer`, `StockTransferService` | Draft/submitted/approved/posted/cancelled flow. |
| Settings | PARTIALLY IMPLEMENTED | settings entities/services | General, discount, inventory, accounting, fiscal periods, POS, centralized document numbers and a complete bilingual role-aware 62-step work procedure catalog across seven ERP modules. |
| Identity & permissions | PARTIALLY IMPLEMENTED | Identity + `PermissionAuthorizationHandler` | Custom permission rows mapped to roles. |

## Current problems
See `06_SECURITY.md`, `04_ACCOUNTING.md`, and `TODO.md`. The historically exposed administrator was closed for the evidenced local Demo environment, an encrypted isolated restore drill reconciled without differences and migrations through 135 are applied with no pending EF model change. The local Demo audit and retention features are enabled through User Secrets. Coverage reconciliation is complete, new legacy writes are frozen behind a reversible flag, and pre/post signed checkpoints preserved all 710 historical legacy rows. Real deployment identities, off-host backup/checkpoint storage, production deployment controls and broader integration testing remain open.

## Recommended next steps
1. Continue only with the single approved slice recorded in `project-control/NEXT_TASK.md`.
2. Keep the zero-MVC-presentation invariant and the 13 runtime-boundary allowlist green.
3. Treat any retained-boundary removal as a separate architecture/security change with the matching session fixture.

## Read first
Read `project-control/PROJECT_CONTEXT.md`, `project-control/PROJECT_STATUS.md`, `project-control/MASTER_ROADMAP.md` and `project-control/NEXT_TASK.md`; then read `01_CODEBASE_MAP.md`, the relevant module document and actual source.

## Guided company onboarding update (2026-09-15)
New-company registration now continues into a bilingual guided setup. Business presets atomically create a starter chart of accounts, linked branch and warehouse, payment, tax, numbering, discount, inventory and POS configuration. Owners may skip for manual setup, and tenant login accepts an optional company code (the tenant slug).


ERP-007 completed 2026-11-16: tenant/branch/terminal-scoped POS cash policy, drawer, session, movement and blind-count domain evidence now exists with terminal exclusivity, rowversion and independent variance approval. No runtime API, POS sale change or accounting posting was introduced; ERP-008 remains the operational slice.

## ERP-008 POS cash operations — complete 2026-11-16

Authorized branch/terminal cash sessions now support setup, opening, reasoned movements, blind close and independent variance approval. Cash sales and returns attach replay-safe evidence, while accounting stays behind the shared journal gateway. The ten-slice ERP plan is 8/10; ERP-009 is next.

## ERP-009 implementation — verification pending 2026-11-16

The POS page now applies terminal product layout, theme/accent, title, cart side, touch targets and autofocus settings. Exact barcode/code/name Enter-add, Escape clear, quantity buttons and a compact cart summary reduce operator clicks. Business authority is unchanged. Release, 1,649 regression checks, 250 Angular tests and production build pass; authenticated Edge acceptance remains pending because the Windows browser helper failed before opening a window.

## ERP-009 completion — 2026-11-16

Authenticated Microsoft Edge acceptance passed 15/15 after hardening session capture and replacing a stale retired-MVC culture probe with the protected Angular redirect contract. ERP progress is 9/10; ERP-010 dashboards and reconciliation is next.

## ERP roadmap closure (2026-11-16)

The planned ERP expansion is complete at 10/10. ERP-010 added a permission-scoped, server-owned branch/company operational dashboard to the existing Angular Branches workspace; it does not introduce alternate accounting or client-calculated financial authority.

## V3 recovery status — 2026-11-17

The historical ERP 10/10 roadmap is complete, but it is not a declaration that the enterprise product is complete. Recovery task REC-001 adds discoverable Branch lifecycle management to the cohesive Angular `/branches` feature with independent create/edit/status permissions, Application-layer validation and semantic auditing. REC-002 subsequently completed warehouse delete safety; the active gate is REC-003 route/function coverage.
## REC-002 warehouse delete safety — 2026-11-17

Warehouse create/edit and named branch/account selection were already complete. REC-002 closes the verified delete-error gap: relational constraints remain authoritative, the infrastructure repository translates a referenced-warehouse FK rejection into a stable business conflict, and the API returns localized ProblemDetails with HTTP 409. SQL integration proves referenced inventory survives and an unused warehouse remains deletable. The active gate is REC-003 route/function coverage.
## REC-003 cross-layer route inventory — 2026-11-17

The executable route/function inventory covers 196 unique Angular screen routes and maps component-local API contracts to controllers, Application services, permission evidence and adjacent Angular tests. No unresolved P1 wiring gap remains; seven P2 routes use authenticated or API-specific authorization and remain explicitly queued for manual security review. Accounting (11), Sales (37) and Inventory (22) are all present with zero P1. REC-004 is the active gate.
## REC-004A operation error toasts — 2026-11-17

Angular now presents concise localized ProblemDetails from failed mutation requests as bounded, dismissible toasts while retaining existing inline recovery and form state. Field-level validation remains inside forms, GET/load failures remain in page recovery states, and 401 continues through the dedicated authentication handler. This is the first REC-004 presentation slice; server error classification remains the active task.
## Error contract update — 2026-11-17

Internal API ProblemDetails now distinguishes ordinary business conflicts (`conflict`) from optimistic row-version conflicts (`concurrency`). HTTP 400 remains field/form validation, 404 remains not-found, and all responses retain correlation evidence. Angular mutation failures can therefore show concise toast feedback without misrepresenting a delete/state restriction as a concurrent edit.
REC-004B2 (2026-11-17) introduces presentation-neutral typed Application failures and a single Web ProblemDetails mapping. Manual journals and sales orders are the first migrated high-risk cohort; no HTTP concepts entered Application services and no business rules changed.
REC-004B3 (2026-11-17) migrates stock-transfer and purchase-request failure semantics to the typed Application problem contract. It changes response classification only; transfer valuation, reservation, receipt, approval and purchase-request rules remain server-authoritative and unchanged.
REC-004B4 (2026-11-17) aligns customer, supplier and product master-data failures with the typed ProblemDetails contract. Duplicate records now return real HTTP 409 responses, concurrency remains distinct, and Products API no longer classifies errors by message text.
REC-004B5 (2026-11-17) migrates product category, attribute and template configuration to typed ProblemDetails semantics. Duplicate detection no longer depends on controller message parsing; catalog business rules are unchanged.

REC-004B6 (2026-11-17) migrates measurement-unit, tax-rate, payment-method and branch reference masters to typed ProblemDetails semantics. Missing references return 404, duplicates return 409/duplicate and invalid branch account state returns 409/conflict; existing business rules are unchanged.

REC-004B7 (2026-11-17) extends typed ProblemDetails semantics across general, sourcing, POS, exchange-rate and accounting settings. Existing concurrency, validation, permission and business rules remain unchanged.

REC-004B8 (2026-11-17) migrates the complete document-design lifecycle to typed ProblemDetails semantics without changing immutable versioning, activation/default behavior or rendering.
