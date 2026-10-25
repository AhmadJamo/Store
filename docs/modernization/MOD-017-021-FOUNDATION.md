# MOD-017..021 foundation bundle

> Status: IN PROGRESS — first usable slice delivered for each phase on 2026-10-25.

This bundle deliberately starts five dependent phases without claiming their broad completion:

- `MOD-017`: completed governed Angular workspace builder, server-authored financial preview and user/role delegation;
- `MOD-018`: completed reusable build/regression/style/bundle/accessibility/browser gate with anonymous and authenticated API evidence plus local performance;
- `MOD-019`: completed source-generated inventory of 89 controllers (79 MVC/10 API), 189 user-facing views,
  344 MVC actions and 198 Razor control rows, plus reversible per-surface cutover controls;
- `MOD-020`: Angular organization overview plus rowversion/audited operating-preferences editing; remaining settings workflows are staged;
- `MOD-021`: server-paged Angular product list with search and status filtering.

All API endpoints are authenticated, permission protected and call Application services. No EF entity is
returned and no financial value is calculated in the browser. MOD-017C adds one additive tenant-owned delegation
migration; MVC remains the rollback path and provider permission is rechecked on every delegated execution. The
generated registry is in `MVC_MIGRATION_REGISTRY.md` and is refreshed with
`scripts/generate-mvc-migration-registry.ps1`.

## Verification evidence

- Web Release build: passed with zero warnings and zero errors.
- Security/inventory regression: 1113 checks passed after the cutover safety assertions.
- Angular production build: passed.
- Angular tests: 64/64 passed across 24 files after MOD-020A.
- External-style policy: passed; no Angular HTML `style=` attributes or component `styles:` blocks.

## Remaining gates

Complete settings/catalog functional parity (`MOD-020..021`) remains open.
