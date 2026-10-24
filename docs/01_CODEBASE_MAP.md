# Codebase map
## Angular modernization foundation

The in-repository workspace `MiniStore Front-End` is the isolated Angular 22 presentation target. It uses standalone components, strict TypeScript, lazy routes, SCSS design tokens, an Arabic RTL/English LTR localization boundary, a responsive shell and a small Signals-owned workspace state contract. Its central API clients bootstrap the user/tenant/permission session, normalize Problem Details and attach the server-issued CSRF token to unsafe internal API calls. Its development proxy is limited to `/api` and `/Account`.

`MiniStore.Web/Controllers/Api/V1` exposes the authenticated session and CSRF foundation. `InternalApiSessionService` composes explicit Application DTOs through tenant and permission boundaries. `InternalApiExceptionMiddleware`, cookie events and API behavior options keep errors JSON-based with correlation IDs. Authenticated OpenAPI is filtered to `/api/v1`; reusable bounded paging contracts live in `MiniStore.Application/DTOs/Api`. MVC remains operational and Phase 2 adds no database migration or business-rule change. See `docs/modernization/PHASE_02_API_FOUNDATION.md`.

`MiniStore Front-End/src/app/core/preferences` owns versioned, non-sensitive theme/density preferences and system-theme resolution. Global semantic tokens provide light/dark palettes, density dimensions, focus, reduced-motion and forced-color behavior. The lazy `/design-system` page is the bilingual RTL/LTR visual-state reference for shared buttons, badges, fields and empty/loading/error patterns. See `docs/modernization/PHASE_03_VISUAL_SYSTEM.md`.

`MiniStore Front-End/src/app/workspace` owns the reusable Angular workspace engine. Its navigation registry is session-permission aware and is also the restoration allowlist; its Signals state provides stable context identities, deduplication, bounded per-tab history, configurable tab/recent limits, safe versioned restoration, close/reopen and dirty-state protection. The shell supplies localized ARIA tabs, direction-aware keyboard movement and explicit unsaved-change decisions. No permissions or business payloads are persisted. See `docs/modernization/PHASE_04_WORKSPACE_ENGINE.md`.

`MiniStore.Web/Controllers/Api/V1/CustomersController.cs` exposes permission-separated Customer list/details/form/create/update contracts through the existing `CustomerService`; it does not access EF or repositories. `MiniStore Front-End/src/app/features/customers` is the first business vertical slice, with an isolated typed client, responsive list/details/create/edit pages, external SCSS, Problem Details field feedback, row-version concurrency and workspace dirty-state integration. MVC Customers remains the rollback path. See `docs/modernization/PHASE_05_CUSTOMERS_VERTICAL_SLICE.md`.

## SAL-130A reusable Analytics Center

`AnalyticsWorkspace` and its widgets persist tenant-owned personal/shared presentation preferences. Generic analytics
DTOs, `IAnalyticsModuleProvider` and `AnalyticsCenterService` form the reusable Application boundary; Sales and
Purchasing register as the first providers. `AnalyticsCenterController` and two generic Razor screens render live or
saved workspaces without depending on a module DTO. Central comparison re-executes the selected provider for the
previous period/year. Migration 135 adds the isolated workspace schema. See `modules/analytics.md` and the Analytics
Center ADR.

## SAL-120B central document identity and designer

`GeneralSettings` owns reusable bilingual trade/legal identity, document-name policy and a bounded logo. `OrganizationIdentityField` adds ordered bilingual custom facts with sales/purchase/receipt/footer visibility. The tenant-owned `DocumentDesign` aggregate adds document type, versioned Draft/Active/Archived lifecycle, per-type default, paper/language/RTL-ready layout, brand colors, font/header/logo/name placement, identity visibility, cover/header/footer/watermark text and up to 30 ordered bilingual sections. `DocumentDesignService`, repository/configuration, administration controller and responsive live-preview editor keep document branding in Settings rather than one sales screen. `IHtmlToPdfEngine` and `PlaywrightPdfEngine` form one reusable HTML-to-PDF boundary for every ERP module; the quotation composer supplies encoded feature-specific HTML. Sending a quotation now freezes the full effective identity, design, content and logo in a SHA-256 protected snapshot, while Draft PDF remains a live preview. Migrations 133-134 contain the additive designer and snapshot schema. See the three SAL-120B ADRs.

## SAL-120A advanced quotation templates

Adds `SalesQuotationTemplate` and item entities, repository contract/implementation, EF configuration and migration 132; `SalesQuotationTemplateService` owns draft creation, version cloning, activation, archive and default selection. `SalesQuotationService` applies only an active exact version, enforces required/optional/alternative and option-group rules, reprices selected lines and freezes the template evidence. `SalesQuotationTemplatesController` plus three Razor screens provide bilingual management; quotation create/detail screens apply and display the result. The competitive extension plan is `SAL120_ADVANCED_QUOTATION_TEMPLATES_PLAN.md`.

## SAL-110 configurable sales controls

Adds `SalesApprovalRule` and sequential step configuration plus immutable order review, finding and decision evidence. `SalesOrderApprovalService` evaluates company-wide or customer-specific order amount, credit exposure, overdue days, discount and margin thresholds and applies Warn, RequireApproval or Block. Rules support effective dates, priority, activation, requester self-approval policy and distinct-approver policy. `SalesApprovalRulesController` and its bilingual responsive screen manage rules; Sales Order details expose the frozen review and authorized decisions. Migration 131 adds the tenant-isolated schema.

## SAL-100D customer credit notes

Adds the tenant-owned `CustomerCreditNote` aggregate and lines, repository, DTOs and `CustomerCreditNoteService`. Posting starts only from Posted customer-return receipts, caps each quantity by the exact invoice line, delivery allocation and still-uncredited receipt quantity, and uses a tenant-unique request key to make retries idempotent. It debits frozen sales revenue/output tax and credits frozen receivables through `JournalPostingService`; invoice and payment balances subtract posted credits. It does not mutate stock, tracking, movements or cash. `CustomerCreditNotesController`, three Razor screens, two permissions and migration 130 expose the controlled flow.

## SAL-100C customer return disposition posting

Extends the receipt aggregate with Posted audit evidence; `CustomerReturnReceiptService` coordinates stock, location, tracking, movement, transaction and journal authorities. `CustomerReturnReceiptsController` and Details expose a dedicated permission and immutable result evidence. Migration 129 is additive; regression and disposable SQL scenarios cover the domain/authorization and full Restock path respectively.

## SAL-100A customer return authorization

Domain aggregate/repository contract, Application DTO/service, Infrastructure repository/configuration/migration and Web controller/views/navigation/localization form the slice. Regression, ADR and operation evidence document it.

## SAL-100B customer return receipt and inspection

Adds receipt/line/history domain types, repository and service, CRR sequence, four permissions, three Razor screens, migration 128 and source-delivery tracked-identity validation. The inspection-before-stock ADR fixes its authority boundary.

> Status: IMPLEMENTED  
> Source of truth: Repository scan  
> Last reviewed: 2026-10-24

## Solution roots
| Path | Project/layer | Purpose |
|---|---|---|
| `README.md` | Repository overview | GitHub-facing product summary, setup, verification, roadmap and documentation entry points. |
| `MiniStore.sln`, `MiniStore.slnx` | Solution | Four active projects. |
| `MiniStore.Domain/` | Domain | Entities, enums, commands, interfaces. |
| `MiniStore.Application/` | Application | DTOs, services, permission definitions. |
| `MiniStore.Infrastructure/` | Infrastructure | EF DbContext/configurations/migrations/repos/seeders. |
| `MiniStore.Web/` | Presentation | MVC controllers, Razor views, auth/navigation/runtime composition. |
| `PermissionsCodeExport/` | Uncompiled export | Duplicate code snapshot; not referenced by active projects. |
| `tests/PerformanceSmoke/` | Local operations test | Dependency-free HTTP workload with JSON p50/p95/p99 evidence and enforced limits. |
| `scripts/verify-local.ps1` | Local verification gate | Builds, runs regression and disposable SQL integration, probes readiness and executes the workload. |

`docs/ARABIC_SHARED_ROADMAP.md` is the shared Arabic execution roadmap. It records ordered ERP/SaaS work, business rules, workflow, task status, completion evidence and the rolling five-suggestion queue.

`docs/MASTER_IMPLEMENTATION_ROADMAP_AR_EN.md` is the approved Arabic/English master sequence. It orders stabilization, security, inventory/accounting reconciliation, controlled reversals, financial reporting, company and user administration, customer management, pricing, sales, receivables, returns, warehouse completion, POS, data exchange, integrations, analytics, verification and production launch. It is a planning authority, not evidence that a planned capability is implemented.

