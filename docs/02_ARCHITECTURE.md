# Architecture

## Incremental Angular API boundary (2026-10-24)

During migration, `MiniStore.Web` remains the composition and same-origin host for both MVC and the versioned internal `/api/v1` surface. API controllers use explicit Application DTOs/services; repositories and EF Core are not exposed to presentation. The existing Identity cookie, tenant middleware and permission authority remain authoritative. Angular owns presentation state only, bootstraps its server session and CSRF token centrally, and treats returned permissions as display guidance while every server endpoint reauthorizes independently. OpenAPI is authenticated and filtered to internal v1 paths. See `modernization/PHASE_02_API_FOUNDATION.md` and the same-origin modernization ADR.

The Angular visual layer uses semantic CSS custom properties as its stable component contract. Runtime theme and density choices alter root data attributes and tokens rather than duplicating markup. Locale owns `lang`/`dir`; components use logical CSS. Browser storage contains only versioned, non-sensitive UI preferences and is never an authority for permissions or business state. See `modernization/PHASE_03_VISUAL_SYSTEM.md`.

The first business slice, Customers, proves coexistence rather than replacement. MVC and `/api/v1/customers` share the same Application service and DTO authority; Angular adds no direct repository or EF path. Server-side paging, numbering, policy resolution, validation, transaction and concurrency remain behind that boundary. The Angular feature keeps API models/client, presentation components and a separate stylesheet within one feature folder and integrates with the reusable workspace without persisting form payloads.

Shared Angular UI is deliberately presentation-only. Headers, feedback states, table overflow regions and pagers accept text/content or emit UI intent; they never fetch data, inspect permissions or interpret business fields. Customers and Suppliers prove reuse. Supplier paging was added through Domain repository criteria and Application DTO/service projection rather than browser slicing or controller data access.

The complete supplier Angular slice follows the same vertical boundary as Customers: typed feature client → versioned API controller → `SupplierService` → existing repositories/assignment service/UnitOfWork. Form-option DTOs are convenience projections, not browser authority; every submitted account and product relationship is revalidated through the Application layer. Supplier concurrency has a dedicated exception mapped to HTTP 409 while remaining compatible with the MVC `InvalidOperationException` handling boundary.

The Angular Command Center uses an explicit typed provider registry over existing feature API clients. It does not introduce a generic query endpoint or reinterpret business data. Provider availability comes from the bootstrapped session, and each request still crosses the feature endpoint's server authorization/tenant boundary. Its local preference contract is versioned and intentionally restricted to allowlisted navigation/provider identifiers and presentation mode.

Shared form productivity remains presentation orchestration: keyboard save emits the existing submit intent, error focus maps validated field identifiers and safe list preferences store only bounded presentation fields. It does not own DTO mapping, validation rules, authorization, concurrency or persistence. Local business-form drafts are explicitly disallowed in MOD-009 pending a separate classified server-side design.

UI layout state remains in the Core preference boundary, not feature or workspace business state. Versioned theme/density/sidebar/content-width enums apply semantic document data attributes consumed by external SCSS. Responsive behavior can override presentation at small widths, but stored preferences never alter navigation authorization, feature availability or backend behavior. Route and modal focus management remains shell-owned.

Recovery presentation uses a stable internal Problem Details `code` plus HTTP status, not translated-message matching.
Application services still decide validation, duplicate and concurrency outcomes; Web maps them to presentation codes,
and Angular maps those codes to non-destructive actions. The reusable recovery component cannot retry mutations,
override row versions, merge duplicates or grant navigation. See `modernization/PHASE_11_DATA_QUALITY_RECOVERY.md`.

Document transparency is a read-only Application composition boundary. It reuses authoritative Sales Order and
Customer Invoice DTOs, emits typed journey/relationship/number evidence and applies source and target View permissions.
Angular formats and localizes the evidence but never recalculates totals, outstanding balances or relationships.

The work center and workspace profiles remain presentation orchestration. Server services select actionable records
using existing View/action permissions; local profiles can only reorder the resulting allowlisted destinations. The
Angular Analytics experience reuses the generic provider boundary and accepts only server datasets and allowlisted
drill targets. See modernization phases 13-15.

