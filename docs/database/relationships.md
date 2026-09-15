# Database relationships
> Source of truth: EF configurations | Last reviewed: 2026-09-15

- Tenant → all ERP business settings and documents: required TenantId, Restrict deletion and tenant query filtering. DocumentSequence additionally has one unique row per tenant/document type. Every FK whose dependent and principal both carry tenant ownership appends `TenantId` on both sides; SQL therefore requires the referenced record to belong to the same company.

- ProductStock → Product and Warehouse: required FK, Restrict deletion.
- StockTransaction → Product and Warehouse: required FK, Restrict deletion.
- Purchase → Supplier and Warehouse: required FK, Restrict; Purchase → PurchaseItems: cascade. PurchaseItem → Product: Restrict.
- Sale → Warehouse: Restrict; Sale → SaleItems: cascade. SaleItem → Product: Restrict.
- StockTransfer → FromWarehouse and ToWarehouse: Restrict; StockTransfer → Items and History: Restrict. Item → Product: Restrict.
- TenantRole → Tenant: cascade when a company is removed. TenantRolePermission → TenantRole cascades and → Permission restricts. TenantUserRole uses `(TenantRoleId, TenantId)` as a composite FK to the matching company role, restricts role deletion until assignments are removed, and links Tenant/IdentityUser with cascades.

Identity relationships are supplied by IdentityDbContext. AuditLog has no foreign-key relationships.

The local schema contains 59 composite tenant foreign keys and zero single-ID relationships between two TenantId-bearing tables. PromotionRedemption → TenantSubscription follows the same rule in the SaaS control plane.

## Guided onboarding
CompanyOnboarding.TenantId is a one-to-one primary/foreign key to Tenant.Id with cascade delete. Template-created ERP rows remain tenant-owned and receive the standard query, write and composite-relationship safeguards.