`docs/README.md` is the documentation index and recommended reading order for contributors.

`docs/ACCOUNTING_REFERENCE_AR.md` is the shared Arabic accounting and software-design reference. It covers the accounting cycle, example postings, inventory valuation, sales/purchases, close, advanced topics, international-standard mapping, implementation invariants and the ordered accounting delivery plan. It is guidance rather than evidence that a feature is implemented.

`docs/WMS_EVOLUTION_PLAN.md` is the approved WMS evolution plan. It records the current inventory model, gap matrix, target authority boundaries, migration safeguards, phased vertical slices and anticipated file impact. WMS-000 reconciliation is implemented; later WMS entities remain planned unless explicitly marked complete.

`docs/PURCHASE_MANAGEMENT_EVOLUTION_PLAN.md` is the approved procure-to-pay evolution plan. It records the legacy direct-purchase boundary, capability gaps, target aggregates, inventory/accounting integration contracts, additive migration safeguards and PUR-000 through PUR-150 vertical slices. Planned purchase types are not implemented unless the matching slice is explicitly marked complete.

`docs/PURCHASE_COMPLETION_ROADMAP_AR_EN.md` is the approved Arabic/English post-PUR-150 purchase plan. PUR-160 through PUR-280 are implemented, including advanced analytics and retirement of new Legacy Direct Purchase creation.

`docs/decisions/2026-10-14-immutable-supplier-price-snapshots.md` fixes the append-only full-snapshot authority for supplier-price history across manual, bulk, CSV and quantity-break changes.

`docs/decisions/2026-10-15-retire-legacy-direct-purchase-creation.md` retires new Legacy Direct Purchase creation after PUR-280 while preserving historical records and directing new activity to Purchase Requests.

`docs/PURCHASE_ARCHITECTURE_ALIGNMENT.md` is the accepted PUR-005 gate. It fixes Supplier compatibility, Purchasing/Inventory/Accounting ownership, derived quantity authorities, staged currency behavior, purchase-first approval boundaries, GRNI, synchronous contracts and deferred event infrastructure before runtime implementation starts.

`docs/SALES_ARCHITECTURE_AUDIT_AND_IMPLEMENTATION_PLAN.md` is the completed `SAL-000` repository audit and planning authority. It classifies the current `Sale` flow as a compatible immediate-sale/invoice/payment/stock workflow, preserves it, and defines the additive customer, pricing, quotation, sales-order, reservation, delivery, invoicing, receivables, payment, return and cutover sequence. It is not evidence that the planned Sales aggregates are implemented.

`docs/SALES_ARCHITECTURE_ALIGNMENT.md` and `docs/decisions/2026-10-15-sales-lifecycle-context-ownership.md` are the accepted documentation-only `SAL-005` gate. They fix legacy Sale/POS compatibility, context and quantity authority, full-reservation default, delivery-time stock/COGS, policy-based invoicing, receivable/payment separation, currency snapshots and deferred generalizations before `SAL-010` begins.

`docs/CUSTOMER_AND_BUSINESS_PARTNER_AUDIT_AND_IMPLEMENTATION_PLAN.md` is the completed documentation-only `CUS-000` customer and business-partner audit. It records the minimal current Customer implementation, compatibility with existing Customer/Supplier IDs, the independent BusinessPartner decision gate, additive migration safeguards, integration ownership, test strategy and the ordered `CUS-005` through `CUS-110` plan. It is not evidence that the planned customer capabilities are implemented.

`docs/decisions/ADR-20261018-CUSTOMER-PARTNER-IDENTITY.md` closes `CUS-005`: CustomerId and SupplierId remain
separate compatibility identities, CUS-010 expands Customer additively, and actual BusinessPartner unification is
deferred to a separately approved CUS-110 gate.

`docs/decisions/ADR-20261019-CONFIGURABLE-POLICY-SAFE-HISTORY.md` establishes configurable policies with immutable posted history and controlled transitions; CUS-030A is its first customer-accounting implementation.

`docs/decisions/ADR-20261019-DETERMINISTIC-SALES-PRICING-TAX-UOM.md` closes the SAL-020A design gate: price-rule precedence, unit normalization, dated currency conversion, rounding order, tax/exemption snapshots and the explainable pricing-result contract are fixed before runtime price-list storage is introduced.

`docs/reference/erp-architecture/` preserves the imported cross-ERP proposed architecture package: system charter, module and ownership maps, workflows, integration contracts, cross-cutting rules, delivery roadmap, agent guidance, test matrix and open decisions. It is a planning reference rather than evidence of implemented code, and its Codex guidance does not supersede repository `AGENTS.md`, accepted ADRs or the required documentation workflow.

`docs/reference/erp-saas-foundation/` installs the imported ERP SaaS Foundation Blueprint with provenance hashes, archived non-authoritative source instructions, the original tenant/audit/integrity/DR/approval/configuration/billing/exception/work/import references and an evidence-based MiniStore compatibility overlay. `FND-000` is documentation-only; none of the proposed gaps are implemented by importing the package.

`docs/project-control/` is the consolidated execution-control source for current project status, verified feature inventory, architecture/database/security/business-integrity audits, documentation conflicts, dependencies, one prioritized roadmap, decisions, technical debt, executed test evidence, quick handoff context and exactly one proposed next task. Its `operations/` folder stores secret-free operational evidence for the local Demo credential closure, encrypted backup/restore drill, 112/112 reconciliation baseline and implemented audit-event foundation. It coordinates rather than replaces detailed module plans or historical ADRs.

`docs/AUDIT_SECURITY_ARCHITECTURE_AND_IMPLEMENTATION_PLAN.md` and the audit ADRs define the audit boundary. `P1-AUD-011` through `P1-AUD-015` implement storage, semantic/security coverage, permission-aware reading/export, retention, legal holds, signed checkpoints and database-role separation. `P1-AUD-016` adds `AuditCoverageCatalog`, repeatable `--inspect-audit` reconciliation and independent `LegacyAuditOptions`; committed configuration freezes new generic `AuditLogs` writes while preserving all historical rows and a configuration-only rollback. Specialized accounting/inventory/purchase/sales histories remain authoritative for detailed facts.

`MiniStore.Web/Guidance/ActionableErrorGuidance.cs` maps recognized, user-resolvable purchase, sales-pricing/customer-default and sales-delivery errors to permission-protected setup, inventory or document destinations. The shared notification partial renders the resulting action for both modal notifications and page validation summaries; ordinary validation errors remain message-only.

Customer create/edit Razor forms post the explicit nested `CustomerFormPostDto.Form` contract through `CustomersController`, matching their `Form.*` field names. The form keeps generated `CustomerCode` and policy-resolved `AccountId` nullable at the presentation boundary; `CustomerService` generates the code and requires an account only for manual-selection policy. Invalid submissions rebuild the complete page model and expose localized summary and field-level errors instead of failing silently.

All Application DTO decimal `RangeAttribute` contracts parse their declared limits and submitted values with invariant culture. This prevents Arabic UI culture from throwing during MVC validation for quantities, rates, dimensions, prices and other decimals; regression reflects over the complete Application assembly to enforce the rule.