Background operation observation uses an Application interface implemented by a Web singleton because hosted-worker
runtime state belongs to the process, not the database or Domain. Workers publish start/success/failure timestamps only;
the Application service applies user permissions and adds tenant-owned analytics refresh-plan metadata. This state is
not durable scheduling evidence and raw exceptions never cross the API. See `modernization/PHASE_16_BACKGROUND_OPERATIONS.md`.

## Reusable analytics boundary (2026-10-22)

The central Analytics Center depends only on generic Application DTOs and `IAnalyticsModuleProvider`. Each module owns its calculations and exposes stable dimensions, filters and widget keys; the center owns workspace persistence, layout, date validation and previous-period/year comparison. Sales and Purchasing are registrations, not dependencies of the controller or generic views. Saved workspaces store presentation/query configuration rather than copied analytical facts. See `decisions/ADR-20261022-REUSABLE-ANALYTICS-CENTER.md`.

## Central document rendering boundary (2026-10-22)

All printable ERP features use the Application-layer `IHtmlToPdfEngine` contract. Infrastructure supplies one singleton `PlaywrightPdfEngine` that owns browser reuse, serialization of rendering calls, paper sizing, logical margins, background printing and page headers/footers. Feature-specific composers, beginning with sales quotations, build encoded HTML and pass only a neutral render request to that engine. This prevents each invoice, order, receipt or report from launching and configuring its own PDF stack. Windows development uses installed Microsoft Edge; Linux/container deployment must install the matching Playwright Chromium runtime. See `decisions/ADR-20261022-CENTRAL-REUSABLE-PDF-AND-FROZEN-QUOTATIONS.md`.
> Status: IMPLEMENTED WITH INCONSISTENCIES  
> Source of truth: Code  
> Last reviewed: 2026-10-15

## Solution and dependency direction
`MiniStore.Domain` contains entities, enums, commands and repository/service interfaces. `MiniStore.Application` references Domain and contains DTOs plus use-case services. `MiniStore.Infrastructure` references Application and Domain and implements EF Core persistence, repositories, seeders, authorization service and Unit of Work. `MiniStore.Web` references Application and Infrastructure and is the MVC presentation/composition root.

`Web → Application → Domain`; `Infrastructure → Application + Domain`. EF configuration lives in Infrastructure. `Program.cs` remains the composition entry point but delegates registrations to cohesive extensions under `MiniStore.Web/Configuration`: presentation, persistence, security and feature-grouped business services. Startup seeding and middleware/route composition are isolated there as well.

Source files are grouped by business feature inside each layer. Entities, service contracts, services, repositories, EF configurations and controllers use the shared feature names Accounting, Catalog, Customers, Inventory, Purchases, Sales, Security, Settings and Suppliers; Web also contains Home. Shared infrastructure stays at the layer root or in a `Shared` folder. Razor views retain the MVC `Views/<ControllerName>/` convention and migrations retain chronological ordering. See `decisions/2026-09-13-feature-folder-organization.md`.

Inventory behavior is policy-driven at the warehouse boundary. Operational use is separate from Simple/LocationManaged/Hybrid control, and global InventorySettings provide defaults rather than overriding an established warehouse. See `decisions/2026-09-14-configurable-inventory-operating-policies.md`.

The approved WMS evolution preserves business documents and warehouse AVCO while separating future physical movements and dimensional balance projections. `StockTransfer` remains the approval document; `ProductStock` remains the warehouse valuation authority; a new physical movement ledger and balance projection will be introduced additively with dual-write reconciliation rather than replacing existing history. See `WMS_EVOLUTION_PLAN.md` and `decisions/2026-09-30-wms-evolution-strategy.md`. These are target architecture, not implemented code.

Warehouse access is layered: branch permissions define the organizational boundary and POS terminal mappings narrow that list. See `decisions/2026-09-14-branch-and-pos-warehouse-access.md`.

POS appearance is composed from a business-profile preset and persisted per-terminal overrides. The setting belongs to the terminal rather than the company or branch because two devices in one branch may serve different workflows. See `decisions/2026-09-14-configurable-pos-experience.md`.

POS order context is stored on the completed sale and its items, while the terminal setting controls which context the operator may submit. This separates durable transaction facts from configurable device behavior. See `decisions/2026-09-14-pos-order-context.md`.

Presentation localization uses ASP.NET request localization with English fallback keys and centralized Arabic resources. Culture precedence is user cookie, cached company default, then English. Domain state stores only a language enum; translated text remains in Web resources. See `decisions/2026-09-14-bilingual-localization.md`.

