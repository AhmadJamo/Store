# MOD-017..021 foundation bundle

> Status: IN PROGRESS — first usable slice delivered for each phase on 2026-10-25.

This bundle deliberately starts five dependent phases without claiming their broad completion:

- `MOD-017`: completed governed Angular workspace builder, server-authored financial preview and user/role delegation;
- `MOD-018`: completed reusable build/regression/style/bundle/accessibility/browser gate with anonymous and authenticated API evidence plus local performance;
- `MOD-019`: completed source-generated inventory of 89 controllers (79 MVC/10 API), 189 user-facing views,
  344 MVC actions and 198 Razor control rows, plus reversible per-surface cutover controls;
- `MOD-020`: complete Angular functional parity for Settings, Document Design, account/session, current user/role administration,
  public company registration and guided onboarding; MVC remains the committed default;
- `MOD-021`: `MOD-021A` completes product list/details/create/edit/delete with all current product form options;
  remaining catalog masters and recipes are staged in `PHASE_21_CATALOG.md`.

All API endpoints are authenticated, permission protected and call Application services. No EF entity is
returned and no financial value is calculated in the browser. MOD-017C adds one additive tenant-owned delegation
migration; MVC remains the rollback path and provider permission is rechecked on every delegated execution. The
generated registry is in `MVC_MIGRATION_REGISTRY.md` and is refreshed with
`scripts/generate-mvc-migration-registry.ps1`.

## Verification evidence

- Web Release build: passed with zero warnings and zero errors.
- Security/inventory regression: 1186 checks passed, including permission-separated and antiforgery-protected product
  mutations in addition to the completed administration/onboarding controls.
- Angular production build: passed.
- Angular tests: 127/127 passed across 38 files after `MOD-021A`.
- External-style policy: passed; no Angular HTML `style=` attributes or component `styles:` blocks.

## Remaining gates

`MOD-020` functional parity is complete. Product master operations are complete in `MOD-021A`; catalog functional
parity remains open beginning with `MOD-021B` product categories.