## Domain source map
| Files | Type/purpose | Consumers/docs |
|---|---|---|
| `Entities/Accounting/*.cs` | chart, branches, journal entries, fiscal periods, tax and payment entities | accounting docs. |
| `Entities/Catalog/{Product,ProductTemplate,ProductCategory,ProductAttributeDefinition,ProductAttributeValue,ProductRecipe,RecipeIngredient,MeasurementUnit}.cs` and product type/unit/dimension/behavior/logistics enums | concrete SKU identity, optional variant template grouping, category-scoped typed attributes/options/values, logistics metadata, tracking/shelf-life policy, sale channels, immutable recipes and managed units | products, sales and inventory docs. |
| `Entities/Customers/{Customer,CustomerContact}.cs`, `Entities/Suppliers/Supplier.cs` | Customer identity, prospective payment/currency/tax defaults, tenant-safe multi-contact and General/Billing/Shipping addresses with defaults, archive and rowversion; suppliers keep their professional profile | customer/sales/purchases/suppliers docs. |
| `Entities/Sales/SalesPriceList.cs` | tenant-owned price lists and deterministic all-product/category/product rules with quantity, unit, date, currency, rounding and archive controls | SAL-020B pricing service/screen and sales docs. |
| `Entities/Sales/{SalesQuotation,SalesQuotationTemplate}.cs` | SAL-030 quotation snapshots/history plus SAL-120A versioned configurable templates and product-option policy | quotation/template application services, EF configuration and screens. |
| `Entities/Sales/SalesOrder.cs` | SAL-040 one-per-quotation order, copied commercial lines and confirmation/cancellation history; SAL-050/060 derive reservation and delivery progress without duplicating it on the order | order/reservation/delivery application services, EF configuration and screens. |
| `Entities/Sales/SalesDelivery.cs` | SAL-060 reservation-backed Draft/Posted/Cancelled delivery header, exact-position lines, frozen cost/tracking evidence and semantic history | delivery service, EF configuration, accounting and screens. |
| `Entities/Sales/CustomerInvoice.cs` | SAL-070/080 Draft/Approved/Posted/Reversed/Cancelled invoice, frozen commercial/account/company-currency evidence, exact delivery sources and semantic history | invoice service, EF configuration, accounting and screens. |
| `Entities/Sales/CustomerPayment.cs` | SAL-090 independent Posted/Reversed receipt with immutable method/account/rate and capped invoice-allocation evidence | payment service, EF configuration, accounting and screens. |
| `Entities/Inventory/*.cs` | warehouses, operating-policy enums, AVCO balances, locations, putaway rules, availability, reservations, adjustments, lot/serial tracking with persisted removal-strategy audit, recall communication evidence, legacy history, physical movements and transfers | inventory, accounting and stock-transfer docs. |
| `Entities/Purchases/*.cs`, `Entities/Sales/*.cs` | purchase/supplier-return and sale/customer-return aggregates plus POS experience/order settings | purchase/sales/settings docs. |
| `Entities/Purchases/SupplierProductPurchasingInfo.cs` | supplier-specific product code, compatible purchase unit, base-currency price, lead time, order policy, validity/preference, quantity price breaks and immutable full price-history snapshots | PUR-010/PUR-230 purchasing data screens/services. |
| `Entities/Purchases/PurchaseRequest.cs` | PRQ-numbered internal demand, frozen unit conversion, immutable action history and optional source provenance used by replenishment | PUR-020/PUR-110 purchase-request workflow. |
| `Entities/Purchases/ReplenishmentAutomation.cs` | tenant schedule, idempotent run and immutable suggestion/calculation evidence with generated-document links | PUR-240 automated replenishment. |
| `Entities/Purchases/PurchaseApproval.cs` | Warehouse/priority-scoped approval rules, ordered role steps and immutable per-request decision snapshots | PUR-025 approval workflow foundation. |
| `Entities/Purchases/PurchaseSourcingEvent.cs` | RFX-numbered approved-request or direct sourcing event, frozen policy/commercial/line snapshots, supplier invitations and bounded database attachments; no inventory or accounting authority | PUR-030 and PUR-200 sourcing workflows. |
| `Entities/Purchases/{SupplierQuotation,PurchaseQuotationAward}.cs` | supplier-specific frozen quote header/lines plus tenant-owned line/quantity award allocations with immutable supplier, price, currency, lead-time and reason evidence | PUR-040 and PUR-210 quotation workflows. |
| `Entities/Purchases/PurchaseOrder.cs` | awarded-quotation Purchase Order, immutable commercial lines, demand ownership, Warehouse/Dropship fulfillment, customer-delivery evidence and lifecycle history | PUR-050/PUR-165/PUR-250B. |
| `Entities/Purchases/SupplierCommunication.cs` | tenant-owned RFQ/PO bilingual message snapshot, attachments and evidence-based delivery/reply/follow-up history | PUR-260 supplier communication screens. |
| `Entities/Purchases/SupplierIntegration.cs` | scoped API credentials, expiring portal capabilities, idempotent inbound submissions, reliable outbox state and external security audit | PUR-270 portal/API/UBL integration. |
| `Entities/Purchases/PurchaseAgreement.cs` | awarded-quotation agreements with frozen commercial terms and capped release lines, plus repeat-order templates that generate traced demand | PUR-120 agreement/template workflow. |
| `Entities/Purchases/LandedCost.cs` | immutable receipt-linked landed-cost components and allocation/valuation snapshots | PUR-130 receipt valuation adjustment. |
| `Entities/Purchases/GoodsReceipt.cs` | GRN header/line receipt evidence, frozen PO/receiving-route snapshots, quality outcomes and optional inventory-tracking inputs | PUR-060/PUR-250A receipt workflow. |
| `Entities/Purchases/GoodsReceiptScan` (in `GoodsReceipt.cs`) | immutable tenant-owned barcode/code scan evidence with a unique retry key | PUR-150 Draft Goods Receipt scanning. |
| `Entities/Purchases/GoodsReceiptReturn.cs` | immutable receipt-based supplier-return header/lines with frozen quantity, original receipt value and removed inventory cost | PUR-070 return workflow. |
| `Entities/Purchases/VendorBill.cs` | staged supplier invoice identity, mixed receipt/order/service allocations and explicitly reviewed attachment-extraction suggestions | PUR-080/PUR-220/PUR-260 billing workflow. |
| `Entities/Purchases/SupplierCreditNote.cs` | immutable billed-return header/lines with frozen bill/receipt allocation, tax, current inventory cost and tracking evidence | PUR-170 credit-note workflow. |
| `Entities/Purchases/PurchaseDocumentReversal.cs` | immutable reasoned reversal identity for receipt, bill, payment and landed-cost documents with retry and opposite-journal evidence | PUR-180 reversal workflow. |
| `Entities/Purchases/PurchaseMatch.cs` | tenant matching settings plus immutable bill match runs, line exceptions and reasoned override evidence | PUR-090 three-way matching. |
| `Entities/Purchases/SupplierPayment.cs` | posted supplier-payment aggregate with immutable partial Vendor Bill allocations | PUR-100 payable settlement. |
| `Entities/Settings/CurrencyExchangeRate.cs` | tenant-owned dated supplier-currency rate, source and audit evidence using company-currency-per-unit convention | PUR-190 multi-currency purchasing. |
| `Entities/Settings/*.cs` | accounting, discount, inventory-policy defaults, centralized `DocumentSequence`, general settings, reusable organization identity/custom fields, versioned multi-type document designs and supported UI language | settings/database/localization and SAL-120B docs. |
| `Entities/Security/*.cs` | audit events, mixed-scope legal holds, permission catalogue and tenant-owned role/permission entities | permissions/security docs. |
| `Entities/Tenancy/*.cs` | company tenant, Identity-user membership and tenant-scoped user-role assignments | tenancy/security/database docs. |
| `Entities/Saas/*.cs` | plans, limits, features, subscriptions, checkout sessions, platform operators and promotion codes/redemptions | SaaS module and control center. |
| `Enums/Sales/DiscountType.cs` | percentage/fixed discount enum | sales/settings docs. |
| `Commands/Inventory/*.cs` | transfer service input commands | transfer service. |
| `Interfaces/<Feature>/*.cs` | persistence/service contracts grouped by feature | matching Application/Infrastructure feature. |
| `Interfaces/Shared/IUnitOfWork.cs` | shared transaction boundary | document services and UnitOfWork. |
| `Interfaces/Tenancy/*.cs` | current tenant and membership persistence contracts | Web tenant session and Infrastructure repository. |

