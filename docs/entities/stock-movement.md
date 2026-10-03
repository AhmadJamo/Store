# StockMovement
> Status: WMS-020A PILOT | Last reviewed: 2026-10-04

Tenant-owned immutable posted fact for physical Putaway and Relocation. It stores product, warehouse, optional source location, destination, positive quantity, actor, reference/notes, posted UTC time and a tenant-unique idempotency key. Each pilot row must reference exactly one preserved LocationMovement from the same tenant. Historic location movements are not backfilled. Reversal behavior and additional movement types are not implemented yet.
