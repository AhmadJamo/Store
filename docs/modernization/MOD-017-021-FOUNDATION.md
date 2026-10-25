# MOD-017..021 foundation bundle

> Status: IN PROGRESS — first usable slice delivered for each phase on 2026-10-25.

This bundle deliberately starts five dependent phases without claiming their broad completion:

- `MOD-017`: governed Angular workspace builder and save over the existing sales/purchasing providers;
- `MOD-018`: one reusable verification command for backend build/regression, Angular tests/build and external-SCSS policy;
- `MOD-019`: source-generated registry of 89 MVC controllers and 189 user-facing views;
- `MOD-020`: read-only Angular organization identity and operating-preferences overview;
- `MOD-021`: server-paged Angular product list with search and status filtering.

All API endpoints are authenticated, permission protected and call Application services. No EF entity is
returned, no financial value is calculated in the browser, and no database migration or business rule changed.
MVC remains the rollback path. The generated registry is in `MVC_MIGRATION_REGISTRY.md` and is refreshed with
`scripts/generate-mvc-migration-registry.ps1`.

## Verification evidence

- Web Release build: passed with zero warnings and zero errors.
- Security/inventory regression: 1096 checks passed.
- Angular production build: passed.
- Angular tests: 59/59 passed.
- External-style policy: passed; no Angular HTML `style=` attributes or component `styles:` blocks.

## Remaining gates

Delegation (`MOD-017`), performance/accessibility/browser/E2E coverage (`MOD-018`),
action/control registry and cutover controls (`MOD-019`), and complete settings/catalog functional parity
(`MOD-020..021`) remain open.