## Application source map
| Files | Purpose | Used by |
|---|---|---|
| `Services/<Feature>/*.cs` | use-case services grouped as Accounting, Catalog, Customers, Inventory, Purchases, Sales, Settings and Suppliers | matching MVC controllers. |
| `Services/Customers/CustomerService.cs`, `Dtos/Customers/CustomerDtos.cs` | professional customer profile, receivable-policy assignment, contacts/addresses and validated configured-currency/tax commercial defaults | Customers controller and responsive forms/details. |
| `Services/Sales/SalesPriceListService.cs`, `Dtos/Sales/SalesPriceListDtos.cs` | price-list/rule administration and read-only explainable evaluation across unit conversion, dated currency, deterministic rule selection and tax/customer defaults | SalesPriceLists controller and responsive evaluator. |
| `Services/Sales/{SalesQuotation,SalesQuotationTemplate}Service.cs`, `Dtos/Sales/SalesQuotation*.cs` | idempotent repriced quotation creation plus versioned template administration, option validation, DTO-only projections and concurrency-protected commands | SalesQuotations and SalesQuotationTemplates controllers/views. |
| `Services/Sales/{SalesOrder,SalesOrderReservation}Service.cs`, `Dtos/Sales/SalesOrder*.cs` | one-time accepted-quotation conversion plus DTO-only list/detail, confirmation/cancellation and full/partial reservation/release projections and commands | SalesOrders controller/views and InventoryReservationService. |
| `Services/Purchases/PurchaseApprovalService.cs` | approval-rule administration, submit-time rule resolution, frozen instance creation and role-authorized ordered decisions | Purchase Approval Rules and Purchase Request screens. |
| `Services/Purchases/PurchaseSourcingService.cs` | sends approved-request sourcing or permission/policy-gated direct RFQs, freezing demand, policy and attachment evidence | Purchase Sourcing screens and PUR-200. |
| `Services/Settings/PurchaseSourcingPolicyService.cs` | manages the tenant direct-RFQ amount, role and supplier-status gate | Purchase sourcing policy Settings screen. |
| `Dtos/Settings/WorkflowCatalogDtos.cs`, `Services/Settings/WorkflowCatalogService.cs` | defines 62 ordered ERP procedure steps across setup, customers, sales, purchasing, inventory, accounting and analytics, with responsible/review roles, segregation principles, completion evidence and permission-aware destinations without duplicating transactional rules | Settings workflow catalog. |
| `Dtos/Settings/GeneralSettingsDto.cs`, `Services/Settings/GeneralSettingsService.cs` | separates regional preferences from bilingual organization identity, validates/stores logo content and maps flexible document-visible custom fields without exposing EF entities | Settings general/organization-identity screens and future document renderers. |
| `Dtos/Settings/DocumentDesignDtos.cs`, `Services/Settings/DocumentDesignService.cs` | validates, versions, activates, archives, defaults and projects typed bilingual document layouts without exposing persistence entities | central Document Designs workspace and quotation design selection. |
| `Dtos/Documents/DocumentPdfDtos.cs`, `Services/Documents/IHtmlToPdfEngine.cs` | reusable feature-neutral HTML/PDF request, page and margin contract | single PDF creation boundary shared by future invoices, orders, receipts and reports. |
| `Dtos/Sales/SalesQuotationDocumentDtos.cs`, `Services/Documents/IDocumentPdfRenderer.cs` | frozen quotation document contract and quotation-specific composition boundary | safe bilingual quotation PDF and historical replay. |
| `Services/Purchases/SupplierQuotationService.cs` | validates invited responses, freezes/submits quotations, compares company-currency totals and atomically records exact per-line supplier quantity allocations | Supplier Quotation screens and PUR-210. |
| `Services/Purchases/{PurchaseOrder,PurchaseOrderLifecycle}Service.cs` | creates supplier-grouped orders, retains demand allocation, configures direct delivery, records delivery evidence and derives receipt/delivery/service/bill progress and closure | Purchase Order screens, PUR-165/PUR-210/PUR-250B. |
| `Dtos/Purchases/SupplierCommunicationDtos.cs`, `Services/Purchases/SupplierCommunicationService.cs` | prepares bilingual RFQ/PO mailto content and records only user-evidenced send, delivery, reply and follow-up states | Supplier Communication controller/views and PUR-260. |
| `Dtos/Purchases/SupplierIntegrationDtos.cs`, `Services/Purchases/{SupplierIntegration,SupplierUbl}Service.cs` | creates hash-only external access, enforces supplier scope/idempotency, maps portal/API commands, parses safe UBL evidence and queues reliable delivery | Supplier Integration/API/Portal controllers and PUR-270. |
| `Services/Purchases/PurchaseServiceAcceptanceService.cs` | validates confirmed-order service lines, caps cumulative completion and records immutable service evidence without inventory mutation | Purchase Service Acceptance screens and PUR-220. |
| `Dtos/Purchases/SupplierPurchasingInfoDtos.cs`, `Services/Purchases/{SupplierPurchasingInfoService,SupplierPurchasingCsvParser,SupplierPurchasingExcelParser,SupplierPriceExceptionAnalyzer}.cs` | manages supplier-selected product terms, quantity price breaks/discounts, append-only price snapshots, exception analysis and controlled CSV/native Excel export/import with preview | Supplier Purchasing screens and PUR-010/PUR-230. |
| `Services/Purchases/{PurchaseAgreement,PurchaseTemplate}Service.cs` | creates and activates awarded agreements, caps Draft order releases, saves repeat templates and generates traced Draft Purchase Requests | PUR-120 agreement/template screens. |
| `Services/Purchases/LandedCostService.cs` | allocates receipt costs, splits current inventory from consumed expense, records value-only stock movements and posts a balanced clearing journal | PUR-130 landed-cost screens. |
| `Dtos/Purchases/PurchaseAnalyticsDtos.cs`, `Services/Purchases/PurchaseAnalyticsService.cs` | bounded, filterable and groupable purchasing KPI, trend, supplier, buyer-workload, exception, legacy-comparison and acceptance projections over tenant-filtered repositories | PUR-140/PUR-280 analytics screen. |
| `Dtos/Purchases/PurchaseAccountingReconciliationDtos.cs`, `Services/Purchases/PurchaseAccountingReconciliationService.cs` | rounds and classifies as-of-date warehouse Inventory/GL and receipt-derived GRNI/GL evidence without mutation | PUR-160 reconciliation screen. |
| `Services/Purchases/GoodsReceiptService.cs` | atomically validates remaining PO quantity and coordinates one-step posting or staged receive/inspect/store quality hold with Inventory-owned stock, valuation and tracking effects | Goods Receipt capture, inspection and storage screens. |
| `Services/Inventory/InventoryScanResolver.cs` | workflow-scoped retry-key generation/validation and normalized exact scan tokens, including Goods Receipt scans | inventory and PUR-150 scanning flows. |
| `Services/Purchases/GoodsReceiptReturnService.cs` | caps returns by the posted, unbilled receipt quantity and atomically coordinates inventory removal, tracking, physical movement and GRNI/PPV accounting | Goods Receipt Return screens. |
| `Services/Purchases/VendorBillService.cs` | builds mixed receipt/order/service/direct-delivery bills, manages bounded attachments and review-first local extraction suggestions, then posts only separately Approved bills | Vendor Bill screens and PUR-220/PUR-250B/PUR-260. |
| `Services/Purchases/SupplierCreditNoteService.cs` | caps cumulative billed returns, removes current stock/tracking and atomically reverses payable/input tax/inventory with PPV balancing | Supplier Credit Note screens. |
| `Services/Purchases/PurchaseDocumentReversalService.cs` | enforces typed downstream dependencies and unchanged valuation, then atomically records a retry-safe reversal, physical/value correction, opposite journal and original-state transition | PUR-180 reversal screens and original document details. |
| `Services/Settings/CurrencyExchangeRateService.cs` | manages dated rates and resolves the latest rate on or before each document date; company currency resolves to one | Settings currency screen and PUR-190 document services. |
| `Services/Purchases/SupplierPaymentService.cs` | calculates outstanding Vendor Bill balances and atomically settles supplier payable against a configured cash/bank payment method | Supplier Payment screens. |
| `Services/Settings/PurchaseMatchingSettingsService.cs` | administers tenant quantity/price matching tolerances | Settings Purchase Matching screen. |
| `Services/Catalog/RecipeService.cs` | lists and creates immutable active recipe versions with compatible ingredient units | Recipes controller/views and prepared-product sales. |
| `Services/Catalog/ProductCategoryService.cs` | creates and activates/deactivates tenant product categories used by logistics metadata | ProductCategories controller/view and product forms. |
| `Services/Catalog/ProductAttributeService.cs` | transactionally creates typed attribute definitions, category links and selection options | ProductAttributes controller/view. |
| `Services/Catalog/ProductTemplateService.cs` | groups existing Product SKUs by canonical signature and safely previews/creates at most 50 selection-based combinations from an assigned source SKU | ProductTemplates controller/view and attribute-value updates. |
| `Services/Catalog/VariantCombinationBuilder.cs` | deterministic bounded Cartesian builder that rejects empty selections and excessive variant plans before creation | ProductTemplateService and regression checks. |
| `Services/Settings/MeasurementUnitService.cs` | initializes protected built-ins, manages custom units and performs dimension-safe conversion | Settings/Units plus product and recipe unit selection. |
| `Services/Inventory/{StorageLocation,UnassignedStock,LocationMovement}Service.cs` | hierarchical location administration and validation, warehouse search, putaway, internal relocation and history | inventory controllers. |
| `Services/Inventory/InventoryReconciliationService.cs` | classifies warehouse/location/latest-movement differences with fixed precision tolerances and read-only paging | InventoryReconciliation controller/view. |
| `Services/Inventory/StockMovementService.cs` | idempotently dual-writes Putaway/Relocation facts, plans/posts/reverses transfer outbound-transit-inbound stages and provides read-only visibility | StockMovements controller, location services and StockTransferService. |
| `Services/Inventory/InventoryBalanceService.cs` | read-only OnHand/Reserved/Available projection with product, warehouse and protected virtual Unassigned location display | InventoryBalances controller/view. |
| `Services/Inventory/InventoryReservationService.cs` | central exact-location/Unassigned reserve, partial/full consume, remaining-only release and read-only history orchestration | StockTransferService, SalesOrderReservationService, SalesDeliveryService and InventoryReservations controller/view. |
| `Services/Inventory/InventoryAdjustmentService.cs` | blind count, approval and atomic dimensional/valued variance posting | InventoryAdjustments controller/views. |
| `Services/Inventory/{InventoryTrackingService,InventoryRemovalAllocator}.cs` | opening allocation, policy activation, shelf-life validation/defaulting, live expiration alerts, recall/quarantine, tracked receipt, warehouse-policy removal ordering, transfer identity preservation and trace projection | products, purchases, sales, stock transfers and InventoryTracking controller/view. |
| `Services/Inventory/UntrackedInventoryRemovalService.cs` | reservation-aware policy allocation across exact pickable locations and Unassigned, with atomic location-balance removal for non-tracked issues | sales, recipes, purchase returns and legacy adjustments. |
| `Services/Inventory/PutawayRuleService.cs` | administers product/category/default putaway rules and resolves priority- and capacity-aware active receivable-location suggestions | Unassigned Stock putaway. |
| `Services/Inventory/ReplenishmentService.cs` | manages product/warehouse replenishment policies and reservation-aware reviewed suggestions | Replenishment screen. |
| `Services/Purchases/ReplenishmentPurchaseDemandService.cs` | converts a reviewed purchase-sourced suggestion into one traced Draft Purchase Request, applying valid preferred-supplier unit/minimum/multiple terms | PUR-110 replenishment bridge. |
| `Services/Purchases/{ReplenishmentAutomationService,ReplenishmentDemandCalculator}.cs` | calculates scheduled min/max or tested historical/seasonal purchase demand, subtracts available/incoming supply, prevents active duplicates and groups resulting requests/RFQs | PUR-240 automated replenishment page and worker. |
| `Services/Inventory/{InventoryInsightsService,InventoryActivityClassifier}.cs` | classifies current positive inventory by actual outbound inactivity without mutating stock or valuation | Inventory Insights screen. |
| `Services/Inventory/{InventoryScanningService,InventoryScanResolver}.cs` | exact adjustment/product/source/destination resolution and safe delegation of scanned putaway, relocation and absolute Draft counts | Inventory Scanning screen and owning inventory services. |
| `Services/Settings/InventorySettingsService.cs` | rowversion-protected defaults for new warehouse operating policies | SettingsController inventory screen. |
| `Services/Settings/InventoryAccessService.cs` | branch warehouse permissions/priorities and POS terminal warehouse policies | Settings InventoryAccess and POS validation. |
| `Services/Settings/PosExperienceSettingsService.cs` | profile presets, custom terminal appearance persistence and POS runtime projection | Settings/Pos and Sales/Pos. |
| `Services/Settings/DocumentNumberService.cs` | default creation, validation, administration projection and transactional document-number generation | sales, transfers, purchase posting and Settings/DocumentNumbers. |
| `Services/Security/TenantRoleService.cs` | tenant-local role CRUD, permission validation, protected-role rules and user assignment options | Roles and Users controllers. |
| `Services/Security/{AuditEventContracts,SemanticAuditEventService,AuditRetentionService,AuditCoverageCatalog}.cs` | audit writer/request-context contracts, fixed event codes, 37-control coverage inventory, retention policy/orchestration, legal holds, checkpoints and manifest-before-delete commands | enrolled services, reconciliation and Infrastructure audit implementations. |
| `Services/Accounting/{JournalPostingService,JournalEntryQueryService}.cs` | central source-idempotent posting/exact reversal plus read-only account/branch/warehouse journal evidence | purchase/sales workflows and JournalEntries Details. |
| `Services/Settings/FiscalPeriodService.cs` | non-overlapping fiscal-period administration and central posting-date validation | Settings/FiscalPeriods and JournalPostingService. |
| `Services/Sales/SalePostingService.cs` | idempotent sale revenue, discount and COGS general-ledger posting | Sales/Details and Sales/Post. |
| `Services/Sales/SalesDeliveryService.cs` | reservation-backed delivery reads/Draft creation plus atomic exact-position issue, reservation consumption, physical movement and delivery-time COGS posting | SalesDeliveries controller/views and SalesOrder progress. |
| `Services/Sales/CustomerInvoiceService.cs` | ordered/delivered eligibility plus idempotent Draft, approval/cancellation, company-currency receivable posting, branch/warehouse allocation and exact reversal | CustomerInvoices, JournalEntries and order/delivery progress. |
| `Services/Sales/SalesReturnService.cs` | cumulative quantity control, identifier-aware tracked restocking, proportional refund and atomic inventory/accounting reversal | SalesReturns controller/views. |
| `Services/Purchases/PurchaseReturnService.cs` | cumulative supplier-return control, explicit tracked identity issue, moving-average inventory issue and proportional accounting reversal | PurchaseReturns controller/views. |
| `Services/Purchases/SupplierPurchasingInfoService.cs` | validates and atomically manages supplier-first single/bulk product purchasing terms without stock or accounting effects | SupplierPurchasing controller/view. |
| `Services/Purchases/SupplierProductAssignmentService.cs` | synchronizes many-supplier/many-product selection from both master-data screens through active purchasing-info rows | Suppliers/Products forms and PUR-010. |
| `Services/Sales/DiscountCalculator.cs` | standalone discount calculation helper; no active consumer found by scan | Unknown. |
| `Services/Shared/ICurrentUserService.cs` | current-user application contract | Web CurrentUserService. |
| `Services/Security/AuditEventContracts.cs` | semantic event command plus writer/redactor/integrity/request-context contracts | Infrastructure audit-event services and enrolled Application use cases. |
| `Permissions/{PermissionDefinitions,IPermissionService}.cs` | permission catalogue/contract | seeders/auth/services. |
| `Tenancy/TenantClaimTypes.cs` | trusted tenant claim names shared by login and request session validation | Web authentication. |
| `Dtos/Catalog/Products/*.cs` | product request/display plus filtered/sorted/paged catalogue DTOs | catalog services/controllers/views. |
| `Dtos/Inventory/<ProductStocks|StockTransfers|Warehouses>/*.cs` | inventory request/display DTO families | inventory services/controllers/views. |
| `Dtos/Inventory/LocationMovements/*.cs` | relocation input, lookup and history page models | LocationMovementService/controller/view. |
| `Dtos/<Accounting|Purchases|Sales|Settings|Suppliers>/*.cs` | feature request/display DTOs | matching services/controllers/views. |

