# ADR: Extend StorageLocation into a hierarchy

## Decision
Preserve the existing `StorageLocation` identity and warehouse ownership, and add an optional self-parent, display name, barcode, sequence and explicit operational capabilities. Existing zone/aisle/rack/level/bin fields remain searchable compatibility metadata.

## Rules
- A parent must belong to the same tenant and warehouse.
- A location cannot parent itself or move below a descendant.
- Code and optional barcode remain unique within a warehouse and tenant.
- Capability flags describe allowed future workflows; current stock movement rules remain authoritative until each workflow adopts the flags explicitly.
- Existing rows become roots. Name is backfilled from code, sequence from ID and initial capabilities from location type.

## Consequences
The WMS can model warehouse, zone, aisle, rack and bin trees without replacing IDs referenced by balances, transfers or movement history. Application validation provides friendly cycle checks while the composite tenant foreign key protects tenant isolation at the database boundary.
