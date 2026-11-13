# MiniStore project context
> Status: PARTIALLY IMPLEMENTED  
> Source of truth: Code  
> Last reviewed: 2026-11-13

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
MiniStore is a server-rendered ASP.NET Core MVC store-management application under incremental Angular modernization. It targets .NET 10, uses EF Core 10 with SQL Server, ASP.NET Core Identity, Razor views, and a Domain/Application/Infrastructure/Web layered solution. The existing Web host now includes a protected versioned internal API foundation; no separate public API project is present.

## Current development stage
The repository implements an operational slice for catalog, warehouses, stock, suppliers, purchases, sales (wholesale and POS), configurable discounts, stock transfers, users/roles, settings and shared-database SaaS tenancy. Public signup, pricing, plans, trials, subscription access, promotion codes and a separate platform-owner control center are implemented. Product classification, generated internal codes, advanced catalog search, managed measurement units, immutable prepared-product recipes, automatic ingredient consumption and controlled kitchen-negative stock are also implemented. It is not yet a complete accounting ERP and does not yet integrate an external payment provider.

The WMS track is temporarily paused after implementing reconciliation, logistics, movements, balances, reservations, counts, lot/serial execution, removal/putaway policies, replenishment transfer/purchase-demand planning, inventory insights and safe scan workflows. The additive procure-to-pay evolution is implemented through PUR-280. Advanced purchasing analytics expose filterable spend, delay, confirmation, price-variance, supplier, seasonal, buyer-workload and open-exception evidence with source definitions and document links. Creation of new Legacy Direct Purchases is retired; historical records remain visible and users are directed to Purchase Requests for new activity.

The imported ERP SaaS Foundation Blueprint remains a proposed reference. The project-control audit provides current control evidence. CUS-010/020/030A/030B provide the professional Customer master and receivable policy. SAL-020A/B through SAL-100D provide pricing through collection and controlled returns/credits. SAL-110 adds tenant-configurable order controls and sequential approvals. SAL-120A/B add versioned quotation templates, central organization identity, a reusable document designer/PDF engine and frozen sent-document snapshots. SAL-130A now adds a reusable provider-based Analytics Center with personal/shared workspaces and Sales/Purchasing providers. Angular functional migration, `MOD-029` dual-run acceptance and reversible runtime cutover through `MOD-030E` are complete. All 81 presentation surfaces are committed `AngularDefault` under `/app`; the remaining Language endpoint is explicitly classified as a POST-only, antiforgery-protected MVC compatibility boundary rather than a presentation gap. Physical MVC retirement remains a separate approval-gated phase. Migration source/model and the local Demo database are reconciled through migration 136. Legacy Sale/POS business behavior remains unchanged.

## Status classification
- IMPLEMENTED: catalog, warehouses, suppliers, stock balances/movements, purchases, sales, transfer workflow, Identity login, role-permission checks, audit-log rows, Razor UI.
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
1. Review and approve the next bounded accounting slice recorded in project-control/NEXT_TASK.md.
2. Retire no MVC cohort until its exact files, mutation/download exceptions and deployment rollback are explicitly approved.
3. Consider physical MVC retirement only after runtime coverage and staging evidence are complete.

## Read first
Read `project-control/PROJECT_CONTEXT.md`, `project-control/PROJECT_STATUS.md`, `project-control/MASTER_ROADMAP.md` and `project-control/NEXT_TASK.md`; then read `01_CODEBASE_MAP.md`, the relevant module document and actual source.

## Guided company onboarding update (2026-09-15)
New-company registration now continues into a bilingual guided setup. Business presets atomically create a starter chart of accounts, linked branch and warehouse, payment, tax, numbering, discount, inventory and POS configuration. Owners may skip for manual setup, and tenant login accepts an optional company code (the tenant slug).

