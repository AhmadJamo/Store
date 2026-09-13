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
| `Entities/Product.cs`, `Warehouse.cs`, `Supplier.cs` | Master-data entities | respective modules/entities docs. |
| `Entities/ProductStock.cs`, `StockTransaction.cs`, `StockTransactionType.cs` | balance and movement model | inventory module. |
| `Entities/Purchase.cs`, `PurchaseItem.cs` | purchase aggregate/items | purchases module. |
| `Entities/Sale.cs`, `SaleItem.cs`, `SaleChannel.cs` | sales aggregate/items/channels | sales module. |
| `Entities/StockTransfer*.cs` | transfer aggregate, item, history, statuses/actions | stock-transfers module. |
| `Entities/{Permission,RolePermission}.cs` | role permission mapping | permissions doc. |
| `Entities/{Account,Branch,JournalEntry,JournalEntryLine,PaymentMethod}.cs` | accounting foundation: hierarchy, dimensions, settlement methods and balanced multi-line entries | accounting doc. |
| `Entities/{AuditLog,GeneralSettings,DiscountSettings,AccountingSettings,InvoiceSettings,DocumentNumberSettings}.cs` | audit/configuration state; accounting posting-account mappings | settings/database docs. |
| `Enum/DiscountType.cs` | percentage/fixed discount enum | sales/settings docs. |
| `Commands/CreateStockTransferCommand.cs`, `UpdateStockTransferCommand.cs` | transfer service input commands | transfer service. |
| `Interfaces/I*Repository.cs` | persistence contracts | matching Infrastructure repository. |
| `Interfaces/IUnitOfWork.cs`, `IStockTransferService.cs` | transaction/transfer contracts | UnitOfWork/StockTransferService. |

## Application source map
| Files | Purpose | Used by |
|---|---|---|
| `Services/{Product,Warehouse,Supplier}Service.cs` | master-data operations | matching MVC controllers. |
| `Services/{ProductStock,StockTransaction,Purchase,Sale,StockTransfer}Service.cs` | inventory/business documents | matching controllers. |
| `Services/{Invoice,General,DiscountSettings,AccountingSettings,PaymentMethod,Branch}Service.cs` | settings operations and branch sales-account mapping | SettingsController, BranchesController and PaymentMethodsController. |
| `Services/DiscountCalculator.cs` | standalone discount calculation helper; no active consumer found by scan | Unknown. |
| `Services/ISaleService.cs`, `ICurrentUserService.cs` | service contracts | SaleService/CurrentUserService. |
| `Permissions/{PermissionDefinitions,IPermissionService}.cs` | permission catalogue/contract | seeders/auth/services. |
| `Dtos/<Products|Warehouses|Suppliers>/*.cs` | request/display DTOs | matching services/controllers/views. |
| `Dtos/<ProductStocks|Purchases|Sales|StockTransfers|Settings>/*.cs` | request/display DTOs | matching services/controllers/views. |

## Infrastructure source map
| Files | Purpose | Consumers |
|---|---|---|
| `Persistence/AppDbContext.cs` | EF + Identity DbContext and DbSets | all repositories/authorization. |
| `Persistence/UnitOfWork.cs` | transaction wrapper | purchase/sale/stock/transfer services. |
| `Persistence/AuditSaveChangesInterceptor.cs` | creates AuditLog rows for tracked changes | registered in Program. |
| `Persistence/{Identity,Permission}Seeder.cs` | default roles/admin and permissions | Program startup. |
| `Persistence/Configurations/*.cs` | per-entity schema mapping | applied by AppDbContext. |

Accounting foundation (2026-09-13): migration `AddAccountingFoundation` adds Accounts, Branches, JournalEntries, JournalEntryLines and optional warehouse branch/inventory-account links. See accounting ADR before adding automated posting.
| `Migrations/*.cs` and snapshot | schema evolution/model snapshots | EF tooling, deployment. |

Inventory concurrency update (2026-09-13): ProductStock and StockTransfer include database-generated rowversions; migration `AddInventoryConcurrency` updates SQL Server. UnitOfWork uses Serializable isolation for atomic inventory workflows. Manual stock transactions accept adjustments only, and purchase/sale aggregates reject duplicate product lines. Focused checks live in `tests/SecurityRegression`.
| `Repositories/*.cs` | EF implementations of Domain repository contracts | application services. |
| `Repositories/JournalEntryRepository.cs` | journal source duplicate-posting lookup and persistence | `PurchasePostingService`. |
| `Authorization/PermissionService.cs` | permission check implementation | SaleService. |

## Web source map
| Files | Purpose | Related docs |
|---|---|---|
| `Program.cs`, `appsettings.json`, `Properties/launchSettings.json` | startup/configuration | configuration/architecture. |
| `Authorization/*.cs` | dynamic permission policy and handler | permissions/security. |
| `Services/CurrentUserService.cs` | current Identity user ID for auditing | security/database. |
| `Controllers/*.cs` | MVC endpoints | `controllers/*.md`. |
| `Navigation/*.cs` | navigation metadata | permissions/screens. |
| `Views/<Module>/*.cshtml` | Razor screens/forms/client JS | `screens/*.md`. |
| `Views/Shared/*.cshtml` | layout, notifications, validation/error/delete partials | screens/shared-ui.md. |
| `Views/*/*.cshtml.cs`, `_View*.cshtml.cs` | generated/companion view files; no custom behaviour verified | do not edit as module logic without inspection. |

## File change impact
For exact module relationships, use `modules/*.md`; for entity and service details use `entities/*.md` and `services/*.md`. Every source addition/removal/move must update this map and `development/DOCUMENTATION_MATRIX.md`.

## Security additions (2026-09-13)
- `MiniStore.Application/Permissions/AdministrationPermissions.cs`: shared Admin-only identity mutation boundary, consumed by both permission evaluators.
- `tests/SecurityRegression/{SecurityRegression.csproj,Program.cs}`: standalone executable authorization/action regression checks; references Web; no test-framework dependency.
- Runtime login limiter and Identity lockout: `Web/Program.cs` and `Controllers/AccountController.cs`.
- Bootstrap opt-in: `Infrastructure/Persistence/IdentitySeeder.cs` and Web appsettings.
- Controller broad-error handling and five index delete forms: see security/controller/screen docs.