SaaS data uses one SQL Server database with a required tenant owner key on every business entity. The authenticated tenant claim is accepted only while an active user membership and active tenant exist. EF global query filters scope reads, SaveChanges stamps inserts and rejects cross-tenant mutations, tenant-aware unique indexes allow repeated business codes across companies, and composite `(business FK, TenantId)` relationships make SQL Server reject references across companies. `TenantIsolationModel` centrally classifies mapped Domain types and the regression suite rejects omissions. Tenant membership and role assignments are explicit control-plane records. Subscription middleware blocks ERP routes when the active company's lifecycle does not allow use. Public, tenant and `/platform` experiences have separate route/authentication boundaries. See `decisions/2026-09-14-tenant-data-isolation.md`, `decisions/2026-09-14-saas-control-plane.md` and `decisions/2026-09-15-tenant-relationship-hardening.md`.

## Request lifecycle
Browser → MVC controller → DTO model binding/ModelState → application service → repository/domain entity → `AppDbContext`/SQL Server → redirect or Razor view. AutoValidateAntiforgeryToken is registered globally. Sales, purchases and transfers use `IUnitOfWork`; several master-data services use repository `SaveChangesAsync` directly.

## Actual patterns
- Domain entities use private setters and behaviour methods.
- Manual DTO mapping is used; AutoMapper is not referenced.
- Repository interfaces are in Domain; implementations are in Infrastructure.
- MVC controllers use application services for role administration; remaining direct repository/controller dependencies should still be reduced when those modules change.
- No validators, CQRS query objects, middleware classes, public API controllers, background jobs, or tests were found.

## Dependency injection
See `08_CONFIGURATION.md`. `Program.cs` is the concise composition entry point and calls the configuration extensions; each service mapping is registered once. Feature registrations remain explicit and searchable rather than using assembly scanning.

## Architectural risks
- Role administration follows Controller → `TenantRoleService` → repository. Identity owns credentials while tenant roles, permissions and assignments form the company authorization boundary.
- Several repositories expose `SaveChangesAsync`, creating inconsistent transaction ownership.
- `PermissionsCodeExport/` is a duplicate export tree, not included by the solution projects; it can drift from active code.

## Change impact
Entity → EF configuration/migration → repository → service → DTO → controller/view → module/entity/database docs. Service → interface/dependencies/controllers/views/tests. See `development/DOCUMENTATION_MATRIX.md`.

2026-09-13 security policy: identity mutations now use the shared Application `AdministrationPermissions` rule in existing permission evaluators. Login limiting uses built-in ASP.NET Core middleware. See `decisions/2026-09-13-admin-mutations.md`; no additional transaction mechanism or database schema was introduced.

## Purchase-management evolution boundary

The procure-to-pay architecture is defined in `PURCHASE_MANAGEMENT_EVOLUTION_PLAN.md` and the accepted ADRs `decisions/2026-10-08-additive-procure-to-pay-evolution.md` and `decisions/2026-10-15-retire-legacy-direct-purchase-creation.md`. The `Purchase` aggregate remains a historical Legacy Direct Purchase because it combines supplier invoice capture and immediate inventory receipt, but PUR-280 retires new creation. New activity must use Purchase Request, sourcing, Purchase Order, Goods Receipt and Vendor Bill boundaries through the existing Domain/Application/Infrastructure/Web layers; controllers must not orchestrate inventory or accounting persistence directly.

`PURCHASE_ARCHITECTURE_ALIGNMENT.md` fixes the cross-context boundary before implementation: Purchasing owns demand/sourcing/orders, Inventory owns physical receipt effects and stock truth, and Accounting/AP owns vendor bills, payables, payments and journals. Goods Receipt is orchestrated against the PO through Application contracts while all physical mutation remains inside Inventory services. Supplier remains the current purchasing party; a generic Party model, generic approval engine and asynchronous event infrastructure are deferred until separately justified.

PUR-240 introduces the first bounded background planner. The web-hosted scanner owns only scheduling and tenant-scope creation; `ReplenishmentAutomationService` owns orchestration, domain entities own validated schedule/run/evidence state, repositories own persistence and `UnitOfWork` remains the transaction boundary. See `decisions/ADR-20261014-AUTOMATED-REPLENISHMENT-TENANT-SCOPE.md`.

## Sales-evolution boundary

