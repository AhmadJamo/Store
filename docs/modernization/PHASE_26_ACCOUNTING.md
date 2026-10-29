# Phase 26 — Accounting and close Angular migration

> Status: IN PROGRESS — `MOD-026A` COMPLETE on 2026-10-29
>
> Presentation default: MVC until a separate cutover approval.

## Scope

Move the current chart, journal evidence, fiscal-period and accounting reconciliation presentation to Angular while all account,
posting, source uniqueness, balance and close authority remains in Application/Domain services.

## `MOD-026A` delivered — chart of accounts

- Added an Application-authored account page projection and authenticated list/create API under `Administration.Access`, with
  antiforgery on creation and no persistence access from the controller.
- Added a bilingual RTL-safe Angular chart showing code, name, localized type and named parent plus top-level/subaccount creation.
- `AccountService` remains authoritative for duplicate codes, parent existence and entity validation.

## Evidence

- Web Release: zero warnings/errors; SecurityRegression: 1302 checks.
- Angular: 202/202 tests across 97 files; 109 templates passed accessibility; external SCSS passed.
- Bundles: 318,327/358,400 initial bytes and 81,743/81,920 largest lazy bytes.

## Next slice

`MOD-026B` migrates immutable Journal Entry details with named accounts, dimensions and source evidence.
