# Codebase map
> Status: IMPLEMENTED  
> Source of truth: Repository scan  
> Last reviewed: 2026-10-04

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

`docs/ARABIC_SHARED_ROADMAP.md` is the shared Arabic execution roadmap. It records ordered ERP/SaaS work, business rules, workflow, task status, completion evidence and the rolling five-suggestion queue.

`docs/README.md` is the documentation index and recommended reading order for contributors.

`docs/ACCOUNTING_REFERENCE_AR.md` is the shared Arabic accounting and software-design reference. It covers the accounting cycle, example postings, inventory valuation, sales/purchases, close, advanced topics, international-standard mapping, implementation invariants and the ordered accounting delivery plan. It is guidance rather than evidence that a feature is implemented.

`docs/WMS_EVOLUTION_PLAN.md` is the approved WMS evolution plan. It records the current inventory model, gap matrix, target authority boundaries, migration safeguards, phased vertical slices and anticipated file impact. WMS-000 reconciliation is implemented; later WMS entities remain planned unless explicitly marked complete.

`docs/PURCHASE_MANAGEMENT_EVOLUTION_PLAN.md` is the approved procure-to-pay evolution plan. It records the legacy direct-purchase boundary, capability gaps, target aggregates, inventory/accounting integration contracts, additive migration safeguards and PUR-000 through PUR-150 vertical slices. Planned purchase types are not implemented unless the matching slice is explicitly marked complete.

`docs/PURCHASE_ARCHITECTURE_ALIGNMENT.md` is the accepted PUR-005 gate. It fixes Supplier compatibility, Purchasing/Inventory/Accounting ownership, derived quantity authorities, staged currency behavior, purchase-first approval boundaries, GRNI, synchronous contracts and deferred event infrastructure before runtime implementation starts.

`docs/reference/erp-architecture/` preserves the imported cross-ERP proposed architecture package: system charter, module and ownership maps, workflows, integration contracts, cross-cutting rules, delivery roadmap, agent guidance, test matrix and open decisions. It is a planning reference rather than evidence of implemented code, and its Codex guidance does not supersede repository `AGENTS.md`, accepted ADRs or the required documentation workflow.

