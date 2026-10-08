# Database tables reference
> Last reviewed: 2026-10-08

Schema source is `AppDbContext`, configurations and migrations. Identity's standard `AspNet*` tables are supplied by IdentityDbContext.

- **InventoryReservations:** auditable source-owned Active/Consumed/Released inventory holds with rowversion and tenant-unique source identity.
- **InventoryReservationLines:** positive product/warehouse/exact-location or Unassigned allocations that own `InventoryBalance.Reserved`.
- **InventoryAdjustments:** numbered blind/visible count documents and immutable workflow actor/timestamp evidence.
- **InventoryAdjustmentLines:** frozen expected, counted and derived variance quantities per product.
- **InventoryTrackingBalances:** lot/serial quantities, dimensional position, dates, status, source and concurrency token.
- **InventoryTrackingTransactions:** immutable lot/serial trace events linked to their balance.

- **Tenants:** company identity, unique URL-safe Slug, active state, creation time and RowVersion.
- **TenantMemberships / TenantRoles / TenantRolePermissions / TenantUserRoles:** company membership, tenant-owned role definitions and permission/user assignments; Admin status is evaluated inside the active company.
- **Plans / PlanFeatures / PlanLimits / TenantSubscriptions:** bilingual plan catalogue, entitlements and one lifecycle-aware subscription per company.
- **PlatformOperators:** explicit platform-owner/admin/billing allow-list, separate from tenant Admin.
- **PromotionCodes / PromotionRedemptions:** percentage promotions bounded by time, plan, total use and one use per company.
- **BillingCheckoutSessions:** priced monthly/annual quotes with expiry, promotion, status, confirmation reference and rowversion.
- **Tenant ownership:** every ERP business table below also carries required TenantId with a restrictive Tenant FK. Tenant-aware unique indexes allow each company its own codes, invoice numbers and singleton settings. All relationships between TenantId-bearing records use matching composite foreign keys; the database rejects cross-company references.

