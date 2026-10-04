# InventoryBalance
> Status: IMPLEMENTED PROJECTION | Last reviewed: 2026-10-04

Current physical and availability projection per tenant Product + Warehouse + optional StorageLocation. Null StorageLocation is the protected virtual Unassigned position. `Available` is computed as `OnHand - Reserved`. Controlled negative OnHand is allowed only with zero Reserved. RowVersion protects future reservation concurrency. ProductStock remains AVCO/warehouse authority and ProductLocationStock remains compatibility input until read cutover is approved.
