# Codebase map
> Status: IMPLEMENTED  
> Source of truth: Repository scan  
> Last reviewed: 2026-09-29

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

## Domain source map
| Files | Type/purpose | Consumers/docs |
|---|---|---|
| `Entities/Accounting/*.cs` | chart, branches, journal entries, fiscal periods, tax and payment entities | accounting docs. |
| `Entities/Catalog/{Product,ProductRecipe,RecipeIngredient,MeasurementUnit}.cs` and product type/unit/dimension/behavior enums | product code, optional barcode, raw/direct/prepared classification, sale channels, immutable recipe versions and managed/legacy recipe units | products, sales and inventory docs. |
| `Entities/Customers/Customer.cs`, `Entities/Suppliers/Supplier.cs` | commercial-party master data | sales/purchases docs. |
| `Entities/Inventory/*.cs` | warehouses, operating-policy enums, moving-average balances/cost snapshots, exact locations, putaway/relocation history, stock movements and transfers | inventory, accounting and stock-transfer docs. |
| `Entities/Purchases/*.cs`, `Entities/Sales/*.cs` | purchase, sale and POS aggregates plus per-terminal experience/order enums and settings | purchase/sales/settings docs. |
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
| `Services/Catalog/RecipeService.cs` | lists and creates immutable active recipe versions with compatible ingredient units | Recipes controller/views and prepared-product sales. |
| `Services/Settings/MeasurementUnitService.cs` | initializes protected built-ins, manages custom units and performs dimension-safe conversion | Settings/Units plus product and recipe unit selection. |
| `Services/Inventory/{StorageLocation,UnassignedStock,LocationMovement}Service.cs` | location administration, warehouse search, putaway, internal relocation and history | inventory controllers. |
| `Services/Settings/InventorySettingsService.cs` | rowversion-protected defaults for new warehouse operating policies | SettingsController inventory screen. |
| `Services/Settings/InventoryAccessService.cs` | branch warehouse permissions/priorities and POS terminal warehouse policies | Settings InventoryAccess and POS validation. |
| `Services/Settings/PosExperienceSettingsService.cs` | profile presets, custom terminal appearance persistence and POS runtime projection | Settings/Pos and Sales/Pos. |
| `Services/Settings/DocumentNumberService.cs` | default creation, validation, administration projection and transactional document-number generation | sales, transfers, purchase posting and Settings/DocumentNumbers. |
| `Services/Security/TenantRoleService.cs` | tenant-local role CRUD, permission validation, protected-role rules and user assignment options | Roles and Users controllers. |
| `Services/Accounting/JournalPostingService.cs` | central source-idempotency, numbering, balance/posting and persistence gateway used inside caller transactions | purchase, sale and sales-return posting workflows. |
| `Services/Settings/FiscalPeriodService.cs` | non-overlapping fiscal-period administration and central posting-date validation | Settings/FiscalPeriods and JournalPostingService. |
| `Services/Sales/SalePostingService.cs` | idempotent sale revenue, discount and COGS general-ledger posting | Sales/Details and Sales/Post. |
| `Services/Sales/SalesReturnService.cs` | cumulative quantity control, proportional refund and atomic inventory/accounting reversal | SalesReturns controller/views. |
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
| `Persistence/UnitOfWork.cs` | transaction wrapper | purchase/sale/stock/transfer services. |
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
| `Repositories/<Feature>/*.cs` | EF implementations grouped by feature | application services. |
| `Repositories/Catalog/ProductRecipeRepository.cs` | active/versioned recipe persistence with ingredients | RecipeService and SaleService. |
| `Repositories/Tenancy/TenantMembershipRepository.cs` | active company membership lookup and user list | login, tenant middleware and user administration. |
| `Repositories/Security/TenantRoleRepository.cs` | company-filtered role, permission and assignment persistence | TenantRoleService and authorization. |
| `Repositories/Saas/SaasRepository.cs` | control-plane plan, subscription, entitlement usage, operator and promotion persistence | public pricing and Platform area. |
| `Repositories/Accounting/JournalEntryRepository.cs` | journal source duplicate-posting lookup and persistence | purchase and sale posting services. |
| `Repositories/Accounting/FiscalPeriodRepository.cs` | tenant period lookup, overlap validation and rowversion handling | FiscalPeriodService. |
| `Repositories/Inventory/StockTransactionRepository.cs` | inventory movement history plus source/type lookup for accounting settlement | inventory services and `PurchasePostingService`. |
| `Repositories/Sales/SalesReturnRepository.cs` | immutable return history and original-sale aggregation | `SalesReturnService`. |
| `Authorization/PermissionService.cs` | permission check implementation | SaleService. |

## Web source map
| Files | Purpose | Related docs |
|---|---|---|
| `Program.cs`, `appsettings.json`, `Properties/launchSettings.json` | startup/configuration, including configurable registration throttling and friendly rejection routing | configuration/architecture. |
| `Authorization/*.cs` | dynamic permission policy and handler | permissions/security. |
| `Services/Security/CurrentUserService.cs` | current Identity user ID for auditing | security/database. |
| `Services/Tenancy/HttpTenantContext.cs`, `Middleware/TenantSessionMiddleware.cs` | resolve, validate and refresh the authenticated company boundary | AppDbContext, login and localization. |
| `Controllers/<Feature>/*.cs` | MVC endpoints grouped by business feature; namespaces remain stable | `controllers/*.md`. |
| `Controllers/Catalog/RecipesController.cs` | bilingual recipe list/version editor using Products permissions | recipe views and RecipeService. |
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
- Runtime login limiter and Identity lockout: `Web/Program.cs` and `Controllers/Security/AccountController.cs`.
- Bootstrap opt-in: `Infrastructure/Persistence/IdentitySeeder.cs` and Web appsettings.
- Controller broad-error handling and five index delete forms: see security/controller/screen docs.

## Guided onboarding additions (2026-09-15)
- `Domain/Entities/Saas/CompanyOnboarding.cs`: one-time setup state and business-profile choices.
- `Application/Saas/CompanyOnboardingModels.cs`: setup DTOs and Application contract.
- `Infrastructure/Services/Saas/CompanyOnboardingService.cs`: versioned transactional starter template.
- `Web/Controllers/Saas/CompanyOnboardingController.cs` and `Views/CompanyOnboarding/Index.cshtml`: authenticated bilingual setup/skip flow.
- `AddCompanyGuidedOnboarding`: onboarding table and compatible existing-tenant backfill.
