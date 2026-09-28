# Product catalogue classification and internal coding

## Decision
Use a database-generated, language-neutral `ProductCode` as MiniStore's internal identity label and keep manufacturer barcode optional. Do not introduce an external SKU until an integration contract requires it. Classify products as RawMaterial, DirectSale or PreparedToOrder, retain inventory behavior for stock execution, and store separate POS and ordinary-sales availability flags plus active state.

Prepared products have no direct purchase price or finished-item stock; purchases and opening balances target their ingredients. New sale lines now derive and freeze recipe cost from moving-weighted-average ingredient valuation. Raw materials are not directly sellable. UI filtering is backed by service validation so crafted requests cannot bypass product/channel rules.

## Consequences
Existing stocked products backfill as active direct-sale products and existing prepared products remain prepared with PurchasePrice zero. ProductCode is derived from the database identity as `PRD-########`; it is not an integration SKU. Future unit management, valuation, POS assortment and import/export work can rely on stable product classification without reinterpreting historical products.