## Domain source map
| Files | Type/purpose | Consumers/docs |
|---|---|---|
| `Entities/Accounting/*.cs` | chart, branches, journal entries, fiscal periods, tax and payment entities | accounting docs. |
| `Entities/Catalog/{Product,ProductTemplate,ProductCategory,ProductAttributeDefinition,ProductAttributeValue,ProductRecipe,RecipeIngredient,MeasurementUnit}.cs` and product type/unit/dimension/behavior/logistics enums | concrete SKU identity, optional variant template grouping, category-scoped typed attributes/options/values, logistics metadata, tracking/shelf-life policy, sale channels, immutable recipes and managed units | products, sales and inventory docs. |
| `Entities/Customers/Customer.cs`, `Entities/Suppliers/Supplier.cs` | commercial-party master data | sales/purchases docs. |
| `Entities/Inventory/*.cs` | warehouses, operating-policy enums, AVCO balances, locations, putaway rules, availability, reservations, adjustments, lot/serial tracking with persisted removal-strategy audit, recall communication evidence, legacy history, physical movements and transfers | inventory, accounting and stock-transfer docs. |
| `Entities/Purchases/*.cs`, `Entities/Sales/*.cs` | purchase/supplier-return and sale/customer-return aggregates plus POS experience/order settings | purchase/sales/settings docs. |
| `Entities/Purchases/SupplierProductPurchasingInfo.cs` | supplier-specific product code, compatible purchase unit, base-currency price, lead time, order policy, validity and preference | PUR-010 purchasing data screen/service. |
| `Entities/Purchases/PurchaseRequest.cs` | PRQ-numbered Draft/Submitted/Cancelled internal demand, frozen unit conversion lines and immutable action history | PUR-020 purchase-request workflow. |
| `Entities/Purchases/PurchaseApproval.cs` | Warehouse/priority-scoped approval rules, ordered role steps and immutable per-request decision snapshots | PUR-025 approval workflow foundation. |
| `Entities/Purchases/PurchaseSourcingEvent.cs` | RFX-numbered sourcing event, frozen request-line snapshots and supplier invitations; no price, inventory or accounting authority | PUR-030 sourcing workflow. |
| `Entities/Purchases/{SupplierQuotation,PurchaseQuotationAward}.cs` | supplier-specific frozen quote header/lines, commercial terms, comparison totals and reasoned selection record | PUR-040 quotation workflow. |
| `Entities/Purchases/PurchaseOrder.cs` | awarded-quotation Purchase Order and immutable commercial line snapshots; no receipt or accounting authority | PUR-050 order workflow. |
| `Entities/Purchases/GoodsReceipt.cs` | GRN header/line receipt evidence with frozen PO snapshots and optional inventory-tracking inputs | PUR-060 receipt workflow. |
| `Entities/Settings/*.cs` | accounting, discount, inventory-policy defaults, centralized `DocumentSequence`, general settings and supported UI language | settings/database/localization docs. |
| `Entities/Security/*.cs` | audit, permission catalogue and tenant-owned role/permission entities | permissions/security docs. |
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
| `Services/Purchases/PurchaseApprovalService.cs` | approval-rule administration, submit-time rule resolution, frozen instance creation and role-authorized ordered decisions | Purchase Approval Rules and Purchase Request screens. |
| `Services/Purchases/PurchaseSourcingService.cs` | creates and sends supplier invitations from exactly one approved request, while freezing demand snapshots | Purchase Sourcing screens. |
| `Services/Purchases/SupplierQuotationService.cs` | validates invited supplier responses, freezes/submits quotations, exposes comparison and records a reasoned award | Supplier Quotation screens. |
| `Services/Purchases/PurchaseOrderService.cs` | creates one order from an award and governs approve/confirm/cancel transitions without stock or journal mutation | Purchase Order screens. |
| `Services/Purchases/GoodsReceiptService.cs` | atomically validates remaining PO quantity and coordinates receipt posting with Inventory-owned stock, valuation and tracking effects | Goods Receipt capture screen. |
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
| `Services/Inventory/InventoryReservationService.cs` | exact-location/Unassigned reservation, consume/release and read-only history orchestration | StockTransferService and InventoryReservations controller/view. |
| `Services/Inventory/InventoryAdjustmentService.cs` | blind count, approval and atomic dimensional/valued variance posting | InventoryAdjustments controller/views. |
| `Services/Inventory/{InventoryTrackingService,InventoryRemovalAllocator}.cs` | opening allocation, policy activation, shelf-life validation/defaulting, live expiration alerts, recall/quarantine, tracked receipt, warehouse-policy removal ordering, transfer identity preservation and trace projection | products, purchases, sales, stock transfers and InventoryTracking controller/view. |
| `Services/Inventory/UntrackedInventoryRemovalService.cs` | reservation-aware policy allocation across exact pickable locations and Unassigned, with atomic location-balance removal for non-tracked issues | sales, recipes, purchase returns and legacy adjustments. |
| `Services/Inventory/PutawayRuleService.cs` | administers product/category/default putaway rules and resolves priority- and capacity-aware active receivable-location suggestions | Unassigned Stock putaway. |
| `Services/Inventory/ReplenishmentService.cs` | manages product/warehouse replenishment policies and reservation-aware reviewed suggestions | Replenishment screen. |
| `Services/Inventory/{InventoryInsightsService,InventoryActivityClassifier}.cs` | classifies current positive inventory by actual outbound inactivity without mutating stock or valuation | Inventory Insights screen. |
| `Services/Inventory/{InventoryScanningService,InventoryScanResolver}.cs` | exact adjustment/product/source/destination resolution and safe delegation of scanned putaway, relocation and absolute Draft counts | Inventory Scanning screen and owning inventory services. |
| `Services/Settings/InventorySettingsService.cs` | rowversion-protected defaults for new warehouse operating policies | SettingsController inventory screen. |
| `Services/Settings/InventoryAccessService.cs` | branch warehouse permissions/priorities and POS terminal warehouse policies | Settings InventoryAccess and POS validation. |
| `Services/Settings/PosExperienceSettingsService.cs` | profile presets, custom terminal appearance persistence and POS runtime projection | Settings/Pos and Sales/Pos. |
| `Services/Settings/DocumentNumberService.cs` | default creation, validation, administration projection and transactional document-number generation | sales, transfers, purchase posting and Settings/DocumentNumbers. |
| `Services/Security/TenantRoleService.cs` | tenant-local role CRUD, permission validation, protected-role rules and user assignment options | Roles and Users controllers. |
| `Services/Accounting/JournalPostingService.cs` | central source-idempotency, numbering, balance/posting and persistence gateway used inside caller transactions | purchase, sale and sales-return posting workflows. |
| `Services/Settings/FiscalPeriodService.cs` | non-overlapping fiscal-period administration and central posting-date validation | Settings/FiscalPeriods and JournalPostingService. |
| `Services/Sales/SalePostingService.cs` | idempotent sale revenue, discount and COGS general-ledger posting | Sales/Details and Sales/Post. |
| `Services/Sales/SalesReturnService.cs` | cumulative quantity control, identifier-aware tracked restocking, proportional refund and atomic inventory/accounting reversal | SalesReturns controller/views. |
| `Services/Purchases/PurchaseReturnService.cs` | cumulative supplier-return control, explicit tracked identity issue, moving-average inventory issue and proportional accounting reversal | PurchaseReturns controller/views. |
| `Services/Purchases/SupplierPurchasingInfoService.cs` | validates and manages supplier/product purchasing terms without stock or accounting effects | SupplierPurchasing controller/view. |
| `Services/Sales/DiscountCalculator.cs` | standalone discount calculation helper; no active consumer found by scan | Unknown. |
| `Services/Shared/ICurrentUserService.cs` | current-user application contract | Web CurrentUserService. |
| `Permissions/{PermissionDefinitions,IPermissionService}.cs` | permission catalogue/contract | seeders/auth/services. |
| `Tenancy/TenantClaimTypes.cs` | trusted tenant claim names shared by login and request session validation | Web authentication. |
| `Dtos/Catalog/Products/*.cs` | product request/display plus filtered/sorted/paged catalogue DTOs | catalog services/controllers/views. |
| `Dtos/Inventory/<ProductStocks|StockTransfers|Warehouses>/*.cs` | inventory request/display DTO families | inventory services/controllers/views. |
| `Dtos/Inventory/LocationMovements/*.cs` | relocation input, lookup and history page models | LocationMovementService/controller/view. |
| `Dtos/<Accounting|Purchases|Sales|Settings|Suppliers>/*.cs` | feature request/display DTOs | matching services/controllers/views. |

