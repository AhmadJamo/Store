# How to update MiniStore documentation
> Last reviewed: 2026-09-13

Source code is authoritative. Before code changes read `00_PROJECT_CONTEXT.md`, `01_CODEBASE_MAP.md`, the matching module document and actual source.

## By change type
- **Entity:** review entity/configuration/migration, DTOs, services, repositories, controller/screens, module, `03_DATABASE.md`, `04_ACCOUNTING.md` if financial, map and tests.
- **DTO/service:** review relevant service/module/controller/screen/map and tests. Preserve Application layer.
- **Controller/Razor/JavaScript:** review controller + screen + module + permission/navigation docs; confirm backend authorization, antiforgery and model binding.
- **Database:** create migration; update database/table/entity/module docs; inspect existing data and delete behaviour.
- **Permission/security:** update definition/seeder/handler/controller/navigation and `05_PERMISSIONS.md`/`06_SECURITY.md`; add authorization tests.
- **New/delete/move file:** update `01_CODEBASE_MAP.md`, module document, links and references. Search references before deletion.
- **Accounting rule:** update accounting, relevant entities/services/modules/database and ADR when it changes system design.

After change: build, run relevant tests, review diff, update docs' date/status, TODO status and `history/AI_WORKLOG.md`.

Place new source files in the matching feature subfolder inside their layer. Use the established feature names documented in `decisions/2026-09-13-feature-folder-organization.md`; do not add a file to a flat Entities, Interfaces, Services, Repositories, Configurations or Controllers root unless it is cross-cutting.
