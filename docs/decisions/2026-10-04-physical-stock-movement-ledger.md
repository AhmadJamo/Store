# ADR: Additive physical stock movement ledger

## Decision
Introduce `StockMovement` as a tenant-owned, posted physical-movement fact without replacing `LocationMovement`, `ProductLocationStock`, `ProductStock` or `StockTransaction`. The WMS-020 pilot covers Putaway and Relocation only.

Each successful pilot operation updates the existing location balance, writes the existing `LocationMovement`, saves its identity, and writes one linked `StockMovement` inside the same Serializable UnitOfWork transaction. A tenant-unique idempotency key makes an identical repeated form submission a no-op and rejects reuse for different movement details. The database also enforces one StockMovement per legacy LocationMovement.

## Consequences
- Existing history and read paths remain authoritative during the pilot.
- Historic LocationMovement rows are deliberately not backfilled.
- Every new StockMovement has a same-tenant database FK to its legacy movement, product, warehouse and locations.
- The pilot screen is read-only and exposes linkage coverage; it does not repair data.
- Transfer, sale, purchase, reservation, lot/serial and balance-projection integration remain later WMS slices.
