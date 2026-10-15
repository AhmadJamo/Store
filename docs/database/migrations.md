# Migrations and seeding
> Last reviewed: 2026-10-16

## 20261015173055_AddAuditEventFoundation

Creates the empty additive `AuditEvents` table for Tenant, Platform and System semantic audit evidence. It adds trusted scope/tenant, classification and JSON checks; unique EventId and scope/tenant/OperationId protections; correlation, resource and tenant-time indexes; and a restrictive optional Tenant foreign key. It does not alter or backfill `AuditLogs`. Applied to local `MiniStoreDb` on 2026-10-16 as migration 113; post-apply inspection found 0 new rows, 707 legacy rows, 3 trusted checks and 6 enabled indexes.

The generated Down path may be used only before the feature is enabled and only while `AuditEvents` is proven empty. After the first event exists, rollback means disabling the feature flag and applying a forward fix; deployment procedures must not execute the destructive Down path or delete audit evidence.

## 20261013024801_AddPurchaseDocumentReversals

Creates the empty tenant-owned `PurchaseDocumentReversals` audit table with tenant-unique reversal number, request key and original type/id. Adds `LandedCosts.Status` and backfills existing records to Posted. The migration was applied successfully to local `MiniStoreDb` on 2026-10-13.

## 20261012122525_AddPurchaseOrderDerivedClosure

Adds nullable current closure type/reason/actor/time columns to existing Purchase Orders and creates the empty tenant-owned `PurchaseOrderLifecycleHistories` table with a tenant-composite cascade relationship. It performs no historical inference or backfill. The model has no pending changes and the migration was applied successfully to the local `MiniStoreDb` on 2026-10-12.

## 20261012125000_AddSupplierCreditNotes

Creates empty tenant-owned Supplier Credit Note headers and lines with rowversion, tenant-unique numbering and composite Vendor Bill, receipt, product, supplier, warehouse and account relationships. It performs no bill, stock or journal backfill. The migration was applied successfully to the local `MiniStoreDb` on 2026-10-12.

Migrations evolve the schema chronologically from the initial ERP through accounting, inventory/POS, localization and tenancy. The current tail includes `EnforceTenantRoleAssignmentBoundary`, `HardenTenantIsolationRelationships` and `HardenTenantSubscriptionRedemption`. The hardening migrations add alternate tenant keys and composite foreign keys across ERP relationships and the subscription-redemption relationship. Both applied successfully to the local two-company test database. `AppDbContextModelSnapshot.cs` is the latest EF model snapshot.

Startup invokes Identity, permission and platform-operator seeders. There is no code call to `Database.Migrate`; deployment must apply migrations explicitly. `UnifyDocumentNumbering` copies legacy sales/POS/transfer settings, inserts any missing tenant defaults and then removes the legacy tables. Runtime generation also creates a missing sequence safely inside the calling document transaction.

## 20260915140248_AddCompanyGuidedOnboarding
Creates one onboarding-state row per tenant and backfills existing development companies as Skipped so their current manual setup remains unchanged. New registration inserts Pending explicitly.

## 20260927221520_AddCafeRecipesAndControlledNegativeStock
Adds stocked/prepared product behavior, base stock unit and per-ingredient controlled-negative policy; creates tenant-isolated immutable ProductRecipes and RecipeIngredients; snapshots ProductRecipeId on SaleItems; and expands ProductStock/StockTransaction quantities to decimal(18,6). Existing products backfill as stocked pieces with negative recipe consumption disabled. Its SQL/model were validated and the migration was applied successfully to local `MiniStoreDb` on server `AHMAD` on 2026-09-28; a repeated EF update reported the database already up to date.

## 20260928133432_AddProductCatalogControls
Adds SQL-generated ProductCode, optional tenant-unique barcode, raw/direct/prepared classification, active state and POS/sales channel flags. Existing stocked products backfill as active direct-sale products; existing prepared products remain prepared and their direct PurchasePrice is set to zero. Applied successfully to local `MiniStoreDb` on server `AHMAD` on 2026-09-28.

## 20260928140113_AddManagedMeasurementUnits
Creates the tenant-isolated managed measurement-unit catalogue with code, dimension, conversion factor, precision and protected system/active state. Applied successfully to local `MiniStoreDb` on server `AHMAD` on 2026-09-28. Built-ins are initialized by the application when the unit settings page is first opened for a company.

## 20260928145232_ConnectManagedUnitsToProductsAndRecipes
Adds tenant-safe managed-unit foreign keys to Products and RecipeIngredients, immutable recipe unit-code/factor snapshots and compatibility backfill. It idempotently initializes the eight built-in units for every existing tenant, maps legacy stock/recipe units, and repairs the alternate tenant key required by older local schemas. Applied successfully to local `MiniStoreDb` on server `AHMAD` on 2026-09-28.

## 20260928153229_AddMovingWeightedAverageInventoryCost
Adds product/warehouse average cost, inventory value and reference cost; full before/after cost snapshots and variance on stock movements; and unit-cost/COGS snapshots on sale lines. Existing balances initialize from Product.PurchasePrice while historical movements remain zero-valued because their original cost cannot be reconstructed reliably. Applied successfully to local `MiniStoreDb` on server `AHMAD` on 2026-09-28.

## 20261006205205_AddReplenishmentRules
Creates the empty tenant-isolated replenishment-policy table and tenant-safe Product/source/destination relationships. It performs no inventory backfill or document creation. Applied to `AHMAD/MiniStoreDb` on 2026-10-06.
# 2026-10-08 — AddSupplierPurchasingData

Creates only `SupplierProductPurchasingInfos`, its tenant-safe foreign keys, indexes and rowversion. It performs no data backfill and does not change legacy Purchase records. Generation and pending-model verification passed. Initial sandbox attempts could not use the developer's Windows SQL identity; the authorized host execution applied migration `20261008161855_AddSupplierPurchasingData` successfully to `AHMAD/MiniStoreDb`.

## 2026-10-08 — AddPurchaseRequestFoundation

Migration `20261008180612_AddPurchaseRequestFoundation` creates PurchaseRequests, lines and histories with tenant-safe composite relationships, request-number and line-identity uniqueness, rowversion and frozen quantity conversion. It was applied successfully to `AHMAD/MiniStoreDb`; it changes no inventory or accounting table.
