# Configurable inventory operating policies

## Status
Accepted — 2026-09-14

## Context
MiniStore must support small outlets that need only a warehouse balance and large stores whose sales floor, backroom and central warehouse use structured locations. A warehouse's business use does not determine whether it may supply a POS. The same product may occupy multiple exact locations inside one warehouse while retaining one warehouse-level balance.

## Decision
Warehouse configuration separates `WarehouseType` from `InventoryControlMode`. Type describes operational use; control mode is Simple, LocationManaged or Hybrid. POS eligibility, picking strategy, capacity enforcement and transfer-location requirements are independent warehouse policies.

`InventorySettings` is a rowversion-protected singleton containing defaults for newly created warehouses. Warehouse settings remain the effective policy. Existing warehouses are migrated to Hybrid with location-priority picking and capacity enforcement to preserve current location workflows.

Simple warehouses cannot own new storage locations or accept exact locations on transfers. Structured warehouses participate in Unassigned Stock and internal location movements. Changing a warehouse to Simple is rejected while it has locations or location allocations.

## Consequences
The model supports a simple restaurant store, an organized mall sales floor and a central distribution warehouse without hard-coded facility profiles. POS warehouse priority, automated FIFO/FEFO allocation and replenishment workflows can build on these persisted policies in later phases.
