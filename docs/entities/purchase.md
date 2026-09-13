# Purchase and PurchaseItem entities
> Status: IMPLEMENTED | Last reviewed: 2026-09-13

Purchase holds SupplierId, WarehouseId, supplier invoice number, date/notes, total and items and rejects the same product more than once. Item validates product, positive quantity and non-negative price and computes total. Purchases table has unique invoice number, supplier/warehouse Restrict FKs and cascading items. Used by PurchaseService and purchase screens; creation increases stock. No creator, status, posting, cancellation or return fields exist.
