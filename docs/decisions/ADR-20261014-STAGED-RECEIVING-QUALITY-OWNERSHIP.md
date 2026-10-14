# ADR: Staged receiving quality evidence and inventory ownership

Date: 2026-10-14
Status: Accepted

## Decision

Each warehouse configures one purchase receiving route: One Step or Receive/Inspect/Store. A Goods Receipt freezes that route when created so later warehouse-setting changes cannot alter an in-flight document.

Purchasing owns delivered, accepted, rejected, damaged and short evidence plus the discrepancy reason. Inventory remains the only stock mutation owner. A staged receipt and its quality decision create no ProductStock, tracking balance, StockTransaction, physical StockMovement or journal. The separately authorized storage action sends only accepted quantity through the existing atomic inventory/tracking/movement and Inventory/GRNI posting path.

For compatibility with downstream billing, returns and Purchase Order progress, `ReceivedQuantity` becomes the accepted quantity after inspection while `DeliveredQuantity` preserves the physical delivery evidence. Serial-tracked lines are fully accepted or fully rejected until accepted-serial selection is explicitly implemented.

## Consequences

- Rejected, damaged and short quantities never become available stock.
- Accounting recognizes only accepted quantity at storage, not at dock receipt or inspection.
- Fully rejected receipts finish without a storage action or financial effect.
- The route snapshot and quality evidence are auditable even if warehouse configuration changes later.
- Demand allocation and dropshipping remain a separate PUR-250B slice; this decision does not create internal stock for dropshipping.
