# Architecture
> Status: IMPLEMENTED WITH INCONSISTENCIES  
> Source of truth: Code  
> Last reviewed: 2026-09-14

## Solution and dependency direction
`MiniStore.Domain` contains entities, enums, commands and repository/service interfaces. `MiniStore.Application` references Domain and contains DTOs plus use-case services. `MiniStore.Infrastructure` references Application and Domain and implements EF Core persistence, repositories, seeders, authorization service and Unit of Work. `MiniStore.Web` references Application and Infrastructure and is the MVC presentation/composition root.

`Web → Application → Domain`; `Infrastructure → Application + Domain`. EF configuration lives in Infrastructure. `Program.cs` registers all concrete services/repositories and MVC/Identity.

Source files are grouped by business feature inside each layer. Entities, service contracts, services, repositories, EF configurations and controllers use the shared feature names Accounting, Catalog, Customers, Inventory, Purchases, Sales, Security, Settings and Suppliers; Web also contains Home. Shared infrastructure stays at the layer root or in a `Shared` folder. Razor views retain the MVC `Views/<ControllerName>/` convention and migrations retain chronological ordering. See `decisions/2026-09-13-feature-folder-organization.md`.

Inventory behavior is policy-driven at the warehouse boundary. Operational use is separate from Simple/LocationManaged/Hybrid control, and global InventorySettings provide defaults rather than overriding an established warehouse. See `decisions/2026-09-14-configurable-inventory-operating-policies.md`.

Warehouse access is layered: branch permissions define the organizational boundary and POS terminal mappings narrow that list. See `decisions/2026-09-14-branch-and-pos-warehouse-access.md`.

POS appearance is composed from a business-profile preset and persisted per-terminal overrides. The setting belongs to the terminal rather than the company or branch because two devices in one branch may serve different workflows. See `decisions/2026-09-14-configurable-pos-experience.md`.

POS order context is stored on the completed sale and its items, while the terminal setting controls which context the operator may submit. This separates durable transaction facts from configurable device behavior. See `decisions/2026-09-14-pos-order-context.md`.

Presentation localization uses ASP.NET request localization with English fallback keys and centralized Arabic resources. Culture precedence is user cookie, cached company default, then English. Domain state stores only a language enum; translated text remains in Web resources. See `decisions/2026-09-14-bilingual-localization.md`.

SaaS data uses one SQL Server database with a required tenant owner key on every business entity. The authenticated tenant claim is accepted only while an active user membership and active tenant exist. EF global query filters scope reads, SaveChanges stamps inserts and rejects cross-tenant mutations, and tenant-aware unique indexes allow repeated business codes across companies. Tenant and membership records are control-plane data and are deliberately outside business-data query filters. See `decisions/2026-09-14-tenant-data-isolation.md`.

## Request lifecycle
Browser → MVC controller → DTO model binding/ModelState → application service → repository/domain entity → `AppDbContext`/SQL Server → redirect or Razor view. AutoValidateAntiforgeryToken is registered globally. Sales, purchases and transfers use `IUnitOfWork`; several master-data services use repository `SaveChangesAsync` directly.

## Actual patterns
- Domain entities use private setters and behaviour methods.
- Manual DTO mapping is used; AutoMapper is not referenced.
- Repository interfaces are in Domain; implementations are in Infrastructure.
- MVC controllers sometimes directly use repository interfaces or `AppDbContext` (`RolesController`), which is an architecture inconsistency against the intended application-layer rule.
- No validators, CQRS query objects, middleware classes, public API controllers, background jobs, or tests were found.

## Dependency injection
See `08_CONFIGURATION.md`. `Program.cs` is the sole DI composition root. `ISaleRepository`/`ISaleService` are registered twice (same mapping) in `Program.cs`; no behavioural difference is implied.

## Architectural risks
- Authorization administration accesses EF directly in `RolesController`.
- Identity roles and role-permission mappings remain global; tenant-specific role assignment is the next authorization phase.
- Several repositories expose `SaveChangesAsync`, creating inconsistent transaction ownership.
- `PermissionsCodeExport/` is a duplicate export tree, not included by the solution projects; it can drift from active code.

## Change impact
Entity → EF configuration/migration → repository → service → DTO → controller/view → module/entity/database docs. Service → interface/dependencies/controllers/views/tests. See `development/DOCUMENTATION_MATRIX.md`.

2026-09-13 security policy: identity mutations now use the shared Application `AdministrationPermissions` rule in existing permission evaluators. Login limiting uses built-in ASP.NET Core middleware. See `decisions/2026-09-13-admin-mutations.md`; no additional transaction mechanism or database schema was introduced.
