# StockMovement
> Status: WMS-020A PILOT | Last reviewed: 2026-10-04

Tenant-owned physical movement aggregate. Putaway and Relocation are created Posted and link to one preserved LocationMovement. TransferOutbound, TransferTransit and TransferInbound are created Planned from an approved StockTransfer line, become Posted with the atomic stock transfer and become Reversed with cancellation. It stores primary/related warehouse, optional locations, positive quantity, actors/timestamps, neutral source-document identity and a tenant-unique idempotency key. Historic movements/transfers are not backfilled.
