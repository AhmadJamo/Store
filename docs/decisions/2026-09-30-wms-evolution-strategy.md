# WMS evolution without inventory rewrite

Date: 2026-09-30  
Status: Accepted as implementation direction

## Context

MiniStore already has warehouse-level AVCO balances and valuation movements, exact storage locations, location balances, putaway/relocation history, structured warehouse policies and a mature StockTransfer approval/cancellation workflow. A full replacement would risk quantity, valuation and audit history. The target WMS needs a unified physical movement ledger, dimensional balances, reservations and traceability that the current warehouse-level StockTransaction cannot safely represent alone.

## Decision

Evolve additively. Keep business documents and their workflows. Keep ProductStock as the warehouse AVCO authority. Introduce a physical StockMovement ledger beside StockTransaction, then a transactionally maintained InventoryBalance projection after movement behavior is stable. Extend existing StorageLocation IDs into a tree rather than replacing them. Convert implicit Unassigned quantity into a protected explicit location only after reconciliation. Migrate workflows vertically with dual-write comparison and feature-gated read cutover. Posted legacy history is preserved and never fabricated into unsupported location/lot/serial detail.

## Alternatives rejected

1. Replace StockTransaction immediately: rejected because it combines valuable valuation snapshots with years of document references and has no safe source/destination backfill.
2. Make StockTransaction the full WMS movement record: rejected because one warehouse row cannot naturally represent source and destination, multi-step transit, reservation lifecycle or future dimensional allocations without destabilizing current valuation semantics.
3. Replace StorageLocation with a new tree table immediately: rejected because current IDs are referenced by balances, transfer items and movement history; an in-place additive extension is safer.
4. Treat ProductStock, ProductLocationStock and a new InventoryBalance as independent authorities: rejected because duplicated editable truth would be unreconcilable.

## Consequences

The transition temporarily carries compatibility ledgers and requires atomic dual-write plus reconciliation. Delivery is slower than a rewrite but rollback is practical and existing workflows/data remain usable. Deprecating old projections or histories requires a later ADR after all writers/readers migrate and production reconciliation is proven.
