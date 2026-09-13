# Documentation drift detection
> Last reviewed: 2026-09-13

Before a major commit: run `rg --files` and compare new/deleted/moved source paths with `01_CODEBASE_MAP.md`; search docs for old paths; compare permission definitions with controller attributes/navigation; compare DbContext/configurations/migrations/table docs; compare views with controller actions and DTOs; re-read business-rule claims against services/entities. Mark anything not determinable as `Unknown / Not determined from code`; never guess.
