# Codebase map
> Status: IMPLEMENTED  
> Source of truth: Repository scan  
> Last reviewed: 2026-09-14

## Solution roots
| Path | Project/layer | Purpose |
|---|---|---|
| `MiniStore.sln`, `MiniStore.slnx` | Solution | Four active projects. |
| `MiniStore.Domain/` | Domain | Entities, enums, commands, interfaces. |
| `MiniStore.Application/` | Application | DTOs, services, permission definitions. |
| `MiniStore.Infrastructure/` | Infrastructure | EF DbContext/configurations/migrations/repos/seeders. |
| `MiniStore.Web/` | Presentation | MVC controllers, Razor views, auth/navigation/runtime composition. |
| `PermissionsCodeExport/` | Uncompiled export | Duplicate code snapshot; not referenced by active projects. |

## Domain source map
| Files | Type/purpose | Consumers/docs |
|---|---|---|
| `Entities/Accounting/*.cs` | chart, branches, journal entries, tax and payment entities | accounting docs. |
| `Entities/Catalog/Product.cs` | product master data | products and entity docs. |
| `Entities/Customers/Customer.cs`, `Entities/Suppliers/Supplier.cs` | commercial-party master data | sales/purchases docs. |
| `Entities/Inventory/*.cs` | warehouses, operating-policy enums, balances, exact locations, putaway/relocation history, stock movements and transfers | inventory and stock-transfer docs. |
| `Entities/Purchases/*.cs`, `Entities/Sales/*.cs` | purchase, sale and POS aggregates plus per-terminal experience/order enums and settings | purchase/sales/settings docs. |
| `Entities/Settings/*.cs` | accounting, discount, inventory-policy defaults, numbering, invoice/general settings and supported UI language | settings/database/localization docs. |
| `Entities/Security/*.cs` | audit and role-permission entities | permissions/security docs. |
| `Entities/Tenancy/*.cs` | company tenant and Identity-user membership control-plane entities | tenancy/security/database docs. |
| `Enums/Sales/DiscountType.cs` | percentage/fixed discount enum | sales/settings docs. |
| `Commands/Inventory/*.cs` | transfer service input commands | transfer service. |
| `Interfaces/<Feature>/*.cs` | persistence/service contracts grouped by feature | matching Application/Infrastructure feature. |
| `Interfaces/Shared/IUnitOfWork.cs` | shared transaction boundary | document services and UnitOfWork. |
| `Interfaces/Tenancy/*.cs` | current tenant and membership persistence contracts | Web tenant session and Infrastructure repository. |

## Application source map
| Files | Purpose | Used by |
|---|---|---|
| `Services/<Feature>/*.cs` | use-case services grouped as Accounting, Catalog, Customers, Inventory, Purchases, Sales, Settings and Suppliers | matching MVC controllers. |
| `Services/Inventory/{StorageLocation,UnassignedStock,LocationMovement}Service.cs` | location administration, warehouse search, putaway, internal relocation and history | inventory controllers. |
| `Services/Settings/InventorySettingsService.cs` | rowversion-protected defaults for new warehouse operating policies | SettingsController inventory screen. |
| `Services/Settings/InventoryAccessService.cs` | branch warehouse permissions/priorities and POS terminal warehouse policies | Settings InventoryAccess and POS validation. |
| `Services/Settings/PosExperienceSettingsService.cs` | profile presets, custom terminal appearance persistence and POS runtime projection | Settings/Pos and Sales/Pos. |
| `Services/Sales/DiscountCalculator.cs` | standalone discount calculation helper; no active consumer found by scan | Unknown. |
| `Services/Shared/ICurrentUserService.cs` | current-user application contract | Web CurrentUserService. |
| `Permissions/{PermissionDefinitions,IPermissionService}.cs` | permission catalogue/contract | seeders/auth/services. |
| `Tenancy/TenantClaimTypes.cs` | trusted tenant claim names shared by login and request session validation | Web authentication. |
| `Dtos/Catalog/Products/*.cs` | product request/display DTOs | catalog services/controllers/views. |
| `Dtos/Inventory/<ProductStocks|StockTransfers|Warehouses>/*.cs` | inventory request/display DTO families | inventory services/controllers/views. |
| `Dtos/Inventory/LocationMovements/*.cs` | relocation input, lookup and history page models | LocationMovementService/controller/view. |
| `Dtos/<Accounting|Purchases|Sales|Settings|Suppliers>/*.cs` | feature request/display DTOs | matching services/controllers/views. |