## Infrastructure source map
| Files | Purpose | Consumers |
|---|---|---|
| `Persistence/AppDbContext.cs`, `Persistence/TenantIsolationModel.cs` | EF + Identity DbContext/DbSets and immutable business/control-plane entity classification; applies query/write guards and composite tenant relationship convention | all repositories/authorization. |
| `Persistence/UnitOfWork.cs` | Serializable transaction wrapper with an explicit in-transaction flush for workflows that need generated document identities before recording dependent facts | purchase/sale/stock/transfer services. |
| `Persistence/AuditSaveChangesInterceptor.cs` | creates AuditLog rows for tracked changes | registered in Program. |
| `Persistence/{Identity,Permission}Seeder.cs` | default roles/admin and permissions | Program startup. |
| `Persistence/Configurations/Tenancy/TenantConfiguration.cs` | tenant identity, membership keys, Identity relationships and slug uniqueness | AppDbContext/migration. |
| `Persistence/Configurations/<Feature>/*.cs` | per-entity schema mapping grouped by feature | applied automatically by AppDbContext. |

Accounting foundation (2026-09-13): migration `AddAccountingFoundation` adds Accounts, Branches, JournalEntries, JournalEntryLines and optional warehouse branch/inventory-account links. See accounting ADR before adding automated posting.
| `Migrations/*.cs` and snapshot | schema evolution/model snapshots | EF tooling, deployment. |

