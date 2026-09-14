# Database tables reference
> Last reviewed: 2026-09-14

Schema source is `AppDbContext`, configurations and migrations. Identity's standard `AspNet*` tables are supplied by IdentityDbContext.

- **Products:** Id, Name, Barcode, PurchasePrice, SalePrice, WholesalePrice; all prices decimal(18,2).
- **Accounts / Branches:** chart account code/name/type/parent and branch code/name with optional SalesRevenueAccountId subaccount mapping.
- **JournalEntries / JournalEntryLines:** entry number/status/date/description, optional source type/reference protected against duplicate posting, and balanced account debit-credit lines with optional branch/warehouse.
- **Warehouses:** Id, Name, BranchId?, InventoryAccountId?, Type, ControlMode, PickingStrategy, AllowPosSales, EnforceLocationCapacity and source/destination transfer-location requirements.
- **BranchWarehouseAccesses:** composite BranchId/WarehouseId key, priority, branch POS default and POS/purchase/transfer/replenishment permissions.
- **PosTerminalWarehouses:** composite PosTerminalId/WarehouseId terminal allow-list with priority; each terminal retains DefaultWarehouseId.
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
- **Sales:** Id, InvoiceNumber, Channel, CreatedByUserId, CreatedAt, WarehouseId, PosTerminalId?, CustomerId?, PaymentMethodId?, Date, Notes?, subtotal/discount/total fields; POS sales retain their terminal for audit while historical/legacy sales may be null.
- **StockTransfers:** identity, transfer number, source/destination warehouse, status, actor/time/reason fields; each item may identify optional exact source and destination storage locations.
- **Permissions / RolePermissions:** permission catalogue and Identity role mapping.
- **AuditLogs:** EntityName, EntityId, Action, UserId, CreatedAt.
- **GeneralSettings / DiscountSettings / InventorySettings / InvoiceSettings / DocumentNumberSettings:** application settings; InventorySettings stores rowversion-protected defaults for new warehouse operating policies.

See `03_DATABASE.md` for constraints and `entities/*.md` for behaviour.
