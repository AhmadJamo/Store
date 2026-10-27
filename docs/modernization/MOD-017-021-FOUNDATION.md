# MOD-017..021 foundation bundle

> Status: IN PROGRESS — first usable slice delivered for each phase on 2026-10-25.

This bundle deliberately starts five dependent phases without claiming their broad completion:

- `MOD-017`: completed governed Angular workspace builder, server-authored financial preview and user/role delegation;
- `MOD-018`: completed reusable build/regression/style/bundle/accessibility/browser gate with anonymous and authenticated API evidence plus local performance;
- `MOD-019`: completed source-generated inventory of 89 controllers (79 MVC/10 API), 189 user-facing views,
  344 MVC actions and 198 Razor control rows, plus reversible per-surface cutover controls;
- `MOD-020`: complete Angular functional parity for Settings, Document Design, account/session, current user/role administration,
  public company registration and guided onboarding; MVC remains the committed default;
- `MOD-021`: `MOD-021A..F` complete product master through recipes plus tax/payment/branch masters; warehouse master
  remains staged in `PHASE_21_CATALOG.md`.

All API endpoints are authenticated, permission protected and call Application services. No EF entity is
returned and no financial value is calculated in the browser. MOD-017C adds one additive tenant-owned delegation
migration; MVC remains the rollback path and provider permission is rechecked on every delegated execution. The
generated registry is in `MVC_MIGRATION_REGISTRY.md` and is refreshed with
`scripts/generate-mvc-migration-registry.ps1`.

## Verification evidence

- Web Release build: passed with zero warnings and zero errors.
- Security/inventory regression: 1201 checks passed, including the administration-protected master operations.
- Angular production build: passed.
- Angular tests: 147/147 passed across 49 files after `MOD-021F`.
- External-style policy: passed; no Angular HTML `style=` attributes or component `styles:` blocks.

## Remaining gates

`MOD-020` functional parity is complete. Catalog operations are complete through `MOD-021F`; parity remains open only
for `MOD-021G` warehouse master data in this wave.