Inventory concurrency update (2026-09-13): ProductStock and StockTransfer include database-generated rowversions; migration `AddInventoryConcurrency` updates SQL Server. UnitOfWork uses Serializable isolation for atomic inventory workflows. Manual stock transactions accept adjustments only, and purchase/sale aggregates reject duplicate product lines. Focused checks live in `tests/SecurityRegression`.

Inventory operating-policy update (2026-09-14): warehouses separate operational type from Simple/LocationManaged/Hybrid control and persist picking, POS, capacity and transfer-location rules. InventorySettings stores defaults for new warehouses; migration `AddInventoryOperatingPolicies` updates the schema.

Branch/POS access update (2026-09-14): `BranchWarehouseAccess` and `PosTerminalWarehouse` provide layered warehouse allow-lists and priorities. Settings manages both levels and POS sale creation validates them. Migration `AddBranchAndPosWarehouseAccess` includes compatible backfill.

POS experience update (2026-09-14): `PosTerminalSettings`, its Sales repository/configuration and Settings application service persist validated per-terminal profile/layout preferences. `Views/Settings/Pos.cshtml` manages them with a live preview and `Views/Sales/Pos.cshtml` applies them at runtime. Migration `AddPosExperienceSettings` backfills existing terminals with Retail defaults.

POS order-context update (2026-09-14): Sale/SaleItem persist order type, service reference, guest count and preparation notes. Terminal settings define allowed/default order types and capture behavior; SaleService validates the submitted context. Migration `AddPosOrderWorkflow` backfills operational preferences from each saved profile.

Warehouse location movement update (2026-09-14): `LocationMovement` and its repository/configuration/service record Putaway and Relocation operations. `LocationMovementsController` and `Views/LocationMovements/Index.cshtml` provide internal movement and searchable history. Migration `AddLocationMovementHistory` creates the audit table and indexes.

Hierarchical location update (2026-09-30): StorageLocation retains its identity and structured labels while adding parent, display name, optional barcode, sequence and receive/pick/reserve/ship/count capabilities. WarehouseLocations now supports create/edit and cycle/same-warehouse validation. Migration `AddHierarchicalStorageLocations` is applied to `AHMAD/MiniStoreDb`.
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
| `Repositories/Inventory/StockMovementRepository.cs` | tenant-filtered physical movement persistence and idempotency lookup | WMS-020 StockMovementService. |
| `Repositories/Inventory/InventoryBalanceRepository.cs` | tenant-filtered current dimensional availability projection reads | WMS-040 InventoryBalanceService. |
| `Repositories/Inventory/InventoryReservationRepository.cs` | tracked dimensional balances and tenant-filtered reservation aggregate persistence | WMS-050 InventoryReservationService. |
| `Repositories/Inventory/InventoryAdjustmentRepository.cs` | tenant-filtered adjustment aggregate and current dimensional balance persistence | WMS-060 InventoryAdjustmentService. |
| `Repositories/Inventory/InventoryTrackingRepository.cs` | tenant-filtered tracked balances for application-level removal ordering plus immutable trace history | WMS-070/WMS-080 InventoryTrackingService. |
| `Repositories/Inventory/InventoryRecallRepository.cs` | tenant-filtered recall lifecycle persistence and active-identity guard | InventoryTrackingService recall workflow. |
| `Repositories/Sales/SalesReturnRepository.cs` | immutable return history and original-sale aggregation | `SalesReturnService`. |
| `Repositories/Purchases/PurchaseReturnRepository.cs` | immutable supplier-return history and original-purchase aggregation | `PurchaseReturnService`. |
| `Repositories/Purchases/SupplierProductPurchasingInfoRepository.cs` | tenant-filtered purchasing-term persistence, uniqueness and preferred-supplier lookup | `SupplierPurchasingInfoService`. |
| `Repositories/Purchases/{SupplierQuotation,PurchaseOrder}Repository.cs` | tenant-filtered quotation award and purchase-order persistence, including one-order-per-quotation enforcement | PUR-040/PUR-050 purchase services. |
| `Repositories/Purchases/GoodsReceiptRepository.cs` | tenant-filtered receipt and posted-receipt quantity reads | PUR-060 receipt service. |
| `Authorization/PermissionService.cs` | permission check implementation | SaleService. |