- **Products:** Id, SQL-computed ProductCode, optional Barcode, Name, prices, ProductType, InventoryBehavior, StockUnit, optional ProductCategory, nullable net/gross weight and dimensions with managed units, handling flags, None/Lot/Serial policy, POS/sales channel flags, IsActive and AllowNegativeRecipeConsumption. Existing products keep their IDs and default to unknown logistics and no tracking.
- **ProductCategories:** tenant-local unique code/name, active state and rowversion; referenced optionally by products through a tenant-safe Restrict FK.
- **ProductRecipes / RecipeIngredients:** immutable product recipe versions, yield, creator/time and active state; ingredient lines store a stocked product, authored quantity/unit and converted stock quantity/unit snapshot. One active version is allowed per tenant/product.
- **MeasurementUnits:** tenant-local unit code/name/symbol, Count/Mass/Volume/Length dimension, decimal(24,12) factor to base, precision, system/active flags. Built-ins are protected from deactivation. Products and recipe ingredients use tenant-safe foreign keys; recipe rows additionally preserve unit-code/factor snapshots.
- **Accounts / Branches:** chart account code/name/type/parent and branch code/name with optional SalesRevenueAccountId subaccount mapping.
- **JournalEntries / JournalEntryLines:** entry number/status/date/description, optional source type/reference protected against duplicate posting, and balanced account debit-credit lines with optional branch/warehouse.
- **Warehouses:** Id, Name, BranchId?, InventoryAccountId?, Type, ControlMode, PickingStrategy, AllowPosSales, EnforceLocationCapacity and source/destination transfer-location requirements.
- **BranchWarehouseAccesses:** composite BranchId/WarehouseId key, priority, branch POS default and POS/purchase/transfer/replenishment permissions.
- **PosTerminalWarehouses:** composite PosTerminalId/WarehouseId terminal allow-list with priority; each terminal retains DefaultWarehouseId.
- **PosTerminalSettings:** one-to-one shared-key settings for each POS terminal; stores experience profile, product layout, theme, cart position, accent/header, grid density, operator visibility/touch preferences and order workflow flags/defaults with RowVersion concurrency.
- **StorageLocations:** WarehouseId, ParentLocationId?, warehouse-unique Code and optional Barcode, Name, Sequence, Zone?/Aisle?/Rack?/Level?/Bin?, Type, Status, MaximumQuantity? and receive/pick/reserve/ship/count capability flags. The self-reference is tenant-safe and restrictive on delete.
- **ProductAttributeDefinitions / ProductAttributeOptions / ProductCategoryAttributes:** tenant-owned typed attribute dictionary, ordered selection choices and category applicability. Codes are tenant-safe unique and relationships use tenant-composite foreign keys.
- **ProductAttributeValues:** one tenant-safe typed value per Product + definition. Nullable typed columns are protected by a database check requiring exactly one value; selected options use tenant-composite foreign keys.
- **ProductTemplates / Product variant columns:** category-owned template name/code/state plus Product.TemplateId?, VariantSignature? and VariantLabel?. A filtered tenant-safe unique index prevents repeated signatures within one template.
- **Suppliers:** Id, Name, Phone?, Address?, AccountId?.
- **Customers:** Id, Name, Phone?, Address?, AccountId.
- **TaxRates:** Name, Rate, OutputAccountId, InputAccountId, IsPriceInclusive.
- **AccountingSettings:** singleton posting links for purchase discount, sales discount, sales revenue and cost of sales.
- **PaymentMethods:** Name, AccountId, IsActive; maps cash, bank, card or similar settlement method to a chart account.
- **ProductStocks:** Id, ProductId, WarehouseId, Quantity decimal(18,6), AverageUnitCost, InventoryValue and LastReferenceUnitCost decimal(24,8); unique product/warehouse. Negative values are permitted only through controlled recipe/kitchen methods and retain provisional value.
- **ProductLocationStocks:** ProductId, WarehouseId, StorageLocationId, Quantity and RowVersion; unique product/location allocation.
- **LocationMovements:** immutable putaway/relocation audit rows with ProductId, WarehouseId, FromStorageLocationId?, ToStorageLocationId, Quantity, Type, Reference?, Notes?, CreatedByUserId and CreatedAt.
- **StockTransactions:** immutable quantity/valuation movement with quantity, average and value before/after, unit cost, transaction value, cost variance, Type, Reference and CreatedAt; recipe consumption and kitchen variance have distinct types.
- **Purchases:** header plus items containing ProductId, WarehouseId?, Quantity, PurchasePrice, DiscountAmount, TaxRateId? and Total; new invoices select warehouse per item.
- **PurchaseRequests / PurchaseRequestLines / PurchaseRequestHistories:** numbered internal demand with destination, priority, needed date, frozen unit conversion, explicit lifecycle actors and immutable action history.
- **PurchaseApprovalRules / PurchaseApprovalRuleSteps:** editable warehouse/priority-scoped configuration with ordered tenant-role name steps and rowversion concurrency.
- **PurchaseApprovalInstances / PurchaseApprovalSteps:** one frozen approval plan per request, preserving the selected rule name, ordered role requirements, decisions, actor, timestamp and reason/note.
- **Sales:** Id, InvoiceNumber, Channel, CreatedByUserId, CreatedAt, WarehouseId, PosTerminalId?, CustomerId?, PaymentMethodId?, TaxRateId?, TaxRatePercent, TaxOutputAccountId?, IsTaxInclusive, TaxAmount, Date, Notes?, PosOrderType?, ServiceReference?, GuestCount? and totals; SaleItems may hold preparation notes and the ProductRecipeId version used for prepared-item consumption.
- **SalesReturns / SalesReturnItems:** immutable return number/date/reason linked to original Sale, warehouse and payment method; header/lines freeze revenue, discount, output tax, refund and eligible historical restock cost. Lines reference original SaleItem and Product, and cumulative quantity is enforced by the Application transaction.
- **StockTransfers:** identity, transfer number, source/destination warehouse, status, actor/time/reason fields; each item may identify optional exact source and destination storage locations.
- **Permissions:** global technical permission catalogue selected by tenant-owned role mappings.
- **AuditLogs:** EntityName, EntityId, Action, UserId, CreatedAt.
- **GeneralSettings / DiscountSettings / InventorySettings:** company settings; GeneralSettings stores the English/Arabic default UI language and InventorySettings stores rowversion-protected defaults for new warehouse operating policies.
- **DocumentSequences:** tenant-specific numbering for wholesale sales, POS sales, stock transfers and journal entries. Each row stores prefix, suffix, tokenized format, padding length, next number, reset start/period/current period and rowversion.

See `03_DATABASE.md` for constraints and `entities/*.md` for behaviour.

## CompanyOnboardings
One row per tenant stores guided-setup status and choices, template/audit metadata and rowversion. TenantId is both the primary key and cascading FK to Tenants. This is explicitly tenant-addressed control-plane state.
### StockMovements
Additive physical movement facts. Putaway/Relocation rows link to preserved LocationMovement. Transfer rows use stable StockTransfer document, line and stage identity and represent Planned/Posted/Reversed outbound, transit and inbound stages. Related warehouse and optional locations support simple, hybrid and location-managed transfers. The table does not replace ProductStock, ProductLocationStock or valuation StockTransaction.
### InventoryBalances
Rebuildable current-position projection by Product + Warehouse + optional StorageLocation. Null location is virtual Unassigned. Stores OnHand, Reserved, UpdatedAt and RowVersion; Available is computed in the domain/application layer. Filtered tenant-aware unique indexes protect both null and exact-location dimensions. It is synchronized in the same persistence transaction as legacy balance changes.

### ReplenishmentRules
Tenant-owned Product + destination Warehouse planning policy with minimum, maximum, safety stock, lead days, optional preferred source Warehouse and active state. It produces suggestions only and stores no forecast or stock movement.
# PUR-010 table addition

`SupplierProductPurchasingInfos` owns editable supplier/product commercial master data: supplier/product/unit references, supplier product code/description, minimum/order-multiple quantities, lead time, base-currency price, validity, preferred/priority/active flags, rowversion and shadow TenantId. It is not a price-history ledger and is never used as a substitute for a confirmed document snapshot.
