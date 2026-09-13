# ADR: Organize source files by feature inside each layer
> Date: 2026-09-13 | Status: Accepted

## Context
The solution already separates Domain, Application, Infrastructure and Web, but most entities, services, repository interfaces, repository implementations, EF configurations and MVC controllers were stored in large flat folders. As the ERP modules grew, related files became difficult to find and unrelated modules were mixed together.

## Decision
Keep the four project/layer boundaries and group files by business feature inside each artifact folder. The standard business feature names are `Accounting`, `Catalog`, `Customers`, `Inventory`, `Purchases`, `Sales`, `Security`, `Settings` and `Suppliers`; the Web project also has `Home`. Cross-cutting contracts belong in `Shared`.

Examples:

- `MiniStore.Domain/Entities/Inventory/`
- `MiniStore.Application/Services/Purchases/`
- `MiniStore.Infrastructure/Repositories/Accounting/`
- `MiniStore.Infrastructure/Persistence/Configurations/Inventory/`
- `MiniStore.Web/Controllers/Sales/`

DTOs remain grouped by use case; inventory DTO families are collected below `Dtos/Inventory/`. Razor views remain under `Views/<ControllerName>/` because this is the default MVC view-discovery convention. EF migrations remain in the chronological `Migrations/` folder. Composition, authorization, navigation and persistence infrastructure remain in their existing cross-cutting folders.

Existing namespaces stay unchanged during this move. This keeps controller discovery, dependency injection, EF metadata, migrations and external code references stable while improving physical navigation. New files should use the matching feature folder and should not be placed in a flat artifact root unless they are truly cross-cutting.

## Consequences
Developers can find a feature's entities, services, contracts and persistence code predictably without changing runtime behavior. A feature may still span projects because dependency direction remains `Web → Application → Domain` with Infrastructure implementing persistence. Namespace alignment can be considered separately if a future version intentionally accepts that larger breaking change.