## Infrastructure source map
| Files | Purpose | Consumers |
|---|---|---|
| `Persistence/AppDbContext.cs`, `Persistence/TenantIsolationModel.cs` | EF + Identity DbContext/DbSets and business/control-plane/mixed-scope classification; applies tenant and append-only audit guards plus composite tenant relationship convention | all repositories/authorization. |
| `Persistence/UnitOfWork.cs` | Serializable transaction wrapper with an explicit in-transaction flush for workflows that need generated document identities before recording dependent facts | purchase/sale/stock/transfer services. |
| `Persistence/AuditSaveChangesInterceptor.cs` | creates AuditLog rows for tracked changes | registered in Program. |
| `Services/Security/{AuditEvent*,SecurityAuditEventWriter,FileAuditArchiveStore,LegacyAuditOptions}.cs`, security repositories/configurations | atomic writers, separate SQL reader connection, allowlist redaction, HMAC-SHA256 integrity, reversible legacy-writer flag, legal-hold-aware retention and signed external evidence | Application audit contracts, AppDbContext and maintenance worker. |
| `Persistence/{Identity,Permission}Seeder.cs` | default roles/admin and permissions | Program startup. |
| `Persistence/Configurations/Tenancy/TenantConfiguration.cs` | tenant identity, membership keys, Identity relationships and slug uniqueness | AppDbContext/migration. |
| `Persistence/Configurations/<Feature>/*.cs` | per-entity schema mapping grouped by feature | applied automatically by AppDbContext. |
| `Persistence/Configurations/Sales/SalesPriceListConfiguration.cs` | SAL-020B tenant relationships, concurrency and scope-specific rule uniqueness | AppDbContext and migration 119. |
| `Persistence/Configurations/Sales/SalesQuotationConfiguration.cs` | SAL-030 snapshot precision, tenant relationships, uniqueness, rowversion and aggregate ownership | AppDbContext and migration 120. |
| `Persistence/Configurations/Sales/SalesOrderConfiguration.cs` | SAL-040 copied snapshots, tenant/source relationships, one-order uniqueness, rowversion and aggregate ownership | AppDbContext and migration 121. |
| `Persistence/Configurations/Inventory/InventoryReservationConfiguration.cs` | tenant-safe transfer-source uniqueness plus multiple source-line-aware SalesOrder allocations and exact-dimension duplicate guards | AppDbContext and migration 122. |
| `Persistence/Configurations/Sales/SalesDeliveryConfiguration.cs` | SAL-060 tenant-safe order/reservation/line/location ownership, open-Draft and line uniqueness, precision and rowversion | AppDbContext and migration 123. |
| `Persistence/Configurations/Sales/CustomerInvoiceConfiguration.cs` | tenant-safe order/customer/commercial/delivery/account ownership, source/request uniqueness, posting precision/evidence and rowversion | AppDbContext and migrations 124–125. |

