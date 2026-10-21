# Database relationships
## Customer return requests

The header references one Posted delivery, order and customer. Lines reference exact delivery/order lines, product, warehouse and optional location. Business foreign keys include `TenantId`; deletes are restrictive.

> Source of truth: EF configurations | Last reviewed: 2026-10-21

`PurchaseDocumentReversals` belongs to one tenant. Its original document is a polymorphic `DocumentType + OriginalDocumentId` reference enforced by the application transaction rather than a database foreign key; a tenant-scoped unique index permits only one reversal per original document.

- Tenant → all ERP business settings and documents: required TenantId, Restrict deletion and tenant query filtering. DocumentSequence additionally has one unique row per tenant/document type. Every FK whose dependent and principal both carry tenant ownership appends `TenantId` on both sides; SQL therefore requires the referenced record to belong to the same company.

- ProductStock → Product and Warehouse: required FK, Restrict deletion.
- StockTransaction → Product and Warehouse: required FK, Restrict deletion.
- Purchase → Supplier and Warehouse: required FK, Restrict; Purchase → PurchaseItems: cascade. PurchaseItem → Product: Restrict.
- PurchaseOrder → PurchaseOrderLines and PurchaseOrderLifecycleHistories: tenant-composite cascade. Lifecycle history is tenant-owned and cannot reference another company's order.
- SupplierCreditNote → VendorBill, PurchaseOrder, Supplier and Warehouse: tenant-composite Restrict; → lines: tenant-composite cascade. Each line references its VendorBillLine, GoodsReceiptLine, Product and optional frozen input-tax Account through tenant-composite Restrict relationships.
- Product → MeasurementUnit: optional only for migrated compatibility rows, Restrict. ProductRecipe → prepared Product: Restrict; ProductRecipe → RecipeIngredients: cascade. RecipeIngredient → stocked Product and authored/stock MeasurementUnits: Restrict. All use composite TenantId boundaries.
- Sale → Warehouse: Restrict; Sale → SaleItems: cascade. SaleItem → Product and optional ProductRecipe version: Restrict.
- StockTransfer → FromWarehouse and ToWarehouse: Restrict; StockTransfer → Items and History: Restrict. Item → Product: Restrict.
- SalesDelivery → SalesOrder, InventoryReservation and Warehouse: tenant-composite Restrict; → Lines and Histories: aggregate-owned tenant-composite relationships. Each line restricts to its exact SalesOrderLine, InventoryReservationLine, Product, Warehouse and optional StorageLocation in the same tenant.
- CustomerInvoice → SalesOrder, Customer, optional CustomerAddress and optional frozen receivable/default-revenue Accounts: tenant-composite Restrict; → Lines and Histories: aggregate-owned tenant-composite Cascade. Each line restricts to its SalesOrderLine, Product, MeasurementUnit, optional SalesPriceList/Rule, TaxRate and frozen output-tax Account; each Delivered-basis source allocation restricts to the exact Posted SalesDelivery and SalesDeliveryLine in the same tenant. Journal linkage is the source-unique `CustomerInvoice`/invoice-number pair; reversal uses `CustomerInvoiceReversal` and references the original JournalEntry.
- CustomerPayment → Customer, PaymentMethod and frozen settlement Account: tenant-composite Restrict; → CustomerPaymentLines: tenant-composite Cascade. Each line restricts to its CustomerInvoice and frozen receivable Account. Journal linkage uses `CustomerPayment`/payment-number; exact reversal uses `CustomerPaymentReversal` with the same reference.
- TenantRole → Tenant: cascade when a company is removed. TenantRolePermission → TenantRole cascades and → Permission restricts. TenantUserRole uses `(TenantRoleId, TenantId)` as a composite FK to the matching company role, restricts role deletion until assignments are removed, and links Tenant/IdentityUser with cascades.

Identity relationships are supplied by IdentityDbContext. AuditLog has no foreign-key relationships.

The local schema contains 65 composite tenant foreign keys and zero single-ID relationships between two TenantId-bearing tables. PromotionRedemption → TenantSubscription follows the same rule in the SaaS control plane.

## Guided onboarding
CompanyOnboarding.TenantId is a one-to-one primary/foreign key to Tenant.Id with cascade delete. Template-created ERP rows remain tenant-owned and receive the standard query, write and composite-relationship safeguards.
# PUR-010 relationships

`SupplierProductPurchasingInfo` references Supplier, Product and MeasurementUnit with Restrict deletion. The tenant-isolation convention rewrites each relationship to include TenantId, so SQL Server rejects cross-company supplier/product/unit references.
