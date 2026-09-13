# Codebase map
> Status: IMPLEMENTED  
> Source of truth: Repository scan  
> Last reviewed: 2026-09-13

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
| `Entities/Inventory/*.cs` | warehouses, balances, locations, movements and transfers | inventory and stock-transfer docs. |
| `Entities/Purchases/*.cs`, `Entities/Sales/*.cs` | purchase, sale and POS aggregates | purchase/sales docs. |
| `Entities/Settings/*.cs` | accounting, discount, numbering, invoice and general settings | settings/database docs. |
| `Entities/Security/*.cs` | audit and role-permission entities | permissions/security docs. |
| `Enums/Sales/DiscountType.cs` | percentage/fixed discount enum | sales/settings docs. |
| `Commands/Inventory/*.cs` | transfer service input commands | transfer service. |
| `Interfaces/<Feature>/*.cs` | persistence/service contracts grouped by feature | matching Application/Infrastructure feature. |
| `Interfaces/Shared/IUnitOfWork.cs` | shared transaction boundary | document services and UnitOfWork. |

## Application source map
| Files | Purpose | Used by |
|---|---|---|
| `Services/<Feature>/*.cs` | use-case services grouped as Accounting, Catalog, Customers, Inventory, Purchases, Sales, Settings and Suppliers | matching MVC controllers. |
| `Services/Inventory/{StorageLocation,UnassignedStock}Service.cs` | location administration, warehouse search and stock putaway | inventory controllers. |
| `Services/Sales/DiscountCalculator.cs` | standalone discount calculation helper; no active consumer found by scan | Unknown. |
| `Services/Shared/ICurrentUserService.cs` | current-user application contract | Web CurrentUserService. |
| `Permissions/{PermissionDefinitions,IPermissionService}.cs` | permission catalogue/contract | seeders/auth/services. |
| `Dtos/Catalog/Products/*.cs` | product request/display DTOs | catalog services/controllers/views. |
| `Dtos/Inventory/<ProductStocks|StockTransfers|Warehouses>/*.cs` | inventory request/display DTO families | inventory services/controllers/views. |
| `Dtos/<Accounting|Purchases|Sales|Settings|Suppliers>/*.cs` | feature request/display DTOs | matching services/controllers/views. |

## Infrastructure source map
| Files | Purpose | Consumers |
|---|---|---|
| `Persistence/AppDbContext.cs` | EF + Identity DbContext and DbSets | all repositories/authorization. |
| `Persistence/UnitOfWork.cs` | transaction wrapper | purchase/sale/stock/transfer services. |
| `Persistence/AuditSaveChangesInterceptor.cs` | creates AuditLog rows for tracked changes | registered in Program. |
| `Persistence/{Identity,Permission}Seeder.cs` | default roles/admin and permissions | Program startup. |
| `Persistence/Configurations/<Feature>/*.cs` | per-entity schema mapping grouped by feature | applied automatically by AppDbContext. |

Accounting foundation (2026-09-13): migration `AddAccountingFoundation` adds Accounts, Branches, JournalEntries, JournalEntryLines and optional warehouse branch/inventory-account links. See accounting ADR before adding automated posting.
| `Migrations/*.cs` and snapshot | schema evolution/model snapshots | EF tooling, deployment. |

Inventory concurrency update (2026-09-13): ProductStock and StockTransfer include database-generated rowversions; migration `AddInventoryConcurrency` updates SQL Server. UnitOfWork uses Serializable isolation for atomic inventory workflows. Manual stock transactions accept adjustments only, and purchase/sale aggregates reject duplicate product lines. Focused checks live in `tests/SecurityRegression`.
| `Repositories/<Feature>/*.cs` | EF implementations grouped by feature | application services. |
| `Repositories/Accounting/JournalEntryRepository.cs` | journal source duplicate-posting lookup and persistence | `PurchasePostingService`. |
| `Authorization/PermissionService.cs` | permission check implementation | SaleService. |

## Web source map
| Files | Purpose | Related docs |
|---|---|---|
| `Program.cs`, `appsettings.json`, `Properties/launchSettings.json` | startup/configuration | configuration/architecture. |
| `Authorization/*.cs` | dynamic permission policy and handler | permissions/security. |
| `Services/Security/CurrentUserService.cs` | current Identity user ID for auditing | security/database. |
| `Controllers/<Feature>/*.cs` | MVC endpoints grouped by business feature; namespaces remain stable | `controllers/*.md`. |
| `Controllers/Home/HomeController.cs` | application entry page | home view. |
| `Navigation/*.cs` | navigation metadata | permissions/screens. |
| `Views/<Module>/*.cshtml` | Razor screens/forms/client JS | `screens/*.md`. |
| `Views/Shared/*.cshtml` | layout, notifications, validation/error/delete partials | screens/shared-ui.md. |
| `Views/*/*.cshtml.cs`, `_View*.cshtml.cs` | generated/companion view files; no custom behaviour verified | do not edit as module logic without inspection. |

## File change impact
For exact module relationships, use `modules/*.md`; for entity and service details use `entities/*.md` and `services/*.md`. Every source addition/removal/move must update this map and `development/DOCUMENTATION_MATRIX.md`.

## Security additions (2026-09-13)
- `MiniStore.Application/Permissions/AdministrationPermissions.cs`: shared Admin-only identity mutation boundary, consumed by both permission evaluators.
- `tests/SecurityRegression/{SecurityRegression.csproj,Program.cs}`: standalone executable authorization/action regression checks; references Web; no test-framework dependency.
- Runtime login limiter and Identity lockout: `Web/Program.cs` and `Controllers/Security/AccountController.cs`.
- Bootstrap opt-in: `Infrastructure/Persistence/IdentitySeeder.cs` and Web appsettings.
- Controller broad-error handling and five index delete forms: see security/controller/screen docs.