Accounting foundation (2026-09-13): migration `AddAccountingFoundation` adds Accounts, Branches, JournalEntries, JournalEntryLines and optional warehouse branch/inventory-account links. See accounting ADR before adding automated posting.
| `Documents/PlaywrightPdfEngine.cs`, `Documents/PlaywrightDocumentPdfRenderer.cs` | singleton Chromium HTML-to-PDF runtime plus encoded bilingual quotation composer | central reusable PDF runtime; Windows uses installed Edge and other deployments install Playwright Chromium. |
| `Migrations/*.cs` and snapshot | schema evolution/model snapshots, currently through `20261022122434_AddFrozenSalesQuotationPdf` (134) | EF tooling, deployment. |

Inventory concurrency update (2026-09-13): ProductStock and StockTransfer include database-generated rowversions; migration `AddInventoryConcurrency` updates SQL Server. UnitOfWork uses Serializable isolation for atomic inventory workflows. Manual stock transactions accept adjustments only, and purchase/sale aggregates reject duplicate product lines. Focused checks live in `tests/SecurityRegression`.

Inventory operating-policy update (2026-09-14): warehouses separate operational type from Simple/LocationManaged/Hybrid control and persist picking, POS, capacity and transfer-location rules. InventorySettings stores defaults for new warehouses; migration `AddInventoryOperatingPolicies` updates the schema.

Branch/POS access update (2026-09-14): `BranchWarehouseAccess` and `PosTerminalWarehouse` provide layered warehouse allow-lists and priorities. Settings manages both levels and POS sale creation validates them. Migration `AddBranchAndPosWarehouseAccess` includes compatible backfill.

POS experience update (2026-09-14): `PosTerminalSettings`, its Sales repository/configuration and Settings application service persist validated per-terminal profile/layout preferences. `Views/Settings/Pos.cshtml` manages them with a live preview and `Views/Sales/Pos.cshtml` applies them at runtime. Migration `AddPosExperienceSettings` backfills existing terminals with Retail defaults.

POS order-context update (2026-09-14): Sale/SaleItem persist order type, service reference, guest count and preparation notes. Terminal settings define allowed/default order types and capture behavior; SaleService validates the submitted context. Migration `AddPosOrderWorkflow` backfills operational preferences from each saved profile.

Warehouse location movement update (2026-09-14): `LocationMovement` and its repository/configuration/service record Putaway and Relocation operations. `LocationMovementsController` and `Views/LocationMovements/Index.cshtml` provide internal movement and searchable history. Migration `AddLocationMovementHistory` creates the audit table and indexes.

Hierarchical location update (2026-09-30): StorageLocation retains its identity and structured labels while adding parent, display name, optional barcode, sequence and receive/pick/reserve/ship/count capabilities. WarehouseLocations supports create/edit and cycle/same-warehouse validation. Its 2026-10-18 P1-ARC-030 slice moved warehouse/parent/location read models into `StorageLocationService` DTO projections and removed direct repository/domain-entity exposure from the controller/views. Migration `AddHierarchicalStorageLocations` is applied to `AHMAD/MiniStoreDb`; the architecture slice needed no migration.
| `Repositories/<Feature>/*.cs` | EF implementations grouped by feature | application services. |
| `Repositories/Catalog/ProductRecipeRepository.cs` | active/versioned recipe persistence with ingredients | RecipeService and SaleService. |
| `Repositories/Catalog/ProductAttributeRepository.cs` | tenant-filtered attribute definition, category-applicability and option persistence | ProductAttributeService. |
| `Repositories/Catalog/ProductTemplateRepository.cs` | tenant-filtered template persistence and duplicate variant-signature checks | ProductTemplateService. |
| `Repositories/Tenancy/TenantMembershipRepository.cs` | active company membership lookup and user list | login, tenant middleware and user administration. |
| `Repositories/Security/TenantRoleRepository.cs` | company-filtered role, permission and assignment persistence | TenantRoleService and authorization. |
| `Repositories/Saas/SaasRepository.cs` | control-plane plan, subscription, entitlement usage, operator and promotion persistence | public pricing and Platform area. |
| `Repositories/Accounting/JournalEntryRepository.cs` | journal source duplicate-posting lookup and persistence | purchase and sale posting services. |
| `Repositories/Accounting/FiscalPeriodRepository.cs` | tenant period lookup, overlap validation and rowversion handling | FiscalPeriodService. |
| `Repositories/Inventory/StockTransactionRepository.cs` | inventory movement history plus source/type lookup for accounting settlement | inventory services and `PurchasePostingService`. |
| `Repositories/Inventory/InventoryReconciliationRepository.cs` | tenant-filtered SQL aggregates for warehouse balances, location allocations, orphan allocations and latest movement snapshots | WMS-000 reconciliation service. |
| `Repositories/Purchases/PurchaseAccountingReconciliationRepository.cs` | tenant-filtered as-of-date Inventory ledger, operational valuation, receipt, return, bill-clearing and GRNI ledger projections | PUR-160 reconciliation service. |
| `Repositories/Inventory/StockMovementRepository.cs` | tenant-filtered physical movement persistence and idempotency lookup | WMS-020 StockMovementService. |
| `Repositories/Inventory/InventoryBalanceRepository.cs` | tenant-filtered current dimensional availability projection reads | WMS-040 InventoryBalanceService. |
| `Repositories/Inventory/InventoryReservationRepository.cs` | tracked dimensional balances plus tenant-filtered source/id reservation aggregate persistence, including multiple SaleOrder allocations | WMS-050/SAL-050 reservation services. |
| `Repositories/Inventory/InventoryAdjustmentRepository.cs` | tenant-filtered adjustment aggregate and current dimensional balance persistence | WMS-060 InventoryAdjustmentService. |
| `Repositories/Inventory/InventoryTrackingRepository.cs` | tenant-filtered tracked balances for application-level removal ordering plus immutable trace history | WMS-070/WMS-080 InventoryTrackingService. |
| `Repositories/Inventory/InventoryRecallRepository.cs` | tenant-filtered recall lifecycle persistence and active-identity guard | InventoryTrackingService recall workflow. |
| `Repositories/Sales/SalesReturnRepository.cs` | immutable return history and original-sale aggregation | `SalesReturnService`. |
| `Repositories/Sales/SalesPriceListRepository.cs` | tenant-filtered price-list/rule loading, code checks and persistence | `SalesPriceListService`. |
| `Repositories/Sales/SalesQuotationRepository.cs` | tenant-filtered quotation list/detail, request-key/number lookup and persistence | `SalesQuotationService`. |
| `Repositories/Sales/SalesQuotationTemplateRepository.cs` | tenant-filtered template search, exact/default/active-version lookup and persistence | `SalesQuotationTemplateService` and quotation application. |
| `Repositories/Sales/SalesOrderRepository.cs` | tenant-filtered order list/detail and source-quotation lookup/persistence | `SalesOrderService` and quotation generated-document link. |
| `Repositories/Sales/SalesDeliveryRepository.cs` | tenant-filtered delivery list/detail, per-order posted progress and open-Draft lookup/persistence | `SalesDeliveryService` and SalesOrder cancellation/progress. |
| `Repositories/Sales/CustomerInvoiceRepository.cs` | tenant-filtered aggregate list/detail, request-key and per-order reads with line/source/history loading | `CustomerInvoiceService`. |
| `Repositories/Purchases/PurchaseReturnRepository.cs` | immutable supplier-return history and original-purchase aggregation | `PurchaseReturnService`. |
| `Repositories/Purchases/SupplierProductPurchasingInfoRepository.cs` | tenant-filtered purchasing-term persistence, supplier/product assignment reads, price-break/history loading, uniqueness and preferred-supplier lookup | purchasing-info and assignment services. |
| `Repositories/Purchases/{SupplierQuotation,PurchaseOrder}Repository.cs` | tenant-filtered quote/allocation and purchase-order persistence, source-event order lookup, one-order-per-quotation enforcement and lifecycle history loading | PUR-040/PUR-050/PUR-165/PUR-210 services. |
| `Repositories/Purchases/GoodsReceiptRepository.cs` | tenant-filtered receipt and posted-receipt quantity reads | PUR-060 receipt service. |
| `Repositories/Purchases/GoodsReceiptReturnRepository.cs` | tenant-filtered receipt-return reads and cumulative returned-quantity evidence | PUR-070 return service. |
| `Repositories/Purchases/VendorBillRepository.cs` | tenant-filtered bill reads, cumulative receipt-line billing and active normalized invoice duplicate checks | PUR-080 bill service. |
| `Repositories/Purchases/SupplierCreditNoteRepository.cs` | tenant-filtered credit history, per-bill documents and credited-amount aggregates used by payment availability | PUR-170 credit and payment services. |
| `Repositories/Purchases/PurchaseDocumentReversalRepository.cs` | tenant-filtered reversal history plus original-document and request-key uniqueness lookups | PUR-180 reversal service. |
| `Repositories/Purchases/PurchaseMatchRepository.cs` | persists match runs/exceptions and the singleton tenant tolerance settings | PUR-090 matching and settings services. |
| `Repositories/Purchases/SupplierPaymentRepository.cs` | reads payment history and aggregates cumulative allocations by Vendor Bill | PUR-100 payment service. |
| `Authorization/PermissionService.cs` | permission check implementation | SaleService. |

