# ADR: Opening lot and serial allocation

Date: 2026-10-04  
Status: Accepted

Existing non-zero stock cannot switch to Lot or Serial tracking until every dimensional InventoryBalance is allocated. The opening workflow therefore requires exact coverage of all warehouse/location positions and commits tracking balances, immutable opening history and the Product policy atomically. Negative stock blocks activation. Serial identifiers are unique per tenant product and always hold zero or one unit; lot identifiers may span locations. Manufacture and expiration dates are immutable receipt metadata.

This is WMS-070A. New receipts, issues, transfers and returns remain blocked from claiming full tracked-operation support until WMS-070B integrates their document inputs and allocation rules.
