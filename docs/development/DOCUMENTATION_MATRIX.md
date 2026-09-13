# Documentation update matrix
> Last reviewed: 2026-09-13

| Change | Must review/update |
|---|---|
| Entity or enum | entity doc, database/tables, configuration/migration, module, services, map, tests |
| DTO/command | service, controller, screen, module, map, tests |
| Service/repository | service, module, entities, controller/screen, transaction/accounting docs, map/tests |
| Controller | controller, screen, permissions, module, map |
| Razor/JS/layout | screen/shared UI, controller, module, permissions/navigation |
| Database | database docs, entity/configuration, migration, modules, accounting where relevant |
| Permission/role | permissions, controller/screen/navigation, security, module, tests |
| Configuration/DI | configuration, architecture, dependencies, security |
| New/deleted/renamed file | codebase map plus all linked documentation |
| Architectural decision | architecture, ADR, affected module docs, TODO/worklog |

Security regression executable: `tests/SecurityRegression` must be reviewed/run when changing permission evaluation, identity mutation policy or delete action HTTP methods. Shared policy helper is mapped in `01_CODEBASE_MAP.md`.
