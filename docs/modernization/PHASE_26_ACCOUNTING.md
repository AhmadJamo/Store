# Phase 26 — Accounting and close Angular migration

> Status: COMPLETE — `MOD-026A..B` COMPLETE on 2026-10-29
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

## `MOD-026B` delivered — journal entry details

- Added a read-only authenticated Journal Entry details API over `JournalEntryQueryService` with `JournalEntries.View`.
- Added a bilingual RTL-safe Angular evidence screen for entry status/date/source, named accounts, branch/warehouse dimensions,
  line descriptions, debit/credit and balanced totals.
- No mutation was introduced; journal source uniqueness, posting and reversal authority remain in accounting services.

## Evidence

- Web Release: zero warnings/errors; SecurityRegression: 1302 checks.
- Angular: 203/203 tests across 98 files; 110 templates passed accessibility; external SCSS passed.
- Bundles: 318,598/358,400 initial bytes and 81,743/81,920 largest lazy bytes.

## Scope closure

The current accounting settings, fiscal periods, taxes, payment methods and branch account mappings were already migrated in earlier
slices. `MOD-027A` starts the remaining SaaS presentation with tenant subscription and checkout.