## Infrastructure source map
| Files | Purpose | Consumers |
|---|---|---|
| `Persistence/AppDbContext.cs` | EF + Identity DbContext and DbSets | all repositories/authorization. |
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
| `Repositories/Tenancy/TenantMembershipRepository.cs` | active company membership lookup and user list | login, tenant middleware and user administration. |
| `Repositories/Accounting/JournalEntryRepository.cs` | journal source duplicate-posting lookup and persistence | `PurchasePostingService`. |
| `Authorization/PermissionService.cs` | permission check implementation | SaleService. |

## Web source map
| Files | Purpose | Related docs |
|---|---|---|
| `Program.cs`, `appsettings.json`, `Properties/launchSettings.json` | startup/configuration | configuration/architecture. |
| `Authorization/*.cs` | dynamic permission policy and handler | permissions/security. |
| `Services/Security/CurrentUserService.cs` | current Identity user ID for auditing | security/database. |
| `Services/Tenancy/HttpTenantContext.cs`, `Middleware/TenantSessionMiddleware.cs` | resolve, validate and refresh the authenticated company boundary | AppDbContext, login and localization. |
| `Controllers/<Feature>/*.cs` | MVC endpoints grouped by business feature; namespaces remain stable | `controllers/*.md`. |
| `Localization/*.cs`, `Resources/SharedResource.ar.resx` | supported cultures, cached database-default provider and centralized Arabic translations | shared layout and localized views/controllers. |
| `Controllers/Home/HomeController.cs` | application entry page | home view. |
| `Navigation/*.cs` | navigation metadata | permissions/screens. |
| `Views/<Module>/*.cshtml` | Razor screens/forms/client JS | `screens/*.md`. |
| `Views/Shared/*.cshtml` | localized LTR/RTL layout, language switch, notifications, validation/error/delete partials | screens/shared-ui.md. |

Localization update (2026-09-14): ASP.NET request localization supports `en-US` and `ar-JO`; GeneralSettings provides the cached company default and a whitelisted cookie endpoint provides user override. Shared layout/navigation/auth/dialogs and core Settings pages use `SharedResource`; migration `AddLocalizationSettings` backfills English.

Tenant isolation update (2026-09-14): `Tenant`, `TenantMembership`, tenant context and session middleware establish a shared-database company boundary. AppDbContext adds TenantId/query filters/write guards to 34 business entities and changes business uniqueness to tenant-aware indexes. Migration `AddTenantIsolation` backfills the Demo Company and current users.
| `Views/*/*.cshtml.cs`, `_View*.cshtml.cs` | generated/companion view files; no custom behaviour verified | do not edit as module logic without inspection. |

## File change impact
For exact module relationships, use `modules/*.md`; for entity and service details use `entities/*.md` and `services/*.md`. Every source addition/removal/move must update this map and `development/DOCUMENTATION_MATRIX.md`.

## Security additions (2026-09-13)
- `MiniStore.Application/Permissions/AdministrationPermissions.cs`: shared Admin-only identity mutation boundary, consumed by both permission evaluators.
- `tests/SecurityRegression/{SecurityRegression.csproj,Program.cs}`: standalone executable authorization/action regression checks; references Web; no test-framework dependency.
- Runtime login limiter and Identity lockout: `Web/Program.cs` and `Controllers/Security/AccountController.cs`.
- Bootstrap opt-in: `Infrastructure/Persistence/IdentitySeeder.cs` and Web appsettings.
- Controller broad-error handling and five index delete forms: see security/controller/screen docs.
