# MOD-020 — Administration, settings, identity and onboarding

> Status: FUNCTIONAL PARITY IMPLEMENTED — 2026-10-27  
> Presentation default: MVC (unchanged; Angular cutover requires a later explicit gate)

## Purpose

`MOD-020` moves the complete current administration wave to Angular without moving business authority into the browser. It covers company settings, document design, account/session, tenant users and roles, public company registration, and guided company onboarding. MVC remains the functional rollback path.

## Non-negotiable rules

- Controllers call Application services; Angular and API controllers never access EF directly.
- API contracts use DTOs and stable codes, never persistence entities.
- Every unsafe request is same-origin and antiforgery protected; login and registration retain independent rate limits.
- Tenant and operation permissions are repeated at the endpoint. Navigation visibility is not authorization.
- Passwords, access tokens, permission decisions and business form payloads are never persisted in browser storage.
- Every changed user-facing message exists in English and Arabic; layouts support RTL/LTR and use external SCSS.
- Failed mutations preserve correctable non-secret input, never retry automatically, and return Problem Details.
- MVC stays the committed default until the registry, dual-run and cutover gates in `MOD-029..030` pass.

## Completed slices

| Slice | Completed capability |
|---|---|
| `MOD-020A..Q` | All current Settings operations: regional/organization identity, logo/custom fields, discounts, purchase matching, inventory and sourcing policy, numbering, fiscal periods, exchange rates, units, accounting, branch accounts, inventory/POS access, POS profile and workflow catalog. |
| `MOD-020R` | Document Design list/details/create/clone/editor plus activate/archive/default lifecycle. |
| `MOD-020S` | Cookie-backed login/logout, company-code selection, access denied, expiry recovery and session shell evidence. |
| `MOD-020T` | Current tenant-user list/create and tenant-role list/create/edit/delete, including system-role and concurrency protection. |
| `MOD-020U` | Public registration, selected-plan 14-day trial, automatic sign-in, business preset selection, transactional complete, explicit skip and pending-setup resume. |

Registration and login share Web services between MVC and API so cookie issuance, tenant claims and provisioning do not drift. Guided setup continues to use the existing serializable `CompanyOnboardingService`; Angular supplies choices but cannot create accounts, branches, warehouses, tax, payments, numbering or POS records itself.

## Verification evidence

- Web Release: zero warnings and zero errors.
- Security/inventory regression: 1182 checks.
- Angular: 122/122 tests across 37 files.
- Static accessibility: 50 templates; external-SCSS policy passed.
- Production bundles: 297,465 / 358,400 initial bytes; largest lazy chunk 80,866 / 81,920 bytes.

## What remains

- `MOD-021` completes catalog presentation, starting with the remaining Product create/edit/delete operations.
- Existing-user assignment editing, invitations and role data scopes remain future identity capabilities because the MVC source does not implement them.
- `MOD-029` must prove whole-system dual-run parity; `MOD-030` is the only gate allowed to make Angular the sole presentation and retire MVC screens.

The authoritative future sequence is `FULL_MVC_TO_ANGULAR_MIGRATION_PLAN.md`; the single executable next increment is always `../project-control/NEXT_TASK.md`.
