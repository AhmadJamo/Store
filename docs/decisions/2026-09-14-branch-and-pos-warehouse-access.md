# Branch and POS warehouse access

## Status
Accepted — 2026-09-14

## Context
A branch may sell from a local sales floor, replenish from its backroom and optionally access a central warehouse. A POS must not gain access to every POS-enabled warehouse merely because it exists.

## Decision
`BranchWarehouseAccess` is the branch boundary. It stores priority, branch POS default and permissions for POS sales, purchases, transfer directions and replenishment. `PosTerminalWarehouse` is the narrower terminal allow-list with its own priority; `PosTerminal.DefaultWarehouseId` must be included in that list.

POS sale creation validates the warehouse, branch access and selected active terminal. The UI filters warehouse choices by terminal and selects its default. Installations with no configured terminals retain legacy POS operation; once any terminal exists, terminal selection is mandatory. Migration backfills links where source data exists.

## Consequences
Different terminals in one branch can use different sources without exposing every warehouse. Replenishment can later use the same access records to find permitted source and destination warehouses.