## Web source map
| Files | Purpose | Related docs |
|---|---|---|
| `Program.cs`, `Configuration/*.cs`, `appsettings.json`, `Properties/launchSettings.json` | concise composition entry point plus separated presentation, persistence, security, feature DI, startup seeding, middleware and configurable throttling | configuration/architecture. |
| `Authorization/*.cs` | dynamic permission policy and handler | permissions/security. |
| `Services/Security/CurrentUserService.cs` | current Identity user ID for auditing | security/database. |
| `Services/Tenancy/HttpTenantContext.cs`, `Middleware/TenantSessionMiddleware.cs` | resolve, validate and refresh the authenticated company boundary | AppDbContext, login and localization. |
| `Controllers/<Feature>/*.cs` | MVC endpoints grouped by business feature; namespaces remain stable | `controllers/*.md`. |
| `Controllers/Catalog/RecipesController.cs` | bilingual recipe list/version editor using Products permissions | recipe views and RecipeService. |
| `Controllers/Catalog/ProductCategoriesController.cs` | bilingual product-category administration using Products View/Edit permissions | ProductCategories view and ProductCategoryService. |
| `Controllers/Catalog/ProductAttributesController.cs` | bilingual typed product-attribute administration using Products View/Edit permissions | ProductAttributes view and ProductAttributeService. |
| `Controllers/Catalog/ProductTemplatesController.cs` | bilingual template creation, existing-SKU assignment and explicit bounded variant preview/generation using Products View/Edit permissions | ProductTemplates view and ProductTemplateService. |
| `Controllers/Inventory/StockMovementsController.cs` | permission-protected read-only WMS-020 pilot ledger and legacy-link status | StockMovements view and service. |
| `Controllers/Inventory/InventoryBalancesController.cs` | permission-protected read-only dimensional inventory availability | InventoryBalances view and service. |
| `Controllers/Inventory/InventoryReservationsController.cs` | permission-protected read-only reservation history | InventoryReservations view and service. |
| `Controllers/Inventory/InventoryAdjustmentsController.cs` | permission-split count/approve/post/cancel workflow | InventoryAdjustments views and service. |
| `Controllers/Inventory/InventoryTrackingController.cs` | permission-protected opening allocation, tracking report and quarantine/release commands | InventoryTracking view and service. |
| `Controllers/Purchases/SupplierPurchasingController.cs` | permission-split bilingual supplier purchasing-data administration | PUR-010 service and `Views/SupplierPurchasing/Index.cshtml`. |
| `Controllers/Purchases/{SupplierQuotations,PurchaseOrders}Controller.cs`, `Views/SupplierQuotations/*`, `Views/PurchaseOrders/*` | quotation capture/comparison/award and award-derived Purchase Order list/detail/state actions | PUR-040/PUR-050 services and permissions. |
| `Controllers/Purchases/GoodsReceiptsController.cs`, `Views/GoodsReceipts/Create.cshtml` | permission-protected partial Goods Receipt capture and immediate posting | PUR-060 receipt service. |
| `Areas/Platform/*` | separately authenticated platform-owner control center for plans, companies/subscriptions, promotion codes and pending payment confirmations | SaaS module. |
| `Controllers/PublicController.cs`, `Controllers/Saas/SubscriptionController.cs` | public landing/pricing and tenant subscription/checkout flows | SaaS module and public/subscription views. |
| `Middleware/SubscriptionAccessMiddleware.cs` | blocks tenant ERP access when the current subscription is not usable | SaaS module/security. |
| `Localization/*.cs`, `Resources/SharedResource.ar.resx` | supported cultures, cached database-default provider and centralized Arabic translations | shared layout and localized views/controllers. |
| `Controllers/Home/{Dashboard,Home}Controller.cs` | authenticated Dashboard entry and legacy Home redirect | Dashboard view and login flow. |
| `Navigation/*.cs` | navigation metadata | permissions/screens. |
| `Views/<Module>/*.cshtml` | Razor screens/forms/client JS | `screens/*.md`. |
| `Views/Shared/*.cshtml` | localized LTR/RTL layout, language switch, notifications, validation/error/delete partials | screens/shared-ui.md. |

