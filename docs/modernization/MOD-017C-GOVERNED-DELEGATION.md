# MOD-017C governed analytics delegation

> Status: COMPLETED IN SOURCE — 2026-10-25

## Delivered

- time-bounded, revocable delegation to one active tenant user or tenant role;
- owner or `AnalyticsCenter.Manage` administration behind `AnalyticsCenter.Customize` endpoints;
- creator/revoker/time evidence, duplicate-current protection, parent revision/rowversion concurrency and a 100-current-grant bound;
- tenant-filtered availability for the current user and current tenant roles;
- provider/source permission recheck on every execution, including delegated workspaces;
- server-supplied user/role choices and a bilingual RTL/LTR Angular management screen with external SCSS;
- additive source migration `20261025025522_AddAnalyticsWorkspaceDelegation` with tenant-composite workspace and role relationships.

## Verification

- Web Release build: zero warnings and zero errors.
- Angular production build: passed.
- Angular tests: 61/61 passed.
- Security/inventory regression: 1106 checks passed.
- EF reports no pending model changes after the additive migration.
- The migration is present in source; it has not been applied to the local Demo database.

## Deliberate boundaries

Delegation does not grant module permissions, expose arbitrary fields, generate files, deliver email or schedule a
background report. Those capabilities require their own governed contracts and acceptance gates.