`SALES_ARCHITECTURE_AUDIT_AND_IMPLEMENTATION_PLAN.md` records the completed `SAL-000` audit. The current `Sale` aggregate is a compatible immediate sale/invoice/payment/stock workflow and remains authoritative for its existing history; it is not reinterpreted as a quotation, Sales Order, Delivery, receivable Invoice or allocated Payment. The additive evolution reuses Catalog, Inventory reservation/valuation, central numbering, tenant permissions and the journal gateway. The accepted `SAL-005` gate in `SALES_ARCHITECTURE_ALIGNMENT.md` fixes context ownership, delivered-versus-ordered invoicing policy, delivery-time COGS, dated currency snapshots, full-reservation default and continued immediate POS compatibility before `SAL-010` begins.

SAL-050 keeps InventoryReservation as the only Reserved-balance mutation authority while SalesOrderReservationService owns order-line limits and shortage policy. StockTransfer retains one tenant-unique source record; a SalesOrder may have multiple immutable allocation records for partial release/reallocation, each linked to its exact source line. See `decisions/2026-10-20-sales-order-reservation-allocation.md`.

SAL-060 keeps `SalesDeliveryService` as the fulfillment coordinator while Inventory services remain the only stock/location/tracking mutation authorities and `JournalPostingService` remains the accounting gateway. The delivery aggregate stores immutable source and cost evidence, but does not duplicate balance logic or create invoice/payment state. See `decisions/2026-10-20-sales-delivery-reservation-cost-boundary.md`.

SAL-070/080 keep `CustomerInvoiceService` as the billing and posting coordinator without bypassing Application boundaries. It reads order/delivery/customer/account masters through repositories, freezes invoice/account/company-currency evidence, derives branch/warehouse journal dimensions from exact delivery allocations and atomically calls the shared `JournalPostingService`. Accounting posting/reversal never calls an inventory mutation authority. `JournalEntryQueryService` separately projects read-only evidence. See `decisions/2026-10-20-customer-invoice-eligibility-and-posting-boundary.md` and `decisions/2026-10-21-customer-invoice-accounting-posting.md`.

SAL-090 keeps `CustomerPaymentService` as the collection coordinator. It reads invoices, customers, methods, rates and accounts through repositories, calculates capped allocation and carrying values inside the Serializable UnitOfWork, and calls the shared journal gateway. Invoice balances remain projections of Posted allocations; reversal preserves rows and excludes them by status. No controller accesses EF and no collection path calls inventory authorities. See `decisions/2026-10-21-customer-payment-allocation.md`.

## ERP SaaS foundation reference boundary

`reference/erp-saas-foundation/` is a proposed cross-cutting architecture and operations reference. MiniStore remains a modular monolith; the package does not authorize a public API, microservices, broker, distributed cache, SQL row-level security, object storage or deployment stamps. Those remain ADR triggers that require measured need. The repository-specific `MINISTORE_COMPATIBILITY.md` preserves existing tenant, finance, inventory, purchase approval, guidance and SaaS control-plane implementations while ordering remaining foundation work from operational recovery evidence through audit, performance, provisioning, exceptions/work and safe import/export.

## Audit-security evolution boundary

`AUDIT_SECURITY_ARCHITECTURE_AND_IMPLEMENTATION_PLAN.md` and `decisions/2026-10-15-audit-event-v2-boundary.md` define the accepted documentation-only `P1-AUD-010` boundary. A later additive `AuditEvent` model will carry semantic actor, scope, outcome, reason, correlation, redacted changes and integrity evidence through an Application-owned writer implemented by Infrastructure. Successful mutation events share the business UnitOfWork; denied/failed security attempts use a distinct bounded path and never claim a committed mutation.

The existing EF interceptor and 707 Demo `AuditLogs` remain legacy compatibility evidence and are not reinterpreted or backfilled. Accounting journals, inventory movements and specialized document histories remain authoritative for amounts, quantities and lifecycle facts. `P1-AUD-011` implements the additive mixed-scope entity, schema, writer, redactor and keyed integrity service. `P1-AUD-012` keeps semantic construction in Application services, obtains request correlation through an Application contract implemented by Web, and persists enrolled role/permission and sensitive-setting events in the same transaction/save as their business change. Authentication/platform/external coverage, reading permissions/screens, retention and external checkpoints remain gated by `P1-AUD-013..016`.