Localization update (2026-09-14): ASP.NET request localization supports `en-US` and `ar-JO`; GeneralSettings provides the cached company default and a whitelisted cookie endpoint provides user override. Shared layout/navigation/auth/dialogs and core Settings pages use `SharedResource`; migration `AddLocalizationSettings` backfills English.

Tenant isolation update (2026-09-14): `Tenant`, `TenantMembership`, tenant context and session middleware establish a shared-database company boundary. AppDbContext adds TenantId/query filters/write guards to 34 business entities and changes business uniqueness to tenant-aware indexes. Migration `AddTenantIsolation` backfills the Demo Company and current users.

SaaS control-plane update (2026-09-14): public product/pricing pages, separate Platform cookie and operator allow-list, plan features/limits, tenant subscription lifecycle, warehouse/user limit enforcement and time/usage/plan-bounded promotion codes. Migration `AddSaasControlPlane` seeds three bilingual plans and a 30-day Professional trial for existing tenants.

SaaS onboarding/billing update (2026-09-15): company self-registration creates the Identity owner, tenant membership, tenant-scoped Admin assignment and 14-day trial atomically. Tenant checkout stores a priced monthly/annual session, applies a valid one-use-per-company promotion and activates the subscription only after protected platform confirmation. `SubscriptionAccessMiddleware` enforces lifecycle access. Migrations `AddBillingCheckout`, `AddTenantScopedRoles` and `BootstrapPlatformOwner` are applied locally.

Document numbering update (2026-09-15): `DocumentSequence`, its Settings DTO/service/repository/configuration and bilingual `Views/Settings/DocumentNumbers.cshtml` replace the separate invoice and transfer settings. Sale, transfer and purchase-journal workflows generate numbers inside their existing Serializable transactions. Migration `UnifyDocumentNumbering` preserves legacy counters and creates missing tenant defaults.

Tenant role update (2026-09-15): `TenantRole`, `TenantRolePermission`, `TenantRoleService` and its repository separate company authorization from global Identity roles. RolesController no longer accesses EF directly; Users uses tenant-local role options. `AddTenantOwnedRoleDefinitions` preserves existing role permissions and user assignments, while `EnforceTenantRoleAssignmentBoundary` adds the composite database boundary between a role and its owning tenant.
| `Views/*/*.cshtml.cs`, `_View*.cshtml.cs` | generated/companion view files; no custom behaviour verified | do not edit as module logic without inspection. |

## File change impact
For exact module relationships, use `modules/*.md`; for entity and service details use `entities/*.md` and `services/*.md`. Every source addition/removal/move must update this map and `development/DOCUMENTATION_MATRIX.md`.

## Security additions (2026-09-13)
- `MiniStore.Application/Permissions/AdministrationPermissions.cs`: shared Admin-only identity mutation boundary, consumed by both permission evaluators.
- `tests/SecurityRegression/{SecurityRegression.csproj,Program.cs}`: standalone executable authorization/action regression checks; references Web; no test-framework dependency.
- `tests/InventorySqlIntegration/{InventorySqlIntegration.csproj,Program.cs}`: disposable SQL Server fixture for two-tenant reconciliation translation/isolation and concurrent last-unit protection.
- Runtime login limiter and Identity lockout: `Web/Configuration/{WebPresentation,Security}Extensions.cs` and `Controllers/Security/AccountController.cs`.
- Bootstrap opt-in: `Infrastructure/Persistence/IdentitySeeder.cs` and Web appsettings.
- Controller broad-error handling and five index delete forms: see security/controller/screen docs.

## Guided onboarding additions (2026-09-15)
- `Domain/Entities/Saas/CompanyOnboarding.cs`: one-time setup state and business-profile choices.
- `Application/Saas/CompanyOnboardingModels.cs`: setup DTOs and Application contract.
- `Infrastructure/Services/Saas/CompanyOnboardingService.cs`: versioned transactional starter template.
- `Web/Controllers/Saas/CompanyOnboardingController.cs` and `Views/CompanyOnboarding/Index.cshtml`: authenticated bilingual setup/skip flow.
- `AddCompanyGuidedOnboarding`: onboarding table and compatible existing-tenant backfill.
