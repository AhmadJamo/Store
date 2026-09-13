# Architecture
> Status: IMPLEMENTED WITH INCONSISTENCIES  
> Source of truth: Code  
> Last reviewed: 2026-09-13

## Solution and dependency direction
`MiniStore.Domain` contains entities, enums, commands and repository/service interfaces. `MiniStore.Application` references Domain and contains DTOs plus use-case services. `MiniStore.Infrastructure` references Application and Domain and implements EF Core persistence, repositories, seeders, authorization service and Unit of Work. `MiniStore.Web` references Application and Infrastructure and is the MVC presentation/composition root.

`Web → Application → Domain`; `Infrastructure → Application + Domain`. EF configuration lives in Infrastructure. `Program.cs` registers all concrete services/repositories and MVC/Identity.

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
- No tenant boundary exists; SaaS isolation is not implemented.
- Several repositories expose `SaveChangesAsync`, creating inconsistent transaction ownership.
- `PermissionsCodeExport/` is a duplicate export tree, not included by the solution projects; it can drift from active code.

## Change impact
Entity → EF configuration/migration → repository → service → DTO → controller/view → module/entity/database docs. Service → interface/dependencies/controllers/views/tests. See `development/DOCUMENTATION_MATRIX.md`.

2026-09-13 security policy: identity mutations now use the shared Application `AdministrationPermissions` rule in existing permission evaluators. Login limiting uses built-in ASP.NET Core middleware. See `decisions/2026-09-13-admin-mutations.md`; no additional transaction mechanism or database schema was introduced.
