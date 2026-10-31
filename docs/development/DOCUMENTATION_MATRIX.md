# Documentation update matrix
> Last reviewed: 2026-09-13

| Change | Must review/update |
|---|---|
| Entity or enum | entity doc, database/tables, configuration/migration, module, services, map, tests |
| DTO/command | service, controller, screen, module, map, tests |
| Service/repository | service, module, entities, controller/screen, transaction/accounting docs, map/tests |
| Controller | controller, screen, permissions, module, map |
| Razor/JS/layout | screen/shared UI, controller, module, permissions/navigation |
| User-facing error or prerequisite | localization, owning screen/controller/module, actionable-error destination when an authorized setup page can resolve it, and regression coverage |
| Successful workflow step | localization, owning controller/screen/module, permission-aware next-step destination and explanation, shared guidance mapping/filter, and regression coverage |
| Completed one-time action | owning DTO/service status or remaining balance, screen action visibility, localized completed state, downstream document link, module/screen docs and regression coverage |
| Bulk editor | nested DTO binding, per-row identity and concurrency validation, atomic transaction, permission/antiforgery, localization, owning module/screen/controller docs and regression coverage |
| Database | database docs, entity/configuration, migration, modules, accounting where relevant |
| Permission/role | permissions, controller/screen/navigation, security, module, tests |
| Configuration/DI | configuration, architecture, dependencies, security |
| New/deleted/renamed file | codebase map plus all linked documentation |
| Architectural decision | architecture, ADR, affected module docs, TODO/worklog |
| Internal API/Angular contract | API DTO/service/controller, auth/tenant/antiforgery behavior, OpenAPI, Angular client/interceptor, architecture, security, configuration, dependencies, modernization phase, map, tests and worklog |

Security regression executable: `tests/SecurityRegression` must be reviewed/run when changing permission evaluation, identity mutation policy or delete action HTTP methods. Shared policy helper is mapped in `01_CODEBASE_MAP.md`.

Guided company setup changes map to tenancy/settings modules, tenancy entities, database tables/relationships/migrations, MVC controllers, screen map, settings services, security, testing, TODO, worklog and an ADR.
