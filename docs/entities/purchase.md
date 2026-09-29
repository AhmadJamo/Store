# Purchase and PurchaseItem entities
> Status: IMPLEMENTED | Last reviewed: 2026-09-13

Purchase holds SupplierId, WarehouseId, supplier invoice number, date/notes, total and items and rejects the same product more than once per warehouse. Item validates product, positive quantity and non-negative price and computes total. Purchases table has a tenant-unique invoice number, supplier/warehouse Restrict FKs and cascading items. Creation immediately receives stock; explicit journal posting must precede a supplier return.

PurchaseReturn and PurchaseReturnItem are immutable posted reversal snapshots linked to the original purchase/items. They preserve proportional payable, discount, input-tax and original-inventory amounts plus actual moving-average inventory cost removed. Cumulative quantity cannot exceed the original line, and available warehouse stock cannot go negative. Return/item relationships are tenant-enforced and history is retained with Restrict deletion.
