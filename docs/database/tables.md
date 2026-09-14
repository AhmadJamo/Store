# Database tables reference
> Last reviewed: 2026-09-15

Schema source is `AppDbContext`, configurations and migrations. Identity's standard `AspNet*` tables are supplied by IdentityDbContext.

- **Tenants:** company identity, unique URL-safe Slug, active state, creation time and RowVersion.
- **TenantMemberships / TenantUserRoles:** company membership plus tenant-scoped role assignment; Admin status is evaluated inside the active company.
- **Plans / PlanFeatures / PlanLimits / TenantSubscriptions:** bilingual plan catalogue, entitlements and one lifecycle-aware subscription per company.
- **PlatformOperators:** explicit platform-owner/admin/billing allow-list, separate from tenant Admin.
- **PromotionCodes / PromotionRedemptions:** percentage promotions bounded by time, plan, total use and one use per company.
- **BillingCheckoutSessions:** priced monthly/annual quotes with expiry, promotion, status, confirmation reference and rowversion.
- **Tenant ownership:** every ERP business table below also carries required TenantId with a restrictive Tenant FK. Tenant-aware unique indexes allow each company its own codes, invoice numbers and singleton settings.

- **Products:** Id, Name, Barcode, PurchasePrice, SalePrice, WholesalePrice; all prices decimal(18,2).
- **Accounts / Branches:** chart account code/name/type/parent and branch code/name with optional SalesRevenueAccountId subaccount mapping.
- **JournalEntries / JournalEntryLines:** entry number/status/date/description, optional source type/reference protected against duplicate posting, and balanced account debit-credit lines with optional branch/warehouse.
- **Warehouses:** Id, Name, BranchId?, InventoryAccountId?, Type, ControlMode, PickingStrategy, AllowPosSales, EnforceLocationCapacity and source/destination transfer-location requirements.
- **BranchWarehouseAccesses:** composite BranchId/WarehouseId key, priority, branch POS default and POS/purchase/transfer/replenishment permissions.
- **PosTerminalWarehouses:** composite PosTerminalId/WarehouseId terminal allow-list with priority; each terminal retains DefaultWarehouseId.
- **PosTerminalSettings:** one-to-one shared-key settings for each POS terminal; stores experience profile, product layout, theme, cart position, accent/header, grid density, operator visibility/touch preferences and order workflow flags/defaults with RowVersion concurrency.
- **StorageLocations:** WarehouseId, unique Code within warehouse, Zone?, Aisle?, Rack?, Level?, Bin?, Type, Status and MaximumQuantity?.
- **Suppliers:** Id, Name, Phone?, Address?, AccountId?.
- **Customers:** Id, Name, Phone?, Address?, AccountId.
- **TaxRates:** Name, Rate, OutputAccountId, InputAccountId, IsPriceInclusive.
- **AccountingSettings:** singleton posting links for purchase discount, sales discount, sales revenue and cost of sales.
- **PaymentMethods:** Name, AccountId, IsActive; maps cash, bank, card or similar settlement method to a chart account.
- **ProductStocks:** Id, ProductId, WarehouseId, Quantity decimal(18,3); unique product/warehouse.
- **ProductLocationStocks:** ProductId, WarehouseId, StorageLocationId, Quantity and RowVersion; unique product/location allocation.
- **LocationMovements:** immutable putaway/relocation audit rows with ProductId, WarehouseId, FromStorageLocationId?, ToStorageLocationId, Quantity, Type, Reference?, Notes?, CreatedByUserId and CreatedAt.
- **StockTransactions:** Id, ProductId, WarehouseId, Quantity decimal(18,3), Type, Reference?, CreatedAt.
- **Purchases:** header plus items containing ProductId, WarehouseId?, Quantity, PurchasePrice, DiscountAmount, TaxRateId? and Total; new invoices select warehouse per item.
- **Sales:** Id, InvoiceNumber, Channel, CreatedByUserId, CreatedAt, WarehouseId, PosTerminalId?, CustomerId?, PaymentMethodId?, Date, Notes?, PosOrderType?, ServiceReference?, GuestCount? and subtotal/discount/total fields; SaleItems may hold a preparation note. POS sales retain terminal and order context for audit while historical/legacy values may be null.
- **StockTransfers:** identity, transfer number, source/destination warehouse, status, actor/time/reason fields; each item may identify optional exact source and destination storage locations.
- **Permissions / RolePermissions:** permission catalogue and Identity role mapping.
- **AuditLogs:** EntityName, EntityId, Action, UserId, CreatedAt.
- **GeneralSettings / DiscountSettings / InventorySettings / InvoiceSettings / DocumentNumberSettings:** application settings; GeneralSettings stores the English/Arabic default UI language and InventorySettings stores rowversion-protected defaults for new warehouse operating policies.

See `03_DATABASE.md` for constraints and `entities/*.md` for behaviour.