## Web source map
| Files | Purpose | Related docs |
|---|---|---|
| `Program.cs`, `Configuration/*.cs`, `appsettings.json`, `Properties/launchSettings.json` | concise composition entry point plus separated presentation, persistence, security, feature DI, startup seeding, middleware and configurable throttling | configuration/architecture. |
| `Authorization/*.cs` | dynamic permission policy and handler | permissions/security. |
| `Services/Security/CurrentUserService.cs` | current Identity user ID for auditing | security/database. |
| `Services/Security/HttpAuditRequestContext.cs` | trusted correlation/route/channel projection with IPv4/IPv6 masking and SHA-256 user-agent fingerprinting | Application semantic and independent security audit services. |
| `Services/Tenancy/HttpTenantContext.cs`, `Middleware/TenantSessionMiddleware.cs` | resolve, validate and refresh the authenticated company boundary | AppDbContext, login and localization. |
| `Controllers/<Feature>/*.cs` | MVC endpoints grouped by business feature; namespaces remain stable | `controllers/*.md`. |
| `Controllers/Catalog/RecipesController.cs` | bilingual recipe list/version editor using Products permissions | recipe views and RecipeService. |
| `Controllers/Catalog/ProductCategoriesController.cs` | bilingual product-category administration using Products View/Edit permissions | ProductCategories view and ProductCategoryService. |
| `Controllers/Catalog/ProductAttributesController.cs` | bilingual typed product-attribute administration using Products View/Edit permissions | ProductAttributes view and ProductAttributeService. |
| `Controllers/Catalog/ProductTemplatesController.cs` | bilingual template creation, existing-SKU assignment and explicit bounded variant preview/generation using Products View/Edit permissions | ProductTemplates view and ProductTemplateService. |
| `Controllers/Sales/SalesPriceListsController.cs`, `Views/SalesPriceLists/Index.cshtml` | permission-separated list/rule mutations and responsive read-only explainable evaluator | SAL-020B pricing service and sales docs. |
| `Controllers/Sales/SalesQuotationsController.cs`, `Views/SalesQuotations/*` | permission-separated Draft creation and quotation lifecycle with responsive immutable detail/history | SAL-030 quotation service, permissions and sales docs. |
| `Controllers/Sales/SalesQuotationTemplatesController.cs`, `Views/SalesQuotationTemplates/*` | permission-separated template search/create/version/activate/archive/default management | SAL-120A template service, permissions and sales docs. |
| `Controllers/Settings/SettingsController.cs`, `Views/Settings/{OrganizationIdentity,_OrganizationIdentityField}.cshtml`, `Models/Settings/OrganizationIdentityFieldRowViewModel.cs` | administration-only bilingual identity editor, secure logo upload/read and repeatable custom-field rows | shared organization identity and SAL-120B foundation. |
| `Controllers/Settings/DocumentDesignsController.cs`, `Views/DocumentDesigns/*.cshtml`, `Models/Settings/DocumentDesignSectionRowViewModel.cs` | administration-only versioned multi-document designer with filters, lifecycle actions, live preview and reorderable bilingual sections | shared rendering policy and SAL-120B quotation integration. |
| `Controllers/Sales/SalesOrdersController.cs`, `Views/SalesOrders/*` | one-time quotation conversion, permission-separated confirmation/cancellation and responsive full/partial reservation/release/history workflow | SAL-040/050 order services, permissions and sales docs. |
| `Controllers/Sales/SalesDeliveriesController.cs`, `Views/SalesDeliveries/*` | permission-separated delivery list/create/detail, Draft cancellation and atomic posting with state-aware completion/next-step guidance | SAL-060 delivery service, permissions, inventory and accounting docs. |
| `Controllers/Sales/CustomerInvoicesController.cs`, `Views/CustomerInvoices/*` | six-permission invoice lifecycle, responsive source/accounting evidence, one-time post and controlled reversal | SAL-070/080 invoice service and accounting docs. |
| `Controllers/Accounting/JournalEntriesController.cs`, `Views/JournalEntries/Details.cshtml` | dedicated-permission read-only source-linked journal detail with responsive dimensional lines | JournalEntryQueryService and accounting docs. |
| `Controllers/Inventory/StockMovementsController.cs` | permission-protected read-only WMS-020 pilot ledger and legacy-link status | StockMovements view and service. |
| `Controllers/Inventory/InventoryBalancesController.cs` | permission-protected read-only dimensional inventory availability | InventoryBalances view and service. |
| `Controllers/Inventory/InventoryReservationsController.cs` | permission-protected read-only reservation history | InventoryReservations view and service. |
| `Controllers/Inventory/InventoryAdjustmentsController.cs` | permission-split count/approve/post/cancel workflow | InventoryAdjustments views and service. |
| `Controllers/Inventory/InventoryTrackingController.cs` | permission-protected opening allocation, tracking report and quarantine/release commands | InventoryTracking view and service. |
| `Controllers/Purchases/SupplierPurchasingController.cs` | permission-split bilingual supplier-first bulk purchasing-data administration | PUR-010 service and `Views/SupplierPurchasing/Index.cshtml`. |
| `Controllers/Purchases/{SupplierQuotations,PurchaseOrders}Controller.cs`, `Views/SupplierQuotations/*`, `Views/PurchaseOrders/*` | quotation capture, responsive per-line split award, generated-order links, Purchase Order progress, state-aware actions, separately permitted remainder close/reopen and immutable history | PUR-040/PUR-050/PUR-165/PUR-210 services and permissions. |
| `Controllers/Purchases/PurchaseServiceAcceptancesController.cs`, `Views/PurchaseServiceAcceptances/*` | permission-protected service-completion capture and immutable list/detail evidence, with no warehouse-stock mutation | PUR-220 service acceptance. |
| `Controllers/Purchases/GoodsReceiptsController.cs`, `Views/GoodsReceipts/Create.cshtml` | permission-protected partial Goods Receipt capture and immediate posting | PUR-060 receipt service. |
| `Views/GoodsReceipts/Scan.cshtml` and Goods Receipt scan actions | Draft receipt creation, exact product scanning, accumulated-quantity review and explicit final posting | PUR-150 receipt service. |
| `Controllers/Purchases/GoodsReceiptReturnsController.cs`, `Views/GoodsReceiptReturns/*.cshtml` | permission-protected receipt-based supplier return capture, history and detail | PUR-070 return service. |
| `Controllers/Purchases/VendorBillsController.cs`, `Views/VendorBills/*.cshtml` | responsive card-based mixed-source Draft entry with attachments, separate approval/override/post actions, history, attachment download and immutable posted detail without horizontal line scrolling | PUR-080/PUR-220 bill service. |
| `Controllers/Purchases/SupplierPurchasingController.cs`, `Views/SupplierPurchasing/*` | supplier-scoped responsive bulk editor, quantity price-break editor, read-only price history, expired/duplicate exception cards, permission-protected CSV/Excel downloads and atomic upload previews | PUR-010/PUR-230 supplier-price management. |
| `Controllers/Purchases/PurchaseReplenishmentController.cs`, `Views/PurchaseReplenishment/Index.cshtml`, `Services/Purchases/ReplenishmentAutomationWorker.cs` | permission-separated responsive settings/history and tenant-scoped scheduled execution | PUR-240 automated replenishment. |
| `Controllers/Purchases/SupplierCreditNotesController.cs`, `Views/SupplierCreditNotes/*.cshtml` | permission-protected responsive billed-return capture, immediate posting, history and immutable detail | PUR-170 credit-note service. |
| `Controllers/Purchases/PurchaseDocumentReversalsController.cs`, `Views/PurchaseDocumentReversals/*.cshtml` | permission-protected responsive reversal confirmation, history and audit detail | PUR-180 reversal service. |
| `Controllers/Purchases/SupplierPaymentsController.cs`, `Views/SupplierPayments/*.cshtml` | permission-protected partial allocation, immediate posting, payment history and immutable detail | PUR-100 payment service. |
| `Controllers/Purchases/{PurchaseAgreements,PurchaseTemplates}Controller.cs`, matching views | permission-split agreement creation/activation/release and repeat-template creation/generation | PUR-120 services. |
| `Controllers/Purchases/LandedCostsController.cs`, `Views/LandedCosts/*` | permission-split landed-cost history, allocation capture and immutable detail | PUR-130 service. |
| `Controllers/Purchases/PurchaseAnalyticsController.cs`, `Views/PurchaseAnalytics/Index.cshtml` | permission-protected responsive purchasing dashboard with KPI sources, trends, drill-through exceptions, buyer workload and final lifecycle acceptance | PUR-140/PUR-280 analytics service. |
| `Controllers/Purchases/PurchaseAccountingReconciliationController.cs`, `Views/PurchaseAccountingReconciliation/Index.cshtml` | permission-protected read-only Inventory/GL and GRNI/GL reconciliation with responsive evidence cards and setup/source links | PUR-160 reconciliation service. |
| `Views/Settings/PurchaseMatching.cshtml` | administration screen for quantity and price tolerance percentages | PUR-090 settings service. |
| `Views/Settings/WorkflowCatalog.cshtml` | responsive bilingual 62-step catalog grouped into seven ERP modules and phases, with module navigation, step owner, reviewer, control principle, completion evidence and authorized destination | `WorkflowCatalogService`; Administration access. |
| `Areas/Platform/*` | separately authenticated platform-owner control center for plans, companies/subscriptions, promotion codes and pending payment confirmations | SaaS module. |
| `Controllers/PublicController.cs`, `Controllers/Saas/SubscriptionController.cs` | public landing/pricing and tenant subscription/checkout flows | SaaS module and public/subscription views. |
| `Middleware/SubscriptionAccessMiddleware.cs` | blocks tenant ERP access when the current subscription is not usable | SaaS module/security. |
| `Localization/*.cs`, `Resources/SharedResource.ar.resx` | supported cultures, cached database-default provider and centralized Arabic translations | shared layout and localized views/controllers. |
| `Formatting/NumberDisplayExtensions.cs` | culture-aware display and invariant HTML-input formatting with settings-controlled maximum precision and no insignificant trailing zeros | purchase and settings screens. |
| `Guidance/{ActionableErrorGuidance,WorkflowNextStepGuidance,WorkflowCompletionFilter}.cs` | permission-aware setup/recovery links for recognized purchase and sales-delivery errors plus centralized next-step prompts and automatic success fallback for workflow POST redirects | shared notification partial, presentation registration and focused regression checks. |
| Purchase workflow detail/comparison view models and Razor actions | state-aware one-time actions, completed-state labels, downstream document links and outstanding Vendor Bill balances | purchase repositories/services, localized views and regression checks. |
| `Controllers/Home/{Dashboard,Home}Controller.cs` | authenticated Dashboard application-launcher entry and legacy Home redirect | Dashboard definitions/view and login flow. |
| `Navigation/*.cs` | navigation metadata plus permission-aware Dashboard application-launcher definitions | permissions/screens/dashboard module. |
| `Views/<Module>/*.cshtml` | Razor screens/forms/client JS | `screens/*.md`. |
| `Views/Shared/*.cshtml` | localized LTR/RTL layout, language switch, notifications, validation/error/delete partials | screens/shared-ui.md. |

