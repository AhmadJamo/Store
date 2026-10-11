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

For every new or changed user-facing error, decide whether the user can resolve it on an existing screen. When a relevant configuration or master-data page exists, add or extend actionable-error guidance so the message includes a permission-aware button to that page. Do not attach setup links to ordinary input-validation errors, and never show an action the current user is not authorized to perform.

For every completed user operation, define the next logical step, a short explanation and its permission-aware destination. Purchase, inventory and warehouse POST redirects receive a safe generic completion prompt automatically, but known workflow transitions must be added to `WorkflowNextStepGuidance` with the document route and exact permission. New modules must adopt the same pattern when they introduce a workflow; never leave a successful multi-step operation without guidance.

An action button must represent an operation that is still available. After a one-time step succeeds, hide its button and show a localized completed/posted/finished state in the same action area. If the step created another document, show a view link to that document instead of another create button. For partial or repeatable actions, calculate remaining quantity/balance and remove the action only when nothing remains.

Place new source files in the matching feature subfolder inside their layer. Use the established feature names documented in `decisions/2026-09-13-feature-folder-organization.md`; do not add a file to a flat Entities, Interfaces, Services, Repositories, Configurations or Controllers root unless it is cross-cutting.
