# ADR: Transactional inventory balance projection

## Decision
Add tenant-owned `InventoryBalance` rows by Product + Warehouse + optional StorageLocation. `OnHand` is physical quantity, `Reserved` is active reservation quantity, and `Available = OnHand - Reserved`. The null location is a protected virtual Unassigned position and has its own filtered unique index; users cannot edit or delete it as a storage location.

`ProductStock` remains the warehouse quantity and AVCO authority during migration. `ProductLocationStock` remains the exact-location compatibility source. Before every save that changes either legacy balance, AppDbContext deterministically refreshes affected InventoryBalance rows inside the same transaction. This central persistence projection prevents purchase, sale, return, adjustment, recipe, transfer or location workflows from omitting the new projection.

## Migration and safety
The migration refuses backfill when assigned location quantity exceeds a non-negative warehouse balance beyond tolerance. It then copies exact-location rows and calculates Unassigned as warehouse quantity minus assigned quantity. It does not change either legacy source table. Controlled negative warehouse balances are represented as negative Unassigned with zero Reserved.

## Consequences
- InventoryBalance is a rebuildable projection, never an independent movement ledger or costing source.
- Reserved starts at zero; WMS-050 owns reservation mutation.
- Database and domain constraints reject negative reservation and reservation above non-negative OnHand.
- ProductStock/ProductLocationStock read cutover or removal is explicitly deferred until sustained reconciliation proves safe.