Localization update (2026-09-14): ASP.NET request localization supports `en-US` and `ar-JO`; GeneralSettings provides the cached company default and a whitelisted cookie endpoint provides user override. Shared layout/navigation/auth/dialogs and core Settings pages use `SharedResource`; migration `AddLocalizationSettings` backfills English.

Tenant isolation update (2026-09-14): `Tenant`, `TenantMembership`, tenant context and session middleware establish a shared-database company boundary. AppDbContext adds TenantId/query filters/write guards to 34 business entities and changes business uniqueness to tenant-aware indexes. Migration `AddTenantIsolation` backfills the Demo Company and current users.

SaaS control-plane update (2026-09-14): public product/pricing pages, separate Platform cookie and operator allow-list, plan features/limits, tenant subscription lifecycle, warehouse/user limit enforcement and time/usage/plan-bounded promotion codes. Migration `AddSaasControlPlane` seeds three bilingual plans and a 30-day Professional trial for existing tenants.

SaaS onboarding/billing update (2026-09-15): company self-registration creates the Identity owner, tenant membership, tenant-scoped Admin assignment and 14-day trial atomically. Tenant checkout stores a priced monthly/annual session, applies a valid one-use-per-company promotion and activates the subscription only after protected platform confirmation. `SubscriptionAccessMiddleware` enforces lifecycle access. Migrations `AddBillingCheckout`, `AddTenantScopedRoles` and `BootstrapPlatformOwner` are applied locally.

Local Demo operator update (2026-10-17): the rotated Demo administrator was promoted through the existing opt-in `PlatformOperatorSeeder` to one active `PlatformOperatorRole.Owner` record. The temporary `PlatformOwner` bootstrap flag was disabled and its username removed immediately after startup. A real form login was verified to redirect to `/platform`; no credential is committed or documented.

Document numbering update (2026-09-15): `DocumentSequence`, its Settings DTO/service/repository/configuration and bilingual `Views/Settings/DocumentNumbers.cshtml` replace the separate invoice and transfer settings. Sale, transfer and purchase-journal workflows generate numbers inside their existing Serializable transactions. Migration `UnifyDocumentNumbering` preserves legacy counters and creates missing tenant defaults.

Tenant role update (2026-09-15): `TenantRole`, `TenantRolePermission`, `TenantRoleService` and its repository separate company authorization from global Identity roles. RolesController no longer accesses EF directly; Users uses tenant-local role options. `AddTenantOwnedRoleDefinitions` preserves existing role permissions and user assignments, while `EnforceTenantRoleAssignmentBoundary` adds the composite database boundary between a role and its owning tenant.
| `Views/*/*.cshtml.cs`, `_View*.cshtml.cs` | generated/companion view files; no custom behaviour verified | do not edit as module logic without inspection. |

## File change impact
For exact module relationships, use `modules/*.md`; for entity and service details use `entities/*.md` and `services/*.md`. Every source addition/removal/move must update this map and `development/DOCUMENTATION_MATRIX.md`.

## Security additions (2026-09-13)
- `MiniStore.Application/Permissions/AdministrationPermissions.cs`: shared Admin-only identity mutation boundary, consumed by both permission evaluators.
- `tests/SecurityRegression/{SecurityRegression.csproj,Program.cs}`: standalone executable authorization/action regression checks; references Web; no test-framework dependency.
- `tests/InventorySqlIntegration/{InventorySqlIntegration.csproj,Program.cs}`: disposable SQL Server fixture for tenant isolation, inventory controls, replenishment purchase-demand deduplication and concurrent Goods Receipt, receipt-return, Vendor Bill and Supplier Payment accounting/overpayment protection.
- `MiniStore.Web/Observability/*`: configuration, health, request/error/duration/active-request metrics and structured failure/slow-request logs.
- `tests/PerformanceSmoke/*`: bounded concurrent local availability workload and JSON p50/p95/p99 result.
- `scripts/verify-local.ps1`: owner-approved local gate; it does not require Git or a hosted build provider.
- Runtime login limiter and Identity lockout: `Web/Configuration/{WebPresentation,Security}Extensions.cs` and `Controllers/Security/AccountController.cs`.
- Bootstrap opt-in: `Infrastructure/Persistence/IdentitySeeder.cs` and Web appsettings.
- Controller broad-error handling and five index delete forms: see security/controller/screen docs.
- MVC form hardening: every public `[HttpPost]` action is covered by explicit or controller-level antiforgery validation. Regression also parses non-partial Razor files and verifies every static `asp-action`/`asp-controller` target resolves to a public controller action.

## Guided onboarding additions (2026-09-15)
- `Domain/Entities/Saas/CompanyOnboarding.cs`: one-time setup state and business-profile choices.
- `Application/Saas/CompanyOnboardingModels.cs`: setup DTOs and Application contract.
- `Infrastructure/Services/Saas/CompanyOnboardingService.cs`: versioned transactional starter template.
- `Web/Controllers/Saas/CompanyOnboardingController.cs` and `Views/CompanyOnboarding/Index.cshtml`: authenticated bilingual setup/skip flow.
- `AddCompanyGuidedOnboarding`: onboarding table and compatible existing-tenant backfill.
