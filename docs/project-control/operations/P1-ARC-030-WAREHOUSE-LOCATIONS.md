# P1-ARC-030 — Warehouse-location application boundary

> Status: COMPLETE — 2026-10-18  
> Scope: one bounded read/presentation slice; no database or business-rule change

## Problem

`WarehouseLocationsController` depended directly on `IWarehouseRepository` to prepare selector data. Its Razor views
also received `Warehouse` and `StorageLocation` domain entities. This crossed the Application boundary and made the
page perform an unnecessarily broad product/stock search merely to load parent-location choices.

## Change

- `StorageLocationService` now owns warehouse and parent-location page options.
- New application DTOs project warehouse options, parent options and location rows.
- The controller depends only on `StorageLocationService` plus localization; it has no warehouse/location repository.
- Index/Create/Edit views consume application DTOs rather than EF-backed domain entities.
- The existing create/update persistence path and its repository save boundary are unchanged.

## Verification

- Release solution build: zero warnings and zero errors.
- Focused regression: 780 checks passed. Three new checks prevent direct repository reinjection, domain-location
  exposure and removal of the application-owned page-options contract.
- Runtime composition/readiness and 500-request availability smoke passed; p95 16.36 ms and p99 45.58 ms.
- SQL integration was not rerun for this projection-only slice because no entity, mapping, transaction, schema or
  persistence behavior changed. The immediately preceding P1-OBS-020 full SQL gate remains green.

## Rollback

Code-only rollback restores the controller repository dependency and entity-shaped view data. There is no migration,
data conversion or configuration rollback.
